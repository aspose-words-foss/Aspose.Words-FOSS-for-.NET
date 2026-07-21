// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/24/2013 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Effects;
using Aspose.Words.Drawing.Core.Dml.Fills;

namespace Aspose.Words.Drawing.Core.Dml.ShapeEffects
{
    /// <summary>
    /// 20.1.8.29 fillOverlay (Fill Overlay Effect)
    /// This element specifies a fill overlay effect. A fill overlay can be used to specify an additional fill for an object and
    /// blend the two fills together.
    /// 
    /// Class is wrapper for <see cref="DmlFillOverlayEffect"/>. This effect can be applied to blip as well to whole shape.
    /// </summary>
    internal class DmlShapeFillOverlayEffect : DmlShapeEffect
    {
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlShapeFillOverlayEffect value = (DmlShapeFillOverlayEffect)obj;

            return object.Equals(value.FillOverlay, FillOverlay);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= FillOverlay.GetHashCode();
            return hash;
        }

        internal override DmlShapeEffect Clone()
        {
            DmlShapeFillOverlayEffect lhs = (DmlShapeFillOverlayEffect)MemberwiseClone();

            lhs.mFillOverlay = (DmlFillOverlayEffect)mFillOverlay.Clone();

            return lhs;
        }

        internal override DmlShapeEffectType EffectType
        {
            get { return DmlShapeEffectType.FillOverlay; }
        }

        /// <summary>
        /// Specifies how to blend the fill with the base effect.
        /// </summary>
        internal DmlEffectBlendMode Blend
        {
            get { return mFillOverlay.Blend; }
            set { mFillOverlay.Blend = value; }
        }

        internal DmlFill Fill
        {
            get { return mFillOverlay.Fill; }
            set { mFillOverlay.Fill = value; }
        }

        internal DmlFillOverlayEffect FillOverlay
        {
            get { return mFillOverlay; }
        }

        private DmlFillOverlayEffect mFillOverlay = new DmlFillOverlayEffect();
    }
}
