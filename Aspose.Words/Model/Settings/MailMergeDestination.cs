// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Specifies the possible results which may be generated when a mail merge is carried out on a document.
    /// </summary>
    /// <seealso cref="MailMergeSettings.Destination"/>
    /// <dev>Do not renumber these as they values are used in the DOC codec.</dev>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags",
        Justification = "Not used as a flag.")]
    public enum MailMergeDestination
    {
        /// <summary>
        /// Specifies that conforming hosting applications shall generate new documents by populating the fields
        /// within a given document with data from the specified external data source.
        /// </summary>
        NewDocument = 0,
        /// <summary>
        /// Specifies that conforming hosting applications shall print the documents that result from populating the
        /// fields within a given document with external data from the specified external data source.
        /// </summary>
        Printer = 1,
        /// <summary>
        /// Specifies that conforming hosting applications shall generate emails using the documents that result from
        /// populating the fields within a given document with data from the specified external data source.
        /// </summary>
        Email = 2,
        /// <summary>
        /// Specifies that conforming hosting applications shall generate faxes using the documents that result from
        /// populating the fields within a given document with data from the specified external data source.
        /// </summary>
        Fax = 4,

        /// <summary>
        /// Equals to the <see cref="NewDocument"/> value.
        /// </summary>
        Default = NewDocument
    }
}
