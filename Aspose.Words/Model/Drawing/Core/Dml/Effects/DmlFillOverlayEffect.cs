// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Fills;

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.29 fillOverlay (Fill Overlay Effect)
    /// This element specifies a fill overlay effect. 
    /// A fill overlay can be used to specify an additional 
    /// fill for an object and blend the two fills together.
    /// </summary>
    internal class DmlFillOverlayEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.FillOverlay; }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlFillOverlayEffect value = (DmlFillOverlayEffect)obj;

            return object.Equals(value.Fill, Fill) && (value.Blend == Blend);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Fill.GetHashCode();
            hash ^= Blend.GetHashCode();
            return hash;
        }

        internal override DmlEffect Clone()
        {
            DmlFillOverlayEffect lhs = (DmlFillOverlayEffect)MemberwiseClone();

            if (mFill != null)
                lhs.mFill = mFill.Clone();
            
            return lhs;
        }

        internal DmlFill Fill
        {
            get { return mFill; }
            set { mFill = value; }
        }

        internal DmlEffectBlendMode Blend
        {
            get { return mBlend; }
            set { mBlend = value; }
        }

        private DmlFill mFill;
        private DmlEffectBlendMode mBlend;
    }
}
