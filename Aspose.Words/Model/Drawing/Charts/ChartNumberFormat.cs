// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/07/2017 by Andrey Noskov

using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents number formatting of the parent element.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    public class ChartNumberFormat
    {
        internal ChartNumberFormat(INumberFormatProvider numFmtProvider, DmlChart chart)
        {
            mNumFmtProvider = numFmtProvider;
            mChart = chart;
        }

        internal void Warn(WarningType warningType, string message)
        {
            if (mChart != null)
                mChart.Warn(warningType, message);
        }

        /// <summary>
        /// If the underlying number format value refers to inherited property value, i.e. to number format of a parent
        /// collection, the method removes this reference and assigns blank value (source-linked) to the number format.
        /// </summary>
        private void UnlinkFromParent()
        {
            if (mNumFmtProvider.IsInherited)
                NumFmt = new DmlChartNumFormat();
        }

        /// <summary>
        /// Gets or sets the format code applied to a data label.
        /// </summary>
        /// <remarks>
        /// Number formatting is used to change the way a value appears in data label and can be used in some very creative ways.
        /// The examples of number formats:
        /// <para>Number - "#,##0.00"</para>
        /// <para>Currency - "\"$\"#,##0.00"</para>
        /// <para>Time - "[$-x-systime]h:mm:ss AM/PM"</para>
        /// <para>Date - "d/mm/yyyy"</para>
        /// <para>Percentage - "0.00%"</para>
        /// <para>Fraction - "# ?/?"</para>
        /// <para>Scientific - "0.00E+00"</para>
        /// <para>Text - "@"</para>
        /// <para>Accounting - "_-\"$\"* #,##0.00_-;-\"$\"* #,##0.00_-;_-\"$\"* \"-\"??_-;_-@_-"</para>
        /// <para>Custom with color - "[Red]-#,##0.0"</para>
        /// </remarks>
        public string FormatCode
        {
            get
            {
                return (NumFmt != null) ? NumFmt.SourceFormatCode : "";
            }
            set
            {
                UnlinkFromParent();

                // Mimic MSW behavior and set NumberFormatIsLinkedToSource = false when any custom number format is set.
                IsLinkedToSource = false;

                if (StringUtil.HasChars(value))
                {
                    NumFmt.FormatCode = value;
                }
                else
                {
                    NumFmt.FormatCode = GeneralFormatCode;
                    Warn(WarningType.Hint, "An empty string is incorrect NumberFormat, changed to General.");
                }
            }
        }

        /// <summary>
        /// Specifies whether the format code is linked to a source cell.
        /// Default is true.
        /// </summary>
        /// <remarks>The NumberFormat will be reset to general if format code is linked to source.</remarks>
        public bool IsLinkedToSource
        {
            get
            {
                return LinkedToSource();
            }
            set
            {
                if (LinkedToSource() == value)
                    return;

                UnlinkFromParent();

                if (NumFmt == null)
                    NumFmt = new DmlChartNumFormat();

                NumFmt.SourceLinked = value;
                NumFmt.FormatCode = value ? "" : GeneralFormatCode;
            }
        }

        private bool LinkedToSource()
        {
            return (NumFmt == null) || NumFmt.SourceLinked;
        }

        private DmlChartNumFormat NumFmt
        {
            get { return mNumFmtProvider.NumFmt_INumberFormatProvider; }
            set { mNumFmtProvider.NumFmt_INumberFormatProvider = value; }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly INumberFormatProvider mNumFmtProvider;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DmlChart mChart;

        /// <summary>
        /// The General format is the default number format that MSW applies when you type a number. 
        /// </summary>
        internal const string GeneralFormatCode = "General";

        /// <summary>
        /// Default date format. When this format is specified, MS Word uses current culture date format.
        /// </summary>
        internal const string DefaultDateFormatCode = "m/d/yyyy";

        /// <summary>
        /// Default time format.
        /// </summary>
        internal const string DefaultTimeFormatCode = "h:mm";
    }
}
