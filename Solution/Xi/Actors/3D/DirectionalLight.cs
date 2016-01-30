using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A 3D directional light.
    /// </summary>
    public class DirectionalLight : Light
    {
        /// <summary>
        /// Create a DirectionalLight3D with an orthoganol shadow.
        /// </summary>
        /// <param name="game">The game.</param>
        public DirectionalLight(XiGame game)
            : base(game)
        {
            shadow = new NullDirectionalShadow(game);
        }

        /// <summary>
        /// The shadow camera.
        /// </summary>
        [Browsable(false)]
        public OrthoCamera ShadowCamera { get { return shadow.Camera; } }

        /// <summary>
        /// The rendered shadow map.
        /// May be null.
        /// </summary>
        [Browsable(false)]
        public Texture2D VolatileShadowMap { get { return shadow.VolatileShadowMap; } }

        /// <summary>
        /// The direction of the light.
        /// </summary>
        public Vector3 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        /// <summary>
        /// The position of the shadow camera. Used only if ShadowCameraRelativeToSceneCamera is
        /// false.
        /// </summary>
        public Vector3 ShadowCameraPosition
        {
            get { return shadowCameraPosition; }
            set { shadowCameraPosition = value; }
        }

        /// <summary>
        /// The diffuse color of the light.
        /// </summary>
        public Color DiffuseColor
        {
            get { return diffuseColor; }
            set { diffuseColor = value; }
        }

        /// <summary>
        /// The specular color of the light.
        /// </summary>
        public Color SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; }
        }

        /// <summary>
        /// The offset of the shadow camera in the direction of the scene camera's backward vector.
        /// Used only if ShadowCameraRelativeToSceneCamera is true.
        /// </summary>
        public float ShadowCameraOffset
        {
            get { return shadowCameraOffset; }
            set { shadowCameraOffset = value; }
        }

        /// <summary>
        /// The interval to which the relative shadow camera snaps.
        /// </summary>
        public float ShadowCameraSnap
        {
            get { return shadowCameraSnap; }
            set { shadowCameraSnap = value; }
        }

        public bool ShadowEnabled
        {
            get { return shadow is DirectionalShadow; }
            set
            {
                if (ShadowEnabled == value) return; // OPTIMIZATION
                shadow.Dispose();
                shadow = value ? CreateDirectionalShadow() : CreateNullDirectionalShadow();
            }
        }

        /// <summary>
        /// Is the position of the shadow camera relative to the scene camera?
        /// </summary>
        public bool ShadowCameraRelativeToViewCamera
        {
            get { return shadowCameraRelativeToViewCamera; }
            set { shadowCameraRelativeToViewCamera = value; }
        }

        /// <summary>
        /// Draw the shadow map.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the scene is viewed.</param>
        public void DrawShadow(GameTime gameTime, Camera camera)
        {
            XiHelper.ArgumentNullCheck(gameTime, camera);
            ConfigureShadowCamera(camera);
            shadow.Draw(gameTime);
        }

        /// <inheritdoc />
        protected override void Destroy(bool destroying)
        {
            if (destroying) shadow.Dispose();
            base.Destroy(destroying);
        }

        private IDirectionalShadow CreateNullDirectionalShadow()
        {
            return new NullDirectionalShadow(Game);
        }

        private IDirectionalShadow CreateDirectionalShadow()
        {
            OrthoCamera shadowCamera = new OrthoCamera(Game.GraphicsDevice)
            {
                Width = Constants.DirectionalShadowSize.X,
                Height = Constants.DirectionalShadowSize.Y,
                NearPlane = 0,
                FarPlane = Constants.DirectionalShadowRange
            };
            return new DirectionalShadow(Game, shadowCamera);
        }

        private void ConfigureShadowCamera(Camera camera)
        {
            ConfigureShadowCameraView(camera);
        }

        private void ConfigureShadowCameraView(Camera camera)
        {
            Vector3 position = CalculateShadowCameraPosition(camera), right, up;
            direction.GetComplimentaryOrientationVectors(out up, out right);
            shadow.Camera.SetTransformByLookForward(position, up, direction);
        }

        private Vector3 CalculateShadowCameraPosition(Camera camera)
        {
            if (!shadowCameraRelativeToViewCamera) return shadowCameraPosition;
            Vector3 cameraPosition = camera.Position - direction * shadowCameraOffset;
            return shadowCameraSnap != 0 ? cameraPosition.GetSnap(shadowCameraSnap) : cameraPosition;
        }

        private IDirectionalShadow shadow;
        private Vector3 shadowCameraPosition = Vector3.Up * Constants.DirectionalShadowRange * 0.5f;
        private Vector3 direction = Vector3.Down;
        private Color diffuseColor = Color.Gray;
        private Color specularColor = Color.Gray;
        private float shadowCameraOffset = Constants.DirectionalShadowRange * 0.5f;
        private float shadowCameraSnap = Constants.DirectionalShadowCameraSnap;
        private bool shadowCameraRelativeToViewCamera = true;
    }
}
