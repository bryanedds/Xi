using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Helps with value type operations such as conversion and parsing.
    /// Would not be necessary if type converter classes were available on the Xbox.
    /// </summary>
    public static class ValueTypeHelper
    {
        /// <summary>
        /// Parse out a value type.
        /// </summary>
        public static object ParseValueType(this string valueString, Type valueType)
        {
            bool valueStringValid = valueString.Length != 0;
            Type underlyingType = Nullable.GetUnderlyingType(valueType) ?? valueType;
            if (underlyingType.IsEnum) return valueStringValid ? Enum.Parse(underlyingType, valueString, false) : null;
            switch (underlyingType.Name)
            {
                case "Boolean": return valueStringValid ? (object)Boolean.Parse(valueString) : null;
                case "Byte": return valueStringValid ? (object)Byte.Parse(valueString) : null;
                case "Char": return valueStringValid ? (object)(Char)Byte.Parse(valueString) : null;
                case "UInt16": return valueStringValid ? (object)UInt16.Parse(valueString) : null;
                case "Int16": return valueStringValid ? (object)Int16.Parse(valueString) : null;
                case "UInt32": return valueStringValid ? (object)UInt32.Parse(valueString) : null;
                case "Int32": return valueStringValid ? (object)Int32.Parse(valueString) : null;
                case "UInt64": return valueStringValid ? (object)UInt64.Parse(valueString) : null;
                case "Int64": return valueStringValid ? (object)Int64.Parse(valueString) : null;
                case "Single": return valueStringValid ? (object)valueString.ParseFloat() : null;
                case "Double": return valueStringValid ? (object)valueString.ParseDouble() : null;
                case "Vector2": return valueStringValid ? (object)valueString.ParseVector2() : null;
                case "Vector3": return valueStringValid ? (object)valueString.ParseVector3() : null;
                case "Vector4": return valueStringValid ? (object)valueString.ParseVector4() : null;
                case "Quaternion": return valueStringValid ? (object)valueString.ParseQuaternion() : null;
                case "Matrix": return valueStringValid ? (object)valueString.ParseMatrix() : null;
                case "Color": return valueStringValid ? (object)valueString.ParseColor() : null;
                case "Point": return valueStringValid ? (object)valueString.ParsePoint() : null;
                case "String": return valueString;
                default: throw new ArgumentException("Does not know how to parse value type '" + underlyingType.FullName + "'.");
            }
        }

        /// <summary>
        /// Parse out a float with an invariant culture.
        /// </summary>
        public static float ParseFloat(this string valueString)
        {
            return float.Parse(valueString, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parse out a double with an invariant culture.
        /// </summary>
        public static double ParseDouble(this string valueString)
        {
            return Double.Parse(valueString, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parse out an XNA vector 2.
        /// </summary>
        public static Vector2 ParseVector2(this string valueString)
        {
            string[] fs = valueString.SplitVector();
            return new Vector2(fs[0].ParseFloat(), fs[1].ParseFloat());
        }

        /// <summary>
        /// Parse out an XNA vector 3.
        /// </summary>
        public static Vector3 ParseVector3(this string valueString)
        {
            string[] fs = valueString.SplitVector();
            return new Vector3(fs[0].ParseFloat(), fs[1].ParseFloat(), fs[2].ParseFloat());
        }

        /// <summary>
        /// Parse out an XNA vector 4.
        /// </summary>
        public static Vector4 ParseVector4(this string valueString)
        {
            string[] fs = valueString.SplitVector();
            return new Vector4(fs[0].ParseFloat(), fs[1].ParseFloat(), fs[2].ParseFloat(), fs[3].ParseFloat());
        }

        /// <summary>
        /// Parse out an XNA quaternion.
        /// </summary>
        public static Quaternion ParseQuaternion(this string valueString)
        {
            string[] fs = valueString.SplitVector();
            return new Quaternion(fs[0].ParseFloat(), fs[1].ParseFloat(), fs[2].ParseFloat(), fs[3].ParseFloat());
        }

        /// <summary>
        /// Parse out an XNA matrix.
        /// </summary>
        public static Matrix ParseMatrix(this string valueString)
        {
            string[] fs = valueString.SplitVector();
            return new Matrix(
                fs[00].ParseFloat(), fs[01].ParseFloat(), fs[02].ParseFloat(), fs[03].ParseFloat(),
                fs[04].ParseFloat(), fs[05].ParseFloat(), fs[06].ParseFloat(), fs[07].ParseFloat(),
                fs[08].ParseFloat(), fs[09].ParseFloat(), fs[10].ParseFloat(), fs[11].ParseFloat(),
                fs[12].ParseFloat(), fs[13].ParseFloat(), fs[14].ParseFloat(), fs[15].ParseFloat());
        }

        /// <summary>
        /// Parse out an XNA color.
        /// </summary>
        public static Color ParseColor(this string valueString)
        {
            string[] fs = valueString.SplitVector();
            return new Color(Byte.Parse(fs[0]), Byte.Parse(fs[1]), Byte.Parse(fs[2]), Byte.Parse(fs[3]));
        }

        /// <summary>
        /// Parse out an XNA point.
        /// </summary>
        public static Point ParsePoint(this string valueString)
        {
            string[] fs = valueString.SplitVector();
            return new Point(int.Parse(fs[0]), int.Parse(fs[1]));
        }

        /// <summary>
        /// Stringize a value type. Has special handling for value types with non-standard ToString
        /// methods.
        /// </summary>
        public static string StringizeValueType(this object value, Type valueType)
        {
            Type underlyingType = Nullable.GetUnderlyingType(valueType) ?? valueType;
            if (underlyingType.IsEnum) return value != null ? value.ToString() : string.Empty;
            switch (underlyingType.Name)
            {
                case "Boolean":
                case "Byte":
                case "Char":
                case "UInt16":
                case "Int16":
                case "UInt32":
                case "Int32":
                case "UInt64":
                case "Int64": return value.ToString();
                case "Single": return StringizeFloat((float)value);
                case "Double": return StringizeDouble((double)value);
                case "Vector2": return StringizeVector2((Vector2)value);
                case "Vector3": return StringizeVector3((Vector3)value);
                case "Vector4": return StringizeVector4((Vector4)value);
                case "Quaternion": return StringizeQuaternion((Quaternion)value);
                case "Matrix": return StringizeMatrix((Matrix)value);
                case "Color": return StringizeColor((Color)value);
                case "Point": return StringizePoint((Point)value);
                case "String": return (string)value;
                default: throw new ArgumentException("Does not know how to stringize value type '" + underlyingType.FullName + "'.");
            }
        }

        /// <summary>
        /// Stringize a float with an Invariant culture.
        /// </summary>
        public static string StringizeFloat(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Stringize a double with an Invariant culture.
        /// </summary>
        public static string StringizeDouble(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Stringize an XNA vector 2.
        /// </summary>
        public static string StringizeVector2(Vector2 value)
        {
            return
                value.X + ", " +
                value.Y;
        }

        /// <summary>
        /// Stringize an XNA vector 3.
        /// </summary>
        public static string StringizeVector3(Vector3 value)
        {
            return
                value.X + ", " +
                value.Y + ", " +
                value.Z;
        }

        /// <summary>
        /// Stringize an XNA vector 4.
        /// </summary>
        public static string StringizeVector4(Vector4 value)
        {
            return
                value.X + ", " +
                value.Y + ", " +
                value.Z + ", " +
                value.W;
        }

        /// <summary>
        /// Stringize an XNA quaternion.
        /// </summary>
        public static string StringizeQuaternion(Quaternion value)
        {
            return
                value.X + ", " +
                value.Y + ", " +
                value.Z + ", " +
                value.W;
        }

        /// <summary>
        /// Stringize an XNA matrix.
        /// TODO: consider using a StringBuilder here.
        /// </summary>
        public static string StringizeMatrix(Matrix value)
        {
            return
                value.M11 + ", " + value.M12 + ", " + value.M13 + ", " + value.M14 + ", " +
                value.M21 + ", " + value.M22 + ", " + value.M23 + ", " + value.M24 + ", " +
                value.M31 + ", " + value.M32 + ", " + value.M33 + ", " + value.M34 + ", " +
                value.M41 + ", " + value.M42 + ", " + value.M43 + ", " + value.M44;
        }

        /// <summary>
        /// Stringize an XNA color.
        /// </summary>
        public static string StringizeColor(Color value)
        {
            return
                value.R + ", " +
                value.G + ", " +
                value.B + ", " +
                value.A;
        }

        /// <summary>
        /// Stringize an XNA point.
        /// </summary>
        public static string StringizePoint(Point value)
        {
            return
                value.X + ", " +
                value.Y;
        }

        /// <summary>
        /// Parse out a vector of comma-delimited numbers with optional white space.
        /// </summary>
        public static string[] SplitVector(this string vectorString)
        {
            return vectorString.Replace(" ", "").Split(',');
        }
    }
}
