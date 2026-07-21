// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2005 by Roman Korchagin

using System;
using System.ComponentModel;

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// Represents info about an insert or delete revision, occurs on runs of text.
    ///
    /// Three different revision operations are possible on a run: inserted, deleted or formatted.
    /// Formatted revision can be combined with deleted or inserted revision.
    /// A piece of text can be inserted by one author and deleted by another author,
    /// in this case both insert and delete revisions can be present on a run.
    ///
    /// 1. Formatted is same as formatting change revision on other objects, for example section properties.
    /// There is an sprm that is followed by sprms for the properties that have changed. We read this
    /// into revision attributes collection and store it as an attribute in the run attribute collection.
    /// 2. Inserted and deleted in WordML actually come not inside rPr element, but at the level of the r element.
    /// </summary>
    internal class EditRevision : RevisionBase, IComplexAttr
    {
        internal EditRevision(EditRevisionType type)
            : this(type, string.Empty, DateTime.MinValue)
        {
        }

        internal EditRevision(EditRevisionType type, string author, DateTime dateTime)
            : base(author, dateTime)
        {
            Type = type;
        }

        /// <summary>
        /// Clones this instance of revision.
        /// </summary>
        internal EditRevision Clone()
        {
            return (EditRevision)MemberwiseClone();
        }

        /// <summary>
        /// Indicates whether the run is inserted or deleted during the revision.
        /// </summary>
        internal EditRevisionType Type { get; }

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
