using System;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A managed vertex declaration.
    /// </summary>
    public class ManagedVertexDeclaration : Disposable
    {
        /// <summary>
        /// Create a ManagedVertexDeclaration.
        /// </summary>
        /// <param name="device">The graphics device.</param>
        /// <param name="elements">The vertex elements.</param>
        public ManagedVertexDeclaration(GraphicsDevice device, VertexElement[] elements)
        {
            this.elements = elements;
            this.device = device;
            vertexDeclaration = CreateVertexDeclaration();
            device.DeviceReset += device_DeviceReset;
        }

        /// <summary>
        /// Activate the vertex declaration for drawing.
        /// </summary>
        public void Activate()
        {
            device.VertexDeclaration = vertexDeclaration;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                device.DeviceReset -= device_DeviceReset;
                vertexDeclaration.Dispose();
            }            
            base.Dispose(disposing);
        }

        private void device_DeviceReset(object sender, EventArgs e)
        {
            vertexDeclaration.Dispose();
            vertexDeclaration = CreateVertexDeclaration();
        }

        private VertexDeclaration CreateVertexDeclaration()
        {
            return new VertexDeclaration(device, elements);
        }

        private readonly VertexElement[] elements;
        private readonly GraphicsDevice device;
        private VertexDeclaration vertexDeclaration;
    }
}
