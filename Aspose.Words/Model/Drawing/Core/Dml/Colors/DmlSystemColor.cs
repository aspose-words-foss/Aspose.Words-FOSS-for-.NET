// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2011 by Alexey Titov

using System;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Colors
{
    /// <summary>
    /// This element specifies a color bound to predefined operating system elements.
    /// </summary>
    internal class DmlSystemColor : DmlColor
    {
        protected override DrColor CreateUnmodifiedColor(IThemeProvider themeProvider)
        {
            // WORDSNET-6840 Word uses actual system color value if possible.
            DrColor resultColor = DrColor.GetSystemColor(Value);

            if (resultColor == DrColor.Empty)
            {
                // If color is not found or not supported use LastColor.
                resultColor = (LastColor.Length > 0) ? DmlHexRgbColor.ReadHexColor(LastColor) : DrColor.Empty;
            }

            return resultColor;
        }

        public override DmlColor Clone()
        {
            DmlSystemColor result = new DmlSystemColor();
            result.Value = Value;
            result.LastColor = LastColor;
            CopyColorModifiersTo(result);
            return result;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlSystemColor value = (DmlSystemColor)obj;

            return object.Equals(value.Value, Value);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Value.GetHashCode();
            return hash;
        }

        public override DmlColorType ColorType
        {
            get { return DmlColorType.SystemColor; }
        }

        /// <summary>
        /// Specifies the system color value.
        /// </summary>
        internal string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Specifies the color value that was last computed by the generating application.
        /// </summary>
        internal string LastColor
        {
            get
            {
                if (mLastColor == null)
                    mLastColor = String.Empty;
                return mLastColor;
            }
            set { mLastColor = value; }
        }

        /// <summary>
        /// Returns true, if this color value is empty or null.
        /// </summary>
        internal override bool IsEmpty
        {
            get { return !StringUtil.HasChars(mValue); }
        }

        private string mLastColor;
        private string mValue = String.Empty;
    }
}
