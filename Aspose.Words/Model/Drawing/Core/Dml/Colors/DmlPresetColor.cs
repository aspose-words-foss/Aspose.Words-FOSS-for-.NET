// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2011 by Alexey Titov

using System;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Colors
{
    /// <summary>
    /// This element specifies a color which is bound to one of a predefined collection of colors.
    /// </summary>
    internal class DmlPresetColor : DmlColor
    {
        internal DmlPresetColor()
        {
        }

        internal DmlPresetColor(string value)
        {
            mValue = value;
        }

        protected override DrColor CreateUnmodifiedColor(IThemeProvider themeProvider)
        {
            return DrKnownColors.FromName(Value);
        }

        public override DmlColor Clone()
        {
            DmlPresetColor result = new DmlPresetColor();
            result.Value = Value;
            CopyColorModifiersTo(result);
            return result;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlPresetColor value = (DmlPresetColor)obj;

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
            get { return DmlColorType.PresetColor; }
        }

        /// <summary>
        /// Specifies the actual preset color value. 
        /// </summary>
        internal string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }
        /// <summary>
        /// Returns true, if this color value is empty or null.
        /// </summary>
        internal override bool IsEmpty
        {
            get { return !StringUtil.HasChars(mValue); }
        }
        
        private string mValue = String.Empty;
    }
}
