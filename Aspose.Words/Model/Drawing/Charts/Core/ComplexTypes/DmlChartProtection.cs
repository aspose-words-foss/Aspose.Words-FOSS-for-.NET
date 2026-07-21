// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.150 protection (Protection) element.
    /// This element specifies protection for the chart.
    /// </summary>
    internal class DmlChartProtection
    {
        internal DmlChartProtection Clone()
        {
            return (DmlChartProtection)MemberwiseClone();
        }

        internal bool ChartObject
        {
            get { return mChartObject; }
            set { mChartObject = value; }
        }

        internal bool Data
        {
            get { return mData; }
            set { mData = value; }
        }

        internal bool Formatting
        {
            get { return mFormatting; }
            set { mFormatting = value; }
        }

        internal bool Selection
        {
            get { return mSelection; }
            set { mSelection = value; }
        }

        internal bool UserInterface
        {
            get { return mUserInterface; }
            set { mUserInterface = value; }
        }

        internal bool HasProtection
        {
            get { return ChartObject || Data || Formatting || Selection || UserInterface; }
        }

        private bool mChartObject;
        private bool mData;
        private bool mFormatting;
        private bool mSelection;
        private bool mUserInterface;
    }
}
