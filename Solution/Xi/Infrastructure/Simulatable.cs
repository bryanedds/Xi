using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// An object that can take part in a simulation, relate to other simulatable objects, and be
    /// manipulated by the editor.
    /// </summary>
    public abstract class Simulatable : Focusable
    {
        /// <summary>
        /// Create a Simulatable object.
        /// </summary>
        /// <param name="game">The game.</param>
        public Simulatable(XiGame game) : base(game) { }

#if MESSAGE_STUFF
        public string MessageFileName
        {
            get { return _messageFileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (_messageFileName == value) return; // OPTIMIZATION
                messages.Assign(CreateMessageDefinitions(value));
                _messageFileName = value;
            }
        }

        public void RegisterMessage(Simulatable source, string messageName, Action correspondingEvent)
        {
            string messageSource = source.GetMessagePath(this, messageName);
            correspondingEvent += () => Message.InvokeMessage(source, messages[messageSource]);
        }

        private static Dictionary<string, string> CreateMessageDefinitions(string messageFileName)
        {
            if (messageFileName.Length == 0) return new Dictionary<string, string>();
            XDocument messageDocument = XDocument.Load(messageFileName);
            IEnumerable<KeyValuePair<string, string>> messageDefinitions =
                from node in messageDocument.Nodes()
                let messageSource = node["Source"]
                let messageContent = node["Content"]
                select new KeyValuePair<string, string>(messageSource.Value, messageContent.Value);
            return messageDefinitions.ToDictionary();
        }

        private readonly Dictionary<string, string> messages = new Dictionary<string, string>();
        private string _messageFileName = string.Empty;
#endif

        /// <summary>
        /// Generalized data string that can be specified at design-time and interpreted by user
        /// code at play-time.
        /// </summary>
        public string DesignTimeData
        {
            get { return designTimeData; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                designTimeData = value;
            }
        }

        /// <summary>
        /// The simulatable parent. May be null.
        /// Throws ArgumentException if parent is set to an invalid type.
        /// </summary>
        [Browsable(false), IgnoreSerialization]
        public Simulatable SimulatableParent
        {
            get { return SimulatableParentHook; }
            internal set { SimulatableParentHook = value; }
        }

        /// <summary>
        /// The simulable children.
        /// Only use in editor as it generates garbage.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Simulatable> SimulatableChildren
        {
            get { return CollectSimulatableChildren(new List<Simulatable>()); }
        }

        /// <summary>
        /// The simulable peer objects.
        /// Only use in editor as it generates garbage.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Simulatable> SimulatablePeers
        {
            get
            {
                Simulatable parent = SimulatableParent;
                return
                    parent != null ?
                    parent.SimulatableChildren.Where(x => x != this) :
                    new Simulatable[0];
            }
        }

        /// <summary>
        /// The persistent simulatable children.
        /// Only use in editor as it generates garbage.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Simulatable> PersistentSimulatableChildren
        {
            get { return SimulatableChildren.Where(x => x.PersistentAsSimulatableChild); }
        }

        /// <summary>
        /// The name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (name == value) return;
                string oldName = name;
                name = value;
                Simulatable parent = SimulatableParent;
                if (parent != null) parent.NotifySimulatableChildNameChanged(this, oldName);
                Game.RaiseSimulationStructureChanged();
            }
        }

        /// <summary>
        /// The name if meaningful or a default based on type full name.
        /// </summary>
        [Browsable(false)]
        public string NameOrDefault { get { return HasMeaningfulName ? Name : "[" + GetType().FullName + "]"; } }

        /// <summary>
        /// The family name.
        /// </summary>
        [Browsable(false)]
        public string FamilyName
        {
            get
            {
                string ancestry = Ancestry;
                return ancestry.Length != 0 ? ancestry + "." + name : name;
            }
        }

        /// <summary>
        /// The ancestry.
        /// </summary>
        [Browsable(false)]
        public string Ancestry
        {
            get
            {
                Simulatable parent = SimulatableParent;
                return parent != null ? parent.FamilyName : string.Empty;
            }
        }

        [Browsable(false)]
        public bool PersistentAsSimulatableChild
        {
            get
            {
                return
                    Persistence == Persistence.Persistent &&
                    OwnedBySimulatableParent;
            }
        }

        /// <summary>
        /// Does the object have a meaningful name?
        /// </summary>
        [Browsable(false)]
        public bool HasMeaningfulName { get { return name.Length != 0; } }

        /// <summary>
        /// Is the object enabled?
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value) return;
                bool oldEnabled = enabled;
                enabled = value;
                try
                {
                    OnEnabledChanged();
                }
                catch
                {
                    enabled = oldEnabled;
                    throw;
                }
            }
        }

        /// <summary>
        /// Is the object visible?
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (visible == value) return;
                bool oldVisible = visible;
                visible = value;
                try
                {
                    OnVisibleChanged();
                }
                catch
                {
                    visible = oldVisible;
                    throw;
                }
            }
        }

        /// <summary>
        /// Is the object frozen in the editor?
        /// In other words, is the object excluded from selection in the editor view port?
        /// </summary>
        public bool Frozen
        {
            get { return frozen; }
            set
            {
                if (frozen == value) return; // OPTIMIZATION: avoid raising sim structure changed
                frozen = value;
                Game.RaiseSimulationStructureChanged();
            }
        }

        /// <summary>
        /// Is the object selected by the editor?
        /// Setting this clears the selection.
        /// Only set to true in editor as doing so generates garbage.
        /// </summary>
        [Browsable(false), IgnoreSerialization]
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (value) Select();
                else Deselect();
            }
        }

        /// <summary>
        /// Is the object selected exclusively among its peers?
        /// Only use in editor as it generates garbage.
        /// </summary>
        [Browsable(false), IgnoreSerialization]
        public bool SelectedExclusively
        {
            get
            {
                if (SimulatableParent == null) return true;
                return
                    SimulatableParent.Selection.HasOne() &&
                    SimulatableParent.Selection.Contains(this) &&
                    Selection.Empty();
            }
            set
            {
                ClearPeerSelection();
                Selected = value;
            }
        }

        /// <summary>
        /// The selected children.
        /// Only use in editor as it generates garbage.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Simulatable> Selection
        {
            get
            {
                foreach (Simulatable child in SimulatableChildren)
                    if (child.Selected)
                        yield return child;
            }
        }

        /// <summary>
        /// The selected descendants.
        /// Only use in editor as it generates garbage.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Simulatable> SelectionDescendant
        {
            get
            {
                bool atBottom = AtBottom;
                foreach (Simulatable child in Selection)
                {
                    yield return child;
                    if (atBottom) continue;
                    foreach (Simulatable grandChild in child.SelectionDescendant) yield return grandChild;
                }
            }
        }

        /// <summary>
        /// The bottom selected descendents.
        /// Only use in editor as it generates garbage.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Simulatable> SelectionBottom
        {
            get
            {
                bool atBottom = AtBottom;
                foreach (Simulatable child in Selection)
                {
                    if (atBottom) yield return child;
                    else foreach (Simulatable grandChild in child.SelectionBottom) yield return grandChild;
                }
            }
        }

        /// <summary>
        /// Is the simulatable selectable in the editor's view port?
        /// </summary>
        [Browsable(false)]
        public bool ViewSelectable { get { return Visible && !Frozen; } }

        /// <summary>
        /// Is the object owned by its simulatable parent?
        /// </summary>
        [Browsable(false), IgnoreSerialization]
        public bool OwnedBySimulatableParent
        {
            get { return ownedBySimulatableParent; }
            set { ownedBySimulatableParent = value; }
        }

        /// <summary>
        /// Get a simulatable child with the give name.
        /// May return null.
        /// </summary>
        public Simulatable GetSimulatableChild(string childName)
        {
            XiHelper.ArgumentNullCheck(childName);
            return GetSimulatableChildHook(childName);
        }

        /// <summary>
        /// Collect all the simulatable children.
        /// </summary>
        public List<Simulatable> CollectSimulatableChildren(List<Simulatable> result)
        {
            XiHelper.ArgumentNullCheck(result);
            CollectSimulatableChildrenHook(result);
            return result;
        }

        /// <summary>
        /// Get the simulatable relative, immediate or distant, using the given destination.
        /// May return null if the relative is not found.
        /// </summary>
        public Simulatable GetSimulatableRelative(string[] destinationParts)
        {
            XiHelper.ArgumentNullCheck(destinationParts);
            return GetSimulatableRelative(new Navigator(destinationParts, this));
        }

        /// <summary>
        /// Get the simulatable relative, immediate or distant, using the given navigator.
        /// May return null if the relative is not found.
        /// </summary>
        public Simulatable GetSimulatableRelative(Navigator navigator)
        {
            return
                navigator.IsTerminating ?
                navigator.Context :
                navigator.Context.GetSimulatableRelative(new Navigator(navigator));
        }

        /// <summary>
        /// Get the immediate relative with the given name.
        /// "." string returns the this reference and ".." returns parent.
        /// May return null if the given relative is not found.
        /// </summary>
        public Simulatable GetImmediateSimulatableRelative(string relativeName)
        {
            XiHelper.ArgumentNullCheck(relativeName);
            if (relativeName.Length == 0) return null;
            if (relativeName == ".") return this;
            if (relativeName == "..") return SimulatableParent;
            return GetSimulatableChild(relativeName);
        }

        /// <summary>
        /// Notify that a simulatable child's name has changed.
        /// </summary>
        public void NotifySimulatableChildNameChanged(Simulatable child, string oldName)
        {
            XiHelper.ArgumentNullCheck(child, oldName);
            OnSimulatableChildNameChanged(child, oldName);
        }

        /// <summary>
        /// Select all of the simulatable children.
        /// Only use in editor as it generates garbage.
        /// </summary>
        public void SelectChildren()
        {
            foreach (Simulatable child in SimulatableChildren) child.Selected = true;
        }

        /// <summary>
        /// Select all of the simulatable children that satisfy a predicate.
        /// Only use in editor as it generates garbage.
        /// </summary>
        public void SelectChildren(Func<Simulatable, bool> predicate)
        {
            foreach (Simulatable child in SimulatableChildren)
                if (predicate(child))
                    child.Selected = true;
        }

        /// <summary>
        /// Select self and all descendants.
        /// Only use in editor as it generates garbage.
        /// </summary>
        public void SelectAll()
        {
            Selected = true;
            foreach (Simulatable child in SimulatableChildren) child.SelectAll();
        }

        /// <summary>
        /// Clear the selection state of all children.
        /// </summary>
        public void ClearSelection()
        {
            foreach (Simulatable child in Selection) child.Selected = false;
        }

        /// <summary>
        /// Update during both editing and play.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            XiHelper.ArgumentNullCheck(gameTime);
            UpdateHook(gameTime);
        }

        /// <summary>
        /// Update during editing.
        /// </summary>
        public void Edit(GameTime gameTime)
        {
            XiHelper.ArgumentNullCheck(gameTime);
            EditHook(gameTime);
        }

        /// <summary>
        /// Update during play.
        /// </summary>
        public void Play(GameTime gameTime)
        {
            XiHelper.ArgumentNullCheck(gameTime);
            PlayHook(gameTime);
        }

        /// <summary>
        /// Synchronize the visuals to the physics.
        /// </summary>
        public void Visualize(GameTime gameTime)
        {
            XiHelper.ArgumentNullCheck(gameTime);
            VisualizeHook(gameTime);
        }

        /// <inheritdoc />
        protected override bool AcceptFocusHook
        {
            get { return Enabled && Visible; }
        }

        /// <summary>
        /// Handle getting and setting the simulatable parent.
        /// May be null.
        /// </summary>
        protected abstract Simulatable SimulatableParentHook { get; set; }

        protected override void OnDeallocated()
        {
            base.OnDeallocated();
            FocusIndex = null;
            OwnedBySimulatableParent = false;
            Selected = false;
        }

        protected override void OnAllocated()
        {
            base.OnAllocated();
            RefreshOverlaidProperties();
        }

        /// <summary>
        /// Handle getting a simulatable child.
        /// May return null.
        /// </summary>
        protected abstract Simulatable GetSimulatableChildHook(string childName);

        /// <summary>
        /// Handle collecting the simulatable children.
        /// </summary>
        protected abstract void CollectSimulatableChildrenHook(List<Simulatable> result);

        /// <summary>
        /// Handle the fact that a simulatable child's name has changed.
        /// </summary>
        protected virtual void OnSimulatableChildNameChanged(Simulatable child, string oldName) { }

        /// <summary>
        /// Handle updating during both editing and play.
        /// </summary>
        protected virtual void UpdateHook(GameTime gameTime) { }

        /// <summary>
        /// Handle updating during editing.
        /// </summary>
        protected virtual void EditHook(GameTime gameTime) { }

        /// <summary>
        /// Handle updating during play.
        /// </summary>
        protected virtual void PlayHook(GameTime gameTime) { }

        /// <summary>
        /// Handle synchronizing visuals to phsyics.
        /// </summary>
        protected virtual void VisualizeHook(GameTime gameTime) { }

        /// <summary>
        /// Handle a change in enabledness.
        /// </summary>
        protected virtual void OnEnabledChanged()
        {
            ValidateEnabledCanChange();
        }

        /// <summary>
        /// Handle a change in visibility.
        /// </summary>
        protected virtual void OnVisibleChanged()
        {
            ValidateVisibleCanChange();
        }

        /// <summary>
        /// Handle removing a simulatable child without disposing it.
        /// </summary>
        protected abstract void RemoveSimulatableChildWithoutDisposingHook(Simulatable child);

        internal void SelectUpward()
        {
            Simulatable parent = SimulatableParent;
            if (parent != null)
            {
                parent.ClearPeerSelection();
                parent.SelectUpward();
            }
            _selected = true;
            Game.RaiseSimulationSelectionChanged();
        }

        internal void ClearPeerSelection()
        {
            foreach (Simulatable peer in SimulatablePeers) peer.Selected = false;
        }

        internal void RemoveSimulatableChildWithoutDisposing(Simulatable child)
        {
            RemoveSimulatableChildWithoutDisposingHook(child);
        }

        private bool AtBottom
        {
            get
            {
                IEnumerable<Simulatable> selection = Selection;
                return
                    selection.HasOne() &&
                    selection.First().Selection.FirstOrDefault() == null ||
                    selection.Count() > 1;
            }
        }

        private void Select()
        {
            ClearSelection();
            SelectUpward();
        }

        private void Deselect()
        {
            ClearSelection();
            _selected = false;
            Game.RaiseSimulationSelectionChanged();
        }

        private void ValidateEnabledCanChange()
        {
            if (Focused)
                throw new InvalidOperationException("Cannot change enabled-ness while focused.");
        }

        private void ValidateVisibleCanChange()
        {
            if (Focused)
                throw new InvalidOperationException("Cannot change visibility while focused.");
        }

        private string name = string.Empty;
        private bool enabled = true;
        private bool visible = true;
        private bool frozen = false;
        private bool ownedBySimulatableParent;
        private string designTimeData = string.Empty;
        private bool _selected;
    }
}
