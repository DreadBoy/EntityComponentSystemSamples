﻿using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// JobComponentSystems can run on worker threads.
// However, creating and removing Entities can only be done on the main thread to prevent race conditions.
// The system uses an EntityCommandBuffer to defer tasks that can't be done inside the Job.

// ReSharper disable once InconsistentNaming
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class SpawnerSystem_FromFile : JobComponentSystem
{
    // BeginInitializationEntityCommandBufferSystem is used to create a command buffer which will then be played back
    // when that barrier system executes.
    // Though the instantiation command is recorded in the SpawnJob, it's not actually processed (or "played back")
    // until the corresponding EntityCommandBufferSystem is updated. To ensure that the transform system has a chance
    // to run on the newly-spawned entities before they're rendered for the first time, the SpawnerSystem_FromEntity
    // will use the BeginSimulationEntityCommandBufferSystem to play back its commands. This introduces a one-frame lag
    // between recording the commands and instantiating the entities, but in practice this is usually not noticeable.
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    struct SpawnJob : IJobForEachWithEntity<MachineSpawner, LocalToWorld>
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;
        [ReadOnly] public BufferFromEntity<EntityBuffer> prefabBuffer;
        [ReadOnly] public BufferFromEntity<MachineBuffer> machineBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref MachineSpawner machineSpawner,
            [ReadOnly] ref LocalToWorld location)
        {
            for (int i = 0; i < machineBuffer[entity].Length; i++)
            {
                var machine = machineBuffer[entity][i].Value;
                var prefab = prefabBuffer[entity][machine.Type].Value;
                var instance = CommandBuffer.Instantiate(index, prefab);
                var position = math.transform(location.Value, machine.Position);
                CommandBuffer.SetComponent(index, instance, new Translation {Value = position});
                CommandBuffer.AddComponent(index, instance, new MachineComponentData {Type = machine.Type});
            }

            CommandBuffer.DestroyEntity(index, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //Instead of performing structural changes directly, a Job can add a command to an EntityCommandBuffer to perform such changes on the main thread after the Job has finished.
        //Command buffers allow you to perform any, potentially costly, calculations on a worker thread, while queuing up the actual insertions and deletions for later.

        // Schedule the job that will add Instantiate commands to the EntityCommandBuffer.
        var job = new SpawnJob
        {
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            prefabBuffer = GetBufferFromEntity<EntityBuffer>(),
            machineBuffer = GetBufferFromEntity<MachineBuffer>()
        }.Schedule(this, inputDeps);


        // SpawnJob runs in parallel with no sync point until the barrier system executes.
        // When the barrier system executes we want to complete the SpawnJob and then play back the commands (Creating the entities and placing them).
        // We need to tell the barrier system which job it needs to complete before it can play back the commands.
        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}