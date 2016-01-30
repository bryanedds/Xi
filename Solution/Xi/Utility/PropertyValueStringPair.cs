using System;

namespace Xi
{
    /// <summary>
    /// Assists in memoization of boxed property values.
    /// </summary>
    public struct PropertyValueStringPair : IEquatable<PropertyValueStringPair>
    {
        /// <summary>
        /// Create a PropertyValueStringPair.
        /// </summary>
        /// <param name="propertyType">The property type.</param>
        /// <param name="valueString">The property value in string form.</param>
        public PropertyValueStringPair(Type propertyType, string valueString)
        {
            PropertyType = propertyType;
            ValueString = valueString;
        }

        /// <summary>
        /// Evaluates two property value strings for equality.
        /// </summary>
        public bool Equals(PropertyValueStringPair other)
        {
            return
                other.PropertyType == PropertyType &&
                other.ValueString == ValueString;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            try
            {
                PropertyValueStringPair other = (PropertyValueStringPair)obj;
                return Equals(other);
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return
                PropertyType.GetHashCode() ^
                ValueString.GetHashCode() * 31; // TODO: investigate if this is a good prime number
        }

        public readonly Type PropertyType;
        public readonly string ValueString;
    }
}
