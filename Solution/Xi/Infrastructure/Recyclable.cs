using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// An object that can be recycled to avoid garbage generation at play-time.
    /// </summary>
    public abstract class Recyclable : Serializable
    {
        /// <summary>
        /// Create a Recyclable object.
        /// </summary>
        /// <param name="game">The game.</param>
        public Recyclable(XiGame game) : base(game) { }

        /// <summary>
        /// Is recycling enabled for this object?
        /// </summary>
        [Browsable(false)]
        public bool RecyclingEnabled { get { return recycleBinName.Length != 0; } }

        /// <summary>
        /// The recycle bin name.
        /// TODO: expand on this.
        /// </summary>
        public string RecycleBinName
        {
            get { return recycleBinName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                recycleBinName = value;
            }
        }

        /// <inheritdoc />
        protected sealed override void Dispose(bool disposing)
        {
            if (disposing && RecyclingEnabled && allocated) Game.Recycler.Deallocate(this);
            else Destroy(disposing);
        }

        /// <summary>
        /// Handle destroying the object.
        /// Replaces the Dispose hook for recyclable objects.
        /// </summary>
        /// <param name="destroying">Is the object being destroyed manually?</param>
        protected virtual void Destroy(bool destroying)
        {
            base.Dispose(destroying);
        }

        /// <summary>
        /// Handle the fact that the object has just been deallocated from use.
        /// </summary>
        protected virtual void OnDeallocated() { }

        /// <summary>
        /// Handle the fact that the object has just been allocated for use.
        /// </summary>
        protected virtual void OnAllocated() { }

        internal bool Allocated
        {
            get { return allocated; }
            set
            {
                ValidateAllocatedChanging(value);
                allocated = value;
                if (allocated) OnAllocated();
                else OnDeallocated();
            }
        }

        private void ValidateAllocatedChanging(bool value)
        {
            if (allocated == value)
                throw new ArgumentException("Property 'Allocated' set redundantly.");
        }

        private bool allocated = true;
        private string recycleBinName = string.Empty;
    }
}
