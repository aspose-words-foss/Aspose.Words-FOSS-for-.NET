// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2014 by Alexey Butalov

using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html
{
    /// <summary>
    /// Represents CSS margins. 
    /// </summary>
    internal class CssMargins : CssDimensions
    {
        internal CssMargins()
            : base("margin-")
        {
        }

        internal CssMargins(double top, double right, double bottom, double left)
            : this()
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }

        internal CssMargins(CssStyle style)
            : this()
        {
            Debug.Assert(style != null);
            FromStyle(style);
        }

        private CssMargins(CssMargins other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new CssMargins(this);
        }

        /// <summary>
        /// Converts logical margins to phisical margins.
        /// </summary>
        /// <param name="blockFlowDirection">Block flow direction.</param>
        /// <param name="rtl">Indicates whether text direction is right-to-left.</param>
        internal CssMargins ToPhisicalMargins(CssBlockFlowDirection blockFlowDirection, bool rtl)
        {
            return (CssMargins)ToPhisical(blockFlowDirection, rtl);
        }

        /// <summary>
        /// Converts phisical margins to logical margins.
        /// </summary>
        /// <param name="blockFlowDirection">Block flow direction.</param>
        /// <param name="rtl">Indicates whether text direction is right-to-left.</param>
        internal CssMargins ToLogicalMargins(CssBlockFlowDirection blockFlowDirection, bool rtl)
        {
            return (CssMargins)ToLogical(blockFlowDirection, rtl);
        }
    }
}