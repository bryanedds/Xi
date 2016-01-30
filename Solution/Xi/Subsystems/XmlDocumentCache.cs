using System.Collections.Generic;
using System.Xml;

namespace Xi
{
    /// <summary>
    /// Caches XML documents.
    /// </summary>
    public class XmlDocumentCache
    {
        /// <summary>
        /// Create an XmlDocumentCache.
        /// </summary>
        /// <param name="game">The game.</param>
        public XmlDocumentCache(XiGame game)
        {
            this.game = game;
        }

        /// <summary>
        /// Get an XML document, loading from disk if not in cache (or not using cache).
        /// Do NOT mutate any documents retrieved from here - they are intended to be immutable.
        /// Only caches while editing.
        /// </summary>
        /// <param name="fileName">The file name of the XML document.</param>
        public XmlDocument GetXmlDocument(string fileName)
        {
            XiHelper.ArgumentNullCheck(fileName);
            XiHelper.ValidateFileName(fileName);
            return game.Editing ? LoadXmlDocument(fileName) : LookUpXmlDocument(fileName);
        }

        /// <summary>
        /// Clear the cached documents.
        /// </summary>
        public void ClearXmlDocuments()
        {
            xmlDocuments.Clear();
        }

        private static XmlDocument LoadXmlDocument(string fileName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);
            return xmlDocument;
        }

        private XmlDocument LookUpXmlDocument(string fileName)
        {
            XmlDocument xmlDocument;
            if (!xmlDocuments.TryGetValue(fileName, out xmlDocument))
                xmlDocuments.Add(fileName, xmlDocument = LoadXmlDocument(fileName));
            return xmlDocument;
        }

        private readonly XiGame game;
        private readonly Dictionary<string, XmlDocument> xmlDocuments = new Dictionary<string, XmlDocument>();
    }
}
