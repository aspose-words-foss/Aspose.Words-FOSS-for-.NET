// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.202 strRef (String Reference) or 5.7.2.124 numRef (Number Reference) element.
    /// This element specifies a reference to data for a single data label or title with a cache of the last values used.
    /// Also it is used to store data of the CT_NumericDimension and CT_StringDimension complex types [MS-ODRAWXML].
    /// </summary>
    internal class DmlChartValueRef : DmlExtensionListSource
    {
        internal DmlChartValueRef(DmlChartValueType refType)
        {
            mValues = new DmlChartValueCollection(refType);
        }

        internal DmlChartValueRef(DmlChartValueCollection values)
        {
            mValues = values;
        }

        internal DmlChartValueRef Clone()
        {
            DmlChartValueRef lhs = (DmlChartValueRef)MemberwiseClone();
            lhs.mValues = mValues.Clone();

            if (mFormula != null)
                lhs.mFormula = mFormula.Clone();

            if (mNameFormula != null)
                lhs.mNameFormula = mNameFormula.Clone();

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Removes the formula and the extensions so that the instance represents literal data rather than a reference
        /// to XLSX content.
        /// </summary>
        /// <remarks>
        /// The formula becomes invalid after adding/removing chart data values and must be removed, otherwise
        /// MS Word will generate incorrect XLSX after performing Edit Data.
        /// </remarks>
        /// <dev>
        /// Data reference is stored as a 21.2.2.123 numRef (Number Reference) or 21.2.2.201 strRef (String Reference)
        /// element. Literal data is stored as a 21.2.2.122 numLit (Number Literal) or 21.2.2.200 strLit (String Literal)
        /// element.
        /// </dev>
        internal void MarkAsLiteralData()
        {
            if (IsLiteralData)
                return;

            mFormula = null;

            // Clear extensions because they may be incorrect for a numLit/strLit element.
            if (Extensions != null)
                Extensions.Clear();
        }

        internal DmlChartFormula Formula
        {
            get
            {
                if (mFormula == null)
                    mFormula = new DmlChartFormula();

                return mFormula;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether this instance is a container for literal data (a 21.2.2.200 strLit (String
        /// Literal) or 21.2.2.122 numLit (Number Literal) element). If value of the property is <c>false</c>, the
        /// instance is a 21.2.2.201 strRef (String Reference) or 21.2.2.123 numRef (Number Reference) element.
        /// </summary>
        internal bool IsLiteralData
        {
            get
            {
                return ((mFormula == null) || !StringUtil.HasChars(mFormula.Value));
            }
        }

        /// <summary>
        /// Gets or sets a formula that specifies dimension name reference.
        /// </summary>
        /// <remarks>
        /// It is the nf element of the CT_NumericDimension and CT_StringDimension complex types [MS-ODRAWXML]
        /// </remarks>
        internal DmlChartFormula NameFormula
        {
            get
            {
                if (mNameFormula == null)
                    mNameFormula = new DmlChartFormula();
                return mNameFormula;
            }
        }

        /// <summary>
        /// Gets series data.
        /// </summary>
        internal DmlChartValueCollection Values
        {
            get { return mValues; }
        }

        /// <summary>
        /// Gets or sets type of this dimension. 
        /// </summary>
        /// <remarks>
        /// Represents the 'type' attribute of the CT_NumericDimension or CT_StringDimension complex
        /// type [MS-ODRAWXML].
        /// </remarks>
        internal DimensionType DimensionType
        {
            get { return mDimensionType; }
            set { mDimensionType = value; }
        }

        private DmlChartFormula mFormula;
        private DmlChartFormula mNameFormula;
        private DmlChartValueCollection mValues;
        private DimensionType mDimensionType;
    }
}
