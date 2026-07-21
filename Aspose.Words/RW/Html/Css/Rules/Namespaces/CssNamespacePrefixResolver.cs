// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/04/2020 by Victor Chebotok

using System.Collections.Generic;
using Aspose.Collections.Generic;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Generates unique prefixes for namespaces imported from different stylesheets.
    /// </summary>
    internal class CssNamespacePrefixResolver
    {
        internal string GetPrefix(string namespaceName, string suggestedPrefix)
        {
            // Note that the namespace name can be empty.
            Debug.Assert(namespaceName != null);

            string knownPrefix;
            if (mNamespaceToPrefixMap.TryGetValue(namespaceName, out knownPrefix))
            {
                return knownPrefix;
            }

            if (!StringUtil.HasChars(suggestedPrefix))
            {
                suggestedPrefix = "ns";
            }

            string uniquePrefix = suggestedPrefix;
            int counter = 1;
            while (mUsedPrefixes.Contains(uniquePrefix))
            {
                uniquePrefix = suggestedPrefix + FormatterPal.IntToStr(counter);
                ++counter;
            }

            mNamespaceToPrefixMap.Add(namespaceName, uniquePrefix);
            mUsedPrefixes.Add(uniquePrefix);

            return uniquePrefix;
        }

        internal string RedefinePrefix(string oldNamespaceName, string newNamespaceName)
        {
            string prefix = mNamespaceToPrefixMap[oldNamespaceName];
            mNamespaceToPrefixMap.Remove(oldNamespaceName);
            mNamespaceToPrefixMap.Add(newNamespaceName, prefix);
            return prefix;
        }

        internal bool IsRegistered(string namespaceName, string prefix)
        {
            string usedPrefix;
            if (!mNamespaceToPrefixMap.TryGetValue(namespaceName, out usedPrefix))
            {
                return false;
            }
            return usedPrefix == prefix;
        }

        private readonly Dictionary<string, string> mNamespaceToPrefixMap = new Dictionary<string, string>();

        private readonly HashSetGeneric<string> mUsedPrefixes = new HashSetGeneric<string>();
    }
}
