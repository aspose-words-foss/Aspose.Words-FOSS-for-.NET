// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/02/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Outlines
{
    /// <summary>
    /// 20.1.8.22 ds (Dash Stop)
    /// This element specifies a dash stop primitive. 
    /// Dashing schemes are built by specifying an ordered 
    /// list of dash stop primitive. A dash stop primitive 
    /// consists of a dash and a space.
    /// </summary>
    internal class DmlDashStop
    {
        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlDashStop value = (DmlDashStop)obj;

            return MathUtil.AreEqual(value.DashLength, DashLength) &&
                   MathUtil.AreEqual(value.SpaceLength, SpaceLength);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= DashLength.GetHashCode();
            hash ^= SpaceLength.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Specifies the length of the dash relative to the line width.
        /// </summary>
        internal double DashLength
        {
            get { return mDashLength; }
            set { mDashLength = value; }
        }

        /// <summary>
        /// Specifies the length of the space relative to the line width.
        /// </summary>
        internal double SpaceLength
        {
            get { return mSpaceLength; }
            set { mSpaceLength = value; }
        }

        private double mDashLength;
        private double mSpaceLength;

        public DmlDashStop Clone()
        {
            DmlDashStop result = new DmlDashStop();
            result.SpaceLength = SpaceLength;
            result.DashLength = DashLength;
            return result;
        }
    }
}