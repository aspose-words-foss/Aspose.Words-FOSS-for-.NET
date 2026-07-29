// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2011 by Alexey Titov

using System;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Colors
{
    /// <summary>
    /// This element specifies a color using the red, green, blue RGB color model.
    /// Red, green, and blue is expressed as sequence of hex digits, RRGGBB.
    /// A perceptual gamma of 2.2 is used.
    /// </summary>
    internal class DmlHexRgbColor : DmlColor
    {
        internal DmlHexRgbColor()
        {
        }

        internal DmlHexRgbColor(string value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlHexRgbColor value = (DmlHexRgbColor)obj;

            return object.Equals(value.Value, Value);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Value.GetHashCode();
            return hash;
        }

        protected override DrColor CreateUnmodifiedColor(IThemeProvider themeProvider)
        {
            if (IsEmpty)
                return DrColor.Empty;
            return ReadHexColor(Value);
        }

        public override DmlColor Clone()
        {
            DmlHexRgbColor result = new DmlHexRgbColor();
            result.Value = Value;
            result.mColorIndex = mColorIndex;
            CopyColorModifiersTo(result);
            return result;
        }

#if DEBUG
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return string.Format("0x{0}", Value);
        }
#endif

        public override DmlColorType ColorType
        {
            get { return DmlColorType.HexRgbColor; }
        }

        /// <summary>
        /// Initialize DrColor from following string RRGGBB
        /// </summary>
        internal static DrColor ReadHexColor(string value)
        {
            // We need to add 0xff alpha to the rgb color.
            int argb = FormatterPal.ParseHex(value) | unchecked((int)0xff000000);
            return new DrColor(argb);
        }

        internal static DmlHexRgbColor FromDrColor(DrColor drColor)
        {
            DmlHexRgbColor result = new DmlHexRgbColor();

            // We need to remove 0xff alpha from the drColor.
            result.Value = FormatterPal.IntToStrX8(drColor.ToArgb()).Substring(2);
            return result;
        }

        /// <summary>
        /// The actual color value. Expressed as a sequence of hex digits RRGGBB.
        /// </summary>
        internal string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Index into a color table specified by the "indexedColors" element.
        /// </summary>
        /// <remarks>
        /// This attribute used for maintain compatibility with implementations
        /// of Office Open XML file formats. When present in the context of a spreadsheet application,
        /// this attribute overrides any other color information present under its parent.
        /// </remarks>
        internal string ColorIndex
        {
            get { return mColorIndex;  }
            set { mColorIndex = value; }
        }

        /// <summary>
        /// Returns true, if this color value is empty or null.
        /// </summary>
        internal override bool IsEmpty
        {
            get { return !StringUtil.HasChars(mValue); }
        }

        private string mValue = String.Empty;
        private string mColorIndex;
    }
}
