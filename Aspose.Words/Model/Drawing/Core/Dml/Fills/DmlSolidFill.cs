// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// 20.1.8.54 solidFill (Solid Fill)
    /// This element specifies a solid color fill. The shape is filled entirely with the specified color.
    /// </summary>
    internal class DmlSolidFill : DmlFill
    {
        internal DmlSolidFill()
        {
        }

        internal DmlSolidFill(DmlColor color)
        {
            mColor = color;
        }

        /// <summary>
        /// Set style color to any color placeholders used in the fill.
        /// </summary>
        public override void ApplyStyleColor(DmlColor styleColor)
        {
            Color.ApplyStyleColor(styleColor);
        }

        public override bool Equals(object obj)
        {
            if(!base.Equals(obj))
                return false;

            DmlSolidFill value = (DmlSolidFill)obj;

            return object.Equals(value.Color, Color);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Color.GetHashCode();
            return hash;
        }

        internal override DmlFill Clone()
        {
            DmlSolidFill result = new DmlSolidFill();
            result.Color = Color.Clone();
            return result;
        }

        internal override DmlColor DmlColorInternal
        {
            get { return Color; }
            set { Color = value; }
        }

        /// <summary>
        /// Gets type of the Dml Fill.
        /// </summary>
        internal override DmlFillType DmlFillType
        {
            get { return DmlFillType.SolidFill; }
        }

        internal DmlColor Color
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mColor == null)
                    mColor = new DmlHexRgbColor();
                return mColor;
            }
            set { mColor = value; }
        }

        /// <summary>
        /// Returns true if fill color is empty.
        /// </summary>
        internal bool IsColorEmpty
        {
            get { return Color.IsEmpty; }
        }

        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private DmlColor mColor;
    }
}
