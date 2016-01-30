using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// The game domain object type. Useful as a base class for avatars, enemies, obstacles, etc.
    /// </summary>
    public abstract class Actor : Simulatable<ActorGroup, Facet>
    {
        /// <summary>
        /// Create an Actor.
        /// </summary>
        /// <param name="game">The game.</param>
        public Actor(XiGame game) : base(game) { }

        /// <summary>
        /// The parent actor group.
        /// May be null.
        /// </summary>
        [Browsable(false), IgnoreSerialization]
        public ActorGroup ActorGroup
        {
            get { return SimulatableParent; }
            internal set { SimulatableParent = value; }
        }

        /// <summary>
        /// Get the actor group as type T.
        /// May return null.
        /// </summary>
        public T GetActorGroup<T>() where T : class
        {
            return GetSimulatableParent<T>();
        }

        /// <summary>
        /// Grab the actor group as type T.
        /// Will not return null.
        /// </summary>
        public T GrabActorGroup<T>() where T : class
        {
            return GrabSimulatableParent<T>();
        }

        /// <summary>
        /// Get the first facet of type T where 'Name == typeof(T).FullName'.
        /// May return null.
        /// </summary>
        public T GetFacet<T>() where T : class
        {
            return GetSimulatableChild<T>();
        }

        /// <summary>
        /// Get the first facet of type T with the given name.
        /// May return null.
        /// </summary>
        public T GetFacet<T>(string name) where T : class
        {
            XiHelper.ArgumentNullCheck(name);
            return GetSimulatableChild<T>(name);
        }

        /// <summary>
        /// Grab the first facet of type T where 'Name == typeof(T).FullName'.
        /// Will not return null.
        /// </summary>
        public T GrabFacet<T>() where T : class
        {
            return GrabSimulatableChild<T>();
        }

        /// <summary>
        /// Grab the first facet of type T with the given name.
        /// Will not return null.
        /// </summary>
        public T GrabFacet<T>(string name) where T : class
        {
            XiHelper.ArgumentNullCheck(name);
            return GrabSimulatableChild<T>(name);
        }

        /// <summary>
        /// Add a facet.
        /// </summary>
        public bool AddFacet(Facet facet)
        {
            XiHelper.ArgumentNullCheck(facet);
            return AddSimulatableChild(facet);
        }

        /// <summary>
        /// Remove a facet.
        /// Facet will be disposed if OwnedBySimulatableParent is true.
        /// </summary>
        public bool RemoveFacet(Facet facet)
        {
            XiHelper.ArgumentNullCheck(facet);
            return RemoveSimulatableChild(facet);
        }

        /// <summary>
        /// Remove the facets.
        /// Facet will be disposed if OwnedBySimulatableParent is true.
        /// </summary>
        public bool RemoveFacets(List<Facet> facets)
        {
            XiHelper.ArgumentNullCheck(facets);
            return RemoveSimulatableChildren(facets);
        }

        /// <summary>
        /// Does contain the facet of type T where 'Name == typeof(T).FullName'?
        /// </summary>
        public bool ContainsFacet<T>() where T : class
        {
            return ContainsSimulatableChild<T>();
        }

        /// <summary>
        /// Does contain the facet of type T with the given name?
        /// </summary>
        public bool ContainsFacet<T>(string name) where T : class
        {
            XiHelper.ArgumentNullCheck(name);
            return ContainsSimulatableChild<T>(name);
        }

        /// <summary>
        /// Create (and add) a facet of minimum type T that is of given type name.
        /// </summary>
        public T CreateFacet<T>(string typeName) where T : Facet
        {
            XiHelper.ArgumentNullCheck(typeName);
            return CreateSimulatableChild<T>(typeName);
        }

        /// <summary>
        /// Create (and add) a facet of minimum type T from the given object definition.
        /// </summary>
        public T CreateFacetFromDefinition<T>(string definition) where T : Facet
        {
            XiHelper.ArgumentNullCheck(definition);
            return CreateSimulatableChildFromDefinition<T>(definition);
        }

        /// <summary>
        /// Create (and add) a facet of minimum type T from the given file name.
        /// </summary>
        public T CreateFacetFromFile<T>(string fileName) where T : Facet
        {
            XiHelper.ArgumentNullCheck(fileName);
            return CreateSimulatableChildFromFile<T>(fileName);
        }

        /// <summary>
        /// Create (and add) a facet of minimum type T from the XML node.
        /// </summary>
        public T CreateFacetFromDocument<T>(XmlNode node) where T : Facet
        {
            XiHelper.ArgumentNullCheck(node);
            return CreateSimulatableChildFromDocument<T>(node);
        }

        /// <inheritdoc />
        protected override void InputHook(GameTime gameTime)
        {
            base.InputHook(gameTime);
            InputFacets(gameTime);
        }

        private void InputFacets(GameTime gameTime)
        {
            foreach (Facet facet in SimulatableChildren)
                facet.Input(gameTime, FocusIndex.Value);
        }
    }
}
