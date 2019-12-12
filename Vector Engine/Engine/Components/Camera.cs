using VectorEngine.Core.Common;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Core.Rendering.Objects
{
    public class Camera
    {
        private Vector3d position = new Vector3d(0, 0, 0);
        private Vector3d rotation = new Vector3d(0, 0, 0);

        public Camera() { }

        public void Move(Vector3d pos)
        {
            position += pos;
        }

        public void MoveDirectionBased(Vector3d movement)
        {
            Vector3d toAdd = (Vector3d)(Quaternion.FromEulerAngles(0, (float)MathLib.ConvertToRadians(rotation.Y), 0) * (Vector3)movement);
            toAdd.X = -toAdd.X;
            position += toAdd;
        }

        public void Rotate(Vector3d rot)
        {
            rotation += rot;
        }

        public void SetPosition(Vector3d pos)
        {
            position = pos;
        }

        public void SetRotation(Vector3d rot)
        {
            rotation = rot;
        }

        public Vector3d GetPosition()
        {
            return position;
        }

        public Vector3d GetRotation()
        {
            return rotation;
        }
    }
}
