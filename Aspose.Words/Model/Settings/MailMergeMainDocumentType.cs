// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Specifies the possible types for a mail merge source document.
    /// </summary>
    /// <seealso cref="MailMergeSettings.MainDocumentType"/>
    /// <dev>Do not renumber these because the values are used in the DOC codec.</dev>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags",
        Justification = "Not used as a flag.")]
    public enum MailMergeMainDocumentType
    {
        /// <summary>
        /// This document is not a mail merge document.
        /// </summary>
        NotAMergeDocument = 0x00,
        /// <summary>
        /// Specifies that the mail merge source document is of the form letter type.
        /// </summary>
        FormLetters = 0x01,
        /// <summary>
        /// Specifies that the mail merge source document is of the mailing label type.
        /// </summary>
        MailingLabels = 0x02,
        /// <summary>
        /// Specifies that the mail merge source document is of the envelope type.
        /// </summary>
        Envelopes = 0x04,
        /// <summary>
        /// Specifies that the mail merge source document is of the catalog type.
        /// </summary>
        Catalog = 0x08,
        /// <summary>
        /// Specifies that the mail merge source document is of the e-mail message type.
        /// </summary>
        Email = 0x10,
        /// <summary>
        /// Specifies that the mail merge source document is of the fax type.
        /// </summary>
        Fax = 0x20, 

        /// <summary>
        /// Equals to <see cref="NotAMergeDocument"/>
        /// </summary>
        Default = NotAMergeDocument,
    }
}
