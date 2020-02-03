using OpenTK;
using VectorEngine.Core.Common;

namespace VectorEngine.Core.Rendering.Objects
{
    public class Camera
    {
        public Vector3d Position = new Vector3d(0, 0, 0);
        public Vector3d Rotation = new Vector3d(0, 0, 0);
        public Matrix4 ProjectionMatrix { get { RecreateProjectionMatrix(); return projectionMatrix; } }
        public float FOV = 60f;
        public float NearPlane = 0.01f;
        public float FarPlane = 1000f;
        public bool IsPerspective = true;
        public Vector2 ViewPortSize = new Vector2(1, 1);
        public Vector2 ViewPortOffset = new Vector2(0, 0);

        private Matrix4 projectionMatrix;

        private void RecreateProjectionMatrix()
        {
            (int width, int height) = GameEngine.windowHandler.GetWindowDimensions();
            float aspectRatio = (width * ViewPortSize.X) / (height * ViewPortSize.Y);
            projectionMatrix = IsPerspective ? Matrix4.CreatePerspectiveFieldOfView((float)Mathmatics.ConvertToRadians(FOV), aspectRatio, NearPlane, FarPlane) : Matrix4.CreateOrthographic((int)(width * ViewPortSize.X), (int)(height * ViewPortSize.Y), NearPlane, FarPlane);
        }

        public void MoveDirectionBased(Vector3d movement)
        {
            Vector3d toAdd = (Vector3d)(Quaternion.FromEulerAngles(0, (float)Mathmatics.ConvertToRadians(-Rotation.Y), 0) * (Vector3)movement);
            Position += toAdd;
        }
    }
}
