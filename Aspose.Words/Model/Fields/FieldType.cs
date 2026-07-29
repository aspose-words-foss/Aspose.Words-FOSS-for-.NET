// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/04/2006 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Fields
{
    // NOTE: FieldUtil.GetFieldType() should be updated after each change of this enum.

    /// <summary>
    /// Specifies Microsoft Word field types.
    /// </summary>
    [CppEnumEnableMetadata]
    public enum FieldType
    {
        /// <summary>
        /// Field type is not specified or unknown.
        /// </summary>
        FieldNone = 0,
        /// <summary>
        /// Specifies that the field was unable to be parsed.
        /// </summary>
        FieldCannotParse = 1,
        /// <summary>
        /// Specifies the ADDIN field.
        /// </summary>
        FieldAddin = 81,
        /// <summary>
        /// Specifies the ADDRESSBLOCK field.
        /// </summary>
        FieldAddressBlock = 93,
        /// <summary>
        /// Specifies the ADVANCE field.
        /// </summary>
        FieldAdvance = 84,
        /// <summary>
        /// Specifies the ASK field.
        /// </summary>
        FieldAsk = 38,
        /// <summary>
        /// Specifies the AUTHOR field.
        /// </summary>
        FieldAuthor = 17,
        /// <summary>
        /// Specifies the AUTONUM field.
        /// </summary>
        FieldAutoNum = 54,
        /// <summary>
        /// Specifies the AUTONUMLGL field.
        /// </summary>
        FieldAutoNumLegal = 53,
        /// <summary>
        /// Specifies the AUTONUMOUT field.
        /// </summary>
        FieldAutoNumOutline = 52,
        /// <summary>
        /// Specifies the AUTOTEXT field.
        /// </summary>
        FieldAutoText = 79,
        /// <summary>
        /// Specifies the AUTOTEXTLIST field.
        /// </summary>
        FieldAutoTextList = 89,
        /// <summary>
        /// Specifies the BARCODE field.
        /// </summary>
        FieldBarcode = 63,
        /// <summary>
        /// Specifies the BIBLIOGRAPHY field.
        /// </summary>
        FieldBibliography = 100500,
        /// <summary>
        /// Specifies the BIDIOUTLINE field.
        /// </summary>
        FieldBidiOutline = 92,
        /// <summary>
        /// Specifies the CITATION field.
        /// </summary>
        FieldCitation = 1980,
        /// <summary>
        /// Specifies the COMMENTS field.
        /// </summary>
        FieldComments = 19,
        /// <summary>
        /// Specifies the COMPARE field.
        /// </summary>
        FieldCompare = 80,
        /// <summary>
        /// Specifies the CREATEDATE field.
        /// </summary>
        FieldCreateDate = 21,
        /// <summary>
        /// Specifies the DATA field.
        /// </summary>
        FieldData = 40,
        /// <summary>
        /// Specifies the DATABASE field.
        /// </summary>
        FieldDatabase = 78,
        /// <summary>
        /// Specifies the DATE field.
        /// </summary>
        FieldDate = 31,
        /// <summary>
        /// Specifies the DDE field.
        /// </summary>
        FieldDDE = 45,
        /// <summary>
        /// Specifies the DISPLAYBARCODE field.
        /// </summary>
        FieldDisplayBarcode = 6301,
        /// <summary>
        /// Specifies the MERGEBARCODE field.
        /// </summary>
        FieldMergeBarcode = 6302,
        /// <summary>
        /// Specifies the DDEAUTO field.
        /// </summary>
        FieldDDEAuto = 46,
        /// <summary>
        /// Specifies the DOCPROPERTY field.
        /// </summary>
        FieldDocProperty = 85,
        /// <summary>
        /// Specifies the DOCVARIABLE field.
        /// </summary>
        FieldDocVariable = 64,
        /// <summary>
        /// Specifies the EDITTIME field.
        /// </summary>
        FieldEditTime = 25,
        /// <summary>
        /// Specifies the EMBED field.
        /// </summary>
        FieldEmbed = 58,
        /// <summary>
        /// Specifies the EQ field.
        /// </summary>
        FieldEquation = 49,
        /// <summary>
        /// Specifies the FILENAME field.
        /// </summary>
        FieldFileName = 29,
        /// <summary>
        /// Specifies the FILESIZE field.
        /// </summary>
        FieldFileSize = 69,
        /// <summary>
        /// Specifies the FILLIN field.
        /// </summary>
        FieldFillIn = 39,
        /// <summary>
        /// Specifies the FOOTNOTEREF field.
        /// </summary>
        FieldFootnoteRef = 5,
        /// <summary>
        /// Specifies the FORMCHECKBOX field.
        /// </summary>
        FieldFormCheckBox = 71,
        /// <summary>
        /// Specifies the FORMDROPDOWN field.
        /// </summary>
        FieldFormDropDown = 83,
        /// <summary>
        /// Specifies the FORMTEXT field.
        /// </summary>
        FieldFormTextInput = 70,
        /// <summary>
        /// Specifies the = (formula) field.
        /// </summary>
        FieldFormula = 34,
        /// <summary>
        /// Specifies the GREETINGLINE field.
        /// </summary>
        FieldGreetingLine = 94,
        /// <summary>
        /// Specifies the GLOSSARY field.
        /// </summary>
        FieldGlossary = 47,
        /// <summary>
        /// Specifies the GOTOBUTTON field.
        /// </summary>
        FieldGoToButton = 50,
        /// <summary>
        /// Specifies the field that represents an HTML control.
        /// </summary>
        FieldHtmlActiveX = 91,
        /// <summary>
        /// Specifies the HYPERLINK field.
        /// </summary>
        FieldHyperlink = 88,
        /// <summary>
        /// Specifies the IF field.
        /// </summary>
        FieldIf = 7,
        /// <summary>
        /// Specifies the INCLUDE field.
        /// </summary>
        FieldInclude = 36,
        /// <summary>
        /// Specifies the INCLUDEPICTURE field.
        /// </summary>
        FieldIncludePicture = 67,
        /// <summary>
        /// Specifies the INCLUDETEXT field.
        /// </summary>
        FieldIncludeText = 68,
        /// <summary>
        /// Specifies the INDEX field.
        /// </summary>
        FieldIndex = 8,
        /// <summary>
        /// Specifies the XE field.
        /// </summary>
        FieldIndexEntry = 4,
        /// <summary>
        /// Specifies the INFO field.
        /// </summary>
        FieldInfo = 14,
        /// <summary>
        /// Specifies the IMPORT field.
        /// </summary>
        FieldImport = 55,
        /// <summary>
        /// Specifies the KEYWORDS field.
        /// </summary>
        FieldKeyword = 18,
        /// <summary>
        /// Specifies the LASTSAVEDBY field.
        /// </summary>
        FieldLastSavedBy = 20,
        /// <summary>
        /// Specifies the LINK field.
        /// </summary>
        FieldLink = 56,
        /// <summary>
        /// Specifies the LISTNUM field.
        /// </summary>
        FieldListNum = 90,
        // <summary>
        // Undocumented.
        // </summary>
        // FieldMacro = 76,
        /// <summary>
        /// Specifies the MACROBUTTON field.
        /// </summary>
        FieldMacroButton = 51,
        /// <summary>
        /// Specifies the MERGEFIELD field.
        /// </summary>
        FieldMergeField = 59,
        /// <summary>
        /// Specifies the MERGEREC field.
        /// </summary>
        FieldMergeRec = 44,
        /// <summary>
        /// Specifies the MERGESEQ field.
        /// </summary>
        FieldMergeSeq = 75,
        /// <summary>
        /// Specifies the NEXT field.
        /// </summary>
        FieldNext = 41,
        /// <summary>
        /// Specifies the NEXTIF field.
        /// </summary>
        FieldNextIf = 42,
        /// <summary>
        /// Specifies the NOTEREF field.
        /// </summary>
        FieldNoteRef = 72,
        /// <summary>
        /// Specifies the NUMCHARS field.
        /// </summary>
        FieldNumChars = 28,
        /// <summary>
        /// Specifies the NUMPAGES field.
        /// </summary>
        FieldNumPages = 26,
        /// <summary>
        /// Specifies the NUMWORDS field.
        /// </summary>
        FieldNumWords = 27,
        /// <summary>
        /// <p>Specifies the OCX field.</p>
        /// <p>Normally, Aspose.Words will represent an ActiveX control as a
        /// <see cref="Aspose.Words.Drawing.Shape"/> object, but for some documents, where a control does
        /// not have data and/or seems to be invalid, it will be represented as a field.</p>
        /// </summary>
        FieldOcx = 87,
        /// <summary>
        /// Specifies the PAGE field.
        /// </summary>
        FieldPage = 33,
        /// <summary>
        /// Specifies the PAGEREF field.
        /// </summary>
        FieldPageRef = 37,
        /// <summary>
        /// Specifies the PRINT field.
        /// </summary>
        FieldPrint = 48,
        /// <summary>
        /// Specifies the PRINTDATE field.
        /// </summary>
        FieldPrintDate = 23,
        /// <summary>
        /// Specifies the PRIVATE field.
        /// </summary>
        FieldPrivate = 77,
        /// <summary>
        /// Specifies the QUOTE field.
        /// </summary>
        FieldQuote = 35,
        /// <summary>
        /// Specifies the REF field.
        /// </summary>
        FieldRef = 3,
        /// <summary>
        /// Specifies that the field represents a REF field where the keyword has been omitted.
        /// </summary>
        FieldRefNoKeyword = 2,
        /// <summary>
        /// Specifies the RD field.
        /// </summary>
        FieldRefDoc = 11,
        /// <summary>
        /// Specifies the REVNUM field.
        /// </summary>
        FieldRevisionNum = 24,
        /// <summary>
        /// Specifies the SAVEDATE field.
        /// </summary>
        FieldSaveDate = 22,
        /// <summary>
        /// Specifies the SECTION field.
        /// </summary>
        FieldSection = 65,
        /// <summary>
        /// Specifies the SECTIONPAGES field.
        /// </summary>
        FieldSectionPages = 66,
        /// <summary>
        /// Specifies the SEQ field.
        /// </summary>
        FieldSequence = 12,
        /// <summary>
        /// Specifies the SET field.
        /// </summary>
        FieldSet = 6,
        /// <summary>
        /// Specifies the SHAPE field.
        /// </summary>
        FieldShape = 95,
        /// <summary>
        /// Specifies the SKIPIF field.
        /// </summary>
        FieldSkipIf = 43,
        /// <summary>
        /// Specifies the STYLEREF field.
        /// </summary>
        FieldStyleRef = 10,
        /// <summary>
        /// Specifies the SUBJECT field.
        /// </summary>
        FieldSubject = 16,
        // <summary>
        // Undocumented.
        // </summary>
        // FieldSubscriber = 82,
        /// <summary>
        /// Specifies the SYMBOL field.
        /// </summary>
        FieldSymbol = 57,
        /// <summary>
        /// Specifies the TEMPLATE field.
        /// </summary>
        FieldTemplate = 30,
        /// <summary>
        /// Specifies the TIME field.
        /// </summary>
        FieldTime = 32,
        /// <summary>
        /// Specifies the TITLE field.
        /// </summary>
        FieldTitle = 15,
        /// <summary>
        /// Specifies the TOA field.
        /// </summary>
        FieldTOA = 73,
        /// <summary>
        /// Specifies the TA field.
        /// </summary>
        FieldTOAEntry = 74,
        /// <summary>
        /// Specifies the TOC field.
        /// </summary>
        FieldTOC = 13,
        /// <summary>
        /// Specifies the TC field.
        /// </summary>
        FieldTOCEntry = 9,
        /// <summary>
        /// Specifies the USERADDRESS field.
        /// </summary>
        FieldUserAddress = 62,
        /// <summary>
        /// Specifies the USERINITIALS field.
        /// </summary>
        FieldUserInitials = 61,
        /// <summary>
        /// Specifies the USERNAME field.
        /// </summary>
        FieldUserName = 60
    }
}
