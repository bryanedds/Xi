using Microsoft.Xna.Framework.Input;

namespace Xi
{
    /// <summary>
    /// Provides helper methods for processing input.
    /// </summary>
    public static class InputHelper
    {
        /// <summary>
        /// Get the current keyboard state modifier.
        /// </summary>
        public static KeyboardModifier GetModifier(this KeyboardState keyboard)
        {
            bool controlKeyState, shiftKeyState;
            keyboard.GetModifierStates(out controlKeyState, out shiftKeyState);
            if (controlKeyState && shiftKeyState) return KeyboardModifier.ControlShift;
            if (controlKeyState) return KeyboardModifier.Control;
            if (shiftKeyState) return KeyboardModifier.Shift;
            return KeyboardModifier.None;
        }

        /// <summary>
        /// Get the current keyboard state modifiers.
        /// </summary>
        public static void GetModifierStates(this KeyboardState keyboard, out bool controlState, out bool shiftState)
        {
            controlState = keyboard.GetControlState();
            shiftState = keyboard.GetShiftState();
        }

        /// <summary>
        /// Is the keyboard in a shift state?
        /// </summary>
        public static bool GetShiftState(this KeyboardState keyboard)
        {
            return keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
        }

        /// <summary>
        /// Is the keyboard in a control state?
        /// </summary>
        public static bool GetControlState(this KeyboardState keyboard)
        {
            return keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
        }
    }
}
