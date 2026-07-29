// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/12/2015 by Victor Chebotok

using System;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Parser.IEConditionalExpressions
{
    /// <summary>
    /// Represents a supported version vector (version number) of <see cref="Feature"/>. Supported version vectors are either
    /// one or two 32-bit non-negative integers delimited by a single dot. For example, "9", or "9.10", or "0.1"
    /// </summary>
    /// <remarks>
    /// For more information see https://msdn.microsoft.com/en-us/library/ms537512(v=vs.85).aspx#Version_Vectors.
    /// </remarks>
    internal class NumericVersionVector : VersionVector
    {
        internal NumericVersionVector(int major)
        {
            Debug.Assert(major >= 0);

            mMajor = major;
            mMinor = -1;
        }

        internal NumericVersionVector(int major, int minor)
        {
            Debug.Assert(major >= 0);
            Debug.Assert(minor >= 0);

            mMajor = major;
            mMinor = minor;
        }

        internal bool HasMinorPart
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mMinor >= 0; }
        }

        internal override bool CompareTo(VersionVector other, ComparisonOperation operation)
        {
            NumericVersionVector otherAsNumericVector = other as NumericVersionVector;
            if (otherAsNumericVector == null)
            {
                // Cannot compare unsupported types of version vectors.
                return false;
            }

            int compareResult = CompareTo(otherAsNumericVector);

            switch (operation)
            {
                case ComparisonOperation.Equal:
                    return compareResult == 0;
                case ComparisonOperation.Greater:
                    return compareResult > 0;
                case ComparisonOperation.GreaterOrEqual:
                    return compareResult >= 0;
                case ComparisonOperation.Less:
                    return compareResult < 0;
                case ComparisonOperation.LessOrEqual:
                    return compareResult <= 0;
                default:
                    // Unknown comparison operation.
                    throw new InvalidOperationException();
            }
        }

        public override string ToString()
        {
            string result = FormatterPal.IntToStr(mMajor);
            if (HasMinorPart)
            {
                result += "." + FormatterPal.IntToStr(mMinor);
            }
            return result;
        }

        private int CompareTo(NumericVersionVector other)
        {
            int result = mMajor.CompareTo(other.mMajor);

            // Minor parts of version vectors are considered only if both vectors has a minor part. This rule complies with
            // to comparison rules of IE conditional expressions.
            // For example, all the following version comparisons are true:
            //   9 == 9.1
            //   9.2 <= 9
            //   9.2 >= 9
            if ((result == 0) && HasMinorPart && other.HasMinorPart)
            {
                result = mMinor.CompareTo(other.mMinor);
            }

            return result;
        }

        /// <summary>
        /// The major part of the version vector. Always non-negative.
        /// </summary>
        private readonly int mMajor;

        /// <summary>
        /// The minor part of the version vector. If this part is negative, the version vector has no minor part and it is
        /// not considered during comparison to other vectors.
        /// </summary>
        private readonly int mMinor;
    }
}
