using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace Xi
{
    /// <summary>
    /// Marks a property to be ignored by the serialization process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnoreSerializationAttribute : Attribute { }

    /// <summary>
    /// An object that can be serialized.
    /// TODO: implement a Type struct that includes type name AND a version so the type tag becomes <Type>Xi.Blah, 1</Type>
    /// </summary>
    public abstract class Serializable : Overlayable
    {
        /// <summary>
        /// Create a Serializable object.
        /// </summary>
        /// <param name="game">The game.</param>
        public Serializable(XiGame game) : base(game) { }

        /// <summary>
        /// How the serializable object is intended to persist.
        /// </summary>
        [Browsable(false), IgnoreSerialization]
        public Persistence Persistence
        {
            get { return persistence; }
            set { persistence = value; }
        }

        /// <summary>
        /// Read in a serializable object.
        /// </summary>
        /// <param name="node">The XML node to read from.</param>
        public void Read(XmlNode node)
        {
            XiHelper.ArgumentNullCheck(node);
            ReadProperties(node);
            ReadAdditional(node);
            OnReadFinishing();
        }

        /// <summary>
        /// Read in the serializable object's properties to a file.
        /// </summary>
        /// <param name="fileName">The name of the file to read from.</param>
        public void ReadProperties(string fileName)
        {
            XiHelper.ArgumentNullCheck(fileName);
            XmlDocument document = Game.XmlDocumentCache.GetXmlDocument(fileName);
            XmlNode rootNode = document.SelectSingleNode("Root");
            ReadProperties(rootNode);
            document.Save(fileName);
        }

        /// <summary>
        /// Read in only the properties of a serializable object.
        /// </summary>
        /// <param name="node">The XML node to read from.</param>
        public void ReadProperties(XmlNode node)
        {
            foreach (PropertyInfo property in GetType().GetPropertiesFast())
                if (ShouldReadProperty(property))
                    TryReadProperty(node, property);
            RefreshOverlaidProperties();
        }

        /// <summary>
        /// Write out the serializable object as type T to a file.
        /// </summary>
        /// <param name="fileName">The name of the file to write to.</param>
        public void Write(string fileName)
        {
            XiHelper.ArgumentNullCheck(fileName);
            XiHelper.ValidateFileName(fileName);
            XmlDocument document = new XmlDocument();
            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "utf-8", null);
            document.AppendChild(declaration);
            XmlNode rootNode = document.CreateElement("Root");
            document.AppendChild(rootNode);
            XmlNode actorGroupNode = document.CreateElement("Serializable");
            rootNode.AppendChild(actorGroupNode);
            Write(document, actorGroupNode);
            document.Save(fileName);
        }

        /// <summary>
        /// Write out a serializable object.
        /// </summary>
        /// <param name="document">The XML document that's being written to.</param>
        /// <param name="node">The XML node to write to.</param>
        public void Write(XmlDocument document, XmlNode node)
        {
            XiHelper.ArgumentNullCheck(document, node);
            WriteType(document, node);
            WriteProperties(document, node, true);
            WriteAdditional(document, node);
        }

        /// <summary>
        /// Write out the serializable object's properties to a file.
        /// </summary>
        /// <param name="fileName">The name of the file to write to.</param>
        /// <param name="cullRedundantProperties">Cull the redundant properties from writing.</param>
        public void WriteProperties(string fileName, bool cullRedundantProperties)
        {
            XiHelper.ArgumentNullCheck(fileName);
            XiHelper.ValidateFileName(fileName);
            XmlDocument document = new XmlDocument();
            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "utf-8", null);
            document.AppendChild(declaration);
            XmlNode rootNode = document.CreateElement("Root");
            document.AppendChild(rootNode);
            WriteProperties(document, rootNode, cullRedundantProperties);
            document.Save(fileName);
        }

        /// <summary>
        /// Write out only the properties of a serializable object.
        /// </summary>
        /// <param name="document">The XML document that's being written to.</param>
        /// <param name="node">The XML node to write to.</param>
        /// <param name="cullRedundantProperties">Cull the redundant properties from writing.</param>
        public void WriteProperties(XmlDocument document, XmlNode node, bool cullRedundantProperties)
        {
            foreach (PropertyInfo property in GetType().GetPropertiesFast())
                if (ShouldWriteProperty(property, cullRedundantProperties))
                    WriteProperty(document, node, property);
        }

        /// <summary>
        /// Notify the serializable that its serializable parent just finished reading.
        /// </summary>
        public void NotifySerializableParentReadFinishing()
        {
            OnSerializableParentReadFinishing();
        }

        /// <summary>
        /// Perform additional read operations, such as reading in collections of serializable
        /// children.
        /// </summary>
        protected virtual void ReadAdditional(XmlNode node) { }

        /// <summary>
        /// Perform additional write operations, such as writing out collections of serializable
        /// children.
        /// </summary>
        protected virtual void WriteAdditional(XmlDocument document, XmlNode node) { }

        /// <summary>
        /// Handle the fact that the serializable's serializable parent just finished reading.
        /// </summary>
        protected virtual void OnSerializableParentReadFinishing() { }

        /// <summary>
        /// Handle the fact that the serializable just finished reading.
        /// </summary>
        protected virtual void OnReadFinishing() { }

        private void TryReadProperty(XmlNode node, PropertyInfo property)
        {
            XmlNode propertyNode = node.SelectSingleNode(property.Name);
            if (propertyNode == null) return;
            if (!TryReadPropertyOfCompatibleType(propertyNode, property))
                TryReadPropertyOfSerializableType(propertyNode, property);
        }

        private bool TryReadPropertyOfCompatibleType(XmlNode propertyNode, PropertyInfo property)
        {   
            object propertyValue;
            if (!property.TryConvertToCompatibleValueFast(propertyNode.InnerText, out propertyValue)) return false;
            property.SetValue(this, propertyValue, null);
            return true;
        }

        private bool TryReadPropertyOfSerializableType(XmlNode propertyNode, PropertyInfo property)
        {
            Type propertyType = property.PropertyType;
            if (!propertyType.IsSubclassOf(serializableType)) return false;
            ConstructorInfo constructor = propertyType.GetConstructor(constructorTypes);
            Serializable serializable = null;
            try
            {
                serializable = XiHelper.Cast<Serializable>(constructor.Invoke(new[] { Game }));
                serializable.Read(propertyNode);
                property.SetValue(this, serializable, null);
                return true;
            }
            catch (Exception e)
            {
                Trace.Fail("Serializable property read error.", e.ToString());
                if (serializable != null) serializable.Dispose();
                return false;
            }
        }

        private static bool ShouldReadProperty(PropertyInfo property)
        {
            return property.IsSerializable();
        }

        private void WriteType(XmlDocument document, XmlNode node)
        {
            XmlNode typeNode = document.CreateElement("Type");
            XmlText typeTextNode = document.CreateTextNode(GetType().FullName);
            typeNode.AppendChild(typeTextNode);
            node.AppendChild(typeNode);
        }

        private void WriteProperty(XmlDocument document, XmlNode node, PropertyInfo property)
        {
            if (!TryWritePropertyOfConvertibleType(document, node, property))
                TryWritePropertyOfSerializableType(document, node, property);
        }

        private bool TryWritePropertyOfConvertibleType(XmlDocument document, XmlNode node, PropertyInfo property)
        {
            object propertyValue = property.GetValue(this, null);
            string propertyValueString;
            if (!property.TryConvertCompatibleToString(propertyValue, out propertyValueString)) return false;
            WriteProperty(document, node, property, propertyValueString);
            return true;
        }

        private bool TryWritePropertyOfSerializableType(XmlDocument document, XmlNode node, PropertyInfo property)
        {
            Serializable serializable = property.GetValue(this, null) as Serializable;
            if (serializable == null || serializable.Persistence != Persistence.Persistent) return false;
            WriteProperty(document, node, property, serializable);
            return true;
        }

        private static void WriteProperty(XmlDocument document, XmlNode node, PropertyInfo property, string propertyValueString)
        {
            XmlNode propertyNode = document.CreateElement(property.Name);
            XmlText propertyTextNode = document.CreateTextNode(propertyValueString);
            propertyNode.AppendChild(propertyTextNode);
            node.AppendChild(propertyNode);
        }

        private static void WriteProperty(XmlDocument document, XmlNode node, PropertyInfo property, Serializable serializable)
        {
            XmlNode propertyNode = document.CreateElement(property.Name);
            serializable.Write(document, propertyNode);
            node.AppendChild(propertyNode);
        }

        private bool ShouldWriteProperty(PropertyInfo property, bool cullRedundant)
        {
            return
                property.IsSerializable() &&
                (
                    !cullRedundant ||
                    !IsOverlaid(property) &&
                    !IsDefault(property)
                );
        }
        
        private static readonly Type[] constructorTypes = { typeof(XiGame) };
        private static readonly Type serializableType = typeof(Serializable);
        private Persistence persistence;
    }
}
