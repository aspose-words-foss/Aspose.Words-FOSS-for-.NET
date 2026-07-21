// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2024 by Konstantin Kornilov

namespace Aspose
{
    /// <summary>
    /// Represents 2.14 fixed-point value.
    /// </summary>
    public class FixedPoint2Dot14 : FixedPoint
    {
        public FixedPoint2Dot14(short value)
        {
            Value = value;
        }

        internal FixedPoint2Dot14(long value)
        {
            Value = (short)Clamp(value);
        }

        public FixedPoint2Dot14(double value)
        {
            Value = (short)Clamp(FromDoubleCore(value));
        }

        public bool Less(FixedPoint2Dot14 other)
        {
            return Value < other.Value;
        }

        public bool Greater(FixedPoint2Dot14 other)
        {
            return Value > other.Value;
        }

        public bool GreaterOrEqual(FixedPoint2Dot14 other)
        {
            return Value >= other.Value;
        }

        public bool GreaterOrEqual(FixedPoint16Dot16 other)
        {
            return To16Dot16Value() >= other.Value;
        }

        public FixedPoint16Dot16 To16Dot16()
        {
            return new FixedPoint16Dot16(To16Dot16Value());
        }

        private int To16Dot16Value()
        {
            return Value << 2;
        }

        protected bool Equals(FixedPoint2Dot14 other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((FixedPoint2Dot14)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public short Value { get; }
        public override double DoubleValue { get { return ToDoubleCore(Value); } }
        public float FloatValue { get { return (float)DoubleValue; } }

        protected override int FractionLength { get { return 14; } }
        protected override long MaxValue { get { return short.MaxValue; } }
        protected override long MinValue { get { return short.MinValue; } }
    }
}
