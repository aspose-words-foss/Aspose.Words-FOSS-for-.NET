// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.NonVisualProperties
{
    /// <summary>
    /// 21.3.2.8 cNvSpPr (Non-Visual Shape Drawing Properties)
    /// This element specifies the non-visual drawing properties for a shape.
    /// </summary>
    internal class DmlCnvPrShape : DmlCnvPrBase
    {
        internal override DmlNvHolder Holder
        {
            get { return DmlNvHolder.Shape; }
        }

        /// <summary>
        /// Specifies that the corresponding shape is a text box and thus should be treated as such by the generating application.
        /// Default is false.
        /// </summary>
        internal bool TextBox
        {
            get { return mTextBox; }
            set { mTextBox = value; }
        }

        private bool mTextBox;
    }
}
