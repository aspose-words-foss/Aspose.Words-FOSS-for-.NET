// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/11/2013 by Andrey Noskov

using System;
using System.ComponentModel;

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// Represents info about an moveFrom or moveTo revision, occurs on runs of text.
    ///
    /// Two different move revision operations are possible on a run: moveFrom, and moveTo.
    /// </summary>
    internal class MoveRevision : RevisionBase, IComplexAttr
    {
        internal MoveRevision(MoveRevisionType type)
            : this(type, string.Empty, DateTime.MinValue)
        {
        }

        internal MoveRevision(MoveRevisionType type, string author, DateTime dateTime)
            : base(author, dateTime)
        {
            Type = type;
        }

        /// <summary>
        /// Clones this instance of revision.
        /// </summary>
        internal MoveRevision Clone()
        {
            return (MoveRevision)MemberwiseClone();
        }

        /// <summary>
        /// Indicates whether the run is 'moved from' or 'moved to' during the revision.
        /// </summary>
        internal MoveRevisionType Type { get; }

        /// <summary>
        /// Reserved for system use. IComplexAttr.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsInheritedComplexAttr
        {
            get { return false; }
        }

        /// <summary>
        /// Reserved for system use. IComplexAttr.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IComplexAttr IComplexAttr.DeepCloneComplexAttr()
        {
            return Clone();
        }
    }
}
