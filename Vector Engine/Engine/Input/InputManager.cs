using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine.Input
{
    public static class InputManager
    {
        private static KeyboardState lastState;
        private static KeyboardState currentState;
        private static MouseState lastMouseState;
        private static MouseState currentMouseState;
        private static Mouse

        public static bool UpdateInput()
        {
            lastState = currentState;
            currentState = Keyboard.GetState();

            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        public static bool IsKeyDown(Key key)
        {
            return currentState.IsKeyDown(key);
        }

        public static bool IsKeyNowDown(Key key)
        {
            return currentState.IsKeyDown(key) && !lastState.IsKeyDown(key);
        }

        public static bool IsKeyNowUp(Key key)
        {
            return !currentState.IsKeyDown(key) && lastState.IsKeyDown(key);
        }

        public static bool IsMouseDown(MouseButton button)
        {
            return currentMouseState.IsButtonDown(button);
        }

        public static bool IsMouseNowDown()
        {

        }

        public static bool IsMouseNowUp()
        {

        }
    }
}
