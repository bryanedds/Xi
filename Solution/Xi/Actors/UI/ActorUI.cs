using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// An actor that implements a user interface.
    /// </summary>
    public abstract class ActorUI : Actor
    {
        /// <summary>
        /// Create an ActorUI.
        /// </summary>
        /// <param name="game">The game.</param>
        public ActorUI(XiGame game) : base(game) { }

        /// <summary>
        /// The position.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
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
        /// The bounding rectangle.
        /// </summary>
        [Browsable(false)]
        public Rectangle Bounds
        {
            get
            {
                Vector3 position = Position;
                Vector2 size = Size;
                return new Rectangle(
                    (int)(position.X - size.X * 0.5f),
                    (int)(position.Y - size.Y * 0.5f),
                    (int)(size.X),
                    (int)(size.Y));
            }
        }

        /// <summary>
        /// Handle getting the size.
        /// </summary>
        protected abstract Vector2 SizeHook { get; }

        /// <inheritdoc />
        protected override void OnDirectionInput(GameTime gameTime, InputType inputType, Direction2D direction)
        {
            base.OnDirectionInput(gameTime, inputType, direction);
            if (CanProcessTransferFocusInput &&
                (inputType == InputType.ClickDown ||
                 inputType == InputType.Repeat))
                ProcessTransferFocusInput(direction);
        }

        private bool CanProcessTransferFocusInput { get { return ActorGroup != null; } }

        private void ProcessTransferFocusInput(Direction2D direction)
        {
            // NOTE: this method is highly imperative so that actor ui lists can be recycled to
            // avoid generating garbage.
            ActorGroup actorGroup = GrabActorGroup<ActorGroup>();
            actorGroup.CollectActors<ActorUI>(cachedActorUIs);
            cachedActorUIs.GetActorUIsThatAcceptFocus(cachedActorUIs2);
            cachedActorUIs.Clear();
            cachedActorUIs2.GetActorUIsInDirection(cachedActorUIs, this, direction);
            cachedActorUIs2.Clear();
            cachedActorUIs.SortActorUIsInDirection(this, direction);
            ActorUI target = cachedActorUIs.FirstOrDefault();
            cachedActorUIs.Clear();
            if (target != null) target.FocusIndex = FocusIndex;
        }

        private Vector3 position;
        private readonly List<ActorUI> cachedActorUIs = new List<ActorUI>();
        private readonly List<ActorUI> cachedActorUIs2 = new List<ActorUI>();
    }
}
