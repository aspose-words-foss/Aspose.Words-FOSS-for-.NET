// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/08/2017 by Alexander Sevidov

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Specifies available options for leading space handling during import from <see cref="LoadFormat.Text"/> file.
    /// </summary>
    public enum TxtLeadingSpacesOptions
    {
        /// <summary>
        /// Leading spaces are removed and converted to left indent.
        /// </summary>
        ConvertToIndent = 0,

        /// <summary>
        /// Leading spaces are trimmed
        /// </summary>
        Trim = 1,

        /// <summary>
        /// Leading spaces are preserved. 
        /// </summary>
        Preserve = 2
    }
}
