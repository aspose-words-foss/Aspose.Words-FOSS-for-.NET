// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/04/2024 by Vyacheslav Deryushev

using Aspose.Common;
using Aspose.Words.Drawing.Core.Dml.Guides;
using Aspose.Words.RW.Vml;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Represents adjustment values that are applied to the specified shape.
    /// </summary>
    public class Adjustment
    {
        /// <summary>
        /// Creates instance of the <see cref="Adjustment"/> class.
        /// </summary>
        internal Adjustment(string name, int value, DmlOneArgumentFormula formula, ShapeBase shape)
        {
            mName = name;
            mValue = value;
            mFormula = formula;
            mShape = shape;
        }

        /// <summary>
        /// Creates instance of the <see cref="Adjustment"/> class.
        /// </summary>
        internal Adjustment(int index, int value, ShapeBase shape)
        {
            mIndex = index;
            mName = string.Format("adj{0}", mIndex);
            mValue = value;
            mShape = shape;
        }

        /// <summary>
        /// Gets the name of the adjustment.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets or sets the raw value of the adjustment.
        /// </summary>
        /// <remarks>
        /// An adjust value is simply a guide that has a value based formula specified.
        /// That is, no calculation takes place for an adjust value guide.
        /// Instead, this guide specifies a parameter value that is used for calculations within the shape guides.
        /// </remarks>
        public int Value
        {
            get
            {
                return mValue;
            }
            set
            {
                if (mValue == value)
                    return;

                mValue = value;

                if (mShape.IsDmlShape)
                {
                    mFormula.Source = string.Format("val {0}", value);
                    mFormula.X = FormatterPal.IntToStr(value);
                }
                else
                {
                    int adjustKey = VmlUtil.GetAdjustKey(mIndex);
                    mShape.ShapePr[adjustKey] = value;
                }
            }
        }

        private int mValue;
        private readonly int mIndex;
        private readonly string mName;
        private readonly DmlOneArgumentFormula mFormula;
        private readonly ShapeBase mShape;
    }
}
