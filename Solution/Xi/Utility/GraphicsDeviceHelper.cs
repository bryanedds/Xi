using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// An extension method class for GraphicsDevice.
    /// </summary>
    public static class GraphicsDeviceHelper
    {
        /// <summary>
        /// Adjust the GraphicsDevice's render state to render the faces in a faceMode.
        /// </summary>
        public static void BeginFaceMode(this GraphicsDevice device, FaceMode faceMode)
        {
            switch (faceMode)
            {
                case FaceMode.FrontFaces: device.RenderState.CullMode = CullMode.CullCounterClockwiseFace; break;
                case FaceMode.BackFaces: device.RenderState.CullMode = CullMode.CullClockwiseFace; break;
                case FaceMode.AllFaces: device.RenderState.CullMode = CullMode.None; break;
            }
        }

        /// <summary>
        /// Reset the GraphicsDevice's render state after adjusting its face drawing mode.
        /// Must be called after BeginFaceMode.
        /// </summary>
        public static void EndFaceMode(this GraphicsDevice device)
        {
            device.RenderState.CullMode = CullMode.CullClockwiseFace;
        }
    }
}
