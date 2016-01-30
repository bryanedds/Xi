namespace Xi
{
    /// <summary>
    /// Augments Surface with a generically-typed actor reference.
    /// </summary>
    public abstract class Surface<T> : Surface where T : Actor3D
    {
        /// <summary>
        /// Initialize a Surface3D.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="actor">The parent actor.</param>
        /// <param name="effectFileName">The effect file name.</param>
        public Surface(XiGame game, T actor, string effectFileName)
            : base(game, actor, effectFileName)
        {
            this.actor = actor;
        }

        /// <inheritdoc />
        protected new T Actor { get { return actor; } }

        private T actor;
    }
}
