// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Colors
{
    /// <summary>
    /// This element specifies a color bound to a user's theme.
    /// As with all elements which define a color,
    /// it is possible to apply a list of color transforms to the base color defined.
    /// </summary>
    internal class DmlSchemeColor : DmlColor
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        internal DmlSchemeColor()
        {
        }

        /// <summary>
        /// Ctor with specifying the desired theme color.
        /// </summary>
        internal DmlSchemeColor(ThemeColor value)
        {
            Value = value;
        }

        protected override DrColor CreateUnmodifiedColor(IThemeProvider themeProvider)
        {
            if(themeProvider == null)
                return DrColor.Empty;

            DmlColor themeColor = themeProvider.GetThemeColor(Value);
            return themeColor.CreateDrColor(themeProvider, null);
        }

        public override DmlColor Clone()
        {
            DmlSchemeColor result = new DmlSchemeColor(Value);
            CopyColorModifiersTo(result);
            return result;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlSchemeColor value = (DmlSchemeColor)obj;

            return object.Equals(value.Value, Value);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Value.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Resolves this scheme color to a concrete RGB color using a specified theme provider.
        /// </summary>
        internal DmlHexRgbColor Resolve(IThemeProvider themeProvider)
        {
            DrColor drColor = CreateUnmodifiedColor(themeProvider);
            DmlHexRgbColor dmlHexRgbColor = DmlHexRgbColor.FromDrColor(drColor);
            CopyColorModifiersTo(dmlHexRgbColor);

            return dmlHexRgbColor;
        }

        public override DmlColorType ColorType
        {
            get { return DmlColorType.SchemeColor; }
        }

        /// <summary>
        /// Specifies the desired scheme.
        /// </summary>
        internal ThemeColor Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        private ThemeColor mValue;
    }
}
