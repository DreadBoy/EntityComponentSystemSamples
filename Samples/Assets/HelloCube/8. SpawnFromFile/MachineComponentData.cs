using System;
using Unity.Entities;

[Serializable]
public struct MachineComponentData : IComponentData
{
    public int Type;
}