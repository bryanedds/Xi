using System.ComponentModel;
using System.Drawing.Design;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// A sprite actor.
    /// </summary>
    public class Sprite : Actor2D
    {
        /// <summary>
        /// Create a Sprite.
        /// </summary>
        /// <param name="game">The game.</param>
        public Sprite(XiGame game)
            : base(game, true)
        {
            SetUpFixture();
        }

        /// <summary>
        /// The physics body type.
        /// </summary>
        [PhysicsBrowse]
        public BodyType BodyType
        {
            get { return Body.BodyType; }
            set { Body.BodyType = value; }
        }

        /// <summary>
        /// The name of the image file that contains rendering information used to draw the sprite.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string ImageFileName
        {
            get { return imageFileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (imageFileName == value) return; // OPTIMIZATION: avoid calling SetUpFixture
                imageFileName = value;
                SetUpFixture();
            }
        }

        /// <inheritdoc />
        protected override Vector2 SizeHook
        {
            get { return SpriteHelper.GetSize(Game.Content, imageFileName); }
        }

        /// <inheritdoc />
        protected override void VisualizeHook(GameTime gameTime)
        {
            base.VisualizeHook(gameTime);
            Game.SpriteBatch2D.TryDraw(Game.Content, imageFileName, PositionXY, Rotation, PositionZ);
        }

        private void SetUpFixture()
        {
            Fixture = FixtureFactory.CreateRectangle(Game.World, Size.X, Size.Y, 1);
        }

        private string imageFileName = "Xi/2D/SpriteImage";
    }
}
