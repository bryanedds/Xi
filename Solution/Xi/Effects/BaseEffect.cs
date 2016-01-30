using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Represents the base shader effect for 3D objects.
    /// </summary>
    public class BaseEffect : Effect
    {
        /// <summary>
        /// Initializes a new instance of BaseEffect by cloning an existing effect.
        /// </summary>
        /// <param name="device">The graphics device that will create the effect.</param>
        /// <param name="cloneSource">The effect to clone.</param>
        public BaseEffect(GraphicsDevice device, Effect cloneSource)
            : base(device, cloneSource)
        {
            worldParam = Parameters["xWorld"];
            viewParam = Parameters["xView"];
            projectionParam = Parameters["xProjection"];
            viewProjectionParam = Parameters["xViewProjection"];
            worldViewProjectionParam = Parameters["xWorldViewProjection"];
            worldInverseParam = Parameters["xWorldInverse"];
            cameraPositionParam = Parameters["xCameraPosition"];
            diffuseMapParam = Parameters["xDiffuseMap"];
        }
        
        /// <summary>
        /// The world matrix.
        /// </summary>
        public Matrix World
        {
            get { return _world; }
            set
            {
                if (_world == value) return; // OPTIMIZATION
                _world = value;
                worldParam.TrySetValue(value);
            }
        }

        /// <summary>
        /// The view matrix.
        /// </summary>
        public Matrix View
        {
            get { return _view; }
            set
            {
                if (_view == value) return; // OPTIMIZATION
                _view = value;
                viewParam.TrySetValue(value);
            }
        }

        /// <summary>
        /// The projection matrix.
        /// </summary>
        public Matrix Projection
        {
            get { return _projection; }
            set
            {
                if (_projection == value) return; // OPTIMIZATION
                _projection = value;
                projectionParam.TrySetValue(value);
            }
        }

        /// <summary>
        /// The view * projection matrix.
        /// </summary>
        public Matrix ViewProjection
        {
            get { return _viewProjection; }
            set
            {
                if (_viewProjection == value) return; // OPTIMIZATION
                _viewProjection = value;
                viewProjectionParam.TrySetValue(value);
            }
        }

        /// <summary>
        /// The world * view * projection matrix.
        /// </summary>
        public Matrix WorldViewProjection
        {
            get { return _worldViewProjection; }
            set
            {
                if (_worldViewProjection == value) return; // OPTIMIZATION
                _worldViewProjection = value;
                worldViewProjectionParam.TrySetValue(value);
            }
        }

        /// <summary>
        /// The world inverse matrix.
        /// </summary>
        public Matrix WorldInverse
        {
            get { return _worldInverse; }
            set
            {
                if (_worldInverse == value) return; // OPTIMIZATION
                _worldInverse = value;
                worldInverseParam.TrySetValue(value);
            }
        }

        /// <summary>
        /// The position of the camera.
        /// </summary>
        public Vector3 CameraPosition
        {
            get { return _cameraPosition; }
            set
            {
                if (_cameraPosition == value) return; // OPTIMIZATION
                _cameraPosition = value;
                cameraPositionParam.TrySetValue(value);
            }
        }

        /// <summary>
        /// The default map for the effect. May be null.
        /// </summary>
        public Texture2D DiffuseMap
        {
            get { return _diffuseMap; }
            set
            {
                if (_diffuseMap == value) return; // OPTIMIZATION
                _diffuseMap = value;
                diffuseMapParam.TrySetValue(value);
            }
        }

        /// <summary>
        /// Populate the local and world transforms of a BaseEffect.
        /// </summary>
        public void PopulateTransform(Camera camera, ref Matrix world)
        {
            XiHelper.ArgumentNullCheck(camera);
            PopulateTransformLocal(camera, ref world);
            PopulateTransformWorld(camera);
        }

        /// <summary>
        /// Populate the World, WorldInverse, and WorldViewProjection parameters.
        /// </summary>
        public void PopulateTransformLocal(Camera camera, ref Matrix world)
        {
            XiHelper.ArgumentNullCheck(camera);
            World = world;
            WorldInverse = world.Determinant() != 0 ? Matrix.Invert(world) : Matrix.Identity;
            WorldViewProjection = world * camera.ViewProjection;
        }

        /// <summary>
        /// Populate the View, Projection, ViewProjection, and CameraPosition parameters.
        /// </summary>
        public void PopulateTransformWorld(Camera camera)
        {
            XiHelper.ArgumentNullCheck(camera);
            View = camera.View;
            Projection = camera.Projection;
            ViewProjection = camera.ViewProjection;
            CameraPosition = camera.Position;
        }

        private readonly EffectParameter worldParam;
        private readonly EffectParameter viewParam;
        private readonly EffectParameter projectionParam;
        private readonly EffectParameter viewProjectionParam;
        private readonly EffectParameter worldViewProjectionParam;
        private readonly EffectParameter worldInverseParam;
        private readonly EffectParameter cameraPositionParam;
        private readonly EffectParameter diffuseMapParam;
        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;
        private Matrix _viewProjection;
        private Matrix _worldViewProjection;
        private Matrix _worldInverse;
        private Vector3 _cameraPosition;
        private Texture2D _diffuseMap;
    }
}
