// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2009 by Roman Korchagin

using System;
using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Properties
{
    /// <summary>
    /// Used as a value for the <see cref="BuiltInDocumentProperties.Security"/> property.
    /// Specifies the security level of a document as a numeric value.
    /// </summary>
    [Flags]
    [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames",
        Justification = "Public API.")]
    public enum DocumentSecurity
    {
        /// <summary>
        /// There are no security states specified by the property.
        /// </summary>
        None = 0x0000,
        /// <summary>
        /// The document is password protected. (Note has never been seen in a document so far).
        /// </summary>
        PasswordProtected = 0x0001,
        /// <summary>
        /// The document to be opened read-only if possible, but the setting can be overridden.
        /// </summary>
        ReadOnlyRecommended = 0x0002,
        /// <summary>
        /// The document to always be opened read-only.
        /// </summary>
        ReadOnlyEnforced = 0x0004,
        /// <summary>
        /// The document to always be opened read-only except for annotations.
        /// </summary>
        ReadOnlyExceptAnnotations = 0x0008
    }
}
