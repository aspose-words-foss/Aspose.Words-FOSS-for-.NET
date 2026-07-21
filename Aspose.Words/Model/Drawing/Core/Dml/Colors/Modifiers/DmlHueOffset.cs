// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.16 hueOff (Hue Offset)
    /// This element specifies the input color with its hue shifted, 
    /// but with its saturation and luminance unchanged.
    /// </summary>
    internal class DmlHueOffset : DmlColorModifier
    {
        public override DrColor Modify(DrColor color)
        {
            HSLColor hsl = new HSLColor(color);
            hsl.Hue += Value.ValueInDegrees/360.0;
            return new DrColor(color.A, hsl.ToDrColor());
        }

        public override IDmlColorModifier Clone()
        {
            DmlHueOffset result = new DmlHueOffset();
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
            DmlHueOffset hueOffset = obj as DmlHueOffset;
            return (hueOffset != null) && (((mValue == null) && (hueOffset.Value == null)) ||
                                           ((mValue != null) && mValue.Equals(hueOffset.Value))) &&
                   base.Equals(obj);
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            writer.Builder.WriteElementWithAttributes(
                string.Format("{0}:hueOff", prefix),
                DmlNamespaceUtil.GetAttrName(prefix, "val"),
                Value.Value);
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.HueOffset; }
        }

        public DmlAngle Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        private DmlAngle mValue = new DmlAngle();
    }
}
