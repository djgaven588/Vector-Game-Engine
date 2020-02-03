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

namespace VectorEngine.Core
{
    public class GameEngine
    {
        private const float FOV = 60.0f;
        private const float NEAR_PLANE = 0.1f;
        private const float FAR_PLANE = 1000f;

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

        StaticShader staticShader;
        Light light;
        Camera camera;
        public static Mesh treeMesh;
        public static Mesh testMesh;
        public static int treeTexture;

        public void OnLoad(EventArgs e)
        {
            entryPoint.OnLoad();
            light = new Light(new Vector3d(-500, 500, 500), new Vector3d(1, 1, 1));
            camera = new Camera
            {
                Position = new Vector3d(0, 0, 0),
                Rotation = new Vector3d(0, 0, 0)
            };

            treeMesh = OBJLoader.LoadObjModel("Tree");

            testMesh = RenderDataLoader.LoadMeshData(new Vector3d[] {
                new Vector3d(-0.5, 0.5, -2),
                new Vector3d(-0.5, -0.5, -2),
                new Vector3d(0.5, -0.5, -2),
                new Vector3d(0.5, 0.5, -2)
            },
            new int[] {
                0, 1, 2,
                2, 3, 0
            },
            new Vector2d[] {
                new Vector2d(0, 0),
                new Vector2d(0, 1),
                new Vector2d(1, 1),
                new Vector2d(1, 0)
            },
                new Vector3d[] {
                new Vector3d(0, 0, 1),
                new Vector3d(0, 0, 1),
                new Vector3d(0, 0, 1),
                new Vector3d(0, 0, 1)
             });

            treeTexture = RenderDataLoader.LoadTexture("Tree");

            staticShader = new StaticShader(treeTexture);
            RenderEngine.Setup(staticShader);

            RenderEngine.SetProjectionMatrix(CreateProjectionMatrix());
        }

        public void OnClosed(EventArgs e)
        {
            entryPoint.OnClose();
            staticShader.CleanUp();
            RenderEngine.CleanUp();
            RenderDataLoader.CleanUp();
        }

        public void OnResize(EventArgs e)
        {
            RenderEngine.SetProjectionMatrix(CreateProjectionMatrix());
            (int width, int height) = windowHandler.GetWindowDimensions();
            GL.Viewport(0, 0, width, height);
        }

        private Matrix4 CreateProjectionMatrix()
        {
            (int width, int height) = windowHandler.GetWindowDimensions();
            float aspectRatio = (float)width / height;
            return Matrix4.CreatePerspectiveFieldOfView((float)Mathmatics.ConvertToRadians(FOV), aspectRatio, NEAR_PLANE, FAR_PLANE);
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
            windowHandler.SetWindowTitle($"Vector Engine | VSync: { EntryPoint.VSyncEnabled } FPS: { ((int)(1 / e.Time * 10)) / 10f }");

            RenderEngine.CleanUp();
            light.Position = camera.Position;

            RenderEngine.PrepareForRendering();
            staticShader.LoadViewMatrix(camera);
            staticShader.LoadLight(light);

            entryPoint.OnRender(e.Time);
            // Run render code here
            RenderEngine.RenderMeshNow(Mathmatics.CreateTransformationMatrix(new Vector3d(0, 0, -2), Vector3d.Zero, Vector3d.One), testMesh, treeTexture);
            RenderEngine.RenderMeshNow(Mathmatics.CreateTransformationMatrix(new Vector3d(5, 0, -5), Vector3d.Zero, Vector3d.One), treeMesh, treeTexture);

            staticShader.DisableShader();

            windowHandler.SwapBuffers();

            if (EntryPoint.VSyncEnabled && e.Time < 1000d / EntryPoint.TargetFPS)
            {
                Thread.Sleep((int)Math.Round(1000 / EntryPoint.TargetFPS - e.Time));
            }
            else if (!EntryPoint.VSyncEnabled && e.Time < 1000d / EntryPoint.MaxFPS)
            {
                Thread.Sleep((int)Math.Round(1000 / EntryPoint.MaxFPS - e.Time));
            }
        }
    }
}
