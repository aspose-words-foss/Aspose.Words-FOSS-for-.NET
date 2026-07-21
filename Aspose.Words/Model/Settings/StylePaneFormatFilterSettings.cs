// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/02/2010 by Denis Darkin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Encapsulates different settings that are defined for 
    /// stylePaneFormatFilter (Suggested Filtering for List of Document Styles).
    /// ISO 29500 feature.
    /// </summary>

    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal class StylePaneFormatFilterSettings
    {
        /// <summary>
        /// default ctor.
        /// </summary>
        internal StylePaneFormatFilterSettings()
        {
            mStylePaneFormatFilterFlags = StylePaneFormatFilterDefault;
        }

        internal StylePaneFormatFilterSettings(int defaultFlags)
        {
            mStylePaneFormatFilterFlags = defaultFlags;
        }
        
        /// <summary>
        /// Returns int32 flag containing all filter settings as bits.
        /// </summary>
        internal int Data
        {
            get { return mStylePaneFormatFilterFlags; }
            set { mStylePaneFormatFilterFlags = value; }
        }
        
        /// <summary>
        /// (Display All Styles). Specifies that all styles present in the styles part 
        /// should be displayed in the list of document styles.
        /// </summary>
        internal bool AllStyles
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags, AllStylesBit); }
            set { SetFlag(AllStylesBit, value); }
        }
        
        /// <summary>
        /// (Display Only Custom Styles). Specifies that only styles with the customStyle 
        /// attribute should be displayed in the list of document styles.
        /// </summary>
        internal bool CustomStyles
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags,CustomStylesBit); }
            set { SetFlag(CustomStylesBit, value); }
        }
        
        /// <summary>
        /// (Display Latent Styles). Specifies that all latent styles should be 
        /// displayed in the list of document styles.
        /// </summary>
        internal bool LatentStyles
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags, LatentStylesBit); }
            set { SetFlag(LatentStylesBit, value); }
        }
        
        /// <summary>
        /// (Display Styles in Use). Specifies that only styles used in the 
        /// document should be displayed in the list of document styles.
        /// </summary>
        internal bool StylesInUse
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags, StylesInUseBit); }
            set { SetFlag(StylesInUseBit, value); }
        }
        
        /// <summary>
        /// (Display Heading Styles). Specifies that heading styles 
        /// (styles with a styleId of Heading1 to Heading9) should be displayed 
        /// in the list of document styles when the previous style is used in the
        /// document and/or is present in the Styles part.
        /// </summary>
        internal bool HeadingStyles
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags, HeadingStylesBit); }
            set { SetFlag(HeadingStylesBit, value); }
        }
        
        /// <summary>
        /// (Display NumberingStyles). Specifies that numbering styles should be displayed
        /// in the list of document styles.
        /// </summary>
        internal bool NumberingStyles
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags, NumberingStylesBit); }
            set { SetFlag(NumberingStylesBit, value); }
        }
        
        /// <summary>
        /// (Display Table Styles). Specifies that table styles should be displayed 
        /// in the list of document styles.
        /// </summary>
        internal bool TableStyles
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags, TableStylesBit); }
            set { SetFlag(TableStylesBit, value); }
        }
        
        /// <summary>
        /// (Display Run Level Direct Formatting). Specifies that all unique forms 
        /// of run-level direct formatting should be displayed in the list of document 
        /// styles as though they were each a unique style.
        /// </summary>
        internal bool DirectFormattingOnRuns
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags, DirectFormattingOnRunsBit); }
            set { SetFlag(DirectFormattingOnRunsBit, value); }
        }
        
        /// <summary>
        /// (Display Paragraph Level Direct Formatting). Specifies that all
        /// unique forms of paragraph-level direct formatting should be displayed 
        /// in the list of document styles as though they were each a unique style.
        /// </summary>
        internal bool DirectFormattingOnParagraphs
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags, DirectFormattingOnParagraphsBit); }
            set { SetFlag(DirectFormattingOnParagraphsBit, value); }
        }
        
        /// <summary>
        /// (Display Direct Formatting on Numbering Data). Specifies that all unique forms
        /// of direct formatting of numbering data should be displayed in the list of document 
        /// styles as though they were each a unique style.
        /// </summary>
        internal bool DirectFormattingOnNumbering
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags, DirectFormattingOnNumberingBit); }
            set { SetFlag(DirectFormattingOnNumberingBit, value); }
        }

        /// <summary>
        /// (Display Direct Formatting on Tables). Specifies that all unique forms of direct 
        /// formatting of tables should be displayed in the list of document styles as 
        /// though they were each a unique style.
        /// </summary>
        internal bool DirectFormattingOnTables
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags, DirectFormattingOnTablesBit); }
            set { SetFlag(DirectFormattingOnTablesBit, value); }
        }
        
        /// <summary>
        /// (Display Styles to Remove Formatting). Specifies that a style should be
        /// present which removes all formatting and styles from text.
        /// </summary>
        internal bool ClearFormatting
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags, ClearFormattingBit); }
            set { SetFlag(ClearFormattingBit, value); }
        }
        
        /// <summary>
        /// (Display Heading 1 through 3).Specifies that heading styles with a 
        /// styleId of Heading1 to Heading3 should always be displayed in the 
        /// list of document styles.
        /// </summary>
        internal bool Top3HeadingStyles
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags, Top3HeadingStylesBit); }
            set { SetFlag(Top3HeadingStylesBit, value); }
        }
        
        /// <summary>
        /// (Only Show Visible Styles). Specifies that styles should only be 
        /// shown if the semiHidden element (§17.7.4.16) is false and the hidden element 
        /// (§17.7.4.4) is false.
        /// </summary>
        internal bool VisibleStyles
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags, VisibleStylesBit); }
            set { SetFlag(VisibleStylesBit, value); }
        }
        
        /// <summary>
        /// (Use the Alternate Style Name). Specifies that primary names for styles should not be shown if 
        /// an alternate name using the name element (§17.7.4.9) exists.
        /// </summary>
        internal bool AlternateStyleNames
        {
            get { return BitUtil.IsSetInt32(mStylePaneFormatFilterFlags, AlternateStyleNamesBit); }
            set { SetFlag(AlternateStyleNamesBit, value); }
        }

        private void SetFlag(int flag, bool value)
        {
            mStylePaneFormatFilterFlags = BitUtil.SetBit(mStylePaneFormatFilterFlags, flag, value);
        }

        private int mStylePaneFormatFilterFlags;

        /// <summary>
        /// RK While the spec says default value is 0, it seems MS Word does not write this value to DOCX
        /// when it equals 0x5024, therefore I use it as a default.
        /// </summary>
        internal const int StylePaneFormatFilterDefault = 0x5024;

        private const int AllStylesBit = 0x0001;
        private const int CustomStylesBit = 0x0002;
        private const int LatentStylesBit = 0x0004;
        private const int StylesInUseBit = 0x0008;
        private const int HeadingStylesBit = 0x0020;
        private const int NumberingStylesBit = 0x0040;
        private const int TableStylesBit = 0x0080;
        private const int DirectFormattingOnRunsBit = 0x0100;
        private const int DirectFormattingOnParagraphsBit = 0x0200;
        private const int DirectFormattingOnNumberingBit = 0x0400;
        private const int DirectFormattingOnTablesBit = 0x0800;
        private const int ClearFormattingBit = 0x1000;
        private const int Top3HeadingStylesBit = 0x2000;
        private const int VisibleStylesBit = 0x4000;
        private const int AlternateStyleNamesBit = 0x8000;
    }
}
