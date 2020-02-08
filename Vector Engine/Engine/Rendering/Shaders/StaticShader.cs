using OpenTK.Graphics.OpenGL4;

namespace VectorEngine.Core.Rendering.Shaders
{
    public class StaticShader : ShaderProgram
    {
        private const string VERTEX_FILE = @"Engine\Rendering\Shaders\VertexShader.txt";
        private const string FRAGMENT_FILE = @"Engine\Rendering\Shaders\FragmentShader.txt";

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

        public override void BeforeRenderGroup()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
        }

        public override void BeforeRenderIndividual() { }

        public override void AfterRenderInvividual() { }

        public override void AfterRenderGroup()
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
        }
    }
}
