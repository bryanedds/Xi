using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A partial implementation of IVertices.
    /// </summary>
    /// <typeparam name="T">The type of vertex data structure to use.</typeparam>
    public abstract class Vertices<T> : IVertices where T : struct
    {
        /// <summary>
        /// Initialize a Vertices.
        /// </summary>
        /// <param name="length">The length of the data array.</param>
        public Vertices(int length)
        {
            data = new T[length];
        }

        /// <inheritdoc />
        public VertexElement[] VertexElements { get { return VertexElementsHook; } }

        /// <inheritdoc />
        public int VertexSize { get { return VertexSizeHook; } }

        /// <inheritdoc />
        public int Length { get { return data.Length; } }

        /// <summary>
        /// Initialize a Vertices.
        /// </summary>
        /// <param name="data">The data to copy to the array.</param>
        public Vertices(T[] data)
        {
            XiHelper.ArgumentNullCheck(data);
            this.data = XiHelper.Cast<T[]>(data.Clone());
        }

        /// <summary>
        /// Copy data to the array.
        /// </summary>
        public void SetData(T[] data)
        {
            XiHelper.ArgumentNullCheck(data);
            data.CopyTo(this.data, 0);
        }

        /// <summary>
        /// Get a copy of the data array.
        /// </summary>
        /// <returns></returns>
        public T[] GetData()
        {
            return XiHelper.Cast<T[]>(data.Clone());
        }

        /// <inheritdoc />
        public Vector3 GetPosition(int index)
        {
            return GetPositionHook(index);
        }

        /// <inheritdoc />
        public void SetPosition(int index, Vector3 value)
        {
            SetPositionHook(index, value);
        }

        /// <inheritdoc />
        public Vector3 GetNormal(int index)
        {
            return GetNormalHook(index);
        }

        /// <inheritdoc />
        public void SetNormal(int index, Vector3 value)
        {
            SetNormalHook(index, value);
        }

        /// <inheritdoc />
        public Color GetColor(int index)
        {
            return GetColorHook(index);
        }

        /// <inheritdoc />
        public void SetColor(int index, Color value)
        {
            SetColorHook(index, value);
        }

        /// <inheritdoc />
        public Vector2 GetTexCoord(int index)
        {
            return GetTexCoordHook(index);
        }

        /// <inheritdoc />
        public void SetTexCoord(int index, Vector2 value)
        {
            SetTexCoordHook(index, value);
        }

        /// <inheritdoc />
        public object GetUserDefined(string name, int index)
        {
            XiHelper.ArgumentNullCheck(name);
            return GetUserDefinedHook(name, index);
        }

        /// <inheritdoc />
        public void SetUserDefined(string name, int index, object value)
        {
            XiHelper.ArgumentNullCheck(name);
            SetUserDefinedHook(name, index, value);
        }

        /// <inheritdoc />
        public void GetDataOfVertexBuffer(VertexBuffer buffer)
        {
            XiHelper.ArgumentNullCheck(buffer);
            buffer.GetData(data);
        }

        /// <inheritdoc />
        public void SetDataOfVertexBuffer(VertexBuffer buffer)
        {
            XiHelper.ArgumentNullCheck(buffer);
            buffer.SetData(data);
        }

        /// <summary>
        /// The raw vertex data.
        /// </summary>
        protected T[] Data { get { return data; } }

        /// <summary>
        /// Handle getting raw vertex elements.
        /// </summary>
        protected abstract VertexElement[] VertexElementsHook { get; }

        /// <summary>
        /// Handle getting raw vertex size.
        /// </summary>
        protected abstract int VertexSizeHook { get; }

        /// <summary>
        /// Handle getting the position of the specified vertex.
        /// </summary>
        protected abstract Vector3 GetPositionHook(int index);

        /// <summary>
        /// Handle setting the position of the specified vertex.
        /// </summary>
        protected abstract void SetPositionHook(int index, Vector3 value);

        /// <summary>
        /// Handle getting the normal of the specified vertex.
        /// </summary>
        protected abstract Vector3 GetNormalHook(int index);

        /// <summary>
        /// Handle setting the normal of the specified vertex.
        /// </summary>
        protected abstract void SetNormalHook(int index, Vector3 value);

        /// <summary>
        /// Handle getting the color of the specified vertex.
        /// </summary>
        protected abstract Color GetColorHook(int index);

        /// <summary>
        /// Handle setting the color of the specified vertex.
        /// </summary>
        protected abstract void SetColorHook(int index, Color value);

        /// <summary>
        /// Handle getting the texture coordinate of the specified vertex.
        /// </summary>
        protected abstract Vector2 GetTexCoordHook(int index);

        /// <summary>
        /// Handle setting the texture coordinate of the specified vertex.
        /// </summary>
        protected abstract void SetTexCoordHook(int index, Vector2 value);

        /// <summary>
        /// Handle getting a user-defined vertex property of the specified vertex.
        /// </summary>
        protected abstract object GetUserDefinedHook(string name, int index);

        /// <summary>
        /// Handle setting a user-defined vertex property of the specified vertex.
        /// </summary>
        protected abstract void SetUserDefinedHook(string name, int index, object value);

        private readonly T[] data;
    }
}
