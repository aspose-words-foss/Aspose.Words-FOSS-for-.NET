// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/05/2015 by Denis Darkin

using System;

namespace Aspose.Words.Sections
{
    internal static class TextFlowOrientationConverter
    {
        internal static TextOrientation FromTextFlow(TextFlow textFlow)
        {
            switch (textFlow)
            {
                case TextFlow.Horizontal:
                    return TextOrientation.Horizontal;
                case TextFlow.HorizontalRotatedFarEast:
                    return TextOrientation.HorizontalRotatedFarEast;
                case TextFlow.LrBt:
                    return  TextOrientation.Upward;
                case TextFlow.VNonV:
                    return TextOrientation.VerticalFarEast;
                case TextFlow.Vertical:
                    return TextOrientation.Downward;
                case TextFlow.VerticalRotatedFarEast:
                    return TextOrientation.VerticalRotatedFarEast;
                case TextFlow.RlTb:
                    return TextOrientation.Upward;
                default:
                    throw new ArgumentOutOfRangeException("textFlow");
            }
        }

        internal static TextFlow FromTextOrientation(TextOrientation textOrientation)
        {
            switch (textOrientation)
            {
                    case TextOrientation.Downward:
                        return TextFlow.Vertical;
                    case TextOrientation.Horizontal:
                        return TextFlow.Horizontal;
                    case TextOrientation.HorizontalRotatedFarEast:
                        return TextFlow.HorizontalRotatedFarEast;
                    case TextOrientation.Upward:
                        return TextFlow.LrBt;
                    case TextOrientation.VerticalFarEast:
                        return TextFlow.VNonV;
                    case TextOrientation.VerticalRotatedFarEast:
                        return TextFlow.VerticalRotatedFarEast;
                default:
                    throw new ArgumentOutOfRangeException("textOrientation");
            }
        }
    }
}
