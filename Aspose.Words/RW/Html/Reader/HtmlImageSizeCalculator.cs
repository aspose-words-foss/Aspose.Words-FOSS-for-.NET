// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/09/2023 by Nikolay Sezganov

using Aspose.Words.Drawing;
using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Calculates image size by CSS properties, intrinsic and parent container size.
    /// </summary>
    internal class HtmlImageSizeCalculator
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal HtmlImageSizeCalculator(
            SizeD imageSize,
            SizeD containerSize,
            CssDeclarationCollection declarations,
            bool preserveAspectRatio)
        {
            mImageSize = (imageSize == null)
                ? new SizeD(-1, -1)
                : imageSize;
            mContainerSize = containerSize;
            mDeclarations = declarations;
            mPreserveAspectRatio = preserveAspectRatio;

            mWidth = ReadWidth(declarations[WidthProperty], double.MinValue);
            mMinWidth = ReadWidth(declarations[MinWidthProperty], double.MinValue);
            mMaxWidth = ReadWidth(declarations[MaxWidthProperty], double.MaxValue);

            mHeight = ReadHeight(declarations[HeightProperty], double.MinValue);
            mMinHeight = ReadHeight(declarations[MinHeightProperty], double.MinValue);
            mMaxHeight = ReadHeight(declarations[MaxHeightProperty], double.MaxValue);
        }

        internal HtmlImageSizeCalculator(
            SizeD imageSize,
            SizeD containerSize,
            CssDeclarationCollection declarations)
            : this(
                  imageSize,
                  containerSize,
                  declarations,
                  true)
        {
            // Empty constructor.
        }

        internal SizeD Calculate(CssDisplayType parentDisplayType)
        {
            double width = mImageSize.Width;
            double height = mImageSize.Height;

            double ratio = width / height;

            if (ratio <= 0)
                return null;

            if (HasProperty(WidthProperty) && HasProperty(HeightProperty))
            {
                width = mWidth;
                height = mHeight;
                ratio = width / height;
            }
            else if (HasProperty(WidthProperty))
            {
                width = mWidth;
                height = mPreserveAspectRatio ? mWidth / ratio : mImageSize.Height;
            }
            else if (HasProperty(HeightProperty))
            {
                height = mHeight;
                width = mPreserveAspectRatio ? mHeight * ratio : mImageSize.Width;
            }

            if (ratio <= 0)
                return null;

            if (IsZeroWidthContainer())
            {
                return new SizeD(width, height);
            }

            if (IsAffectedOnly(width, height, MaxWidthProperty))
            {
                return new SizeD(mMaxWidth, System.Math.Max(mMaxWidth / ratio, mMinHeight));
            }

            // WORDSNET-26855 If a container containing an image has the `display: table` property,
            // then it's not the image that will adopt the size of the container,
            // but the container will adopt the size of the image.
            // The `min-width` property does not work in this case.
            if ((parentDisplayType != CssDisplayType.Table) &&
                IsAffectedOnly(width, height, MinWidthProperty))
            {
                return new SizeD(mMinWidth, System.Math.Min(mMinWidth / ratio, mMaxHeight));
            }

            if (IsAffectedOnly(width, height, MaxHeightProperty))
            {
                return new SizeD(System.Math.Max(mMaxHeight * ratio, mMinWidth), mMaxHeight);
            }

            if (IsAffectedOnly(width, height, MinHeightProperty))
            {
                return new SizeD(System.Math.Min(mMinHeight * ratio, mMaxWidth), mMinHeight);
            }

            if (IsAffectedOnly(width, height, MaxWidthProperty, MaxHeightProperty) &&
                ((mMaxWidth / width) <= (mMaxHeight / height)))
            {
                return new SizeD(mMaxWidth, System.Math.Max(mMinHeight, mMaxWidth / ratio));
            }

            if (IsAffectedOnly(width, height, MaxWidthProperty, MaxHeightProperty) &&
                ((mMaxWidth / width) > (mMaxHeight / height)))
            {
                return new SizeD(System.Math.Max(mMinWidth, mMaxHeight * ratio), mMaxHeight);
            }

            if (IsAffectedOnly(width, height, MinWidthProperty, MinHeightProperty) &&
                ((mMinWidth / width) <= (mMinHeight / height)))
            {
                return new SizeD(System.Math.Min(mMaxWidth, mMinHeight * ratio), mMinHeight);
            }

            if (IsAffectedOnly(width, height, MinWidthProperty, MinHeightProperty) &&
                ((mMinWidth / width) > (mMinHeight / height)))
            {
                return new SizeD(mMinWidth, System.Math.Min(mMaxHeight, mMinWidth / ratio));
            }

            if (IsAffectedOnly(width, height, MinWidthProperty, MaxHeightProperty))
            {
                return new SizeD(mMinWidth, mMaxHeight);
            }

            if (IsAffectedOnly(width, height, MaxWidthProperty, MinHeightProperty))
            {
                return new SizeD(mMaxWidth, mMinHeight);
            }

            if (HasSizeProperties())
            {
                return new SizeD(width, height);
            }

            return null;
        }

        internal bool HasAnyCssSizeProperties()
        {
            return HasSizeProperties() ||
                   HasProperty(MinWidthProperty) || HasProperty(MinHeightProperty) ||
                   HasProperty(MaxWidthProperty) || HasProperty(MaxHeightProperty);
        }

        internal double CssWidth
        {
            get
            {
                double width = GetsCssWidth();
                return MathUtil.IsMinValue(width)
                    ? -1
                    : width;
            }
        }

        internal double CssHeight
        {
            get
            {
                double height = GetsCssHeight();
                return MathUtil.IsMinValue(height)
                    ? -1
                    : height;
            }
        }

        private double ReadWidth(CssDeclaration cssDeclaration, double defaultValue)
        {
            // WORDSNET-28510 When an image has 'min-width:none' or 'max-width:none', it should not be resized.
            if (IsNullOrNone(cssDeclaration))
                return defaultValue;

            return ReadValue(
                cssDeclaration,
                (mContainerSize != null) ? mContainerSize.Width : 0);
        }

        private double ReadHeight(CssDeclaration cssDeclaration, double defaultValue)
        {
            // WORDSNET-28510 When an image has 'min-height:none' or 'max-height:none', it should not be resized.
            if (IsNullOrNone(cssDeclaration))
                return defaultValue;

            return ReadValue(
                cssDeclaration,
                (mContainerSize != null) ? mContainerSize.Height : 0);
        }

        private static bool IsNullOrNone(CssDeclaration cssDeclaration)
        {
            return (cssDeclaration == null) ||
                   cssDeclaration.Value.Equals(CssValue.None);
        }

        /// <summary>
        /// Checks that only the specified max/min properties affect the image and other max/min properties don't.
        /// </summary>
        private bool IsAffectedOnly(double width, double height, params string[] propertiesThatAffect)
        {
            string[] allProperties = new string[] { MaxWidthProperty, MaxHeightProperty, MinWidthProperty, MinHeightProperty };
            foreach (string property in allProperties)
            {
                bool affects = IsAffected(property, width, height);
                bool shouldAffect = ArrayUtil.Contains(propertiesThatAffect, property);

                if (affects != shouldAffect)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsAffected(string property, double width, double height)
        {
            if (mDeclarations[property] == null)
                return false;

            switch (property)
            {
                case MaxWidthProperty:
                    return width > mMaxWidth;
                case MaxHeightProperty:
                    return height > mMaxHeight;
                case MinWidthProperty:
                    return width < mMinWidth;
                case MinHeightProperty:
                    return height < mMinHeight;
                default:
                    Debug.Assert(false);
                    return false;
            }
        }

        private static double ReadValue(CssDeclaration cssDeclaration, double containerSize)
        {
            if (cssDeclaration.HasSingleValueOfType(CssValueType.Percentage))
            {
                return containerSize / 100 * cssDeclaration.Value.FirstValue.DoubleValue;
            }

            return CssUtil.LengthToPoint(cssDeclaration.Value);
        }

        private double GetsCssWidth()
        {
            double width = (mWidth > 0)
                ? mWidth
                : mImageSize.Width;

            if ((mMinWidth > 0) && (width < mMinWidth))
                return mMinWidth;
            if ((mMaxWidth > 0) && (width > mMaxWidth))
                return mMaxWidth;

            return mWidth;
        }

        private double GetsCssHeight()
        {
            double height = (mHeight > 0)
                ? mHeight
                : mImageSize.Height;

            if ((mMinHeight > 0) && (height < mMinHeight))
                return mMinHeight;
            if ((mMaxHeight > 0) && (height > mMaxHeight))
                return mMaxHeight;

            return mHeight;
        }

        private bool IsZeroWidthContainer()
        {

            if ((mContainerSize == null) ||
                (mContainerSize.Width > 0))
            {
                return false;
            }

            CssDeclaration declaration = mDeclarations[MaxWidthProperty];
            return (declaration != null) && declaration.HasSingleValueOfType(CssValueType.Percentage);
        }

        private bool HasSizeProperties()
        {
            return HasProperty(WidthProperty) || HasProperty(HeightProperty);
        }

        private bool HasProperty(string propName)
        {
            CssDeclaration declaration = mDeclarations[propName];
            if (declaration == null)
                return false;
            return declaration.HasSingleValueOfType(CssValueType.Number) ||
                   declaration.HasSingleValueOfType(CssValueType.Length) ||
                   declaration.HasSingleValueOfType(CssValueType.Percentage);
        }

        private const string WidthProperty = "width";
        private const string MinWidthProperty = "min-width";
        private const string MaxWidthProperty = "max-width";

        private const string HeightProperty = "height";
        private const string MinHeightProperty = "min-height";
        private const string MaxHeightProperty = "max-height";

        private readonly SizeD mImageSize;
        private readonly SizeD mContainerSize;

        private readonly CssDeclarationCollection mDeclarations;
        private readonly bool mPreserveAspectRatio;

        private readonly double mWidth;
        private readonly double mMinWidth;
        private readonly double mMaxWidth;
        private readonly double mHeight;
        private readonly double mMinHeight;
        private readonly double mMaxHeight;
    }
}
