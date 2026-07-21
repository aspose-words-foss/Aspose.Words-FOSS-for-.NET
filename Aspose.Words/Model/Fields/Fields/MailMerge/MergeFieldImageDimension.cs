// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/10/2012 by Ivan Lyagin

using System;
using Aspose.Collections;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents an image dimension (i.e. the width or the height) used across a mail merge process.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// To indicate that the image should be inserted with its original dimension during a mail merge,
    /// you should assign a negative value to the <see cref="Value"/> property.
    /// <seealso cref="MergeFieldImageDimensionUnit"/>
    /// </remarks>
    public class MergeFieldImageDimension
    {
        /// <summary>
        /// Creates an image dimension instance with the given value in points.
        /// </summary>
        /// <remarks>
        /// You should use a negative value to indicate that the original value of the corresponding image dimension
        /// should be applied.
        /// </remarks>
        /// <param name="value">The value.</param>
        public MergeFieldImageDimension(double value)
        {
            // Assign through the properties to perform value checks.
            Value = value;
        }

        /// <summary>
        /// Creates an image dimension instance with the given value and the given unit.
        /// </summary>
        /// <remarks>
        /// You should use a negative value to indicate that the original value of the corresponding image dimension
        /// should be applied.
        /// </remarks>
        /// <param name="value">The value.</param>
        /// <param name="unit">The unit.</param>
        public MergeFieldImageDimension(double value, MergeFieldImageDimensionUnit unit)
        {
            // Assign through the properties to perform value checks.
            Value = value;
            Unit = unit;
        }

        /// <summary>
        /// Ctor for internal using.
        /// </summary>
        internal MergeFieldImageDimension()
        {
            Value = -1.0;
        }

        /// <summary>
        /// Tries to parse the value and the unit from the corresponding string representations and
        /// if succeeded, creates a <see cref="MergeFieldImageDimension"/> instance based on the parsed data.
        /// </summary>
        /// <remarks>
        /// Note that we do not use a <see cref="Boolean"/> result in conjunction with an out parameter
        /// not to create an extra array object on Java, as this method is internal anyways.
        /// </remarks>
        /// <param name="valueString">The string representation of the value.</param>
        /// <param name="unitString">The string representation of the unit.</param>
        /// <returns>The <see cref="MergeFieldImageDimension"/> instance or <c>null</c> if parsing
        /// has not been succeeded.</returns>
        internal static MergeFieldImageDimension TryParse(string valueString, string unitString)
        {
            double value = FormatterPal.TryParseDoubleInvariant(valueString);
            if (double.IsNaN(value))
                return null;

            // The empty unit string corresponds to the Point unit.
            MergeFieldImageDimensionUnit unit = MergeFieldImageDimensionUnit.Point;
            if (StringUtil.HasChars(unitString))
            {
                int unitInt = gUnitNameToValueMap[unitString];
                if (StringToIntDictionary.IsNullSubstitute(unitInt))
                    return null;

                unit = (MergeFieldImageDimensionUnit)unitInt;
            }

            MergeFieldImageDimension dimension = new MergeFieldImageDimension();
            dimension.Value = value;
            dimension.mUnit = unit;
            return dimension;
        }

        /// <summary>
        /// Creates a deep copy of the instance.
        /// </summary>
        /// <returns></returns>
        internal MergeFieldImageDimension Clone()
        {
            // A shallow copy is equal to a deep copy in this case, as all member variables of this class are of value types.
            return (MergeFieldImageDimension)MemberwiseClone();
        }

        /// <summary>
        /// Converts the value of the given <see cref="MergeFieldImageDimension"/> instance to the points.
        /// </summary>
        /// <param name="imageDimension">The <see cref="MergeFieldImageDimension"/> instance.</param>
        /// <param name="originalValueInPoints">The original value of the corresponding image dimension in points.</param>
        /// <returns></returns>
        internal static double ToPoints(MergeFieldImageDimension imageDimension, double originalValueInPoints)
        {
            // Return the original value of the corresponding image dimension if the given instance does not provide
            // a positive value.
            if (!HasValue(imageDimension))
                return originalValueInPoints;

            switch (imageDimension.mUnit)
            {
                case MergeFieldImageDimensionUnit.Point:
                    return imageDimension.Value;
                case MergeFieldImageDimensionUnit.Percent:
                    return originalValueInPoints * imageDimension.Value / 100.0;
                default:
                    // Throw the exception for the compiler. This code is to be unreachable.
                    throw new InvalidOperationException("Undefined MergeFieldImageDimensionUnit has been encountered.");
            }
        }

        /// <summary>
        /// Returns a value indicating whether the given <see cref="MergeFieldImageDimension"/> instance provides
        /// a significant (i.e. positive) value or not.
        /// </summary>
        internal static bool HasValue(MergeFieldImageDimension imageDimension)
        {
            return (imageDimension != null) && (imageDimension.Value >= 0);
        }

        /// <summary>
        /// The value.
        /// </summary>
        /// <remarks>
        /// You should use a negative value to indicate that the original value of the corresponding image dimension
        /// should be applied.
        /// </remarks>
        public double Value { get; set; }

        /// <summary>
        /// The unit.
        /// </summary>
        public MergeFieldImageDimensionUnit Unit
        {
            get { return mUnit; }
            set
            {
                switch (value)
                {
                    case MergeFieldImageDimensionUnit.Point:
                    case MergeFieldImageDimensionUnit.Percent:
                        mUnit = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
            }
        }

        private MergeFieldImageDimensionUnit mUnit = MergeFieldImageDimensionUnit.Point;

        /// <summary>
        /// Stores the map of the unit names to the corresponding unit values.
        /// </summary>
        private static readonly StringToIntDictionary gUnitNameToValueMap = BuildUnitNameToValueMap();

        private static StringToIntDictionary BuildUnitNameToValueMap()
        {
            StringToIntDictionary unitNameToValueMap = new StringToIntDictionary(false);
            unitNameToValueMap.Add("pt", (int)MergeFieldImageDimensionUnit.Point);
            unitNameToValueMap.Add("%", (int)MergeFieldImageDimensionUnit.Percent);
            return unitNameToValueMap;
        }
    }
}
