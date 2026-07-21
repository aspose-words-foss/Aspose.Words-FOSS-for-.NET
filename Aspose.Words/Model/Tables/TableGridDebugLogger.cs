// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/03/2022 by Dmitry Matveenko

using System.Diagnostics;
using System.Text;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Implements debug output methods used for table grid calculation.
    /// </summary>
    internal static class TableGridDebugLogger
    {
        /// <summary>
        /// Gets or sets a boolean value indicating if debugging output should be generated.
        /// </summary>
        /// <remarks>
        /// With many rows, generating debugging output may slow the tests noticeably.
        /// </remarks>
        internal static bool IsLogging
        {
            get { return mIsDebugOutputEnabled; }
            set { mIsDebugOutputEnabled = value; }
        }
        private static bool mIsDebugOutputEnabled;

        [Conditional("DEBUG")]
        internal static void DebugWriteLine(string line)
        {
            Debug.WriteLineIf(IsLogging, line);
        }

        [Conditional("DEBUG")]
        internal static void DebugWriteGrid(string context, TableGrid grid)
        {
            if (!IsLogging)
                return;

            StringBuilder sb = new StringBuilder((grid.Count + 1) * 200);
            sb.AppendLine(context);

            for(int i = 0; i < grid.Count; ++i)
            {
                AppendDebugString(sb, grid[i]);
            }

            sb.AppendFormat("End of grid, {0} elements.", grid.Count);

            Debug.WriteLine(sb.ToString());
        }

        [Conditional("DEBUG")]
        private static void AppendDebugString(StringBuilder sb, TableGridColumn column)
        {
            sb.AppendFormat("Width: {3,5}; Pref twips: {0,5}; Pref %: {1,5}; HasPercent: {2,5};",
                            column.Twips, column.Percent, column.HasPreferredPercent, column.Width);
            sb.Append(" SingleCellMatch: ");
            AppendDebugString(sb, column.SingleCellMatch);
            sb.Append("; ");
            sb.AppendFormat("Minimum: {0,5}; ContentMin: {1,5}; ContentMax: {2,5}",
                            column.Minimum, column.ContentMinimum, column.ContentMaximum);
            sb.AppendLine();
        }

        [Conditional("DEBUG")]
        private static void AppendDebugString(StringBuilder sbParent, GridCellMatch cellMatch)
        {
            StringBuilder sb = new StringBuilder(11);
            if (cellMatch.IsNone)
            {
                sb.Append(CellMatchNames[0]);
            }
            else
            {
                int valueIdx = 1;
                for (int value = 1; value <= (int)GridCellMatchType.Percent; value = value << 1)
                {
                    if (cellMatch.IsSet((GridCellMatchType)value))
                        sb.Append(CellMatchNames[valueIdx]);

                    valueIdx++;
                }
            }

            sbParent.AppendFormat("{0, 11}", sb.ToString());
        }

        private static string[] CellMatchNames = new string[]
        {
            "None", "Auto", "Twip", "Pct",
        };

        [Conditional("DEBUG")]
        internal static void DebugWriteCalculatedGrid(TableGridAttr grid)
        {
            if (!IsLogging)
                return;

            Debug.WriteLine(string.Format("*** Calculated grid, column count {0}:", grid.Columns.Count));
            for (int i = 0; i < grid.Columns.Count; ++i)
                Debug.WriteLine(grid.Columns[i]);
        }
    }
}
