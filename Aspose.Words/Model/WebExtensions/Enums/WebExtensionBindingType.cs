// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2019 by Dmitry Sokolov

namespace Aspose.Words.WebExtensions
{
    /// <summary>
    /// Enumerates available types of binding between a web extension and the data in the document.
    /// </summary>
    /// <dev>https://docs.microsoft.com/en-us/javascript/api/office/office.bindingtype?view=office-js</dev>
    public enum WebExtensionBindingType
    {
        /// <summary>
        /// Tabular data without a header row.
        /// </summary>
        Matrix,

        /// <summary>
        /// Tabular data with a header row.
        /// </summary>
        Table,

        /// <summary>
        /// Plain text.
        /// </summary>
        Text,

        /// <summary>
        /// Matrix used by default.
        /// </summary>
        Default = Matrix // Default for a while.
    }
}
