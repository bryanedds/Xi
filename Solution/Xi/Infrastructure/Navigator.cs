using System;

namespace Xi
{
    /// <summary>
    /// A relationship navigator.
    /// OPTIMIZATION: implemented as a struct to avoid generating garbage.
    /// </summary>
    public struct Navigator
    {
        /// <summary>
        /// Create a child Navigator of the given parent.
        /// </summary>
        public Navigator(Navigator parent)
        {
            ValidateParentIsNonTerminating(parent);
            destinationParts = parent.DestinationParts;
            destinationPartIndex = parent.DestinationPartIndex + 1;
            context = parent.Context.GetImmediateSimulatableRelative(destinationParts[destinationPartIndex]);
        }

        /// <summary>
        /// Create a Navigator.
        /// </summary>
        /// <param name="destinationParts">The destination split into string parts. May not be null.</param>
        /// <param name="context">The navigation context. May not be null.</param>
        public Navigator(string[] destinationParts, Simulatable context)
        {
            XiHelper.ArgumentNullCheck(destinationParts, context);
            this.destinationParts = destinationParts;
            this.destinationPartIndex = -1;
            this.context = context;
        }

        /// <summary>
        /// Is the navigator terminating?
        /// </summary>
        public bool IsTerminating
        {
            get
            {
                return
                    context == null ||
                    destinationPartIndex >= destinationParts.Length - 1;
            }
        }

        /// <summary>
        /// The destination split into string parts.
        /// May be null due only to the .net struct's required default ctor.
        /// </summary>
        public string[] DestinationParts { get { return destinationParts; } }

        /// <summary>
        /// The index of the current destination part.
        /// May be -1.
        /// </summary>
        public int DestinationPartIndex { get { return destinationPartIndex; } }

        /// <summary>
        /// The navigation context.
        /// May be null.
        /// </summary>
        public Simulatable Context { get { return context; } }

        private static void ValidateParentIsNonTerminating(Navigator parent)
        {
            if (parent.IsTerminating)
                throw new ArgumentException("Cannot make a child from a terminating navigator.");
        }

        /// <summary>May be null.</summary>
        private readonly Simulatable context;
        /// <summary>May be null.</summary>
        private readonly string[] destinationParts;
        private readonly int destinationPartIndex;
    }
}
