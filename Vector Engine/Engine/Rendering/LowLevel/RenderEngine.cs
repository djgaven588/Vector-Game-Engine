using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using VectorEngine.Core.Common;
using VectorEngine.Core.Rendering.Objects;
using VectorEngine.Core.Rendering.Shaders;
using VectorEngine.Engine.Rendering;

namespace VectorEngine.Core.Rendering.LowLevel
{
    public static class RenderEngine
    {
        private static Matrix4 projectionMatrix;
        private static StaticShader staticShader;
        private static double timeSinceStart;
        private static Queue<Light> lights = new Queue<Light>();
        private static Queue<Camera> cameras = new Queue<Camera>();

        private static Queue<(Material, Queue<(Mesh, Queue<Matrix4>)>)> renderingQueue = new Queue<(Material, Queue<(Mesh, Queue<Matrix4>)>)>();

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
            renderingQueue.Clear();
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
        public static void RenderMeshNow(Matrix4 matrix, Mesh mesh, int textureId)
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
        public static void RenderMeshNowInstanced(Matrix4[] matrixes, Mesh mesh, int textureId)
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

        public static void AddLight(Light light)
        {
            lights.Enqueue(light);
        }

        public static void SetTimeData(double timeSinceStart)
        {
            RenderEngine.timeSinceStart = timeSinceStart;
        }

        public static void AddCamera(Camera cam)
        {
            cameras.Enqueue(cam);
        }

        public static void RenderAll()
        {
            Camera camera;
            Material material;
            Queue<(Mesh, Queue<Matrix4>)> objectsToRender;
            Mesh currentMesh;
            Queue<Matrix4> currentObjects;
            Matrix4 currentObject;

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            Light[] lightData = lights.ToArray();
            lights.Clear();

            Camera[] cameraData = cameras.ToArray();
            cameras.Clear();
            (int width, int height) = GameEngine.windowHandler.GetWindowDimensions();
            for (int cameraIndex = 0; cameraIndex < cameraData.Length; cameraIndex++)
            {
                camera = cameraData[cameraIndex];
                GL.Viewport((int)(camera.ViewPortOffset.X * width), (int)(camera.ViewPortOffset.Y * height), (int)(camera.ViewPortSize.X * width), (int)(camera.ViewPortSize.Y * height));

                while (renderingQueue.Count > 0)
                {
                    (material, objectsToRender) = renderingQueue.Dequeue();

                    material.Shader.EnableShader();

                    if (material.UsesLights)
                    {
                        material.SetLights(lightData);
                    }

                    if (material.UsesLights)
                    {
                        material.SetTimeData(timeSinceStart);
                    }

                    material.SetMatrix("projectionMatrix", camera.ProjectionMatrix);

                    if (material.UsesViewMatrix)
                    {
                        material.SetMatrix("viewMatrix", Mathmatics.CreateViewMatrix(camera));
                    }

                    material.Shader.BeforeRenderShader();

                    while (objectsToRender.Count > 0)
                    {
                        (currentMesh, currentObjects) = objectsToRender.Dequeue();
                        GL.BindVertexArray(currentMesh.VaoID);

                        material.Shader.BeforeRenderObject();

                        while (currentObjects.Count > 0)
                        {
                            currentObject = currentObjects.Dequeue();
                            material.SetMatrix("transformationMatrix", currentObject);

                            GL.DrawElements(BeginMode.Triangles, currentMesh.VertexCount, DrawElementsType.UnsignedInt, 0);
                        }

                        material.Shader.AfterRenderObject();
                    }

                    material.Shader.AfterRenderShader();
                }
            }

            GL.Viewport(0, 0, width, height);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }

        public static void AddToRenderQueue(Material mat, Mesh mesh, Matrix4 pos)
        {
            // TODO : Add ability to add to the queue
            // Garbage to force error
            addToQueue
        }

        public static void AddToRenderQueueInstanced(Material matrix, Mesh mesh, Matrix4[] positions)
        {
            // TODO : Add ability to add multiple positions to the queue at once.
            // Garbage to force error
            addToQueue
        }
    }
}
