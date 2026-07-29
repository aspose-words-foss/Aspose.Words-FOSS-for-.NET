// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2022 by Edward Voronov

using System.Collections.Generic;
using System.Data;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides data for the <see cref="FieldDatabase"/> field result. Please see <see cref="DataTable"/> instance.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    public class FieldDatabaseDataTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDatabaseDataTable"/> class.
        /// </summary>
        public FieldDatabaseDataTable(params string[] columnNames)
        {
            ColumnNames = columnNames;
            Rows = new List<FieldDatabaseDataRow>();
        }

        /// <summary>
        /// Gets columns that belong to this table.
        /// </summary>
        public string[] ColumnNames { get; }

        /// <summary>
        /// Gets rows that belong to this table.
        /// </summary>
        public IList<FieldDatabaseDataRow> Rows { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDatabaseDataTable"/> class
        /// from the <see cref="DataTable"/> instance.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition] // C++ doesn't support System.Data
        public static FieldDatabaseDataTable CreateFrom(DataTable dataTable)
        {
            string[] columnNames = new string[dataTable.Columns.Count];
            for (int i = 0; i < dataTable.Columns.Count; i++)
                columnNames[i] = dataTable.Columns[i].ColumnName;

            FieldDatabaseDataTable result = new FieldDatabaseDataTable(columnNames);

            foreach (DataRow dataRow in dataTable.Rows)
                result.Rows.Add(FieldDatabaseDataRow.CreateFrom(dataRow));

            return result;
        }
    }
}
