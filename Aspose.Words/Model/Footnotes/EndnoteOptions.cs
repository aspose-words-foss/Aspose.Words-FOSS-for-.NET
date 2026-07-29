// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/06/2017 by Alexander Zhiltsov

namespace Aspose.Words.Notes
{
    /// <summary>
    /// Represents the endnote numbering options for a document or section.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-footnote-and-endnote/">Working with Footnote and Endnote</a> documentation article.</para>
    /// </summary>
    /// <seealso cref="Document.EndnoteOptions"/>
    /// <seealso cref="PageSetup.EndnoteOptions"/>
    public sealed class EndnoteOptions : IFootnoteOptions
    {
        internal EndnoteOptions(ISectionAttrSource parent)
        {
            Debug.Assert(parent != null);
            mParent = parent;
        }

        /// <summary>
        /// Specifies the endnotes position.
        /// </summary>
        public EndnotePosition Position
        {
            get { return (EndnotePosition)FetchAttr(SectAttr.EndnoteLocation); }
            set { SetAttr(SectAttr.EndnoteLocation, value); }
        }

        /// <summary>
        /// Specifies the number format for automatically numbered endnotes.
        /// </summary>
        /// <remarks>
        /// <para>Not all number styles are applicable for this property. For the list of applicable
        /// number styles see the Insert Footnote or Endnote dialog box in Microsoft Word. If you select
        /// a number style that is not applicable, Microsoft Word will revert to a default value.</para>
        /// </remarks>
        public NumberStyle NumberStyle
        {
            get { return (NumberStyle)FetchAttr(SectAttr.EndnoteNumberStyle); }
            set { SetAttr(SectAttr.EndnoteNumberStyle, value); }
        }

        /// <summary>
        /// Specifies the starting number or character for the first automatically numbered endnotes.
        /// </summary>
        /// <remarks>
        /// <para>This property has effect only when <see cref="RestartRule"/> is set to
        /// <see cref="FootnoteNumberingRule.Continuous"/>.</para>
        /// </remarks>
        public int StartNumber
        {
            get { return (int)FetchAttr(SectAttr.EndnoteStartNumber); }
            set { SetAttr(SectAttr.EndnoteStartNumber, value); }
        }

        /// <summary>
        /// Determines when automatic numbering restarts.
        /// </summary>
        /// <remarks>
        /// <para>Not all values are applicable to endnotes.
        /// To ascertain which values are applicable see <see cref="FootnoteNumberingRule"/>.</para>
        /// </remarks>
        public FootnoteNumberingRule RestartRule
        {
            get { return (FootnoteNumberingRule)FetchAttr(SectAttr.EndnoteNumberingRule); }
            set { SetAttr(SectAttr.EndnoteNumberingRule, value); }
        }

        /// <summary>
        /// Specifies the endnotes position.
        /// </summary>
        /// <remarks>
        /// <para>Not all values are applicable to endnotes.
        /// To ascertain which values are applicable see <see cref="FootnoteLocation"/>.</para>
        /// </remarks>
        FootnoteLocation IFootnoteOptions.Location
        {
            get { return (FootnoteLocation)Position; }
            set { Position = (EndnotePosition)value; }
        }

        private object FetchAttr(int key)
        {
            return mParent.FetchSectionAttr(key);
        }

        private void SetAttr(int key, object value)
        {
            mParent.SetSectionAttr(key, value);
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly ISectionAttrSource mParent;
    }
}
