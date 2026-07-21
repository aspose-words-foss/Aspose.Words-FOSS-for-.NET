// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.14 hue (Hue)
    /// This element specifies the input color with the specified hue, 
    /// but with its saturation and luminance unchanged.
    /// </summary>
    internal class DmlHue : DmlColorModifier
    {
        public override DrColor Modify(DrColor color)
        {
            HSLColor hsl = new HSLColor(color);
            hsl.Hue = Value.ValueInDegrees/360.0;
            return new DrColor(color.A, hsl.ToDrColor());
        }

        public override IDmlColorModifier Clone()
        {
            DmlHue result = new DmlHue();
            result.Value = Value;
            return result;
        }

        public override int GetHashCode()
        {
            int hash = ModifierType.GetHashCode();
            if (Value != null)
                hash ^= Value.Value.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            DmlHue hue = obj as DmlHue;
            return (hue != null) && (((mValue == null) && (hue.Value == null)) ||
                                     ((mValue != null) && mValue.Equals(hue.Value))) &&
                   base.Equals(obj);
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            writer.Builder.WriteElementWithAttributes(
                string.Format("{0}:hue", prefix),
                DmlNamespaceUtil.GetAttrName(prefix, "val"),
                Value.Value);
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.Hue; }
        }

        /// <summary>
        /// Specifies the angular value describing the wavelength. Expressed in 1/60000ths of a degree.
        /// </summary>
        public DmlAngle Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        private DmlAngle mValue;
    }
}
