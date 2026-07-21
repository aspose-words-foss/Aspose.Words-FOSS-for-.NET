// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2018 by Alexey Butalov

using System;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Tests
{
    /// <summary>
    /// Specifies the conversion scenario (load format and save format) for a unified test.
    /// Also specifies the flag whether the gold verification should take place.
    /// </summary>
    [Flags]
    [CppEnumEnableMetadata]
    public enum UnifiedScenario
    {
        /// <summary>
        /// LoadFormat value occupies the bits 0..7.
        /// </summary>
        LoadFormatMask = 0x000000ff,
        /// <summary>
        /// SaveFormat value occupies the bits 8..15.
        /// </summary>
        SaveFormatMask = 0x0000ff00,

        /// <summary>
        /// When this bit is set, NO gold verification will take place.
        /// </summary>
        NoGold = 0x00010000,

        /// <summary>
        /// When this bit is set, ExportOnly gold verification will take place.
        /// </summary>
        ExportOnly = 0x00020000,

        /// <summary>
        /// Part of the standard unified scenarios.
        /// The verify gold is requested, but currently there is no gold verification for this binary scenario.
        /// Could be a good idea to create another module that will read DOC and dump as TXT so we can create golds for these.
        /// </summary>
        Doc2Doc = LoadFormat.Doc | (SaveFormat.Doc << 8),
        /// <summary>
        /// Part of the standard unified scenarios.
        /// </summary>
        Doc2Docx = LoadFormat.Doc | (SaveFormat.Docx << 8),
        /// <summary>
        /// Part of the standard unified scenarios, used instead of Doc2Docx for DOCM files with macros.
        /// </summary>
        Doc2Docm = LoadFormat.Doc | (SaveFormat.Docm << 8),
        /// <summary>
        /// Part of the standard unified scenarios.
        /// </summary>
        Doc2Rtf = LoadFormat.Doc | (SaveFormat.Rtf << 8),
        /// <summary>
        /// Part of the standard unified scenarios.
        /// </summary>
        Doc2Wml = LoadFormat.Doc | (SaveFormat.WordML << 8),
        /// <summary>
        /// Part of the standard unified scenarios.
        /// Open as OOXML with DrawingML and save as OOXML with DrawingML.
        /// This scenario is not for all files, but only for those that have graphics and shapes.
        /// </summary>
        DocxDml2DocxDml = LoadFormatTest.TestDocxDml | (SaveFormat.Docx << 8),
        /// <summary>
        /// Part of the standard unified scenarios.
        /// </summary>
        Docx2DocxNoGold = LoadFormat.Docx | (SaveFormat.Docx << 8) | NoGold,
        /// <summary>
        /// Part of the standard unified scenarios. Used instead of Docx2DocxNoGold for DOCM documents containing macros.
        /// </summary>
        Docm2DocmNoGold = LoadFormat.Docm | (SaveFormat.Docm << 8) | NoGold,
        /// <summary>
        /// Part of the standard unified scenarios.
        /// </summary>
        Rtf2RtfNoGold = LoadFormat.Rtf | (SaveFormat.Rtf << 8) | NoGold,
        /// <summary>
        /// Part of the standard unified scenarios.
        /// </summary>
        Wml2WmlNoGold = LoadFormat.WordML | (SaveFormat.WordML << 8) | NoGold,

        Docx2Docx = LoadFormat.Docx | (SaveFormat.Docx << 8),
        Docx2Rtf = LoadFormat.Docx | (SaveFormat.Rtf << 8),
        Docx2Wml = LoadFormat.Docx | (SaveFormat.WordML << 8),
        Docx2Md = LoadFormat.Docx | (SaveFormat.Markdown << 8),

        Rtf2Rtf = LoadFormat.Rtf | (SaveFormat.Rtf << 8),
        Rtf2Docx = LoadFormat.Rtf | (SaveFormat.Docx << 8),

        Wml2Wml = LoadFormat.WordML | (SaveFormat.WordML << 8),

        Md2Md = LoadFormat.Markdown | (SaveFormat.Markdown << 8),
        Md2Docx = LoadFormat.Markdown | (SaveFormat.Docx << 8),
    }
}
