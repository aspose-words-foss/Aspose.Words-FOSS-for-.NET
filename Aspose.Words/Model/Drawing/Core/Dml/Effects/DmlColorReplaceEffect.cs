// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/17/2014 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    internal class DmlColorReplaceEffect : DmlEffect
    {
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.ColorReplace; }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlColorReplaceEffect value = (DmlColorReplaceEffect)obj;

            return object.Equals(value.Color, Color);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            if (Color != null)
                hash ^= Color.GetHashCode();
            return hash;
        }

        internal override DmlEffect Clone()
        {
            DmlColorReplaceEffect lhs = (DmlColorReplaceEffect)MemberwiseClone();

            if (mColor != null)
                lhs.mColor = mColor.Clone();

            return lhs;
        }

        internal DmlColor Color
        {
            get { return mColor; }
            set { mColor = value; }
        }

        private DmlColor mColor;
    }
}
