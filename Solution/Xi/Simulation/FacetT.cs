using System;

namespace Xi
{
    /// <summary>
    /// A facet with a fully-specified actor.
    /// </summary>
    /// <typeparam name="T">The name of the minimum required actor type.</typeparam>
    public class Facet<T> : Facet where T : Actor
    {
        /// <summary>
        /// Create a Facet.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="exposed">Is the facet 'exposed' by default?</param>
        public Facet(XiGame game, bool exposed) : base(game, exposed) { }

        /// <inheritdoc />
        protected internal override Actor ActorBaseHook
        {
            get { return actor; }
        }

        /// <inheritdoc />
        protected override Simulatable SimulatableParentHook
        {
            get { return actor; }
            set
            {
                ValidateActor(value);
                actor = XiHelper.Cast<T>(value);
                base.SimulatableParentHook = value;
            }
        }

        /// <summary>
        /// The actor to which we're attached.
        /// May be null except in the context of UpdateHook, PlayHook, and EditHook.
        /// </summary>
        protected T Actor { get { return actor; } }

        private static void ValidateActor(Simulatable actor)
        {
            if (actor != null && !(actor is T))
                throw new ArgumentException(
                    "Actor of type '" + actor.GetType().FullName +
                    "' is not of required type '" + typeof(T).FullName +
                    "'.");
        }

        private T actor;
    }
}
