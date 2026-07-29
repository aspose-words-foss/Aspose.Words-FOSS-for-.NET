// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2007 by Roman Korchagin

namespace Aspose.Words.Notes
{
    /// <summary>
    /// Represents the footnote numbering options for a document or section.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-footnote-and-endnote/">Working with Footnote and Endnote</a> documentation article.</para>
    /// </summary>
    /// <seealso cref="Document.FootnoteOptions"/>
    /// <seealso cref="PageSetup.FootnoteOptions"/>
    public sealed class FootnoteOptions : IFootnoteOptions
    {
        internal FootnoteOptions(ISectionAttrSource parent)
        {
            Debug.Assert(parent != null);
            mParent = parent;
        }

        /// <summary>
        /// Specifies the footnotes position.
        /// </summary>
        public FootnotePosition Position
        {
            get { return (FootnotePosition)FetchAttr(SectAttr.FootnoteLocation); }
            set { SetAttr(SectAttr.FootnoteLocation, value); }
        }

        /// <summary>
        /// Specifies the number format for automatically numbered footnotes.
        /// </summary>
        /// <remarks>
        /// <para>Not all number styles are applicable for this property. For the list of applicable
        /// number styles see the Insert Footnote or Endnote dialog box in Microsoft Word. If you select
        /// a number style that is not applicable, Microsoft Word will revert to a default value.</para>
        /// </remarks>
        public NumberStyle NumberStyle
        {
            get { return (NumberStyle)FetchAttr(SectAttr.FootnoteNumberStyle); }
            set { SetAttr(SectAttr.FootnoteNumberStyle, value); }
        }

        /// <summary>
        /// Specifies the starting number or character for the first automatically numbered footnotes.
        /// </summary>
        /// <remarks>
        /// <para>This property has effect only when <see cref="RestartRule"/> is set to
        /// <see cref="FootnoteNumberingRule.Continuous"/>.</para>
        /// </remarks>
        public int StartNumber
        {
            get { return (int)FetchAttr(SectAttr.FootnoteStartNumber); }
            set { SetAttr(SectAttr.FootnoteStartNumber, value); }
        }

        /// <summary>
        /// Determines when automatic numbering restarts.
        /// </summary>
        public FootnoteNumberingRule RestartRule
        {
            get { return (FootnoteNumberingRule)FetchAttr(SectAttr.FootnoteNumberingRule); }
            set { SetAttr(SectAttr.FootnoteNumberingRule, value); }
        }

        /// <summary>
        /// Specifies the number of columns with which the footnotes area is formatted.
        /// </summary>
        /// <remarks>
        /// If this property has the value of 0, the footnotes area is formatted with a number of columns based on
        /// the number of columns on the displayed page. The default value is 0.
        /// </remarks>
        public int Columns
        {
            get { return (int)FetchAttr(SectAttr.FootnoteColumns); }
            set { SetAttr(SectAttr.FootnoteColumns, value); }
        }

        /// <summary>
        /// Specifies the footnotes position.
        /// </summary>
        /// <remarks>
        /// <para>Not all values are applicable to footnotes.
        /// To ascertain which values are applicable see <see cref="FootnoteLocation"/>.</para>
        /// </remarks>
        FootnoteLocation IFootnoteOptions.Location
        {
            get { return (FootnoteLocation)Position; }
            set { Position = (FootnotePosition)value; }
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
