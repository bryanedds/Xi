using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using PropertyInfoDictionaries = System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.Dictionary<string, System.Reflection.PropertyInfo>>;
using PropertyInfoDictionary = System.Collections.Generic.Dictionary<string, System.Reflection.PropertyInfo>;
using CustomAttributesDictionary = System.Collections.Generic.Dictionary<System.Reflection.ICustomAttributeProvider, object[]>;
using PropertyValueDictionary = System.Collections.Generic.Dictionary<Xi.PropertyValueStringPair, object>;

namespace Xi
{
    /// <summary>
    /// Provides helper methods for operations involving reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// An empty type array.
        /// Needed on Xbox 360 for its limited CF.
        /// </summary>
        public static Type[] EmptyTypes { get { return emptyTypes; } }

        /// <summary>
        /// Is the object definition a file definition?
        /// </summary>
        public static bool IsFileDefinition(string definition)
        {
            return definition.StartsWith("FileName=");
        }

        /// <summary>
        /// Convert an object definition to an file definition array.
        /// </summary>
        public static string[] ToFileDefinitionArray(string definition)
        {
            return definition.Split('=');
        }

        /// <summary>
        /// Convert an object definition to a string definition array.
        /// </summary>
        public static string[] ToStringDefinitionArray(string definition)
        {
            XiHelper.ArgumentNullCheck(definition);
            return definition.Split(':');
        }

