// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/08/2012 by Denis Shvydkiy

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Indicates the symbol set that is used to represent numbers
    /// while rendering to fixed page formats.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum NumeralFormat
    {
        /// <summary>
        /// European numerals: 0123456789.
        /// </summary>
        European = 0,
        /// <summary>
        /// Numerals used in Arabic: ٠١٢٣٤٥٦٧٨٩.
        /// Unicode range U+0660 - u+0669.
        /// </summary>
        ArabicIndic = 1,
        /// <summary>
        /// Numerals used in Persian and Urdu: ۰۱۲۳۴۵۶۷۸۹.
        /// Unicode range U+06F0 - u+06F9.
        /// </summary>
        EasternArabicIndic = 2,
        /// <summary>
        /// Symbol set is decided from context(locale and RTL property).
        /// </summary>
        Context = 3,
        /// <summary>
        /// THIS OPTION IS NOT SUPPORTED.
        /// Symbol set is decided from regional settings.
        /// </summary>
        System = 4
    }
}
