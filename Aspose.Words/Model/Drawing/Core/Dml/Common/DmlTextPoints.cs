// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Common
{
    /// <summary>
    /// Represents a size in increments of point.
    /// Whole points are specified in increments of 100 starting with 100 being a point size of 1. 
    /// For instance a font point size of 12 would be 1200 and a font point size of 12.5 would be 1250. 
    /// </summary>
    internal class DmlTextPoints
    {
        internal DmlTextPoints()
        {
        }

        public DmlTextPoints Clone()
        {
            return (DmlTextPoints)MemberwiseClone();
        }

        internal DmlTextPoints(int value)
        {
            mValue = value;
        }

        internal static DmlTextPoints FromPoints(double points)
        {
            return new DmlTextPoints((int) (points*ValuesInPoint));
        }

        internal static DmlTextPoints FromEmus(double emus)
        {
            return FromPoints(ConvertUtilCore.EmuToPoint(emus));
        }

        public bool Equals(DmlTextPoints other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return other.mValue == mValue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof (DmlTextPoints))
                return false;
            return Equals((DmlTextPoints)obj);
        }

        public override int GetHashCode()
        {
            return mValue;
        }

        internal int Value
        {
            get { return mValue; }
        }

        internal double ValueInPoints
        {
            get { return Value / ValuesInPoint; }
        }

        internal double ValueInEmus
        {
            get { return ConvertUtilCore.PointToEmu(ValueInPoints); }
        }

        private readonly int mValue;
        private const double ValuesInPoint = 100;
    }
}
