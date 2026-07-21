// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/06/2022 by Vadim Saltykov

namespace Aspose.Words
{
    /// <summary>
    /// Specifies preset margins.
    /// </summary>
    public enum Margins
    {
        /// <summary>
        /// Normal margins.
        /// </summary>
        Normal,

        /// <summary>
        /// Narrow margins.
        /// </summary>
        Narrow,

        /// <summary>
        /// Moderate margins.
        /// </summary>
        Moderate,

        /// <summary>
        /// Wide margins.
        /// </summary>
        Wide,

        /// <summary>
        /// Mirrored margins.
        /// </summary>
        /// <remarks>
        /// Setting margins to Mirrored will set the appropriate value for <see cref="Aspose.Words.PageSetup.MultiplePages"/> property.
        /// This will affect the whole document, not just the current section.
        /// </remarks>
        Mirrored,

        /// <summary>
        /// Custom margins.
        /// </summary>
        Custom
    }
}
