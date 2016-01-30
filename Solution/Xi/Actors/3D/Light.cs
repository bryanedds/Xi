using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// A light that can be added to a 3D scene.
    /// </summary>
    public abstract class Light : Actor3D
    {
        /// <summary>
        /// Create a Light3D object.
        /// </summary>
        /// <param name="game">The game.</param>
        public Light(XiGame game) : base(game, false) { }
    }

    /// <summary>
    /// Helper methods for various 3D lighting tasks.
    /// </summary>
    public static class Light3DHelper
    {
        /// <summary>
        /// Sort a list of point lights by distance from an origin.
        /// </summary>
        /// <param name="list">The list of point lights to be sorted.</param>
        /// <param name="origin">The origin from which distance is measured.</param>
        /// <param name="sortOrder">The order in which sorting takes place.</param>
        public static void DistanceSort(this List<PointLight> list, Vector3 origin, SpatialSortOrder sortOrder)
        {
            XiHelper.ArgumentNullCheck(list);
            IDistanceComparer<PointLight> comparer =
                sortOrder == SpatialSortOrder.FarToNear ?
                farToNearComparer :
                nearToFarComparer;
            comparer.Origin = origin;
            list.Sort(comparer);
        }

        private static readonly IDistanceComparer<PointLight> nearToFarComparer = new NearToFarComparer<PointLight>(Vector3.Zero);
        private static readonly IDistanceComparer<PointLight> farToNearComparer = new FarToNearComparer<PointLight>(Vector3.Zero);
    }
}
