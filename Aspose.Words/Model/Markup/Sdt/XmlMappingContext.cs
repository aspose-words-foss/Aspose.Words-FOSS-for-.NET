// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/09/2020 by Alexey Morozov

using System;
using System.Collections.Generic;
using System.Xml;
using Aspose.Crypto;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Provides context information to bulk XML mapping update.
    /// </summary>
    internal class XmlMappingContext
    {
        /// <summary>
        /// Returns mapped custom XML document from cache.
        /// </summary>
        internal XmlDocument Get(string storeItemId)
        {
            return mXmlCache.ContainsKey(storeItemId) ? mXmlCache[storeItemId] : null;
        }

        /// <summary>
        /// Returns mapped XML nodes form cache.s
        /// </summary>
        internal IList<XmlNode> Get(string storeItemId, string xPath)
        {
            string cacheKey = string.Format("{0}{1}", storeItemId, xPath);
            return mXPathCache[cacheKey];
        }

        /// <summary>
        /// Returns cached custom XML document CRC.
        /// </summary>
        /// <remarks>
        /// AM. Still do not want to cache CRC in CustomXmlPart class as we provide public access to byte array and
        /// we cannot detect changes in byte array itself, for example CustomXmlPart.Data[0] = 100.
        /// This context caches data for bulk update during post-loading/validation only.
        /// </remarks>
        internal string GetStoreItemChecksum(XmlMapping xmlMapping)
        {
            // WORDSNET-23673 Missing CustomXmlPart.
            if (xmlMapping.CustomXmlPart == null)
                return null;

            string storeItemId = xmlMapping.StoreItemId;

            if (!mStoreItemChecksumCache.ContainsKey(storeItemId))
            {
                string checksum = Crc32Xfer.GetBase64String(xmlMapping.CustomXmlPart.Data);
                mStoreItemChecksumCache[storeItemId] = checksum;
            }

            return mStoreItemChecksumCache[storeItemId];
        }

        internal bool ContainsXmlDocument(string storeItemId)
        {
            return mXmlCache.ContainsKey(storeItemId);
        }

        internal bool ContainsXPath(string storeItemId, string xPath)
        {
            string cacheKey = string.Format("{0}{1}", storeItemId, xPath);
            return mXPathCache.ContainsKey(cacheKey);
        }

        internal void Add(string storeItemId, string xPath, IList<XmlNode> xmlNodes)
        {
            string cacheKey = string.Format("{0}{1}", storeItemId, xPath);
            mXPathCache.Add(cacheKey, xmlNodes);
        }

        internal XmlDocument Add(string storeItemId, XmlDocument xmlDocument)
        {
            mXmlCache.Add(storeItemId, xmlDocument);
            return xmlDocument;
        }

        internal void Remove(string storeItemId)
        {
            if (mXmlCache.ContainsKey(storeItemId))
                mXmlCache.Remove(storeItemId);

            if (mStoreItemChecksumCache.ContainsKey(storeItemId))
                mStoreItemChecksumCache.Remove(storeItemId);

            if (mXPathCache.Keys.Count > 0)
            {
                List<string> itemsToRemove = new List<string>();
                foreach (string key in mXPathCache.Keys)
                {
                    if (key.StartsWith(storeItemId, StringComparison.Ordinal))
                        itemsToRemove.Add(key);
                }

                foreach (string key in itemsToRemove)
                    mXPathCache.Remove(key);
            }
        }

        /// <summary>
        /// Collects SDT loaded with mapped documents.
        /// </summary>
        internal List<StructuredDocumentTag> InnerTags
        {
            get { return mInnerTags; }
        }

        internal TimeZoneInfo CustomTimeZone = TimeZoneInfo.Local;
        private readonly Dictionary<string, XmlDocument> mXmlCache = new Dictionary<string, XmlDocument>();
        private readonly Dictionary<string, string> mStoreItemChecksumCache = new Dictionary<string, string>();
        private readonly Dictionary<string, IList<XmlNode>> mXPathCache = new Dictionary<string, IList<XmlNode>>();
        private readonly List<StructuredDocumentTag> mInnerTags = new List<StructuredDocumentTag>();
    }
}
