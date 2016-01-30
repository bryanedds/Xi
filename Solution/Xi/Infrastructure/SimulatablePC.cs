using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// A partial generic implementation of a Simulatable object.
    /// </summary>
    /// <typeparam name="P">The parent type.</typeparam>
    /// <typeparam name="C">The child type.</typeparam>
    public class Simulatable<P, C> : Simulatable
        where P : Simulatable
        where C : Simulatable
    {
        /// <summary>
        /// Create a Simulatable object.
        /// </summary>
        /// <param name="game">The game.</param>
        public Simulatable(XiGame game) : base(game) { }

        /// <summary>
        /// Get the first simulatable child or null.
        /// May return null, obviously.
        /// </summary>
        protected internal C FirstSimulatableChildOrDefault
        {
            get { return simulatableChildren.FirstOrDefault; }
        }

        /// <summary>
        /// The simulatable parent.
        /// May be null.
        /// </summary>
        protected internal new P SimulatableParent
        {
            get { return simulatableParent; }
            internal set
            {
                if (simulatableParent == value) return;
                P oldSimulatableParent = simulatableParent;
                simulatableParent = value;
                OnSimulatableParentChanged(oldSimulatableParent);
            }
        }

        /// <summary>
        /// The simulatable children.
        /// Do NOT mutate this collection - it is intended to be immutable.
        /// </summary>
        protected internal new QueriableSimulatables<C> SimulatableChildren
        {
            get { return simulatableChildren; }
        }

        /// <summary>
        /// Get the simulatable parent as type T.
        /// May return null.
        /// </summary>
        protected internal T GetSimulatableParent<T>() where T : class
        {
            return simulatableParent as T;
        }

        /// <summary>
        /// Grab the simulatable parent as type T.
        /// Will not return null.
        /// </summary>
        protected internal T GrabSimulatableParent<T>() where T : class
        {
            if (simulatableParent == null) throw new InvalidOperationException("Failed to grab parent.");
            return XiHelper.Cast<T>(simulatableParent);
        }

        /// <summary>
        /// Get the first simulatable child of type T where 'Name == typeof(T).FullName'.
        /// May return null.
        /// </summary>
        protected internal T GetSimulatableChild<T>() where T : class
        {
            return simulatableChildren.Get<T>();
        }

        /// <summary>
        /// Get the first simulatable child of type T with the given name.
        /// May return null.
        /// </summary>
        protected internal T GetSimulatableChild<T>(string name) where T : class
        {
            XiHelper.ArgumentNullCheck(name);
            return simulatableChildren.Get<T>(name);
        }

        /// <summary>
        /// Grab the first simulatable child of type T where 'Name == typeof(T).FullName'.
        /// Will not return null.
        /// </summary>
        protected internal T GrabSimulatableChild<T>() where T : class
        {
            return simulatableChildren.Grab<T>();
        }

        /// <summary>
        /// Grab the first simulatable child of type T with the given name.
        /// Will not return null.
        /// </summary>
        protected internal T GrabSimulatableChild<T>(string name) where T : class
        {
            XiHelper.ArgumentNullCheck(name);
            return simulatableChildren.Grab<T>(name);
        }

        /// <summary>
        /// Collect all the simulatable children of type T.
        /// </summary>
        protected internal List<T> CollectSimulatableChildren<T>(List<T> result) where T : class
        {
            XiHelper.ArgumentNullCheck(result);
            return simulatableChildren.Collect(result);
        }

        /// <summary>
        /// Collect all the simulatable children of type T that satisfy the given predicate.
        /// </summary>
        protected internal List<T> CollectSimulatableChildren<T>(Func<T, bool> predicate, List<T> result) where T : class
        {
            XiHelper.ArgumentNullCheck(predicate, result);
            return simulatableChildren.Collect(predicate, result);
        }

        /// <summary>
        /// Add a simulatable child.
        /// </summary>
        protected internal bool AddSimulatableChild(C child)
        {
            XiHelper.ArgumentNullCheck(child);
            bool childAdded = AddSimulatableChildEventless(child);
            if (childAdded) Game.RaiseSimulationStructureChanged();
            return childAdded;
        }

        /// <summary>
        /// Remove a simulatable child.
        /// Child will be disposed if OwnedBySimulatableParent is true.
        /// </summary>
        protected internal bool RemoveSimulatableChild(C child)
        {
            XiHelper.ArgumentNullCheck(child);
            bool childRemoved = RemoveSimulatableChildEventless(child);
            if (childRemoved) Game.RaiseSimulationStructureChanged();
            return childRemoved;
        }

        /// <summary>
        /// Remove a simulatable child without disposing regardless if OwnedBySimulatableParent is
        /// true.
        /// </summary>
        protected internal bool RemoveSimulatableChildWithoutDisposing(C child)
        {
            XiHelper.ArgumentNullCheck(child);
            bool ownedByParent = OwnedBySimulatableParent;
            child.OwnedBySimulatableParent = false;
            bool result = RemoveSimulatableChild(child);
            child.OwnedBySimulatableParent = ownedByParent;
            return result;
        }

        /// <summary>
        /// Remove the simulatable children.
        /// Children will be disposed if OwnedBySimulatableParent is true.
        /// </summary>
        protected internal bool RemoveSimulatableChildren(List<C> children)
        {
            XiHelper.ArgumentNullCheck(children);
            bool childsRemoved = RemoveSimulatableChildrenEventless(children);
            if (childsRemoved) Game.RaiseSimulationStructureChanged();
            return childsRemoved;
        }

        /// <summary>
        /// Clear the simulatable children.
        /// Children will be disposed if OwnedBySimulatableParent is true.
        /// </summary>
        protected internal void ClearSimulatableChildren()
        {
            if (ClearSimulatableChildrenEventless())
                Game.RaiseSimulationStructureChanged();
        }

        /// <summary>
        /// Change the parent of the simulatable children.
        /// </summary>
        protected internal void ReparentSimulatableChildren(Simulatable<P, C> parent)
        {
            if (parent != this)
                while (simulatableChildren.Count != 0)
                    parent.AddSimulatableChild(simulatableChildren.First());
        }

        /// <summary>
        /// Does contain the simulatable child of type T where 'Name == typeof(T).FullName'?
        /// </summary>
        protected internal bool ContainsSimulatableChild<T>() where T : class
        {
            return simulatableChildren.Contains<T>();
        }

        /// <summary>
        /// Does contain the simulatable child of type T with the given name?
        /// </summary>
        protected internal bool ContainsSimulatableChild<T>(string name) where T : class
        {
            XiHelper.ArgumentNullCheck(name);
            return simulatableChildren.Contains<T>(name);
        }

        /// <summary>
        /// Does contain the given simulatable child?
        /// </summary>
        protected internal bool ContainsSimulatableChild(C child)
        {
            XiHelper.ArgumentNullCheck(child);
            return simulatableChildren.Contains(child);
        }

        /// <summary>
        /// Create (and add) a simulatable child of minimum type T that is of given type name.
        /// </summary>
        protected internal T CreateSimulatableChild<T>(string typeName) where T : C
        {
            XiHelper.ArgumentNullCheck(typeName);
            T simulatableChild = Game.CreateRecyclable<T>(typeName);
            simulatableChild.RefreshOverlaidProperties();
            simulatableChild.OwnedBySimulatableParent = true;
            AddSimulatableChild(simulatableChild);
            return simulatableChild;
        }

        /// <summary>
        /// Create (and add) a simulatable child of minimum type T from the given object definition.
        /// </summary>
        protected internal T CreateSimulatableChildFromDefinition<T>(string simulatableChildDefinition) where T : C
        {
            XiHelper.ArgumentNullCheck(simulatableChildDefinition);
            T simulatableChild = Game.CreateRecyclableFromDefinition<T>(simulatableChildDefinition);
            simulatableChild.RefreshOverlaidProperties();
            simulatableChild.OwnedBySimulatableParent = true;
            AddSimulatableChild(simulatableChild);
            return simulatableChild;
        }

        /// <summary>
        /// Create (and add) a simulatable child of minimum type T from the given file name.
        /// </summary>
        protected internal T CreateSimulatableChildFromFile<T>(string fileName) where T : C
        {
            XiHelper.ArgumentNullCheck(fileName);
            T simulatableChild = Game.ReadRecyclable<T>(fileName);
            simulatableChild.RefreshOverlaidProperties();
            simulatableChild.OwnedBySimulatableParent = true;
            AddSimulatableChild(simulatableChild);
            return simulatableChild;
        }

        /// <summary>
        /// Create (and add) a simulatable child of minimum type T from the XML node.
        /// </summary>
        protected internal T CreateSimulatableChildFromDocument<T>(XmlNode node) where T : C
        {
            XiHelper.ArgumentNullCheck(node);
            T simulatableChild = Game.ReadRecyclable<T>(node);
            simulatableChild.RefreshOverlaidProperties();
            simulatableChild.OwnedBySimulatableParent = true;
            AddSimulatableChild(simulatableChild);
            return simulatableChild;
        }

        /// <inheritdoc />
        protected override Simulatable SimulatableParentHook
        {
            get { return SimulatableParent; }
            set { SimulatableParent = XiHelper.Cast<P>(value); }
        }

        /// <summary>
        /// Handle the fact that the simulatable parent has changed.
        /// </summary>
        protected virtual void OnSimulatableParentChanged(P oldSimulatableParent) { }

        ///<inheritdoc />
        protected override void Destroy(bool destroying)
        {
            if (destroying) TearDown();
            base.Destroy(destroying);
        }

        /// <inheritdoc />
        protected override void OnDeallocated()
        {
            base.OnDeallocated();
            TearDown();
        }

        /// <inheritdoc />
        protected override void OnRefreshOverlaidPropertiesFinishing()
        {
            base.OnRefreshOverlaidPropertiesFinishing();
            Game.RaiseSimulationSelectionChanged();
        }

        /// <inheritdoc />
        protected override void OnReadFinishing()
        {
            base.OnReadFinishing();
            CollectSimulatableChildren(cachedSimulatableChildren);
            foreach (C child in cachedSimulatableChildren) child.NotifySerializableParentReadFinishing();
            cachedSimulatableChildren.Clear();
        }

        /// <inheritdoc />
        protected override void ReadAdditional(XmlNode node)
        {
            base.ReadAdditional(node);
            ReadSimulatableChildren(node);
        }

        /// <inheritdoc />
        protected override void WriteAdditional(XmlDocument document, XmlNode node)
        {
            base.WriteAdditional(document, node);
            WriteSimulatableChildren(document, node);
        }

        /// <inheritdoc />
        protected override void UpdateHook(GameTime gameTime)
        {
            foreach (C child in simulatableChildren)
                if (child.Enabled)
                    child.Update(gameTime);
        }

        /// <inheritdoc />
        protected override void EditHook(GameTime gameTime)
        {
            foreach (C child in simulatableChildren)
                if (child.Enabled)
                    child.Edit(gameTime);
        }

        /// <inheritdoc />
        protected override void PlayHook(GameTime gameTime)
        {
            CollectSimulatableChildren(cachedSimulatableChildren);
            foreach (C child in cachedSimulatableChildren)
                if (child.Enabled)
                    child.Play(gameTime);
            cachedSimulatableChildren.Clear();
        }

        /// <inheritdoc />
        protected override void VisualizeHook(GameTime gameTime)
        {
            foreach (C child in simulatableChildren)
                if (child.Visible)
                    child.Visualize(gameTime);
        }

        /// <inheritdoc />
        protected override Simulatable GetSimulatableChildHook(string childName)
        {
            return simulatableChildren.Get<Actor>(childName);
        }

        /// <inheritdoc />
        protected override void RemoveSimulatableChildWithoutDisposingHook(Simulatable child)
        {
            RemoveSimulatableChildWithoutDisposing(child);
        }

        /// <inheritdoc />
        protected override void CollectSimulatableChildrenHook(List<Simulatable> result)
        {
            simulatableChildren.Collect(result);
        }

        /// <inheritdoc />
        protected override void OnSimulatableChildNameChanged(Simulatable child, string oldName)
        {
            base.OnSimulatableChildNameChanged(child, oldName);
            simulatableChildren.RefreshDictionary(XiHelper.Cast<C>(child), oldName);
        }

        private void TearDown()
        {
            if (Selected) Game.RaiseSimulationSelectionChanged();
            ClearSimulatableChildrenEventless();
            if (SimulatableParent != null) SimulatableParent.RemoveSimulatableChildWithoutDisposing(this);
        }

        private bool AddSimulatableChildEventless(C child)
        {
            Simulatable currentParent = child.SimulatableParent;
            if (currentParent == this) return false;
            if (currentParent != null) currentParent.RemoveSimulatableChildWithoutDisposing(child);
            try { child.SimulatableParent = this; }
            catch { child.Dispose(); throw; }
            simulatableChildren.Add(child);
            return true;
        }

        private bool RemoveSimulatableChildEventless(C child)
        {
            if (!simulatableChildren.Remove(child)) return false;
            child.SimulatableParent = null;
            if (child.OwnedBySimulatableParent) child.Dispose();
            return true;
        }

        private bool RemoveSimulatableChildrenEventless(List<C> children)
        {
            bool anySimulatablesRemoved = false;
            foreach (C child in children)
                if (RemoveSimulatableChildEventless(child))
                    anySimulatablesRemoved = true;
            return anySimulatablesRemoved;
        }

        private bool ClearSimulatableChildrenEventless()
        {
            CollectSimulatableChildren(cachedSimulatableChildren);
            bool result = RemoveSimulatableChildrenEventless(cachedSimulatableChildren);
            cachedSimulatableChildren.Clear();
            return result;
        }

        private void ReadSimulatableChildren(XmlNode node)
        {
            XmlNodeList childNodes = node.SelectNodes("Serializable");
            foreach (XmlNode childNode in childNodes) ReadSimulatableChild(childNode);
        }

        private void ReadSimulatableChild(XmlNode node)
        {
            XmlNode typeNode = node.SelectSingleNode("Type");
            Simulatable child = CreateSimulatableChild<C>(typeNode.InnerText);
            child.Read(node);
        }

        private void WriteSimulatableChildren(XmlDocument document, XmlNode node)
        {
            foreach (Simulatable child in CollectSimulatableChildren(new List<Simulatable>()))
                if (child.PersistentAsSimulatableChild)
                    WriteSimulatableChild(document, node, child);
        }

        private void WriteSimulatableChild(XmlDocument document, XmlNode node, Simulatable child)
        {
            XmlNode childNode = document.CreateElement("Serializable");
            child.Write(document, childNode);
            node.AppendChild(childNode);
        }

        private readonly QueriableSimulatables<C> simulatableChildren = new QueriableSimulatables<C>();
        private P simulatableParent;
        private readonly List<C> cachedSimulatableChildren = new List<C>();
    }
}
