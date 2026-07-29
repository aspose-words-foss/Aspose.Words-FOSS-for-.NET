// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/02/2017 by Alexey Butalov

using System.Collections.Generic;
using System.Drawing;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Helper class containing methods used for a DrawingML conversion.
    /// </summary>
    internal static class DmlUtilCore
    {
        internal static object ConvertDmlNullEffect(object value)
        {
            return (value is DmlNullEffect ? null : value);
        }

        /// <summary>
        /// Converts <see cref="ShapeTextOrientation"/> to <see cref="LayoutFlow"/> value.
        /// </summary>
        internal static LayoutFlow DmlToTextVerticalType(ShapeTextOrientation textOrientation, bool normalEastAsianFlow)
        {
            // According to specification attribute that specifies whether the text flow of the text contents of the shape 
            // ignores the text flow value specified by the vert attribute of the bodyPr element.
            // MS Word seams uses LayoutFlow.HorizontalIdeographic do the same.
            if (normalEastAsianFlow)
                return LayoutFlow.HorizontalIdeographic;

            switch (textOrientation)
            {
                case ShapeTextOrientation.Horizontal:
                    return LayoutFlow.Horizontal;
                case ShapeTextOrientation.VerticalRotatedFarEast:
                case ShapeTextOrientation.Downward:
                    return LayoutFlow.Vertical;
                case ShapeTextOrientation.VerticalFarEast:
                case ShapeTextOrientation.WordArtVertical:
                case ShapeTextOrientation.WordArtVerticalRightToLeft:
                    return LayoutFlow.TopToBottomIdeographic;
                case ShapeTextOrientation.Upward:
                    return LayoutFlow.BottomToTop;
                default:
                    return LayoutFlow.Horizontal;
            }
        }

        /// <summary>
        /// Converts <see cref="LayoutFlow"/> to <see cref="ShapeTextOrientation"/> value.
        /// </summary>
        internal static ShapeTextOrientation TextVerticalTypeToDml(LayoutFlow layoutFlow)
        {
            switch (layoutFlow)
            {
                case LayoutFlow.Horizontal:
                    return ShapeTextOrientation.Horizontal;
                case LayoutFlow.Vertical:
                case LayoutFlow.TopToBottom:
                    return ShapeTextOrientation.Downward;
                case LayoutFlow.TopToBottomIdeographic:
                    return ShapeTextOrientation.VerticalFarEast;
                case LayoutFlow.BottomToTop:
                    return ShapeTextOrientation.Upward;
                case LayoutFlow.HorizontalIdeographic:
                default:
                    return ShapeTextOrientation.Horizontal;
            }
        }

        /// <summary>
        /// Converts <see cref="DmlTextAnchoringType"/> to <see cref="TextBoxAnchor"/> value.
        /// </summary>
        internal static TextBoxAnchor DmlToTextboxAnchor(DmlTextAnchoringType dmlTextAnchoringType)
        {
            switch (dmlTextAnchoringType)
            {
                case DmlTextAnchoringType.Bottom:
                    return TextBoxAnchor.Bottom;
                case DmlTextAnchoringType.Top:
                    return TextBoxAnchor.Top;
                case DmlTextAnchoringType.Center:
                    return TextBoxAnchor.Middle;
                case DmlTextAnchoringType.Distributed:
                case DmlTextAnchoringType.Justified:
                default:
                    return TextBoxAnchor.Top;
            }
        }

        /// <summary>
        /// Converts <see cref="TextBoxAnchor"/> to <see cref="DmlTextAnchoringType"/> value.
        /// </summary>
        internal static DmlTextAnchoringType TextboxAnchorToDml(TextBoxAnchor textBoxAnchor)
        {
            switch (textBoxAnchor)
            {
                case TextBoxAnchor.Bottom:
                    return DmlTextAnchoringType.Bottom;
                case TextBoxAnchor.Top:
                    return DmlTextAnchoringType.Top;
                case TextBoxAnchor.Middle:
                    return DmlTextAnchoringType.Center;
                case TextBoxAnchor.BottomBaseline:
                case TextBoxAnchor.BottomCentered:
                case TextBoxAnchor.BottomCenteredBaseline:
                case TextBoxAnchor.TopBaseline:
                case TextBoxAnchor.TopCentered:
                case TextBoxAnchor.TopCenteredBaseline:
                case TextBoxAnchor.MiddleCentered:
                default:
                    return DmlTextAnchoringType.Top;
            }
        }

        /// <summary>
        /// Parses DmlGradientFill.GradientStops array.
        /// </summary>
        internal static GradientColor[] DmlToGradientStops(IList<DmlGradientStop> list, IThemeProvider themeProvider)
        {
            GradientColor[] result = new GradientColor[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                DmlGradientStop dmlGradientStop = list[i];
                GradientColor gradientColor = new GradientColor();
                gradientColor.Color = dmlGradientStop.Color.CreateDrColor(themeProvider, null);
                gradientColor.Start = MathUtil.DoubleToInt(DmlPercentageUtil.ToDmlPercent(dmlGradientStop.Position) / 100000 * 0x10000);

                result[i] = gradientColor;
            }
            return result;
        }

        /// <summary>
        /// Converts alignment to origin point.
        /// </summary>
        internal static PointF GetOrigin(DmlRectangleAlignment alignment)
        {
            switch (alignment)
            {
                case DmlRectangleAlignment.TopLeft:
                    return new PointF(-0.5f, -0.5f);
                case DmlRectangleAlignment.Top:
                    return new PointF(0, -0.5f);
                case DmlRectangleAlignment.TopRight:
                    return new PointF(0.5f, -0.5f);
                case DmlRectangleAlignment.Left:
                    return new PointF(-0.5f, 0);
                case DmlRectangleAlignment.Center:
                case DmlRectangleAlignment.None: // for Dml Text Effects.
                    return new PointF(0, 0);
                case DmlRectangleAlignment.Right:
                    return new PointF(0.5f, 0);
                case DmlRectangleAlignment.BottomLeft:
                    return new PointF(-0.5f, 0.5f);
                case DmlRectangleAlignment.Bottom:
                    return new PointF(0, 0.5f);
                case DmlRectangleAlignment.BottomRight:
                    return new PointF(0.5f, 0.5f);
                default:
                    return PointF.Empty;
            }
        }

        /// <summary>
        /// Calculates value in the range between "-spread" and "spread". Result depends from 
        /// ratio of the shape width and height, when width greater then height, then value is positive,
        /// otherwise negative.
        /// </summary>
        internal static int GetScaledSpread(double width, double height, int spread)
        {
            int delta = 0;
            if (MathUtil.AreEqual(width, height))
                return delta;

            if (width > height)
                delta = (int)System.Math.Round((1 - height / width) * spread);
            else
                delta = (int)System.Math.Round((1 - width / height) * -spread);

            return delta;
        }
    }
}
