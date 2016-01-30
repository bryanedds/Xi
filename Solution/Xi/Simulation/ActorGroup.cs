using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Xi
{
    /// <summary>
    /// A logical group of actors.
    /// For example, one actor group could be the UI and another would be the play area.
    /// </summary>
    public class ActorGroup : Simulatable<Screen, Actor>
    {
        /// <summary>
        /// Create an ActorGroup.
        /// </summary>
        /// <param name="game">The game.</param>
        public ActorGroup(XiGame game) : base(game) { }

        /// <summary>
        /// The parent screen.
        /// May be null.
        /// </summary>
        [Browsable(false), IgnoreSerialization]
        public Screen Screen
        {
            get { return SimulatableParent; }
            internal set { SimulatableParent = value; }
        }

        /// <summary>
        /// Get the screen as type T.
        /// May return null.
        /// </summary>
        public T GetScreen<T>() where T : class
        {
            return GetSimulatableParent<T>();
        }

        /// <summary>
        /// Grab the screen as type T.
        /// Will not return null.
        /// </summary>
        public T GrabScreen<T>() where T : class
        {
            return GrabSimulatableParent<T>();
        }

        /// <summary>
        /// Get the first actor of type T with the given name.
        /// May return null.
        /// </summary>
        public T GetActor<T>(string name) where T : class
        {
            XiHelper.ArgumentNullCheck(name);
            return GetSimulatableChild<T>(name);
        }

        /// <summary>
        /// Grab the first actor of type T with the given name.
        /// Will not return null.
        /// </summary>
        public T GrabActor<T>(string name) where T : class
        {
            XiHelper.ArgumentNullCheck(name);
            return GrabSimulatableChild<T>(name);
        }

        /// <summary>
        /// Collect all the actors of type T.
        /// </summary>
        public List<T> CollectActors<T>(List<T> result) where T : class
        {
            XiHelper.ArgumentNullCheck(result);
            return CollectSimulatableChildren(result);
        }

        /// <summary>
        /// Collect all the actors of type T that satisfy the given predicate.
        /// </summary>
        public List<T> CollectActors<T>(Func<T, bool> predicate, List<T> result) where T : class
        {
            XiHelper.ArgumentNullCheck(predicate, result);
            return CollectSimulatableChildren(predicate, result);
        }

        /// <summary>
        /// Add an actor.
        /// </summary>
        public bool AddActor(Actor actor)
        {
            XiHelper.ArgumentNullCheck(actor);
            return AddSimulatableChild(actor);
        }

        /// <summary>
        /// Remove an actor.
        /// Actor will be disposed if OwnedBySimulatableParent is true.
        /// </summary>
        public bool RemoveActor(Actor actor)
        {
            XiHelper.ArgumentNullCheck(actor);
            return RemoveSimulatableChild(actor);
        }

        /// <summary>
        /// Remove the actors.
        /// Actors will be disposed if OwnedBySimulatableParent is true.
        /// </summary>
        public bool RemoveActors(List<Actor> actors)
        {
            XiHelper.ArgumentNullCheck(actors);
            return RemoveSimulatableChildren(actors);
        }

        /// <summary>
        /// Clear the actors.
        /// Actors will be disposed if OwnedBySimulatableParent is true.
        /// </summary>
        public void ClearActors()
        {
            ClearSimulatableChildren();
        }

        /// <summary>
        /// Change the actor group of the actors.
        /// </summary>
        public void ReparentActors(ActorGroup actorGroup)
        {
            ReparentSimulatableChildren(actorGroup);
        }

        /// <summary>
        /// Does contain the actor of type T with the given name?
        /// </summary>
        public bool ContainsActor(Actor actor)
        {
            XiHelper.ArgumentNullCheck(actor);
            return ContainsSimulatableChild(actor);
        }

        /// <summary>
        /// Create (and add) an actor of minimum type T that is of given type name.
        /// </summary>
        public T CreateActor<T>(string typeName) where T : Actor
        {
            XiHelper.ArgumentNullCheck(typeName);
            return CreateSimulatableChild<T>(typeName);
        }

        /// <summary>
        /// Create (and add) an actor of minimum type T from the given object definition.
        /// </summary>
        public T CreateActorFromDefinition<T>(string actorDefinition) where T : Actor
        {
            XiHelper.ArgumentNullCheck(actorDefinition);
            return CreateSimulatableChildFromDefinition<T>(actorDefinition);
        }

        /// <summary>
        /// Create (and add) an actor of minimum type T from the given file name.
        /// </summary>
        public T CreateActorFromFile<T>(string fileName) where T : Actor
        {
            XiHelper.ArgumentNullCheck(fileName);
            return CreateSimulatableChildFromFile<T>(fileName);
        }

        /// <summary>
        /// Create (and add) an actor of minimum type T from the XML node.
        /// </summary>
        public T CreateActorFromDocument<T>(XmlNode node) where T : Actor
        {
            XiHelper.ArgumentNullCheck(node);
            return CreateSimulatableChildFromDocument<T>(node);
        }
    }
}
