// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2005 by Roman Korchagin

using System;
using System.ComponentModel;

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// Represents info about a format revision (change of attributes) of a run, paragraph or a section.
    /// </summary>
    internal class FormatRevision : RevisionBase,  IComplexAttr
    {
        internal FormatRevision(WordAttrCollection revPr, string author, DateTime dateTime)
            : base(author, dateTime)
        {
            if (revPr == null)
                throw new ArgumentNullException("revPr");
            if (author == null)
                throw new ArgumentNullException("author");

            RevPr = revPr;
        }

        /// <summary>
        /// Reserved for system use. IComplexAttr.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IComplexAttr IComplexAttr.DeepCloneComplexAttr()
        {
            FormatRevision lhs = (FormatRevision)MemberwiseClone();
            lhs.RevPr = RevPr.Clone();
            return lhs;
        }

        /// <summary>
        /// Reserved for system use. IComplexAttr.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsInheritedComplexAttr
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the revised (modified) attributes.
        /// This will not be null, but could be an empty collection.
        /// </summary>
        internal WordAttrCollection RevPr { get; set; }
    }
}
