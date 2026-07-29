// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/01/2011 by Alexey Titov

using System.Drawing;
using System.Drawing.Drawing2D;
using Aspose.Drawing;
using Aspose.Images;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml.Effects;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// 20.1.8.14 blipFill (Picture Fill)
    /// This element specifies the type of picture fill that the picture object has.
    /// Because a picture has a picture fill already by default, it is possible to
    /// have two fills specified for a picture object. An example of this is shown below.
    /// </summary>
    internal class DmlBlipFill : DmlFill
    {
        internal override DmlFill Clone()
        {
            DmlBlipFill result = new DmlBlipFill();
            result.DotsPerInch = DotsPerInch;
            result.RotateWithShape = RotateWithShape;
            if (mSourceRectangle != null)
                result.mSourceRectangle = mSourceRectangle.Clone();
            if (mBlipFillMode != null)
                result.mBlipFillMode = mBlipFillMode.Clone();
            if (mBlip != null)
                result.mBlip = mBlip.Clone();
            return result;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlBlipFill value = (DmlBlipFill)obj;

            return (value.DotsPerInch == DotsPerInch) &&
                   (value.RotateWithShape == RotateWithShape) &&
                   object.Equals(value.SourceRectangle, SourceRectangle) &&
                   object.Equals(value.BlipFillMode, BlipFillMode) &&
                   object.Equals(value.Blip, Blip);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= DotsPerInch.GetHashCode();
            hash ^= RotateWithShape.GetHashCode();

            if (SourceRectangle != null)
                hash ^= SourceRectangle.GetHashCode();
            if (BlipFillMode != null)
                hash ^= BlipFillMode.GetHashCode();
            if (Blip != null)
                hash ^= Blip.GetHashCode();

            return hash;
        }

        public override double Opacity
        {
            get
            {
                DmlAlphaModulateFixedEffect alphaFixedEffect = GetAlphaFixedEffect();
                return (alphaFixedEffect == null) ? 1.0 : alphaFixedEffect.Amount;
            }

            set
            {
                // WORDSNET-16359 Set opacity for appropriate blip instead of fill color.
                DmlAlphaModulateFixedEffect alphaFixedEffect = GetAlphaFixedEffect();

                if (alphaFixedEffect == null)
                {
                    alphaFixedEffect = new DmlAlphaModulateFixedEffect();
                    Blip.Effects.Add(alphaFixedEffect);
                }

                alphaFixedEffect.Amount = value;
            }
        }

        /// <summary>
        /// Specifies that the fill should rotate with the shape. That is, when the shape
        /// that has been filled with a picture and the containing shape (say a rectangle)
        /// is transformed with a rotation then the fill is transformed with the same rotation.
        /// </summary>
        public override bool RotateWithShape
        {
            get { return mRotateWithShape; }
            set { mRotateWithShape = value; }
        }

        /// <summary>
        /// Gets the raw bytes of the fill texture or pattern.
        /// </summary>
        /// <remarks>
        /// <p>The default value is null.</p>
        /// </remarks>
        public override byte[] ImageBytes
        {
            get { return Blip.GetImageBytes(); }
        }

        internal ImageSizeCore ImageSize
        {
            get { return mImageSize; }
        }

        /// <summary>
        /// Gets or sets the type of fill.
        /// </summary>
        public override FillTypeCore FillType
        {
            get
            {
                if ((mBlipFillMode == null) || (mBlipFillMode.FillType == FillTypeCore.Picture))
                    return FillTypeCore.Picture;

                return FillTypeCore.Texture;
            }
        }

        internal override DmlFillType DmlFillType
        {
            get { return DmlFillType.BlipFill; }
        }

        internal RectangleF CalculateImageArea()
        {
            RectangleF originalRect = new RectangleF(0, 0, mImageSize.Width, mImageSize.Height);
            RectangleF rect = SourceRectangle.Adjust(originalRect);
            return rect;
        }

        /// <summary>
        /// Obtains alpha modulate fixed effect from blip fill.
        /// </summary>
        private DmlAlphaModulateFixedEffect GetAlphaFixedEffect()
        {
            DmlAlphaModulateFixedEffect alphaFixed = null;

            // AW removes existing effect to replace with new one to prepare arranged collection of DrawingML effects.
            // So, retrieve last alpha fixed effect, which contains actual value.
            for (int i = Blip.Effects.Count - 1; i >= 0; i--)
            {
                if (Blip.Effects[i].Type == DmlEffectType.AlphaModulateFixed)
                {
                    alphaFixed = (DmlAlphaModulateFixedEffect)Blip.Effects[i];
                    break;
                }
            }

            return alphaFixed;
        }

        internal DmlBlip Blip
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mBlip == null)
                    mBlip = new DmlBlip();

                return mBlip;
            }
            set { mBlip = value; }
        }

        /// <summary>
        /// Specifies the DPI (dots per inch) used to calculate the size of the blip.
        /// If not present or zero, the DPI in the blip is used.
        /// </summary>
        /// <remarks>
        /// VML does not contain appropriate properties, so value always is 0 while converting VML to DML.
        /// </remarks>
        internal uint DotsPerInch
        {
            get { return mDotsPerInch; }
            set { mDotsPerInch = value; }
        }

        /// <summary>
        /// This element specifies the portion of the blip used for the fill.
        /// Each edge of the source rectangle is defined by a percentage offset from the
        /// corresponding edge of the bounding box. A positive percentage specifies an inset,
        /// while a negative percentage specifies an outset.
        /// For example, a left offset of  25% specifies that the left edge of the source
        /// rectangle is located to the right of the bounding box's left edge by an
        /// amount equal to 25% of the bounding box's width.
        /// </summary>
        internal DmlPercentageOffsetRectangle SourceRectangle
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mSourceRectangle == null)
                    mSourceRectangle = new DmlPercentageOffsetRectangle();

                return mSourceRectangle;
            }
            set { mSourceRectangle = value; }
        }

        /// <summary>
        /// Defines the fill mode (tiling or stretching).
        /// </summary>
        internal IDmlBlipFillMode BlipFillMode
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mBlipFillMode == null)
                    mBlipFillMode = new DmlBlipFillStretch(); // Use stretch mode by default.

                return mBlipFillMode;
            }
            set { mBlipFillMode = value; }
        }

        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private DmlBlip mBlip;
        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private IDmlBlipFillMode mBlipFillMode;
        private uint mDotsPerInch;
        private ImageSizeCore mImageSize = null;
        private bool mRotateWithShape = true;
        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private DmlPercentageOffsetRectangle mSourceRectangle = new DmlPercentageOffsetRectangle();
    }
}
