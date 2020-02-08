using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Threading;
using VectorEngine.Core.Common;
using VectorEngine.Core.Rendering.LowLevel;
using VectorEngine.Core.Rendering.Objects;
using VectorEngine.Core.Rendering.Shaders;
using VectorEngine.Engine;
using VectorEngine.Engine.Common;
using VectorEngine.Engine.Input;
using VectorEngine.Engine.Rendering;

namespace VectorEngine.Core
{
    public class GameEngine
    {
        public static WindowHandler windowHandler;

        private readonly IStartEngine entryPoint;

        public GameEngine(string[] startParameters, IStartEngine entryPoint)
        {
            this.entryPoint = entryPoint;
            windowHandler = new WindowHandler(640, 480, "Vector Engine", this)
            {
                VSync = VSyncMode.Off
            };

            windowHandler.Run();

            while (windowHandler.Exists)
            {
                Thread.Sleep(50);
            }

            windowHandler.Dispose();
        }

        Light light;
        Light mainLight;
        Light anotherLight;

        Camera camera;
        Camera secondCamera;
        
        private Mesh treeMesh;
        private int treeTexture;
        private Material standardMaterial;

        private DateTime engineStartTime;

        public void OnLoad(EventArgs e)
        {
            engineStartTime = DateTime.UtcNow;
            entryPoint.OnLoad();
            light = new Light(new Vector3d(10000000, 10000000, 10000000), new Vector3d(0, 1, 0), 100000000, 3f);
            mainLight = new Light(new Vector3d(-10000000, 10000000, 10000000), new Vector3d(1, 0, 0), 100000000, 3f);
            anotherLight = new Light(new Vector3d(0, -10000000, 10000000), new Vector3d(0, 0, 1), 100000000, 3f);
            camera = new Camera(RenderDataLoader.GenerateFrameBuffer(), RenderDataLoader.GenerateTexture(), RenderDataLoader.GenerateRenderBuffer())
            {
                Position = new Vector3d(0, 0, 5),
                Rotation = new Vector3d(0, 0, 0),
                FarPlane = 1000f,
                NearPlane = 0.01f,
                FOV = 60,
                IsPerspective = true,
                ViewPortOffset = Vector2.Zero,
                ViewPortSize = Vector2.One,
                ClearColor = new Vector4(0.2f, 0.2f, 0.2f, 1f)
            };

            secondCamera = new Camera(RenderDataLoader.GenerateFrameBuffer(), RenderDataLoader.GenerateTexture(), RenderDataLoader.GenerateRenderBuffer())
            {
                Position = new Vector3d(0, 0, 0),
                Rotation = new Vector3d(0, 0, 0),
                FarPlane = 1000f,
                NearPlane = 0.01f,
                FOV = 60,
                IsPerspective = true,
                ViewPortOffset = Vector2.One * -0.5f,
                ViewPortSize = Vector2.One * 0.5f,
                ClearColor = new Vector4(0, 0, 0, 0)
            };

            treeMesh = OBJLoader.LoadObjModel("Tree");

            treeTexture = RenderDataLoader.LoadTexture("Tree");
            standardMaterial = new Material(new StaticShader(treeTexture), true, true, false);
            RenderEngine.Setup();
        }

        public void OnClosed(EventArgs e)
        {
            entryPoint.OnClose();
            RenderEngine.CleanUp();
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            RenderDataLoader.CleanUp();
        }

        public void OnResize(EventArgs e)
        {
            (int width, int height) = windowHandler.GetWindowDimensions();
            GL.Viewport(0, 0, width, height);
        }

        public void OnUpdateFrame(FrameEventArgs e)
        {
            InputManager.UpdateInput();
            entryPoint.OnUpdate(e.Time);
            //TODO:
            //Look at SpinWait which would replace Thread.Sleep(), which is known for being inconsistent.
            double xAxis = InputManager.IsKeyDown(Key.D) && !InputManager.IsKeyDown(Key.A) ? 1 : !InputManager.IsKeyDown(Key.D) && InputManager.IsKeyDown(Key.A) ? -1 : 0;
            double yAxis = InputManager.IsKeyDown(Key.Space) && !InputManager.IsKeyDown(Key.ShiftLeft) ? 1 : !InputManager.IsKeyDown(Key.Space) && InputManager.IsKeyDown(Key.ShiftLeft) ? -1 : 0;
            double zAxis = -(InputManager.IsKeyDown(Key.W) && !InputManager.IsKeyDown(Key.S) ? 1 : !InputManager.IsKeyDown(Key.W) && InputManager.IsKeyDown(Key.S) ? -1 : 0);

            double yRotation = InputManager.IsKeyDown(Key.E) && !InputManager.IsKeyDown(Key.Q) ? 1 : !InputManager.IsKeyDown(Key.E) && InputManager.IsKeyDown(Key.Q) ? -1 : 0;
            double xRotation = InputManager.IsKeyDown(Key.Z) && !InputManager.IsKeyDown(Key.X) ? 1 : !InputManager.IsKeyDown(Key.Z) && InputManager.IsKeyDown(Key.X) ? -1 : 0;

            camera.Rotation += new Vector3d(xRotation, yRotation, 0) * 90 * e.Time;

            camera.MoveDirectionBased(new Vector3d(xAxis, yAxis, zAxis) * 10 * e.Time);
        }

        public void OnRenderFrame(FrameEventArgs e)
        {
            double time = (DateTime.UtcNow - engineStartTime).TotalSeconds;
            secondCamera.Position = new Vector3d(Math.Sin(time) * 5, 3, Math.Cos(time) * 5);
            secondCamera.Rotation = new Vector3d(25, -Math.Atan2(Math.Sin(time), Math.Cos(time)) * 180 / Mathmatics.PI, 0);
            windowHandler.SetWindowTitle($"Vector Engine | VSync: { entryPoint.UseVSync } FPS: { ((int)(1 / e.Time * 10)) / 10f }");
            RenderEngine.CleanUp();
            RenderEngine.SetTimeData(time);
            RenderEngine.AddCamera(camera);
            RenderEngine.AddCamera(secondCamera);
            RenderEngine.AddLight(light);
            RenderEngine.AddLight(mainLight);
            RenderEngine.AddLight(anotherLight);
            RenderEngine.AddToRenderQueue(standardMaterial, treeMesh, Mathmatics.CreateTransformationMatrix(new Vector3d(0, 0, 0), Vector3d.Zero, Vector3d.One));
            RenderEngine.AddToRenderQueue(standardMaterial, treeMesh, Mathmatics.CreateTransformationMatrix(new Vector3d(0, 0, -5), Vector3d.Zero, Vector3d.One));

            entryPoint.OnRender(e.Time);
            RenderEngine.RenderAll();

            windowHandler.SwapBuffers();

            if (entryPoint.UseVSync && e.Time < 1000d / entryPoint.TargetFrameRate)
            {
                Thread.Sleep((int)Math.Round(1000 / entryPoint.TargetFrameRate - e.Time));
            }
            else if (!entryPoint.UseVSync && e.Time < 1000d / entryPoint.MaxFrameRate)
            {
                Thread.Sleep((int)Math.Round(1000 / entryPoint.MaxFrameRate - e.Time));
            }
        }
    }
}
