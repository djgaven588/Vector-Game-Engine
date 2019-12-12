using VectorEngine.Core.Rendering.Objects;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Core.Common
{
    public static class MathLib
    {
        public static Matrix4 createTransformationMatrix(Vector3d translation, Vector3d rotation, Vector3d scale)
        {
            Matrix4 matrix = Matrix4.Identity;

            matrix *= Matrix4.CreateTranslation((float)translation.X, (float)translation.Y, (float)translation.Z);
            matrix *= Matrix4.CreateRotationX((float)ConvertToRadians(rotation.X));
            matrix *= Matrix4.CreateRotationY((float)ConvertToRadians(rotation.Y));
            matrix *= Matrix4.CreateRotationZ((float)ConvertToRadians(rotation.Z));
            matrix *= Matrix4.CreateScale((float)scale.X, (float)scale.Y, (float)scale.Z);
            return matrix;
        }

        public static Matrix4 createViewMatrix(Camera camera)
        {
            Matrix4 matrix = Matrix4.Identity;

            Vector3 negativeCameraPos = (Vector3)(-camera.GetPosition());
            matrix *= Matrix4.CreateTranslation(negativeCameraPos);
            matrix *= Matrix4.CreateRotationY((float)ConvertToRadians(camera.GetRotation().Y));
            matrix *= Matrix4.CreateRotationX((float)ConvertToRadians(camera.GetRotation().X));
            matrix *= Matrix4.CreateRotationZ((float)ConvertToRadians(camera.GetRotation().Z));
            return matrix;
        }
        /// <summary>
        /// Converts degrees into radians
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double ConvertToRadians(double degrees) {
            return (Math.PI / 180) * degrees;
        }
    }
}
