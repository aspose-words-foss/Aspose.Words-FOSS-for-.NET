// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2017 by Dmitry Sokolov

using System.Collections.Generic;
using Aspose.Collections.Generic;

namespace Aspose.Xml
{
    /// <summary>
    /// Implements storage for XML namespaces.
    /// </summary>
    public class XmlTextReaderNamespaceStorage
    {
        public XmlTextReaderNamespaceStorage(XmlTextReaderNamespaceStorage parent)
        {
            mParent = parent;
        }

        public IDictionary<string, string> ConvertTo()
        {
            IDictionary<string, string> namesapcesMap = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> item in mNamespaces)
            {
                namesapcesMap.Add(item.Key, item.Value);
            }

            return namesapcesMap;
        }

        internal bool IsPrefixDefined(string prefix)
        {
            XmlTextReaderNamespaceStorage storage = this;
            while (storage != null)
            {
                if (storage.mNamespaces.ContainsKey(prefix))
                    return true;

                storage = storage.mParent;
            }
            return false;
        }

        internal SortedStringListGeneric<string> Namespaces { get { return mNamespaces; } }

        private readonly SortedStringListGeneric<string> mNamespaces = new SortedStringListGeneric<string>();
        private readonly XmlTextReaderNamespaceStorage mParent;
    }
}
