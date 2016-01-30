using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Xi game engine constants.
    /// </summary>
    public static class Constants
    {
        public const DeviceType DeviceTypeSetting = DeviceType.Hardware;
        public const MultiSampleType MultiSampleTypeSetting = MultiSampleType.None;
        public const RenderTargetUsage RenderTargetUsageSetting = RenderTargetUsage.DiscardContents;
        public const string ContentPath = "Content";
        public const float DepthStencilBufferSurfaceToScreenRatio = 2;
        public const int DirectionalLightCount = 4;
        public const int PointLightCount = 8;
        public const float WaterReflectionMapSurfaceToScreenRatio = 1;
        public const int DirectionalShadowCount = 1;
        public const float DirectionalShadowMapSurfaceToScreenRatio = 2;
        public const float DirectionalShadowRange = 512;
        public const float DirectionalShadowCameraSnap = 4;
        public const float DirectionalShadowDepthBias = 0.01f;
        public static readonly Vector2 DirectionalShadowSize = new Vector2(1024);
    }
}
