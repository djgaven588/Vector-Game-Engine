using VectorEngine;
using VectorEngine.Core;
using VectorEngine.Engine;
using VectorEngine.Engine.Common;

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
            GameEngine engine = new GameEngine(args, this);

            Debug.Log("Game engine exited. Closing!");
        }

        public void OnClose()
        {

        }

        public void OnLoad()
        {

        }

        public void OnRender(double timeDelta)
        {

        }

        public void OnUpdate(double timeDelta)
        {

        }
    }
}
