using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// The surface of water in 3D.
    /// TODO: give a proper bounding box.
    /// </summary>
    public class WaterSurface : Surface<Water>
    {
        /// <summary>
        /// Create a WaterSurface.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="actor">The parent actor.</param>
        public WaterSurface(XiGame game, Water actor)
            : base(game, actor, "Xi/3D/XiWater")
        {
            reflectionMapTarget = new ManagedRenderTarget2D(game, Constants.WaterReflectionMapSurfaceToScreenRatio, 1, SurfaceFormat.Color, MultiSampleType.None, 0, 0);
            geometry = QuadGeometry.Create<VerticesPositionNormalTexture>(game.GraphicsDevice);
            DrawStyle = DrawStyle.Transparent;
            DrawProperties = DrawProperties.Reflecting;
            WaveMap0FileName = "Xi/3D/waves";
            WaveMap1FileName = "Xi/3D/waves";
        }

        /// <summary>
        /// The scale of the water surface on the x, z plane.
        /// </summary>
        public Vector2 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// The velocity of the 0th wave normal map.
        /// </summary>
        public Vector2 WaveMap0Velocity
        {
            get { return waveMap0Velocity; }
            set { waveMap0Velocity = value; }
        }
        
        /// <summary>
        /// The velocity of the 1st wave normal map.
        /// </summary>
        public Vector2 WaveMap1Velocity
        {
            get { return waveMap1Velocity; }
            set { waveMap1Velocity = value; }
        }
        
        /// <summary>
        /// The multiplier of the water's color.
        /// </summary>
        public Color ColorMultiplier
        {
            get { return colorMultiplier; }
            set { colorMultiplier = value; }
        }
        
        /// <summary>
        /// The color of the water.
        /// </summary>
        public Color ColorAdditive
        {
            get { return colorAdditive; }
            set { colorAdditive = value; }
        }
        
        /// <summary>
        /// The name of the 0th wave normal map.
        /// </summary>
        public string WaveMap0FileName
        {
            get { return _waveMap0FileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (_waveMap0FileName == value) return; // OPTIMIZATION
                Texture2D newWaveMap0 = Game.Content.Load<Texture2D>(value);
                _waveMap0 = newWaveMap0;
                _waveMap0FileName = value;
            }
        }
        
        /// <summary>
        /// The name of the 1st wave normal map.
        /// </summary>
        public string WaveMap1FileName
        {
            get { return _waveMap1FileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (_waveMap1FileName == value) return; // OPTIMIZATION
                Texture2D newWaveMap1 = Game.Content.Load<Texture2D>(value);
                _waveMap1 = newWaveMap1;
                _waveMap1FileName = value;
            }
        }
        
        /// <summary>
        /// The length of the waves.
        /// </summary>
        public float WaveLength
        {
            get { return waveLength; }
            set { waveLength = value; }
        }
        
        /// <summary>
        /// The height of the waves.
        /// </summary>
        public float WaveHeight
        {
            get { return waveHeight; }
            set { waveHeight = value; }
        }

        /// <inheritdoc />
        protected override BoundingBox BoundingBoxHook
        {
            get
            {
                Matrix world = WaterWorld;
                return new BoundingBox(
                    world.Left + world.Forward + world.Translation,
                    world.Right + world.Backward + world.Translation);
            }
        }

        /// <inheritdoc />
        protected override bool BoundlessHook { get { return false; } }

        /// <inheritdoc />
        protected override Effect EffectHook { get { return _effect; } }

        /// <inheritdoc />
        protected override string EffectFileNameHook
        {
            get { return _effectFileName; }
            set
            {
                if (_effectFileName == value) return; // OPTIMIZATION
                Effect newEffect = CreateEffect(value);
                _effect = newEffect;
                _effectFileName = value;
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                reflectionMapTarget.Dispose();
                geometry.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override void PreDrawHook(GameTime gameTime, Camera camera)
        {
            // grab subsystems
            GraphicsDevice device = Game.GraphicsDevice;
            Scene scene = Game.Scene;

            // calculate water height variables
            float waterHeight = Actor.Position.Y;
            float waterHeightTimesTwo = waterHeight * 2;

            // cache off original camera transform
            Vector3 originalPosition = camera.Position;
            Vector3 originalLookUp = camera.LookUp;
            Vector3 originalLookForward = camera.LookForward;

            // find reflected camera transform
            Vector3 reflectedPosition = new Vector3(camera.Position.X, waterHeightTimesTwo - camera.Position.Y, camera.Position.Z);
            Vector3 reflectedLookTarget = new Vector3(camera.LookTarget.X, waterHeightTimesTwo - camera.LookTarget.Y, camera.LookTarget.Z);

            // reflect camera
            camera.SetTransformByLookTarget(reflectedPosition, Vector3.Up, reflectedLookTarget);
            {
                // update the reflection view
                camera.GetView(out reflectionView);

                // create the reflection plane
                Plane reflectionClipPlane = WaterHelper.CreateWaterReflectionPlane(camera, waterHeight);

                // organize the surfaces for reflection drawing
                OrganizeSurfaces(camera);

                // draw the reflection map
                reflectionMapTarget.Activate();
                {
                    // clear the water reflection target
                    device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.Black, 1, 0);

                    // draw the objects whose drawing position is dependent on the reflection camera's
                    // position - such as a skybox - before you set the clip plane!
                    // Otherwise the object is drawn around the reflecting camera and might therefore
                    // be underneath the clip plane. This would cause properly visible objects to be
                    // clipped.
                    DrawSurfaces(
                        gameTime,
                        camera,
                        cachedDependantPriors,
                        cachedDependantOpaques,
                        cachedDependantTransparents,
                        scene.DrawOpaquesNearToFar);

                    // set the clip planes on the water reflection target
                    device.ClipPlanes[0].IsEnabled = true;
                    device.ClipPlanes[0].Plane = reflectionClipPlane;

                    // draw all the camera-independant surfaces
                    DrawSurfaces(gameTime, camera, cachedPriors, cachedOpaques, cachedTransparents, scene.DrawOpaquesNearToFar);

                    // disable the clip planes
                    device.ClipPlanes[0].IsEnabled = false;
                }
                // resolve the reflection map
                reflectionMapTarget.Resolve();
            }
            // restore camera
            camera.SetTransformByLookForward(originalPosition, originalLookUp, originalLookForward);

            //reflectionMapTarget.VolatileTexture.Save("waterReflectionMap.png", ImageFileFormat.Png);
        }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime, Camera camera, string drawMode)
        {
            if (drawMode != "Normal") return;
            PopulateEffect(gameTime, camera);
            BeginRenderState();
            BeginEffect();
            DrawGeometryPasses(gameTime);
            EndEffect();
            EndRenderState();
        }

        private Matrix WaterWorld
        {
            get
            {
                Matrix world = Actor.WorldTransform;
                world.M11 = scale.X;
                world.M33 = scale.Y;
                return world;
            }
        }

        private Texture2D WaveMap0 { get { return _waveMap0; } }

        private Texture2D WaveMap1 { get { return _waveMap1; } }

        private void OrganizeSurfaces(Camera camera)
        {
            ClearSurfaces();
            PopulateSurfaces(camera);
        }

        private void ClearSurfaces()
        {
            cachedDependantPriors.Clear();
            cachedDependantOpaques.Clear();
            cachedDependantTransparents.Clear();
            cachedPriors.Clear();
            cachedOpaques.Clear();
            cachedTransparents.Clear();
        }

        private void PopulateSurfaces(Camera camera)
        {
            foreach (Surface surface in Game.Scene.CachedSurfaces)
            {
                if (IsReflecting(camera, surface) && surface != this)
                {
                    if (surface.HasDrawProperties(DrawProperties.DependantTransform)) OrganizeDependantSurface(surface);
                    else OrganizeSurface(surface);
                }
            }
        }

        private void OrganizeDependantSurface(Surface surface)
        {
            switch (surface.DrawStyle)
            {
                case DrawStyle.Prioritized: cachedDependantPriors.Add(surface); break;
                case DrawStyle.Opaque: cachedDependantOpaques.Add(surface); break;
                case DrawStyle.Transparent: cachedDependantTransparents.Add(surface); break;
            }
        }

        private void OrganizeSurface(Surface surface)
        {
            switch (surface.DrawStyle)
            {
                case DrawStyle.Prioritized: cachedPriors.Add(surface); break;
                case DrawStyle.Opaque: cachedOpaques.Add(surface); break;
                case DrawStyle.Transparent: cachedTransparents.Add(surface); break;
            }
        }

        private void DrawSurfaces(GameTime gameTime, Camera camera, List<Surface> priors,
            List<Surface> opaques, List<Surface> transparents, bool drawOpaquesNearToFar)
        {
            DrawPriors(gameTime, camera, priors);
            DrawOpaques(gameTime, camera, opaques, drawOpaquesNearToFar);
            DrawTransparents(gameTime, camera, transparents);
        }

        private void DrawPriors(GameTime gameTime, Camera camera, List<Surface> priors)
        {
            SurfeceDrawHelper.PrioritySort(priors);
            foreach (Surface surface in priors) surface.Draw(gameTime, camera, "Normal");
        }

        private void DrawOpaques(GameTime gameTime, Camera camera, List<Surface> opaques, bool drawOpaquesNearToFar)
        {
            if (drawOpaquesNearToFar) SurfeceDrawHelper.DistanceSort(opaques, camera.Position, SpatialSortOrder.NearToFar);
            foreach (Surface surface in opaques) surface.Draw(gameTime, camera, "Normal");
        }

        private void DrawTransparents(GameTime gameTime, Camera camera, List<Surface> transparents)
        {
            SurfeceDrawHelper.DistanceSort(transparents, camera.Position, SpatialSortOrder.FarToNear);
            foreach (Surface surface in transparents) surface.Draw(gameTime, camera, "Normal");
        }

        private Effect CreateEffect(string effectFileName)
        {
            Effect effectFromDisk = Game.Content.Load<Effect>(effectFileName);
            return new LightReceiverEffect(Game.GraphicsDevice, effectFromDisk);
        }

        private void PopulateEffect(GameTime gameTime, Camera camera)
        {
            LightReceiverEffect lrEffect = XiHelper.Cast<LightReceiverEffect>(Effect);
            // OPTIMIZATION: cache these properties
            Scene scene = Game.Scene;
            Texture2D reflectionMap = reflectionMapTarget.VolatileTexture;
            if (reflectionMap == null) return;
            float totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
            Matrix world = WaterWorld;
            lrEffect.TrySetCurrentTechnique("Normal");
            lrEffect.Parameters["xWaterColorMultiplier"].TrySetValue(ColorMultiplier.ToVector4());
            lrEffect.Parameters["xWaterColorAdditive"].TrySetValue(ColorAdditive.ToVector4());
            lrEffect.Parameters["xReflectionMap"].TrySetValue(reflectionMap);
            lrEffect.Parameters["xReflectionView"].TrySetValue(reflectionView);
            lrEffect.Parameters["xWaveOffset0"].TrySetValue(waveMap0Velocity * totalTime);
            lrEffect.Parameters["xWaveOffset1"].TrySetValue(waveMap1Velocity * totalTime);
            lrEffect.Parameters["xWaveLength"].TrySetValue(waveLength);
            lrEffect.Parameters["xWaveHeight"].TrySetValue(waveHeight);
            lrEffect.Parameters["xWaveMap0"].TrySetValue(WaveMap0);
            lrEffect.Parameters["xWaveMap1"].TrySetValue(WaveMap1);
            lrEffect.PopulateTransform(camera, ref world);
            lrEffect.PopulateFogging(scene.Fog);
            lrEffect.PopulateLighting(this, scene.CachedAmbientLights, scene.CachedDirectionalLights, scene.CachedPointLights);
        }

        private void BeginEffect()
        {
            Effect.Begin();
        }

        private void EndEffect()
        {
            Effect.End();
        }

        private void DrawGeometryPasses(GameTime gameTime)
        {
            foreach(EffectPass pass in Effect.CurrentTechnique.Passes)
                DrawGeometryPass(gameTime, pass);
        }

        private void DrawGeometryPass(GameTime gameTime, EffectPass pass)
        {
            pass.Begin();
            geometry.Draw(gameTime);
            pass.End();
        }

        private void BeginRenderState()
        {
            GraphicsDevice device = Game.GraphicsDevice;
            device.BeginFaceMode(FaceMode);
            if (DrawStyle == DrawStyle.Transparent)
            {
                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.SourceBlend = Blend.SourceAlpha;
                device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            }
        }

        private void EndRenderState()
        {
            GraphicsDevice device = Game.GraphicsDevice;
            if (DrawStyle == DrawStyle.Transparent)
            {
                device.RenderState.DestinationBlend = Blend.Zero;
                device.RenderState.SourceBlend = Blend.One;
                device.RenderState.AlphaBlendEnable = false;
            }
            device.EndFaceMode();
        }

        private static bool IsReflecting(Camera camera, Surface surface)
        {
            return
                surface.HasDrawProperties(DrawProperties.Reflecting) &&
                (
                    surface.Boundless ||
                    camera.Contains(surface.BoundingBox) != ContainmentType.Disjoint
                );
        }
        
        private readonly List<Surface> cachedDependantPriors = new List<Surface>();
        private readonly List<Surface> cachedDependantOpaques = new List<Surface>();
        private readonly List<Surface> cachedDependantTransparents = new List<Surface>();
        private readonly List<Surface> cachedPriors = new List<Surface>();
        private readonly List<Surface> cachedOpaques = new List<Surface>();
        private readonly List<Surface> cachedTransparents = new List<Surface>();
        /// <summary>May be null.</summary>
        private readonly ManagedRenderTarget2D reflectionMapTarget;
        private readonly Geometry geometry;
        private Vector2 waveMap0Velocity = new Vector2(0.07f, 0);
        private Vector2 waveMap1Velocity = new Vector2(0, 0.1f);
        private Vector2 scale = new Vector2(4096);
        private Matrix reflectionView;
        private Color colorAdditive = new Color(100, 100, 100, 0);
        private Color colorMultiplier = new Color(0, 0, 128, 160);
        private float waveLength = 0.0125f;
        private float waveHeight = 0.0125f;
        private Texture2D _waveMap0;
        private Texture2D _waveMap1;
        private Effect _effect;
        private string _effectFileName;
        private string _waveMap0FileName;
        private string _waveMap1FileName;
    }
}
