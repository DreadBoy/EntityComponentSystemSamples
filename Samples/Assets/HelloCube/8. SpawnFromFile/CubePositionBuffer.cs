using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(8)]
public struct MachineBuffer: IBufferElementData
{
    public static implicit operator MachineComponentData(MachineBuffer e) { return e.Value; }
    public static implicit operator MachineBuffer(MachineComponentData e) { return new MachineBuffer { Value = e }; }
    public MachineComponentData Value;
}