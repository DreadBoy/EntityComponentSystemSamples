using Unity.Entities;
using Unity.Mathematics;

public interface IPositionData : IComponentData
{
    float3 position { get; set; }
}
