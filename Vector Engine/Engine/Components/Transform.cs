using OpenTK;
using Svelto.ECS;

namespace VectorEngine.Engine.Components
{
    public struct Transform : IEntityStruct
    {
        public Vector3d position;
        public Vector3d scale;
        public Quaterniond rotation;

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Transform))
            {
                Transform other = (Transform)obj;
                if (position == other.position && scale == other.scale && rotation == other.rotation)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool operator ==(Transform left, Transform right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Transform left, Transform right)
        {
            return !(left == right);
        }
    }
}
