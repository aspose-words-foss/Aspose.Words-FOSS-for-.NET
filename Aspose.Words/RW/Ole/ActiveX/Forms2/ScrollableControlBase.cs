// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/12/2018 by Ilya Navrotskiy

using System;
using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// The base class for scrollable controls such as SpinButton or ScrollBar.
    /// </summary>
    internal abstract class ScrollableControlBase : Forms2OleControl
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal ScrollableControlBase(string name, Forms2Pr pr = null) : base(name, pr)
        {
        }

        /// <summary>
        /// Gets or sets integer that specifies the minimum acceptable value for the <see cref="Position"/> property.
        /// </summary>
        /// <remarks>
        /// Note, Min can be set greater than Max and vice versa.
        /// </remarks>
        internal int Min
        {
            get { return (int)Pr.FetchAttr(Forms2Attr.Min); }
            set
            {
                Pr.SetAttr(Forms2Attr.Min, value);
                // Adjust position accordingly to fit in range of (Min, Max).
                int curPosition = Position;
                int max = Max;
                if (!MathUtil.IsInRange(curPosition, value, max))
                {
                    Position = (value < max)
                        ? (int)MathUtil.FitToRange(curPosition, value, max)
                        : (int)MathUtil.FitToRange(curPosition, max, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets integer that specifies the maximum acceptable value for the <see cref="Position"/> property.
        /// </summary>
        /// <remarks>
        /// Note, Max can be set less than Min and vice versa.
        /// </remarks>
        internal int Max
        {
            get { return (int)Pr.FetchAttr(Forms2Attr.Max); }
            set
            {
                Pr.SetAttr(Forms2Attr.Max, value);
                // Adjust position accordingly to fit in range of (Min, Max).
                int curPosition = Position;
                int min = Min;
                if (!MathUtil.IsInRange(curPosition, min, value))
                {
                    Position = (min < value)
                        ? (int)MathUtil.FitToRange(curPosition, min, value)
                        : (int)MathUtil.FitToRange(curPosition, value, min);
                }
            }
        }

        /// <summary>
        /// Gets or sets integer that specifies the value of a control.
        /// </summary>
        internal int Position
        {
            get { return (int)Pr.FetchAttr(Forms2Attr.Position); }
            set
            {
                if (!MathUtil.IsInRange(value, Min, Max))
                {
                    throw new ArgumentOutOfRangeException("value",
                        "Could not set the Position property. Invalid property value.");
                }

                Pr.SetAttr(Forms2Attr.Position, value);
            }
        }
    }
}
