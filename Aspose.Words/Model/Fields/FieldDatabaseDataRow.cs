// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2022 by Edward Voronov

using System;
using System.Data;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides data for the <see cref="FieldDatabase"/> field result. Please see <see cref="DataRow"/> instance.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    public class FieldDatabaseDataRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDatabaseDataRow"/> class.
        /// </summary>
        public FieldDatabaseDataRow(params string[] values)
        {
            Values = values;
        }

        /// <summary>
        /// Gets values that belong to this row.
        /// </summary>
        public string[] Values { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDatabaseDataRow"/> class
        /// from the <see cref="DataRow"/> instance.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition] // C++ doesn't support System.Data
        public static FieldDatabaseDataRow CreateFrom(DataRow dataRow)
        {
            string[] values = new string[dataRow.ItemArray.Length];
            for (int i = 0; i < dataRow.ItemArray.Length; i++)
            {
                object value = dataRow.ItemArray[i];
                values[i] = value != null && value != DBNull.Value
                    ? value.ToString()
                    : string.Empty;
            }

            return new FieldDatabaseDataRow(values);
        }
    }
}
