// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/08/2025 by Edward

using System;

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// Used to track field numbering changes.
    /// </summary>
    internal class FieldNumberRevision : RevisionBase, IComplexAttr
    {
        internal FieldNumberRevision(string author, DateTime dateTime, string original)
            : base(author, dateTime)
        {
            Original = original;
        }

        /// <summary>
        /// Original field numbering.
        /// </summary>
        public string Original { get; }

        bool IComplexAttr.IsInheritedComplexAttr
        {
            get { return false; }
        }

        IComplexAttr IComplexAttr.DeepCloneComplexAttr()
        {
            return (IComplexAttr)MemberwiseClone();
        }
    }
}
