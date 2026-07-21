// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

using Aspose.Drawing;

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.15 blur (Blur Effect)
    /// This element specifies a blur effect that is applied to the 
    /// entire shape, including its fill. All color channels, 
    /// including alpha, are affected.
    /// </summary>
    internal class DmlBlurEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.Blur; }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlBlurEffect value = (DmlBlurEffect)obj;

            return (value.Grow == Grow) && MathUtil.AreEqual(value.Radius, Radius);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Radius.GetHashCode();
            hash ^= Grow.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Specifies whether the bounds of the object should be grown as a 
        /// result of the blurring. True indicates the bounds are grown while 
        /// false indicates that they are not.
        /// Default value is true.
        /// </summary>
        internal bool Grow
        {
            get { return mGrow; }
            set { mGrow = value; }
        }

        /// <summary>
        /// Specifies the radius of blur.
        /// </summary>
        internal double Radius
        {
            get { return mRadius; }
            set { mRadius = value; }
        }

        /// <summary>
        /// Specifies that the resulting bitmap must be cropped to the original size after applying blur.
        /// </summary>
        internal bool CropToOriginalSize
        {
            get { return mCropToOriginalSize; }
            set { mCropToOriginalSize = value; }
        }

        internal DrColor BackgroundColor
        {
            get { return mBackgroundColor; }
            set { mBackgroundColor = value; }
        }

        private bool mCropToOriginalSize = true;
        private bool mGrow = true;
        private double mRadius;

        // Use transparent white as a default background.
        private DrColor mBackgroundColor = new DrColor(1, 255, 255, 255);
    }
}