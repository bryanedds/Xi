using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Defines a collection of vertices using the VertexPositionNormalTextureBinormalTangent format.
    /// </summary>
    public class VerticesPositionNormalTextureBinormalTangent :
        Vertices<VertexPositionNormalTextureBinormalTangent>
    {
        /// <summary>
        /// Create a VerticesPositionNormalTextureBinormalTangent.
        /// </summary>
        /// <param name="length">The length of the data array.</param>
        public VerticesPositionNormalTextureBinormalTangent(int length) : base(length) { }

        /// <summary>
        /// Create a VerticesPositionNormalTextureBinormalTangent.
        /// </summary>
        /// <param name="data">The data to copy to the array.</param>
        public VerticesPositionNormalTextureBinormalTangent(
            VertexPositionNormalTextureBinormalTangent[] data) : base(data) { }

        /// <inheritdoc />
        protected override VertexElement[] VertexElementsHook
        {
            get { return VertexPositionNormalTextureBinormalTangent.VertexElements; }
        }

        /// <inheritdoc />
        protected override int VertexSizeHook
        {
            get { return VertexPositionNormalTextureBinormalTangent.SizeInBytes; }
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
            switch (name)
            {
                case "Tangent": return Data[index].Tangent;
                case "Binormal": return Data[index].Binormal;
                default: return null;
            }
        }

        /// <inheritdoc />
        protected override void SetUserDefinedHook(string name, int index, object value)
        {
            switch (name)
            {
                case "Tangent": Data[index].Tangent = (Vector3)value; break;
                case "Binormal": Data[index].Binormal = (Vector3)value; break;
            }
        }
    }
}
