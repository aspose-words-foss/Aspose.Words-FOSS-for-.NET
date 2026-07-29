// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/26/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.NonVisualProperties
{
    /// <summary>
    /// 2.5.36 CT_NonVisualInkContentPartProperties.
    /// A complex type that specifies non-visual ink properties for a content part. 
    /// This provides additional information that does not affect the appearance of ink in the content part to be stored.
    /// </summary>
    internal class DmlCnvPrContentPart : DmlCnvPrBase
    {
        /// <summary>
        /// Specifies whether the ink shape is a comment or an annotation. 
        /// If true, the ink is a comment; otherwise, it is an annotation.
        /// Default is true.
        /// </summary>
        internal bool IsComment
        {
            get { return mIsComment; }
            set { mIsComment = value; }
        }

        internal override DmlNvHolder Holder
        {
            get { return DmlNvHolder.ContentPart; }
        }

        private bool mIsComment = true;
    }
}
