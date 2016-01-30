using Microsoft.Xna.Framework.Input;

namespace Xi
{
    /// <summary>
    /// Polls input from raw input states.
    /// </summary>
    public class InputRouter
    {
        /// <summary>
        /// Get the state of the given direction button.
        /// </summary>
        public ButtonState GetDirectionState(Direction2D direction, ref GamePadState gamePadState)
        {
            switch (direction)
            {
                case Direction2D.Up: return GetDirectionUpState(ref gamePadState);
                case Direction2D.Down: return GetDirectionDownState(ref gamePadState);
                case Direction2D.Left: return GetDirectionLeftState(ref gamePadState);
                case Direction2D.Right: return GetDirectionRightState(ref gamePadState);
                default: return ButtonState.Released;
            }
        }

        /// <summary>
        /// Get the state of semantic input.
        /// </summary>
        public ButtonState GetSemanticButtonState(SemanticButtonType type, ref GamePadState gamePadState)
        {
            switch (type)
            {
                case SemanticButtonType.AffirmButton: return GetAffirmState(ref gamePadState);
                case SemanticButtonType.CancelButton: return GetCancelState(ref gamePadState);
                case SemanticButtonType.NextPageButton: return GetNextPageState(ref gamePadState);
                case SemanticButtonType.PreviousPageButton: return GetPreviousPageState(ref gamePadState);
                default: return ButtonState.Released;
            }
        }

        private ButtonState GetDirectionRightState(ref GamePadState gamePadState)
        {
            return
                gamePadState.DPad.Left == ButtonState.Released ?
                gamePadState.DPad.Right :
                ButtonState.Released;
        }

        private ButtonState GetDirectionLeftState(ref GamePadState gamePadState)
        {
            return
                gamePadState.DPad.Right == ButtonState.Released ?
                gamePadState.DPad.Left :
                ButtonState.Released;
        }

        private ButtonState GetDirectionDownState(ref GamePadState gamePadState)
        {
            return
                gamePadState.DPad.Up == ButtonState.Released ?
                gamePadState.DPad.Down :
                ButtonState.Released;
        }

        private ButtonState GetDirectionUpState(ref GamePadState gamePadState)
        {
            return
                gamePadState.DPad.Down == ButtonState.Released ?
                gamePadState.DPad.Up :
                ButtonState.Released;
        }

        private ButtonState GetAffirmState(ref GamePadState gamePadState)
        {
            return gamePadState.Buttons.A;
        }

        private ButtonState GetCancelState(ref GamePadState gamePadState)
        {
            return gamePadState.Buttons.B;
        }

        private ButtonState GetNextPageState(ref GamePadState gamePadState)
        {
            return gamePadState.Buttons.RightShoulder;
        }

        private ButtonState GetPreviousPageState(ref GamePadState gamePadState)
        {
            return gamePadState.Buttons.LeftShoulder;
        }
    }
}
