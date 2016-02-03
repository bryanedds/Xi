using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A base model that generalizes actor model functionality.
    /// </summary>
    public abstract class BaseModel<S> : Actor3D where S : BaseModelSurface
    {
        /// <summary>
        /// Create a BaseModel.
        /// </summary>
        /// <param name="game">The game.</param>
        public BaseModel(XiGame game) : base(game)
        {
            SetUpModel();
        }

        /// <summary>
        /// The shape of the physics body.
        /// </summary>
        public BodyShape BodyShape
        {
            get { return _bodyShape; }
            set
            {
                if (_bodyShape == value) return; // OPTIMIZATION: avoid ResetPhysics
                _bodyShape = value;
                ResetPhysics();
            }
        }

        /// <summary>
        /// The XNA Model.
        /// </summary>
        [Browsable(false)]
        public Model Model { get { return _model; } }

        /// <summary>
        /// The name of the model.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string ModelFileName
        {
            get { return _modelFileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (_modelFileName == value) return; // OPTIMIZATION
                Game.Content.Load<Model>(value); // ensure model loads properly
                _modelFileName = value;
                ResetModel();
            }
        }

        /// <summary>
        /// Get the surface at [meshIndex, partIndex].
        /// </summary>
        public S GetSurface(int meshIndex, int partIndex)
        {
            S result;
            if (TryGetSurface(meshIndex, partIndex, out result)) return result;
            throw new InvalidOperationException("BaseModel is missing a surface for a ModelMeshPart.");
        }

        /// <summary>
        /// A bone transform in absolute space.
        /// </summary>
        public void GetBoneAbsolute(int boneIndex, out Matrix boneAbsolute)
        {
            boneAbsolute = bonesAbsolute[boneIndex];
        }

        /// <summary>
        /// A bone transform in absolute world space.
        /// </summary>
        public void GetBoneAbsoluteWorld(int boneIndex, out Matrix boneTransform)
        {
            Matrix boneAbsolute;
            GetBoneAbsolute(boneIndex, out boneAbsolute);
            Matrix worldTransform;
            GetWorldTransform(out worldTransform);
            Matrix.Multiply(ref boneAbsolute, ref worldTransform, out boneTransform);
        }

        /// <summary>
        /// Create a surface for mesh[i] and meshPart[j].
        /// </summary>
        protected abstract S CreateSurface(int i, int j);

        /// <inheritdoc />
        protected override void Destroy(bool destroying)
        {
            if (destroying)
            {
                TearDownSurfaces();
                TearDownPhysics();
            }
            base.Destroy(destroying);
        }

        /// <inheritdoc />
        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            RefreshBonesAbsolute();
        }

        /// <inheritdoc />
        protected override List<Surface> CollectSurfacesHook(List<Surface> surfaces)
        {
            surfaces.Add(this.surfaces);
            return surfaces;
        }
        protected override void GetMountPointTransformHook(int mountPoint, out Matrix transform)
        {
            if (IsBoneMount(mountPoint)) GetBoneAbsoluteWorld(mountPoint - 1, out transform);
            else base.GetMountPointTransformHook(mountPoint, out transform);
        }

        private IModelPhysics CreateModelPhysics()
        {
            switch (BodyShape)
            {
                case BodyShape.Box: return new BoxModelPhysics(Game, Model, Position, Mass);
                case BodyShape.Sphere: return new SphereModelPhysics(Game, Model, Position, Mass);
                case BodyShape.Capsule: return new CapsuleModelPhysics(Game, Model, Position, Mass);
                case BodyShape.StaticMesh: return new StaticMeshModelPhysics(Game, Model, Position, Mass);
                case BodyShape.Amorphous: return null;
                default: throw new ArgumentException("Invalid body shape '" + BodyShape.ToString() + "'.");
            }
        }

        private void ResetModel()
        {
            SetUpModel();
        }

        private void ResetSurfaces()
        {
            TearDownSurfaces();
            SetUpSurfaces();
        }

        private void ResetPhysics()
        {
            TearDownPhysics();
            SetUpPhysics();
        }

        private void SetUpModel()
        {
            _model = Game.Content.Load<Model>(ModelFileName);
            ResetSurfaces();
            ResetPhysics();
            RefreshBonesAbsolute();
        }

        private void SetUpSurfaces()
        {
            for (int i = 0; i < Model.Meshes.Count; ++i)
                for (int j = 0; j < Model.Meshes[i].MeshParts.Count; ++j)
                    surfaces.Add(CreateSurface(i, j));
        }

        private void SetUpPhysics()
        {
            modelPhysics = CreateModelPhysics();
            Entity = modelPhysics != null ? modelPhysics.Entity : new AmorphousEntity();
        }

        private void TearDownSurfaces()
        {
            foreach (S surface in surfaces) surface.Dispose();
            surfaces.Clear();
        }

        private void TearDownPhysics()
        {
            if (modelPhysics != null) modelPhysics.Dispose();
            modelPhysics = null;
        }

        private void RefreshBonesAbsolute()
        {
            EnsureBonesAbsoluteCapacity();
            Model.CopyAbsoluteBoneTransformsTo(bonesAbsolute);
        }

        private bool TryGetSurface(int meshIndex, int partIndex, out S result)
        {
            result =
                surfaces.
                Where(x => x.MeshIndex == meshIndex && x.PartIndex == partIndex).
                OfType<S>().
                FirstOrDefault(); // MEMORYCHURN
            return result != null;
        }

        private void EnsureBonesAbsoluteCapacity()
        {
            int bonesCount = Model.Bones.Count;
            if (bonesAbsolute.Length >= bonesCount) return;
            bonesAbsolute = new Matrix[bonesCount];
        }

        private bool IsBoneMount(int mountPoint)
        {
            return
                mountPoint > 0 &&
                mountPoint < Model.Bones.Count + 1;
        }

        private readonly List<S> surfaces = new List<S>();
        private IModelPhysics modelPhysics;
        private Matrix[] bonesAbsolute = new Matrix[0];
        private Model _model;
        private string _modelFileName = "Xi/3D/cube";
        private BodyShape _bodyShape;
    }
}
