using OpenTK;
using Svelto.ECS;

namespace VectorEngine.Engine.Components
{
    public struct Transform : IEntityStruct
    {
        public Vector3d position;
        public Vector3d scale;
        public Quaterniond rotation;
    }
}
