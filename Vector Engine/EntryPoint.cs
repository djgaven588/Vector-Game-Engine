using VectorEngine.Core;
using VectorEngine.Engine;
using VectorEngine.Engine.Common;

namespace VectorEngine
{
    public class EntryPoint : IStartEngine
    {
        public const int TargetFPS = 60;
        public const int MaxFPS = 300;
        public const bool VSyncEnabled = true;

        public static void Main(string[] args)
        {
            new EntryPoint().Start(args);
        }

        public void OnLoad()
        {

        }

        public void OnClose()
        {

        }

        public void OnRender(double timeDelta)
        {

        }

        public void OnUpdate(double timeDelta)
        {

        }

        public void Start(string[] startParameters)
        {
            new GameEngine(startParameters, this);

            Debug.Log("Game engine exited. Closing!");
        }
    }
}
