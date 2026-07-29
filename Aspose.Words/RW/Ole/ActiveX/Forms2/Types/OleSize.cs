// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Specifies size of control measured in HIMETRIC units.
    /// </summary>
    internal class OleSize
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal OleSize(int width, int height)
        {
            Width = width;
            Height = height;
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

            return Equals((OleSize)obj);
        }

        /// <summary>
        /// Gets hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Width * 397) ^ Height;
            }
        }

#if TEST || DEBUG
        /// <summary>
        /// Converts the OleSize object to a string.
        /// </summary>
        public override string ToString()
        {
            long raw = (long)Width | ((long)Height << 32);
            return string.Format("Raw:0x{0:X8}, (W:{1}hm, H:{2}hm)", raw, Width, Height);
        }
#endif

        /// <summary>
        /// Creates a OleSize object from raw.
        /// </summary>
        internal static OleSize FromRaw(byte[] raw)
        {
            Debug.Assert(raw.Length == 8);
            return new OleSize(BitConverter.ToInt32(raw, 0), BitConverter.ToInt32(raw, 4));
        }

        /// <summary>
        /// Gets raw value of this OleSize object.
        /// </summary>
        internal byte[] ToRaw()
        {
            long raw = (long)Width | ((long)Height << 32);
            return BitConverter.GetBytes(raw);
        }

        /// <summary>
        /// Creates a OleSize object from Width and Height measured in points.
        /// </summary>
        internal static OleSize FromPoints(double widthInPoints, double heightInPoints)
        {
            return new OleSize(ConvertUtilCore.PointToHimetricInt(widthInPoints),
                ConvertUtilCore.PointToHimetricInt(heightInPoints));
        }

        /// <summary>
        /// Determines whether two OleSize objects are equal.
        /// </summary>
        private bool Equals(OleSize other)
        {
            Debug.Assert(other != null);

            return (Width == other.Width) &&
                   (Height == other.Height);
        }

        /// <summary>
        /// Gets width of this OleSize, measured in HIMETRIC.
        /// </summary>
        internal int Width { get; }

        /// <summary>
        /// Gets height of this OleSize, measured in HIMETRIC.
        /// </summary>
        internal int Height { get; }
    }
}
