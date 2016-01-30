using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A symbol that denotes an argument value should be a message context or event argument.
    /// </summary>
    public class ArgumentSymbol
    {
        /// <summary>
        /// The context argument symbol.
        /// </summary>
        public static readonly ArgumentSymbol Context = new ArgumentSymbol();
        
        /// <summary>
        /// The event argument symbol.
        /// </summary>
        public static readonly ArgumentSymbol Event = new ArgumentSymbol();

        /// <summary>
        /// Disallow instantiation outside of class.
        /// </summary>
        private ArgumentSymbol() { }
    }

    /// <summary>
    /// A message argument.
    /// OPTIMIZATION: Implemented as a struct for efficiency.
    /// </summary>
    public struct Argument
    {
        /// <summary>
        /// The argument type.
        /// </summary>
        public Type Type { get { return type; } }

        /// <summary>
        /// The argument value.
        /// </summary>
        public object Value { get { return value; } }

        /// <summary>
        /// Populate the argument with values parsed from a string.
        /// </summary>
        public void Populate(string argumentString)
        {
            XiHelper.ArgumentNullCheck(argumentString);
            if (IsContextArgument(argumentString)) SetUpAsContextArgument();
            else if (IsEventArgument(argumentString)) SetUpAsEventArgument();
            else SetUpAsValue(argumentString);
        }

        private void SetUpAsContextArgument()
        {
            type = typeof(object);
            value = ArgumentSymbol.Context;
        }

        private void SetUpAsEventArgument()
        {
            type = typeof(object);
            value = ArgumentSymbol.Event;
        }

        private void SetUpAsValue(string argumentString)
        {
            string[] argumentParts = PartitionArgumentString(argumentString);
            string typeName = ParseTypeName(argumentParts);
            SetUpType(typeName);
            SetUpValue(argumentParts, typeName);
        }

        private static string[] PartitionArgumentString(string argumentString)
        {
            string[] parts = argumentString.Split('=');
            ValidateArgumentParts(argumentString, parts);
            return parts;
        }

        private static string ParseTypeName(string[] argumentParts)
        {
            string typeName = argumentParts[0];
            ValidateTypeName(typeName);
            return typeName;
        }

        private void SetUpType(string typeName)
        {
            type = LookUpType(typeName);
            ValidateType(type);
        }

        private void SetUpValue(string[] argumentParts, string typeName)
        {
            string valueString = argumentParts[1];
            value = valueString.ParseValueType(type);
        }

        private static Type LookUpType(string typeName)
        {
            switch (typeName)
            {
                case "b":
                case "bool": return typeof(bool);
                case "y":
                case "byte": return typeof(byte);
                case "c":
                case "char": return typeof(char);
                case "ui":
                case "uint": return typeof(uint);
                case "i":
                case "int": return typeof(int);
                case "ul":
                case "ulong": return typeof(ulong);
                case "l":
                case "long": return typeof(long);
                case "f":
                case "float": return typeof(float);
                case "d":
                case "double": return typeof(double);
                case "v2":
                case "Vector2": return typeof(Vector2);
                case "v3":
                case "Vector3": return typeof(Vector3);
                case "v4":
                case "Vector4": return typeof(Vector4);
                case "q":
                case "Quaternion": return typeof(Quaternion);
                case "m":
                case "Matrix": return typeof(Matrix);
                case "p":
                case "Point": return typeof(Point);
                case "color":
                case "Color": return typeof(Color);
                case "s":
                case "string": return typeof(string);
                default: throw new ArgumentException("Invalid argument type '" + typeName + "'.");
            }
        }

        private static bool IsContextArgument(string argumentString)
        {
            return
                argumentString == "c" ||
                argumentString == "Context";
        }

        private static bool IsEventArgument(string argumentString)
        {
            return
                argumentString == "a" ||
                argumentString == "Argument";
        }

        private static void ValidateArgumentParts(string argumentString, string[] parts)
        {
            if (parts.Length != 2)
                throw new ArgumentException("Invalid argument string '" + argumentString + "'.");
        }

        private static void ValidateTypeName(string typeName)
        {
            if (typeName.Length == 0)
                throw new ArgumentException("Invalid argument type '" + typeName + "'.");
        }

        private static void ValidateTypeConverter(string typeName, TypeConverter typeConverter)
        {
            if (typeConverter == null)
                throw new ArgumentException("No type converter found for type '" + typeName + "'.");
        }

        private static void ValidateType(Type type)
        {
            if (type == null)
                throw new ArgumentException("Could not find type '" + type + "'.");
        }

        private Type type;
        private object value;
    }
}
