// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2021 by Artem Shabarshin

using Aspose.Words.Drawing;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to apply frame properties from our custom CSS properties to document model.
    /// </summary>
    internal class CssFrameStyleConverter
    {
        internal static void ToParagraphFormat(CssDeclarationCollection declarations, ParagraphFormat paragraphFormat)
        {
            string identifierValue = declarations.GetIdentifier(HtmlConstants.FrameWrapType);
            if (StringUtil.HasChars(identifierValue))
                paragraphFormat.WrapType = HtmlUtil.ParseFrameWrapType(identifierValue);

            identifierValue = declarations.GetIdentifier(HtmlConstants.FrameRelativeVerticalPosition);
            if (StringUtil.HasChars(identifierValue))
            {
                RelativeVerticalPosition verticalPosition;
                if (HtmlUtil.ParseFrameRelativeVerticalPosition(identifierValue, out verticalPosition))
                    paragraphFormat.RelativeVerticalPosition = verticalPosition;
            }

            identifierValue = declarations.GetIdentifier(HtmlConstants.FrameRelativeHorizontalPosition);
            if (StringUtil.HasChars(identifierValue))
            {
                paragraphFormat.RelativeHorizontalPosition =
                        HtmlUtil.ParseFrameRelativeHorizontalPosition(identifierValue);
            }

            identifierValue = declarations.GetIdentifier(HtmlConstants.FrameHeightRule);
            if (StringUtil.HasChars(identifierValue))
            {
                paragraphFormat.FrameHeightRule =
                        HtmlUtil.ParseFrameHeightRule(identifierValue);
            }

            identifierValue = declarations.GetIdentifier(HtmlConstants.FrameLockAnchor);
            if (identifierValue.ToLowerInvariant() == HtmlConstants.FrameLockAnchorLockedValue)
            {
                paragraphFormat.FrameLockAnchor = true;
            }

            // The frame top value may be positive or negative.
            double lengthValue = declarations.GetLength(HtmlConstants.FrameTop);
            if (!MathUtil.IsMinValue(lengthValue))
            {
                paragraphFormat.FrameTop = lengthValue;
            }
            else
            {
                identifierValue = declarations.GetIdentifier(HtmlConstants.FrameTop);
                if (StringUtil.HasChars(identifierValue))
                {
                    paragraphFormat.VerticalAlignment =
                            HtmlUtil.ParseFrameVerticalAlignment(identifierValue);
                }
            }

            // The frame left value may be positive or negative.
            lengthValue = declarations.GetLength(HtmlConstants.FrameLeft);
            if (!MathUtil.IsMinValue(lengthValue))
            {
                paragraphFormat.FrameLeft = lengthValue;
            }
            else
            {
                identifierValue = declarations.GetIdentifier(HtmlConstants.FrameLeft);
                if (StringUtil.HasChars(identifierValue))
                {
                    paragraphFormat.HorizontalAlignment =
                            HtmlUtil.ParseFrameHorizontalAligment(identifierValue);
                }
            }

            // The valid range for frame width and height in MS Word (in editor) is from 0.5pt to 1584pt.
            // Since this isn't documented we accept all non-negative values here.
            lengthValue = declarations.GetLength(HtmlConstants.FrameWidth);
            if (!MathUtil.IsMinValue(lengthValue))
            {
                paragraphFormat.FrameWidth = System.Math.Max(0, lengthValue);
            }

            lengthValue = declarations.GetLength(HtmlConstants.FrameHeight);
            if (!MathUtil.IsMinValue(lengthValue))
            {
                paragraphFormat.FrameHeight = System.Math.Max(0, lengthValue);
            }

            // The valid range for frame horizontal (vertical) distance from text in MS Word (in editor) is from 0.5pt to 1584pt.
            // Since this isn't documented we accept all non-negative values here.
            lengthValue = declarations.GetLength(HtmlConstants.FrameHorizontalDistanceFromText);
            if (!MathUtil.IsMinValue(lengthValue))
            {
                paragraphFormat.FrameHorizontalDistanceFromText = System.Math.Max(0, lengthValue);
            }

            lengthValue = declarations.GetLength(HtmlConstants.FrameVerticalDistanceFromText);
            if (!MathUtil.IsMinValue(lengthValue))
            {
                paragraphFormat.FrameVerticalDistanceFromText = System.Math.Max(0, lengthValue);
            }
        }
    }
}
