// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/03/2006 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Indicates the format of the document that is to be loaded.
    /// </summary>
    /// <dev>
    /// We should only use values 0..255 here because these values are used as bit flags in the UnifiedScenario enum.
    /// </dev>
    [CppEnumEnableMetadata]
    [CppEnumWithOperators]
    public enum LoadFormat
    {
        /// <summary>
        /// Instructs Aspose.Words to recognize the format automatically.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Microsoft Works 8 Document.
        /// </summary>
        MsWorks = 8,

        /// <summary>
        /// Microsoft Word 95 or Word 97 - 2003 Document.
        /// </summary>
        Doc = 10,
        /// <summary>
        /// Microsoft Word 95 or Word 97 - 2003 Template.
        /// </summary>
        Dot = 11,
        /// <summary>
        /// The document is in pre-Word 95 format.
        /// Aspose.Words does not currently support loading such documents.
        /// </summary>
        DocPreWord60 = 12,

        /// <summary>
        /// Office Open XML WordprocessingML Document (macro-free).
        /// </summary>
        Docx = 20,
        /// <summary>
        /// Office Open XML WordprocessingML Macro-Enabled Document.
        /// </summary>
        Docm = 21,
        /// <summary>
        /// Office Open XML WordprocessingML Template (macro-free).
        /// </summary>
        Dotx = 22,
        /// <summary>
        /// Office Open XML WordprocessingML Macro-Enabled Template.
        /// </summary>
        Dotm = 23,
        /// <summary>
        /// Office Open XML WordprocessingML stored in a flat XML file instead of a ZIP package.
        /// </summary>
        FlatOpc = 24,
        /// <summary>
        /// Office Open XML WordprocessingML Macro-Enabled Document stored in a flat XML file instead of a ZIP package.
        /// </summary>
        FlatOpcMacroEnabled = 25,
        /// <summary>
        /// Office Open XML WordprocessingML Template (macro-free) stored in a flat XML file instead of a ZIP package.
        /// </summary>
        FlatOpcTemplate = 26,
        /// <summary>
        /// Office Open XML WordprocessingML Macro-Enabled Template stored in a flat XML file instead of a ZIP package.
        /// </summary>
        FlatOpcTemplateMacroEnabled = 27,
        /// <summary>
        /// RTF format.
        /// </summary>
        Rtf = 30,
        /// <summary>
        /// Microsoft Word 2003 WordprocessingML format.
        /// </summary>
        WordML = 31,

        /// <summary>
        /// HTML format.
        /// </summary>
        Html = 50,
        /// <summary>
        /// MHTML (Web archive) format.
        /// </summary>
        Mhtml = 51,
        /// <summary>
        /// MOBI format. Used by MobiPocket reader and Amazon Kindle readers.
        /// </summary>
        Mobi = 52,
        /// <summary>
        /// CHM (Compiled HTML Help) format.
        /// </summary>
        Chm = 53,
        /// <summary>
        /// AZW3 format. Used by Amazon Kindle readers.
        /// </summary>
        Azw3 = 54,
        /// <summary>
        /// EPUB format.
        /// </summary>
        Epub = 55,

        /// <summary>
        /// ODF Text Document.
        /// </summary>
        Odt = 60,
        /// <summary>
        /// ODF Text Document Template.
        /// </summary>
        Ott = 61,

        /// <summary>
        /// Plain Text.
        /// </summary>
        Text = 62,

        /// <summary>
        /// Markdown text document.
        /// </summary>
        Markdown = 63,

        /// <summary>
        /// Pdf document.
        /// </summary>
        Pdf = 64,

        /// <summary>
        /// XML document.
        /// </summary>
        Xml = 65,

        /// <summary>
        /// Unrecognized format, cannot be loaded by Aspose.Words.
        /// </summary>
        Unknown = 255,
    }

    /// <summary>
    /// This is used during unit testing as an additional load format to the one specified in LoadFormat above.
    /// </summary>
    [CppEnumWithOperators]
    internal enum LoadFormatTest
    {
        /// <summary>
        /// Used in unified tests only to request loading of a DOCX file with graphics in DrawingML format.
        /// </summary>
        TestDocxDml = 254,
    }
}
