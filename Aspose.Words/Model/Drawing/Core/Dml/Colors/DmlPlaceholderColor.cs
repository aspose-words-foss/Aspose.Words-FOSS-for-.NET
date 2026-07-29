// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Colors
{
    /// <summary>
    /// This color can appear in themes. It is 
    /// represented by scheme color with value "phClr".
    /// </summary>
    internal class DmlPlaceholderColor : DmlColor
    {
        protected override DrColor CreateUnmodifiedColor(IThemeProvider themeProvider)
        {
            if (StyleColor != null)
                return StyleColor.CreateDrColor(themeProvider, null);
            else
                return DrColor.Empty;
        }

        public override DmlColor Clone()
        {
            DmlPlaceholderColor result = new DmlPlaceholderColor();
            if (StyleColor != null)
                result.StyleColor = StyleColor.Clone();
            CopyColorModifiersTo(result);
            return result;
        }

        public override DmlColorType ColorType
        {
            get { return DmlColorType.PlaceholderColor; }
        }

        public override void ApplyStyleColor(DmlColor styleColor)
        {
            StyleColor = styleColor;
        }

        /// <summary>
        /// Color assigned from style to the placeholder.
        /// </summary>
        internal DmlColor StyleColor
        {
            get { return mStyleColor; }
            set { mStyleColor = value; }
        }

        /// <summary>
        /// Returns true, if style color is null.
        /// </summary>
        internal override bool IsEmpty
        {
            get { return (mStyleColor == null); }
        }

        private DmlColor mStyleColor;
    }
}
