// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/08/2016 by Edward Voronov

using System;
using Aspose.Collections;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a cache of hidden attribute values.
    /// </summary>
    internal class HiddenAttributeCache
    {
        internal bool GetHiddenAttribute(Inline node)
        {
            int hashCode = node.GetHashCode();

            if (mCache.ContainsKey(hashCode))
                return Convert.ToBoolean(mCache[hashCode]);

            bool isHidden = node.IsHiddenOrDeleted;
            mCache.Add(hashCode, Convert.ToInt32(isHidden));

            return isHidden;
        }

        internal void Invalidate()
        {
            mCache.Clear();
        }

        private readonly IntToIntDictionary mCache = new IntToIntDictionary();
    }
}
