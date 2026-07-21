// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/01/2011 by Andrey Soldatov

namespace Aspose.Words.Fonts
{
    /// <summary>
    /// <para>Specifies format of particular embedded font inside <see cref="FontInfo"/> object.</para>
    /// <para>When saving a document to a file, only embedded fonts of corresponding format are written down.</para>
    /// </summary>
    public enum EmbeddedFontFormat
    {
        /// <summary>
        /// <para>Specifies Embedded OpenType (EOT) File Format.</para>
        /// <para>This format of embedded fonts used in DOC files.</para>
        /// </summary>
        /// <remarks>
        /// <para>See http://www.w3.org/Submission/EOT for description of the format.</para>
        /// </remarks>
        EmbeddedOpenType,

        /// <summary>
        /// <para>Specifies font, embedded as plain copy of OpenType (TrueType) font file.</para>
        /// <para>This format of embedded fonts used in Open Office XML format, including DOCX files.</para>
        /// </summary>
        OpenType,
    }
}
