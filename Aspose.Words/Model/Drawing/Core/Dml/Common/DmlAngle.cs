// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2010 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Common
{
    /// <summary>
    /// Represents an angle in 60000ths of a degree
    /// </summary>
    internal class DmlAngle
    {
        internal DmlAngle() :  this (0.0)
        {
        }

        internal DmlAngle(double value)
        {
            mValue = value;
            mRawValue = mValue;
        }

        internal DmlAngle Clone()
        {
            return (DmlAngle)MemberwiseClone();
        }

        internal static DmlAngle CreateWithNormalization(double value)
        {
            DmlAngle angle = new DmlAngle(value);
            angle.NormalizeAngle();
            return angle;
        }

        internal static DmlAngle FromDegrees(double degrees)
        {
            return new DmlAngle(ConvertUtilCore.DegreesToDmlAngles(degrees));
        }
        
        internal static DmlAngle FromRadians(double radians)
        {
            return new DmlAngle(ConvertUtilCore.RadiansToDmlAngles(radians));
        }

        public bool Equals(DmlAngle other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return other.mValue.Equals(mValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof (DmlAngle))
                return false;
            return Equals((DmlAngle)obj);
        }

        public override int GetHashCode()
        {
            return mValue.GetHashCode();
        }

        /// <summary>
        /// Normalizes specified angle so it becomes greater or equal to zero and less then 360 degrees.
        /// </summary>
        private void NormalizeAngle()
        {
            double angle360 = ConvertUtilCore.DegreesToDmlAngles(360.0f);
            if (System.Math.Abs(mValue) > angle360)
                mValue %= angle360;

            while (mValue < 0)
                mValue += angle360;
        }

        /// <summary>
        /// An angle in 60,000ths of a degree. Positive angles are clockwise 
        /// (i.e., towards the positive y axis); negative angles are 
        /// counter-clockwise (i.e., towards the negative y axis).
        /// </summary>
        internal double Value
        {
            get { return mValue; }
        }

        internal double ValueInRadians
        {
            get { return ConvertUtilCore.DmlAnglesToRadians(Value); }
        }

        internal double ValueInDegrees
        {
            get { return ConvertUtilCore.DmlAnglesToDegrees(Value); }
        }

        /// <summary>
        /// Returns original value read from document in degrees (without normalization)
        /// </summary>
        internal double RawValueInDegrees
        {
            get { return ConvertUtilCore.DmlAnglesToDegrees(mRawValue); }
        }

        private double mValue;
        private readonly double mRawValue;
    }
}