        /// <summary>
        /// Create a default-constructible object from a given constructor.
        /// </summary>
        public static object CreateObject(ConstructorInfo constructor)
        {
            XiHelper.ArgumentNullCheck(constructor);
            try
            {
                return constructor.Invoke(null);
            }
            catch (Exception e)
            {
                Trace.Fail("Object creation error.", e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Cast an instance to type T, and dispose on cast failure.
        /// </summary>
        public static T CastInstanceToTAndDisposeOnFailure<T>(string instanceTypeName, object instance) where T : class
        {
            XiHelper.ArgumentNullCheck(instanceTypeName, instance);
            T instanceT = instance as T;
            if (instanceT != null) return instanceT;
            IDisposable instanceDisposable = instance as IDisposable;
            if (instanceDisposable != null) instanceDisposable.Dispose();
            throw new ArgumentException("Type '" + instanceTypeName + "' is not a type of '" + typeof(T).FullName + "'.");
        }

        /// <summary>
        /// Configure a disposable object created with the given definition.
        /// If supported, disposes object on failure.
        /// </summary>
        public static void ConfigureCreatedObject(string definition, string[] definitionArray, object instance)
        {
            XiHelper.ArgumentNullCheck(definition, definitionArray);
            ValidateStringDefinitionArray(definition, definitionArray);
            try
            {
                if (definitionArray.Length == 2)
                    ConfigureProperties(instance, definitionArray[1]);
            }
            catch
            {
                IDisposable disposable = instance as IDisposable;
                if (disposable != null) disposable.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Configure the properties of an object with key-value-pair strings.
        /// </summary>
        public static void ConfigureProperties(object instance, string kvpsString)
        {
            XiHelper.ArgumentNullCheck(instance, kvpsString);
            // TODO: make sure '>' won't screw up the parser when used in an XML file
            string[] kvpsArray = kvpsString.Split('>');
            foreach (string kvpString in kvpsArray) ConfigureProperty(instance, kvpString);
        }

        /// <summary>
        /// Is a property serializable?
        /// </summary>
        public static bool IsSerializable(this PropertyInfo property)
        {
            return
                property.CanRead &&
                property.CanWrite &&
                !property.HasCustomAttributeFast(typeof(IgnoreSerializationAttribute));
        }

        /// <summary>
        /// Is a property overlayable?
        /// </summary>
        public static bool IsOverlayable(this PropertyInfo property)
        {
            return property.CanRead && property.CanWrite;
        }

        /// <summary>
        /// Is a property automatically convertible?
        /// </summary>
        public static bool IsAutomaticallyConvertible(this PropertyInfo property)
        {
            Type propertyType = property.PropertyType;
            return
                propertyType.IsValueType ||
                propertyType == typeof(string);
        }

        /// <summary>
        /// Configure a property of an object with a property key-value-pair string.
        /// </summary>
        public static void ConfigureProperty(object instance, string kvpString)
        {
            XiHelper.ArgumentNullCheck(instance, kvpString);
            string[] kvp = kvpString.Split('=');
            if (kvp.Length != 2) throw new ArgumentException("Invalid property key-value-pair '" + kvpString + "'.");
            string key = kvp[0];
            string valueString = kvp[1];
            Type type = instance.GetType();
            PropertyInfo property = type.GetWritablePropertyFast(key);
            property.SetValue(instance, property.ConvertToCompatibleValueFast(valueString), null);
        }

        /// <summary>
        /// Convert a string to an object compatible with a property.
        /// Optimized to avoid generating garbage.
        /// </summary>
        public static bool TryConvertToCompatibleValueFast(this PropertyInfo property, string propertyValueString, out object propertyValue)
        {
            XiHelper.ArgumentNullCheck(property, propertyValueString);
            propertyValue = null;
            if (!property.IsAutomaticallyConvertible()) return false;
            propertyValue = property.ConvertToCompatibleValueFast(propertyValueString);
            return true;
        }

        /// <summary>
        /// Convert a string to an object compatible with a property.
        /// Optimized to avoid generating garbage.
        /// </summary>
        public static object ConvertToCompatibleValueFast(this PropertyInfo property, string propertyValueString)
        {
            XiHelper.ArgumentNullCheck(property, propertyValueString);
            return property.GetPropertyValueFast(propertyValueString);
        }

        /// <summary>
        /// Convert an object compatible with a property to a string.
        /// </summary>
        public static string ConvertCompatibleToString(this PropertyInfo property, object propertyValue)
        {
            XiHelper.ArgumentNullCheck(property, propertyValue);
            string propertyValueString;
            if (property.TryConvertCompatibleToString(propertyValue, out propertyValueString)) return propertyValueString;
            throw new ArgumentException("Could convert property '" + propertyValue.ToString() + "' to string.");
        }

        /// <summary>
        /// Try to convert an object compatible with a property to a string.
        /// </summary>
        public static bool TryConvertCompatibleToString(this PropertyInfo property, object propertyValue, out string propertyValueString)
        {
            XiHelper.ArgumentNullCheck(property);
            propertyValueString =
                property.PropertyType.IsValueType ?
                propertyValue.StringizeValueType(property.PropertyType) :
                propertyValue as string;
            return propertyValueString != null;
        }

        /// <summary>
        /// Get the property value from a string in an optimized manner.
        /// May return null.
        /// </summary>
        public static object GetPropertyValueFast(this PropertyInfo property, string propertyValueString)
        {
            // OPTIMIZATION: memoized to avoid generating garbage
            object propertyValue;
            Type propertyType = property.PropertyType;
            PropertyValueStringPair pvsp = new PropertyValueStringPair(propertyType, propertyValueString);
            if (!propertyValueDictionary.TryGetValue(pvsp, out propertyValue))
                propertyValueDictionary.Add(pvsp, propertyValue = propertyValueString.ParseValueType(propertyType));
            return propertyValue;
        }

        /// <summary>
        /// Get the property info of a type's given property in an optimized manner.
        /// </summary>
        public static PropertyInfo GetPropertyFast(this Type type, string name)
        {
            XiHelper.ArgumentNullCheck(type, name);
            PropertyInfo property;
            type.GetPropertyDictionaryFast().TryGetValue(name, out property);
            if (property == null) throw new ArgumentException("Invalid property '" + type.FullName + "." + name + "'.");
            return property;
        }

        /// <summary>
        /// Get a dictionary containing all the property info of a type, including inherited types.
        /// OPTIMIZATION: The dictionary is statically cached.
        /// </summary>
        public static PropertyInfoDictionary GetPropertyDictionaryFast(this Type type)
        {
            XiHelper.ArgumentNullCheck(type);
            PropertyInfoDictionary dictionary;
            propertyInfoDictionaries.TryGetValue(type, out dictionary);
            return dictionary ?? type.CreatePropertyInfoDictionary();
        }

        /// <summary>
        /// Get a writable property with the given name in an optimized manner.
        /// </summary>
        public static PropertyInfo GetWritablePropertyFast(this Type type, string name)
        {
            XiHelper.ArgumentNullCheck(type, name);
            PropertyInfo property = type.GetPropertyFast(name);
            if (!property.CanWrite) throw new ArgumentException("Property '" + type.FullName + "." + name + "' is read-only.");
            return property;
        }

        /// <summary>
        /// Get all of a type's properties in an optimized manner.
        /// </summary>
        public static Dictionary<string, PropertyInfo>.ValueCollection GetPropertiesFast(this Type type)
        {
            XiHelper.ArgumentNullCheck(type);
            return type.GetPropertyDictionaryFast().Values;
        }

        /// <summary>
        /// Has the given type of custom attribute?
        /// Uses inheritance.
        /// Optimized to avoid generating garbage.
        /// </summary>
        public static bool HasCustomAttributeFast(this ICustomAttributeProvider cap, Type attributeType)
        {
            XiHelper.ArgumentNullCheck(cap, attributeType);
            return cap.TryGetFirstCustomAttributeFast(attributeType) != null;
        }

        /// <summary>
        /// Try to get the first given type of a custom attribute.
        /// Uses inheritance.
        /// Optimized to avoid generating garbage.
        /// </summary>
        public static object TryGetFirstCustomAttributeFast(this ICustomAttributeProvider cap, Type attributeType)
        {
            XiHelper.ArgumentNullCheck(cap, attributeType);
            foreach (object customAttribute in cap.GetCustomAttributesFast())
                if (customAttribute.GetType() == attributeType)
                    return customAttribute;
            return null;
        }

        /// <summary>
        /// Get the custom attributes of a provider.
        /// Uses inheritance.
        /// Optimized to avoid generating garbage.
        /// </summary>
        public static object[] GetCustomAttributesFast(this ICustomAttributeProvider cap)
        {
            XiHelper.ArgumentNullCheck(cap);
            object[] customAttributes;
            if (!customAttributesDictionary.TryGetValue(cap, out customAttributes))
                customAttributesDictionary.Add(cap, customAttributes = cap.GetCustomAttributes(true));
            return customAttributes;
        }

        /// <summary>
        /// Validate a file definition.
        /// </summary>
        public static void ValidateFileDefinitionArray(string definition, string[] fileDefinitionArray)
        {
            if (fileDefinitionArray.Length != 2)
                throw new ArgumentException("Invalid file name definition '" + definition + "'.");
        }

        /// <summary>
        /// Validate an object definition.
        /// </summary>
        public static void ValidateStringDefinitionArray(string definition, string[] definitionArray)
        {
            XiHelper.ArgumentNullCheck(definition, definitionArray);
            if (definitionArray.Length == 0 || definitionArray.Length > 2)
                throw new ArgumentException("Invalid object definition '" + definition + "'.");
        }

        private static PropertyInfoDictionary CreatePropertyInfoDictionary(this Type type)
        {
            IEnumerable<PropertyInfo> properties = type.GetProperties();
            PropertyInfoDictionary dictionary = properties.ToDictionary(x => x.Name);
            propertyInfoDictionaries[type] = dictionary;
            return dictionary;
        }

        private static readonly PropertyInfoDictionaries propertyInfoDictionaries = new PropertyInfoDictionaries();
        private static readonly CustomAttributesDictionary customAttributesDictionary = new CustomAttributesDictionary();
        private static readonly PropertyValueDictionary propertyValueDictionary = new Dictionary<PropertyValueStringPair, object>();
        private static readonly Type[] emptyTypes = new Type[0];
    }
}
