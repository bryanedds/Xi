using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// An object that can have input focus.
    /// </summary>
    public abstract class Focusable : Recyclable
    {
        /// <summary>
        /// Create a Focusable object.
        /// </summary>
        /// <param name="game">The game.</param>
        public Focusable(XiGame game) : base(game) { }

        /// <summary>
        /// Establishes focus for a given player input.
        /// Will not be null during InputHook.
        /// </summary>
        public PlayerIndex? FocusIndex
        {
            get { return _focusIndex; }
            set
            {
                ValidateFocusIndex(value);
                if (_focusIndex.HasValue) Game.FocusManager.SetFocusedObject(null, _focusIndex.Value);
                if (value.HasValue) Game.FocusManager.SetFocusedObject(this, value.Value);
                _focusIndex = value;
                OnFocusIndexChanged();
            }
        }

        /// <summary>
        /// Is currently focused?
        /// </summary>
        [Browsable(false)]
        public bool Focused { get { return FocusIndex.HasValue; } }

        /// <summary>
        /// Does accept focus if given?
        /// </summary>
        [Browsable(false)]
        public bool AcceptFocus { get { return AcceptFocusHook; } }

        /// <summary>
        /// Handle input during editing and game play.
        /// </summary>
        public void Input(GameTime gameTime)
        {
            XiHelper.ArgumentNullCheck(gameTime);
            ValidateCanInput();
            InputHook(gameTime);
        }

        /// <summary>
        /// Notify that directional input has taken place.
        /// </summary>
        public void NotifyDirectionInput(GameTime gameTime, InputType inputType, Direction2D direction)
        {
            OnDirectionInput(gameTime, inputType, direction);
        }

        /// <summary>
        /// Notify that semantic input has taken place.
        /// TODO: expand on meaning of 'semantic input'.
        /// </summary>
        public void NotifySemanticInput(GameTime gameTime, InputType inputType, SemanticInputType semantic)
        {
            OnSemanticInput(gameTime, inputType, semantic);
        }

        /// <summary>
        /// Handle providing the value of the AcceptFocus property.
        /// </summary>
        protected virtual bool AcceptFocusHook { get { return true; } }

        /// <inheritdoc />
        protected override void Destroy(bool destroying)
        {
            if (destroying) FocusIndex = null;
            base.Destroy(destroying);
        }

        /// <inheritdoc />
        protected override bool IsHidden(PropertyDescriptor property)
        {
            return
                base.IsHidden(property) ||
                !AcceptFocus && property.Name == "FocusIndex";
        }

        /// <summary>
        /// Handle processing input.
        /// </summary>
        protected virtual void InputHook(GameTime gameTime) { }

        /// <summary>
        /// Handle the fact that directional input took place.
        /// </summary>
        protected virtual void OnDirectionInput(GameTime gameTime, InputType inputType, Direction2D direction) { }

        /// <summary>
        /// Handle the fact that semantic input took place.
        /// </summary>
        protected virtual void OnSemanticInput(GameTime gameTime, InputType inputType, SemanticInputType semantic) { }

        /// <summary>
        /// Handle the fact that the focus index has changed.
        /// </summary>
        protected virtual void OnFocusIndexChanged() { }

        private void ValidateFocusIndex(PlayerIndex? value)
        {
            if (value.HasValue && !AcceptFocus)
                throw new InvalidOperationException("Cannot focus on an unfocusable object.");
        }

        private void ValidateCanInput()
        {
            if (!Focused)
                throw new InvalidOperationException("Cannot input to an unfocused object.");
        }

        private PlayerIndex? _focusIndex;
    }
}
