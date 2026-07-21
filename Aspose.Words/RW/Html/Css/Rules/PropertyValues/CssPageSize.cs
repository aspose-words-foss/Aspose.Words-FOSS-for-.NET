// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Specifies page size media names.
    /// </summary>
    /// <remarks>
    /// A page size can be specified using one of the following media names. 
    /// This is the equivalent of specifying the ‘page-size’ using length values. 
    /// The definition of the media names comes from Media Standardized Names.
    /// http://www.w3.org/TR/css3-page/#page-size-prop
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    internal enum CssPageSize
    {
        NotSpecified,
        /// <summary>
        /// Equivalent to the size of ISO A5 media: 148mm wide and 210 mm high.
        /// </summary>
        A5,
        /// <summary>
        /// Equivalent to the size of ISO A4 media: 210 mm wide and 297 mm high.
        /// </summary>
        A4,
        /// <summary>
        /// Equivalent to the size of ISO A3 media: 297mm wide and 420mm high.
        /// </summary>
        A3,
        /// <summary>
        /// Equivalent to the size of ISO B5 media: 176mm wide by 250mm high.
        /// </summary>
        B5,
        /// <summary>
        /// Equivalent to the size of ISO B4 media: 250mm wide by 353mm high.
        /// </summary>
        B4,
        /// <summary>
        /// Equivalent to the size of North American letter media: 8.5 inches wide and 11 inches high
        /// </summary>
        Letter,
        /// <summary>
        /// Equivalent to the size of North American legal: 8.5 inches wide by 14 inches high.
        /// </summary>
        Legal,
        /// <summary>
        /// Equivalent to the size of North American ledger: 11 inches wide by 17 inches high.
        /// </summary>
        Ledger
    }
}
