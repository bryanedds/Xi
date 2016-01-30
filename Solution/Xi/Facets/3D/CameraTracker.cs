using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// Causes the camera to track a 3D actor.
    /// </summary>
    public class CameraTracker : Facet<Actor3D>
    {
        /// <summary>
        /// Create a CameraTracker3D.
        /// </summary>
        /// <param name="game">The game.</param>
        public CameraTracker(XiGame game) : base(game, false) { }

        /// <inheritdoc />
        protected override void PlayHook(GameTime gameTime)
        {
            base.PlayHook(gameTime);
            Vector3 cameraPosition = Actor.Position + Vector3.Up * 10 + Actor.OrientationMatrix.Backward * 40;
            Vector3 cameraLookTarget = cameraPosition + Actor.OrientationMatrix.Forward * 10;
            Game.Camera.SetTransformByLookTarget(cameraPosition, Vector3.Up, cameraLookTarget);
        }
    }
}
