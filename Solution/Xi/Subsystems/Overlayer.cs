using System.Collections.Generic;
using System.Xml;

namespace Xi
{
    /// <summary>
    /// Overlays registered objects.
    /// </summary>
    public class Overlayer
    {
        /// <summary>
        /// Create an Overlayer.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="overlayFileName">The initial overlay file nane.</param>
        public Overlayer(XiGame game, string overlayFileName)
        {
            XiHelper.ArgumentNullCheck(game, overlayFileName);
            this.game = game;
            this.overlayFileName = overlayFileName;
            SetUpDocument();
        }

        /// <summary>
        /// Refresh the overlay data from the file.
        /// </summary>
        public void RefreshOverlayData()
        {
            game.RefreshDocumentFile(overlayFileName);
            SetUpDocument();
            RefreshOverlayables();
        }

        /// <summary>
        /// Does a given overlay property exist?
        /// </summary>
        public bool DoesPropertyExist(string overlayName, string propertyName)
        {
            // TODO: pray that no garbage is created here (or better, profile)
            XiHelper.ArgumentNullCheck(overlayName, propertyName);
            return SelectNode(overlayName, propertyName) != null;
        }

        /// <summary>
        /// Select an overlay property node.
        /// NOTE: do NOT mutate the XML node - it is intended to be immutable.
        /// </summary>
        public XmlNode SelectNode(string overlayName, string propertyName)
        {
            // TODO: pray that no garbage is created here (or better, profile)
            XiHelper.ArgumentNullCheck(overlayName, propertyName);
            if (overlayName.Length == 0 || propertyName.Length == 0) return null;
            XmlNode branch = root.SelectSingleNode(overlayName);
            if (branch == null) return null;
            XmlNode leaf = branch.SelectSingleNode(propertyName);
            if (leaf != null) return leaf;
            XmlNode parentNames = branch.SelectSingleNode("Inherits");
            if (parentNames == null) return null;
            // TODO: make sure '>' won't screw up the parser when used in an XML file
            foreach (string parentName in parentNames.InnerText.Split('>'))
            {
                XmlNode parent = SelectNode(parentName, propertyName);
                if (parent != null) return parent;
            }
            return null;
        }

        internal bool AddOverlayable(Overlayable overlayable)
        {
            XiHelper.ArgumentNullCheck(overlayable);
            return overlayablePSet.TryAddValue(overlayable);
        }

        internal bool RemoveOverlayable(Overlayable overlayable)
        {
            XiHelper.ArgumentNullCheck(overlayable);
            return overlayablePSet.Remove(overlayable);
        }

        private void SetUpDocument()
        {
            document = game.XmlDocumentCache.GetXmlDocument(overlayFileName);
            root = document.SelectSingleNode("Root");
        }

        private void RefreshOverlayables()
        {
            foreach (Overlayable overlayable in overlayablePSet.Values)
                overlayable.RefreshOverlaidProperties();
        }

        private readonly XiGame game;
        private readonly Dictionary<Overlayable, Overlayable> overlayablePSet = new Dictionary<Overlayable, Overlayable>();
        private XmlDocument document;
        private XmlNode root; // OPTIMIZATION: cache root node
        private readonly string overlayFileName;
    }
}
