using System;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// An Array extension method class.
    /// </summary>
    public static class ArrayHelper
    {
        /// <summary>
        /// Convert a .NET bool to a GPU-friendly 'bool'.
        /// </summary>
        public static int ToGpuBool(this bool value)
        {
            return value ? -1 : 0;
        }

        /// <summary>
        /// A friendlier version of the simple Array.Copy method.
        /// </summary>
        /// <param name="destination">The copy destination.</param>
        /// <param name="source">The copy source.</param>
        /// <param name="length">The number of elements to copy.</param>
        public static void CopyFrom(this Array destination, Array source, int length)
        {
            Array.Copy(source, destination, length);
        }

        /// <summary>
        /// Copies a bool array to a GPU-friendly 'bool' array.
        /// </summary>
        /// <param name="destination">The copy destination.</param>
        /// <param name="source">The copy source.</param>
        /// <param name="length">The number of elements to copy.</param>
        public static void CopyFrom(this int[] destination, bool[] source, int length)
        {
            XiHelper.ArgumentNullCheck(destination, source);
            if (length > destination.Length || length > source.Length)
                throw new ArgumentOutOfRangeException("Length of copy must be <= length of both arrays.");
            for (int i = 0; i < length; ++i)
                destination[i] = ToGpuBool(source[i]);
        }

        /// <summary>
        /// Are two arrays of the same size and equal elements?
        /// </summary>
        public static bool EqualsValue(this int[] first, bool[] second)
        {
            XiHelper.ArgumentNullCheck(first, second);
            if (first.Length != second.Length) return false;
            for (int i = 0; i < first.Length; ++i)
                if (first[i] != ToGpuBool(second[i]))
                    return false;
            return true;
        }

        /// <summary>
        /// Are two arrays of the same size and equal elements?
        /// </summary>
        public static bool EqualsValue(this bool[] first, bool[] second)
        {
            XiHelper.ArgumentNullCheck(first, second);
            if (first.Length != second.Length) return false;
            for (int i = 0; i < first.Length; ++i)
                if (first[i] != second[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Are two arrays of the same size and equal elements?
        /// </summary>
        public static bool EqualsValue(this float[] first, float[] second)
        {
            XiHelper.ArgumentNullCheck(first, second);
            if (first.Length != second.Length) return false;
            for (int i = 0; i < first.Length; ++i)
                if (first[i] != second[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Are two arrays of the same size and equal elements?
        /// </summary>
        public static bool EqualsValue(this Vector2[] first, Vector2[] second)
        {
            XiHelper.ArgumentNullCheck(first, second);
            if (first.Length != second.Length) return false;
            for (int i = 0; i < first.Length; ++i)
                if (first[i] != second[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Are two arrays of the same size and equal elements?
        /// </summary>
        public static bool EqualsValue(this Vector3[] first, Vector3[] second)
        {
            XiHelper.ArgumentNullCheck(first, second);
            if (first.Length != second.Length) return false;
            for (int i = 0; i < first.Length; ++i)
                if (first[i] != second[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Are two arrays of the same size and equal elements?
        /// </summary>
        public static bool EqualsValue(this Vector4[] first, Vector4[] second)
        {
            XiHelper.ArgumentNullCheck(first, second);
            if (first.Length != second.Length) return false;
            for (int i = 0; i < first.Length; ++i)
                if (first[i] != second[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Are two arrays of the same size and equal elements?
        /// </summary>
        public static bool EqualsValue(this Matrix[] first, Matrix[] second)
        {
            XiHelper.ArgumentNullCheck(first, second);
            if (first.Length != second.Length) return false;
            for (int i = 0; i < first.Length; ++i)
                if (first[i] != second[i])
                    return false;
            return true;
        }
    }
}
