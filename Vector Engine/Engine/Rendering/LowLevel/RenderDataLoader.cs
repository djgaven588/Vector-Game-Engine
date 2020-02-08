using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace VectorEngine.Core.Rendering.LowLevel
{
    public static class RenderDataLoader
    {
        private static readonly List<int> vaos = new List<int>();
        private static readonly List<int> vbos = new List<int>();
        private static readonly List<int> textures = new List<int>();
        private static readonly List<int> fbos = new List<int>();
        private static readonly List<int> rbos = new List<int>();

        /// <summary>
        /// Loads a mesh, optionally takes a mesh ID which you want to override
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="indices"></param>
        /// <param name="textureCoords"></param>
        /// <param name="normals"></param>
        /// <param name="meshID"></param>
        /// <returns></returns>
        public static Mesh LoadMeshData(Vector3d[] positions, int[] indices, Vector2d[] textureCoords, Vector3d[] normals, int meshID = 0)
        {
            int eboID = BindIndices(indices);
            int vboPos = BindVector3Array(positions);
            int vboTex = BindVector2Array(textureCoords);
            int vboNor = BindVector3Array(normals);
            meshID = BindToVAO(meshID, eboID, vboPos, vboTex, vboNor);

            return new Mesh(meshID, indices.Length);
        }

        /// <summary>
        /// Loads a mesh, specifically meant for things like UI and sprites, optionally takes a mesh ID which you want to override
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="indices"></param>
        /// <param name="textureCoords"></param>
        /// <param name="normals"></param>
        /// <param name="meshID"></param>
        /// <returns></returns>
        public static Mesh LoadMeshData2d(Vector3d[] positions, int[] indices, Vector2d[] textureCoords, int meshID = 0)
        {
            int eboID = BindIndices(indices);
            int vboPos = BindVector3Array(positions);
            int vboTex = BindVector2Array(textureCoords);
            meshID = BindToVAO2D(meshID, eboID, vboPos, vboTex);

            return new Mesh(meshID, indices.Length);
        }

        /// <summary>
        /// Loads a file from the specified path into OpenTK so it can be used
        /// </summary>
        /// <param name="filepath">The file path to the texture, starts at 'Game/Textures/' and the extension is preset to PNG</param>
        /// <returns></returns>
        public static int LoadTexture(string filepath)
        {
            try
            {
                Bitmap bitmap = new Bitmap("Game/Textures/" + filepath + ".png");

                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                int textureID = GenerateTexture();
                textures.Add(textureID);
                GL.BindTexture(TextureTarget.Texture2D, textureID);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                bitmap.UnlockBits(data);

                GL.BindTexture(TextureTarget.Texture2D, 0);

                bitmap.Dispose();

                return textureID;
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public static int GenerateTexture()
        {
            int textureID = GL.GenTexture();
            textures.Add(textureID);

            return textureID;
        }

        /// <summary>
        /// Clears VAOS, VBOS, and textures. Should be called before application close.
        /// </summary>
        public static void CleanUp()
        {
            for (int i = 0; i < vaos.Count; i++)
            {
                GL.DeleteVertexArray(vaos[i]);
            }

            for (int i = 0; i < vbos.Count; i++)
            {
                GL.DeleteBuffer(vbos[i]);
            }

            for (int i = 0; i < textures.Count; i++)
            {
                DeleteTexture(textures[i]);
            }

            for (int i = 0; i < rbos.Count; i++)
            {
                DeleteRenderBuffer(rbos[i]);
            }

            for (int i = 0; i < fbos.Count; i++)
            {
                DeleteFrameBuffer(fbos[i]);
            }
        }

        /// <summary>
        /// Creates a new FBO, frame buffer object, and returns the ID
        /// </summary>
        /// <returns></returns>
        public static int GenerateFrameBuffer()
        {
            int generatedFbo = GL.GenFramebuffer();
            fbos.Add(generatedFbo);
            return generatedFbo;
        }

        /// <summary>
        /// Deletes the provided FBO, frame buffer object, for use of cleaning up
        /// </summary>
        /// <param name="fbo">The FBO to delete</param>
        public static void DeleteFrameBuffer(int fbo)
        {
            try
            {
                GL.DeleteFramebuffer(fbo);
            }
            finally
            {
                fbos.Remove(fbo);
            }
        }

        /// <summary>
        /// Delete the provided texture
        /// </summary>
        /// <param name="texture"></param>
        public static void DeleteTexture(int texture)
        {
            GL.DeleteTexture(texture);
            textures.Remove(texture);
        }
        
        /// <summary>
        /// Creates an RBO (render buffer object)
        /// </summary>
        /// <returns></returns>
        public static int GenerateRenderBuffer()
        {
            int rbo = GL.GenRenderbuffer();
            rbos.Add(rbo);
            return rbo;
        }

        /// <summary>
        /// Deletes the provided RBO (render buffer object)
        /// </summary>
        /// <param name="rbo"></param>
        public static void DeleteRenderBuffer(int rbo)
        {
            GL.DeleteRenderbuffer(rbo);
            rbos.Remove(rbo);
        }

        private static int BindVector3Array(Vector3d[] vec)
        {
            GL.GenBuffers(1, out int vboID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vec.Length * Vector3d.SizeInBytes), vec, BufferUsageHint.StaticDraw);
            return vboID;
        }

        private static int BindVector2Array(Vector2d[] vec)
        {
            GL.GenBuffers(1, out int vboID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vec.Length * Vector2d.SizeInBytes), vec, BufferUsageHint.StaticDraw);
            return vboID;
        }

        private static int BindIndices(int[] ind)
        {
            GL.GenBuffers(1, out int eboID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(uint) * ind.Length), ind, BufferUsageHint.StaticDraw);
            return eboID;
        }

        private static int BindToVAO(int vaoID, int indID, int posVBO, int texVBO, int normVBO)
        {
            if (vaoID == 0)
            {
                GenerateVAO(out vaoID);
            }
            else
            {
                LoadVAO(vaoID);
            }

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, posVBO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Double, false, Vector3d.SizeInBytes, 0);
            GL.DisableVertexAttribArray(0);

            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, texVBO);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Double, false, Vector2d.SizeInBytes, 0);
            GL.DisableVertexAttribArray(1);

            GL.EnableVertexAttribArray(2);
            GL.BindBuffer(BufferTarget.ArrayBuffer, normVBO);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Double, false, Vector3d.SizeInBytes, 0);
            GL.DisableVertexAttribArray(2);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indID);

            UnloadVAO();

            return vaoID;
        }

        private static int BindToVAO2D(int vaoID, int indID, int posVBO, int texVBO)
        {
            if (vaoID == 0)
            {
                GenerateVAO(out vaoID);
            }
            else
            {
                LoadVAO(vaoID);
            }

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, posVBO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Double, false, Vector3d.SizeInBytes, 0);
            GL.DisableVertexAttribArray(0);

            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, texVBO);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Double, false, Vector2d.SizeInBytes, 0);
            GL.DisableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indID);

            UnloadVAO();

            return vaoID;
        }

        private static void GenerateVAO(out int vaoID)
        {
            GL.GenVertexArrays(1, out vaoID);
            vaos.Add(vaoID);
            GL.BindVertexArray(vaoID);
        }

        private static void LoadVAO(int vaoID)
        {
            GL.BindVertexArray(vaoID);
        }

        private static void UnloadVAO()
        {
            GL.BindVertexArray(0);
        }
    }
}
