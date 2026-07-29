// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2006 by Roman Korchagin

using Aspose.Drawing;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Defines attributes of a horizontal rule shape.
    /// </summary>
    internal class HorizontalRule
    {
        internal HorizontalRule(IShapeAttrSource parent)
        {
            Debug.Assert(parent != null);
            mParent = parent;
        }

        /// <summary>
        /// True when this shape is a horizontal rule.
        /// </summary>
        internal bool On
        {
            get { return (bool)FetchAttr(ShapeAttr.HROn); }
            set { SetAttr(ShapeAttr.HROn, value); }
        }

        /// <summary>
        /// True when this horizontal rule is drawn using one color.
        /// </summary>
        internal bool NoShade
        {
            get { return (bool)FetchAttr(ShapeAttr.HRNoShade); }
            set { SetAttr(ShapeAttr.HRNoShade, value); }
        }

        /// <summary>
        /// True when this horizontal rule is a standard one (does not use a picture).
        /// </summary>
        internal bool Standard
        {
            get { return (bool)FetchAttr(ShapeAttr.HRStandard); }
            set { SetAttr(ShapeAttr.HRStandard, value); }
        }

        /// <summary>
        /// Specifies alignment of the horizontal rule.
        /// </summary>
        internal HorizontalRuleAlignment Align
        {
            get { return (HorizontalRuleAlignment)FetchAttr(ShapeAttr.HRAlign); }
            set { SetAttr(ShapeAttr.HRAlign, value); }
        }

        /// <summary>
        /// Gets or sets the width of the horizontal rule in percent.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 100.</p>
        /// <p>If this value is zero, then the width of the horizontal rule is specified
        /// by the shape size in points.</p>
        /// </remarks>
        internal double Percent
        {
            get { return TenthsPercentToPercent((int)FetchAttr(ShapeAttr.HRPct)); }
            set { SetPercentCore(value, true); }
        }

        internal void SetPercentSafe(double value)
        {
            SetPercentCore(value, false);
        }

        /// <summary>
        /// Sets percentage value to the horizontal rule.
        /// </summary>
        private void SetPercentCore(double value, bool isThrow)
        {
            value = ArgumentUtil.ValidateRange(value, 0, 0, 100, 100, isThrow, "percent");
            SetAttr(ShapeAttr.HRPct, PercentToTenthsPercent(value));
        }

        private static double TenthsPercentToPercent(int value)
        {
            return value / 10.0;
        }

        private static int PercentToTenthsPercent(double value)
        {
            return (int)(value * 10.0);
        }

        private object FetchAttr(int key)
        {
            return mParent.FetchShapeAttr(key);
        }

        private void SetAttr(int key, object value)
        {
            mParent.SetShapeAttr(key, value);
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IShapeAttrSource mParent;

        internal static readonly DrColor DefaultColor = DrColor.Gray;
    }
}
