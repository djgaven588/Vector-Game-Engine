using OpenTK;

namespace VectorEngine.Core.Rendering.Objects
{
    public class Light
    {
        private Vector3d position;
        private Vector3d color;

        public Light(Vector3d pos, Vector3d col)
        {
            position = pos;
            color = col;
        }

        public Vector3d getPosition()
        {
            return position;
        }

        public Vector3d getColor()
        {
            return color;
        }

        public void SetPosition(Vector3d pos)
        {
            position = pos;
        }

        public void SetColor(Vector3d col)
        {
            color = col;
        }
    }

}
