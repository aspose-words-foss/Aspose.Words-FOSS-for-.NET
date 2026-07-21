// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2022 by Edward Voronov

using System.Collections.Generic;

using DataTable = Aspose.Words.Fields.FieldDatabaseDataTable;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides utility methods for the <see cref="FieldDatabase"/> field.
    /// </summary>
    internal static class FieldDatabaseUtils
    {
        internal static FieldDatabaseDataTable TrimRecords(FieldDatabaseDataTable dataTable, int firstRecord, int lastRecord)
        {
            ResultDataTableBuilder builder = new ResultDataTableBuilder(dataTable);

            if (firstRecord == int.MinValue && lastRecord == int.MinValue)
                builder.AppendAll();
            else if (firstRecord == int.MinValue)
                TrimEnd(builder, lastRecord);
            else if (lastRecord == int.MinValue)
                TrimStart(builder, firstRecord);
            else
                TrimStartEnd(builder, firstRecord, lastRecord);

            return builder.Build();
        }

        private static void TrimStartEnd(ResultDataTableBuilder builder, int firstRecord, int lastRecord)
        {
            Debug.Assert(firstRecord != int.MinValue);
            Debug.Assert(lastRecord != int.MinValue);

            if (firstRecord > 0 && lastRecord > 0)
            {
                if (firstRecord < lastRecord)
                    builder.AppendRange(firstRecord, lastRecord);
                else // firstRecord >= lastRecord
                    builder.AppendRange(lastRecord, firstRecord);
            }
            else if (firstRecord == 0)
            {
                // empty
            }
            else if (lastRecord == 0)
            {
                if (firstRecord > 0)
                {
                    builder.AppendLast();
                    builder.AppendTo(firstRecord);
                }
                else if (firstRecord == -1)
                {
                    builder.AppendFirst();
                }
                else // firstRecord < -1
                {
                    builder.AppendLast();
                    builder.AppendAll();
                }
            }
            else if (firstRecord > 0)
            {
                builder.AppendFrom(firstRecord);
            }
            else if (lastRecord > 0)
            {
                if (firstRecord == -1)
                    builder.AppendTo(lastRecord);
                else // firstRecord < -1
                    builder.AppendFrom(lastRecord);
            }
            else // firstRecord < 0 && lastRecord < 0
            {
                TrimNegativeStartEnd(builder, firstRecord, lastRecord);
            }
        }

        private static void TrimNegativeStartEnd(ResultDataTableBuilder builder, int firstRecord, int lastRecord)
        {
            Debug.Assert(firstRecord < 0);
            Debug.Assert(lastRecord < 0);

            firstRecord = -firstRecord;
            lastRecord = -lastRecord;

            if (firstRecord == 1)
            {
                builder.AppendAll();
            }
            else if (firstRecord < Bound && lastRecord == 1)
            {
                int count = firstRecord + 1;
                for (int i = 0; i < count; i++)
                {
                    int index = builder.SourceCount - count + 1 + i;
                    if (index < 1)
                        index = 1;
                    builder.Append(index);
                }

                builder.AppendAll();
            }
            else if (firstRecord < Bound && lastRecord < Bound)
            {
                TrimStartEndNegativeRange(builder, firstRecord, lastRecord);
            }
            else if (firstRecord <= Bound && lastRecord <= Bound)
            {
                builder.AppendLast();
            }
            else if (firstRecord > Bound && lastRecord > Bound)
            {
                TrimStartEndNegativeRange(builder, firstRecord, lastRecord);
            }
            else if (firstRecord > Bound)
            {
                TrimBound(builder, firstRecord);
            }
            else // lastRecord > bound
            {
                TrimBound(builder, lastRecord);
            }
        }

        private static void TrimStartEndNegativeRange(ResultDataTableBuilder builder, int firstRecord, int lastRecord)
        {
            int from;
            int to;
            if (firstRecord < lastRecord)
            {
                from = builder.SourceCount - lastRecord;
                to = builder.SourceCount - firstRecord;
            }
            else
            {
                from = builder.SourceCount - firstRecord;
                to = builder.SourceCount - lastRecord;
            }

            int count = to - from + 1;

            for (int i = 0; i < count; i++)
            {
                int index = from + i;
                if (index < 1)
                    index = 1;
                builder.Append(index);
            }
        }

        private static void TrimStart(ResultDataTableBuilder builder, int firstRecord)
        {
            Debug.Assert(firstRecord != int.MinValue);
            if (firstRecord == -1)
            {
                builder.AppendAll();
            }
            else if (firstRecord < -1)
            {
                TrimBound(builder, -firstRecord);
            }
            else if (firstRecord > 0)
            {
                builder.AppendFrom(firstRecord);
            }
            else // firstRecord == 0
            {
                // empty
            }
        }

        private static void TrimEnd(ResultDataTableBuilder builder, int lastRecord)
        {
            Debug.Assert(lastRecord != int.MinValue);

            if (lastRecord < 0)
            {
                builder.AppendAll();
            }
            else if (lastRecord == 0)
            {
                builder.AppendLast();
                builder.AppendFirst();
            }
            else // lastRecord > 0
            {
                builder.AppendRange(1, lastRecord);
            }
        }

        private static void TrimBound(ResultDataTableBuilder builder, int number)
        {
            int count = number - Bound;
            for (int i = 0; i < count; i++)
            {
                int index = builder.SourceCount - number + i;
                if (index < 1)
                    index = 1;
                builder.Append(index);
            }

            builder.AppendLast();
        }

        private const int Bound = 16;

        private class ResultDataTableBuilder
        {
            internal ResultDataTableBuilder(DataTable source)
            {
                Debug.Assert(source.Rows.Count > 0);
                mSource = source;
                mRows = new List<int>(mSource.Rows.Count);
            }

            internal int SourceCount
            {
                get { return mSource.Rows.Count; }
            }

            internal void AppendAll()
            {
                AppendRange(1, mSource.Rows.Count);
            }

            // 1-based
            internal void AppendRange(int from, int to)
            {
                for (int i = from; i <= to; i++)
                    Append(i);
            }

            // 1-based
            internal void AppendFrom(int from)
            {
                AppendRange(from, mSource.Rows.Count);
            }

            // 1-based
            internal void AppendTo(int to)
            {
                AppendRange(1, to);
            }

            // 1-based
            internal void Append(int index)
            {
                if(index >= 1 && index <= mSource.Rows.Count)
                    mRows.Add(index - 1);
            }

            internal void AppendFirst()
            {
                Append(1);
            }

            internal void AppendLast()
            {
                Append(mSource.Rows.Count);
            }

            internal DataTable Build()
            {
                if (mRows.Count == 0)
                    return new DataTable();

                if (ReturnSource())
                    return mSource;

                DataTable clone = new DataTable(mSource.ColumnNames);

                foreach (int row in mRows)
                    clone.Rows.Add(mSource.Rows[row]);

                return clone;
            }

            private bool ReturnSource()
            {
                if (mRows.Count != mSource.Rows.Count)
                    return false;

                for (int i = 0; i < mRows.Count; i++)
                    if (mRows[i] != i)
                        return false;

                return true;
            }

            private readonly List<int> mRows;
            private readonly DataTable mSource;
        }
    }
}
