// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.24 rule (Rule)
    /// </summary>
    internal class DmlNumericRule
    {
        internal DmlConstraintAttributes ConstraintAttributes
        {
            get { return mConstraintAttributes; }
            set { mConstraintAttributes = value; }
        }

        internal double Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        internal double Factor
        {
            get { return mFactor; }
            set { mFactor = value; }
        }

        internal double Max
        {
            get { return mMax; }
            set { mMax = value; }
        }

        internal DmlNumericRule DeepCopy()
        {
            return (DmlNumericRule)MemberwiseClone();
        }

        private double mValue = double.NaN;
        private double mFactor = double.NaN;
        private double mMax = double.MaxValue;
        private DmlConstraintAttributes mConstraintAttributes = new DmlConstraintAttributes();
    }
}
