using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Represents a style in which to draw a surface.
    /// </summary>
    public enum DrawStyle
    {
        Opaque = 0,
        Transparent,
        Prioritized
    }

    /// <summary>
    /// Represents a combination of drawing properties for a surface.
    /// </summary>
    [Flags]
    public enum DrawProperties
    {
        None = 0,
        Shadowing = 1,
        Reflecting = 1 << 1,
        DependantTransform = 1 << 2
    }

    /// <summary>
    /// A drawable surface that is dependent on its parent actor's transformation.
    /// </summary>
    public abstract class Surface : Disposable
    {
        /// <summary>
        /// Initialize a Surface3D object.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="actor">The parent actor.</param>
        /// <param name="effectFileName">The effect file name.</param>
        public Surface(XiGame game, Actor3D actor, string effectFileName)
        {
            XiHelper.ArgumentNullCheck(game);
            this.game = game;
            this.actor = actor;
            EffectFileName = effectFileName;
        }

        /// <summary>
        /// The drawing properties.
        /// </summary>
        public DrawProperties DrawProperties
        {
            get { return drawProperties; }
            set { drawProperties = value; }
        }

        /// <summary>
        /// The bounding box.
        /// </summary>
        public BoundingBox BoundingBox { get { return BoundingBoxHook; } }

        /// <summary>
        /// The drawing style.
        /// </summary>
        public DrawStyle DrawStyle
        {
            get { return drawStyle; }
            set { drawStyle = value; }
        }

        /// <summary>
        /// How to draw the faces of the geometry.
        /// </summary>
        public FaceMode FaceMode
        {
            get { return faceMode; }
            set { faceMode = value; }
        }

        /// <summary>
        /// The effect used to draw.
        /// </summary>
        public Effect Effect { get { return EffectHook; } }

        /// <summary>
        /// The diffuse color.
        /// </summary>
        public Color DiffuseColor
        {
            get { return diffuseColor; }
            set { diffuseColor = value; }
        }

        /// <summary>
        /// The specular color.
        /// </summary>
        public Color SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; }
        }

        /// <summary>
        /// The name of the effect file.
        /// </summary>
        public string EffectFileName
        {
            get { return EffectFileNameHook; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (value.Length != 0) EffectFileNameHook = value;
            }
        }

        /// <summary>
        /// The specular power.
        /// </summary>
        public float SpecularPower
        {
            get { return specularPower; }
            set { specularPower = value; }
        }

        /// <summary>
        /// Handle getting if the surface is boundless.
        /// </summary>
        public bool Boundless { get { return BoundlessHook; } }

        /// <summary>
        /// Is the surface affected by light?
        /// </summary>
        public bool LightingEnabled
        {
            get { return lightingEnabled; }
            set { lightingEnabled = value; }
        }

        /// <summary>
        /// Should the transparent pixels be drawn, thereby affecting the depth map?
        /// </summary>
        public bool DrawTransparentPixels
        {
            get { return drawTransparentPixels; }
            set { drawTransparentPixels = value; }
        }

        /// <summary>
        /// The drawing order for prioritized drawing.
        /// </summary>
        public int DrawPriority
        {
            get { return drawPriority; }
            set { drawPriority = value; }
        }

        /// <summary>
        /// PreDraw the surface in a scene. See notes on PreDraw in
        /// <see cref="Scene"/>.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the surface is viewed.</param>
        public void PreDraw(GameTime gameTime, Camera camera)
        {
            PreDrawHook(gameTime, camera);
        }

        /// <summary>
        /// Draw the surface in a scene using the specified drawing mode.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the surface is viewed.</param>
        /// <param name="drawMode">The manner in which to draw the surface.</param>
        public void Draw(GameTime gameTime, Camera camera, string drawMode)
        {
            DrawHook(gameTime, camera, drawMode);
        }

        /// <summary>
        /// Does the surface have the specified DrawProperties?
        /// </summary>
        /// <param name="properties">The draw properties to check for.</param>
        /// <returns>True if the surface has the specified draw properties.</returns>
        public bool HasDrawProperties(DrawProperties properties)
        {
            return (DrawProperties & properties) == properties;
        }

        /// <summary>
        /// The surface's parent actor.
        /// </summary>
        protected Actor3D Actor { get { return actor; } }

        /// <summary>
        /// Handle getting the bounding box.
        /// </summary>
        protected abstract BoundingBox BoundingBoxHook { get; }

        /// <summary>
        /// Handle getting the effect.
        /// </summary>
        protected abstract Effect EffectHook { get; }

        /// <summary>
        /// Handle getting and setting the effect file name.
        /// </summary>
        protected abstract string EffectFileNameHook { get; set; }

        /// <summary>
        /// Handle getting if the surface is boundless.
        /// </summary>
        protected abstract bool BoundlessHook { get; }

        /// <summary>
        /// Handle predrawing the surface.
        /// </summary>
        protected abstract void PreDrawHook(GameTime gameTime, Camera camera);

        /// <summary>
        /// Handle predrawing the surface.
        /// </summary>
        protected abstract void DrawHook(GameTime gameTime, Camera camera, string drawMode);
        
        /// <summary>
        /// The game.
        /// </summary>
        protected XiGame Game { get { return game; } }

        private readonly XiGame game;
        private readonly Actor3D actor;
        private DrawProperties drawProperties = DrawProperties.Reflecting | DrawProperties.Shadowing;
        private DrawStyle drawStyle = DrawStyle.Opaque;
        private FaceMode faceMode = FaceMode.FrontFaces;
        private Color diffuseColor = Color.White;
        private Color specularColor = Color.Gray;
        private float specularPower = 8;
        private bool lightingEnabled = true;
        private bool drawTransparentPixels;
        private int drawPriority;
    }
}
