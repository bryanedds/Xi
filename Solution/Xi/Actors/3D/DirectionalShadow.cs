using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A vanilla implementation of IDirectionalShadow.
    /// </summary>
    public class DirectionalShadow : Disposable, IDirectionalShadow
    {
        /// <summary>
        /// Create a DirectionalShadow.
        /// </summary>
        public DirectionalShadow(XiGame game, OrthoCamera shadowCamera)
        {
            XiHelper.ArgumentNullCheck(game, shadowCamera);
            this.game = game;
            this.shadowCamera = shadowCamera;
            shadowMapTarget = new ManagedRenderTarget2D(game, Constants.DirectionalShadowMapSurfaceToScreenRatio, 1, SurfaceFormat.Single, MultiSampleType.None, 0, 0);
            screenQuad = ScreenQuadGeometry.Create<VerticesPositionNormalTexture>(game.GraphicsDevice);
        }

        /// <inheritdoc />
        public Texture2D VolatileShadowMap { get { return shadowMapTarget.VolatileTexture; } }

        /// <inheritdoc />
        public OrthoCamera Camera { get { return shadowCamera; } }

        /// <inheritdoc />
        public void Draw(GameTime gameTime)
        {
            XiHelper.ArgumentNullCheck(gameTime);
            DrawShadowMap(gameTime);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                screenQuad.Dispose();
                shadowMapTarget.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DrawShadowMap(GameTime gameTime)
        {
            List<Surface> surfaces = game.Scene.CachedSurfaces;
            shadowMapTarget.Activate();
            game.GraphicsDevice.Clear(Color.White);
            foreach (Surface surface in surfaces)
                if (IsShadowing(shadowCamera, surface))
                    surface.Draw(gameTime, shadowCamera, "DirectionalShadow");
            shadowMapTarget.Resolve();
            //shadowMapTarget.VolatileTexture.Save("directionalShadowMap.png", ImageFileFormat.Png);
        }

        private static bool IsShadowing(Camera shadowCamera, Surface surface)
        {
            return
                surface.HasDrawProperties(DrawProperties.Shadowing) &&
                (
                    surface.Boundless ||
                    shadowCamera.Contains(surface.BoundingBox) != ContainmentType.Disjoint
                );
        }

        private readonly ManagedRenderTarget2D shadowMapTarget;
        private readonly XiGame game;
        private readonly Geometry screenQuad;
        private readonly OrthoCamera shadowCamera;
    }
}
