// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2024 by Konstantin Kornilov

using System;
using System.Globalization;

namespace Aspose
{
    /// <summary>
    /// Base class for fixed point numbers.
    /// </summary>
    /// <remarks>
    /// There is a lot of room for optimization. For now these classes are not used in heavy scenarios.
    /// The target for now are precise calculations.
    /// </remarks>
    public abstract class FixedPoint
    {
        protected long Clamp(long value)
        {
            if (value > MaxValue)
                return MaxValue;
            if (value < MinValue)
                return MinValue;
            return value;
        }

        protected double ToDoubleCore(long value)
        {
            return value / FractionDivider;
        }

        protected long FromDoubleCore(double value)
        {
            return FromDoubleCore(value, FractionDivider);
        }

        protected static long FromDoubleCore(double value, double fractionDivider)
        {
            return (long)Math.Round(value * fractionDivider);
        }

        protected long FromIntCore(int value)
        {
            return (long)(value * FractionDivider);
        }

        protected static long AddCore(long a, long b)
        {
            return a + b;
        }

        protected static long SubCore(long a, long b)
        {
            return a - b;
        }

        protected long MulCore(long a, long b)
        {
            return (a * b) >> FractionLength;
        }

        protected long DivCore(long a, long b)
        {
            return (a << FractionLength) / b;
        }

        protected long SqrtCore(long a)
        {
            // Not exactly correct implementation. Ignore for now.
            return FromDoubleCore(Math.Sqrt(ToDoubleCore(a)));
        }

        protected static long NegCore(long a)
        {
            return -a;
        }

        protected static long AbsCore(long a)
        {
            return Math.Abs(a);
        }

        protected static bool LessCore(long a, long b)
        {
            return a < b;
        }

        public abstract double DoubleValue { get; }

#if DEBUG
        public override string ToString()
        {
            return DoubleValue.ToString(CultureInfo.InvariantCulture);
        }
#endif

        protected abstract int FractionLength { get; }
        private double FractionDivider { get { return 1 << FractionLength; } }
        protected abstract long MaxValue { get; }
        protected abstract long MinValue { get; }
    }
}
