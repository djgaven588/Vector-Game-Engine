using OpenTK;
using OpenTK.Input;
using VectorEngine.Core;

namespace VectorEngine.Engine.Input
{
    public static class InputManager
    {
        private static KeyboardState lastState;
        private static KeyboardState currentState;
        private static MouseState lastMouseState;
        private static MouseState currentMouseState;

        public static void UpdateInput()
        {
            lastState = currentState;
            currentState = GameEngine.windowHandler.Focused ? Keyboard.GetState() : new KeyboardState();

            lastMouseState = currentMouseState;
            currentMouseState = GameEngine.windowHandler.Focused ? Mouse.GetState() : new MouseState();
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

        public static bool IsMouseNowDown(MouseButton button)
        {
            return currentMouseState.IsButtonDown(button) && !lastMouseState.IsButtonDown(button);
        }

        public static bool IsMouseNowUp(MouseButton button)
        {
            return !currentMouseState.IsButtonDown(button) && lastMouseState.IsButtonDown(button);
        }

        public static Vector2 MouseDelta()
        {
            return new Vector2(currentMouseState.X - lastMouseState.X, currentMouseState.Y - lastMouseState.Y);
        }

        public static Vector2 MousePosition()
        {
            return new Vector2(currentMouseState.X, currentMouseState.Y);
        }
    }
}
