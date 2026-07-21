// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS page size property value.
    /// </summary>
    /// <remarks>
    /// http://www.w3.org/TR/css3-page/#page-size-prop
    /// Value:    length{1,2} | auto | [ page-size || [ portrait | landscape] ]
    /// </remarks>
    internal class CssPageSizePropertyValue : CssPropertyValue
    {
        /// <summary>
        /// Creates an instance with 'auto' page size value.
        /// </summary>
        private CssPageSizePropertyValue(CssValueList values)
            : base(values)
        {
            // Empty constructor.
        }

        /// <summary>
        /// Creates page size value with width and height lengths.
        /// </summary>
        internal static CssPageSizePropertyValue CreateLength(CssLengthValue width, CssLengthValue height)
        {
            CssPageSizePropertyValue value = new CssPageSizePropertyValue(new CssValueList(width, height));
            value.mWidth = width;
            value.mHeight = height;
            return value;
        }

        /// <summary>
        /// Creates page size value with one length.
        /// </summary>
        internal static CssPageSizePropertyValue CreateLength(CssLengthValue width)
        {
            // If only one length value is specified, it sets both the width and height of the page box (i.e., the box is a square).
            CssPageSizePropertyValue value = new CssPageSizePropertyValue(new CssValueList(width));
            value.mWidth = width;
            value.mHeight = width;
            return value;
        }

        /// <summary>
        /// Creates page size value with page-size media name or orientation.
        /// </summary>
        internal static CssPageSizePropertyValue CreatePageSize(CssIdentifierValue pageSizeOrOrientationValue)
        {
            CssPageSizePropertyValue propertyValue;
            CssPageSize pageSize;
            if (GetPageSize(pageSizeOrOrientationValue, out pageSize))
            {
                propertyValue = new CssPageSizePropertyValue(new CssValueList(pageSizeOrOrientationValue));
                propertyValue.mSize = pageSize;
                propertyValue.mOrientation = CssPageOrientation.NotSpecified;
            }
            else
            {
                CssPageOrientation pageOrientation;
                if (GetPageOrientation(pageSizeOrOrientationValue, out pageOrientation))
                {
                    propertyValue = new CssPageSizePropertyValue(new CssValueList(pageSizeOrOrientationValue));
                    // Letter is default page size.
                    propertyValue.mSize = CssPageSize.Letter;
                    propertyValue.mOrientation = pageOrientation;
                }
                else
                {
                    propertyValue = null;
                }
            }

            return propertyValue;
        }

        /// <summary>
        /// Creates page size value with page-size media name and orientation.
        /// </summary>
        internal static CssPageSizePropertyValue CreatePageSize(CssIdentifierValue identifierValue1, CssIdentifierValue identifierValue2)
        {
            CssPageSizePropertyValue value;
            CssPageSize pageSize;
            CssPageOrientation pageOrientation;

            CssIdentifierValue pageSizeValue;
            CssIdentifierValue pageOrientationValue;

            if (GetPageSize(identifierValue1, out pageSize))
            {
                pageSizeValue = identifierValue1;

                if (!GetPageOrientation(identifierValue2, out pageOrientation))
                    return null;

                pageOrientationValue = identifierValue2;
            }
            else if (GetPageSize(identifierValue2, out pageSize))
            {
                pageSizeValue = identifierValue2;

                if (!GetPageOrientation(identifierValue1, out pageOrientation))
                    return null;

                pageOrientationValue = identifierValue1;
            }
            else
            {
                return null;
            }

            value = new CssPageSizePropertyValue(new CssValueList(pageSizeValue, pageOrientationValue));
            value.mSize = pageSize;
            value.mOrientation = pageOrientation;

            return value;
        }

        /// <summary>
        /// Creates 'auto' page size value.
        /// </summary>
        internal static CssPageSizePropertyValue CreateAuto()
        {
            CssPageSizePropertyValue value = new CssPageSizePropertyValue(new CssValueList(CssValue.Auto));
            value.mIsAuto = true;
            return value;
        }

        private static bool GetPageOrientation(CssIdentifierValue cssValue, out CssPageOrientation pageOrientation)
        {
            switch (cssValue.Value.ToLowerInvariant())
            {
                case "landscape":
                    pageOrientation = CssPageOrientation.Landscape;
                    return true;
                case "portrait":
                    pageOrientation = CssPageOrientation.Portrait;
                    return true;
                default:
                    pageOrientation = CssPageOrientation.NotSpecified;
                    return false;
            }
        }

        private static bool GetPageSize(CssIdentifierValue cssValue, out CssPageSize pageSize)
        {
            switch (cssValue.Value.ToLowerInvariant())
            {
                case "a5":
                    pageSize = CssPageSize.A5;
                    return true;
                case "a4":
                    pageSize = CssPageSize.A4;
                    return true;
                case "a3":
                    pageSize = CssPageSize.A3;
                    return true;
                case "b5":
                    pageSize = CssPageSize.B5;
                    return true;
                case "b4":
                    pageSize = CssPageSize.B4;
                    return true;
                case "letter":
                    pageSize = CssPageSize.Letter;
                    return true;
                case "legal":
                    pageSize = CssPageSize.Legal;
                    return true;
                case "ledger":
                    pageSize = CssPageSize.Ledger;
                    return true;
                default:
                    pageSize = CssPageSize.NotSpecified;
                    return false;
            }
        }

        private CssLengthValue GetPageSizeWidth()
        {
            double width;
            CssUnit unit = CssUnit.Mm;
            switch (mSize)
            {
                case CssPageSize.A5:
                    width = 148;
                    break;
                case CssPageSize.A4:
                case CssPageSize.NotSpecified:
                    width = 210;
                    break;
                case CssPageSize.A3:
                    width = 297;
                    break;
                case CssPageSize.B5:
                    width = 176;
                    break;
                case CssPageSize.B4:
                    width = 250;
                    break;
                case CssPageSize.Letter:
                    // According to `size` CSS at-rule decription (https://developer.mozilla.org/en-US/docs/Web/CSS/@page/size)
                    // dimensions of letter paper in North America are 8.5in x 11in.
                    width = 8.5;
                    unit = CssUnit.In;
                    break;
                case CssPageSize.Legal:
                    // This keyword is a equivalent to the dimensions of legal papers in North America i.e. 8.5in x 14in.
                    width = 8.5;
                    unit = CssUnit.In;
                    break;
                case CssPageSize.Ledger:
                    // Dimensions of ledger paper in North America are 11in x 17in.
                    width = 11;
                    unit = CssUnit.In;
                    break;
                default:
                    Debug.Assert(false);
                    return gAutoWidthValue;
            }

            return new CssLengthValue(width, unit);
        }

        private CssLengthValue GetPageSizeHeight()
        {
            double height;
            CssUnit unit = CssUnit.Mm;
            switch (mSize)
            {
                case CssPageSize.A5:
                    height = 210;
                    break;
                case CssPageSize.A4:
                case CssPageSize.NotSpecified:
                    height = 297;
                    break;
                case CssPageSize.A3:
                    height = 420;
                    break;
                case CssPageSize.B5:
                    height = 250;
                    break;
                case CssPageSize.B4:
                    height = 353;
                    break;
                case CssPageSize.Letter:
                    // According to `size` CSS at-rule decription (https://developer.mozilla.org/en-US/docs/Web/CSS/@page/size)
                    // dimensions of letter paper in North America are 8.5in x 11in.
                    height = 11;
                    unit = CssUnit.In;
                    break;
                case CssPageSize.Legal:
                    // This keyword is a equivalent to the dimensions of legal papers in North America i.e. 8.5in x 14in.
                    height = 14;
                    unit = CssUnit.In;
                    break;
                case CssPageSize.Ledger:
                    // Dimensions of ledger paper in North America are 11in x 17in.
                    height = 17;
                    unit = CssUnit.In;
                    break;
                default:
                    Debug.Assert(false);
                    return gAutoHeightValue;
            }

            return new CssLengthValue(height, unit);
        }

        /// <summary>
        /// Page size media name.
        /// </summary>
        internal CssPageSize Size
        {
            get { return mSize; }
        }

        /// <summary>
        /// Page orientation.
        /// </summary>
        internal CssPageOrientation Orientation
        {
            get { return mOrientation; }
        }

        /// <summary>
        /// Page width.
        /// </summary>
        internal CssLengthValue Width
        {
            get
            {
                CssLengthValue width;
                if (mWidth != null)
                    width = mWidth;
                else if (mIsAuto)
                    width = gAutoWidthValue;
                else
                    width = GetPageSizeWidth();
                return width;
            }
        }

        /// <summary>
        /// Page height.
        /// </summary>
        internal CssLengthValue Height
        {
            get
            {
                CssLengthValue height;
                if (mHeight != null)
                    height = mHeight;
                else if (mIsAuto)
                    height = gAutoHeightValue;
                else
                    height = GetPageSizeHeight();
                return height;
            }
        }

        private bool mIsAuto;
        private CssLengthValue mWidth;
        private CssLengthValue mHeight;
        private CssPageSize mSize;
        private CssPageOrientation mOrientation;

        // We use the size of ISO Letter media (8.5" wide and 11" high) as the auto value.
        private static readonly CssLengthValue gAutoWidthValue = new CssLengthValue(8.5, CssUnit.In);
        private static readonly CssLengthValue gAutoHeightValue = new CssLengthValue(11, CssUnit.In);
    }
}
