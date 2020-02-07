using OpenTK;

namespace VectorEngine.Core.Rendering.Objects
{
    public class Light
    {
        public Vector3d Position;
        public Vector3d Color;
        public float Distance;
        public float Intensity;

        public Light(Vector3d pos, Vector3d col, float dist, float intens)
        {
            Position = pos;
            Color = col;
            Distance = dist;
            Intensity = intens;
        }
    }

}
