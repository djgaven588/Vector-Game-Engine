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
using VectorEngine.Engine.Common.LowLevel;
using VectorEngine.Engine.Particles;

namespace VectorEngine.Core
{
    public class GameEngine
    {
        private const float FOV = 60.0f;
        private const float NEAR_PLANE = 0.1f;
        private const float FAR_PLANE = 1000f;

        public static WindowHandler windowHandler;

        public GameEngine(string[] startParameters)
        {
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
        private VectorCompositionRoot root;
        private ParticleSystem testParticles;

        public void OnLoad(EventArgs e)
        {
            light = new Light(new Vector3d(-500, 500, 500), new Vector3d(1, 1, 1));
            camera = new Camera();
            camera.Position = new Vector3d(0, 0, 0);
            camera.Rotation = new Vector3d(0, 0, 0);

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

            root = new VectorCompositionRoot();

            testParticles = new ParticleSystem(10, TestParticleGenerator, TestParticleCreator);
            testParticles.VelocityChangeOverLifeTime(TestParticleVelocityChange);
        }

        private static float lastSpawned = 0f;
        private static int TestParticleGenerator((int particleCount, float lifeTime) systemData)
        {
            if (systemData.lifeTime - lastSpawned > 0.25 && systemData.particleCount < 10)
            {
                lastSpawned += 0.25f;
                return 1;
            }

            return 0;
        }

        private static Random rand = new Random();
        private static (Vector3, Vector3, float) TestParticleCreator()
        {
            float orbitalPosition = (float)rand.NextDouble() * 2;
            //return (Vector3.One, Vector3.Zero, 1);
            return (new Vector3((float)Math.Sin(orbitalPosition), orbitalPosition, (float)Math.Cos(orbitalPosition)), Vector3.Zero, orbitalPosition);
        }

        private static Vector3 TestParticleVelocityChange(float remainingLifetime, Vector3 position)
        {
            return (Vector3.Zero - position) * (5 - remainingLifetime) * 5;
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
            return Matrix4.CreatePerspectiveFieldOfView((float)Mathmatics.ConvertToRadians(FOV), aspectRatio, NEAR_PLANE, FAR_PLANE);
        }

        public void OnUpdateFrame(FrameEventArgs e)
        {
            VectorSchedulers.RunUpdate();

            //TODO:
            //Look at SpinWait which would replace Thread.Sleep(), which is known for being inconsistent.
            if (windowHandler.Focused)
            {
                KeyboardState keyboard = Keyboard.GetState();
                if (keyboard.IsKeyDown(Key.W))
                {
                    camera.MoveDirectionBased(new Vector3d(0, 0, -10 * e.Time));
                }

                if (keyboard.IsKeyDown(Key.A))
                {
                    camera.MoveDirectionBased(new Vector3d(10 * e.Time, 0, 0));
                }

                if (keyboard.IsKeyDown(Key.S))
                {
                    camera.MoveDirectionBased(new Vector3d(0, 0, 10 * e.Time));
                }

                if (keyboard.IsKeyDown(Key.D))
                {
                    camera.MoveDirectionBased(new Vector3d(-10 * e.Time, 0, 0));
                }

                if (keyboard.IsKeyDown(Key.Space))
                {
                    camera.Position += new Vector3d(0, 10 * e.Time, 0);
                }

                if (keyboard.IsKeyDown(Key.ShiftLeft))
                {
                    camera.Position += new Vector3d(0, -10 * e.Time, 0);
                }

                if (keyboard.IsKeyDown(Key.Q))
                {
                    camera.Rotation += new Vector3d(0, -80 * e.Time, 0);
                }

                if (keyboard.IsKeyDown(Key.E))
                {
                    camera.Rotation += new Vector3d(0, 80 * e.Time, 0);
                }

                if (keyboard.IsKeyDown(Key.X))
                {
                    camera.Rotation += new Vector3d(-80 * e.Time, 0, 0);
                }

                if (keyboard.IsKeyDown(Key.Z))
                {
                    camera.Rotation += new Vector3d(80 * e.Time, 0, 0);
                }

                if (keyboard.IsKeyDown(Key.K))
                {
                    testParticles.RunUpdate((float)e.Time);
                }
            }
        }

        public void OnRenderFrame(FrameEventArgs e)
        {
            windowHandler.SetWindowTitle($"Vector Engine | VSync: { EntryPoint.VSyncEnabled } FPS: { ((int)(1 / e.Time * 10)) / 10f }");

            RenderEngine.CleanUp();
            light.Position = camera.Position;

            RenderEngine.PrepareForRendering();
            staticShader.LoadViewMatrix(camera);
            staticShader.LoadLight(light);

            VectorSchedulers.RunRender();

            // Run render code here
            RenderEngine.RenderMeshNow(Mathmatics.CreateTransformationMatrix(new Vector3d(0, 0, -2), Vector3d.Zero, Vector3d.One), testMesh, treeTexture);
            RenderEngine.RenderMeshNow(Mathmatics.CreateTransformationMatrix(new Vector3d(5, 0, -5), Vector3d.Zero, Vector3d.One), treeMesh, treeTexture);
            testParticles.RenderParticles(Mathmatics.CreateTransformationMatrix(Vector3d.Zero, Vector3d.Zero, Vector3d.One), testMesh, treeTexture);

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
