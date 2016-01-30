using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;

namespace Xi
{
    /// <summary>
    /// An object whose properties can be overlaid.
    /// TODO: try to contrive a way to overlay _structural_ data, like child objects (perhaps
    /// in a way similar to XAML).
    /// </summary>
    public abstract partial class Overlayable : Disposable
    {
        /// <summary>
        /// Create an Overlayable object.
        /// </summary>
        /// <param name="game">The game.</param>
        public Overlayable(XiGame game)
        {
            XiHelper.ArgumentNullCheck(game);
            this.game = game;
            this.overlayName = GetType().FullName;
            game.Overlayer.AddOverlayable(this);
        }

        /// <summary>
        /// The name of the applied overlay.
        /// TODO: expand on how this matches to the Overlay.xiol file.
        /// </summary>
        [IgnoreSerialization]
        public string OverlayName
        {
            get { return overlayName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (overlayName == value) return;
                overlayName = value;
                RefreshOverlaidProperties();
            }
        }

        /// <summary>
        /// The name of the applied overlay for serialization.
        /// Needed in addition to OverlayName to make sure that overlay is not refreshed when
        /// serializing in object properties.
        /// </summary>
        [Browsable(false)]
        public string OverlayNameSerialized
        {
            get { return overlayName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                overlayName = value;
            }
        }

        /// <summary>
        /// Is the object being overlaid?
        /// </summary>
        [Browsable(false)]
        public bool Overlaid { get { return overlayName.Length != 0; } }

        /// <summary>
        /// Refresh the overlaid property values.
        /// </summary>
        public void RefreshOverlaidProperties()
        {
            if (!Overlaid) return; // OPTIMIZATION
            foreach (PropertyInfo property in GetType().GetPropertiesFast())
                if (property.IsOverlayable())
                    ReadProperty(property);
            OnRefreshOverlaidPropertiesFinishing();
        }

        /// <summary>
        /// The game.
        /// </summary>
        protected XiGame Game { get { return game; } }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) game.Overlayer.RemoveOverlayable(this);
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handle telling is the given property is overlaid.
        /// </summary>
        protected bool IsOverlaid(PropertyInfo property)
        {
            return game.Overlayer.DoesPropertyExist(overlayName, property.Name);
        }

        /// <summary>
        /// Does the property have a default value as specified in the overlay file?
        /// Creates garbage, so only call when serializing out.
        /// </summary>
        protected bool IsDefault(PropertyInfo property)
        {
            string defaultTypeName = GetType().FullName + ".Default";
            XmlNode node = game.Overlayer.SelectNode(defaultTypeName, property.Name);
            if (node == null) return false;
            string propertyValueDefaultString = node.InnerText;
            object propertyValueDefault = property.ConvertToCompatibleValueFast(propertyValueDefaultString);
            object propertyValue = property.GetValue(this, null);
            if (propertyValue == null) return propertyValue == propertyValueDefault;
            return propertyValue.Equals(propertyValueDefault);
        }

        /// <summary>
        /// Handle the fact that the overlaid properties have just been refreshed from the overlay
        /// file.
        /// </summary>
        protected virtual void OnRefreshOverlaidPropertiesFinishing() { }

        /// <summary>
        /// Handle telling if the given property is hidden in the editor's property grid.
        /// </summary>
        protected virtual bool IsHidden(PropertyDescriptor property)
        {
            return game.Overlayer.DoesPropertyExist(overlayName, property.Name);
        }

        private void ReadProperty(PropertyInfo property)
        {
            XmlNode node = game.Overlayer.SelectNode(overlayName, property.Name);
            if (node == null) return;
            object value = property.ConvertToCompatibleValueFast(node.InnerText);
            property.SetValue(this, value, null);
        }

        private readonly XiGame game;
        private string overlayName = string.Empty;
    }
}
