using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Xi
{
    /// <summary>
    /// Caches XNA input values.
    /// </summary>
    public class InputCache
    {
        /// <summary>
        /// Update during both editing and play.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            XiHelper.ArgumentNullCheck(gameTime);
            UpdateGamePadStates();
            UpdateKeyboardState();
        }

        /// <summary>
        /// Get the current keyboard state.
        /// </summary>
        public KeyboardState KeyboardState { get { return keyboardState; } }

        /// <summary>
        /// Get the current game pad state for the given player index.
        /// </summary>
        public GamePadState GetGamePadState(PlayerIndex playerIndex)
        {
            return gamePadStates[(int)playerIndex];
        }

        private void UpdateGamePadStates()
        {
            for (PlayerIndex playerIndex = 0; playerIndex < (PlayerIndex)4; ++playerIndex)
                gamePadStates[(int)playerIndex] = GamePad.GetState(playerIndex);
        }

        private void UpdateKeyboardState()
        {
            keyboardState = Keyboard.GetState();
        }

        private KeyboardState keyboardState;
        private readonly GamePadState[] gamePadStates = new GamePadState[4];
    }
}
