using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using BEPUphysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Xi
{
    /// <summary>
    /// Responsible for processing Xi engine updates, drawing, exposing subsystems across the
    /// application, and other high-level tasks.
    /// </summary>
    /// <remarks>
    /// This being the game engine object, it's name of 'XiGame' is a misnomer inherited from XNA's
    /// Game class. You would never put high-level game state or game logic in here - that is what
    /// the Application object is for.
    /// </remarks>
    public class XiGame : Game
    {
        /// <summary>
        /// Create a XiGame.
        /// </summary>
        /// <param name="sourcePath">The source path from which documents are loaded.</param>
        /// <param name="userAssemblies">The assemblies to search for user-defined types.</param>
        public XiGame(string sourcePath, params Assembly[] userAssemblies)
        {
            // set up members
            this.sourcePath = sourcePath;

            // set up the assemblies
            assemblies.Add(Assembly.GetExecutingAssembly());
            assemblies.AddRange(userAssemblies);

            // set up the content root directory
            Content.RootDirectory = Constants.ContentPath;

            // create the graphics device manager
            graphics = new GraphicsDeviceManager(this);
            graphics.MinimumPixelShaderProfile = ShaderProfile.PS_1_1;
            graphics.MinimumVertexShaderProfile = ShaderProfile.VS_1_1;
            graphics.PreparingDeviceSettings += (s, e) =>
            {
                e.GraphicsDeviceInformation.DeviceType = Constants.DeviceTypeSetting;
                e.GraphicsDeviceInformation.PresentationParameters.MultiSampleType = Constants.MultiSampleTypeSetting;
                e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = Constants.RenderTargetUsageSetting;
            };

            // set up the rendering (part 1)
            scene3D = new Scene(this);
            surfaceDrawer3D = new SurfaceDrawer(this);

            // set up BEPU
            sceneSpace = new Space();
            sceneSpace.SimulationSettings.MotionUpdate.Gravity = new Vector3(0, -20, 0);
            if (Environment.ProcessorCount > 1)
            {
                for (int i = 0; i < Environment.ProcessorCount; i++) sceneSpace.ThreadManager.AddThread();
                sceneSpace.UseMultithreadedUpdate = true;
            }
            sceneSpace.SimulationSettings.CollisionResponse.Iterations = 8;
        }

        /// <summary>
        /// Raised when the game's physics are enabled or disabled.
        /// </summary>
        public event Action PhysicsEnabledChanged;

        /// <summary>
        /// Raised when the game's editing state is changed.
        /// </summary>
        public event Action EditingChanged;

        /// <summary>
        /// Raised when a simulation structure is changed.
        /// Intended only for editor use.
        /// </summary>
        public event Action SimulationStructureChanged;

        /// <summary>
        /// Raised when the simulation selection is changed.
        /// Intended only for editor use.
        /// </summary>
        public event Action SimulationSelectionChanged;

        /// <summary>
        /// The XML document cache.
        /// </summary>
        public XmlDocumentCache XmlDocumentCache { get { return xmlDocumentCache; } }

        /// <summary>
        /// The overlayer.
        /// </summary>
        public Overlayer Overlayer { get { return overlayer; } }

        /// <summary>
        /// The object recycler.
        /// </summary>
        public Recycler Recycler { get { return recycler; } }

        /// <summary>
        /// The resolution manager.
        /// </summary>
        public ResolutionManager ResolutionManager { get { return resolutionManager; } }

        /// <summary>
        /// The camera.
        /// </summary>
        public Camera Camera { get { return camera; } }

        /// <summary>
        /// The 3D physics scene space.
        /// </summary>
        public Space SceneSpace { get { return sceneSpace; } }

        /// <summary>
        /// The 2D physics world.
        /// </summary>
        public World World { get { return world; } }

        /// <summary>
        /// The 3D scene.
        /// </summary>
        public Scene Scene { get { return scene3D; } }

        /// <summary>
        /// The 3D surface drawer.
        /// </summary>
        public SurfaceDrawer SurfaceDrawer3D { get { return surfaceDrawer3D; } }

        /// <summary>
        /// The 2D sprite batch.
        /// </summary>
        public SpriteBatch SpriteBatch2D { get { return spriteBatch2D; } }

        /// <summary>
        /// The UI sprite batch.
        /// </summary>
        public SpriteBatch SpriteBatchUI { get { return spriteBatchUI; } }

        /// <summary>
        /// The keyboard state.
        /// </summary>
        public KeyboardState KeyboardState { get { return inputCache.KeyboardState; } }

        /// <summary>
        /// Are the physics boxes being drawn?
        /// </summary>
        public bool PhysicsBoxDrawerVisible
        {
            get { return physicsBoxDrawer.Visible; }
            set { physicsBoxDrawer.Visible = value; }
        }

        /// <summary>
        /// Are the physics enabled?
        /// </summary>
        public bool PhysicsEnabled
        {
            get { return physicsEnabled; }
            set
            {
                if (physicsEnabled == value) return;
                physicsEnabled = value;
                PhysicsEnabledChanged.TryRaise();
            }
        }

        /// <summary>
        /// Is the game in play mode (not editing)?
        /// </summary>
        public bool Playing
        {
            get { return !Editing; }
            set { Editing = !value; }
        }

        /// <summary>
        /// Is the game in editing mode (not playing)?
        /// </summary>
        public bool Editing
        {
            get { return _editing; }
            set
            {
                if (_editing == value) return;
                _editing = value;
                EditingChanged.TryRaise();
            }
        }

        /// <summary>
        /// Refresh a document file from its source.
        /// </summary>
        public void RefreshDocumentFile(string documentFileName)
        {
            string sourceFileName = Path.Combine(sourcePath, documentFileName);
            File.Copy(sourceFileName, documentFileName, true);
        }

        /// <summary>
        /// Create a recyclable object.
        /// </summary>
        /// <typeparam name="T">The minimum type of the object.</typeparam>
        /// <param name="instanceTypeName">The type name of the instance to create.</param>
        public T CreateRecyclable<T>(string instanceTypeName) where T : Recyclable
        {
            XiHelper.ArgumentNullCheck(instanceTypeName);
            return CreateSerializable<T>(instanceTypeName);
        }

        /// <summary>
        /// Create a dynamically-configured recyclable object.
        /// </summary>
        /// <typeparam name="T">The minimum type of the object.</typeparam>
        /// <param name="definition">The definition used to create and configure the instance.</param>
        public T CreateRecyclableFromDefinition<T>(string definition) where T : Recyclable
        {
            XiHelper.ArgumentNullCheck(definition);
            return recycler.Allocate<T>(definition);
        }

        /// <summary>
        /// Create a dynamically-configured recyclable object from a file definition.
        /// </summary>
        /// <typeparam name="T">The minimum type of the object.</typeparam>
        /// <param name="fileDefinition">The file definition used to create and configure the instance.</param>
        public T CreateRecyclableFromFileDefinition<T>(string fileDefinition) where T : Recyclable
        {
            XiHelper.ArgumentNullCheck(fileDefinition);
            return CreateSerializableFromFileDefinition<T>(fileDefinition);
        }

        /// <summary>
        /// Create a dynamically-configured recyclable object from a string definition.
        /// </summary>
        /// <typeparam name="T">The minimum type of the object.</typeparam>
        /// <param name="stringDefinition">The string definition used to create and configure the instance.</param>
        public T CreateRecyclableFromStringDefinition<T>(string stringDefinition) where T : Recyclable
        {
            XiHelper.ArgumentNullCheck(stringDefinition);
            return CreateSerializableFromStringDefinition<T>(stringDefinition);
        }

        /// <summary>
        /// Create a serializable object.
        /// </summary>
        /// <typeparam name="T">The minimum type of the object.</typeparam>
        /// <param name="instanceTypeName">The type name of the instance to create.</param>
        public T CreateSerializable<T>(string instanceTypeName) where T : Serializable
        {
            ValidateInstanceTypeName(instanceTypeName);
            Type instanceType = GetType(instanceTypeName);
            ValidateInstanceType(instanceTypeName, instanceType);
            ConstructorInfo xiConstructor = instanceType.GetConstructor(xiConstructorParameterTypes);
            ValidateConstructor(instanceTypeName, xiConstructor);
            object instance = CreateXiObject(xiConstructor);
            return ReflectionHelper.CastInstanceToTAndDisposeOnFailure<T>(instanceTypeName, instance);
        }

        /// <summary>
        /// Create a dynamically-configured serializable object.
        /// </summary>
        /// <typeparam name="T">The minimum type of the object.</typeparam>
        /// <param name="definition">The definition used to create and configure the instance.</param>
        public T CreateSerializableFromDefinition<T>(string definition) where T : Serializable
        {
            // TODO: add XML definition case by pattern matching on '<...'
            return
                ReflectionHelper.IsFileDefinition(definition) ?
                CreateSerializableFromFileDefinition<T>(definition) :
                CreateSerializableFromStringDefinition<T>(definition);
        }

        /// <summary>
        /// Create a dynamically-configured serializable object from a file definition.
        /// </summary>
        /// <typeparam name="T">The minimum type of the object.</typeparam>
        /// <param name="fileDefinition">The file definition used to create and configure the instance.</param>
        public T CreateSerializableFromFileDefinition<T>(string fileDefinition) where T : Serializable
        {
            string[] fileDefinitionArray = ReflectionHelper.ToFileDefinitionArray(fileDefinition);
            ReflectionHelper.ValidateFileDefinitionArray(fileDefinition, fileDefinitionArray);
            return ReadSerializable<T>(fileDefinitionArray[1]);
        }

        /// <summary>
        /// Create a dynamically-configured serializable object from a string definition.
        /// </summary>
        /// <typeparam name="T">The minimum type of the object.</typeparam>
        /// <param name="stringDefinition">The definition used to create and configure the instance.</param>
        public T CreateSerializableFromStringDefinition<T>(string stringDefinition) where T : Serializable
        {
            string[] stringDefinitionArray = ReflectionHelper.ToStringDefinitionArray(stringDefinition);
            ReflectionHelper.ValidateStringDefinitionArray(stringDefinition, stringDefinitionArray);
            string instanceTypeName = stringDefinitionArray[0];
            T instance = CreateSerializable<T>(instanceTypeName);
            ReflectionHelper.ConfigureCreatedObject(stringDefinition, stringDefinitionArray, instance);
            return instance;
        }

        /// <summary>
        /// Create a dynamically-configured, default-constructible object.
        /// If supported, disposes object on failure.
        /// </summary>
        /// <typeparam name="T">The minimum type of the object.</typeparam>
        /// <param name="definition">The definition used to create and configure the instance.</param>
        public T CreateObjectFromDefinition<T>(string definition) where T : class
        {
            XiHelper.ArgumentNullCheck(definition);
            string[] definitionArray = ReflectionHelper.ToStringDefinitionArray(definition);
            ReflectionHelper.ValidateStringDefinitionArray(definition, definitionArray);
            string instanceTypeName = definitionArray[0];
            T instance = CreateObject<T>(instanceTypeName);
            ReflectionHelper.ConfigureCreatedObject(definition, definitionArray, instance);
            return instance;
        }

        /// <summary>
        /// Create a default-constructible object using its default constructor.
        /// If supported, disposes object on failure.
        /// </summary>
        /// <typeparam name="T">The minimum type of the object.</typeparam>
        /// <param name="instanceTypeName">The type name of the instance to create.</param>
        public T CreateObject<T>(string instanceTypeName) where T : class
        {
            XiHelper.ArgumentNullCheck(instanceTypeName);
            ValidateInstanceTypeName(instanceTypeName);
            Type instanceType = GetType(instanceTypeName);
            ValidateInstanceType(instanceTypeName, instanceType);
            ConstructorInfo constructor = instanceType.GetConstructor(ReflectionHelper.EmptyTypes);
            ValidateConstructor(instanceTypeName, constructor);
            object instance = ReflectionHelper.CreateObject(constructor);
            return ReflectionHelper.CastInstanceToTAndDisposeOnFailure<T>(instanceTypeName, instance);
        }

        /// <summary>
        /// Read in a recyclable object.
        /// </summary>
        /// <typeparam name="T">The minimum type of the object.</typeparam>
        /// <param name="fileName">The name of the file to read from.</param>
        public T ReadRecyclable<T>(string fileName) where T : Recyclable
        {
            XiHelper.ArgumentNullCheck(fileName);
            XiHelper.ValidateFileName(fileName);
            XmlDocument document = XmlDocumentCache.GetXmlDocument(fileName);
            XmlNode rootNode = document.SelectSingleNode("Root");
            XmlNode instanceNode = rootNode.SelectSingleNode("Serializable");
            return ReadRecyclable<T>(instanceNode);
        }

        /// <summary>
        /// Read in a recyclable object.
        /// </summary>
        /// <typeparam name="T">The minimum type of the object.</typeparam>
        /// <param name="node">The node where the recyclable's data resides.</param>
        public T ReadRecyclable<T>(XmlNode node) where T : Recyclable
        {
            XmlNode typeNode = node.SelectSingleNode("Type");
            T instance = CreateRecyclable<T>(typeNode.InnerText);
            try
            {
                instance.Read(node);
                return instance;
            }
            catch
            {
                instance.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Read in a serializable object.
        /// </summary>
        /// <typeparam name="T">The minimum type of the object.</typeparam>
        /// <param name="fileName">The name of the file to read from.</param>
        public T ReadSerializable<T>(string fileName) where T : Serializable
        {
            XiHelper.ArgumentNullCheck(fileName);
            XiHelper.ValidateFileName(fileName);
            XmlDocument document = XmlDocumentCache.GetXmlDocument(fileName);
            XmlNode rootNode = document.SelectSingleNode("Root");
            XmlNode instanceNode = rootNode.SelectSingleNode("Serializable");
            XmlNode typeNode = instanceNode.SelectSingleNode("Type");
            using (T instance = CreateSerializable<T>(typeNode.InnerText))
            {
                instance.Read(instanceNode);
                return instance;
            }
        }

        /// <summary>
        /// Read in a serializable object.
        /// </summary>
        /// <typeparam name="T">The minimum type of the object.</typeparam>
        /// <param name="node">The node where the recyclable's data resides.</param>
        public T ReadSerializable<T>(XmlNode node) where T : Serializable
        {
            XmlNode typeNode = node.SelectSingleNode("Type");
            using (T instance = CreateSerializable<T>(typeNode.InnerText))
            {
                instance.Read(node);
                return instance;
            }
        }

        /// <summary>
        /// Get a type with the matching full name from one of the Xi-registered assemblies.
        /// </summary>
        public Type GetType(string name)
        {
            foreach (Assembly assembly in assemblies)
            {
                Type instanceType = assembly.GetType(name, false);
                if (instanceType != null) return instanceType;
            }
            return null;
        }

        /// <summary>
        /// Get all of the types in the Xi-registered assemblies.
        /// Creates garbage, so avoid using at play-time.
        /// </summary>
        public IEnumerable<Type> GetTypes()
        {
            foreach (Assembly assembly in assemblies)
                foreach (Type type in assembly.GetTypes())
                    yield return type;
        }

        /// <summary>
        /// Raise the SimulationStructureChanged event.
        /// </summary>
        public void RaiseSimulationStructureChanged()
        {
            SimulationStructureChanged.TryRaise();
        }

        /// <summary>
        /// Raise the SimulationSelectionChanged event.
        /// </summary>
        public void RaiseSimulationSelectionChanged()
        {
            SimulationSelectionChanged.TryRaise();
        }

        /// <summary>
        /// Get the game pad state for the given player index.
        /// </summary>
        public GamePadState GetGamePadState(PlayerIndex playerIndex)
        {
            return inputCache.GetGamePadState(playerIndex);
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            // set up the rendering (part 2)
            spriteBatch2D = new SpriteBatch(GraphicsDevice);
            spriteBatchUI = new SpriteBatch(GraphicsDevice);
            depthStencilBuffer = new ManagedDepthStencilBuffer(this, Constants.DepthStencilBufferSurfaceToScreenRatio, DepthFormat.Depth24Stencil8, MultiSampleType.None, 0);
            backBuffer = new ManagedBackBuffer(this, 0);

            // set up the subsystems
            xmlDocumentCache = new XmlDocumentCache(this);
            overlayer = new Overlayer(this, "Documents/Overlay.xiol");
            recycler = new Recycler(this);
            resolutionManager = new ResolutionManager(graphics);
            camera = new FovCamera(GraphicsDevice);
            focusManager = new FocusManager(this);

            // set up the physics box drawer
            physicsBoxDrawer = new PhysicsBoxDrawer(this) { Visible = false };

            base.Initialize();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // tear down the subsystems
                if (recycler != null) recycler.Dispose();

                // tear down the rendering system
                if (backBuffer != null) backBuffer.Dispose();
                if (depthStencilBuffer != null) depthStencilBuffer.Dispose();
                if (spriteBatchUI != null) spriteBatchUI.Dispose();
                if (spriteBatch2D != null) spriteBatch2D.Dispose();
                if (scene3D != null) scene3D.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override void Update(GameTime gameTime)
        {
            // PropertyGrid's internal ShowDialog call (for invalid property values) can cause
            // unintentional reentry into XNA's Update call, so we guard from its effects.
            if (updating) return;
            updating = true;

            // update the subsystems
            resolutionManager.Update(gameTime);
            inputCache.Update(gameTime);

            // process input
            if (!Editing) Input(gameTime);

            // advance the game's logic
            Advance(gameTime);

            // process physics
            if (physicsEnabled) Physics(gameTime);

            // forward to base method
            base.Update(gameTime);

            // clear updating flag
            updating = false; 
        }

        /// <inheritdoc />
        protected override void Draw(GameTime gameTime)
        {
            // begin sprite batch drawing
            Matrix spriteBatch2DTransform = Matrix.CreateTranslation(-camera.Position.X, camera.Position.Y, 0); // BUG: algorithm duplicated elsewhere
            spriteBatch2D.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState, spriteBatch2DTransform);
            spriteBatchUI.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState);

            // visualize the game's object
            Visualize(gameTime);

            // activate the depth stencil buffer
            depthStencilBuffer.Activate();

            // pre-draw the 3D scene
            scene3D.PreDraw(gameTime, Camera);

            // activate the and clear the back buffer
            backBuffer.Activate();
            GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.Black, 1.0f, 0);

            // draw the 3D scene
            scene3D.Draw(gameTime, Camera, "Normal");

            // draw physics boxes
            if (physicsBoxDrawer.Visible) physicsBoxDrawer.Draw(camera.View, camera.Projection);

            // end sprite batch drawing
            spriteBatch2D.End();
            spriteBatchUI.End();

            // resolve the back buffer
            backBuffer.Resolve();

            // forward to base method
            base.Draw(gameTime);
        }

        /// <summary>
        /// Advance the game one tick.
        /// </summary>
        protected virtual void AdvanceHook(GameTime gameTime) { }

        /// <summary>
        /// Visualize the game objects.
        /// </summary>
        protected virtual void VisualizeHook(GameTime gameTime) { }

        /// <summary>
        /// The focus manager.
        /// </summary>
        internal FocusManager FocusManager { get { return focusManager; } }

        private object CreateXiObject(ConstructorInfo xiConstructor)
        {
            XiHelper.ArgumentNullCheck(xiConstructor);
            try
            {
                xiConstructorParameters[0] = this;
                return xiConstructor.Invoke(xiConstructorParameters);
            }
            catch (Exception e)
            {
                Trace.Fail("Xi object creation error.", e.ToString());
                throw;
            }
            finally
            {
                xiConstructorParameters[0] = null; // get rid of unneeded references
            }
        }

        private void Input(GameTime gameTime)
        {
            focusManager.Input(gameTime);
        }

        private void Advance(GameTime gameTime)
        {
            AdvanceHook(gameTime);
        }

        private void Physics(GameTime gameTime)
        {
            sceneSpace.Update(gameTime);
            world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
        }

        private void Visualize(GameTime gameTime)
        {
            VisualizeHook(gameTime);
            camera.Update(gameTime);
        }

        private static void ValidateConstructor(string instanceTypeName, ConstructorInfo constructor)
        {
            XiHelper.ArgumentNullCheck(instanceTypeName);
            if (constructor == null)
                throw new ArgumentException("Type '" + instanceTypeName + "' does not have a compatible constructor.");
        }

        private static void ValidateInstanceTypeName(string instanceTypeName)
        {
            XiHelper.ArgumentNullCheck(instanceTypeName);
            if (instanceTypeName.Length == 0)
                throw new ArgumentException("Instance type name cannot have 0 length.");
        }

        private static void ValidateInstanceType(string instanceTypeName, Type instanceType)
        {
            if (instanceType == null)
                throw new ArgumentException("Invalid type name '" + instanceTypeName + "'.");
        }

        // Xi-compatible constructor objects
        private static readonly Type[] xiConstructorParameterTypes = { typeof(XiGame) };
        private readonly object[] xiConstructorParameters = new object[1];

        // default XNA members
        private readonly GraphicsDeviceManager graphics;

        // rendering
        private readonly Scene scene3D;
        private readonly SurfaceDrawer surfaceDrawer3D;
        private SpriteBatch spriteBatch2D;
        private SpriteBatch spriteBatchUI;
        private ManagedDepthStencilBuffer depthStencilBuffer;
        private ManagedBackBuffer backBuffer;

        // engine subsystems
        private readonly InputCache inputCache = new InputCache();
        private readonly Space sceneSpace;
        private readonly World world = new World(new Vector2(0, 20));
        private XmlDocumentCache xmlDocumentCache;
        private Overlayer overlayer;
        private Recycler recycler;
        private ResolutionManager resolutionManager;
        private Camera camera;
        private FocusManager focusManager;
        private PhysicsBoxDrawer physicsBoxDrawer;

        // engine fields
        private readonly List<Assembly> assemblies = new List<Assembly>();
        private readonly string sourcePath;
        private bool physicsEnabled = true;
        private bool updating;
        private bool _editing;
    }
}
