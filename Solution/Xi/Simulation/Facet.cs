using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// Attaches a behavior to an actor.
    /// </summary>
    public abstract class Facet : Simulatable<Actor, Simulatable>
    {
        /// <summary>
        /// Create a Facet.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="exposed">Is the facet 'exposed' by default?</param>
        public Facet(XiGame game, bool exposed) : base(game)
        {
            if (exposed) Name = GetType().FullName;
        }

        /// <summary>
        /// The actor as a base Actor type.
        /// May be null except in the context of UpdateHook, EditHook, PlayHook, VisualizeHook, or
        /// InputHook.
        /// </summary>
        [Browsable(false), IgnoreSerialization]
        public Actor ActorBase { get { return ActorBaseHook; } }

        /// <summary>
        /// The actor.
        /// May be null except in the context of UpdateHook, EditHook, PlayHook, or VisualizeHook.
        /// </summary>
        protected internal abstract Actor ActorBaseHook { get; }

        /// <summary>
        /// Process input.
        /// </summary>
        public void Input(GameTime gameTime, PlayerIndex focusIndex)
        {
            XiHelper.ArgumentNullCheck(gameTime);
            ValidateActor();
            InputHook(gameTime, focusIndex);
        }

        /// <inheritdoc />
        protected override bool AcceptFocusHook { get { return false; } }

        /// <inheritdoc />
        protected override void UpdateHook(GameTime gameTime)
        {
            ValidateActor();
            base.UpdateHook(gameTime);
        }

        /// <inheritdoc />
        protected override void EditHook(GameTime gameTime)
        {
            ValidateActor();
            base.EditHook(gameTime);
        }

        /// <inheritdoc />
        protected override void PlayHook(GameTime gameTime)
        {
            ValidateActor();
            base.PlayHook(gameTime);
        }

        /// <inheritdoc />
        protected override void VisualizeHook(GameTime gameTime)
        {
            ValidateActor();
            base.VisualizeHook(gameTime);
        }

        /// <summary>
        /// Handle taking input.
        /// </summary>
        protected virtual void InputHook(GameTime gameTime, PlayerIndex focusIndex) { }

        private void ValidateActor()
        {
            if (ActorBase == null)
                throw new InvalidOperationException("Cannot update, edit, play, or visualize a facet while its actor is null.");
        }
    }
}
