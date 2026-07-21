// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/03/2011 by Alexey Titov

using System;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// 20.1.8.36 gs (Gradient stops)
    /// This element defines a gradient stop. A gradient stop consists of
    /// a position where the stop appears in the color band.
    /// </summary>
    internal class DmlGradientStop : IComparable<DmlGradientStop>
    {
        /// <summary>
        /// Ctor without parameters.
        /// </summary>
        internal DmlGradientStop()
        {
        }

        /// <summary>
        /// Ctor that allows setting object properties.
        /// </summary>
        internal DmlGradientStop(double position, DmlColor color)
        {
            mPosition = position;
            mColor = color;
        }

        public int CompareTo(DmlGradientStop gradientStop)
        {
            if (gradientStop == null)
                throw new ArgumentNullException("obj");

            int val = Position.CompareTo(gradientStop.Position);
            if (val == 0)
                return OriginalOrder.CompareTo(gradientStop.OriginalOrder);

            return val;
        }

        [JavaAttributes.JavaDelete]
        public int CompareTo(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            // In Dml can be more then one DmlGradientStop with the same stop position,
            // and we have to preserve their position upon sorting.
            DmlGradientStop gradientStop = obj as DmlGradientStop;
            if (gradientStop != null)
            {
                int val = Position.CompareTo(gradientStop.Position);
                if (val == 0)
                    return OriginalOrder.CompareTo(gradientStop.OriginalOrder);

                return val;
            }

            throw new ArgumentException("Object is not DmlGradientStop.");
        }

        public DmlGradientStop Clone()
        {
            DmlGradientStop result = new DmlGradientStop();
            result.Color = Color.Clone();
            result.Position = Position;
            return result;
        }


        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlGradientStop value = (DmlGradientStop)obj;

            return object.Equals(value.Color, Color) && MathUtil.AreEqual(value.Position, Position);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= Color.GetHashCode();
            hash ^= Position.GetHashCode();
            return hash;
        }

#if DEBUG
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0:P1}: {1}; <{2}>", Position, Color, OriginalOrder);
        }
#endif


        internal DmlColor Color
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mColor == null)
                    mColor = new DmlHexRgbColor();
                return mColor;
            }
            set { mColor = value; }
        }

        /// <summary>
        /// Specifies where this gradient stop should appear in the color band.
        /// This position is specified in the range [0%, 100%], which corresponds
        /// to the beginning and the end of the color band respectively.
        /// Value is in fraction representation.
        /// </summary>
        internal double Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        internal int OriginalOrder
        {
            get { return mOriginalOrder; }
            set { mOriginalOrder = value; }
        }

        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private DmlColor mColor;
        private double mPosition;
        private int mOriginalOrder;
    }
}
