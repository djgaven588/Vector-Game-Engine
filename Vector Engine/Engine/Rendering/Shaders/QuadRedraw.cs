using OpenTK.Graphics.OpenGL4;

namespace VectorEngine.Core.Rendering.Shaders
{
    public class QuadRedraw : ShaderProgram
    {
        private const string VERTEX_FILE = @"Engine\Rendering\Shaders\QuadRedrawVertex.txt";
        private const string FRAGMENT_FILE = @"Engine\Rendering\Shaders\QuadRedrawFragment.txt";

        public QuadRedraw() : base(VERTEX_FILE, FRAGMENT_FILE)
        {

        }

        public override void AfterRenderInvividual() { }

        public override void AfterRenderGroup() 
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
        }

        public override void BeforeRenderGroup()
        {
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
