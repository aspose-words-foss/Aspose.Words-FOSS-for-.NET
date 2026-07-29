// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/05/2011 by Denis Darkin

using Aspose.Collections.Generic;
using Aspose.Common;

namespace Aspose.Words
{
    /// <summary>
    /// Handles unique integer IDs.
    /// Provides facilities to:
    /// - create new unique IDs
    /// - track uniqueness of existing IDs
    /// </summary>
    internal class UniqueIdManager
    {
        /// <summary>
        /// Tries to add id to the set of unique ids.
        /// If such id is already present, then generates a new one and returns it.
        /// </summary>
        internal int AddUniqueId(int id)
        {
            int result = id;

            if (!mUsedIds.Add(id))
                result = AddUniqueId(GenerateInteger());

            return result;
        }

        /// <summary>
        /// Generate a new Id for an object.
        /// </summary>
        internal int GetUniqueId()
        {
            return AddUniqueId(GenerateInteger());
        }

        /// <summary>
        /// Generate a new positive Pseudo-random integer that is not random in TEST builds and pseudo-random in RELEASE builds.
        /// </summary>
        internal static int GenerateInteger()
        {
            // andrnosk: WORDSNET-7613 Return only positive values, because for example MS Word 2010 does not allow negative ID for DrawingML.
            return  System.Math.Abs(RandomUtil.NewGuid().GetHashCode());
        }

        private readonly HashSetGeneric<int> mUsedIds = new HashSetGeneric<int>();
    }
}
