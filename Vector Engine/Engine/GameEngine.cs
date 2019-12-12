using VectorEngine.Core.Common;
using VectorEngine.Core.Rendering.Objects;
using VectorEngine.Core.Rendering.Shaders;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Threading;
using VectorEngine.Core.Rendering.LowLevel;
using VectorEngine.Engine;

namespace VectorEngine.Core
{
    public class GameEngine
    {
        private const float FOV = 60.0f;
        private const float NEAR_PLANE = 0.1f;
        private const float FAR_PLANE = 1000f;

        private readonly WindowHandler windowHandler;

        public GameEngine(string[] startParameters)
        {
            windowHandler = new WindowHandler(640, 480, "Vector Engine", this)
            {
                VSync = VSyncMode.Off
            };

            windowHandler.Run();

            while (windowHandler.Exists) Thread.Sleep(50);

            windowHandler.Dispose();
        }

        StaticShader staticShader;
        Light light;
        Camera camera;

        public void OnLoad(EventArgs e)
        {
            staticShader = new StaticShader();
            RenderEngine.Setup(staticShader);
            light = new Light(new Vector3d(-500, 500, 500), new Vector3d(1, 1, 1));
            camera = new Camera();
            camera.Move(new Vector3d(0, 80, 0));
            camera.Rotate(new Vector3d(0, 90, 0));

            RenderEngine.SetProjectionMatrix(CreateProjectionMatrix());
        }

        public void OnClosed(EventArgs e)
        {
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
            return Matrix4.CreatePerspectiveFieldOfView((float)MathLib.ConvertToRadians(FOV), aspectRatio, NEAR_PLANE, FAR_PLANE);
        }

        public void OnUpdateFrame(FrameEventArgs e)
        {
            //TODO:
            //Look at SpinWait which would replace Thread.Sleep(), which is known for being inconsistent.
            if (windowHandler.Focused)
            {
                KeyboardState keyboard = Keyboard.GetState();
                if (keyboard.IsKeyDown(Key.W))
                    camera.MoveDirectionBased(new Vector3d(0, 0, -10 * e.Time));
                if (keyboard.IsKeyDown(Key.A))
                    camera.MoveDirectionBased(new Vector3d(10 * e.Time, 0, 0));
                if (keyboard.IsKeyDown(Key.S))
                    camera.MoveDirectionBased(new Vector3d(0, 0, 10 * e.Time));
                if (keyboard.IsKeyDown(Key.D))
                    camera.MoveDirectionBased(new Vector3d(-10 * e.Time, 0, 0));
                if (keyboard.IsKeyDown(Key.Space))
                    camera.Move(new Vector3d(0, 10 * e.Time, 0));
                if (keyboard.IsKeyDown(Key.ShiftLeft))
                    camera.Move(new Vector3d(0, -10 * e.Time, 0));
                if (keyboard.IsKeyDown(Key.Q))
                    camera.Rotate(new Vector3d(0, -40 * e.Time, 0));
                if (keyboard.IsKeyDown(Key.E))
                    camera.Rotate(new Vector3d(0, 40 * e.Time, 0));
                if (keyboard.IsKeyDown(Key.X))
                    camera.Rotate(new Vector3d(40 * e.Time, 0, 0));
                if (keyboard.IsKeyDown(Key.Z))
                    camera.Rotate(new Vector3d(-40 * e.Time, 0, 0));
            }
        }

        public void OnRenderFrame(FrameEventArgs e)
        {
            windowHandler.SetWindowTitle($"Vector Engine | VSync: { EntryPoint.VSyncEnabled } FPS: { ((int)(1 / e.Time * 10)) / 10f }");

            RenderEngine.CleanUp();

            light.SetPosition(camera.GetPosition());
            
            staticShader.EnableShader();
            staticShader.LoadViewMatrix(camera);
            staticShader.LoadLight(light);

            // Run render code here

            RenderEngine.ClearScreen();

            ShaderProgram.DisableShader();
            
            windowHandler.SwapBuffers();

            if (EntryPoint.VSyncEnabled && e.Time < 1000d / EntryPoint.TargetFPS)
                Thread.Sleep((int)Math.Round(1000 / EntryPoint.TargetFPS - e.Time));
            else if (!EntryPoint.VSyncEnabled && e.Time < 1000d / EntryPoint.MaxFPS)
                Thread.Sleep((int)Math.Round(1000 / EntryPoint.MaxFPS - e.Time));
        }
    }
}
