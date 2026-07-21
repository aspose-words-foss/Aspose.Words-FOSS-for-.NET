// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/11/2015 by Andrey Noskov

using System;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Vml
{
    internal class VmlFillReader : VmlShapeReaderBase
    {
        internal static void ReadFill(ShapeBase shape, IVmlShapeReaderContext context)
        {
            NrxXmlReader reader = context.XmlReader;

            bool checkGradient = false;

            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "id":  // r:id DOCX.
                    case "src": // WordML.
                    {
                        // WORDSNET-22742 Some bindings are unavailable at the time the markup is parsed.
                        // So memorize the fill source for deferred binding.
                        context.XmlReader.FillSourceMap[shape] = value;
                        break;
                    }
                    case "title": // o:title
                        if (!shape.ShapePr.Contains(ShapeAttr.FillBlipName))
                            shape.SetShapeAttrInternal(ShapeAttr.FillBlipName, value);
                        break;
                    case "opacity":
                        SetFixedAttribute(shape, ShapeAttr.FillOpacity, value);
                        break;
                    case "color":
                        // WORDSNET-11137 Seems, MS Word reads this attribute, but does not write this again to fill element upon resaving.
                        // Instead, it overrides 'fillcolor' attribute with this color.
                        shape.SetShapeAttrInternal(ShapeAttr.FillColor, VmlColor.VmlToColor(value));
                        break;
                    case "color2":
                        shape.SetShapeAttrInternal(ShapeAttr.FillBackColor, VmlColor.VmlToColor(value));

                        DrColor baseColor = VmlColor.GetBaseColor(value);
                        if (baseColor != null)
                            shape.SetShapeAttrInternal(ShapeAttr.FillBackColorExt, baseColor);
                        int colorModifier = VmlColor.GetColorModifier(value);
                        if (colorModifier != 0)
                            shape.SetShapeAttrInternal(ShapeAttr.FillBackColorExtMod, colorModifier);
                        break;
                    case "opacity2": // o:opacity2
                        SetFixedAttribute(shape, ShapeAttr.FillBackOpacity, value);
                        break;
                    case "aspect":
                        shape.SetShapeAttrInternal(ShapeAttr.FillDimensionType, VmlEnum.VmlToDzType(value));
                        break;
                    case "origin":
                        Set2DFixed(shape, ShapeAttr.FillOriginX, ShapeAttr.FillOriginY, value);
                        break;
                    case "position":
                        Set2DFixed(shape, ShapeAttr.FillShapeOriginX, ShapeAttr.FillShapeOriginY, value);
                        break;
                    case "size":
                        // v:fill[size] Seen only as size="0,0" in TestFillStyle ms.wml(542)
                        // and that didn't correspond to any attribute. Need to check. Ignored now.
                        break;
                    case "recolor":
                        SetBoolAttribute(shape, ShapeAttr.FillRecolorAsPicture, value);
                        break;
                    case "rotate":
                        SetBoolAttribute(shape, ShapeAttr.FillUseShapeAnchor, value);
                        break;
                    case "angle":
                        SetFixedAttribute(shape, ShapeAttr.FillAngle, value);
                        break;
                    case "colors":
                        object gradientColorsValue = VmlToGradientColors(value);
                        // VmlToGradientColors can return null, do not set ShapeAttr in this case for stability.
                        if (gradientColorsValue != null)
                            shape.SetShapeAttrInternal(ShapeAttr.FillShadeColors, gradientColorsValue);
                        break;
                    case "focusposition":
                    {
                        Set2DFixed(shape, ShapeAttr.FillToLeft, ShapeAttr.FillToTop, value);
                        Set2DFixed(shape, ShapeAttr.FillToRight, ShapeAttr.FillToBottom, value);
                        break;
                    }
                    case "focussize":
                        // Have not seen 'focussize' taking values other than empty string. Hence, ignored.
                        break;
                    case "method":
                        if (value == "linear sigma")
                            shape.SetShapeAttrInternal(ShapeAttr.FillShadeType, VmlEnum.LinearSigmaGradient);
                        else
                            shape.SetShapeAttrInternal(ShapeAttr.FillShadeType, GradientType.None);
                        break;
                    case "focus":
                        SetPercentAttribute(shape, ShapeAttr.FillFocus, value);
                        break;
                    case "type":
                        shape.SetShapeAttrInternal(ShapeAttr.FillType, VmlEnum.VmlToFillType(value));

                        if (value == "gradientRadial" || value == "gradient")
                            checkGradient = true;
                        break;
                    case "detectmouseclick": // o:detectmouseclick
                        SetBoolAttribute(shape, ShapeAttr.FillNoFillHitTest, value);
                        break;
                    case "href":
                        // Never seen. Ignored.
                        reader.Warn(WarningType.DataLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                    case "althref":
                        // Never seen. Ignored.
                        reader.Warn(WarningType.DataLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                    case "on":
                        shape.SetShapeAttrInternal(ShapeAttr.Filled, reader.ValueAsBool);
                        break;
                    default:
                        reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                }
            }

            // Read 'o:fill' child element, if exists, to check if the FillType is ShadeCenter.

            if (checkGradient)
            {
                while (reader.ReadChild("fill")) // v:fill
                {
                    // andrnosk: WORDSNET-4164 Check 'o:fill' element to get the FillType
                    // which overrides the value of the type attribute in the parent fill element.
                    string gradientType = reader.ReadAttribute("type", "");
                    switch (gradientType)
                    {
                        case "gradientCenter":
                            shape.SetShapeAttrInternal(ShapeAttr.FillType, FillTypeCore.ShadeCenter);
                            break;
                        case "gradientUnscaled":
                            shape.SetShapeAttrInternal(ShapeAttr.FillType, FillTypeCore.ShadeUnscale);
                            break;
                        default:
                            break;
                    }

                }
            }
        }

        /// <summary>
        /// Parses gradient color array string. Does not throw on invalid values, just returns null.
        /// </summary>
        private static GradientColor[] VmlToGradientColors(string value)
        {
            // WORDSNET-266817 Removes the description of an empty trailing gradient. MS Word ignores such values.
            string[] startColorPairs = (StringUtil.HasChars(value) && value.EndsWith(";", StringComparison.Ordinal))
                    ? value.Remove(value.Length - 1).Split(';')
                    : value.Split(';');
            GradientColor[] result = new GradientColor[startColorPairs.Length];

            for (int i = 0; i < startColorPairs.Length; i++)
            {
                string[] startColor = startColorPairs[i].Split(' ');
                VmlQuantity start = new VmlQuantity(startColor[0]);

                if (!start.IsFixed)
                    return null;

                GradientColor gradientColor = new GradientColor();
                gradientColor.Start = start.ToFixed();
                gradientColor.Color = VmlColor.VmlToColor(startColor[1]);

                result[i] = gradientColor;
            }

            return result;
        }
    }
}
