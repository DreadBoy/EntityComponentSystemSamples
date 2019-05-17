using Factorio;
using Unity.Entities;

[InternalBufferCapacity(8)]
public struct MachineBuffer: IBufferElementData
{
    public static implicit operator Machine(MachineBuffer e) { return e.Value; }
    public static implicit operator MachineBuffer(Machine e) { return new MachineBuffer { Value = e }; }
    public Machine Value;
}