using System.ComponentModel;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// The application object. Unlike the XiGame object, this object can contain game state that may
    /// be serialized.
    /// </summary>
    public class Application : Simulatable<Simulatable, Screen>
    {
        /// <summary>
        /// Create an Application.
        /// </summary>
        /// <param name="game">The game.</param>
        public Application(XiGame game) : base(game) { }

        /// <summary>
        /// The current screen.
        /// Ignores serialization as that's already taken care of by Simulatable.
        /// </summary>
        [Browsable(false), IgnoreSerialization]
        public Screen Screen
        {
            get { return FirstSimulatableChildOrDefault; }
            set
            {
                Screen screen = Screen;
                if (screen == value) return;
                ClearSimulatableChildren();
                AddSimulatableChild(value);
            }
        }

        /// <summary>
        /// Advance the application one tick.
        /// </summary>
        public void Advance(GameTime gameTime)
        {
            Update(gameTime);
            if (Game.Editing) Edit(gameTime);
            else Play(gameTime);
        }

        /// <summary>
        /// Get the screen as type T.
        /// May return null.
        /// </summary>
        public T GetScreen<T>() where T : class
        {
            return SimulatableChildren.First as T;
        }

        /// <summary>
        /// Grab the screen as type T.
        /// Will not return null.
        /// </summary>
        public T GrabScreen<T>() where T : class
        {
            return XiHelper.Cast<T>(SimulatableChildren.First);
        }

        /// <summary>
        /// Create (and set) a screen of minimum type T that is of given type name.
        /// </summary>
        public T CreateScreen<T>(string typeName) where T : Screen
        {
            XiHelper.ArgumentNullCheck(typeName);
            ClearSimulatableChildren();
            return CreateSimulatableChild<T>(typeName);
        }

        /// <summary>
        /// Create (and set) a screen of minimum type T from the given object definition.
        /// </summary>
        public T CreateScreenFromDefinition<T>(string screenDefinition) where T : Screen
        {
            XiHelper.ArgumentNullCheck(screenDefinition);
            ClearSimulatableChildren();
            return CreateSimulatableChildFromDefinition<T>(screenDefinition);
        }

        /// <summary>
        /// Create (and set) a screen of minimum type T from the given file name.
        /// </summary>
        public T CreateScreenFromFile<T>(string fileName) where T : Screen
        {
            XiHelper.ArgumentNullCheck(fileName);
            ClearSimulatableChildren();
            return CreateSimulatableChildFromFile<T>(fileName);
        }

        /// <summary>
        /// Create (and set) a screen of minimum type T from the XML node.
        /// </summary>
        public T CreateScreenFromDocument<T>(XmlNode node) where T : Screen
        {
            XiHelper.ArgumentNullCheck(node);
            return CreateSimulatableChildFromDocument<T>(node);
        }
    }
}
