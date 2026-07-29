// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/12/2012 by Ivan Lyagin

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies a predefined LISTNUM or AUTONUM field's list type.
    /// </summary>
    internal enum FieldNumListType
    {
        /// <summary>
        /// Specifies a non-predefined LISTNUM or AUTONUM field's list. Default value.
        /// </summary>
        None,
        /// <summary>
        /// Specifies a list used to maintain LISTNUM field's NumberDefault list functionality
        /// and AUTONUM field functionality.
        /// </summary>
        Number,
        /// <summary>
        /// Specifies a list used to maintain LISTNUM field's OutlineDefault list functionality
        /// and AUTONUMOUT field functionality.
        /// </summary>
        Outline,
        /// <summary>
        /// Specifies a list used to maintain LISTNUM field's LegalDefault list functionality
        /// and AUTONUMLGL field functionality.
        /// </summary>
        Legal
    }
}
