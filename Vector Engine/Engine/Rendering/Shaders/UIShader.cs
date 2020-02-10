using OpenTK.Graphics.OpenGL4;

namespace VectorEngine.Core.Rendering.Shaders
{
    public class UIShader : ShaderProgram
    {
        private const string VERTEX_FILE = @"Engine\Rendering\Shaders\UIVertex.txt";
        private const string FRAGMENT_FILE = @"Engine\Rendering\Shaders\UIFragment.txt";

        private int textureId;

        public UIShader(int textureId) : base(VERTEX_FILE, FRAGMENT_FILE)
        {
            this.textureId = textureId;
        }

        public override void AfterRenderInvividual() { }

        public override void AfterRenderGroup()
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
        }

        public override void BeforeRenderGroup()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
        }

        public override void BeforeRenderIndividual() { }

        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
            BindAttribute(1, "textureCoords");
        }
    }
}
