using System;
using System.ComponentModel;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// A two-dimensional actor.
    /// </summary>
    public abstract class Actor2D : Actor
    {
        /// <summary>
        /// Create an Actor2D.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="physicsEnabled">Is the actor's physics enabled?</param>
        public Actor2D(XiGame game, bool physicsEnabled) : base(game)
        {
            _physicsEnabled = physicsEnabled; // circumvent property to avoid virtual call
            SetUpFixture();
        }

        /// <summary>
        /// Raised when the fixture has collided.
        /// </summary>
        public event CollisionEventHandler Collided
        {
            add { Fixture.OnCollision += value; }
            remove { Fixture.OnCollision -= value; }
        }

        /// <summary>
        /// Raised when the fixture has separated.
        /// </summary>
        public event SeparationEventHandler Separated
        {
            add { Fixture.OnSeparation += value; }
            remove { Fixture.OnSeparation -= value; }
        }

        /// <summary>
        /// Raised after the fixture's physics solution is reached.
        /// </summary>
        public event Action<ContactConstraint> Solved
        {
            add { Fixture.PostSolve += value; }
            remove { Fixture.PostSolve -= value; }
        }

        /// <summary>
        /// Raised when the fixture has been changed.
        /// </summary>
        public event Action<Actor2D, Fixture> FixtureChanged;

        /// <summary>
        /// Which fixtures are collided with?
        /// </summary>
        [PhysicsBrowse]
        public CollisionCategory CollidesWith
        {
            get { return Fixture.CollidesWith; }
            set { Fixture.CollidesWith = value; }
        }

        /// <summary>
        /// The fixture's collision categories.
        /// </summary>
        [PhysicsBrowse]
        public CollisionCategory CollisionCategories
        {
            get { return Fixture.CollisionCategories; }
            set { Fixture.CollisionCategories = value; }
        }

        /// <summary>
        /// The position.
        /// </summary>
        public Vector3 Position
        {
            get { return new Vector3(Body.Position, positionZ); }
            set
            {
                Body.Position = value.GetXY();
                positionZ = value.Z;
            }
        }

        /// <summary>
        /// The XY position.
        /// </summary>
        [IgnoreSerialization]
        public Vector2 PositionXY
        {
            get { return Position.GetXY(); }
            set { Position = new Vector3(value, PositionZ); }
        }

        /// <summary>
        /// The Z position.
        /// </summary>
        [IgnoreSerialization]
        public float PositionZ
        {
            get { return Position.Z; }
            set { Position = new Vector3(PositionXY, value); }
        }

        /// <summary>
        /// The size.
        /// </summary>
        [Browsable(false)]
        public Vector2 Size { get { return SizeHook; } }

        /// <summary>
        /// The rotation in degrees.
        /// </summary>
        [IgnoreSerialization]
        public float RotationDegrees
        {
            get { return MathHelper.ToDegrees(Rotation); }
            set { Rotation = MathHelper.ToRadians(value); }
        }

        /// <summary>
        /// The rotation in radians.
        /// </summary>
        public float Rotation
        {
            get { return Body.Rotation; }
            set { Body.Rotation = value; }
        }

        /// <summary>
        /// The linear velocity.
        /// </summary>
        [PhysicsBrowse]
        public Vector2 LinearVelocity
        {
            get { return Body.LinearVelocity; }
            set { Body.LinearVelocity = value; }
        }

        /// <summary>
        /// The local center of gravity.
        /// </summary>
        [PhysicsBrowse]
        public Vector2 LocalCenter
        {
            get { return Body.LocalCenter; }
            set { Body.LocalCenter = value; }
        }

        /// <summary>
        /// The angular damping.
        /// </summary>
        [PhysicsBrowse]
        public float AngularDamping
        {
            get { return Body.AngularDamping; }
            set { Body.AngularDamping = value; }
        }

        /// <summary>
        /// The angular velocity.
        /// </summary>
        [PhysicsBrowse]
        public float AngularVelocity
        {
            get { return Body.AngularVelocity; }
            set { Body.AngularVelocity = value; }
        }

        /// <summary>
        /// The density.
        /// </summary>
        [PhysicsBrowse]
        public float Density
        {
            get { return Fixture.Density; }
            set { Fixture.Density = value; }
        }

        /// <summary>
        /// The friction rating.
        /// </summary>
        [PhysicsBrowse]
        public float Friction
        {
            get { return Fixture.Friction; }
            set { Fixture.Friction = value; }
        }

        /// <summary>
        /// The inertia.
        /// </summary>
        [PhysicsBrowse]
        public float Inertia
        {
            get { return Body.Inertia; }
            set { Body.Inertia = value; }
        }

        /// <summary>
        /// The linear damping.
        /// </summary>
        [PhysicsBrowse]
        public float LinearDamping
        {
            get { return Body.LinearDamping; }
            set { Body.LinearDamping = value; }
        }

        /// <summary>
        /// The mass.
        /// </summary>
        [IgnoreSerialization, PhysicsBrowse]
        public float Mass
        {
            get { return Body.Mass; }
            set { Body.Mass = value; }
        }

        /// <summary>
        /// The restitution.
        /// </summary>
        [PhysicsBrowse]
        public float Restitution
        {
            get { return Fixture.Restitution; }
            set { Fixture.Restitution = value; }
        }

        /// <summary>
        /// Is affected by gravity?
        /// </summary>
        [PhysicsBrowse]
        public bool AffectedByGravity
        {
            get { return !Body.IgnoreGravity; }
            set
            {
                if (Body.IgnoreGravity == !value) return; // OPTIMIZATION: avoid waking
                Body.IgnoreGravity = !value;
                Body.Awake = true;
            }
        }

        /// <summary>
        /// Is awake to physics processing?
        /// </summary>
        [Browsable(false), PhysicsBrowse]
        public bool Awake
        {
            get { return Body.Awake; }
            set { Body.Awake = value; }
        }

        /// <summary>
        /// Is 'bullet' collision enabled?
        /// </summary>
        [PhysicsBrowse]
        public bool CCDEnabled
        {
            get { return Body.IsBullet; }
            set { Body.IsBullet = value; }
        }

        /// <summary>
        /// Is the rotation fixed?
        /// </summary>
        [PhysicsBrowse]
        public bool FixedRotation
        {
            get { return Body.FixedRotation; }
            set { Body.FixedRotation = value; }
        }

        /// <summary>
        /// Is a sensor-only (EG, has no collision response)?
        /// </summary>
        [PhysicsBrowse]
        public bool Sensor
        {
            get { return Fixture.IsSensor; }
            set { Fixture.IsSensor = value; }
        }

        /// <summary>
        /// Is the actor's phyiscs enabled?
        /// </summary>
        public bool PhysicsEnabled
        {
            get { return _physicsEnabled; }
            set
            {
                _physicsEnabled = value;
                RefreshBodyActive();
                Game.RaiseSimulationSelectionChanged(); // refresh property grid
            }
        }

        /// <summary>
        /// The physics fixture.
        /// </summary>
        [Browsable(false), PhysicsBrowse]
        public Fixture Fixture
        {
            get { return _fixture; }
            protected set
            {
                XiHelper.ArgumentNullCheck(value);
                if (_fixture == value) return; // OPTIMIZATION
                ChangeFixture(value);
            }
        }

        /// <summary>
        /// Apply an angular impulse.
        /// </summary>
        public void ApplyAngularImpulse(float impulse)
        {
            Body.ApplyAngularImpulse(impulse);
        }

        /// <summary>
        /// Apply a linear impulse.
        /// </summary>
        /// <param name="impulse">The impulse vector.</param>
        /// <param name="point">The world position of impulse application.</param>
        public void ApplyLinearImpulse(Vector2 impulse, Vector2 point)
        {
            Body.ApplyLinearImpulse(impulse, point);
        }

        /// <summary>
        /// Apply a force.
        /// </summary>
        /// <param name="force">The force vector.</param>
        public void ApplyForce(Vector2 force)
        {
            Body.ApplyForce(force);
        }

        /// <summary>
        /// Apply a force.
        /// </summary>
        /// <param name="force">The force vector.</param>
        /// <param name="point">The world position of force application.</param>
        public void ApplyForce(Vector2 force, Vector2 point)
        {
            Body.ApplyForce(force, point);
        }

        /// <summary>
        /// Apply torque.
        /// </summary>
        /// <param name="torque">The torque about the z-axis.</param>
        public void ApplyTorque(float torque)
        {
            Body.ApplyTorque(torque);
        }

        /// <summary>
        /// Is the actor's fixture located at a given world point?
        /// </summary>
        public bool IsAt(Vector2 point)
        {
            return Fixture.TestPoint(ref point);
        }

        /// <summary>
        /// The physics body.
        /// </summary>
        protected Body Body { get { return Fixture.Body; } }

        /// <summary>
        /// Handle getting the size.
        /// </summary>
        protected abstract Vector2 SizeHook { get; }

        /// <inheritdoc />
        protected override void Destroy(bool destroying)
        {
            if (destroying) TearDownFixture();
            base.Destroy(destroying);
        }

        /// <inheritdoc />
        protected override bool IsHidden(PropertyDescriptor property)
        {
            return
                base.IsHidden(property) ||
                !PhysicsEnabled &&
                GetType().GetPropertyFast(property.Name).HasCustomAttributeFast(typeof(PhysicsBrowseAttribute));
        }

        /// <inheritdoc />
        protected override void OnDeallocated()
        {
            base.OnDeallocated();
            Fixture.OnCollision = null;
            Fixture.OnSeparation = null;
            Fixture.PostSolve = null;
            FixtureChanged = null;
            RefreshBodyActive();
        }

        /// <inheritdoc />
        protected override void OnAllocated()
        {
            base.OnAllocated();
            RefreshBodyActive();
        }

        /// <inheritdoc />
        protected override void OnEnabledChanged()
        {
            base.OnEnabledChanged();
            RefreshBodyActive();
        }

        private void SetUpFixture()
        {
            _fixture = FixtureFactory.CreateRectangle(Game.World, Size.X, Size.Y, 1);
            _fixture.Body.BodyType = BodyType.Dynamic;
            _fixture.Body.IgnoreGravity = false;
        }

        private void TearDownFixture()
        {
            Game.World.RemoveBody(_fixture.Body);
            _fixture.UserData = null;
            _fixture = null;
        }

        private void ChangeFixture(Fixture newFixture)
        {
            Fixture oldFixture = _fixture;
            AssignPersistentProperties(newFixture, oldFixture);
            AssignEvents(newFixture, oldFixture);
            _fixture = newFixture;
            FixtureChanged.TryRaise(this, oldFixture);
            Game.World.RemoveBody(oldFixture.Body);
            oldFixture.UserData = null;
        }

        private void AssignPersistentProperties(Fixture target, Fixture source)
        {
            Body sourceBody = source.Body;
            Body targetBody = target.Body;
            targetBody.Active = sourceBody.Active;
            targetBody.Awake = sourceBody.Awake;
            targetBody.AngularDamping = sourceBody.AngularDamping;
            targetBody.AngularVelocity = sourceBody.AngularVelocity;
            targetBody.BodyType = sourceBody.BodyType;
            targetBody.FixedRotation = sourceBody.FixedRotation;
            targetBody.IgnoreGravity = sourceBody.IgnoreGravity;
            targetBody.Inertia = sourceBody.Inertia;
            targetBody.IsBullet = sourceBody.IsBullet;
            targetBody.LinearDamping = sourceBody.LinearDamping;
            targetBody.LinearVelocity = sourceBody.LinearVelocity;
            targetBody.LocalCenter = sourceBody.LocalCenter;
            targetBody.Position = sourceBody.Position;
            targetBody.Rotation = sourceBody.Rotation;
            targetBody.SleepingAllowed = Game.Playing;
            target.CollidesWith = source.CollidesWith;
            target.CollisionCategories = source.CollisionCategories;
            target.Density = source.Density;
            target.Friction = source.Friction;
            target.IsSensor = source.IsSensor;
            target.Restitution = source.Restitution;
            target.UserData = source.UserData;
        }

        private void AssignEvents(Fixture target, Fixture source)
        {
            target.OnCollision = source.OnCollision;
            target.OnSeparation = source.OnSeparation;
            target.PostSolve = source.PostSolve;
        }

        private void RefreshBodyActive()
        {
            Body.Active = PhysicsEnabled && Enabled && Allocated;
        }

        private float positionZ;
        private bool _physicsEnabled = true;
        private Fixture _fixture;
    }
}
