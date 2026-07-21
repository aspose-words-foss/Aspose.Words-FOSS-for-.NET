// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/10/2009 by Roman Korchagin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Specifies the possible types used to indicate if a given mail merge field has been mapped to a column in the given external data source.
    /// </summary>
    /// <seealso cref="OdsoFieldMapData.Type"/>
    public enum OdsoFieldMappingType
    {
        /// <summary>
        /// Specifies that the mail merge field has been mapped to a column in the given external data source.
        /// </summary>
        Column,
        /// <summary>
        /// Specifies that the mail merge field has not been mapped to a column in the given external data source.
        /// </summary>
        Null,

        /// <summary>
        /// Equals to <see cref="OdsoFieldMappingType.Null"/>.
        /// </summary>
        Default = Null
    }
}
