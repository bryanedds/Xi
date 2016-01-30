namespace Xi
{
    /// <summary>
    /// An object's persistence strategy.
    /// </summary>
    public enum Persistence
    {
        /// <summary>
        /// The object is to persist.
        /// </summary>
        Persistent = 0,
        /// <summary>
        /// The object is not to persist.
        /// </summary>
        Transient,
        /// <summary>
        /// The object requires special assistance to persist properly.
        /// </summary>
        Assisted
    }
}
