using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// A user interface label.
    /// </summary>
    public class Label : ActorUI
    {
        /// <summary>
        /// Create a Label.
        /// </summary>
        /// <param name="game">The game.</param>
        public Label(XiGame game) : base(game) { }

        /// <summary>
        /// The name of the surface image file.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string SurfaceFileName
        {
            get { return surfaceFileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                surfaceFileName = value;
            }
        }

        /// <inheritdoc />
        protected override Vector2 SizeHook
        {
            get { return SpriteHelper.GetSize(Game.Content, surfaceFileName); }
        }

        /// <inheritdoc />
        protected override bool AcceptFocusHook { get { return false; } }

        /// <inheritdoc />
        protected override void VisualizeHook(GameTime gameTime)
        {
            base.VisualizeHook(gameTime);
            Game.SpriteBatchUI.TryDraw(Game.Content, surfaceFileName, PositionXY, 0, PositionZ);
        }

        private string surfaceFileName = "Xi/UI/LabelImage";
    }
}
