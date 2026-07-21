// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/04/2005 by Roman Korchagin

using System;
using Aspose.Words.Fonts;
using Aspose.Words.Nrx;
using Aspose.Words.Saving;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Defines the document properties.
    /// </summary>
    [CppForceInclude("Aspose.Words.StyleIndex", true)]
    [CppForceInclude("Aspose.Words.Settings.ScreenSize", true)]
    internal class DocPr
    {
        /// <summary>
        /// Creates a deep copy of the object. Don't forget to add any new sub objects to here.
        /// </summary>
        internal DocPr Clone()
        {
            DocPr lhs = (DocPr)MemberwiseClone();
            
            lhs.ActiveWritingStyle = ActiveWritingStyle.Clone();
            lhs.MailMergeSettings = MailMergeSettings.Clone();
            lhs.DocumentProtection = DocumentProtection.Clone();
            lhs.WriteProtection = WriteProtection.Clone();
            lhs.FootnotePr = FootnotePr.Clone();
            lhs.CompatibilityOptions = CompatibilityOptions.Clone();
            lhs.ViewOptions = ViewOptions.Clone();
            lhs.HyphenationOptions = HyphenationOptions.Clone();
            lhs.StylePaneFormatFilterSettings = new StylePaneFormatFilterSettings(this.StylePaneFormatFilterSettings.Data);
            lhs.ThemeFontLanguages = ThemeFontLanguages.Clone();
            lhs.MathProperties = MathProperties.Clone();

            lhs.XmlNamespaces = XmlNamespaces.Clone();
            lhs.XmlSchemaReferences = XmlSchemaReferences.Clone();

            return lhs;
        }

        /// <summary>
        /// Sets all font embedding options equal to embedding options in specified <paramref name="fontInfos"/>.
        /// </summary>
        internal void UpdateEmbedFontOptions(FontInfoCollection fontInfos)
        {
            EmbedTrueTypeFonts = fontInfos.EmbedTrueTypeFonts;
            EmbedSystemFonts = fontInfos.EmbedSystemFonts;
            SaveSubsetFonts = fontInfos.SaveSubsetFonts;
        }
        
        /// <summary>
        /// Specifies presence of footnotes in document. Used only in Word95.
        /// </summary>
        internal GrpfIhdtDop GrpfIhdt;

        /// <summary>
        /// Helps avoid unintentionally distributing hidden information, such as the document's author 
        /// or the names associated with comments or tracked changes (Security option).
        /// //security
        /// </summary>
        internal bool RemovePersonalInformation;

        /// <summary>
        /// Allows PostScript code in PRINT fields in a document to print on top of the document text 
        /// instead of underneath it (Print option). This elements setting has no effect if a document 
        /// does not contain PRINT fields.
        /// //print
        /// </summary>
        internal bool PrintPostScriptOverText;

        /// <summary>
        /// Word for the Macintosh setting that has no effect in other versions of Word (Print option).
        /// //print
        /// </summary>
        internal bool PrintFractionalCharacterWidth;

        /// <summary>
        /// Prints the data entered into an online form without printing the online form (Print option).
        /// //print
        /// </summary>
        internal bool PrintFormsData;

        /// <summary>
        /// Stores the TrueType fonts used to create this document along with the document (Save option). 
        /// Others who open the document will be able to view and print it with the fonts used to create it, 
        /// even if those fonts aren't installed on their computer. (NOTE: TrueType fonts are not embedded in 
        /// XML files.)
        /// //save
        /// </summary>
        internal bool EmbedTrueTypeFonts;

        /// <summary>
        /// For the TrueType fonts in your document, embed fonts that are likely to already be
        /// installed on a computer (Save option). This option takes effect only when Embed TrueType 
        /// Fonts option is on.
        /// //save
        /// </summary>
        internal bool EmbedSystemFonts;

        /// <summary>
        /// For the TrueType fonts in your document, embeds only the font styles you actually used in the document, 
        /// which may decrease the file size of your document (Save option). If you used 32 or fewer 
        /// characters of a font, Word embeds only those characters. This option takes effect only when Embed 
        /// TrueType Fonts option is on.
        /// //save
        /// </summary>
        internal bool SaveSubsetFonts;

        /// <summary>
        /// Saves the data entered in an online form as a single, tab-delimited record so you can use 
        /// it in a database. Word saves the file in Text Only file format.
        /// //save
        /// </summary>
        internal bool SaveFormsData;

        /// <summary>
        /// Aligns paragraph borders and tables with the page border throughout the document (Page Border option).
        /// Setting this element to on eliminates any gaps between adjoining borders. However, Word aligns, or snaps, 
        /// text to the edge of a table only if the text is one character width (10.5 points) or less from the page border.
        /// //page setup
        /// </summary>
        internal bool AlignBordersAndEdges;

        /// <summary>
        /// Causes the page border to exclude the header (Page Border option).
        /// //page setup
        /// </summary>
        internal bool BordersDoNotSurroundHeader;

        /// <summary>
        /// Causes the page border to exclude the footer (Page Border option).
        /// //page setup
        /// </summary>
        internal bool BordersDoNotSurroundFooter;

        /// <summary>
        /// Matches "gutterAtTop" in MS Word document settings (17.15.1.50 of ISO-29500).
        /// The actual gutter position may be different depending on multiple page setup, page orientation and other section or document properties.
        /// <seealso cref="IsGutterSide"/> comments.
        /// </summary>
        internal bool GutterAtTop;

        /// <summary>
        /// It is supposed to report if there is a gutter on a page side, 
        /// but the actual gutter position depends on a number of properties and the result may not be correct.
        /// </summary>
        /// <remarks>
        /// dmatv: This property is not used by the layout anymore.
        /// The logic that places the gutter correctly is currently in <see cref="PageMarginCalculator"/>.
        /// The behavior depends on section orientation, first section orientation and some other conditions.
        /// Also there is a nasty twist when MS Word behaves inconsistently determining if the gutter is on the side for 2-page and book fold setups.
        /// See TestJira16284*. I've submitted WORDSNET-16349 to review the code depending on this property in the model (and probably get rid of it).
        /// </remarks>
        internal bool IsGutterSide
        {
            get { return !GutterAtTop
                || (MultiplePages == MultiplePagesType.MirrorMargins)
                || (MultiplePages == MultiplePagesType.BookFoldPrinting)
                || (MultiplePages == MultiplePagesType.BookFoldPrintingReverse)
                || (MultiplePages == MultiplePagesType.TwoPagesPerSheet);
            }
        }

        /// <summary>
        /// Specifies the writing style you want Word to use to when checking grammar in this document 
        /// (Spelling and Grammar option)
        /// //grammar
        /// </summary>
        internal WritingStylePr ActiveWritingStyle = new WritingStylePr();

        /// <summary>
        /// Gets or sets the state of the spelling checker in this document.
        /// //grammar
        /// </summary>
        internal ProofState ProofStateSpelling = ProofState.None;

        /// <summary>
        /// Gets or sets the state of the grammar checker in this document.
        /// //grammar
        /// </summary>
        internal ProofState ProofStateGrammar = ProofState.None;

        /// <summary>
        /// Specifies the template that's attached to this document (Templates and Add-Ins option).
        /// //templates
        /// </summary>
        internal string AttachedTemplate = String.Empty;

        /// <summary>
        /// Updates the styles in this document to match the styles in the attached template each time you open 
        /// the document (Templates and Add-Ins option). This ensures that your document contains up-to-date 
        /// style formatting.
        /// //templates
        /// </summary>
        internal bool LinkStyles;

        /// <summary>
        /// This element specifies whether applications shall prevent the replacement of the complete set 
        /// of styles stored in the Styles part when editing this document.
        /// </summary>
        internal bool StyleLockQuickFormatSet;

        /// <summary>
        /// This element specifies whether applications shall prevent the modification of the document's 
        /// theme information stored in the Theme part when editing this document.
        /// </summary>
        internal bool StyleLockTheme;

        /// <summary>
        /// Stores filter pane format settings as boolean properties.
        /// </summary>
        internal StylePaneFormatFilterSettings StylePaneFormatFilterSettings = new StylePaneFormatFilterSettings();

        /// <summary>
        /// This element specifies a suggested sorting which should be applied to the list of document 
        /// styles in this application if the styles are displayed in a user interface.
        /// </summary>
        internal StylePaneSortMethod StylePaneSortMethod = StylePaneSortMethod.Default;

        /// <summary>
        /// Stores multiple pages type, which should be applied to the document upon printing or rendering. 
        /// </summary>
        /// <remarks>
        /// Note, changing this property affects all sections in the document.
        /// </remarks>
        internal MultiplePagesType MultiplePages = MultiplePagesType.Default;

        /// <summary>
        /// Gets a boolean value indicating if <see>MultiplePages</see> property specifies book fold layout.
        /// </summary>
        internal bool IsBookFold
        {
            get
            {
                return (MultiplePages == MultiplePagesType.BookFoldPrinting)
                    || (MultiplePages == MultiplePagesType.BookFoldPrintingReverse);
            }
        }

        /// <summary>
        /// Do Not Track Formatting Revisions When Tracking Revisions.
        /// </summary>
        internal bool DoNotTrackFormatting;

        /// <summary>
        /// This element specifies that applications shall not track revisions made to this WordprocessingML document
        /// as moves when the trackRevisions element (§2.15.1.90) is turned on, even when that syntax is appropriate.
        /// Instead, applications should use a standard insertion and deletion annotation syntax.
        /// Existing moves shall not be modified.
        /// </summary>
        internal bool DoNotTrackMoves = false;

        /// <summary>
        /// Specifies the document type used by the AutoFormat feature.
        /// //general
        /// </summary>
        internal AutoFormatDocumentType DocumentType = AutoFormatDocumentType.Normal;

        /// <summary>
        /// Contains the elements that hold mail merge information for this document. 
        /// </summary>
        internal MailMergeSettings MailMergeSettings = new MailMergeSettings();

        /// <summary>
        /// Marks changes in the current document and keeps track of each change by reviewer name.
        /// //revisions
        /// </summary>
        internal bool TrackRevisions;

        /// <summary>
        /// Track changes: show markup
        /// // revisions
        /// </summary>
        internal bool ShowMarkup = true;

        /// <summary>
        /// Track changes: show annotations
        /// // revisions
        /// </summary>
        internal bool ShowAnnotations = true;

        /// <summary>
        /// Track changes: show insertions and deletions
        /// // revisions
        /// </summary>
        internal bool ShowInsertionsDeletions = true;

        /// <summary>
        /// Track changes: show formatting
        /// // revisions
        /// </summary>
        internal bool ShowFormatting = true;

        /// <summary>
        /// Show ink annotations.
        /// // revisions.
        /// </summary>
        internal bool ShowInkAnnotations = true;

        /// <summary>
        /// A shortcut for all show revision properties used for older DOC files.
        /// </summary>
        internal bool ShowRevisions
        {
            get
            {
                return
                    ShowMarkup ||
                    ShowAnnotations ||
                    ShowInsertionsDeletions ||
                    ShowFormatting ||
                    ShowInkAnnotations;
            }
            set
            {
                ShowMarkup = value;
                ShowAnnotations = value;
                ShowInsertionsDeletions = value;
                ShowFormatting = value;
                ShowInkAnnotations = value;
            }
        }
        
        /// <summary>
        /// Helps prevent unintentional changes to all or part of an online form or document, as specified 
        /// (Protect Document option).
        /// </summary>
        internal DocumentProtection DocumentProtection = new DocumentProtection();

        /// <summary>
        /// Write protection settings.
        /// </summary>
        internal WriteProtection WriteProtection = new WriteProtection();

        /// <summary>
        /// Specifies the default spacing between tab stops in twips (Format Tabs option).
        /// </summary>
        internal int DefaultTabStop = DefaultTabStopDefault;

        /// <summary>
        /// Provides document hyphenation options.
        /// </summary>
        internal HyphenationOptions HyphenationOptions = new HyphenationOptions();

        /// <summary>
        /// Displays the Microsoft Office Outlook e-mail header in a document.
        /// </summary>
        internal bool ShowEnvelope;

        /// <summary>
        /// Freeze Document Layout.
        /// </summary>
        internal bool ReadingModeInkLockDown;

        /// <summary>
        /// Specifies that the date and time information shall be removed from all annotations, which are present
        /// in a document when it is saved in MS Word.
        /// </summary>
        internal bool RemoveDateAndTime;

        /// <summary>
        /// Default paragraph style for click and type.
        /// </summary>
        internal int ClickTypeParaStyleIstd = ClickTypeParaStyleIstdDefault;

        /// <summary>
        /// Default table style. 
        /// </summary>
        internal int DefaultTableStyleIstd = DefaultTableStyleIstdDefault;

        /// <summary>
        /// Creates one header or footer for even-numbered pages and a different header or footer 
        /// for odd-numbered pages (Page Setup Layout option).
        /// </summary>
        internal bool EvenAndOddHeaders;

        /// <summary>
        /// For multiple-page documents with book fold and reverse book fold printing, 
        /// sets the number of sheets per booklet (Page Setup Margin option).
        /// </summary>
        internal int BookFoldPrintingSheets;

        /// <summary>
        /// Specifies the amount of horizontal space between vertical gridlines (Drawing Grid option).
        /// </summary>
        internal int DrawingGridHorizontalSpacing = DrawingGridHorizontalSpacingDefault;

        /// <summary>
        /// Specifies the amount of vertical space between horizontal gridlines (Drawing Grid option).
        /// </summary>
        internal int DrawingGridVerticalSpacing = DrawingGridVerticalSpacingDefault;

        /// <summary>
        /// Specifies the amount of space between horizontal gridlines drawn on the screen (Drawing Grid option).
        /// </summary>
        internal int DisplayHorizontalDrawingGridEvery = DisplayHorizontalDrawingGridEveryDefault;

        /// <summary>
        /// Specifies the amount of space between vertical gridlines drawn on the screen (Drawing Grid option).
        /// </summary>
        internal int DisplayVerticalDrawingGridEvery = DisplayVerticalDrawingGridEveryDefault;

        /// <summary>
        /// If set to on overrides the settings for drawingGridHorizontalOrigin and drawingGridVerticalOrigin 
        /// and sets the upper-left corner of the document area within the margins as the grid origin 
        /// (Drawing Grid option).
        /// </summary>
        internal bool UseMarginsForDrawingGridOrigin = true;

        /// <summary>
        /// Specifies the point at the left edge of the page where you want the invisible grid to begin 
        /// (Drawing Grid option). This setting is ignored when useMarginsForDrawingGridOrigin is set to on.
        /// </summary>
        internal int DrawingGridHorizontalOrigin = DrawingGridHorizontalOriginDefault;

        /// <summary>
        /// Specifies the point at the top edge of the page where you want the invisible grid to begin 
        /// (Drawing Grid option). This setting is ignored when useMarginsForDrawingGridOrigin is set to on.
        /// </summary>
        internal int DrawingGridVerticalOrigin = DrawingGridVerticalOriginDefault;

        /// <summary>
        /// Specifies whether to turn off the gray shading on form fields.
        /// </summary>
        internal bool DoNotShadeFormData;

        /// <summary>
        /// When kerning for Latin text is turned on, also kern punctuation text (Asian Typography option).
        /// </summary>
        internal bool PunctuationKerning;

        /// <summary>
        /// Sets the blank-space compression option you want for Asian characters (Asian Typography option). 
        /// The equivalent in HTML is setting text-justify-trim on the BODY element.
        /// </summary>
        internal JustificationMode CharacterSpacingType = JustificationMode.Expand;

        /// <summary>
        /// Specifies whether to use standard characters to start and end lines of text (Asian Typography option).
        /// </summary>
        internal bool StrictFirstAndLastChars;

        /// <summary>
        /// Specified for what Language it's used NoLineBreaksAfter, NoLineBreaksBefore.
        /// </summary>
        internal Language NoLineBreaksLanguage;

        /// <summary>
        /// Specifies which characters are restricted from ending a line (Asian Typography option).
        /// </summary>
        internal string NoLineBreaksAfter;

        /// <summary>
        /// Specifies which characters are restricted from starting a line (Asian Typography option).
        /// </summary>
        internal string NoLineBreaksBefore;

        /// <summary>
        /// Specifies the encoding you want to use when you save as a Web page (Web option).
        /// // web
        /// </summary>
        internal string WebPageEncoding;

        /// <summary>
        /// Specifies whether to disable features not supported by Web browsers (Web option).
        /// // web
        /// </summary>
        internal bool OptimizeForBrowser;

        #if !CPP_DOC
        /// <summary>
        /// Specifies that the web page produced by this document should be optimized for specific output target
        /// (web option).
        /// </summary>
        /// <remarks>It might be a good idea to set <see cref="HtmlSaveOptions.ExportXhtmlTransitional"/> to
        /// true somewhere after document is loaded if the WebTarget is set to 
        /// <see cref="Settings.WebTarget.XhtmlPlusCss1"/>, or <see cref="Settings.WebTarget.XhtmlPlusCss2"/>
        /// </remarks>
        #else
        /// <summary>
        /// Specifies that the web page produced by this document should be optimized for specific output target
        /// (web option).
        /// </summary>
        /// <!-- CPP_DOC <remarks>It might be a good idea to set <see cref="HtmlSaveOptions.ExportXhtmlTransitional"/> to
        /// true somewhere after document is loaded if the WebTarget is set to 
        /// <see cref="Settings.WebTarget.XhtmlPlusCss1"/>, or <see cref="Settings.WebTarget.XhtmlPlusCss2"/>
        /// </remarks> -->
        #endif
        internal WebTarget WebTarget = WebTarget.None;
        
        /// <summary>
        /// Specifies whether to rely on Vector Markup Language (VML) for displaying graphics in browsers (Web option).
        /// // web
        /// </summary>
        internal bool RelyOnVml;

        /// <summary>
        /// Specifies whether to allow Portable Network Graphics (PNG ) as a graphic format (Web option).
        /// // web
        /// </summary>
        internal bool AllowPng;

        /// <summary>
        /// Turns off cascading style sheets (CSS) for font formatting of Web pages (Web option).
        /// // web
        /// </summary>
        internal bool DoNotRelyOnCss;

        /// <summary>
        /// When saving this file as a Web page, does not save as a single-file Web page (MHTML) (Web option).
        /// // web
        /// </summary>
        internal bool DoNotSaveWebPagesAsSingleFile;

        /// <summary>
        /// When saving as a Web page, causes all supporting files such as bullets, background textures, 
        /// and graphics to be stored in the same folder as the Web page.
        /// // web
        /// </summary>
        internal bool DoNotOrganizeInFolder;

        /// <summary>
        /// Disables long file names for Web pages, which forces a file name of no more than eight characters 
        /// (Web option).
        /// // web
        /// </summary>
        internal bool DoNotUseLongFileNames;

        /// <summary>
        /// The number of pixels per inch that you want for the display of pictures in Web pages (Web option). 
        /// The size that you select affects the size of graphics relative to the size of text on the screen.
        /// // web
        /// </summary>
        internal int PixelsPerInch = PixelsPerInchDefault;

        /// <summary>
        /// Specifies the monitor resolution (screen size) that you are optimizing your Web pages for (Web option). 
        /// The screen size that you specify can affect the size and layout of images on Web pages.
        /// // web
        /// </summary>
        internal ScreenSize TargetScreenSize = TargetScreenSizeDefault;

        /// <summary>
        /// Validates document against attached schemas (Templates and Add-Ins XML Schema option).
        /// // xml
        /// </summary>
        internal bool ValidateAgainstSchema = true;

        /// <summary>
        /// Specifies whether to allow saving as XML even if the XML is not valid 
        /// // xml
        /// </summary>
        internal bool SaveInvalidXml;

        /// <summary>
        /// Only Save Custom XML Markup.
        /// // xml
        /// </summary>
        /// <remarks>
        /// This element specifies that the contents of this document shall be saved as an XML file containing only the
        /// custom XML markup in this document in its regular form. The resulting document does not conform to ISO/IEC
        /// 29500 (i.e. this is an export-only save option for a WordprocessingML document).</remarks>
        internal bool SaveXmlDataOnly;

        /// <summary>
        /// Show Visual Indicators for Custom XML Markup Start/End Locations.
        /// // xml
        /// </summary>
        /// <remarks>
        /// This element specifies that some visual indicator shall be provided for the start and end locations of 
        /// custom XML markup present in this document, if any. If this element is omitted, then applications
        ///  should not provide any visual indicator of the locations of custom XML markup start/end tags.
        /// </remarks>
        internal bool ShowXmlTags;

        /// <summary>
        /// Specifies whether save and validate ignores all text not in leaf nodes 
        /// // xml
        /// </summary>
        internal bool IgnoreMixedContent;

        /// <summary>
        /// Turns on display of placeholder text for all empty leaf elements.
        /// // xml
        /// </summary>
        internal bool AlwaysShowPlaceholderText;

        /// <summary>
        /// Turns off wavy underline of schema violations in document.
        /// // xml
        /// </summary>
        internal bool DoNotUnderlineInvalidXml;

        /// <summary>
        /// True if need to use XSLT specified in <see cref="SaveThroughXslt"/>.
        /// // xml
        /// </summary>
        internal bool UseXsltWhenSaving;

        /// <summary>
        /// Specifies the custom transform to apply when saving document as XML (XML Save option).
        /// // xml
        /// </summary>
        internal string SaveThroughXslt;

        /// <summary>
        /// Controls how empty namespace elements that do not belong to a schema are handled. 
        /// If set to on, these elements will not be removed. If set to off, they will be removed.
        /// // xml
        /// </summary>
        internal bool AlwaysMergeEmptyNamespace;

        /// <summary>
        /// When true, causes MS Word to Remove Smart Tags When Saving.
        /// </summary>
        internal bool DoNotEmbedSmartTags;

        /// <summary>
        /// Specifies whether to save smart tag data as an XML-based property bag at the head of 
        /// the HTML page when saving as HTML.
        /// </summary>
        internal bool SaveSmartTagsAsXml;

        internal int RsidRoot;

        /// <summary>
        /// Revision state identifiers table.
        /// </summary>
        internal RsidTable Rsids = new RsidTable();

        /// <summary>
        /// Specifies the code page to use when saving as encoded text.
        /// </summary>
        internal int CodePageText;

        /// <summary>
        /// Contains document-wide footnote and endnote properties.
        /// These are actually for Word2000 and earlier files (except endnote location).
        /// For newer formats there are per-section attributes.
        /// </summary>
        internal SectPr FootnotePr = new SectPr();

        /// <summary>
        /// Contains compatibility options (that is, the user preferences entered on the Compatibility tab of the 
        /// Options dialog in Word).
        /// </summary>
        internal CompatibilityOptions CompatibilityOptions = new CompatibilityOptions();

        internal ViewOptions ViewOptions = new ViewOptions();

        /// <summary>
        /// This is only found in DOCX.
        /// </summary>
        internal ThemeFontLanguages ThemeFontLanguages = new ThemeFontLanguages();

        /// <summary>
        /// Stored document-wide math properties (DOCX only)
        /// </summary>
        internal MathProperties MathProperties = new MathProperties();

        internal XmlNamespaceCollection XmlNamespaces = new XmlNamespaceCollection();

        internal XmlSchemaReferenceCollection XmlSchemaReferences = new XmlSchemaReferenceCollection();

        /// <summary>
        /// Specifies whether footnotes, endnotes and textboxes should be included in word counting.
        /// </summary>
        internal bool DoNotIncludeSubDocsInStats;

        /// <summary>
        /// Specifies whether visual cues are not displayed around content contained in a document which is flagged as a possible spelling error.
        /// </summary>
        internal bool HideSpellingErrors;

        /// <summary>
        /// Specifies whether visual cues are not displayed around content contained in a document which is flagged as a possible grammatical error.
        /// </summary>
        internal bool HideGrammaticalErrors;

        /// <summary>
        /// Specifies that pictures in a document shall not automatically be compressed on saving in MS Word in order
        /// to reduce the overall size of the resulting document.
        /// </summary>
        internal bool DoNotAutoCompressPictures;

        /// <summary>
        /// Specifies whether fields contained in a document should be automatically updated when the document is
        /// opened by MS Word.
        /// </summary>
        internal bool UpdateFields;

        /// <summary>
        /// Specifies that contents of a document can be upgraded and that the resulting document shall not have
        /// its functionality limited to only those functions compatible with earlier word processing applications.
        /// </summary>
        /// <dev>
        /// ISO 29500 says that if an application does not know how to upgrade a document, this element should be
        /// ignored and persisted.
        /// </dev>
        internal bool ForceUpgrade;

        /// <summary>
        /// Specifies how the datapoint properties ([MS-ODRAWXML] section 2.8.3.1) and datalabels ([MS-ODRAWXML]
        /// section 2.2.1.2) in all charts in a document behave.
        /// </summary>
        internal bool ChartTrackingRefBased;

        /// <summary>
        /// Specifies a unique identifier for a set of documents derived from a common source.
        /// </summary>
        internal string DocumentSetId;

        /// <summary>
        /// Specifies that when true, the cropped-out areas of the images are not to be saved in MS Word.
        /// </summary>
        internal bool DiscardImageEditingData;

        /// <summary>
        /// Specifies the resolution in dots per inch (DPI), at which images in a document will be saved in MS Word.
        /// </summary>
        internal int DefaultImageDpi;

        /// <summary>
        /// Specifies an arbitrary identifier for the context of the paragraph identifiers in the document.
        /// </summary>
        internal string DocId;

        /// <summary>
        /// Preferred editing language specified via <see cref="Loading.LanguagePreferences"/>.
        /// </summary>
        /// <remarks>
        /// This is actually an MS Word application setting, not a document property.
        /// However it is needed in the document model in order to imitate MS Word layout for Asian languages.
        /// </remarks>
        internal Language PreferredEditingLanguage;

        // *** Below are DOP Only Properties. I don't know where they are in WordML.
        #region Properties available in DOC only.

        /// <summary>
        /// What outline levels to show. Not in WordML, but present and used in DOP.
        /// </summary>
        internal int ShowOutlineLevels = (int)OutlineLevel.BodyText;

        /// <summary>
        /// Display hidden document contents. Seems to be available in DOC only. 
        /// </summary>
        internal bool ShowHidden;

        /// <summary>
        /// Display field results when set, otherwise display field codes. 
        /// Seems to be available in DOC only.
        /// </summary>
        internal bool ShowFieldResults;

        /// <summary>
        /// Determines if revisions are printed. Not a WordML property. Seems to be obsolete DOC only.
        /// </summary>
        internal bool PrintRevisions;

        /// <summary>
        /// Specifies that this file is based upon an HTML file. Not a WordML property.
        /// </summary>
        internal bool HtmlDoc;

        /// <summary>
        /// True when widow control is enabled. Not a WordML property.
        /// </summary>
        internal bool WidowControl = true;

        /// <summary>
        /// Show Asian grid. Not in WordML.
        /// </summary>
        internal bool ShowGrid;

        /// <summary>
        /// Hide the version created for autoversion. Not in WordML.
        /// </summary>
        internal bool HideLastVersion;

        /// <summary>
        /// Versioning is turned on. Not in WordML.
        /// </summary>
        internal bool HasVersions;

        /// <summary>
        /// Autoversioning is enabled. Not in WordML.
        /// </summary>
        internal bool AutoVersion;

        /// <summary>
        /// This is not really a DOP property. It is just set when reading DOC so the user
        /// can query how many versions were in the file. "Were" because we don't support
        /// versions and on save there will be no versions saved.
        /// </summary>
        internal int VersionsCount;

        /// <summary>
        /// Fit text should be ignored. Can be true only for DOC format.
        /// </summary>
        internal bool NoFitText;

#endregion Properties available in DOC only.


        // RK: *** Default values constants.
        // Note: In Microsoft Office Word 2007 the default sorting order is specified by the \spriorityN control word. 
        // RK OOXML spec is not correct. RTF 1.9.1 is correct.

        /// <summary>
        /// 0    Specifies that visible styles should be sorted by their names.
        /// </summary>
        [CppConstexpr]
        internal const int StylePaneSortMethodNames = 0;
        
        /// <summary>
        /// 1 - Specifies that visible styles should be sorted by the default sorting of the host application.
        /// </summary>
        [CppConstexpr]
        internal const int StylePaneSortMethodDefault = 1;
        
        /// <summary>
        /// 2    Specifies that visible styles should be sorted by the font for which they apply.
        /// </summary>
        [CppConstexpr]
        internal const int StylePaneSortMethodFont = 2;
        
        /// <summary>
        /// 3    Specifies that visible styles should be sorted by the style on which they are based.
        /// </summary>
        [CppConstexpr]
        internal const int StylePaneSortMethodStyle = 3;
        
        /// <summary>
        /// 4    Specifies that visible styles should be sorted by their style types (e.g., character, linked, paragraph).
        /// Any other value    Undefined. Shall not be used.
        /// </summary>
        [CppConstexpr]
        internal const int StylePaneSortMethodType = 4;
            
        [CppConstexpr]
        internal const int DefaultTabStopDefault = 720;
        [CppConstexpr]
        internal const int PixelsPerInchDefault = 96;
        [CppConstexpr]
        internal const ScreenSize TargetScreenSizeDefault = ScreenSize.Size800x600;
        [CppConstexpr]
        internal const int DrawingGridHorizontalSpacingDefault = 180;
        [CppConstexpr]
        internal const int DrawingGridVerticalSpacingDefault = 180;
        [CppConstexpr]
        internal const int DisplayHorizontalDrawingGridEveryDefault = 1;
        [CppConstexpr]
        internal const int DisplayVerticalDrawingGridEveryDefault = 1;
        [CppConstexpr]
        internal const int DrawingGridHorizontalOriginDefault = 1800;
        [CppConstexpr]
        internal const int DrawingGridVerticalOriginDefault = 1440;
        [CppConstexpr]
        internal const int ClickTypeParaStyleIstdDefault = StyleIndex.Normal;
        [CppConstexpr]
        internal const int DefaultTableStyleIstdDefault = StyleIndex.Nil;
    }
}
