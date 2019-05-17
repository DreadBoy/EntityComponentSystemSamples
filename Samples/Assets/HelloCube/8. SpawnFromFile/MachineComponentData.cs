using Unity.Entities;
using Unity.Mathematics;

public struct MachineComponentData : IComponentData
{
    public int Type;
    public float3 Position;
}