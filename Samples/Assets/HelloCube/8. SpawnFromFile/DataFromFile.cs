using System;
using Unity.Mathematics;

namespace Factorio
{
    [Serializable]
    public struct DataFromFile
    {
        public Machine[] Machines;
    }

    [Serializable]
    public struct Machine
    {
        public int Type;
        public float3 Position;

        public Machine(int Type, float3 Position)
        {
            this.Type = Type;
            this.Position = Position;
        }
    }
}