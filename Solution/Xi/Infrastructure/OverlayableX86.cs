using System;
using System.ComponentModel;

namespace Xi
{
    public abstract partial class Overlayable : ICustomTypeDescriptor
    {
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            XiHelper.ArgumentNullCheck(attributes);
            return new PropertyDescriptorCollection(TransformProperties(attributes));
        }

        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }
        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }
        public string GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }
        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }
        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
        public PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
        public Object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }
        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
        public PropertyDescriptorCollection GetProperties() { return TypeDescriptor.GetProperties(this, true); }
        public Object GetPropertyOwner(PropertyDescriptor propertyDescriptor) { return this; }

        private PropertyDescriptor[] TransformProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection visibleProperties = TypeDescriptor.GetProperties(this, attributes, true);
            PropertyDescriptor[] transformedProperties = new PropertyDescriptor[visibleProperties.Count];
            for (int i = 0; i < visibleProperties.Count; ++i) TransformProperty(visibleProperties[i], transformedProperties, i);
            return transformedProperties;
        }

        private void TransformProperty(
            PropertyDescriptor visibleProperty,
            PropertyDescriptor[] transformedProperties,
            int transformedPropertyIndex)
        {
            transformedProperties[transformedPropertyIndex] =
                IsHidden(visibleProperty) ?
                CreateHiddenProperty(visibleProperty) :
                visibleProperty;
        }

        private PropertyDescriptor CreateHiddenProperty(PropertyDescriptor visibleProperty)
        {
            return TypeDescriptor.CreateProperty(GetType(), visibleProperty, BrowsableAttribute.No);
        }
    }
}
