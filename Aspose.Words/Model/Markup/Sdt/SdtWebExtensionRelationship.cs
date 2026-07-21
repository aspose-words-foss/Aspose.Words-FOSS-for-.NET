// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/10/2016 by Alexander Zhiltsov

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Specifies a relationship between a structured document tag and an Office Web Extension (webExtension).
    /// </summary>
    /// <dev>
    /// Represents value of the 2.5.1.12 webExtensionCreated and 2.5.1.13 webExtensionLinked [MS-DOCX] elements.
    /// </dev>
    internal enum SdtWebExtensionRelationship
    {
        /// <summary>
        /// No relationship. This is a default value.
        /// </summary>
        None,

        /// <summary>
        /// The structured document tag was created by, and is bound to, at least one webExtension.
        /// </summary>
        CreatedByWebExtension,

        /// <summary>
        /// The structured document tag is bound to at least one webExtension.
        /// </summary>
        LinkedToWebExtension
    }
}
