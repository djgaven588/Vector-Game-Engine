using OpenTK;

namespace VectorEngine.Core.Rendering.Objects
{
    public class Light
    {
        public Vector3d Position;
        public Vector3d Color;

        public Light(Vector3d pos, Vector3d col)
        {
            Position = pos;
            Color = col;
        }
    }

}
