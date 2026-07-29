// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/07/2014 by Andrey Noskov

using System;
using Aspose.Collections;

namespace Aspose.Words.Drawing.Core.Dml.NonVisualProperties
{
    /// <summary>
    /// Specifies all locking properties for a Dml node.
    /// 
    /// We can use Flags instead of DmlLocks collection, but for roundtrip it is better to keep collection.
    /// Sometimes MS Word writes lock with false value, that means lock is not applied, but it is still present in xml.
    /// </summary>
    internal class DmlLocks : DmlExtensionListSource
    {
        internal DmlLocks Clone()
        {
            DmlLocks lhs = (DmlLocks)MemberwiseClone();

            if (mLocks != null)
            {
                lhs.mLocks = new IntToObjDictionary<Object>();
                IntToObjDictionary<Object>.Enumerator enumerator = mLocks.GetEnumerator();
                while (enumerator.MoveNext())
                    lhs.mLocks.Add(enumerator.CurrentKey, enumerator.CurrentValue);
            }

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        internal void AddLock(DmlLock key, bool value)
        {
            mLocks[(int)key] = value;
        }
        
        internal object GetLock(DmlLock key)
        {
            return mLocks[(int)key];
        }

        internal int Count
        {
            get { return mLocks.Count; }
        }

        private IntToObjDictionary<Object> mLocks = new IntToObjDictionary<Object>();
    }
}
