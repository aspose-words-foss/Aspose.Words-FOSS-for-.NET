// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/08/2012 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Warning messages.
    /// </summary>
    internal static class WarningStrings
    {

        internal const string AbsolutePositionTabNotSupported = "Absolute position tabs are not supported in {0}, using regular tab character.";

        internal const string ReservedStyleIndexUsed = "Style index reserved for built-in style '{0}' is used in style '{1}'.";
        internal const string CannotCopyStyle = "Cannot copy style {0} because it matches a style of a different type.";

        internal const string CustomXmlError = "Error is occurred while parsing Custom XML data.";

        internal const string InvalidHLinks = "Invalid _PID_HLINKS property, ignored.";

        internal const string InvalidDigitalSignature = "Digital signature is invalid, document content was tampered.";

        internal const string EmptyTemplatePath = "Attached template path is empty, styles were not updated.";

        internal const string FailedTemplateLoad = "Attached template could not be loaded at {0}.";

        #region Validator strings

        internal const string ListValidatorNonUniqueDefinition = "Non-unique list definition removed.";

        internal const string FieldValidatorFieldNone = "Fields with FieldType.FieldNone are not supported by Aspose.Words.";

        internal const string FieldValidatorNoStartEnd = "FieldSeparator without corresponding FieldStart and FieldEnd was removed.";

        internal const string FieldValidatorNoStart = "FieldSeparator without corresponding FieldStart was removed.";

        internal const string FieldValidatorRemovedInvalidFormField = "Invalid form field was removed.";

        internal const string FieldValidatorDifferentStoriesField = "Invalid field is located in different document stories, removed.";

        internal const string DocumentValidatorDml = "DrawingML is not supported in {0} format and will be converted to shape.";

        internal const string DocumentValidatorVmlInIsoStrict =
            "VML shape is not supported in Strict Open XML Document format and will be converted to DrawingML.";

        internal const string DocumentValidatorOfficeMath = "Office math is not supported in {0} format and will be converted to shape.";

        internal const string DocumentValidatorSdtXmlMapping = "Data binding failed for StructuredDocumentTag.";

        internal const string DocumentValidatorSdtPlaceholderUnused = "One or more unused StructuredDocumentTag placeholder blocks were removed from Document Glossary.";

        internal const string DocumentValidatorWrongStory = "{0} is not in the main text story and shall be removed.";

        internal const string DocumentValidatorAllocatedCommandInvalid = "Invalid allocated command changed to ApplyStyle Normal.";

        internal const string DocumentValidatorReplacedDml = "DrawingML shape is replaced with fallback Shape, some formatting might be lost.";

        internal const string DocumentValidatorOdtChartToDml = "Open Document Format chart will be converted to DrawingML.";

        internal const string Iso29500CompliancePageBorderArtChanged = "Replaced PageBorderArt value with ISO29500-compliant.";

        internal const string DocumentIsDigitallySigned = "Document is digitally signed, export of digital signatures is not supported by Aspose.Words.";

        internal const string CommentValidatorNoStart = "CommentEnd without corresponding CommentStart was removed.";

        internal const string CommentValidatorNoEnd = "CommentStart without corresponding CommentEnd was removed.";

        internal const string CommentValidatorEmptyRange = "Empty comment range was removed.";

        internal const string CommentAboveCommentRange = "Comment range was removed because located below its Comment node.";

        internal const string EditableRangeValidatorNoStart = "EditableRangeEnd without corresponding EditableRangeStart was removed.";

        internal const string EditableRangeValidatorNoEnd = "EditableRangeStart without corresponding EditableRangeEnd was removed.";

        internal const string CommentValidatorNoContent = "CommentRange without corresponding Comment was removed.";

        internal const string BookmarkValidatorTooLongName = "Bookmark name exceeded maximum allowed length was truncated.";

        internal const string BookmarkValidatorDuplicateName = "Bookmark with duplicated name '{0}' was removed.";

        internal const string BookmarkValidatorNoStart = "Bookmark '{0}' without corresponding BookmarkStart was removed.";

        internal const string BookmarkValidatorNoEnd = "Bookmark '{0}' without corresponding BookmarkEnd was removed.";

        internal const string TableValidatorEmptyTable = "Empty table was removed.";

        internal const string TableValidatorTableResized = "Table width exceed maximum allowed, table was resized.";

        internal const string TableValidatorEmptyRow = "Empty table row was removed.";

        internal const string TableValidatorInvalidCellWidth = "Invalid cell width, table layout updated.";

        internal const string TableValidatorWordCellLimitExceed = "Cell count per row must not exceed 63, document can have problem while opened in MS Word.";

        internal const string InvalidOfficeMathXml = "Aspose.Words failed to load OfficeMath XML for equation.";

        internal const string InvalidShapeField = "Invalid SHAPE field could not be parsed.";

        internal const string InvalidTextboxLink = "Invalid textbox link for shape {0}, removed.";

        internal const string InvalidConnectorRule = "Invalid connector rule for shape {0}, removed.";

        internal const string RowHeightExceedLimit = "Table row height exceed maximum allowed, corrected.";

        internal const string ListCountExceedLimit = "List count exceed maximum allowed, document can have problem while opened in MS Word.";

        internal const string HintToRemoveCompatibilityMode = "The OoxmlSaveOptions.Compliance property should be " +
            "set to OoxmlCompliance.Iso29500_2008_Transitional or higher to remove the Compatibility Mode ribbon.";

        internal const string NonUniqueSignatureLineId = "The value {0} of SignatureLine.Id is not unique and was replaced with {1}.";

        internal const string InvalidCompatibilityForLayoutInCell =
            "Value of the IsLayoutInCell property has no effect for DML shapes if a document is optimized for the version 2013 or higher.";
        internal const string TaskPaneAddinsWillBeSkippedOnSaving =
            "Taskpane add-ins can only be saved to the Office Open XML file formats and will be skipped when saving to other formats.";
        internal const string TaskPaneAddinHasUnexpectedReferenceType =
            "Reference to web extension can not be external.";

        internal const string TextPlainTextDecorationInVml =
            "Text underline property for VML WordArt shapes is not supported by MS Word.";

        #endregion

        #region Binary DOC strings

        internal const string InvalidFontTableFc = "Invalid font table reference, ignored.";

        internal const string UnsupportedTableSprm = "Table formatting modifier 0x{0:x4} is not supported for DOC format by Aspose.Words.";

        internal const string InvalidTableSprm = "Invalid table formatting modifier 0x{0:x4}, ignored.";

        internal const string UnsupportedParaSprm = "Paragraph formatting modifier 0x{0:x4} is not supported for DOC format by Aspose.Words.";

        internal const string UnsupportedRunSprm = "Text formatting modifier 0x{0:x4} is not supported for DOC format by Aspose.Words.";

        internal const string UnsupportedSectSprm = "Section formatting modifier 0x{0:x4} is not supported for DOC format by Aspose.Words.";

        /// <summary>
        /// Specifies warning message for unknown/undocumented SPRMs.
        /// </summary>
        internal const string UnknownSprm = "Unknown formatting modifier 0x{0:x4} occurred while reading DOC file.";

        /// <summary>
        /// TCellShdStyle outside table style definition resilience.
        /// </summary>
        internal const string UnexpectedCellShdStyle = "Unexpected shading information occurred outside table style definition while reading DOC file.";

        internal const string InvalidAssociatedString = "Invalid associated string requested for 0x{0:x4}, replaced with empty string.";

        internal const string CellPropertiesMissed = "Cell properties is missing.";

        internal const string InvalidTablePadding = "Invalid table padding spacing read, replaced with defaults.";

        internal const string FieldTypeReplaced = "Invalid field type specified while reading DOC file.";

        internal const string InvalidCell = "EndStory occurred in a cell, row will be deleted.";

        internal const string UnknownOfficeArtProperty = "Unknown OfficeArt property found 0x{0:x4}, ignored.";

        internal const string MetaStreamMissed = "OLE 1.0 object stream 'META' is missing, possible image size problem.";

        internal const string SectionBreakNotInMainDocument = "Section breaks allowed only inside main document.";

        internal const string SectionBreakInTable = "Section breaks are not allowed inside tables by Aspose.Words.";

        internal const string FileCorruptedAt = "File is corrupted, data loss at FC: 0x{0:x4}.";

        internal const string OleObjectDataMissed = "Embedded OLE object data is missed for ObjectID: 0x{0:x4}.";

        internal const string InvalidPicfAt = "Invalid PICF structure at 0x{0:x4}, shape is ignored.";

        internal const string InvalidPicf = "Invalid PICF structure, shape is ignored.";

        internal const string ShapeFspaMissed = "Couldn't get FSPA structure for shape, shape is ignored.";

        internal const string ShapeOfficeArtMissed = "Couldn't get OfficeArt record for shape, shape is ignored.";

        internal const string InvalidAnnotationAt = "Invalid annotation reference at local CP: 0x{0:x4}.";

        internal const string EmptyStyleSheet = "Document contains empty stylesheet.";

        internal const string InvalidStyleSheet = "Invalid fixed styles count, ignore whole stylesheet.";

        internal const string InvalidStyle = "Style definition for Istd 0x{0:x2} is corrupt, ignored.";

        internal const string InvalidStyleIgnoreRest = "Style definition for Istd 0x{0:x2} is corrupt, rest of stylesheet is ignored.";

        internal const string NormalBasedOn = "Normal style is based on other style which is not allowed, corrected.";

        internal const string InvalidFontName = "Invalid font name, replaced with '{0}'.";

        internal const string InvalidIndent = "Invalid indent value {0} read, ignore.";

        internal const string InvalidUnderline = "Invalid underline type read, used defaults.";

        internal const string InvalidTabStopAlignment = "Invalid tab stop alignment type read, used defaults.";

        internal const string ObjectPoolMissed = "Document 'ObjectPool' stream is missed, OLE objects will be lost.";

        internal const string LoadOverride = "LoadOverride flag is not supported in DOC format by Aspose.Words.";

        internal const string FarEastInstallationLid = "Installation language is FarEast language.";

        internal const string Word60Document = "Document is a Word 6.0/95 document.";

        internal const string PrinterDriverData = "Printer driver information is not supported by Aspose.Words.";

        internal const string LastSelectionData = "Last selection information is not supported by Aspose.Words.";

        internal const string SpellChecker = "Spell checker information is not supported by Aspose.Words.";

        internal const string GrammarChecker = "Grammar checker information is not supported by Aspose.Words.";

        internal const string RouteSlip = "RouteSlip information is DOC format only feature and lost if saved to another format.";

        internal const string SaveHistory = "Word97/2000 save history is not supported by Aspose.Words.";

        internal const string DocumentVersioning = "Document versioning is not supported by Aspose.Words.";

        internal const string DeprecatedOcxData = "Pre-Word2003 additional information for ActiveX is not supported by Aspose.Words.";

        internal const string ThreadingData = "Threading information is not supported by Aspose.Words.";

        internal const string HybridListsFormats = "Hybrid multilevel lists numbering formats is not supported by Aspose.Words.";

        internal const string MsoEnvelope = "MSO Envelope information is not supported by Aspose.Words.";

        internal const string LanguageAutoDetectData = "Language auto-detect data is not supported by Aspose.Words.";

        internal const string GrammarCheckerOptions =  "Grammar checker options is not supported is not supported by Aspose.Words.";

        internal const string GrammarCheckerCookies = "Grammar checker cookies is not supported is not supported by Aspose.Words.";

        internal const string TextFrameworkData = "Microsoft Text Framework data is not supported by Aspose.Words.";

        internal const string SmartTagRecognizerData = "Smart tag recognizer data is not supported by Aspose.Words.";

        internal const string RepairBookmarks = "Repair bookmarks are not supported by Aspose.Words.";

        internal const string ColorMru = "ColorMru OfficeArt record is not supported, ignored.";

        internal const string InvalidNumberRevision = "Invalid number revision found, removed.";

        internal const string InvalidCnfc = "Unknown conditional formatting read 0x{0:x4}.";

        internal const string InvalidParagraphStyleIstd = "Paragraph style '{0}' has incorrect Istd set.";

        internal const string InvalidStyleOutlineLevel = "Style '{0}' has incorrect OutlineLevel, removed.";

        internal const string InvalidOfficeArtLength = "Invalid OfficeArt record length, corrected.";

        internal const string UnknownOfficeArtType = "Unknown OfficeArt record 0x{0:x4} found, ignored.";

        internal const string SectPropertiesMissed = "Section properties is missed, used defaults.";

        internal const string ColumnWidthMissed = "Column width is missed, used default value 17.09 cm.";

        internal const string ExtraColumnDefinitions = "Extra column definitions found, removed.";

        internal const string SmartTagEndMissed = "Smart tag '{0}' ends after paragraph mark.";

        internal const string EmptyPropertyName = "Document property without name found, ignored.";

        internal const string EmbeddedOleWithoutData = "Embedded OLE object without data found, converted to image.";

        internal const string InvalidBoolEx = "Invalid BoolEx value occurred, ignored.";

        internal const string FieldStartMissed = "Invalid field separator without field start occurred.";

        internal const string InvalidListNumberFormat = "Invalid number format definition, skipped.";

        internal const string InvalidListNumberStyle = "Invalid number style definition, skipped.";

        internal const string InvalidListLevelAlignment = "Invalid level alignment definition, skipped.";

        internal const string InvalidListRestartAfterLevel = "Invalid level restart definition, skipped.";

        internal const string InvalidListTrailingCharacter = "Invalid trailing character definition, skipped.";

        internal const string InvalidListLegacySpace = "Invalid legacy space definition, skipped.";

        internal const string ListNumberFormatTooLong = "Number format definition is too long, truncated.";

        internal const string InvalidListLevel = "List level in list definition 0x{0:x} is corrupted.";

        internal const string AutoNumLinkedList = "Lists linked to AutoNum fields are not supported by Aspose.Words.";

        internal const string OfficeArtReadFail = "Read wrong number of bytes from OfficeArt record, ignored.";

        internal const string StyleInvalidReference = "Style '{0}' refers to non-existent style, removed invalid reference.";

        internal const string BadNestingLevel = "Table end is at an incorrect nesting level.";

        internal const string UnexpectedEndOfTable = "Unexpected end of table, ignored.";

        internal const string UnexpectedEndOfRow = "Unexpected end of row, ignored.";

        internal const string TooLongFollowings = "Too long following chars count, used defaults.";

        internal const string TooLongLeadings = "Too long leading chars count, used defaults.";

        internal const string InvalidRevisionNumber = "Invalid revision number.";

        internal const string InvalidDateTimeProperty = "Invalid DateTime property, ignored.";

        internal const string UnsupportedDocumentProperty = "Unsupported document property 0x{0:x4}, ignored.";

        internal const string UnsupportedPowerPointProperty = "Unsupported PowerPoint property 0x{0:x4}, ignored.";

        internal const string UnacceptableMarkup = "Node is unable to accept markup, markup ignored.";

        internal const string InvalidTabStopPosition = "Invalid tab stop position {0}, ignored.";

        internal const string InvalidLineStyle = "Invalid line style value is read for border while reading DOC file.";

        internal const string SdtNotSupported = "Structured Document Tags feature is not supported in DOC.";

        internal const string RepeatingSectionXPathLastIndexerRemoved = "An indexer of last node selector in RepeatingSection SDT XPath is removed.";

        internal const string GlossaryDocumentNotSupported = "Glossary Document feature is not supported in DOC.";

        internal const string TableStyleExpanded = "Table style 0x{0:x2} can not be preserved and will be converted to direct formatting.";

        internal const string OoxmlControlNotSupported = "OOXML control is not supported in DOC and will be converted to image.";

        internal const string AutoVersionOff = "Word2003 versioning feature is not supported, AutoVersion option turned off";

        internal const string UnknownProgId = "Unknown ProgId value '{0}'. This might cause inaccessible OLE embedding.";

        internal const string InvalidStyleType = "Invalid style type value 0x{0:x2}, style is ignored.";

        internal const string InvalidSectionBreak = "Invalid section break character at CP: {0}.";

        internal const string InvalidEmbeddedFontIndex = "Invalid embedded font index {0}.";

        internal const string InvalidNumberRevisionLength = "Invalid number revision length.";

        internal const string TooManyTextboxes = "Too many textboxes, document can have problem while opened in MS Word.";

        internal const string InvalidListDefReference = "List {0} refers to invalid list definition, removed.";

        internal const string InvalidListDef = "Invalid list definition at index {0}, removed.";

        internal const string InvalidStyleExtraBytes = "Invalid extra data in style '{0}', ignored.";

        internal const string ThumbnailPropertyNotSupported = "Thumbnail property is not supported in DOC format by Aspose.Words.";

        internal const string SmartTagPropertyIndexOutOfRange = "Invalid smart tag at index {0}, removed.";

        internal const string InvalidTabStopSprm = "Invalid tab stop data, ignored.";

        internal const string TooManyBookmarks = "Maximum bookmarks count supported by binary DOC file format is reached.";

        internal const string IllegalBookmarkNamePictureBullets = "Bookmark name _PictureBullets is reserved in DOC format, bookmark removed.";

        internal const string DuplicatedOfficeArtProperty = "OfficeArt property with duplicated Id {0} was read, ignored.";

        internal const string InvalidPlcfBtePapx = "Invalid PlcfBtePapx structure, paragraph properties are lost.";

        internal const string InvalidPlcfBteChpx = "Invalid PlcfBteChpx structure, character properties are lost.";

        internal const string InvalidParentComment = "Unable to write parent reference for comment id: {0}.";

        internal const string TextboxVerticalAnchor2007Only = "Textbox vertical alignment is compatible only with Word 2007.";

        internal const string TextboxVerticalAnchorNotSupported = "Textbox vertical alignment is not supported in save format.";

        #endregion

        internal const string UnexpectedElement = "Unexpected element '{0}' is occurred while reading document, ignored.";
        internal const string UnexpectedAttribute = "Unexpected attribute '{0}' is occurred while reading document, ignored.";
        internal const string InvalidOnOffValue = "Unknown value '{0}' for '{1}' element.";
        internal const string InvalidValue = "Invalid value '{0}' for '{1}' element.";
        internal const string UnexpectedTagOrAttribute = "Tag or attribute with name '{0}' is unexpected.";
        internal const string NotSupportedTag = "Tag with name '{0}' is not supported.";
        internal const string NotSupportedAttribute = "Attribute with name '{0}' is not supported.";

        internal const string UnexpectedSectionBreak = "Unexpected section break, ignored.";

        internal const string JpegConvertedToPng = "JPEG images with transparent areas are not supported. " +
            "The image is converted to PNG.";

        internal const string ShapeCannotBeRendered = "Shape has invalid size or other parameters and cannot be rendered.";

        internal const string RtfEmptyThemeData = "Empty themedata, ignored.";

        internal const string FootnoteWithoutReferenceMark = "Custom footnote mark '{0}' without reference mark, ignored.";
        internal const string FootnoteWithEmptyReferenceMark = "Custom footnote mark '{0}' with empty reference mark, ignored.";
        internal const string FootnoteWithSeveralReferences = "There are several references to the footnote with ID {0}.";

        internal const string CorruptedGfxData = "GFX data is corrupted, ignored.";

        internal const string ImageReplacedWithPlaceholder = "Image has been replaced with a placeholder.";

        // Below warnings are not tested (there is no appropriate files).

        internal const string InvalidBlipId = "Invalid OfficeArt blip reference, ignored.";
        internal const string DeletedPspl = "DeletedPspl OfficeArt record is not supported, ignored.";
        internal const string RevisionsAuthorSelection = "Saved revision author selection is not supported, ignored.";
        internal const string UnsupportedOfficeArtProperty = "Unsupported OfficeArt property 0x{0:x4}, ignored.";
        internal const string InvalidLinkToFile = "Invalid link to file, moved to image description.";
        internal const string InvalidOfficeArt = "Invalid OfficeArt record found, ignored.";
        internal const string AutoCaptions = "Captions/AutoCaptions are not supported, ignored.";
        internal const string AutoSummaryPriorities = "Priority of a text ranges for AutoSummary is not supported, ignored.";
        internal const string ConsistencyCheckerBookmarks = "Consistency checker bookmarks are not supported, ignored.";
        internal const string EndStoryInCell = "EndStory occurred in row, row will be deleted.";
        internal const string InvalidFieldChar = "Field char is invalid at FC: 0x{0:x4}.";
        internal const string UnableToGetParent = "Unable to get common parent range, markup ignored.";
        internal const string ZeroShapeSize = "Image has zero dimensions, used defaults.";
        internal const string UnrecognizedBinaryData = "Unrecognized binary data.";
        internal const string ViolationI4I = @"Violation of i4i's patent #5,787,449. Custom xml will no longer be supported.";

        internal const string RemovedStyleListReference = "Removed numbering reference for style '{0}'.";

        internal const string NotSupportedAlgorithm = @"Unsupported AlgorithmId value '{0}'.";

        internal const string NotSupportedByIsoStrict =
            "The element '{0}' is not supported by Strict Open XML Document format, discarded.";
        internal const string NotSupportedNumberStyle = "The number style {0} is not supported by the {1} file format.";

        internal const string NonGuidXmlPartId = "The Id property of a custom XML part has a non-GUID value: '{0}'. " +
            "The latest versions of Microsoft Word may fail on opening this document.";
        internal const string ChangedXmlPartId = "The Id property of a custom XML part has been changed from '{0}' " +
            "to '{1}' to comply with OOXML specification requirements.";

        internal const string FontsNotEmbeddedDueToEmbeddingDisabled =
            @"Fonts are not embedded, despite either 'EmbedSystemFonts' or 'SaveSubsetFonts' is set to 'true' due to 'EmbedTrueTypeFonts' option is set to 'false'.";
        internal const string FontsEmbeddingNotSupportedForFileFormat = @"Fonts are not embedded, as it is not supported for {0} file format.";
        internal const string FontNotEmbeddedDueToSizeLimit = @"Font '{0}' is not embedded due to exceeding the maximum allowed size of {1:N0} bytes.";
        internal const string FontSubsetEmbeddedDueToSizeLimit = @"Font subset embedded for font '{0}' due to exceeding the maximum allowed size of {1:N0} bytes.";
        internal const string InvalidEmbeddedFontStyleName = @"Invalid embedded font style name: {0}.";

        internal const string ShapeIgnored = @"Shape is ignored due to: {0}";

        internal const string NotSupportedEntityPicker =
            "Entity Picker element of Structured Document Tag is not supported by the {0} file format.";
        internal const string NotSupportedWebExtensions =
            "Web Extensions are not supported by the {0} file format.";
        internal const string NotSupportedSdtAppearance =
            "Appearance element of Structured Document Tag is not supported by the {0} file format.";
        internal const string NotSupportedCollapsed =
            "'Collapsed as default' property is not supported by the {0} file format.";
        internal const string NotSupportedFootnoteColumns =
            "Setting footnote columns is not supported by the {0} file format.";
        internal const string NotSupportedPeoplePart =
            "Contact information for authors of comments and revisions is not supported in the {0} file format.";
        internal const string NotSupportedSetting =
            "The '{0}' document setting is not supported by the {0} file format.";
        internal const string NotSupportedSettingByAw =
            "Aspose.Words does not support the '{0}' document setting for the {0} file format.";
        internal const string NotSupportedTableTitle =
            "Table title is not supported by the {0} file format.";
        internal const string NotSupportedTableDescription =
            "Table description is not supported by the {0} file format.";

        internal const string UnexpectedBreak = "Unexpected break, ignored.";
        internal const string UnexpectedBase64 = "Unexpected base-64 string used for protection document, ignored.";

        internal const string CellRevisionLost = @"Table cell edit revisions are lost.";

        internal const string UnsupportedChartExRendering =
            "New chart types of MS Word 2016 are not supported: the chart will be rendered with using its fallback shape.";

        internal const string UnsupportedContentPartRendering = "Content part is not supported and will be replaced with its fallback shape.";

        internal const string UnsupportedShapeMessage = "Some of DrawingML shape features are not supported upon rendering." +
                                        "DrawingML shape will be replaced with its fallback VML shape.";

        internal const string CannotDefineNumberingFormatting =
            "Cannot preserve list number/symbol formatting on saving to ODT file format.";

        internal const string Ecma376NotSupportedXmlMapping =
            "The defined XML mapping is not supported by the ECMA-376 OOXML format.";

        internal const string FootnotePartMissing = "Footnote/endnote document part is missing.";

        internal const string ThemePartMissing = "Theme part '{0}' is missing.";

        internal const string MoveStartMissed = "The start element of moved content is missed. End element will be ignored.";

        internal const string UnsupportedDmlTextAlignment =
            "The alignment {0} is not supported by drawing paragraphs. It has been replaced with the {1} value.";

        internal const string UnexpectedStoryForShape = "Floating shape in unexpected story";

        internal const string UnsupportedVmlColor = "Unsupported color type encountered during {0} export: {1}.";

        internal const string InvalidStyleIstd = "Style '{0}' has style index unsupported by target format.";

        internal const string LoadingProgressUnsupported = "Loading progress is not supported for the '{0}' format.";

        internal const string SavingProgressUnsupported = "Saving progress is not supported for the '{0}' format.";

        internal const string PicturePropertiesIgnore = "Picture properties in an unexpected place, ignored";

        internal const string CannotSetYValue = "Please make sure you are using correct chart type. " +
            "There is no way to set Y value for this chart type.";

        internal const string CannotSetBubbleSize = "Please make sure you are using correct chart type. " +
            "There is no way to set bubble size for non-bubble chart.";

        internal const string ChartDataRegeneration =
            "The embedded Excel workbook is regenerated after updating chart data. " +
            "Any additional data not used for the chart and formatting contained in the workbook will be lost.";

        internal const string CellLimitExcess =
            "The number of cells in a row exceeds 63. " +
            "The cell contents are merged with the last cell in the row.";

        internal const string OptimizedForWord2013 = "The document has been optimized for MS Word 2013.";

        internal const string DocVersionIncreasedTo2013 =
            "The document version has been increased to 2013 to support this feature.";

        internal const string HeaderFooterPartMissing = "Header/Footer relationship part is missing.";

        #region  Html5 strings
        internal const string Html5EmptyTitle = "Document title is empty that doesn't conform to HTML5 standard (element title must not be empty).";
        internal const string Html5ImgUsemapInsideAnchor = "The element img with the attribute usemap appears as a descendant of the a element that doesn't conform to HTML5 standard.";
        internal const string Html5IFrameNoresizeIgnored = "Frame.NoResize property doesn't have a counterpart in iframe HTML element and has been ignored.";
        internal const string Html5IFrameScrollTypeIgnored = "Frame.ScrollType property doesn't have a counterpart in iframe HTML5 element and has been ignored.";
        #endregion

        #region  ODT strings
        internal const string OdtNotSupportedNumberFormatLenght = "Aspose.Word model only support 30 symbols or less in NumberFormat. Excessive symbols will be truncated.";

        internal const string OdtNotSupportedMetaEmptyName = "Empty user-defined property name, ignored.";
        #endregion

        #region  linked textbox strings
        internal const string LinkTextboxInvalidTarget = "Invalid target textbox.";
        internal const string LinkTextboxSameDocument = "Parent document of the linked textboxes must be the same.";
        internal const string LinkTextboxBothShapesText = "Please, make sure both shapes are able to store a text.";
        internal const string LinkTextboxEmptyTarget = "Target textbox must be empty.";
        internal const string LinkTextboxSameMarkup = "Source and target textboxes must use the same markup language (DML/VML).";
        internal const string LinkTextboxTargetIsLinked = "Target textbox must not be a part of another linked chain.";
        internal const string LinkTextboxHasLink = "This textbox already has a link.";
        internal const string LinkTextboxDifferentStoryTypes = "Different story types of textboxes.";
        #endregion

        #region Markdown strings
        internal const string MarkdownFormattingLost = "The ({0}) cannot be properly written into Markdown.";
        #endregion

        #region Xlsx strings
        internal const string UnsupportedComments = "Comments are not supported when saving as {0}.";

        internal const string UnsupportedFootnotes = "Footnotes are not supported when saving as {0}.";

        internal const string UnsupportedSpecialChars = "Special chars are not supported when saving as {0}.";

        internal const string UnsupportedSmartTags = "Smart tags are not supported when saving as {0}.";

        internal const string UnsupportedSdts = "Structured document tags are not supported when saving as {0}.";

        internal const string XlsxUnsupportedFormFields = "Form fields are not supported when saving as XLSX.";

        internal const string XlsxLostShapeFormat = "Shape format may lost when saving as XLSX.";

        internal const string XlsxUnsupportedTableShapes = "Shapes in tables are not supported when saving as XLSX.";
        #endregion

        #region Docling JSON strings
        internal const string DoclingOmittedHeadersFooters = "Headers and footers are omitted when saving as Docling JSON.";

        internal const string DoclingUnsupportedShapeType = "This shape type is not supported when saving as Docling JSON.";
        #endregion
    }
}
