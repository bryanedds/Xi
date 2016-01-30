using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// Describes how an object is focused.
    /// </summary>
    public class FocusDescriptor
    {
        /// <summary>
        /// Create a FocusDescriptor.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="focusIndex">The focus index that is being described.</param>
        public FocusDescriptor(XiGame game, PlayerIndex focusIndex)
        {
            inputForwarder = new InputForwarder(game, focusIndex);
        }

        /// <summary>
        /// The focused object.
        /// May be null.
        /// </summary>
        public Focusable FocusedObject
        {
            get { return focusedObject; }
            set
            {
                if (focusedObjectChanging) return; // block reentry from Focusable.FocusIndex setter.
                focusedObjectChanging = true;
                if (focusedObject != null) focusedObject.FocusIndex = null;
                focusedObject = value;
                focusedObjectChanging = false;
            }
        }

        /// <summary>
        /// Process input.
        /// </summary>
        public void Input(GameTime gameTime)
        {
            XiHelper.ArgumentNullCheck(gameTime);
            if (focusedObject != null) inputForwarder.ForwardInput(gameTime, focusedObject);
        }

        /// <summary>May be null.</summary>
        private Focusable focusedObject;
        private readonly InputForwarder inputForwarder;
        private bool focusedObjectChanging;
    }
}
