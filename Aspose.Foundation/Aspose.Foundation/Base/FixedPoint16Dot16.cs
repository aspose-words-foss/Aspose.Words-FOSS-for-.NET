// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2024 by Konstantin Kornilov

namespace Aspose
{
    /// <summary>
    /// Represents 16.16 fixed-point value.
    /// </summary>
    public class FixedPoint16Dot16 : FixedPoint
    {
        public FixedPoint16Dot16(int value)
        {
            Value = value;
        }

        private FixedPoint16Dot16(long value)
        {
            Value = (int)Clamp(value);
        }

        public FixedPoint16Dot16(double value)
        {
            Value = (int)Clamp(FromDoubleCore(value));
        }

        public FixedPoint16Dot16 Add(FixedPoint16Dot16 other)
        {
            return new FixedPoint16Dot16(AddCore(Value, other.Value));
        }

        public FixedPoint16Dot16 Sub(FixedPoint16Dot16 other)
        {
            return new FixedPoint16Dot16(SubCore(Value, other.Value));
        }

        public FixedPoint16Dot16 Mul(FixedPoint16Dot16 other)
        {
            return new FixedPoint16Dot16(MulCore(Value, other.Value));
        }

        public FixedPoint16Dot16 Div(FixedPoint16Dot16 other)
        {
            return new FixedPoint16Dot16(DivCore(Value, other.Value));
        }

        public FixedPoint16Dot16 Sqrt()
        {
            return new FixedPoint16Dot16(SqrtCore(Value));
        }

        public FixedPoint16Dot16 Neg()
        {
            return new FixedPoint16Dot16(NegCore(Value));
        }

        public FixedPoint16Dot16 Abs()
        {
            return new FixedPoint16Dot16(AbsCore(Value));
        }

        public bool Less(FixedPoint16Dot16 other)
        {
            return LessCore(Value, other.Value);
        }

        public FixedPoint16Dot16 Clamp(FixedPoint16Dot16 min, FixedPoint16Dot16 max)
        {
            if (Value < min.Value)
                return min;
            if (Value > max.Value)
                return max;
            return this;
        }

        public FixedPoint2Dot14 To2Dot14()
        {
            return new FixedPoint2Dot14((long)Value >> 2);
        }

        protected bool Equals(FixedPoint16Dot16 other)
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
            return Equals((FixedPoint16Dot16)obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public int Value { get; }
        public override double DoubleValue { get { return ToDoubleCore(Value); } }

        protected override int FractionLength { get { return 16; } }
        protected override long MinValue { get { return int.MinValue; } }
        protected override long MaxValue { get { return int.MaxValue; } }
    }
}
