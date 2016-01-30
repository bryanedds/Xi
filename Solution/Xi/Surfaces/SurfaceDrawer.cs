using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// Compares the drawing priority of surfaces.
    /// </summary>
    public class DrawPriorityComparer : IComparer<Surface>
    {
        /// <inheritdoc />
        public int Compare(Surface x, Surface y)
        {
            return XiMathHelper.Compare(x.DrawPriority, y.DrawPriority);
        }
    }

    /// <summary>
    /// Compares the distance of surfaces from an origin.
    /// </summary>
    public interface IDrawOrderComparer<T> : IComparer<T>
        where T : Surface
    {
        /// <summary>
        /// The origin from which distance is calculated.
        /// </summary>
        Vector3 Origin { get; set; }
    }

    /// <summary>
    /// Compares the distance of surfaces from an origin, sorting in ascending order.
    /// </summary>
    public class DrawNearToFarComparer<T> : IDrawOrderComparer<T>
        where T : Surface
    {
        public DrawNearToFarComparer(Vector3 origin)
        {
            this.origin = origin;
        }

        public Vector3 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public int Compare(T x, T y)
        {
            XiHelper.ArgumentNullCheck(x, y);
            float distance1 = x.DistanceSquared(origin);
            float distance2 = y.DistanceSquared(origin);
            if (distance1 < distance2) return -1;
            if (distance1 > distance2) return 1;
            if ((int)x.FaceMode < (int)y.FaceMode) return -1;
            if ((int)x.FaceMode > (int)y.FaceMode) return 1;
            return 0;
        }

        private Vector3 origin;
    }

    /// <summary>
    /// Compares the distance of surfaces from an origin, sorting in descending order.
    /// </summary>
    public class DrawFarToNearComparer<T> : IDrawOrderComparer<T>
        where T : Surface
    {
        public DrawFarToNearComparer(Vector3 origin)
        {
            this.origin = origin;
        }

        public Vector3 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public int Compare(T x, T y)
        {
            XiHelper.ArgumentNullCheck(x, y);
            float distance1 = x.DistanceSquared(origin);
            float distance2 = y.DistanceSquared(origin);
            if (distance1 > distance2) return -1;
            if (distance1 < distance2) return 1;
            if ((int)x.FaceMode > (int)y.FaceMode) return -1;
            if ((int)x.FaceMode < (int)y.FaceMode) return 1;
            return 0;
        }

        private Vector3 origin;
    }

    /// <summary>
    /// Helper methods for surface drawing.
    /// </summary>
    public static class SurfeceDrawHelper
    {
        /// <summary>
        /// Sort a list of surfaces by distance from an origin.
        /// </summary>
        /// <param name="surfaces">The surfaces to be sorted.</param>
        /// <param name="origin">The origin from which distance is measured.</param>
        /// <param name="sortOrder">The order in which sorting takes place.</param>
        public static void DistanceSort(this List<Surface> surfaces, Vector3 origin, SpatialSortOrder sortOrder)
        {
            XiHelper.ArgumentNullCheck(surfaces);
            IDrawOrderComparer<Surface> comparer;
            if (sortOrder == SpatialSortOrder.FarToNear) comparer = farToNearComparer;
            else comparer = nearToFarComparer;                     
            comparer.Origin = origin;
            surfaces.Sort(comparer);
        }

        /// <summary>
        /// Sort a list of surfaces by their Priority property.
        /// </summary>
        /// <param name="surfaces">The surfaces to be sorted.</param>
        public static void PrioritySort(this List<Surface> surfaces)
        {
            XiHelper.ArgumentNullCheck(surfaces);
            surfaces.Sort(priorityComparer);
        }
        
        private static readonly DrawNearToFarComparer<Surface> nearToFarComparer = new DrawNearToFarComparer<Surface>(Vector3.Zero);
        private static readonly DrawFarToNearComparer<Surface> farToNearComparer = new DrawFarToNearComparer<Surface>(Vector3.Zero);
        private static readonly DrawPriorityComparer priorityComparer = new DrawPriorityComparer();
    }

    /// <summary>
    /// Draws 3D surfaces.
    /// </summary>
    public class SurfaceDrawer
    {
        /// <summary>
        /// Create a SurfaceDrawer3D object.
        /// </summary>
        /// <param name="game">The game.</param>
        public SurfaceDrawer(XiGame game)
        {
            this.game = game;
        }

        /// <summary>
        /// PreDraw a single surface.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the surface is viewed.</param>
        /// <param name="surface">The surface to pre-draw.</param>
        public void PreDrawSurface(GameTime gameTime, Camera camera, Surface surface)
        {
            XiHelper.ArgumentNullCheck(gameTime, camera);
            surface.PreDraw(gameTime, camera);
        }

        /// <summary>
        /// Draw a single surface.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the surface is viewed.</param>
        /// <param name="drawMode">The manner in which to draw the surface.</param>
        /// <param name="surface">The surface to draw.</param>
        public void DrawSurface(GameTime gameTime, Camera camera, string drawMode, Surface surface)
        {
            XiHelper.ArgumentNullCheck(gameTime, camera);
            surface.Draw(gameTime, camera, drawMode);
        }

        /// <summary>
        /// Pre-draw multiple surfaces.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the surface are viewed.</param>
        /// <param name="surfaces">The surfaces to pre-draw.</param>
        public void PreDrawSurfaces(GameTime gameTime, Camera camera, List<Surface> surfaces)
        {
            XiHelper.ArgumentNullCheck(gameTime, camera, surfaces);
            foreach (Surface surface in surfaces) surface.PreDraw(gameTime, camera);
        }

        /// <summary>
        /// Draw multiple surfaces.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the surface are viewed.</param>
        /// <param name="drawMode">The manner in which to draw the surface.</param>
        /// <param name="surfaces">The surfaces to draw.</param>
        public void DrawSurfaces(GameTime gameTime, Camera camera, string drawMode, List<Surface> surfaces)
        {
            XiHelper.ArgumentNullCheck(gameTime, camera, surfaces);
            OrganizeSurfaces(camera, surfaces);
            DrawSurfaces(gameTime, camera, drawMode);
            ClearSurfaces();
        }

        private void OrganizeSurfaces(Camera camera, List<Surface> surfaces)
        {
            foreach (Surface surface in surfaces)
                OrganizeSurface(camera, surface);
        }

        private void OrganizeSurface(Camera camera, Surface surface)
        {
            if (surface.Boundless || camera.Contains(surface.BoundingBox) != ContainmentType.Disjoint)
            {
                switch (surface.DrawStyle)
                {
                    case DrawStyle.Prioritized: cachedPriors.Add(surface); break;
                    case DrawStyle.Opaque: cachedOpaques.Add(surface); break;
                    case DrawStyle.Transparent: cachedTransparents.Add(surface); break;
                }
            }
        }

        private void DrawSurfaces(GameTime gameTime, Camera camera, string drawMode)
        {
            DrawPriors(gameTime, camera, drawMode);
            DrawOpaques(gameTime, camera, drawMode);
            DrawTransparents(gameTime, camera, drawMode);
        }

        private void DrawPriors(GameTime gameTime, Camera camera, string drawMode)
        {
            SurfeceDrawHelper.PrioritySort(cachedPriors);
            foreach (Surface surface in cachedPriors) surface.Draw(gameTime, camera, drawMode);
        }

        private void DrawOpaques(GameTime gameTime, Camera camera, string drawMode)
        {
            if (game.Scene.DrawOpaquesNearToFar) SurfeceDrawHelper.DistanceSort(cachedOpaques, camera.Position, SpatialSortOrder.NearToFar);
            foreach (Surface surface in cachedOpaques) surface.Draw(gameTime, camera, drawMode);
        }

        private void DrawTransparents(GameTime gameTime, Camera camera, string drawMode)
        {
            SurfeceDrawHelper.DistanceSort(cachedTransparents, camera.Position, SpatialSortOrder.FarToNear);
            foreach (Surface surface in cachedTransparents) surface.Draw(gameTime, camera, drawMode);
        }

        private void ClearSurfaces()
        {
            cachedPriors.Clear();
            cachedOpaques.Clear();
            cachedTransparents.Clear();
        }

        private readonly List<Surface> cachedPriors = new List<Surface>();
        private readonly List<Surface> cachedOpaques = new List<Surface>();
        private readonly List<Surface> cachedTransparents = new List<Surface>();
        private readonly XiGame game;
    }
}
