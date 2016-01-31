using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAnimation;
using XNAnimation.Controllers;

namespace Xi
{
    /// <summary>
    /// An animated model that can be placed in a scene.
    /// </summary>
    public class AnimatedModel : SingleSurfaceActor<AnimatedModelSurface>
    {
        /// <summary>
        /// Create an AnimatedModel.
        /// </summary>
        public AnimatedModel(XiGame game) : base(game)
        {
            SkinnedModelFileName = "Xi/3D/PlayerMarine";
            // populate surface AFTER becoming an otherwise valid object
            surface = new AnimatedModelSurface(Game, this);
        }

        /// <summary>
        /// The material's emissive color.
        /// </summary>
        public Color EmissiveColor
        {
            get { return surface.EmissiveColor; }
            set { surface.EmissiveColor = value; }
        }

        /// <summary>
        /// Use normal mapping?
        /// </summary>
        public bool NormalMapEnabled
        {
            get { return surface.NormalMapEnabled; }
            set { surface.NormalMapEnabled = value; }
        }

        /// <summary>
        /// The animation controller. All changes to AnimationController are discarded when
        /// SkinnedModelFileName is changed.
        /// </summary>
        [Browsable(false)]
        public IAnimationController AnimationController { get { return _animationController; } }

        /// <summary>
        /// The skinned model. All changes to Model are discarded when SkinnedModelFileName is
        /// changed.
        /// </summary>
        [Browsable(false)]
        public SkinnedModel SkinnedModel { get { return _skinnedModel; } }

        /// <summary>
        /// The name of the skinned model file.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string SkinnedModelFileName
        {
            get { return _skinnedModelFileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (_skinnedModelFileName == value) return; // OPTIMIZATION
                SkinnedModel newSkinnedModel = Game.Content.Load<SkinnedModel>(value);
                IAnimationController newAnimationController = new AnimationController(newSkinnedModel.SkeletonBones);
                _skinnedModel = newSkinnedModel;
                _animationController = newAnimationController;
                _skinnedModelFileName = value;
            }
        }

        /// <summary>
        /// A bone transform in absolute space.
        /// </summary>
        public void GetBoneAbsolute(int boneIndex, out Matrix boneAbsolute)
        {
            Matrix inverseBindPoseTransform = SkinnedModel.SkeletonBones[boneIndex].InverseBindPoseTransform;
            Matrix bindPoseTransform;
            Matrix.Invert(ref inverseBindPoseTransform, out bindPoseTransform);
            Matrix skinnedBoneTransform = AnimationController.SkinnedBoneTransforms[boneIndex];
            Matrix.Multiply(ref bindPoseTransform, ref skinnedBoneTransform, out boneAbsolute);
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

        /// <inheritdoc />
        protected override AnimatedModelSurface SurfaceHook { get { return surface; } }

        /// <inheritdoc />
        protected override void Destroy(bool destroying)
        {
            if (destroying) surface.Dispose();
            base.Destroy(destroying);
        }

        /// <inheritdoc />
        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            AnimationController.Update(gameTime.ElapsedGameTime, Matrix.Identity);
        }

        /// <inheritdoc />
        protected override void GetMountPointTransformHook(int mountPoint, out Matrix transform)
        {
            if (IsBoneMount(mountPoint)) GetBoneAbsoluteWorld(mountPoint - 1, out transform);
            else base.GetMountPointTransformHook(mountPoint, out transform);
        }

        private bool IsBoneMount(int mountPoint)
        {
            return
                mountPoint > 0 &&
                mountPoint < SkinnedModel.SkeletonBones.Count + 1;
        }

        private readonly AnimatedModelSurface surface;
        private IAnimationController _animationController;
        private SkinnedModel _skinnedModel;
        private string _skinnedModelFileName;
    }
}
