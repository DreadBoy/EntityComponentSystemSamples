using System.Collections.Generic;
using Factorio;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class SpawnerAuthoring_FromFile : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject[] Prefabs;

    public DataFromFile dataFromFile;

    public void Awake()
    {
        dataFromFile.Machines = new[]
        {
            new Machine(0, new float3(0, 0, 0)),
            new Machine(1, new float3(1, 0, 0)),
        };
    }

    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
    {
        gameObjects.AddRange(Prefabs);
    }

    // Lets you convert the editor data representation to the entity optimal runtime representation

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<EntityBuffer>(entity);
        var prefabBuffer = dstManager.GetBuffer<EntityBuffer>(entity);
        foreach (var prefab in Prefabs)
            prefabBuffer.Add(conversionSystem.GetPrimaryEntity(prefab));

        dstManager.AddBuffer<MachineBuffer>(entity);
        var machineBuffer = dstManager.GetBuffer<MachineBuffer>(entity);
        foreach (var machine in dataFromFile.Machines)
            machineBuffer.Add(new MachineComponentData
            {
                Type = machine.Type,
                Position = machine.Position
            });


        var spawnerData = new MachineSpawner();
        dstManager.AddComponentData(entity, spawnerData);
    }
}