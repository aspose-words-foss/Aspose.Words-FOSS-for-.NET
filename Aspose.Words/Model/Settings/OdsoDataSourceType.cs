// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/10/2009 by Roman Korchagin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Specifies the type of the external data source to be connected to as part of the ODSO connection information.
    /// </summary>
    /// <remarks>
    /// <para>The OOXML specification is very vague for this enum. I guess it might correspond to the WdMergeSubType
    /// enumeration http://msdn.microsoft.com/en-us/library/bb237801.aspx.</para>
    /// 
    /// <seealso cref="Odso.DataSourceType"/>
    /// </remarks>
    /// <dev>Do not renumber these.</dev>
    public enum OdsoDataSourceType
    {
        /// <summary>
        /// Specifies that a given document has been connected to a text file.
        /// Possibly wdMergeSubTypeOther.
        /// </summary>
        Text = 0,
        /// <summary>
        /// Specifies that a given document has been connected to a database.
        /// Possibly wdMergeSubTypeAccess.
        /// </summary>
        Database = 1,
        /// <summary>
        /// Specifies that a given document has been connected to an address book of contacts.
        /// Possibly wdMergeSubTypeOAL.
        /// </summary>
        AddressBook = 2,
        /// <summary>
        /// Specifies that a given document has been connected to another document format supported by the producing application.
        /// Possibly wdMergeSubTypeOLEDBWord.
        /// </summary>
        Document1 = 3,
        /// <summary>
        /// Specifies that a given document has been connected to another document format supported by the producing application.
        /// Possibly wdMergeSubTypeWorks.
        /// </summary>
        Document2 = 4,
        /// <summary>
        /// Specifies that a given document has been connected to another document format native to the producing application.
        /// Possibly wdMergeSubTypeOLEDBText
        /// </summary>
        Native = 5,
        /// <summary>
        /// Specifies that a given document has been connected to an e-mail application.
        /// Possibly wdMergeSubTypeOutlook.
        /// </summary>
        Email = 6,
        /// <summary>
        /// The type of the external data source is not specified.
        /// Possibly wdMergeSubTypeWord.
        /// </summary>
        None = 7,
        /// <summary>
        /// Specifies that a given document has been connected to a legacy document format supported by the producing application 
        /// Possibly wdMergeSubTypeWord2000.
        /// </summary>
        Legacy = 8,
        /// <summary>
        /// Specifies that a given document has been connected to a data source which aggregates other data sources.
        /// </summary>
        Master,

        /// <summary>
        /// Equals to <see cref="OdsoDataSourceType.None"/>.
        /// </summary>
        Default = None
    }
}
