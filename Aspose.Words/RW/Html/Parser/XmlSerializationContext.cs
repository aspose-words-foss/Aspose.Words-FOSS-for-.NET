// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/09/2014 by Alexey Butalov

using System;
using System.Collections.Generic;
using Aspose.Collections.Generic;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// XML serialization context.
    /// </summary>
    internal class XmlSerializationContext
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="htmlNode">HTML node for serialization.</param>
        internal XmlSerializationContext(HtmlNode htmlNode)
        {
            mNsPrefixMapStack = new Stack<SortedStringListGeneric<string>>();
            mPredefinedNsPrefixMap = new SortedStringListGeneric<string>(false);

            // Collect namespaces declared in parent elements for the given node.
            HtmlElementNode elementNode = htmlNode.Parent;
            while (elementNode != null)
            {
                UpdateNsPrefixMap(mPredefinedNsPrefixMap, elementNode.Attributes, false);
                elementNode = elementNode.Parent;
            }

            const string xmlNamespacePrefix = "xml";
            if (!mPredefinedNsPrefixMap.ContainsKey(xmlNamespacePrefix))
                mPredefinedNsPrefixMap.Add(xmlNamespacePrefix, W3CNamespaces.Xml);
        }

        /// <summary>
        /// Call this method before node serialization.
        /// </summary>
        /// <param name="elementNode"></param>
        internal void StartNode(HtmlElementNode elementNode)
        {
            SortedStringListGeneric<string> nsPrefixMap = new SortedStringListGeneric<string>(false);
            nsPrefixMap.AddRange((mNsPrefixMapStack.Count != 0)
                                     ? NsPrefixMap
                                     : mPredefinedNsPrefixMap);
            UpdateNsPrefixMap(nsPrefixMap, elementNode.Attributes, true);
            mNsPrefixMapStack.Push(nsPrefixMap);
        }

        /// <summary>
        /// Call this method after node serialization.
        /// </summary>
        internal void EndNode()
        {
            Debug.Assert(mNsPrefixMapStack.Count != 0);
            mNsPrefixMapStack.Pop();
        }

        /// <summary>
        /// Checks whether the given name is well-formed.
        /// </summary>
        internal bool IsWellFormedName(string name)
        {
            Debug.Assert(mNsPrefixMapStack.Count != 0);

            int colonCharIndex = name.IndexOf(':');
            if (colonCharIndex != -1)
            {
                string prefix = name.Substring(0, colonCharIndex);
                if (!StringUtil.EqualsOrdinalIgnoreCase(prefix, XmlnsPrefix) &&
                    !NsPrefixMap.ContainsKey(prefix))
                {
                    return false;
                }
            }

            return true;
        }

        public void AddXLinkNamespaceToContext()
        {
            if (!NsPrefixMap.ContainsKey("xlink"))
                NsPrefixMap.Add("xlink", W3CNamespaces.XLink);
        }

        private static void UpdateNsPrefixMap(
            SortedStringListGeneric<string> nsPrefixMap,
            HtmlAttributeCollection attributes,
            bool replaceExisting)
        {
            const string xmlnsPrefixWithColon = XmlnsPrefix + ":";
            foreach (HtmlAttribute attribute in attributes)
            {
                if (!attribute.Name.StartsWith(xmlnsPrefixWithColon, StringComparison.OrdinalIgnoreCase))
                    continue;

                string nsPrefix = attribute.Name.Substring(xmlnsPrefixWithColon.Length);
                if (nsPrefix == string.Empty)
                    continue;

                if (replaceExisting || !nsPrefixMap.ContainsKey(nsPrefix))
                    nsPrefixMap[nsPrefix] = attribute.Value;
            }
        }

        private SortedStringListGeneric<string> NsPrefixMap
        {
            get { return mNsPrefixMapStack.Peek(); }
        }

        private readonly Stack<SortedStringListGeneric<string>> mNsPrefixMapStack;
        private readonly SortedStringListGeneric<string> mPredefinedNsPrefixMap;

        private const string XmlnsPrefix = "xmlns";
    }
}
