using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Xi
{
    /// <summary>
    /// Forward input for a given focus index.
    /// </summary>
    public class InputForwarder
    {
        /// <summary>
        /// Create an InputForwarder.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="focusIndex">The focus index for which input is being forwarded.</param>
        public InputForwarder(XiGame game, PlayerIndex focusIndex)
        {
            this.game = game;
            this.focusIndex = focusIndex;
        }

        /// <summary>
        /// Forward input to the given focusable object.
        /// </summary>
        public void ForwardInput(GameTime gameTime, Focusable focusable)
        {
            ValidateMatchingFocusIndex(focusable);
            GamePadState gamePadState = game.GetGamePadState(focusIndex);
            Input(gameTime, focusable);
            DirectionsInput(gameTime, ref gamePadState, focusable);
            SemanticButtonsInput(gameTime, ref gamePadState, focusable);
        }

        private void Input(GameTime gameTime, Focusable focusable)
        {
            focusable.Input(gameTime);
        }

        private void DirectionsInput(GameTime gameTime, ref GamePadState gamePadState, Focusable focusable)
        {
            for (Direction2D direction = 0; direction < Direction2D.Count; ++direction)
            {
                if (!focusable.Focused) break;
                ButtonState directionButtonState = inputRouter.GetDirectionState(direction, ref gamePadState);
                DirectionInput(gameTime, direction, directionButtonState, focusable);
            }
        }

        private void DirectionInput(GameTime gameTime, Direction2D direction, ButtonState directionButtonState, Focusable focusable)
        {
            if (directionButtonState == ButtonState.Pressed)
            {
                if (isDirectionPressed[(int)direction] != ButtonState.Pressed) focusable.NotifyDirectionInput(gameTime, InputType.ClickDown, direction);
                DirectionDownRepeat(gameTime, direction, focusable);
                focusable.NotifyDirectionInput(gameTime, InputType.Down, direction);
            }
            else if (directionButtonState != ButtonState.Pressed)
            {
                if (isDirectionPressed[(int)direction] == ButtonState.Pressed)
                {
                    DirectionUp(direction);
                    focusable.NotifyDirectionInput(gameTime, InputType.ClickUp, direction);
                }
                // OPTIMIZATION: this functionality is not worth the speed cost
                //DirectionUpRepeat(gameTime, direction);
                //screen.NotifyDirectionInput(gameTime, InputType.Up, direction);
            }
            isDirectionPressed[(int)direction] = directionButtonState;
        }

        private void DirectionDownRepeat(GameTime gameTime, Direction2D direction, Focusable focusable)
        {
            directionPressedElapsedTime[(int)direction] += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (directionPressedElapsedTime[(int)direction] <= repeatRate.FirstDelay) return;
            directionPressedElapsedTime2[(int)direction] += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (directionPressedElapsedTime2[(int)direction] <= repeatRate.RepeatDelay) return;
            directionPressedElapsedTime2[(int)direction] = 0;
            focusable.NotifyDirectionInput(gameTime, InputType.Repeat, direction);
        }

        private void DirectionUp(Direction2D direction)
        {
            directionPressedElapsedTime[(int)direction] = 0;
            directionPressedElapsedTime2[(int)direction] = 0;
        }

        private void SemanticButtonsInput(GameTime gameTime, ref GamePadState gamePadState, Focusable focusable)
        {
            for (SemanticButtonType semantic = 0; semantic < SemanticButtonType.Count; ++semantic)
            {
                if (!focusable.Focused) break;
                SemanticButtonInput(gameTime, semantic, ref gamePadState, focusable);
            }
        }

        private void SemanticButtonInput(GameTime gameTime, SemanticButtonType type, ref GamePadState gamePadState, Focusable focusable)
        {
            ButtonState state = inputRouter.GetSemanticButtonState(type, ref gamePadState);
            if (state == ButtonState.Pressed)
            {
                if (isSemanticButtonPressed[(int)type] != ButtonState.Pressed) SemanticButtonInput(gameTime, type, InputType.ClickDown, focusable);
                SemanticButtonDownRepeat(gameTime, type, focusable);
                SemanticButtonInput(gameTime, type, InputType.Down, focusable);
            }
            else if (state != ButtonState.Pressed)
            {
                if (isSemanticButtonPressed[(int)type] == ButtonState.Pressed)
                {
                    SemanticButtonUp(type);
                    SemanticButtonInput(gameTime, type, InputType.ClickUp, focusable);
                }
                // OPTIMIZATION: this functionality is not worth the speed cost
                //SemanticButtonUpRepeat(type);
                //SemanticButtonInput(type, InputType.Up);
            }
            isSemanticButtonPressed[(int)type] = state;
        }

        private void SemanticButtonInput(GameTime gameTime, SemanticButtonType type, InputType inputType, Focusable focusable)
        {
            switch (type)
            {
                case SemanticButtonType.AffirmButton: focusable.NotifySemanticInput(gameTime, inputType, SemanticInputType.Affirm); break;
                case SemanticButtonType.CancelButton: focusable.NotifySemanticInput(gameTime, inputType, SemanticInputType.Cancel); break;
                case SemanticButtonType.NextPageButton: focusable.NotifySemanticInput(gameTime, inputType, SemanticInputType.NextPage); break;
                case SemanticButtonType.PreviousPageButton: focusable.NotifySemanticInput(gameTime, inputType, SemanticInputType.PreviousPage); break;
            }
        }

        private void SemanticButtonDownRepeat(GameTime gameTime, SemanticButtonType type, Focusable focusable)
        {
            semanticButtonPressedElapsedTime[(int)type] += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (semanticButtonPressedElapsedTime[(int)type] <= repeatRate.FirstDelay) return;
            semanticButtonPressedElapsedTime2[(int)type] += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (semanticButtonPressedElapsedTime2[(int)type] <= repeatRate.RepeatDelay) return;
            semanticButtonPressedElapsedTime2[(int)type] = 0;
            SemanticButtonInput(gameTime, type, InputType.Repeat, focusable);
        }

        private void SemanticButtonUp(SemanticButtonType type)
        {
            semanticButtonPressedElapsedTime[(int)type] = 0;
            semanticButtonPressedElapsedTime2[(int)type] = 0;
        }

        private void ValidateMatchingFocusIndex(Focusable focusable)
        {
            if (focusable.FocusIndex != focusIndex)
                throw new ArgumentException("Focusable does not match InputForwarder's input index.");
        }

        private readonly XiGame game;
        private readonly PlayerIndex focusIndex;
        private readonly ButtonState[] isSemanticButtonPressed = new ButtonState[(int)SemanticButtonType.Count];
        private readonly ButtonState[] isDirectionPressed = new ButtonState[(int)Direction2D.Count];
        private readonly float[] semanticButtonPressedElapsedTime = new float[(int)SemanticButtonType.Count];
        private readonly float[] semanticButtonPressedElapsedTime2 = new float[(int)SemanticButtonType.Count];
        private readonly float[] directionPressedElapsedTime = new float[(int)Direction2D.Count];
        private readonly float[] directionPressedElapsedTime2 = new float[(int)Direction2D.Count];
        private readonly InputRouter inputRouter = new InputRouter();
        private readonly RepeatRate repeatRate = new RepeatRate();
    }
}
