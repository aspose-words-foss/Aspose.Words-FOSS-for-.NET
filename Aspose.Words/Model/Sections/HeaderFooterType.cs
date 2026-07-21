// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Identifies the type of header or footer found in a Word file.
    /// </summary>
    /// <rev>
    /// This is a per section header/footer.
    /// Do not renumber as the value of the enum used as an index into plcfhdd.
    /// </rev>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum HeaderFooterType
    {
        /// <summary>
        /// Header for even numbered pages.
        /// </summary>
        HeaderEven = 0,
        /// <summary>
        /// Primary header, also used for odd numbered pages.
        /// </summary>
        HeaderPrimary = 1,
        /// <summary>
        /// Footer for even numbered pages.
        /// </summary>
        FooterEven = 2,
        /// <summary>
        /// Primary footer, also used for odd numbered pages.
        /// </summary>
        FooterPrimary = 3,
        /// <summary>
        /// Header for the first page of the section.
        /// </summary>
        HeaderFirst = 4,
        /// <summary>
        /// Footer for the first page of the section.
        /// </summary>
        FooterFirst = 5
    }
}
