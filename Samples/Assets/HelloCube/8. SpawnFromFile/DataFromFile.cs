using Unity.Mathematics;

namespace Factorio
{
    public struct DataFromFile
    {
        public Machine[] Machines;
    }

    public struct Machine
    {
        public Machine(int Type, float3 Position)
        {
            this.Type = Type;
            this.Position = Position;
        }

        public int Type;
        public float3 Position;
    }
}