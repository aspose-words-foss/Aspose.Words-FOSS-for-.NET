// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.8 constr (Constraint)
    /// </summary>
    internal class DmlConstraint
    {
        internal DmlBooleanOperator Operator
        {
            get { return mOperator; }
            set { mOperator = value; }
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

        internal DmlConstraintAttributes ConstraintAttributes
        {
            get { return mConstraintAttributes;}
            set { mConstraintAttributes = value; }
        }

        internal DmlConstraintAttributes ConstraintReferenceAttributes
        {
            get { return mConstraintReferenceAttributes;}
            set { mConstraintReferenceAttributes = value; }
        }

        internal DmlConstraint DeepCopy()
        {
            return (DmlConstraint)MemberwiseClone();
        }

        private DmlBooleanOperator mOperator = DmlBooleanOperator.None;
        private double mValue = 0d;
        private double mFactor = 1d;
        private DmlConstraintAttributes mConstraintAttributes = new DmlConstraintAttributes();
        private DmlConstraintAttributes mConstraintReferenceAttributes = new DmlConstraintAttributes();
    }
}
