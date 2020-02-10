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

        private DateTime engineStartTime;

        public void OnLoad(EventArgs e)
        {
            engineStartTime = DateTime.UtcNow;
            entryPoint.OnLoad();

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
        }

        public void OnRenderFrame(FrameEventArgs e)
        {
            double time = (DateTime.UtcNow - engineStartTime).TotalSeconds;

            windowHandler.SetWindowTitle($"Vector Engine");

            RenderEngine.BufferFlush();
            RenderEngine.SetTimeData(time);

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
