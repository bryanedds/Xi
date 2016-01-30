using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// The direction in which spatial sorting takes place.
    /// </summary>
    public enum SpatialSortOrder
    {
        NearToFar = 0,
        FarToNear
    }

    /// <summary>
    /// Compares the distance of Actor3Ds from an origin.
    /// </summary>
    public interface IDistanceComparer<T> : IComparer<T>
        where T : Actor3D
    {
        /// <summary>
        /// The origin from which distance is calculated.
        /// </summary>
        Vector3 Origin { get; set; }
    }

    /// <summary>
    /// Compares the distance of Actor3Ds from an origin, sorting in ascending order.
    /// </summary>
    public class NearToFarComparer<T> : IDistanceComparer<T>
        where T : Actor3D
    {
        public NearToFarComparer(Vector3 origin)
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
            return XiMathHelper.Compare(
                x.DistanceSquared(origin),
                y.DistanceSquared(origin));
        }

        private Vector3 origin;
    }

    /// <summary>
    /// Compares the distance of Actor3Ds from an origin, sorting in descending order.
    /// </summary>
    public class FarToNearComparer<T> : IDistanceComparer<T>
        where T : Actor3D
    {
        public FarToNearComparer(Vector3 origin)
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
            return XiMathHelper.CompareInverted(
                x.DistanceSquared(origin),
                y.DistanceSquared(origin));
        }

        private Vector3 origin;
    }

    /// <summary>
    /// Provides helper methods for math operations in Xi.
    /// </summary>
    public static class XiMathHelper
    {
        /// <summary>
        /// Compare the values of two floats.
        /// </summary>
        public static int Compare(this float x, float y)
        {
            if (x < y) return -1;
            if (x == y) return 0;
            return 1;
        }

        /// <summary>
        /// Compare the values of two floats in an inverted manner.
        /// </summary>
        public static int CompareInverted(this float x, float y)
        {
            if (x < y) return 1;
            if (x == y) return 0;
            return -1;
        }

        /// <summary>
        /// Compare the absolute values of two floats.
        /// </summary>
        public static int CompareAbsolute(this float x, float y)
        {
            return Math.Abs(x).Compare(Math.Abs(y));
        }

        /// <summary>
        /// Compare the absolute values of two floats in an inverted manner.
        /// </summary>
        public static int CompareInvertedAbsolute(this float x, float y)
        {
            return Math.Abs(x).CompareInverted(Math.Abs(y));
        }

        /// <summary>
        /// Get the x,y components of the vector.
        /// </summary>
        public static Vector2 GetXY(this Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }

        /// <summary>
        /// Calculate the minimum angle between two vector2s.
        /// </summary>
        public static float AngleBetween(Vector2 v1, Vector2 v2)
        {
            return (float)(Math.Atan2(v2.Y, v2.X) - Math.Atan2(v1.Y, v1.X));
        }

        /// <summary>
        /// Calculate the minimum angle between two vector3s.
        /// </summary>
        public static float AngleBetween(Vector3 v1, Vector3 v2)
        {
            float v1DotV2 = Vector3.Dot(v1, v2);
            float v1LengthTimesV2Length = v1.Length() * v2.Length();
            return (float)Math.Acos(v1DotV2 / v1LengthTimesV2Length);
        }

        /// <summary>
        /// Calculate the component vector of two vectors.
        /// </summary>
        public static Vector3 ComponentVector(Vector3 u, Vector3 v)
        {
            return Vector3.Dot(u, v) * v;
        }

        /// <summary>
        /// Get the center of the view port.
        /// </summary>
        public static Vector2 GetCenter(this Viewport viewport)
        {
            return new Vector2(viewport.Width, viewport.Height) * 0.5f;
        }

        /// <summary>
        /// Get the segment that starts at the near plane and ends at the far plane from the
        /// perspective of a point on the viewport.
        /// </summary>
        public static Segment ToSegment(this Viewport viewport, Camera camera, Vector2 point)
        {
            XiHelper.ArgumentNullCheck(camera);
            Vector2 positionClipPlane = 2 * point / new Vector2(viewport.Width, viewport.Height);
            positionClipPlane.X -= 1.0f;
            positionClipPlane.Y = 1.0f - positionClipPlane.Y;
            Vector3 start = camera.ClipPlaneToWorld(new Vector3(positionClipPlane, 0.0f));
            Vector3 end = camera.ClipPlaneToWorld(new Vector3(positionClipPlane, 1.0f));
            return new Segment(start, end);
        }

        /// <summary>
        /// Are the numbers both positive or both negative?
        /// </summary>
        public static bool SameSign(float a, float b)
        {
            return
                a >= 0 && b >= 0 ||
                a <= 0 && b <= 0;
        }

        /// <summary>
        /// Is a point inside the triangle formed by a, b, and c?
        /// </summary>
        public static bool Inside(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
        {
            // BUG: This code is not working very well and is probably ill-conceived.
            Vector3 ab = b - a;
            Vector3 ap = point - a;
            float apDotAb = Vector3.Dot(ap, ab);
            if (!SameSign(apDotAb, ab.Length())) return false;
            Vector3 bc = c - b;
            Vector3 bp = point - b;
            float bpDotBc = Vector3.Dot(bp, bc);
            if (!SameSign(bpDotBc, bc.Length())) return false;
            Vector3 ca = a - c;
            Vector3 cp = point - c;
            float cpDotCa = Vector3.Dot(cp, ca);
            if (!SameSign(cpDotCa, ca.Length())) return false;
            return true;
        }

        /// <summary>
        /// Does the bounding box intersect the given segment?
        /// </summary>
        public static bool Intersection(ref BoundingBox box, ref Segment segment)
        {
            // this algorithm was straight-up thudged from -
            // http://www.gamasutra.com/features/19991018/Gomez_6.htm
            Vector3 segmentMid = (segment.Start + segment.End) * 0.5f;
            Vector3 segmentDirection = Vector3.Normalize(segment.End - segment.Start);
            float segmentHalfLength = (segment.End - segmentMid).Length();
            Vector3 boxMid = (box.Min + box.Max) * 0.5f;
            Vector3 boxExtent = box.Max - boxMid;
            Vector3 a = boxMid - segmentMid;

            // do any of the principal axes form a separating axis?
            if (Math.Abs(a.X) > boxExtent.X + segmentHalfLength * Math.Abs(segmentDirection.X)) return false;
            if (Math.Abs(a.Y) > boxExtent.Y + segmentHalfLength * Math.Abs(segmentDirection.Y)) return false;
            if (Math.Abs(a.Z) > boxExtent.Z + segmentHalfLength * Math.Abs(segmentDirection.Z)) return false;

            // NOTE: Since the separating axis is perpendicular to the line in these last four
            // cases, the line does not contribute to the projection.
            float b = boxExtent.Y * Math.Abs(segmentDirection.Z) + boxExtent.Z * Math.Abs(segmentDirection.Y);
            if (Math.Abs(a.Y * segmentDirection.Z - a.Z * segmentDirection.Y) > b) return false;
            b = boxExtent.X * Math.Abs(segmentDirection.Z) + boxExtent.Z * Math.Abs(segmentDirection.X);
            if (Math.Abs(a.Z * segmentDirection.X - a.X * segmentDirection.Z) > b) return false;
            b = boxExtent.X * Math.Abs(segmentDirection.Y) + boxExtent.Y * Math.Abs(segmentDirection.X);
            if (Math.Abs(a.X * segmentDirection.Y - a.Y * segmentDirection.X) > b) return false;
            return true;
        }

        /// <summary>
        /// Determines the intersection of a segment and a bounding box.
        /// </summary>
        public static bool Intersection(ref Segment segment, ref BoundingBox box, out float tinter)
        {
            return Intersection(ref box, ref segment, out tinter);
        }

        /// <summary>
        /// Determines the intersection of a bounding box and a segment.
        /// </summary>
        public static bool Intersection(ref BoundingBox box, ref Segment segment, out float tinter)
        {
            // this algorithm was taken from -
            // http://www.gamedev.net/community/forums/topic.asp?topic_id=338987
            tinter = 0;
            float tenter = 0.0f, texit = 1.0f;
            if (!RaySlabIntersects(box.Min.X, box.Max.X, segment.Start.X, segment.End.X, ref tenter, ref texit)) return false;
            if (!RaySlabIntersects(box.Min.Y, box.Max.Y, segment.Start.Y, segment.End.Y, ref tenter, ref texit)) return false;
            if (!RaySlabIntersects(box.Min.Z, box.Max.Z, segment.Start.Z, segment.End.Z, ref tenter, ref texit)) return false;
            tinter = tenter;
            return true;
        }

        /// <summary>
        /// Determines the intersection of a triangle and a ray.
        /// </summary>
        public static bool Intersection(ref Triangle triangle, ref Ray ray, out Vector3 intersection)
        {
            return Intersection(ref ray, ref triangle, out intersection);
        }

        /// <summary>
        /// Determines the intersection of a ray and a triangle.
        /// </summary>
        public static bool Intersection(ref Ray ray, ref Triangle triangle, out Vector3 intersection)
        {
            Plane plane = PlaneHelper.CreateFromPositionAndNormal(triangle.A, triangle.Normal);
            bool rayPlaneIntersection = Intersection(ref ray, ref plane, out intersection);
            return
                rayPlaneIntersection &&
                XiMathHelper.Inside(intersection, triangle.A, triangle.B, triangle.C);
        }

        /// <summary>
        /// Is the plane intersected by the ray?
        /// </summary>
        public static bool Intersection(ref Plane plane, ref Ray ray, out Vector3 intersection)
        {
            return Intersection(ref ray, ref plane, out intersection);
        }

        /// <summary>
        /// Is the ray intersecting the plane?
        /// </summary>
        public static bool Intersection(ref Ray ray, ref Plane plane, out Vector3 intersection)
        {
            intersection = Vector3.Zero;
            Vector3 p1 = ray.Position;
            Vector3 n1 = ray.Direction;
            Vector3 p2 = plane.GetPosition();
            Vector3 n2 = plane.Normal;
            Vector3 d = p2 - p1;
            if (Vector3.Dot(n1, n2) == 0 && p1 == p2) return true;
            float t = Vector3.Dot(p1 - p2, n2) / Vector3.Dot(n1, n2);
            intersection = p1 - n1 * t;
            return true;
        }

        /// <summary>
        /// Create a plane at the specified position and normal.
        /// </summary>
        public static Plane CreatePlaneFromPositionAndNormal(Vector3 position, Vector3 normal)
        {
            float d;
            Vector3.Dot(ref position, ref normal, out d);
            return new Plane(normal, -d);
        }

        /// <summary>
        /// Transform a float to a given snap stride.
        /// </summary>
        public static float GetSnap(this float value, float snap)
        {
            if (snap == 0) return value;
            float low = value - value % snap;
            float high = low + snap;
            float halfSnap = snap * 0.5f;
            return value - low < halfSnap ? low : high;
        }

        /// <summary>
        /// Transform a Vector2 to a given snap stride.
        /// </summary>
        public static Vector2 GetSnap(this Vector2 value, float snap)
        {
            return value.GetSnap(new Vector2(snap));
        }

        /// <summary>
        /// Transform a Vector2 to a given snap space.
        /// </summary>
        public static Vector2 GetSnap(this Vector2 value, Vector2 snap)
        {
            return new Vector2(
                value.X.GetSnap(snap.X),
                value.Y.GetSnap(snap.Y));
        }

        /// <summary>
        /// Transform a Vector3 to a given snap stride.
        /// </summary>
        public static Vector3 GetSnap(this Vector3 value, float snap)
        {
            return value.GetSnap(new Vector3(snap));
        }

        /// <summary>
        /// Transform a Vector3 to a given snap space.
        /// </summary>
        public static Vector3 GetSnap(this Vector3 value, Vector3 snap)
        {
            return new Vector3(
                value.X.GetSnap(snap.X),
                value.Y.GetSnap(snap.Y),
                value.Z.GetSnap(snap.Z));
        }

        /// <summary>
        /// Transform a Vector4 to a given snap stride.
        /// </summary>
        public static Vector4 GetSnap(this Vector4 value, float snap)
        {
            return value.GetSnap(new Vector4(snap));
        }

        /// <summary>
        /// Transform a Vector4 to a given snap space.
        /// </summary>
        public static Vector4 GetSnap(this Vector4 value, Vector4 snap)
        {
            return new Vector4(
                value.X.GetSnap(snap.X),
                value.Y.GetSnap(snap.Y),
                value.Z.GetSnap(snap.Z),
                value.W.GetSnap(snap.W));
        }

        /// <summary>
        /// Get two arbitrary vectors that, given the incoming vector, create a well-formed
        /// orientation matrix.
        /// </summary>
        public static void GetComplimentaryOrientationVectors(this Vector3 v1, out Vector3 v2, out Vector3 v3)
        {
            if (Vector3.Cross(v1, Vector3.Left) != Vector3.Zero)
            {
                v2 = Vector3.Normalize(Vector3.Cross(v1, Vector3.Left));
                v3 = Vector3.Normalize(Vector3.Cross(v1, v2));
            }
            else
            {
                v2 = Vector3.Normalize(Vector3.Cross(v1, Vector3.Down));
                v3 = Vector3.Normalize(Vector3.Cross(v1, v2));
            }
        }

        /// <summary>
        /// Get the squared distance of an Actor3D from an origin.
        /// </summary>
        /// <param name="actor">The actor to find the distance of.</param>
        /// <param name="origin">The origin from which distance is measured.</param>
        /// <returns>The squared distance.</returns>
        public static float DistanceSquared(this Actor3D actor, Vector3 origin)
        {
            XiHelper.ArgumentNullCheck(actor);
            float result;
            Vector3 boxCenter = actor.BoundingBox.GetCenter();
            Vector3.DistanceSquared(ref origin, ref boxCenter, out result);
            return result;
        }

        /// <summary>
        /// Get the squared distance of a Surface3D from an origin.
        /// </summary>
        /// <param name="surface">The surface to find the distance of.</param>
        /// <param name="origin">The origin from which distance is measured.</param>
        /// <returns>The squared distance.</returns>
        public static float DistanceSquared(this Surface surface, Vector3 origin)
        {
            XiHelper.ArgumentNullCheck(surface);
            float result;
            Vector3 boxCenter = surface.BoundingBox.GetCenter();
            Vector3.DistanceSquared(ref origin, ref boxCenter, out result);
            return result;
        }

        private static bool RaySlabIntersects(float slabMin, float slabMax, float rayStart, float rayEnd, ref float tbenter, ref float tbexit)
        {
            // this algorithm was taken from -
            // http://www.gamedev.net/community/forums/topic.asp?topic_id=338987

            // calculate the ray direction
            float raydir = rayEnd - rayStart;

            // ray parallel to the slab
            if (Math.Abs(raydir) < 1.0E-9f)
            {
                // ray parallel to the slab, but ray not inside the slab planes
                if (rayStart < slabMin || rayStart > slabMax) return false;
                // ray parallel to the slab, but ray inside the slab planes
                return true;
            }

            // slab's enter and exit parameters
            float tsenter = (slabMin - rayStart) / raydir;
            float tsexit = (slabMax - rayStart) / raydir;

            // order the enter / exit values.
            if (tsenter > tsexit) XiHelper.Swap(ref tsenter, ref tsexit);

            // make sure the slab interval and the current box intersection interval overlap
            if (tbenter > tsexit || tsenter > tbexit) return false; // Ray missed the box.

            // yep, the slab and current intersection interval overlap...
            // update the intersection interval
            tbenter = MathHelper.Max(tbenter, tsenter);
            tbexit = MathHelper.Min(tbexit, tsexit);
            return true;
        }
    }
}
