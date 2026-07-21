// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov


namespace Aspose.Words.Drawing.Core.Dml.Text
{
    internal class DmlPercentageTextSpacing : DmlTextSpacing
    {
        internal DmlPercentageTextSpacing()
            : this(1.0) // Default is 100% spacing
        {
        }

        internal DmlPercentageTextSpacing(double value)
        {
            mValue = value;
        }

        /// <summary>
        /// Clones this instance of text spacing.
        /// </summary>
        internal override DmlTextSpacing Clone()
        {
            return (DmlPercentageTextSpacing)MemberwiseClone();
        }

        /// <summary>
        /// Value is in fraction representation.
        /// </summary>
        internal double Value
        {
            get { return mValue; }
        }

        private readonly double mValue;
    }
}
