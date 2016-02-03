using System;
using System.Collections.Generic;
using System.ComponentModel;
using BEPUphysics;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// A three-dimensional actor.
    /// NOTE: if we need mounting, it can be done through a layer between the actor and the physics
    /// system where only either the mounting or physics can be on at one time (unless the actor is
    /// the root of the mounting hierarchy).
    /// </summary>
    public abstract class Actor3D : Actor
    {
        /// <summary>
        /// Create an Actor3D.
        /// </summary>
        /// <param name="game">The game.</param>
        public Actor3D(XiGame game) : base(game)
        {
            SetUpEntity();
            game.Scene.AddActor(this);
        }

        /// <summary>
        /// Has the entity changed?
        /// </summary>
        public event Action<Actor3D, Entity> EntityChanged;

        /// <summary>
        /// The entity's event manager.
        /// TODO: consider exposing the events directly instead of exposing this
        /// </summary>
        [Browsable(false)]
        public EntityEventManager EventManager { get { return Entity.EventManager; } }

        /// <summary>
        /// The entity's collision rules.
        /// TODO: consider exposing individual rules to make them available in the editor (if that
        /// even makes sense).
        /// </summary>
        [Browsable(false), IgnoreSerialization]
        public EntityCollisionRules CollisionRules
        {
            get { return Entity.CollisionRules; }
            set { Entity.CollisionRules = value; }
        }

        /// <summary>
        /// The position.
        /// </summary>
        public Vector3 Position
        {
            get { return Entity.InternalCenterPosition; }
            set { Entity.InternalCenterPosition = value; }
        }

        /// <summary>
        /// The right vector.
        /// </summary>
        [Browsable(false)]
        public Vector3 Right { get { return Entity.InternalOrientationMatrix.Right; } }

        /// <summary>
        /// The left vector.
        /// </summary>
        [Browsable(false)]
        public Vector3 Left { get { return Entity.InternalOrientationMatrix.Left; } }

        /// <summary>
        /// The up vector.
        /// </summary>
        [Browsable(false)]
        public Vector3 Up { get { return Entity.InternalOrientationMatrix.Up; } }

        /// <summary>
        /// The down vector.
        /// </summary>
        [Browsable(false)]
        public Vector3 Down { get { return Entity.InternalOrientationMatrix.Down; } }

        /// <summary>
        /// The forward vector.
        /// </summary>
        [Browsable(false)]
        public Vector3 Forward { get { return Entity.InternalOrientationMatrix.Forward; } }

        /// <summary>
        /// The backward vector.
        /// </summary>
        [Browsable(false)]
        public Vector3 Backward { get { return Entity.InternalOrientationMatrix.Backward; } }

        /// <summary>
        /// The orientation matrix.
        /// </summary>
        [Browsable(false), IgnoreSerialization]
        public Matrix OrientationMatrix
        {
            get { return Entity.InternalOrientationMatrix; }
            set { Entity.InternalOrientationMatrix = value; }
        }

        /// <summary>
        /// The orientation quaternion.
        /// </summary>
        [Browsable(false)]
        public Quaternion OrientationQuaternion
        {
            get { return Entity.InternalOrientationQuaternion; }
            set { Entity.InternalOrientationQuaternion = value; }
        }

        /// <summary>
        /// The orientation in eular angles using degrees. Goes out of sync when physics are
        /// processed, entity is changed, or orientation is changed by other means.
        /// </summary>
        [IgnoreSerialization]
        public Vector3 OrientationEularDegrees
        {
            get { return _orientationEularDegrees; }
            set
            {
                OrientationQuaternion = Quaternion.CreateFromYawPitchRoll(
                    MathHelper.ToRadians(value.Y),
                    MathHelper.ToRadians(value.X),
                    MathHelper.ToRadians(value.Z));
                _orientationEularDegrees = value;
            }
        }

        /// <summary>
        /// The bounding box.
        /// </summary>
        [Browsable(false)]
        public BoundingBox BoundingBox { get { return Entity.BoundingBox; } }

        /// <summary>
        /// The inertia tensor in world space.
        /// </summary>
        [Browsable(false)]
        public Matrix InertiaTensor { get { return Entity.InertiaTensor; } }

        /// <summary>
        /// The intertia tensor inverse in world space.
        /// </summary>
        [Browsable(false)]
        public Matrix InertiaTensorInverse { get { return Entity.InertiaTensorInverse; } }

        /// <summary>
        /// The inertia tensor in local space.
        /// </summary>
        [Browsable(false)]
        public Matrix LocalInertiaTensor { get { return Entity.LocalInertiaTensor; } }

        /// <summary>
        /// The inertia tensor inverse in local space.
        /// </summary>
        [Browsable(false)]
        public Matrix LocalInertiaTensorInverse { get { return Entity.LocalInertiaTensorInverse; } }

        /// <summary>
        /// The world transform.
        /// </summary>
        [Browsable(false), IgnoreSerialization]
        public Matrix WorldTransform
        {
            get { return Entity.InternalWorldTransform; }
            set { Entity.InternalWorldTransform = value; }
        }

        /// <summary>
        /// Whether the entity's rotation is fixed.
        /// </summary>
        [PhysicsBrowse]
        public bool FixedRotation
        {
            get { return Entity.LocalInertiaTensorInverse == new Matrix(); }
            set
            {
                if (value)
                {
                    Entity.LocalInertiaTensorInverse = new Matrix();
                    Entity.OrientationMatrix = Matrix.Identity;
                }
                else Entity.LocalInertiaTensorInverse = Matrix.Identity;
            }
        }

        /// <summary>
        /// The angular momentum.
        /// </summary>
        [PhysicsBrowse]
        public Vector3 AngularMomentum
        {
            get { return Entity.InternalAngularMomentum; }
            set { Entity.InternalAngularMomentum = value; }
        }

        /// <summary>
        /// The angular velocity.
        /// </summary>
        [PhysicsBrowse]
        public Vector3 AngularVelocity
        {
            get { return Entity.InternalAngularVelocity; }
            set { Entity.InternalAngularVelocity = value; }
        }

        /// <summary>
        /// The offset of the center of mass.
        /// </summary>
        [PhysicsBrowse]
        public Vector3 CenterOfMassOffset
        {
            get { return Entity.CenterOfMassOffset; }
            set { Entity.CenterOfMassOffset = value; }
        }

        /// <summary>
        /// The linear momentum.
        /// </summary>
        [PhysicsBrowse]
        public Vector3 LinearMomentum
        {
            get { return Entity.InternalLinearMomentum; }
            set { Entity.InternalLinearMomentum = value; }
        }

        /// <summary>
        /// The linear velocity.
        /// </summary>
        [PhysicsBrowse]
        public Vector3 LinearVelocity
        {
            get { return Entity.InternalLinearVelocity; }
            set { Entity.InternalLinearVelocity = value; }
        }

        /// <summary>
        /// The amount of allowed penetration.
        /// </summary>
        [PhysicsBrowse]
        public float AllowedPenetration
        {
            get { return Entity.AllowedPenetration; }
            set { Entity.AllowedPenetration = value; }
        }

        /// <summary>
        /// The angular damping.
        /// </summary>
        [PhysicsBrowse]
        public float AngularDamping
        {
            get { return Entity.AngularDamping; }
            set { Entity.AngularDamping = value; }
        }

        /// <summary>
        /// The bounciness (AKA, coefficient of restitution).
        /// </summary>
        [PhysicsBrowse]
        public float Bounciness
        {
            get { return Entity.Bounciness; }
            set { Entity.Bounciness = value; }
        }

        /// <summary>
        /// The extra margin of collision.
        /// </summary>
        [PhysicsBrowse]
        public float CollisionMargin
        {
            get { return Entity.CollisionMargin; }
            set { Entity.CollisionMargin = value; }
        }

        /// <summary>
        /// The density.
        /// </summary>
        [PhysicsBrowse]
        public float Density
        {
            get { return Entity.Density; }
            set { Entity.Density = value; }
        }

        /// <summary>
        /// The coefficient of kinetic friction.
        /// </summary>
        [PhysicsBrowse]
        public float DynamicFriction
        {
            get { return Entity.DynamicFriction; }
            set { Entity.DynamicFriction = value; }
        }

        /// <summary>
        /// The linear damping.
        /// </summary>
        [PhysicsBrowse]
        public float LinearDamping
        {
            get { return Entity.LinearDamping; }
            set { Entity.LinearDamping = value; }
        }

        /// <summary>
        /// The mass.
        /// </summary>
        [PhysicsBrowse]
        public float Mass
        {
            get { return Entity.Mass; }
            set { Entity.Mass = value; }
        }

        /// <summary>
        /// The coefficient of static friction.
        /// </summary>
        [PhysicsBrowse]
        public float StaticFriction
        {
            get { return Entity.StaticFriction; }
            set { Entity.StaticFriction = value; }
        }

        /// <summary>
        /// The physical volume.
        /// </summary>
        [PhysicsBrowse]
        public float Volume { get { return Entity.Volume; } }

        /// <summary>
        /// Is active in physics processing?
        /// </summary>
        [Browsable(false)]
        public bool Active
        {
            get { return Entity.IsActive; }
            set { Entity.IsActive = value; }
        }

        /// <summary>
        /// Is affected by gravity?
        /// </summary>
        [PhysicsBrowse]
        public bool AffectedByGravity
        {
            get { return Entity.IsAffectedByGravity; }
            set
            {
                if (Entity.IsAffectedByGravity == value) return; // OPTIMIZATION: avoid activating
                Entity.IsActive = true;
                Entity.IsAffectedByGravity = value;
            }
        }

        /// <summary>
        /// Is the entity physically amorphous?
        /// </summary>
        [Browsable(false)]
        public bool Amorphous { get { return Entity is AmorphousEntity; } }

        /// <summary>
        /// The physics entity.
        /// May be set to null (to reset it), but will never return null.
        /// </summary>
        [Browsable(false)]
        public Entity Entity
        {
            get { return _entity; }
            protected set
            {
                if (_entity == value) return; // OPTIMIZATION
                ChangeEntity(value ?? new AmorphousEntity());
            }
        }

        /// <summary>
        /// Apply an angular impulse.
        /// </summary>
        public void ApplyAngularImpulse(Vector3 impulse)
        {
            Entity.IsActive = true;
            Entity.ApplyAngularImpulse(ref impulse);
        }

        /// <summary>
        /// Apply an impulse at the given world position and world direction.
        /// </summary>
        public void ApplyImpulse(Vector3 position, Vector3 direction)
        {
            Entity.ApplyImpulse(position, direction, true);
        }

        /// <summary>
        /// Apply a linear impulse.
        /// </summary>
        public void ApplyLinearImpulse(Vector3 impulse)
        {
            Entity.IsActive = true;
            Entity.ApplyLinearImpulse(ref impulse);
        }

        /// <summary>
        /// Apply rotation.
        /// </summary>
        public void ApplyRotation(Quaternion rotation)
        {
            Entity.IsActive = true;
            Entity.ApplyQuaternion(ref rotation, true);
        }

        /// <summary>
        /// Test the entity for ray intersection.
        /// </summary>
        public bool RayTest(Vector3 origin, Vector3 direction, float maximumLength, bool withMargin, out Vector3 hitLocation, out Vector3 hitNormal, out float t)
        {
            return Entity.RayTest(origin, direction, maximumLength, withMargin, out hitLocation, out hitNormal, out t);
        }

        /// <summary>
        /// Get the world transform.
        /// </summary>
        /// <param name="transform"></param>
        public void GetWorldTransform(out Matrix transform)
        {
            transform = Entity.WorldTransform;
        }

        /// <summary>
        /// Get the world transform of a specified mount point.
        /// </summary>
        public void GetMountPointTransform(int mountPoint, out Matrix transform)
        {
            GetMountPointTransformHook(mountPoint, out transform);
        }

        /// <summary>
        /// Collect all the surfaces used to render the actor.
        /// </summary>
        public List<Surface> CollectSurfaces(List<Surface> surfaces)
        {
            XiHelper.ArgumentNullCheck(surfaces);
            return CollectSurfacesHook(surfaces);
        }

        /// <inheritdoc />
        protected override void Destroy(bool destroying)
        {
            if (destroying)
            {
                Game.Scene.RemoveActor(this);
                TearDownEntity();
            }
            base.Destroy(destroying);
        }

        /// <inheritdoc />
        protected override bool IsHidden(PropertyDescriptor property)
        {
            return
                base.IsHidden(property) ||
                (Amorphous && GetType().GetPropertyFast(property.Name).HasCustomAttributeFast(typeof(PhysicsBrowseAttribute)));
        }

        /// <inheritdoc />
        protected override void OnDeallocated()
        {
            base.OnDeallocated();
            EntityChanged = null;
        }

        /// <inheritdoc />
        protected override void VisualizeHook(GameTime gameTime)
        {
            base.VisualizeHook(gameTime);
            Entity.ForceBoundingBoxRefit(0);
        }

        /// <summary>
        /// Handle collecting all the surfaces used to render the actor.
        /// </summary>
        protected virtual List<Surface> CollectSurfacesHook(List<Surface> surfaces)
        {
            return surfaces;
        }

        /// <summary>
        /// Handle looking up the transform of the specified mount point.
        /// </summary>
        protected virtual void GetMountPointTransformHook(int mountPoint, out Matrix transform)
        {
            GetWorldTransform(out transform);
        }

        private void SetUpEntity()
        {
            _entity = new AmorphousEntity();
            _entity.IsActive = true;
            _entity.IsAffectedByGravity = true;
            _entity.Mass = 1024;
            _entity.Tag = this;
        }

        private void TearDownEntity()
        {
            _entity.Tag = null;
            _entity = null;
        }

        private void ChangeEntity(Entity newEntity)
        {
            Entity oldEntity = _entity;
            AssignPersistentProperties(newEntity, oldEntity);
            _entity = newEntity;
            EntityChanged.TryRaise(this, oldEntity);
            oldEntity.Tag = null;
        }

        private static void AssignPersistentProperties(Entity target, Entity source)
        {
            target.AllowedPenetration = source.AllowedPenetration;
            target.AngularDamping = source.AngularDamping;
            target.Bounciness = source.Bounciness;
            target.CenterOfMassOffset = source.CenterOfMassOffset;
            target.CollisionMargin = source.CollisionMargin;
            target.Density = source.Density;
            target.DynamicFriction = source.DynamicFriction;
            target.InternalMotionState = source.InternalMotionState;
            target.IsActive = source.IsActive;
            target.IsAffectedByGravity = source.IsAffectedByGravity;
            target.LinearDamping = source.LinearDamping;
            target.Mass = source.Mass;
            target.StaticFriction = source.StaticFriction;
            target.Tag = source.Tag;
        }

        private Vector3 AdjustPositionForCenterOfMassOffsetBug()
        {
            return Position += CenterOfMassOffset;
        }

        private Entity _entity;
        private Vector3 _orientationEularDegrees;
    }
}
