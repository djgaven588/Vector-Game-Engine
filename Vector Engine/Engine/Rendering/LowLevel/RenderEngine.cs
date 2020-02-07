using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using VectorEngine.Core.Common;
using VectorEngine.Core.Rendering.Objects;
using VectorEngine.Core.Rendering.Shaders;
using VectorEngine.Engine;
using VectorEngine.Engine.Rendering;

namespace VectorEngine.Core.Rendering.LowLevel
{
    public static class RenderEngine
    {
        private static Matrix4 projectionMatrix;
        private static StaticShader staticShader;
        private static double timeSinceStart;
        private static readonly Queue<Light> lights = new Queue<Light>();
        private static readonly Queue<Camera> cameras = new Queue<Camera>();

        private static Dictionary<Material, Dictionary<Mesh, Queue<Matrix4>>> renderingQueue = new Dictionary<Material, Dictionary<Mesh, Queue<Matrix4>>>();

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

        public static void TEST_Prepare()
        {
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            ChangeClearColor(0.25f, 0f, 0.5f, 1f);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
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
            Dictionary<Mesh, Queue<Matrix4>> objectsToRender;
            Mesh currentMesh;
            Queue<Matrix4> currentObjects;
            Matrix4 currentObject;

            Light[] lightData = lights.ToArray();
            lights.Clear();

            Camera[] cameraData = cameras.ToArray();
            cameras.Clear();
            (int width, int height) = GameEngine.windowHandler.GetWindowDimensions();
            for (int cameraIndex = 0; cameraIndex < cameraData.Length; cameraIndex++)
            {
                camera = cameraData[cameraIndex];
                GL.Viewport((int)(camera.ViewPortOffset.X * width), (int)(camera.ViewPortOffset.Y * height), (int)(camera.ViewPortSize.X * width), (int)(camera.ViewPortSize.Y * height));
                Debug.Log($"Rendering camera {cameraIndex}");
                foreach (KeyValuePair<Material, Dictionary<Mesh, Queue<Matrix4>>> entry in renderingQueue)
                {
                    material = entry.Key;
                    objectsToRender = entry.Value;

                    Debug.Log($"Shader {material.UsesLights}");
                    Debug.Log($"Different meshes to render {objectsToRender.Count}");

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

                    foreach (KeyValuePair<Mesh, Queue<Matrix4>> renderEntry in objectsToRender)
                    {
                        currentMesh = renderEntry.Key;
                        currentObjects = renderEntry.Value;

                        GL.BindVertexArray(currentMesh.VaoID);

                        material.Shader.BeforeRenderGroup();

                        while (currentObjects.Count > 0)
                        {
                            material.Shader.BeforeRenderIndividual();

                            currentObject = currentObjects.Dequeue();
                            material.SetMatrix("transformationMatrix", currentObject);

                            GL.DrawElements(BeginMode.Triangles, currentMesh.VertexCount, DrawElementsType.UnsignedInt, 0);
                        }

                        GL.BindVertexArray(0);

                        material.Shader.AfterRenderObject();
                    }

                    material.Shader.AfterRenderShader();

                    material.Shader.DisableShader();
                }
            }

            renderingQueue.Clear();

            GL.Viewport(0, 0, width, height);
        }

        public static void AddToRenderQueue(Material mat, Mesh mesh, Matrix4 pos)
        {
            if (renderingQueue.ContainsKey(mat))
            {
                if (renderingQueue[mat].ContainsKey(mesh))
                {
                    renderingQueue[mat][mesh].Enqueue(pos);
                }
                else
                {
                    Queue<Matrix4> temp = new Queue<Matrix4>();
                    temp.Enqueue(pos);
                    renderingQueue[mat].Add(mesh, temp);
                }
            }
            else
            {
                Dictionary<Mesh, Queue<Matrix4>> temp = new Dictionary<Mesh, Queue<Matrix4>>();
                temp.Add(mesh, new Queue<Matrix4>());
                temp[mesh].Enqueue(pos);
                renderingQueue.Add(mat, temp);
            }
        }

        public static void AddToRenderQueueInstanced(Material mat, Mesh mesh, Matrix4[] positions)
        {
            if (renderingQueue.ContainsKey(mat))
            {
                if (renderingQueue[mat].ContainsKey(mesh))
                {
                    for (int i = 0; i < positions.Length; i++)
                    {
                        renderingQueue[mat][mesh].Enqueue(positions[i]);
                    }
                }
                else
                {
                    Queue<Matrix4> temp = new Queue<Matrix4>(positions);
                    renderingQueue[mat].Add(mesh, temp);
                }
            }
            else
            {
                Dictionary<Mesh, Queue<Matrix4>> temp = new Dictionary<Mesh, Queue<Matrix4>>();
                temp.Add(mesh, new Queue<Matrix4>(positions));
                renderingQueue.Add(mat, temp);
            }
        }
    }
}
