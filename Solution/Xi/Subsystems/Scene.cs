using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// The 3D scene.
    /// TODO: Instead of figuring out how to light a scene by casting the actors, instead rename
    /// Light to LightSource and add a new Light concept that is collected from actors via a
    /// CollectLights() method (similar to the CollectSurfaces() method). This will allow actors to
    /// emit light without having to inherit from a light actor. It will even allow multiple lights
    /// to be emitted from a single actor.
    /// </summary>
    public class Scene : Disposable
    {
        /// <summary>
        /// Create a Scene3D object.
        /// </summary>
        /// <param name="game">The game.</param>
        public Scene(XiGame game)
        {
            XiHelper.ArgumentNullCheck(game);
            this.game = game;
        }

        /// <summary>
        /// A cached collection of the scene's actors.
        /// </summary>
        public List<Actor3D> CachedActors { get { return cachedActors; } }

        /// <summary>
        /// A cached collection of the scene actors' surfaces.
        /// </summary>
        public List<Surface> CachedSurfaces { get { return cachedSurfaces; } }

        /// <summary>
        /// A cached collection of the scene's visible ambient lights.
        /// </summary>
        public List<AmbientLight> CachedAmbientLights { get { return cachedAmbientLights; } }

        /// <summary>
        /// A cached collection of the scene's visible directional lights.
        /// </summary>
        public List<DirectionalLight> CachedDirectionalLights { get { return cachedDirectionalLights; } }

        /// <summary>
        /// A cached collection of the scene's visible point lights.
        /// </summary>
        public List<PointLight> CachedPointLights { get { return cachedPointLights; } }

        /// <summary>
        /// The bounds within which drawing occurs.
        /// </summary>
        public BoundingBox DrawBounds
        {
            get { return drawBounds; }
            set { drawBounds = value; }
        }

        /// <summary>
        /// The fog.
        /// </summary>
        public Fog Fog { get { return fog; } }

        /// <summary>
        /// Should the opaque items be drawn in near-to-far order?
        /// </summary>
        public bool DrawOpaquesNearToFar
        {
            get { return drawOpaquesFrontToBack; }
            set { drawOpaquesFrontToBack = value; }
        }

        /// <summary>
        /// PreDraw the items in the DrawBounds. Must be called once before Draw.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the scene is viewed.</param>
        /// <remarks>
        /// Pre-drawing is done in a different phase than normal drawing. This is because of the
        /// following performance constraint -
        /// 
        /// Intermixing scene drawing with pre-drawing involves switching render targets while
        /// persisting their data. Persisting render target data during render target changes
        /// involves copying the render target buffer to temporary texture buffers on the Xbox 360,
        /// then copying it back. This is just too slow so long as the Xbox 360 or a device with
        /// the same constraint is a deployment target.
        /// 
        /// For more info, please read -
        /// http://blogs.msdn.com/shawnhar/archive/2007/11/21/rendertarget-changes-in-xna-game-studio-2-0.aspx
        /// 
        /// At some point it might make sense to have a PreDrawOrder property so that items can be
        /// consistently pre-drawn in a certain order rather than the random order in which they
        /// are found in the scene.
        /// </remarks>        
        public void PreDraw(GameTime gameTime, Camera camera)
        {
            XiHelper.ArgumentNullCheck(gameTime, camera);
            CacheActors();
            CacheLights();
            CacheSurfaces();
            DrawShadows(gameTime, camera);
            PreDrawSurfaces(gameTime, camera);
        }

        /// <summary>
        /// Draw the items in the DrawingBounds.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the scene is viewed.</param>
        /// <param name="drawMode">The manned in which to draw the scene.</param>
        public void Draw(GameTime gameTime, Camera camera, string drawMode)
        {
            DrawSurfaces(gameTime, camera, drawMode);
            ClearSurfaces();
            ClearLights();
            ClearActors();
        }

        /// <summary>
        /// Add an actor to the scene.
        /// </summary>
        public void AddActor(Actor3D actor)
        {
            XiHelper.ArgumentNullCheck(actor);
            actorPSet.Add(actor);
        }

        /// <summary>
        /// Remove a actor from the scene.
        /// </summary>
        public bool RemoveActor(Actor3D actor)
        {
            XiHelper.ArgumentNullCheck(actor);
            return actorPSet.Remove(actor);
        }

        private void CacheActors()
        {
            foreach (Actor3D actor in actorPSet.Values)
                if (actor.Visible)
                    cachedActors.Add(actor);
        }

        private void CacheLights()
        {
            // cache lights
            foreach (Actor3D actor in cachedActors)
            {
                Light light = actor as Light;
                if (light != null && light.Enabled) cachedLights.Add(light);
            }

            // organize them
            OrganizeCachedLights();
        }

        private void CacheSurfaces()
        {
            foreach (Actor3D actor in cachedActors) actor.CollectSurfaces(cachedSurfaces2);
            foreach (Surface surface in cachedSurfaces2)
                if (drawBounds.Contains(surface.BoundingBox) != ContainmentType.Disjoint)
                    cachedSurfaces.Add(surface);
            cachedSurfaces2.Clear();
        }

        private void DrawShadows(GameTime gameTime, Camera camera)
        {
            foreach (DirectionalLight directionalLight in CachedDirectionalLights)
                if (directionalLight.Enabled && directionalLight.ShadowEnabled)
                    directionalLight.DrawShadow(gameTime, camera);
        }

        private void PreDrawSurfaces(GameTime gameTime, Camera camera)
        {
            game.SurfaceDrawer3D.PreDrawSurfaces(gameTime, camera, cachedSurfaces);
        }

        private void DrawSurfaces(GameTime gameTime, Camera camera, string drawMode)
        {
            game.SurfaceDrawer3D.DrawSurfaces(gameTime, camera, drawMode, cachedSurfaces);
        }

        private void ClearSurfaces()
        {
            cachedSurfaces.Clear();
        }

        private void ClearLights()
        {
            cachedLights.Clear();
            cachedPointLights.Clear();
            cachedDirectionalLights.Clear();
            cachedAmbientLights.Clear();
        }

        private void ClearActors()
        {
            cachedActors.Clear();
        }

        private void OrganizeCachedLights()
        {
            foreach (Light light in cachedLights)
                OrganizeLight(light);
        }

        private void OrganizeLight(Light light)
        {
            PointLight pointLight = light as PointLight;
            if (pointLight != null) cachedPointLights.Add(pointLight);
            else
            {
                DirectionalLight directionalLight = light as DirectionalLight;
                if (directionalLight != null) cachedDirectionalLights.Add(directionalLight);
                else
                {
                    AmbientLight ambientLight = light as AmbientLight;
                    if (ambientLight != null) cachedAmbientLights.Add(ambientLight);
                }
            }
        }

        private readonly Dictionary<Actor3D, Actor3D> actorPSet = new Dictionary<Actor3D, Actor3D>(); // TODO: replace pseudo-sets with HashSets
        private readonly List<Actor3D> cachedActors = new List<Actor3D>();
        private readonly List<Surface> cachedSurfaces = new List<Surface>();
        private readonly List<Surface> cachedSurfaces2 = new List<Surface>();
        private readonly List<DirectionalLight> cachedDirectionalLights = new List<DirectionalLight>();
        private readonly List<AmbientLight> cachedAmbientLights = new List<AmbientLight>();
        private readonly List<PointLight> cachedPointLights = new List<PointLight>();
        private readonly List<Light> cachedLights = new List<Light>();
        private readonly XiGame game;
        private readonly Fog fog = new Fog();
        private BoundingBox drawBounds = BoundingBoxHelper.CreateAllEncompassing();
        private bool drawOpaquesFrontToBack;
    }
}
