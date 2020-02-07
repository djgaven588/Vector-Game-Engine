using OpenTK;
using OpenTK.Graphics.OpenGL4;
using VectorEngine.Core.Common;
using VectorEngine.Core.Rendering.Objects;

namespace VectorEngine.Core.Rendering.Shaders
{
    public class StaticShader : ShaderProgram
    {
        private const string VERTEX_FILE = @"Engine\Rendering\Shaders\VertexShader.txt";
        private const string FRAGMENT_FILE = @"Engine\Rendering\Shaders\FragmentShader.txt";

        private int location_transformationMatrix;
        private int location_projectionMatrix;
        private int location_viewMatrix;
        private int location_lightPosition;
        private int location_lightColor;
        private int textureId;

        public StaticShader(int textureId) : base(VERTEX_FILE, FRAGMENT_FILE)
        {
            this.textureId = textureId;
        }

        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
            BindAttribute(1, "textureCoords");
            BindAttribute(2, "normal");
        }

        protected override void GetAllUniformLocations()
        {
            location_transformationMatrix = GetUniformLocation("transformationMatrix");
            location_projectionMatrix = GetUniformLocation("projectionMatrix");
            location_viewMatrix = GetUniformLocation("viewMatrix");
            location_lightPosition = GetUniformLocation("lightPosition");
            location_lightColor = GetUniformLocation("lightColor");
        }

        public void LoadTransformationMatrix(Matrix4 matrix)
        {
            LoadMatrix4(location_transformationMatrix, matrix);
        }

        public void LoadProjectionMatrix(Matrix4 projection)
        {
            LoadMatrix4(location_projectionMatrix, projection);
        }

        public void LoadViewMatrix(Camera camera)
        {
            LoadMatrix4(location_viewMatrix, Mathmatics.CreateViewMatrix(camera));
        }

        public void LoadLight(Light light)
        {
            LoadVector(location_lightPosition, (Vector3)light.Position);
            LoadVector(location_lightColor, (Vector3)light.Color);
        }

        public override void BeforeRenderGroup()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
        }

        public override void BeforeRenderIndividual()
        {
            
        }

        public override void AfterRenderObject()
        {

        }

        public override void AfterRenderShader()
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
        }
    }
}
