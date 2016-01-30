using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Defines a collection of vertices using the VertexPositionNormalTexture format.
    /// </summary>
    public class VerticesPositionNormalTexture : Vertices<VertexPositionNormalTexture>
    {
        /// <summary>
        /// Create a VerticesPositionNormalTexture.
        /// </summary>
        /// <param name="length">The length of the data array.</param>
        public VerticesPositionNormalTexture(int length) : base(length) { }

        /// <summary>
        /// Create a VerticesPositionNormalTexture.
        /// </summary>
        /// <param name="data">The data to copy to the array.</param>
        public VerticesPositionNormalTexture(VertexPositionNormalTexture[] data) : base(data) { }

        /// <inheritdoc />
        protected override VertexElement[] VertexElementsHook
        {
            get { return VertexPositionNormalTexture.VertexElements; }
        }

        /// <inheritdoc />
        protected override int VertexSizeHook
        {
            get { return VertexPositionNormalTexture.SizeInBytes; }
        }

        /// <inheritdoc />
        protected override Vector3 GetPositionHook(int index)
        {
            return Data[index].Position;
        }

        /// <inheritdoc />
        protected override void SetPositionHook(int index, Vector3 value)
        {
            Data[index].Position = value;
        }

        /// <inheritdoc />
        protected override Vector3 GetNormalHook(int index)
        {
            return Data[index].Normal;
        }

        /// <inheritdoc />
        protected override void SetNormalHook(int index, Vector3 value)
        {
            Data[index].Normal = value;
        }

        /// <inheritdoc />
        protected override Color GetColorHook(int index)
        {
            return new Color();
        }

        /// <inheritdoc />
        protected override void SetColorHook(int index, Color value) { }

        /// <inheritdoc />
        protected override Vector2 GetTexCoordHook(int index)
        {
            return Data[index].TextureCoordinate;
        }

        /// <inheritdoc />
        protected override void SetTexCoordHook(int index, Vector2 value)
        {
            Data[index].TextureCoordinate = value;
        }

        /// <inheritdoc />
        protected override object GetUserDefinedHook(string name, int index)
        {
            return null;
        }

        /// <inheritdoc />
        protected override void SetUserDefinedHook(string name, int index, object value) { }
    }
}
