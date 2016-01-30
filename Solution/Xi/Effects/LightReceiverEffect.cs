using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Represents a shader effect that draws lit geometry.
    /// </summary>
    public class LightReceiverEffect : FogReceiverEffect
    {
        /// <summary>
        /// Initializes a new instance of LightReceiverEffect by cloning an existing effect.
        /// </summary>
        /// <param name="device">The graphics device that will create the effect.</param>
        /// <param name="cloneSource">The effect to clone.</param>
        public LightReceiverEffect(GraphicsDevice device, Effect cloneSource)
            : base(device, cloneSource)
        {
            lightingEnabledParam = Parameters["xLightingEnabled"];
            diffuseColorParam = Parameters["xDiffuseColor"];
            specularColorParam = Parameters["xSpecularColor"];
            specularPowerParam = Parameters["xSpecularPower"];
            ambientLightEnabledParam = Parameters["xAmbientLightEnabled"];
            ambientLightColorParam = Parameters["xAmbientLightColor"];
            directionalLightEnabledsParam = Parameters["xDirectionalLightEnableds"];
            directionalLightDirectionsParam = Parameters["xDirectionalLightDirections"];
            directionalLightDiffuseColorsParam = Parameters["xDirectionalLightDiffuseColors"];
            directionalLightSpecularColorsParam = Parameters["xDirectionalLightSpecularColors"];
            pointLightEnabledsParam = Parameters["xPointLightEnableds"];
            pointLightPositionsParam = Parameters["xPointLightPositions"];
            pointLightDiffuseColorsParam = Parameters["xPointLightDiffuseColors"];
            pointLightSpecularColorsParam = Parameters["xPointLightSpecularColors"];
            pointLightRangesParam = Parameters["xPointLightRanges"];
            pointLightFalloffsParam = Parameters["xPointLightFalloffs"];
        }

        /// <summary>
        /// Is the object affected by light?
        /// </summary>
        public bool LightingEnabled
        {
            get { return _lightingEnabled; }
            set
            {
                if (_lightingEnabled == value) return; // OPTIMIZATION
                _lightingEnabled = value;
                lightingEnabledParam.TrySetValue(value);
            }
        }
        
        /// <summary>
        /// The diffuse color of the object's material.
        /// </summary>
        public Vector4 DiffuseColor
        {
            get { return _diffuseColor; }
            set
            {
                if (_diffuseColor == value) return; // OPTIMIZATION
                _diffuseColor = value;
                diffuseColorParam.TrySetValue(value);
            }
        }
        
        /// <summary>
        /// The specular color of the object's material.
        /// </summary>
        public Vector3 SpecularColor
        {
            get { return _specularColor; }
            set
            {
                if (_specularColor == value) return; // OPTIMIZATION
                _specularColor = value;
                specularColorParam.TrySetValue(value);
            }
        }
        
        /// <summary>
        /// The specular power of the object's material.
        /// </summary>
        public float SpecularPower
        {
            get { return _specularPower; }
            set
            {
                if (_specularPower == value) return; // OPTIMIZATION
                _specularPower = value;
                specularPowerParam.TrySetValue(value);
            }
        }
        
        /// <summary>
        /// Is the object affected by ambient light?
        /// </summary>
        public bool AmbientLightEnabled
        {
            get { return _ambientLightEnabled; }
            set
            {
                if (_ambientLightEnabled == value) return; // OPTIMIZATION
                _ambientLightEnabled = value;
                ambientLightEnabledParam.TrySetValue(value);
            }
        }
        
        /// <summary>
        /// The color of the ambient light.
        /// </summary>
        public Vector3 AmbientLightColor
        {
            get { return _ambientLightColor; }
            set
            {
                if (_ambientLightColor == value) return; // OPTIMIZATION
                _ambientLightColor = value;
                ambientLightColorParam.TrySetValue(value);
            }
        }

        /// <summary>
        /// Set a bool array of size Constants.DirectionalLightCount that tells if each
        /// directional light is enabled.
        /// </summary>
        public void SetDirectionalLightEnableds(bool[] enableds)
        {
            if (_directionalLightEnableds.EqualsValue(enableds)) return; // OPTIMIZATION
            _directionalLightEnableds.CopyFrom(enableds, enableds.Length);
            directionalLightEnabledsParam.TrySetValue(_directionalLightEnableds);
        }

        /// <summary>
        /// Set a Vector3 array of size Constants.DirectionalLightCount that tells of each
        /// directional light's direction.
        /// </summary>
        public void SetDirectionalLightDirections(Vector3[] directions)
        {
            if (_directionalLightDirections.EqualsValue(directions)) return; // OPTIMIZATION
            _directionalLightDirections.CopyFrom(directions, directions.Length);
            directionalLightDirectionsParam.TrySetValue(_directionalLightDirections);
        }

        /// <summary>
        /// Set a Vector3 array of size Constants.DirectionalLightCount that tells of each
        /// directional light's diffuse color.
        /// </summary>
        public void SetDirectionalLightDiffuseColors(Vector3[] colors)
        {
            if (_directionalLightDiffuseColors.EqualsValue(colors)) return; // OPTIMIZATION
            _directionalLightDiffuseColors.CopyFrom(colors, colors.Length);
            directionalLightDiffuseColorsParam.TrySetValue(_directionalLightDiffuseColors);
        }

        /// <summary>
        /// Set a Color array of size Constants.DirectionalLightCount that tells of each
        /// directional light's specular color.
        /// </summary>
        public void SetDirectionalLightSpecularColors(Vector3[] colors)
        {
            if (_directionalLightSpecularColors.EqualsValue(colors)) return; // OPTIMIZATION
            _directionalLightSpecularColors.CopyFrom(colors, colors.Length);
            directionalLightSpecularColorsParam.TrySetValue(_directionalLightSpecularColors);
        }

        /// <summary>
        /// Sets a bool array of size Constants.PointLightCount that tells is each point
        /// light diffuse is enabled.
        /// </summary>
        public void SetPointLightEnableds(bool[] enableds)
        {
            if (_pointLightEnableds.EqualsValue(enableds)) return; // OPTIMIZATION
            _pointLightEnableds.CopyFrom(enableds, enableds.Length);
            pointLightEnabledsParam.TrySetValue(_pointLightEnableds);
        }

        /// <summary>
        /// Set a Vector3 array of size Constants.PointLightCount that tells of each point
        /// light's position.
        /// </summary>
        public void SetPointLightPositions(Vector3[] positions)
        {
            if (_pointLightPositions.EqualsValue(positions)) return; // OPTIMIZATION
            _pointLightPositions.CopyFrom(positions, positions.Length);
            pointLightPositionsParam.TrySetValue(_pointLightPositions);
        }

        /// <summary>
        /// Set a Vector3 array of size Constants.PointLightCount that tells of each point
        /// light's diffuse color.
        /// </summary>
        public void SetPointLightDiffuseColors(Vector3[] colors)
        {
            if (_pointLightDiffuseColors.EqualsValue(colors)) return; // OPTIMIZATION
            _pointLightDiffuseColors.CopyFrom(colors, colors.Length);
            pointLightDiffuseColorsParam.TrySetValue(_pointLightDiffuseColors);
        }

        /// <summary>
        /// Set a Vector3 array of size Constants.PointLightCount that tells of each point
        /// light's specular color.
        /// </summary>
        public void SetPointLightSpecularColors(Vector3[] colors)
        {
            if (_pointLightSpecularColors.EqualsValue(colors)) return; // OPTIMIZATION
            _pointLightSpecularColors.CopyFrom(colors, colors.Length);
            pointLightSpecularColorsParam.TrySetValue(_pointLightSpecularColors);
        }

        /// <summary>
        /// Set a float array of size Constants.PointLightCount that tells of each point
        /// light's range.
        /// </summary>
        public void SetPointLightRanges(float[] ranges)
        {
            if (_pointLightRanges.EqualsValue(ranges)) return; // OPTIMIZATION
            _pointLightRanges.CopyFrom(ranges, ranges.Length);
            pointLightRangesParam.TrySetValue(_pointLightRanges);
        }

        /// <summary>
        /// Set a float array of size Constants.PointLightCount that tells of each point
        /// light's falloff.
        /// </summary>
        public void SetPointLightFalloffs(float[] falloffs)
        {
            if (_pointLightFalloffs.EqualsValue(falloffs)) return; // OPTIMIZATION
            _pointLightFalloffs.CopyFrom(falloffs, falloffs.Length);
            pointLightFalloffsParam.TrySetValue(_pointLightFalloffs);
        }

        /// <summary>
        /// Populate the lighting parameters for normal drawing.
        /// </summary>
        /// <param name="surface">The lit surface.</param>
        /// <param name="ambientLights">The ambient lights affecting the lit material.</param>
        /// <param name="directionalLights">The directional lights affecting the lit material.</param>
        /// <param name="pointLights">The point lights affecting the lit material.</param>
        public void PopulateLighting(
            Surface surface,
            List<AmbientLight> ambientLights,
            List<DirectionalLight> directionalLights,
            List<PointLight> pointLights)
        {
            XiHelper.ArgumentNullCheck(surface, ambientLights, directionalLights, pointLights);

            // lighting enabled
            LightingEnabled = surface.LightingEnabled;

            // material
            DiffuseColor = surface.DiffuseColor.ToVector4();
            SpecularColor = surface.SpecularColor.ToVector3();
            SpecularPower = surface.SpecularPower;

            // ambient lighting
            bool ambientLightEnabled = false;
            Vector3 ambientLightColor = Vector3.Zero;
            foreach (AmbientLight ambientLight in ambientLights)
            {
                if (ambientLight.Enabled)
                {
                    ambientLightColor += ambientLight.Color.ToVector3();
                    ambientLightEnabled = true;
                }
            }
            this.AmbientLightEnabled = ambientLightEnabled;
            this.AmbientLightColor = ambientLightColor;

            // directional lights
            for (int i = 0; i < Constants.DirectionalLightCount; ++i)
            {
                if (i >= directionalLights.Count) directionalLightEnableds[i] = false;
                else
                {
                    DirectionalLight directionalLight = directionalLights[i];
                    directionalLightDirections[i] = directionalLight.Direction;
                    directionalLightEnableds[i] = directionalLight.Enabled;
                    directionalLightDiffuseColors[i] = directionalLight.DiffuseColor.ToVector3();
                    directionalLightSpecularColors[i] = directionalLight.SpecularColor.ToVector3();
                }
            }
            SetDirectionalLightEnableds(directionalLightEnableds);
            SetDirectionalLightDirections(directionalLightDirections);
            SetDirectionalLightDiffuseColors(directionalLightDiffuseColors);
            SetDirectionalLightSpecularColors(directionalLightSpecularColors);

            // point lights
            Vector3 surfaceCenter = surface.BoundingBox.GetCenter();
            pointLights.DistanceSort(surfaceCenter, SpatialSortOrder.NearToFar);
            for (int i = 0; i < Constants.PointLightCount; ++i)
            {
                if (i >= pointLights.Count) pointLightEnableds[i] = false;
                else
                {
                    PointLight pointLight = pointLights[i];
                    pointLightPositions[i] = pointLight.Position;
                    pointLightEnableds[i] = pointLight.Enabled;
                    pointLightDiffuseColors[i] = pointLight.DiffuseColor.ToVector3();
                    pointLightSpecularColors[i] = pointLight.SpecularColor.ToVector3();
                    pointLightRanges[i] = pointLight.Range;
                    pointLightFalloffs[i] = pointLight.Falloff;
                }
            }
            SetPointLightEnableds(pointLightEnableds);
            SetPointLightPositions(pointLightPositions);
            SetPointLightDiffuseColors(pointLightDiffuseColors);
            SetPointLightSpecularColors(pointLightSpecularColors);
            SetPointLightRanges(pointLightRanges);
            SetPointLightFalloffs(pointLightFalloffs);
        }

        private static bool[] directionalLightEnableds = new bool[Constants.DirectionalLightCount];
        private static Vector3[] directionalLightDirections = new Vector3[Constants.DirectionalLightCount];
        private static Vector3[] directionalLightDiffuseColors = new Vector3[Constants.DirectionalLightCount];
        private static Vector3[] directionalLightSpecularColors = new Vector3[Constants.DirectionalLightCount];
        private static bool[] pointLightEnableds = new bool[Constants.PointLightCount];
        private static Vector3[] pointLightPositions = new Vector3[Constants.PointLightCount];
        private static Vector3[] pointLightDiffuseColors = new Vector3[Constants.PointLightCount];
        private static Vector3[] pointLightSpecularColors = new Vector3[Constants.PointLightCount];
        private static float[] pointLightRanges = new float[Constants.PointLightCount];
        private static float[] pointLightFalloffs = new float[Constants.PointLightCount];
        private readonly EffectParameter lightingEnabledParam;
        private readonly EffectParameter diffuseColorParam;
        private readonly EffectParameter specularColorParam;
        private readonly EffectParameter specularPowerParam;
        private readonly EffectParameter ambientLightEnabledParam;
        private readonly EffectParameter ambientLightColorParam;
        private readonly EffectParameter directionalLightEnabledsParam;
        private readonly EffectParameter directionalLightDirectionsParam;
        private readonly EffectParameter directionalLightDiffuseColorsParam;
        private readonly EffectParameter directionalLightSpecularColorsParam;
        private readonly EffectParameter pointLightEnabledsParam;
        private readonly EffectParameter pointLightPositionsParam;
        private readonly EffectParameter pointLightDiffuseColorsParam;
        private readonly EffectParameter pointLightSpecularColorsParam;
        private readonly EffectParameter pointLightRangesParam;
        private readonly EffectParameter pointLightFalloffsParam;
        private bool _lightingEnabled;
        private Vector4 _diffuseColor;
        private Vector3 _specularColor;
        private float _specularPower;
        private bool _ambientLightEnabled;
        private Vector3 _ambientLightColor;
        private int[] _directionalLightEnableds = new int[Constants.DirectionalLightCount];
        private Vector3[] _directionalLightDirections = new Vector3[Constants.DirectionalLightCount];
        private Vector3[] _directionalLightDiffuseColors = new Vector3[Constants.DirectionalLightCount];
        private Vector3[] _directionalLightSpecularColors = new Vector3[Constants.DirectionalLightCount];
        private int[] _pointLightEnableds = new int[Constants.PointLightCount];
        private Vector3[] _pointLightPositions = new Vector3[Constants.PointLightCount];
        private Vector3[] _pointLightDiffuseColors = new Vector3[Constants.PointLightCount];
        private Vector3[] _pointLightSpecularColors = new Vector3[Constants.PointLightCount];
        private float[] _pointLightRanges = new float[Constants.PointLightCount];
        private float[] _pointLightFalloffs = new float[Constants.PointLightCount];
    }
}
