using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using VectorEngine.Core;

namespace VectorEngine.Engine
{
    public class WindowHandler : GameWindow
    {
        public WindowHandler(int windowWidth, int windowHeight, string windowName, GameEngine engine) : base(windowWidth, windowHeight, GraphicsMode.Default, windowName)
        {
            X = ClientSize.Width / 2 + windowWidth;
            Y = ClientSize.Height / 2;

            this.engine = engine;
        }

        private readonly GameEngine engine;

        protected override void OnLoad(EventArgs e)
        {
            Debug.Log(GL.IsEnabled(EnableCap.CullFace));
            engine.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            engine.OnClosed(e);
        }

        protected override void OnResize(EventArgs e)
        {
            engine.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            engine.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            engine.OnRenderFrame(e);
        }

        public void SetWindowTitle(string title)
        {
            Title = title;
        }

        public (int, int) GetWindowDimensions()
        {
            return (Width, Height);
        }
    }
}
