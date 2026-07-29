// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Specify a position relative to a reference point measured in HIMETRIC units.
    /// </summary>
    internal class OlePosition
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal OlePosition(int top, int left)
        {
            Top = top;
            Left = left;
        }

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != this.GetType())
                return false;

            return Equals((OlePosition)obj);
        }

        /// <summary>
        /// Gets hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Top * 397) ^ Left;
            }
        }

#if TEST || DEBUG
        /// <summary>
        /// Converts the OlePosition object to a string.
        /// </summary>
        public override string ToString()
        {
            string raw = ArrayUtil.DumpArray(ToRaw());
            int topPt = MathUtil.DoubleToInt(ConvertUtilCore.HimetricToPoint(Top));
            int leftPt = MathUtil.DoubleToInt(ConvertUtilCore.HimetricToPoint(Left));
            return string.Format("Raw: {0}, (T:{1}hm ({3}pt), L:{2}hm ({4}pt))", raw, Top, Left, topPt, leftPt);
        }
#endif

        /// <summary>
        /// Creates a OlePosition object from a raw value.
        /// </summary>
        internal static OlePosition FromRaw(byte[] raw)
        {
            Debug.Assert(raw.Length == 8);
            return new OlePosition(BitConverter.ToInt32(raw, 4), BitConverter.ToInt32(raw, 0));
        }

        /// <summary>
        /// Converts this OlePosition object to a raw value.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstMethod]
        internal byte[] ToRaw()
        {
            // long raw = ((long)mTop << 32 | (long)mLeft);

            byte[] raw = new byte[8];
            BitConverter.GetBytes(Top).CopyTo(raw, 4);
            BitConverter.GetBytes(Left).CopyTo(raw, 0);

            return raw;
        }

        /// <summary>
        /// Creates a OlePosition object from Top and Left measured in points.
        /// </summary>
        internal static OlePosition FromPoints(double topInPoints, double leftInPoints)
        {
            return new OlePosition(ConvertUtilCore.PointToHimetricInt(topInPoints),
                ConvertUtilCore.PointToHimetricInt(leftInPoints));
        }

        /// <summary>
        /// Determines whether two OlePosition objects are equal.
        /// </summary>
        private bool Equals(OlePosition other)
        {
            return ((Top == other.Top) && (Left == other.Left));
        }

        /// <summary>
        /// Gets top-coordinate of this OlePosition, measured in HIMETRIC.
        /// </summary>
        internal int Top { get; }

        /// <summary>
        /// Gets left-coordinate of this OlePosition, measured in HIMETRIC.
        /// </summary>
        internal int Left { get; }
    }
}
