// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2016 by Alexander Zhiltsov

using System;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 2.8.3.6 CT_StyleColor [MS-ODRAWXML]
    /// The complex type specifies a color which is retrieved from CT_ColorStyle (section 2.8.3.2).
    /// </summary>
    internal class DmlChartStyleColor : DmlColor
    {
        protected override DrColor CreateUnmodifiedColor(IThemeProvider themeProvider)
        {
            // Now only reading/writing is supported.
            return null;
        }

        /// <summary>
        /// Clones this <see cref="DmlChartStyleColor"/> object.
        /// </summary>
        public override DmlColor Clone()
        {
            DmlChartStyleColor result = new DmlChartStyleColor();
            result.Value = Value;
            CopyColorModifiersTo(result);
            return result;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlChartStyleColor value = (DmlChartStyleColor)obj;

            return object.Equals(value.Value, Value);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Value.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Gets type of this color.
        /// </summary>
        public override DmlColorType ColorType
        {
            get { return DmlColorType.ChartStyleColor; }
        }

        /// <summary>
        /// Specifies what index to use when retrieving a color from a color style.
        /// Represents val: an ST_StyleColorVal attribute that specifies the value which is used to determine the index
        /// of the color in a CT_ColorStyle.
        /// </summary>
        internal string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Gets the color index to use when retrieving a color from a color style.
        /// </summary>
        internal int ColorIndex
        {
            get
            {
                int index = FormatterPal.TryParseInt(mValue);
                return (index >= 0) ? index : 0;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether a chart element index should be used as a color index.
        /// </summary>
        internal bool IsAuto
        {
            get { return string.Equals(Value, "auto", StringComparison.Ordinal); }
        }

        private string mValue;
    }
}
