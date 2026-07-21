// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/05/2006 by Roman Korchagin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Defines the writing style you want Word to use to check grammar in this document 
    /// (Spelling and Grammar option).
    /// </summary>
    internal class WritingStylePr
    {
        internal WritingStylePr Clone()
        {
            return (WritingStylePr)MemberwiseClone();
        }

        /// <summary>
        /// Gets or sets writing-style language as an ISO-6391-letter code or 4-digit hexadecimal code for a language.
        /// </summary>
        internal string Lang = null;

        /// <summary>
        /// Gets or sets writing-style DLL vendor ID.
        /// </summary>
        internal int VendorId = 0;

        /// <summary>
        /// Gets or sets writing-style DLL version.
        /// </summary>
        internal int DllVersion = 0;

        /// <summary>
        /// Specifies whether the DLL is NLCheck or not.
        /// </summary>
        internal bool NlCheck = false;

        /// <summary>
        /// Gets or sets the rule set for the writing style.
        /// </summary>
        internal int OptionSet = 0;
    }
}
