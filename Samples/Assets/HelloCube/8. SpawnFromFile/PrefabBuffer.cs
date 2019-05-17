using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(8)]
public struct EntityBuffer: IBufferElementData
{
    public static implicit operator Entity(EntityBuffer e) { return e.Value; }
    public static implicit operator EntityBuffer(Entity e) { return new EntityBuffer { Value = e }; }
    public Entity Value;
}