// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2022 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implement this interface to provide data for the <see cref="FieldDatabase"/> field when it's updated.
    /// </summary>
    public interface IFieldDatabaseProvider
    {
        /// <summary>
        /// Returns query result.
        /// </summary>
        /// <param name="fileName">The complete path and file name of the database specified in the \d field switch.</param>
        /// <param name="connection">The connection to the data specified in the \c field switch.</param>
        /// <param name="query">The set of SQL instructions that query the database specified in the \s field switch.</param>
        /// <param name="field">The field being updated.</param>
        /// <returns>The <see cref="FieldDatabaseDataTable"/> instance that should be used for the field's update.</returns>
        FieldDatabaseDataTable GetQueryResult(string fileName, string connection, string query, FieldDatabase field);
    }
}
