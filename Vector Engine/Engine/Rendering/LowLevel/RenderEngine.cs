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
        private static double timeSinceStart;
        private static readonly Queue<Light> lights = new Queue<Light>();
        private static readonly Queue<Camera> cameras = new Queue<Camera>();

        private static readonly Dictionary<Material, Dictionary<Mesh, Queue<Matrix4>>> renderingQueue = new Dictionary<Material, Dictionary<Mesh, Queue<Matrix4>>>();

        private static Mesh vboQuad;
        private static FrameBufferRedrawShader frameBufferRedraw;

        /// <summary>
        /// Setup the renderer, defaults some OpenTK options
        /// </summary>
        /// <param name="shader"></param>
        public static void Setup()
        {
            vboQuad = RenderDataLoader.LoadMeshData2d(new Vector3d[]
            {
                new Vector3d(-1, 1, 0),
                new Vector3d(-1, -1, 0),
                new Vector3d(1, -1, 0),
                new Vector3d(1, 1, 0)
            }, new int[] {
                0, 1, 2,
                2, 3, 0
            }, new Vector2d[] {
                new Vector2d(0, 0),
                new Vector2d(0, 1),
                new Vector2d(1, 1),
                new Vector2d(1, 0)
            });

            frameBufferRedraw = new FrameBufferRedrawShader();

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            ChangeClearColor(0.25f, 0f, 0.5f, 1f);
        }

        /// <summary>
        /// Cleans up the renderer
        /// TODO: When implementing batching, clear all buffers
        /// </summary>
        public static void CleanUp()
        {
            renderingQueue.Clear();
        }

        public static void RenderPrepare()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
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
        /// Add a light to the next render
        /// </summary>
        /// <param name="light"></param>
        public static void AddLight(Light light)
        {
            lights.Enqueue(light);
        }

        /// <summary>
        /// Update the current time information
        /// </summary>
        /// <param name="timeSinceStart"></param>
        public static void SetTimeData(double timeSinceStart)
        {
            RenderEngine.timeSinceStart = timeSinceStart;
        }

        /// <summary>
        /// Add a camera to the next render
        /// </summary>
        /// <param name="cam"></param>
        public static void AddCamera(Camera cam)
        {
            cameras.Enqueue(cam);
        }

        /// <summary>
        /// Render all queued up objects
        /// </summary>
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
                // Initialize the camera and view port to prepare for rendering
                camera = cameraData[cameraIndex];
                int viewPortOffsetX = (int)(camera.ViewPortOffset.X * width);
                int viewPortOffsetY = (int)(camera.ViewPortOffset.Y * height);
                int viewPortWidth = (int)(camera.ViewPortSize.X * width);
                int viewPortHeight = (int)(camera.ViewPortSize.Y * height);
                GL.Viewport(viewPortOffsetX, viewPortOffsetY, viewPortWidth, viewPortHeight);

                // Create and bind a FBO
                int cameraFBO = RenderDataLoader.GenerateFrameBuffer();
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, cameraFBO);

                // Create and setup a texture to push our results to
                int cameraTexture = RenderDataLoader.GenerateTexture();
                GL.BindTexture(TextureTarget.Texture2D, cameraTexture);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, viewPortWidth, viewPortHeight, 0, PixelFormat.Rgb, PixelType.UnsignedByte, new System.IntPtr());

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, cameraTexture, 0);

                // Create, initialize, and attach depth and stencil buffer
                int cameraRBO = RenderDataLoader.GenerateRenderBuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, cameraRBO);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, viewPortWidth, viewPortHeight);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, cameraRBO);

                if(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                {
                    Debug.Log($"Frame buffer was not complete! BAIL! Error code: {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer).ToString()}");
                    return;
                }

                ChangeClearColor(0.1f, 0.1f, 0.1f, 1.0f);
                RenderPrepare();

                foreach (KeyValuePair<Material, Dictionary<Mesh, Queue<Matrix4>>> entry in renderingQueue)
                {
                    material = entry.Key;
                    objectsToRender = entry.Value;

                    material.Shader.EnableShader();

                    if (material.UsesLights)
                    {
                        material.SetLights(lightData);
                    }

                    if (material.UsesTime)
                    {
                        material.SetTimeData(timeSinceStart);
                    }

                    if (material.UsesViewMatrix)
                    {
                        material.SetMatrix("viewMatrix", Mathmatics.CreateViewMatrix(camera));
                    }

                    material.SetMatrix("projectionMatrix", camera.ProjectionMatrix);

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

                        material.Shader.AfterRenderInvividual();
                    }

                    material.Shader.AfterRenderGroup();

                    material.Shader.DisableShader();
                }

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                ChangeClearColor(1.0f, 1.0f, 1.0f, 1.0f);
                RenderPrepare();

                frameBufferRedraw.BeforeRenderGroup();

                GL.BindVertexArray(vboQuad.VaoID);

                frameBufferRedraw.BeforeRenderIndividual();
                GL.BindTexture(TextureTarget.Texture2D, cameraTexture);

                frameBufferRedraw.AfterRenderInvividual();

                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.DrawElements(BeginMode.Triangles, vboQuad.VertexCount, DrawElementsType.UnsignedInt, 0);

                frameBufferRedraw.AfterRenderGroup();

                RenderDataLoader.DeleteFrameBuffer(cameraFBO);
                RenderDataLoader.DeleteTexture(cameraTexture);
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            renderingQueue.Clear();

            GL.Viewport(0, 0, width, height);
        }

        /// <summary>
        /// Add a single instance of a mesh to the batch queue
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="mesh"></param>
        /// <param name="pos"></param>
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
                Dictionary<Mesh, Queue<Matrix4>> temp = new Dictionary<Mesh, Queue<Matrix4>>
                {
                    { mesh, new Queue<Matrix4>() }
                };
                temp[mesh].Enqueue(pos);
                renderingQueue.Add(mat, temp);
            }
        }

        /// <summary>
        /// Add many instances of a mesh to the batch queue
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="mesh"></param>
        /// <param name="positions"></param>
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
                Dictionary<Mesh, Queue<Matrix4>> temp = new Dictionary<Mesh, Queue<Matrix4>>
                {
                    { mesh, new Queue<Matrix4>(positions) }
                };
                renderingQueue.Add(mat, temp);
            }
        }
    }
}
