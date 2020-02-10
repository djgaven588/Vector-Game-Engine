using OpenTK;
using OpenTK.Input;
using System;
using VectorEngine;
using VectorEngine.Core;
using VectorEngine.Core.Common;
using VectorEngine.Core.Rendering;
using VectorEngine.Core.Rendering.LowLevel;
using VectorEngine.Core.Rendering.Objects;
using VectorEngine.Core.Rendering.Shaders;
using VectorEngine.Engine;
using VectorEngine.Engine.Common;
using VectorEngine.Engine.Input;
using VectorEngine.Engine.Rendering;

namespace ExampleGame
{
    public class GameEntryPoint : IStartEngine
    {
        public int TargetFrameRate => 60;

        public int MaxFrameRate => 300;

        public bool UseVSync => true;

        private static void Main(string[] args)
        {
            new GameEntryPoint().Start(args);
        }

        private void Start(string[] args)
        {
            engine = new GameEngine(args, this);

            Debug.Log("Game engine exited. Closing!");
        }

        GameEngine engine;
        Light light;
        Light mainLight;
        Light anotherLight;

        Camera camera;
        Camera secondCamera;

        private Mesh treeMesh;
        private int treeTexture;
        private Material standardMaterial;
        private Material uiMaterial;

        private DateTime engineStartTime;
        private ShaderProgram invertColorPostProcessing;
        private Mesh textMesh;
        private FontData arialFont;
        private Material arialFontMaterial;

        public void OnClose()
        {
            standardMaterial.Shader.CleanUp();
            uiMaterial.Shader.CleanUp();
            arialFontMaterial.Shader.CleanUp();
            invertColorPostProcessing.CleanUp();

            RenderDataLoader.DeleteFrameBuffer(camera.fboId);
            RenderDataLoader.DeleteRenderBuffer(camera.bufId);
            RenderDataLoader.DeleteTexture(camera.texId);

            RenderDataLoader.DeleteFrameBuffer(secondCamera.fboId);
            RenderDataLoader.DeleteRenderBuffer(secondCamera.bufId);
            RenderDataLoader.DeleteTexture(secondCamera.texId);
        }

        public void OnLoad()
        {
            arialFont = new FontData("Arial", "./Game/Fonts/Arial.fnt", 1024);
            arialFontMaterial = new Material(new UIShader(arialFont.TextureId), false, false, false);
            invertColorPostProcessing = new InvertedColorPostProcessing();
            engineStartTime = DateTime.UtcNow;

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
                ClearColor = new Vector4(0, 0, 0, 0),
                PostProcessing = new Material(invertColorPostProcessing, false, false, false)
            };

            treeMesh = OBJLoader.LoadObjModel("Tree");

            treeTexture = RenderDataLoader.LoadTexture("Tree");
            standardMaterial = new Material(new StaticShader(treeTexture), true, true, false);
            uiMaterial = new Material(new UIShader(treeTexture), false, false, false);
        }

        public void OnRender(double timeDelta)
        {
            double time = (DateTime.UtcNow - engineStartTime).TotalSeconds;

            secondCamera.Position = new Vector3d(Math.Sin(time) * 5, 3, Math.Cos(time) * 5);
            secondCamera.Rotation = new Vector3d(25, -Math.Atan2(Math.Sin(time), Math.Cos(time)) * 180 / Mathmatics.PI, 0);

            RenderEngine.AddCamera(camera);
            RenderEngine.AddCamera(secondCamera);
            RenderEngine.AddLight(light);
            RenderEngine.AddLight(mainLight);
            RenderEngine.AddLight(anotherLight);

            QueueUI(timeDelta);

            RenderEngine.AddToRenderQueue(standardMaterial, treeMesh, Mathmatics.CreateTransformationMatrix(new Vector3d(0, 0, 0), Vector3d.Zero, Vector3d.One), false);
            RenderEngine.AddToRenderQueue(standardMaterial, treeMesh, Mathmatics.CreateTransformationMatrix(new Vector3d(0, 0, -5), Vector3d.Zero, Vector3d.One), false);
        }

        private void QueueUI(double timeDelta)
        {
            (int windowWidth, int windowHeight) = GameEngine.windowHandler.GetWindowDimensions();
            double uiScale = windowWidth > windowHeight ? windowWidth / 1920d : windowHeight / 1080d;

            textMesh = TextMeshGenerator.RegenerateMesh($"Vector Engine\n" +
                $"VSync: {UseVSync}\n" +
                $"FPS: {((int)(1 / timeDelta * 10)) / 10f}", 
                arialFont, textMesh, 2, 50);

            RenderEngine.AddToRenderQueue(uiMaterial, treeMesh, Mathmatics.CreateTransformationMatrix(new Vector3d(1, 0, 0) / uiScale, Vector3d.Zero, new Vector3d(250d / windowWidth, 250d / windowHeight, 1) * uiScale), true);
            //RenderEngine.AddToRenderQueue(uiMaterial, treeMesh, Mathmatics.CreateTransformationMatrix(new Vector3d(0, 0, 0) / uiScale, Vector3d.Zero, new Vector3d(0.5d, 0.5d, 1) * uiScale), true);
            RenderEngine.AddToRenderQueue(arialFontMaterial, textMesh, Mathmatics.CreateTransformationMatrix(new Vector3d(-windowWidth + 50, windowHeight - 50, 0) / uiScale, Vector3d.Zero, new Vector3d(1d / windowWidth, 1d / windowHeight, 1) * uiScale), true);
        }

        public void OnUpdate(double timeDelta)
        {
            double xAxis = InputManager.IsKeyDown(Key.D) && !InputManager.IsKeyDown(Key.A) ? 1 : !InputManager.IsKeyDown(Key.D) && InputManager.IsKeyDown(Key.A) ? -1 : 0;
            double yAxis = InputManager.IsKeyDown(Key.Space) && !InputManager.IsKeyDown(Key.ShiftLeft) ? 1 : !InputManager.IsKeyDown(Key.Space) && InputManager.IsKeyDown(Key.ShiftLeft) ? -1 : 0;
            double zAxis = -(InputManager.IsKeyDown(Key.W) && !InputManager.IsKeyDown(Key.S) ? 1 : !InputManager.IsKeyDown(Key.W) && InputManager.IsKeyDown(Key.S) ? -1 : 0);

            double yRotation = InputManager.IsKeyDown(Key.E) && !InputManager.IsKeyDown(Key.Q) ? 1 : !InputManager.IsKeyDown(Key.E) && InputManager.IsKeyDown(Key.Q) ? -1 : 0;
            double xRotation = InputManager.IsKeyDown(Key.Z) && !InputManager.IsKeyDown(Key.X) ? 1 : !InputManager.IsKeyDown(Key.Z) && InputManager.IsKeyDown(Key.X) ? -1 : 0;

            camera.Rotation += new Vector3d(xRotation, yRotation, 0) * 90 * timeDelta;

            camera.MoveDirectionBased(new Vector3d(xAxis, yAxis, zAxis) * 10 * timeDelta);
        }
    }
}
