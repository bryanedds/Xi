using System;

namespace Xi
{
    /// <summary>
    /// Generalized helper methods used in Xi.
    /// </summary>
    public static class XiHelper
    {
        /// <summary>
        /// Cast an object to type T.
        /// </summary>
        /// <remarks>
        /// It's rather difficult to find C-style casts on object references.
        /// </remarks>
        /// <param name="obj">The object to cast. May be null.</param>
        public static T Cast<T>(object obj) where T : class
        {
            return (T)obj;
        }

        /// <summary>
        /// Swaps the contents of two values.
        /// </summary>
        public static void Swap<T>(ref T x, ref T y) where T : struct
        {
            T temp = x;
            x = y;
            y = temp;
        }

        /// <summary>
        /// Swaps the contents of two references.
        /// </summary>
        public static void SwapRef<T>(ref T x, ref T y) where T : class
        {
            T temp = x;
            x = y;
            y = temp;
        }

        /// <summary>
        /// Check if the argument is null.
        /// </summary>
        public static void ArgumentNullCheck(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(na, throwMessage);
        }

        /// <summary>
        /// Check if any of the arguments are null.
        /// </summary>
        public static void ArgumentNullCheck(object obj, object obj2)
        {
            if (obj == null || obj2 == null)
                throw new ArgumentNullException(na, throwMessage);
        }

        /// <summary>
        /// Check if any of the arguments are null.
        /// </summary>
        public static void ArgumentNullCheck(object obj, object obj2, object obj3)
        {
            if (obj == null || obj2 == null || obj3 == null)
                throw new ArgumentNullException(na, throwMessage);
        }

        /// <summary>
        /// Check if any of the arguments are null.
        /// </summary>
        public static void ArgumentNullCheck(object obj, object obj2, object obj3, object obj4)
        {
            if (obj == null || obj2 == null || obj3 == null || obj4 == null)
                throw new ArgumentNullException(na, throwMessage);
        }

        /// <summary>
        /// Check if any of the arguments are null.
        /// </summary>
        public static void ArgumentNullCheck(
            object obj, object obj2, object obj3, object obj4, object obj5)
        {
            if (obj == null || obj2 == null || obj3 == null || obj4 == null || obj5 == null)
                throw new ArgumentNullException(na, throwMessage);
        }

        /// <summary>
        /// Raise an action event if not null.
        /// </summary>
        public static void TryRaise(this Action action)
        {
            if (action != null) action();
        }

        /// <summary>
        /// Raise an action event if not null.
        /// </summary>
        public static void TryRaise<T>(this Action<T> action, T obj)
        {
            if (action != null) action(obj);
        }

        /// <summary>
        /// Raise an action event if not null.
        /// </summary>
        public static void TryRaise<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (action != null) action(arg1, arg2);
        }

        /// <summary>
        /// Raise an action event if not null.
        /// </summary>
        public static void TryRaise<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (action != null) action(arg1, arg2, arg3);
        }

        /// <summary>
        /// Raise an action event if not null.
        /// </summary>
        public static void TryRaise<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (action != null) action(arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Validate file name.
        /// </summary>
        public static void ValidateFileName(string fileName)
        {
            if (fileName.Length == 0)
                throw new ArgumentException("File name cannot have 0 length.");
        }

        private const string throwMessage = "One or more arguments are null that shouldn't be.";
        private const string na = "[Not Available]";
    }
}
