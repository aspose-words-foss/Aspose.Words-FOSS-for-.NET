// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/06/2010 by Denis Darkin

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Represents plain/rich text SDT. See Iso29500, part 1, chapter 17.5.2.44 
    /// text (Plain Text Structured Document Tag)
    /// </summary>
    internal class SdtText : SdtControlProperties
    {
        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="isRichText">if false, then this text control only allows plaintext, otherwise allows richtext.</param>
        internal SdtText(bool isRichText)
        {
            mIsRichtext = isRichText;
        }

        /// <summary> Allow Soft Line Breaks. Specifies whether soft line breaks can be added to 
        /// the contents of this structured document tag when this document is modified. 
        /// </summary>
        /// <remarks>This setting shall not affect the ability of the structured document tag to display 
        /// existing soft line breaks (which shall be preserved) and shall only affect the ability to add 
        /// line breaks when the document is modified by an application.</remarks>
        internal bool IsMultiline
        {
            get { return mIsMultiline; }
            set { mIsMultiline = value; }
        }

        internal override SdtType Type
        {
            get { return (mIsRichtext)? SdtType.RichText : SdtType.PlainText; }
        }

        private bool mIsMultiline;
        private readonly bool mIsRichtext;
    }
}
