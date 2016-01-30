using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Provides helper methods for sprite operations.
    /// </summary>
    public static class SpriteHelper
    {
        /// <summary>
        /// Try to draw a sprite.
        /// </summary>
        /// <param name="sprites">The sprite batch to draw into.</param>
        /// <param name="content">The content to find the image texture in.</param>
        /// <param name="imageFileName">The name of the image file.</param>
        /// <param name="position">The position of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        /// <param name="depth">The depth of the sprite.</param>
        public static void TryDraw(
            this SpriteBatch sprites,
            ContentManager content,
            string imageFileName,
            Vector2 position,
            float rotation,
            float depth)
        {
            try
            {
                Texture2D imageResource = content.Load<Texture2D>(imageFileName);
                Vector2 imageResourceSize = new Vector2(imageResource.Width, imageResource.Height);
                Vector2 imageResourceCenter = imageResourceSize * 0.5f;
                sprites.Draw(imageResource, position, null, Color.White, rotation, imageResourceCenter, 1, SpriteEffects.None, depth);
            }
            catch (ContentLoadException)
            {
                // swallow content load exception
                // TODO: consider logging this
            }
        }

        public static Vector2 GetSize(ContentManager content, string imageFileName)
        {
            try
            {
                Texture2D imageResource = content.Load<Texture2D>(imageFileName);
                return new Vector2(imageResource.Width, imageResource.Height);
            }
            catch (ContentLoadException)
            {
                // swallow content load exception
                // TODO: consider logging this
                return Vector2.Zero;
            }
        }
    }
}
