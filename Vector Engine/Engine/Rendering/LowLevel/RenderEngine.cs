using OpenTK;
using OpenTK.Graphics.OpenGL4;
using VectorEngine.Core.Rendering.Shaders;

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

            staticShader = shader;

            ChangeClearColor(0.25f, 0f, 0.5f, 1f);
            PrepareForRendering();
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
        public static void PrepareForRendering()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            staticShader.EnableShader();
            staticShader.LoadProjectionMatrix(projectionMatrix);
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
        /// Render a mesh, ignoring the batching system
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="mesh"></param>
        /// <param name="textureId"></param>
        public static void RenderMesh(Matrix4 matrix, Mesh mesh, int textureId)
        {
            staticShader.LoadTransformationMatrix(matrix);

            GL.BindVertexArray(mesh.VaoID);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.DrawElements(BeginMode.Triangles, mesh.VertexCount, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Render multiple meshes in one call, particles will find this useful
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="mesh"></param>
        /// <param name="textureId"></param>
        public static void RenderMeshInstanced(Matrix4[] matrixes, Mesh mesh, int textureId)
        {
            GL.BindVertexArray(mesh.VaoID);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindTexture(TextureTarget.Texture2D, textureId);
            for (int i = 0; i < matrixes.Length; i++)
            {
                staticShader.LoadTransformationMatrix(matrixes[i]);

                GL.DrawElements(BeginMode.Triangles, mesh.VertexCount, DrawElementsType.UnsignedInt, 0);
            }

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }
    }
}
