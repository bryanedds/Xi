using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A piece of 3D geometry.
    /// </summary>
    public class Geometry : Disposable
    {
        /// <summary>
        /// Create a Geometry object.
        /// </summary>
        /// <param name="device">The graphics device that the geometry will be created on.</param>
        /// <param name="vertexElements">The vertex elements that define the vertices that make up the geometry.</param>
        /// <param name="primitiveType">The type of primitive used to draw the geometry.</param>
        /// <param name="vertexBuffer">The vertex buffer that holds the geometry's vertices.</param>
        /// <param name="vertexSize">The size of each vertex in bytes.</param>
        /// <param name="vertexCount">The number of vertices in the vertex buffer.</param>
        public Geometry(
            GraphicsDevice device,
            VertexElement[] vertexElements,
            PrimitiveType primitiveType,
            VertexBuffer vertexBuffer,
            int vertexSize,
            int vertexCount)
        {
            XiHelper.ArgumentNullCheck(device, vertexElements, vertexBuffer);
            this.device = device;
            this.vertexBuffer = vertexBuffer;
            this.vertexSize = vertexSize;
            this.vertexCount = vertexCount;
            this.primitiveType = primitiveType;
            primitiveCount = primitiveType.GetPrimitiveCount(vertexCount);
            vertexDeclaration = new ManagedVertexDeclaration(device, vertexElements);
        }

        /// <summary>
        /// Create a Geometry object.
        /// </summary>
        /// <param name="device">The graphics device that the geometry will be created on.</param>
        /// <param name="vertexElements">The vertex elements that define the vertices that make up the geometry.</param>
        /// <param name="primitiveType">The type of primitive used to draw the geometry.</param>
        /// <param name="vertexBuffer">The vertex buffer that holds the geometry's vertices.</param>
        /// <param name="vertexSize">The size of each vertex in bytes.</param>
        /// <param name="vertexCount">The number of vertices in the vertex buffer.</param>
        /// <param name="indexBuffer">The index buffer that holds the geomety's indices.</param>
        /// <param name="indexCount">The number of indices in the index buffer.</param>
        public Geometry(
            GraphicsDevice device,
            VertexElement[] vertexElements,
            PrimitiveType primitiveType,
            VertexBuffer vertexBuffer,
            int vertexSize,
            int vertexCount,
            IndexBuffer indexBuffer,
            int indexCount)
            : this(device, vertexElements, primitiveType, vertexBuffer, vertexSize, vertexCount)
        {
            XiHelper.ArgumentNullCheck(device, vertexElements, vertexBuffer, indexBuffer);
            this.indexBuffer = indexBuffer;
            this.indexCount = indexCount;
            primitiveCount = primitiveType.GetPrimitiveCount(indexCount);
        }

        /// <summary>
        /// Create a Geometry object.
        /// </summary>
        /// <param name="device">The graphics device to create the geometry on.</param>
        /// <param name="primitiveType">The type of primitive used to draw the geometry.</param>
        /// <param name="vertices">The vertices of the geometry.</param>
        public Geometry(GraphicsDevice device, PrimitiveType primitiveType, IVertices vertices)
        {
            XiHelper.ArgumentNullCheck(device, vertices);
            this.device = device;
            this.primitiveType = primitiveType;
            vertexSize = vertices.VertexSize;
            vertexCount = vertices.Length;
            vertexBuffer = new VertexBuffer(device, vertices.Length * vertices.VertexSize, BufferUsage.None);
            vertices.SetDataOfVertexBuffer(vertexBuffer);
            primitiveCount = primitiveType.GetPrimitiveCount(vertices.Length);
            vertexDeclaration = new ManagedVertexDeclaration(device, vertices.VertexElements);
        }

        /// <summary>
        /// Create a Geometry object.
        /// </summary>
        /// <param name="device">The graphics device to create the geometry on.</param>
        /// <param name="primitiveType">The type of primitive used to draw the geometry.</param>
        /// <param name="vertices">The vertices of the geometry.</param>
        /// <param name="indices">The indices of the geometry.</param>
        public Geometry(GraphicsDevice device, PrimitiveType primitiveType, IVertices vertices, int[] indices)
            : this(device, primitiveType, vertices)
        {
            XiHelper.ArgumentNullCheck(device, vertices, indices);
            indexCount = indices.Length;
            indexBuffer = new IndexBuffer(device, typeof(int), indices.Length, BufferUsage.None);
            indexBuffer.SetData<int>(indices);
            primitiveCount = primitiveType.GetPrimitiveCount(indices.Length);
        }

        /// <summary>
        /// Draw the geometry.
        /// </summary>
        public void Draw(GameTime gameTime)
        {
            XiHelper.ArgumentNullCheck(gameTime);
            vertexDeclaration.Activate();
            device.Vertices[0].SetSource(vertexBuffer, 0, vertexSize);
            if (indexCount != 0) DrawIndexedPrimitives();
            else DrawPrimitives();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                vertexDeclaration.Dispose();
                vertexBuffer.Dispose();
                if (indexBuffer != null) indexBuffer.Dispose();
            }        
            base.Dispose(disposing);
        }

        private void DrawPrimitives()
        {
            device.DrawPrimitives(primitiveType, 0, primitiveCount);
        }

        private void DrawIndexedPrimitives()
        {
            device.Indices = indexBuffer;
            device.DrawIndexedPrimitives(primitiveType, 0, 0, vertexCount, 0, primitiveCount);
        }

        private readonly GraphicsDevice device;
        private readonly PrimitiveType primitiveType;
        private readonly VertexBuffer vertexBuffer;
        /// <summary>May be null.</summary>
        private readonly IndexBuffer indexBuffer;
        private readonly int primitiveCount;
        private readonly int vertexSize;
        private readonly int vertexCount;
        private readonly int indexCount;
        private ManagedVertexDeclaration vertexDeclaration;
    }
}
