// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/02/2017 by Alexander Zhiltsov

using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents a data reference (such as an Excel formula).
    /// (Stores data of the 2.24.3.28 CT_Formula complex types [MS-ODRAWXML].)
    /// </summary>
    internal class DmlChartFormula
    {
        /// <summary>
        /// Returns a copy of this instance.
        /// </summary>
        internal DmlChartFormula Clone()
        {
            return (DmlChartFormula)MemberwiseClone();
        }

        /// <summary>
        /// Gets or sets value of this formula.
        /// </summary>
        internal string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Gets or sets the orientation of the data reference.
        /// </summary>
        internal FormulaDirection Direction
        {
            get { return mDirection; }
            set { mDirection = value; }
        }

        private string mValue;
        private FormulaDirection mDirection;
    }
}
