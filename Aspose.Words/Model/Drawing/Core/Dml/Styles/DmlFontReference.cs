// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/02/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Styles
{
    /// <summary>
    /// 20.1.4.1.17 fontRef (Font Reference)
    /// This element represents a reference to a themed font. 
    /// When used it specifies which themed font to use along with a choice of color.
    /// </summary>
    internal class DmlFontReference : DmlStyleReferenceBase
    {
        /// <summary>
        /// Specifies the identifier of the font to reference. 
        /// </summary>
        internal DmlFontCollectionIndex FontCollectionIndex
        {
            get { return mFontCollectionIndex; }
            set { mFontCollectionIndex = value; }
        }

        private DmlFontCollectionIndex mFontCollectionIndex;
    }
}
