// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Specifies the type of an external mail merge data source.
    /// </summary>
    /// <seealso cref="MailMergeSettings.DataType"/>
    /// <dev>Do not renumber these values as they are used in the DOC codec.</dev>
    public enum MailMergeDataType
    {
        /// <summary>
        /// No mail merge data source is specified.
        /// </summary>
        None = -1,
        /// <summary>
        /// Specifies that a given document has been connected to a text file via the Dynamic Data Exchange (DDE) system.
        /// </summary>
        TextFile = 0,
        /// <summary>
        /// Specifies that a given document has been connected to an Access database via the Dynamic Data Exchange (DDE) system.
        /// </summary>
        Database = 1,
        /// <summary>
        /// Specifies that a given document has been connected to an Excel spreadsheet via the Dynamic Data Exchange (DDE) system.
        /// </summary>
        Spreadsheet = 2,
        /// <summary>
        /// Specifies that a given document has been connected to an external data source using an external query tool.
        /// </summary>
        Query = 3,
        /// <summary>
        /// Specifies that a given document has been connected to an external data source via the Open Database Connectivity interface.
        /// </summary>
        Odbc = 4,
        /// <summary>
        /// Specifies that a given document has been connected to an external data source via the Office Data Source Object (ODSO) interface.
        /// </summary>
        Native = 5,

        /// <summary>
        /// Equals to <see cref="None"/>.
        /// </summary>
        Default = None
    }
}
