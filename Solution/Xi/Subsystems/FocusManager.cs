using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// Manages the focusing and input processing of objects.
    /// </summary>
    public class FocusManager
    {
        /// <summary>
        /// Create a FocusManager.
        /// </summary>
        /// <param name="game">The game.</param>
        public FocusManager(XiGame game)
        {
            SetUpFocusDescriptors(game);
        }

        /// <summary>
        /// Get the object currently focused by the given focus index.
        /// May return null.
        /// </summary>
        public Focusable GetFocusedObject(PlayerIndex focusIndex)
        {
            FocusDescriptor focusDescriptor = focusDescriptors[(int)focusIndex];
            return focusDescriptor.FocusedObject;
        }

        /// <summary>
        /// Process input.
        /// </summary>
        public void Input(GameTime gameTime)
        {
            XiHelper.ArgumentNullCheck(gameTime);
            foreach (FocusDescriptor focusDescriptor in focusDescriptors)
                focusDescriptor.Input(gameTime);
        }

        /// <summary>
        /// Set an object focused by the given focus index.
        /// </summary>
        internal void SetFocusedObject(Focusable focusable, PlayerIndex focusIndex)
        {
            FocusDescriptor focusDescriptor = focusDescriptors[(int)focusIndex];
            focusDescriptor.FocusedObject = focusable;
        }

        private void SetUpFocusDescriptors(XiGame game)
        {
            for (int i = 0; i < focusDescriptors.Length; ++i)
                focusDescriptors[i] = new FocusDescriptor(game, (PlayerIndex)i);
        }

        private readonly FocusDescriptor[] focusDescriptors = new FocusDescriptor[4];
    }
}
