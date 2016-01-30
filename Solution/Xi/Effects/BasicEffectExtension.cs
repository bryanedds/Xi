using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// An extension method class for BasicEffect.
    /// </summary>
    public static class BasicEffectExtension
    {
        /// <summary>
        /// Populate the transform parameters of a BasicEffect.
        /// </summary>
        /// <param name="camera">The camera from which to extract the view and projection.</param>
        /// <param name="world">The world transform.</param>
        /// <param name="effect">The effect to populate.</param>
        public static void PopulateTransform(this BasicEffect effect, Camera camera, ref Matrix world)
        {
            XiHelper.ArgumentNullCheck(camera);
            Matrix view;
            Matrix projection;
            camera.GetView(out view);
            camera.GetProjection(out projection);
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;
        }

        /// <summary>
        /// Populate the lighting parameters of a BasicEffect.
        /// </summary>
        /// <param name="effect">The effect.</param>
        /// <param name="surface">The lit surface.</param>
        /// <param name="ambientLights">The ambient lights affecting the lit material.</param>
        /// <param name="directionalLights">The directional lights affecting the lit material.</param>
        /// <param name="pointLights">The point lights affecting the lit material.</param>
        public static void PopulateLighting(
            this BasicEffect effect,
            Surface surface,
            List<AmbientLight> ambientLights,
            List<DirectionalLight> directionalLights,
            List<PointLight> pointLights)
        {
            XiHelper.ArgumentNullCheck(surface, ambientLights, directionalLights, pointLights, effect);

            // lighting enabled
            effect.LightingEnabled = surface.LightingEnabled;

            // per-pixel lighting
            effect.PreferPerPixelLighting = true; // MAGICVALUE

            // material
            effect.DiffuseColor = surface.DiffuseColor.ToVector3();
            effect.SpecularColor = surface.SpecularColor.ToVector3();
            effect.SpecularPower = surface.SpecularPower;

            // ambient lighting
            Vector3 ambientLightColor = Vector3.Zero;
            foreach (AmbientLight ambientLight in ambientLights)
                if (ambientLight.Enabled)
                    ambientLightColor += ambientLight.Color.ToVector3();
            effect.AmbientLightColor = ambientLightColor;

            // directional lights
            for (int i = 0; i < Constants.DirectionalLightCount; ++i)
            {
                BasicDirectionalLight effectLight;
                switch (i)
                {
                    case 0: effectLight = effect.DirectionalLight0; break;
                    case 1: effectLight = effect.DirectionalLight1; break;
                    case 2: effectLight = effect.DirectionalLight2; break;
                    default: continue;
                }

                if (i >= directionalLights.Count) effectLight.Enabled = false;
                else
                {
                    DirectionalLight directionalLight = directionalLights[i];
                    effectLight.Enabled = directionalLight.Enabled;
                    effectLight.DiffuseColor = directionalLight.DiffuseColor.ToVector3();
                    effectLight.SpecularColor = directionalLight.SpecularColor.ToVector3();
                    effectLight.Direction = directionalLight.Direction;
                }
            }

            // point lights emulated as directional lights
            Vector3 surfaceCenter = surface.BoundingBox.GetCenter();
            pointLights.DistanceSort(surfaceCenter, SpatialSortOrder.NearToFar);
            for (int i = 0; i < Constants.PointLightCount; ++i)
            {
                BasicDirectionalLight effectLight;
                switch (i + directionalLights.Count)
                {
                    case 0: effectLight = effect.DirectionalLight0; break;
                    case 1: effectLight = effect.DirectionalLight1; break;
                    case 2: effectLight = effect.DirectionalLight2; break;
                    default: continue;
                }

                if (i >= pointLights.Count) effectLight.Enabled = false;
                else
                {
                    PointLight pointLight = pointLights[i];
                    effectLight.Enabled = pointLight.Enabled;
                    effectLight.DiffuseColor = pointLight.DiffuseColor.ToVector3();
                    effectLight.SpecularColor = pointLight.SpecularColor.ToVector3();
                    effectLight.Direction = Vector3.Normalize(surface.BoundingBox.GetCenter() - pointLight.Position);
                }
            }
        }

        /// <summary>
        /// Populate the fogging parameters of a BasicEffect.
        /// </summary>
        public static void PopulateFogging(this BasicEffect basicEffect, Fog fog)
        {
            basicEffect.FogEnabled = fog.Enabled;
            if (!fog.Enabled) return; // OPTIMIZATION
            basicEffect.FogStart = fog.Start;
            basicEffect.FogEnd = fog.End;
            basicEffect.FogColor = fog.Color.ToVector3();
        }
    }
}
