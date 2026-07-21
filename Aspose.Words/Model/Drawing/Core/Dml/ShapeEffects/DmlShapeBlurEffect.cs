// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/24/2013 by Alexey Noskov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Effects;

namespace Aspose.Words.Drawing.Core.Dml.ShapeEffects
{
    /// <summary>
    /// Represents 20.1.8.15 blur (Blur Effect)
    /// This element specifies a blur effect that is applied to the entire shape, including its fill. All color channels,
    /// including alpha, are affected.
    /// 
    /// Class is wrapper for <see cref="DmlBlurEffect"/>. This effect can be applied to blip as well to whole shape.
    /// </summary>
    internal class DmlShapeBlurEffect : DmlShapeEffect
    {
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlShapeBlurEffect value = (DmlShapeBlurEffect)obj;

            return object.Equals(value.Blur, Blur);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Blur.GetHashCode();
            return hash;
        }

        internal DmlShapeBlurEffect()
        {
            mBlur = new DmlBlurEffect();
            mBlur.CropToOriginalSize = false;
        }

        internal override DmlShapeEffect Clone()
        {
            DmlShapeBlurEffect lhs = (DmlShapeBlurEffect)MemberwiseClone();

            if (mBlur != null)
                lhs.mBlur = (DmlBlurEffect)mBlur.Clone();

            return lhs;
        }

        internal override DmlShapeEffectType EffectType
        {
            get { return DmlShapeEffectType.Blur; }
        }

        /// <summary>
        /// Specifies whether the bounds of the object should be grown as a result of the blurring.
        /// Default value is true.
        /// </summary>
        internal bool Grow
        {
            get { return mBlur.Grow; }
            set { mBlur.Grow = value; }
        }

        /// <summary>
        /// Specifies the radius of blur.
        /// </summary>
        internal double Radius
        {
            get { return mBlur.Radius; }
            set { mBlur.Radius = value; }
        }

        internal DrColor BackgroundColor
        {
            get { return mBlur.BackgroundColor; }
            set { mBlur.BackgroundColor = value; }
        }

        internal DmlBlurEffect Blur
        {
            get { return mBlur; }
        }

        private DmlBlurEffect mBlur;
    }
}
