// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2009 by Roman Korchagin

using System.Drawing;
using Aspose.Images.Pal;

namespace Aspose.Images
{
    /// <summary>
    /// Provides methods to deal with MS Word-like image cropping (when crop is specified in fractions of image width).
    /// </summary>
    public class ImageCrop
    {
        public ImageCrop(double cropLeft, double cropRight, double cropTop, double cropBottom)
        {
            mCropLeft = cropLeft;
            mCropRight = cropRight;
            mCropTop = cropTop;
            mCropBottom = cropBottom;
        }

        /// <summary>
        /// Calculates the full "source" crop rectangle.
        /// It is a rectangle that is supposed to be in pixels and includes both positive and negative cropping.
        /// </summary>
        public RectangleF GetSrcFullCropRect(RectangleF srcRect)
        {
            double srcCroppedLeft = srcRect.Left + srcRect.Width * mCropLeft;
            double srcCroppedRight = srcRect.Left + srcRect.Width * (1 - mCropRight);
            double srcCroppedTop = srcRect.Top + srcRect.Height * mCropTop;
            double srcCroppedBottom = srcRect.Top + srcRect.Height * (1 - mCropBottom);

            return RectangleF.FromLTRB((float)srcCroppedLeft, (float)srcCroppedTop, (float)srcCroppedRight, (float)srcCroppedBottom);
        }

        /// <summary>
        /// Calculates the rectangle in pixels that you must take from the source image.
        /// The maximum is the whole source rectangle (image).
        /// </summary>
        public Rectangle GetSrcPositiveCropRect(Rectangle srcRect)
        {
            double srcCroppedLeft = srcRect.Left + srcRect.Width * System.Math.Max(0, mCropLeft);
            double srcCroppedRight = srcRect.Left + srcRect.Width * (1 - System.Math.Max(0, mCropRight));
            double srcCroppedTop = srcRect.Top + srcRect.Height * System.Math.Max(0, mCropTop);
            double srcCroppedBottom = srcRect.Top + srcRect.Height * (1 - System.Math.Max(0, mCropBottom));

            return Rectangle.FromLTRB(
                MathUtil.DoubleToInt(srcCroppedLeft),
                MathUtil.DoubleToInt(srcCroppedTop),
                MathUtil.DoubleToInt(srcCroppedRight),
                MathUtil.DoubleToInt(srcCroppedBottom));
        }

        /// <summary>
        /// Calculates the rectangle in world units that you must output the cropped image to.
        /// The maximum is the whole destination rectangle (shape).
        /// </summary>
        public RectangleF GetDstNegativeCropRect(RectangleF dstRect)
        {
            // Original code by VS.
            double basePictureCoeffX = 1 / (1 - System.Math.Min(0, mCropLeft + mCropRight));
            double basePictureCoeffY = 1 / (1 - System.Math.Min(0, mCropTop + mCropBottom));

            double absNegCropLeft = -System.Math.Min(0, mCropLeft);
            double absNegCropRight = -System.Math.Min(0, mCropRight);
            double absNegCropTop = -System.Math.Min(0, mCropTop);
            double absNegCropBottom = -System.Math.Min(0, mCropBottom);

            double destLeft = dstRect.Left + dstRect.Width * absNegCropLeft * basePictureCoeffX;
            double destRight = dstRect.Left + dstRect.Width * (1 - absNegCropRight * basePictureCoeffX);
            double destTop = dstRect.Top + dstRect.Height * absNegCropTop * basePictureCoeffY;
            double destBottom = dstRect.Top + dstRect.Height * (1 - absNegCropBottom * basePictureCoeffY);

            return RectangleF.FromLTRB((float)destLeft, (float)destTop, (float)destRight, (float)destBottom);
        }

        /// <summary>
        /// Chops the image inside according to the crop.
        /// Do not call this if the crop has no positive cropping as it might be a waste loading the image into memory.
        /// Do not call this for metafiles.
        /// The caller is responsible for destroying the returned image.
        /// </summary>
        public BitmapPal PositivelyCrop(byte[] imageBytes)
        {
            Debug.Assert(HasPositiveCrop);

            using (BitmapPal srcImage = new BitmapPal(imageBytes))
            {
                Rectangle srcRect = GetSrcPositiveCropRect(new Rectangle(0, 0, srcImage.Width, srcImage.Height));
                return srcImage.PositivelyCrop(srcRect);
            }
        }

        public override int GetHashCode()
        {
            // RK This seems to work well, but it was tricky to come up with. I tried some published algorithms and they failed,
            // resulted in duplicate values when left/right or top/bottom had opposite (positive and negative) values.
            return
                (DoublePal.GetHashCode(mCropLeft) >> 1) ^
                (DoublePal.GetHashCode(mCropRight) << 3) ^
                (DoublePal.GetHashCode(mCropTop) << 1) ^
                (DoublePal.GetHashCode(mCropBottom) >> 3);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            ImageCrop other = (ImageCrop)obj;
            return MathUtil.AreEqual(mCropLeft, other.mCropLeft) &&
                   MathUtil.AreEqual(mCropRight, other.mCropRight) &&
                   MathUtil.AreEqual(mCropTop, other.mCropTop) &&
                   MathUtil.AreEqual(mCropBottom, other.mCropBottom);
        }

        public bool IsEmpty
        {
            get { return (mCropLeft == 0) && (mCropRight == 0) && (mCropTop == 0) && (mCropBottom == 0); }
        }

        /// <summary>
        /// Positive cropping is when the crop boundary is moved inside of the image.
        /// </summary>
        public bool HasPositiveCrop
        {
            get { return ((mCropLeft > 0) || (mCropRight > 0) || (mCropTop > 0) || (mCropBottom > 0)); }
        }

        /// <summary>
        /// Negative cropping is when the crop boundary is moved outside of the image.
        /// </summary>
        public bool HasNegativeCrop
        {
            get { return ((mCropLeft < 0) || (mCropRight < 0) || (mCropTop < 0) || (mCropBottom < 0)); }
        }

        public double CropLeft
        {
            get { return mCropLeft; }
        }

        public double CropRight
        {
            get { return mCropRight; }
        }

        public double CropTop
        {
            get { return mCropTop; }
        }

        public double CropBottom
        {
            get { return mCropBottom; }
        }

        public static bool IsNullOrEmpty(ImageCrop crop)
        {
            return (crop == null) || crop.IsEmpty;
        }

        private readonly double mCropLeft;
        private readonly double mCropRight;
        private readonly double mCropTop;
        private readonly double mCropBottom;
    }
}
