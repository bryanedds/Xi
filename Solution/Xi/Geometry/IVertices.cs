using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Represents an array of abstract vertices.
    /// </summary>
    public interface IVertices
    {
        /// <summary>
        /// The vertex elements that compose each vertex.
        /// </summary>
        VertexElement[] VertexElements { get; }

        /// <summary>
        /// The size in bytes of each vertex.
        /// </summary>
        int VertexSize { get; }

        /// <summary>
        /// The number of vertices in the collection.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Get the position of a vertex.
        /// </summary>
        Vector3 GetPosition(int index);

        /// <summary>
        /// Set the position of a vertex.
        /// </summary>
        void SetPosition(int index, Vector3 value);

        /// <summary>
        /// Get the normal of a vertex. Returns Vector3.Zero if normals aren't available.
        /// </summary>
        Vector3 GetNormal(int index);

        /// <summary>
        /// Set the normal of a vertex.
        /// </summary>
        void SetNormal(int index, Vector3 value);

        /// <summary>
        /// Set the color of a vertex. Returns Color.White if colors aren't available.
        /// </summary>
        Color GetColor(int index);

        /// <summary>
        /// Set the color of a vertex.
        /// </summary>
        void SetColor(int index, Color value);

        /// <summary>
        /// Get the texture coordinate of a vertex. Returns Vector2.Zero if texture coordinates
        /// aren't available.
        /// </summary>
        Vector2 GetTexCoord(int index);

        /// <summary>
        /// Set the texture coordinate of a vertex.
        /// </summary>
        void SetTexCoord(int index, Vector2 value);

        /// <summary>
        /// Get the value of a user-defined field of a vertex. Returns null if the field isn't
        /// available.
        /// May return null.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="index">The index of the vertex.</param>
        /// <returns>The value of the field.</returns>
        object GetUserDefined(string name, int index);

        /// <summary>
        /// Set the value of a vertex's user-defined field.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="index">The index of the vertex.</param>
        /// <param name="value">The value to set.</param>
        void SetUserDefined(string name, int index, object value);

        /// <summary>
        /// Populate vertices with the data from a vertex buffer.
        /// </summary>
        void GetDataOfVertexBuffer(VertexBuffer buffer);

        /// <summary>
        /// Populate a vertex buffer with vertices.
        /// </summary>
        void SetDataOfVertexBuffer(VertexBuffer buffer);
    }

    /// <summary>
    /// Helps in constructing vertices.
    /// </summary>
    public static class VerticesHelper
    {
        /// <summary>
        /// Create vertices of a given type and count.
        /// </summary>
        public static IVertices CreateVertices<T>(int vertexCount) where T : IVertices
        {
            Type verticesType = typeof(T);
            ConstructorInfo constructor = verticesType.GetConstructor(constructorParamTypes);
            if (constructor == null) throw new ArgumentException("Vertices type '" + verticesType.FullName + "' has no valid constructor '(int vertexCount)'.");
            object[] constructorParameters = new object[] { vertexCount };
            return XiHelper.Cast<IVertices>(constructor.Invoke(constructorParameters));
        }

        private static readonly Type[] constructorParamTypes = new[] { typeof(int) };
    }
}
