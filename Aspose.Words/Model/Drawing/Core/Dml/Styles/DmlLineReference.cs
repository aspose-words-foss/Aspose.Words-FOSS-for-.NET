// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/02/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Outlines;

namespace Aspose.Words.Drawing.Core.Dml.Styles
{
    /// <summary>
    /// 20.1.4.2.19 lnRef (Line Reference)
    /// This element defines a reference to a line style within the style matrix.
    /// </summary>
    internal class DmlLineReference : DmlStyleReferenceBase
    {
        /// <summary>
        /// Specifies the style matrix index of the style referred to.
        /// The content is a restriction of the W3C XML Schema unsignedInt datatype.
        /// </summary>
        internal int StyleMatrixIndex
        {
            get { return mStyleMatrixIndex; }
            set { mStyleMatrixIndex = value; }
        }

        private int mStyleMatrixIndex;
    }
}
