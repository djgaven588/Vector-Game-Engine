using VectorEngine.Core;
using System.Threading;
using VectorEngine.Engine;

namespace VectorEngine
{
    class EntryPoint
    {
        public const int TargetFPS = 60;
        public const int MaxFPS = 300;
        public const bool VSyncEnabled = true;

        public static void Main(string[] args)
        {
            new EntryPoint().Start(args);
        }

        private void Start(string[] startParameters)
        {
            new GameEngine(startParameters);

            Debug.Log("Game engine exited. Closing!");
        }
    }
}
