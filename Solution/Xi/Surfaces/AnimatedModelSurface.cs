using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Xi
{
    /// <summary>
    /// The surface of an AnimatedModel.
    /// TODO: improve bounding box derivation.
    /// </summary>
    public class AnimatedModelSurface : Surface<AnimatedModel>
    {
        /// <summary>
        /// Create an AnimatedModelSurface.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="actor">The parent actor.</param>
        public AnimatedModelSurface(XiGame game, AnimatedModel actor)
            : base(game, actor, string.Empty)
        {
            boundingBoxBuilder = new BoundingBoxBuilder(GenerateModelBoundingBox(actor));
        }

        /// <summary>
        /// The material's emissive color.
        /// TODO: consider adding this to standard shader.
        /// </summary>
        public Color EmissiveColor
        {
            get { return emissiveColor; }
            set { emissiveColor = value; }
        }

        /// <summary>
        /// Use normal mapping?
        /// </summary>
        public bool NormalMapEnabled
        {
            get { return normalMapEnabled; }
            set { normalMapEnabled = value; }
        }

        /// <inheritdoc />
        protected override BoundingBox BoundingBoxHook
        {
            get
            {
                boundingBoxBuilder.WorldTransform = Actor.WorldTransform;
                return boundingBoxBuilder.BoundingBoxWorld;
            }
        }

        /// <inheritdoc />
        protected override bool BoundlessHook { get { return false; } }

        /// <inheritdoc />
        protected override Effect EffectHook
        {
            get { return Actor.SkinnedModel.Model.Meshes[0].MeshParts[0].Effect; }
        }

        /// <inheritdoc />
        protected override string EffectFileNameHook
        {
            get { return string.Empty; }
            set { }
        }

        /// <inheritdoc />
        protected override void PreDrawHook(GameTime gameTime, Camera camera) { }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime, Camera camera, string drawMode)
        {
            if (drawMode != "Normal" && drawMode != "DirectionalShadow") return;
            GraphicsDevice device = Game.GraphicsDevice;
            BeginRenderState(device);
            DrawSkinnedModelMeshes(camera, drawMode, Actor.AnimationController, Actor.SkinnedModel);
            EndRenderState(device);
        }

        private void DrawSkinnedModelMeshes(Camera camera, string drawMode, IAnimationController animationController, SkinnedModel skinnedModel)
        {
            foreach (ModelMesh modelMesh in skinnedModel.Model.Meshes)
                DrawSkinnedModelMeshParts(camera, drawMode, animationController, modelMesh);
        }

        private void DrawSkinnedModelMeshParts(Camera camera, string drawMode, IAnimationController animationController, ModelMesh modelMesh)
        {
            foreach (ModelMeshPart modelMeshPart in modelMesh.MeshParts)
                DrawSkinnedModelMeshPart(camera, drawMode, animationController, modelMesh, modelMeshPart);
        }

        private void DrawSkinnedModelMeshPart(Camera camera, string drawMode, IAnimationController animationController, ModelMesh modelMesh, ModelMeshPart modelMeshPart)
        {
            // OPTIMIZATION - cache variables
            Scene scene = Game.Scene;
            Fog fog = scene.Fog;
            SkinnedModelBasicEffect effect = XiHelper.Cast<SkinnedModelBasicEffect>(modelMeshPart.Effect);

            // world, view, and projection
            effect.World = Actor.WorldTransform;
            effect.View = camera.View;
            effect.Projection = camera.Projection;

            // bones
            effect.Bones = animationController.SkinnedBoneTransforms;

            // camera position
            effect.CameraPosition = camera.Position;

            // draw mode
            effect.DrawMode = drawMode;

            // draw 'normally'
            if (drawMode == "Normal")
            {
                // material
                effect.Material.EmissiveColor = EmissiveColor.ToVector3();
                effect.Material.DiffuseColor = DiffuseColor.ToVector3();
                effect.Material.SpecularColor = SpecularColor.ToVector3();
                effect.Material.SpecularPower = SpecularPower;

                // normal mapping
                effect.NormalMapEnabled = normalMapEnabled;

                // fogging
                effect.FogEnabled = fog.Enabled;

                if (fog.Enabled) // OPTIMIZATION
                {
                    effect.FogStart = fog.Start;
                    effect.FogEnd = fog.End;
                    effect.FogColor = fog.Color;
                }

                // lighting enabled
                effect.LightEnabled = LightingEnabled;

                // light count
                effect.EnabledLights = skinnedModelEnabledLights;

                // ambient light
                Vector3 ambientLightColor = Vector3.Zero;
                foreach (AmbientLight ambientLight in scene.CachedAmbientLights)
                    if (ambientLight.Enabled)
                        ambientLightColor += ambientLight.Color.ToVector3();
                effect.AmbientLightColor = ambientLightColor;

                // directional lights emulated as point lights
                for (
                    int i = 0;
                    i < Constants.DirectionalLightCount &&
                    i < intSkinnedModelEnabledsLights;
                    ++i)
                {
                    XNAnimation.Effects.PointLight effectPointLight = effect.PointLights[i];
                    if (i >= scene.CachedDirectionalLights.Count)
                    {
                        effectPointLight.Color = Vector3.Zero;
                        effectPointLight.Position = Vector3.Zero;
                    }
                    else
                    {
                        DirectionalLight directionalLight = scene.CachedDirectionalLights[i];
                        Vector3 emulatedPosition = -directionalLight.Direction * emulatedPointLightDistance;
                        Vector3 emulatedColor = (directionalLight.DiffuseColor.ToVector3() + directionalLight.SpecularColor.ToVector3()) * 0.5f;
                        effectPointLight.Color = emulatedColor;
                        effectPointLight.Position = emulatedPosition;
                    }
                }

                // point lights
                scene.CachedPointLights.DistanceSort(BoundingBox.GetCenter(), SpatialSortOrder.NearToFar);
                for (
                    int i = 0;
                    i < Constants.PointLightCount &&
                    i + scene.CachedDirectionalLights.Count < intSkinnedModelEnabledsLights;
                    ++i)
                {
                    XNAnimation.Effects.PointLight effectPointLight = effect.PointLights[i + scene.CachedDirectionalLights.Count];
                    if (i >= scene.CachedPointLights.Count)
                    {
                        effectPointLight.Color = Vector3.Zero;
                        effectPointLight.Position = Vector3.Zero;
                    }
                    else
                    {
                        PointLight pointLight = scene.CachedPointLights[i];
                        Vector3 emulatedColor = (pointLight.DiffuseColor.ToVector3() + pointLight.SpecularColor.ToVector3()) * 0.5f;
                        effectPointLight.Color = emulatedColor;
                        effectPointLight.Position = pointLight.Position;
                    }
                }
            }

            // draw the mesh
            modelMesh.Draw();
        }

        private void BeginRenderState(GraphicsDevice device)
        {
            device.BeginFaceMode(FaceMode);
        }

        private void EndRenderState(GraphicsDevice device)
        {
            device.EndFaceMode();
        }

        private static BoundingBox GenerateModelBoundingBox(AnimatedModel actor)
        {
            BoundingBox modelBoundingBox = actor.SkinnedModel.Model.GenerateBoundingBox();
            BoundingBox doubleBoundingBox = new BoundingBox(
                modelBoundingBox.GetCenter() - modelBoundingBox.GetExtent(),
                modelBoundingBox.GetCenter() + modelBoundingBox.GetExtent());
            return doubleBoundingBox;
        }
        
        private const EnabledLights skinnedModelEnabledLights = EnabledLights.Eight;
        private const float emulatedPointLightDistance = 100000;
        private const float emulatedPointLightRange = emulatedPointLightDistance * 2;
        private const int intSkinnedModelEnabledsLights = 8;
        private readonly BoundingBoxBuilder boundingBoxBuilder;
        private Color emissiveColor;
        private bool normalMapEnabled = true;
    }
}
