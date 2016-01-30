using System;
using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// A user interface button.
    /// </summary>
    public class Button : ActorUI
    {
        /// <summary>
        /// Create a Button.
        /// </summary>
        /// <param name="game">The game.</param>
        public Button(XiGame game) : base(game) { }

        /// <summary>
        /// Raised when the button is clicked.
        /// </summary>
        public event Action<Button> Clicked;

        /// <summary>
        /// The message definition invoked when clicked.
        /// </summary>
        public string ClickedMessageDefinitions
        {
            get { return clickedMessageDefinitions; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                clickedMessageDefinitions = value;
            }
        }

        /// <summary>
        /// The name of the unfocused surface image file.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string UnfocusedSurfaceFileName
        {
            get { return unfocusedSurfaceFileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                unfocusedSurfaceFileName = value;
            }
        }

        /// <summary>
        /// The name of the released surface image file.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string ReleasedSurfaceFileName
        {
            get { return releasedSurfaceFileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                releasedSurfaceFileName = value;
            }
        }

        /// <summary>
        /// The name of the pressed surface image file.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string PressedSurfaceFileName
        {
            get { return pressedSurfaceFileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                pressedSurfaceFileName = value;
            }
        }

        /// <inheritdoc />
        protected override Vector2 SizeHook
        {
            get { return SpriteHelper.GetSize(Game.Content, CurrentSurfaceFileName); }
        }

        /// <inheritdoc />
        protected override void OnDeallocated()
        {
            base.OnDeallocated();
            Clicked = null;
        }

        /// <inheritdoc />
        protected override void VisualizeHook(GameTime gameTime)
        {
            base.VisualizeHook(gameTime);
            Game.SpriteBatchUI.TryDraw(Game.Content, CurrentSurfaceFileName, PositionXY, 0, PositionZ);
        }

        /// <inheritdoc />
        protected override void OnSemanticInput(GameTime gameTime, InputType inputType, SemanticInputType semantic)
        {
            base.OnSemanticInput(gameTime, inputType, semantic);
            if (semantic == SemanticInputType.Affirm) OnAffirmInput(inputType);
        }

        /// <inheritdoc />
        protected override void OnFocusIndexChanged()
        {
            base.OnFocusIndexChanged();
            pressed = false;
        }

        private string CurrentSurfaceFileName
        {
            get
            {
                if (!Focused) return unfocusedSurfaceFileName;
                if (pressed) return pressedSurfaceFileName;
                return releasedSurfaceFileName;
            }
        }

        private void OnAffirmInput(InputType inputType)
        {
            if (inputType == InputType.ClickDown) pressed = true;
            else if (inputType == InputType.ClickUp && pressed) Click();
        }

        private void Click()
        {
            pressed = false;
            Clicked.TryRaise(this);
            Message.TryInvokeMessages(this, clickedMessageDefinitions, null);
        }

        private bool pressed;
        private string unfocusedSurfaceFileName = "Xi/UI/ButtonUnfocusedImage";
        private string releasedSurfaceFileName = "Xi/UI/ButtonReleasedImage";
        private string pressedSurfaceFileName = "Xi/UI/ButtonPressedImage";
        private string clickedMessageDefinitions = string.Empty;
    }
}
