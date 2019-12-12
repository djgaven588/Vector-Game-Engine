using VectorEngine.Core.Rendering.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace VectorEngine.Core.Rendering.LowLevel
{
    public static class RenderEngine
    {
        private static Matrix4 projectionMatrix;
        private static StaticShader staticShader;

        /// <summary>
        /// Setup the renderer, defaults some OpenTK options
        /// </summary>
        /// <param name="shader"></param>
        public static void Setup(StaticShader shader)
        {
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            ChangeClearColor(0.25f, 0f, 0.5f, 1f);

            staticShader = shader;
        }

        /// <summary>
        /// Cleans up the renderer
        /// TODO: When implementing batching, clear all buffers
        /// </summary>
        public static void CleanUp()
        {

        }

        /// <summary>
        /// Change the projection matrix used by the current renderer
        /// </summary>
        /// <param name="projectMatrix"></param>
        public static void SetProjectionMatrix(Matrix4 projectMatrix)
        {
            projectionMatrix = projectMatrix;
        }

        /// <summary>
        /// Clear the screen using the clear color
        /// </summary>
        public static void ClearScreen()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            staticShader.EnableShader();
            staticShader.LoadProjectionMatrix(projectionMatrix);

            ShaderProgram.DisableShader();
        }

        /// <summary>
        /// Update the clear color
        /// </summary>
        /// <param name="red">Red channel</param>
        /// <param name="green">Green channel</param>
        /// <param name="blue">Blue channel</param>
        /// <param name="alpha">Alpha channel</param>
        public static void ChangeClearColor(float red, float green, float blue, float alpha)
        {
            GL.ClearColor(red, green, blue, alpha);
        }

        /// <summary>
        /// Render an entity right now.
        /// TODO: Add a method to do batched rendering
        /// </summary>
        /// <param name="entity"></param>
        /// 
        /*
        public static void Render(Entity entity)
        {
            Mesh mesh = entity.getMesh();

            entity.UpdateMatrix();
            staticShader.LoadTransformationMatrix(entity.GetMatrix());

            GL.BindVertexArray(mesh.GetVAOID());

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.DrawElements(BeginMode.Triangles, mesh.GetVertexCount(), DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }*/
    }
}
