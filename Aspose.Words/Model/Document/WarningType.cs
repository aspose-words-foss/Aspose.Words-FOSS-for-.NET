// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2011 by Roman Korchagin

using System;
using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words
{
    /// <summary>
    /// Specifies the type of a warning that is issued by Aspose.Words during document loading or saving.
    /// </summary>
    [Flags]
    [SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags",
        Justification = "Public API, as designed.")]
    [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames",
        Justification = "Public API, as designed.")]
    public enum WarningType
    {
        /// <summary>
        /// Some text/char/image or other data will be missing from either the document tree following load, 
        /// or from the created document following save.
        /// </summary>
        DataLossCategory = 0x000000FF,
        /// <summary>
        /// Generic data loss, no specific code.
        /// </summary>
        DataLoss = 0x00000001,
        /// <summary>
        /// The resulting document or a particular location in it might look substantially different 
        /// compared to the original document.
        /// </summary>
        MajorFormattingLossCategory = 0x0000FF00,
        /// <summary>
        /// Generic major formatting loss, no specific code.
        /// </summary>
        MajorFormattingLoss = 0x00000100,
        /// <summary>
        /// The resulting document or a particular location in it might look somewhat different compared 
        /// to the original document.
        /// </summary>
        MinorFormattingLossCategory = 0x00FF0000,
        /// <summary>
        /// Generic minor formatting loss, no specific code.
        /// </summary>
        MinorFormattingLoss = 0x00010000,
        /// <summary>
        /// Font has been substituted.
        /// </summary>
        FontSubstitution = 0x00020000,
        /// <summary>
        /// Loss of embedded font information during document saving.
        /// </summary>
        FontEmbedding = 0x00040000,
        /// <summary>
        /// Some content in the source document could not be recognized (i.e. is unsupported), this may or may not 
        /// cause issues or result in data/formatting loss. 
        /// </summary>
        UnexpectedContentCategory = 0x0F000000,
        /// <summary>
        /// Generic unexpected content, no specific code.
        /// </summary>
        UnexpectedContent = 0x01000000,
        /// <summary>
        /// Advises of a potential problem or suggests an improvement.
        /// </summary>
        Hint = 0x10000000
    }

    // RK By Aspose's design there are two more values that we currently don't use. I just don't have good uses for them yet.
    //
    // 0 - SourceFileCorruption  An issue has been detected in the source document which makes it very likely the document 
    //     will be not be able to be opened if saved in it's original format (Load warning only).
    //
    // 4 - CompatibilityIssue  Known issue that will prevent the document being opened by certain user agents, or previous 
    //     versions of user agents (To be explained in WarningInfo.Description). 
}
