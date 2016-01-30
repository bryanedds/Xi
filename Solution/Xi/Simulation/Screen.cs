using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Xi
{
    /// <summary>
    /// A screen that contains an interaction sequence for a game.
    /// TODO: build a ScreenTransition class.
    /// TODO: build a ScreenTransitionContext base class to describe user-definable screen
    /// transitions.
    /// </summary>
    public class Screen : Simulatable<Application, ActorGroup>
    {
        /// <summary>
        /// Create a Screen.
        /// </summary>
        /// <param name="game">The game.</param>
        public Screen(XiGame game) : base(game) { }

        /// <summary>
        /// The parent application.
        /// May be null.
        /// </summary>
        [Browsable(false), IgnoreSerialization]
        public Application Application
        {
            get { return SimulatableParent; }
            internal set { SimulatableParent = value; }
        }

        /// <summary>
        /// Get the application as type T.
        /// May return null.
        /// </summary>
        public T GetApplication<T>() where T : class
        {
            return GetSimulatableParent<T>();
        }

        /// <summary>
        /// Grab the application as type T.
        /// Will not return null.
        /// </summary>
        public T GrabApplication<T>() where T : class
        {
            return GrabSimulatableParent<T>();
        }

        /// <summary>
        /// Get the first actor group of type T with the given name.
        /// May return null.
        /// </summary>
        public T GetActorGroup<T>(string name) where T : class
        {
            XiHelper.ArgumentNullCheck(name);
            return GetSimulatableChild<T>(name);
        }

        /// <summary>
        /// Grab the first actor group of type T with the given name.
        /// Will not return null.
        /// </summary>
        public T GrabActorGroup<T>(string name) where T : class
        {
            XiHelper.ArgumentNullCheck(name);
            return GrabSimulatableChild<T>(name);
        }

        /// <summary>
        /// Collect all the actor groups of type T.
        /// </summary>
        public List<T> CollectActorGroups<T>(List<T> result) where T : class
        {
            XiHelper.ArgumentNullCheck(result);
            return CollectSimulatableChildren(result);
        }

        /// <summary>
        /// Collect all the actor groups of type T that satisfy the given predicate.
        /// </summary>
        public List<T> CollectActorGroups<T>(Func<T, bool> predicate, List<T> result) where T : class
        {
            XiHelper.ArgumentNullCheck(predicate, result);
            return CollectSimulatableChildren(predicate, result);
        }

        /// <summary>
        /// Add an actor group.
        /// </summary>
        public bool AddActorGroup(ActorGroup actor)
        {
            XiHelper.ArgumentNullCheck(actor);
            return AddSimulatableChild(actor);
        }

        /// <summary>
        /// Remove an actor group.
        /// Actor group will be disposed if OwnedBySimulatableParent is true.
        /// </summary>
        public bool RemoveActorGroup(ActorGroup actor)
        {
            XiHelper.ArgumentNullCheck(actor);
            return RemoveSimulatableChild(actor);
        }

        /// <summary>
        /// Remove the actor groups.
        /// Actor groups will be disposed if OwnedBySimulatableParent is true.
        /// </summary>
        public bool RemoveActorGroups(List<ActorGroup> actors)
        {
            XiHelper.ArgumentNullCheck(actors);
            return RemoveSimulatableChildren(actors);
        }

        /// <summary>
        /// Clear the actor groups.
        /// Actor groups will be disposed if OwnedBySimulatableParent is true.
        /// </summary>
        public void ClearActorGroups()
        {
            ClearSimulatableChildren();
        }

        /// <summary>
        /// Does contain the actor group of type T with the given name?
        /// </summary>
        public bool ContainsActorGroup(ActorGroup actor)
        {
            XiHelper.ArgumentNullCheck(actor);
            return ContainsSimulatableChild(actor);
        }

        /// <summary>
        /// Create (and add) an actor group of minimum type T that is of given type name.
        /// </summary>
        public T CreateActorGroup<T>(string typeName) where T : ActorGroup
        {
            XiHelper.ArgumentNullCheck(typeName);
            return CreateSimulatableChild<T>(typeName);
        }

        /// <summary>
        /// Create (and add) an actor group of minimum type T from the given object definition.
        /// </summary>
        public T CreateActorGroupFromDefinition<T>(string actorDefinition) where T : ActorGroup
        {
            XiHelper.ArgumentNullCheck(actorDefinition);
            return CreateSimulatableChildFromDefinition<T>(actorDefinition);
        }

        /// <summary>
        /// Create (and add) an actor group of minimum type T from the given file name.
        /// </summary>
        public T CreateActorGroupFromFile<T>(string fileName) where T : ActorGroup
        {
            XiHelper.ArgumentNullCheck(fileName);
            return CreateSimulatableChildFromFile<T>(fileName);
        }

        /// <summary>
        /// Create (and add) an actor group of minimum type T from the XML node.
        /// </summary>
        public T CreateActorGroupFromDocument<T>(XmlNode node) where T : ActorGroup
        {
            XiHelper.ArgumentNullCheck(node);
            return CreateSimulatableChildFromDocument<T>(node);
        }
    }
}
