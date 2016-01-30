using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A surface of a BaseModel.
    /// </summary>
    public abstract class BaseModelSurface : Surface<Actor3D>
    {
        /// <summary>
        /// Create a BaseModelSurface.
        /// </summary>
        public BaseModelSurface(XiGame game, Actor3D actor, string effectFileName, int meshIndex, int partIndex)
            : base(game, actor, effectFileName)
        {
            this.meshIndex = meshIndex;
            this.partIndex = partIndex;
            boundingBoxBuilder = new BoundingBoxBuilder(Part.GenerateBoundingBox(Mesh));
        }

        /// <summary>
        /// The index of the model mesh that the surface draws.
        /// </summary>
        public int MeshIndex { get { return meshIndex; } }

        /// <summary>
        /// The index of the model mesh part that the surface draws.
        /// </summary>
        public int PartIndex { get { return partIndex; } }

        /// <summary>
        /// The model.
        /// </summary>
        protected abstract Model Model { get; }

        /// <summary>
        /// The mesh.
        /// </summary>
        protected ModelMesh Mesh { get { return Model.Meshes[meshIndex]; } }

        /// <summary>
        /// The model mesh part that the surface draws.
        /// </summary>
        protected ModelMeshPart Part { get { return Mesh.MeshParts[partIndex]; } }

        /// <summary>
        /// The bone in the absolute world frame.
        /// </summary>
        protected abstract void GetBoneAbsoluteWorld(out Matrix worldTransform);

        /// <inheritdoc />
        protected override BoundingBox BoundingBoxHook
        {
            get
            {
                Matrix worldTransform;
                GetBoneAbsoluteWorld(out worldTransform);
                boundingBoxBuilder.WorldTransform = worldTransform;
                return boundingBoxBuilder.BoundingBoxWorld;
            }
        }

        /// <inheritdoc />
        protected override bool BoundlessHook { get { return false; } }

        /// <inheritdoc />
        protected override Effect EffectHook { get { return Part.Effect; } }

        /// <inheritdoc />
        protected override string EffectFileNameHook
        {
            get { return string.Empty; }
            set { }
        }

        private readonly BoundingBoxBuilder boundingBoxBuilder;
        private readonly int meshIndex;
        private readonly int partIndex;
    }
}
