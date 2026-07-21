// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/11/2015 by Andrey Noskov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Vml
{
    internal class VmlShadowReader : VmlShapeReaderBase
    {
        internal static void ReadShadow(ShapeBase shape, NrxXmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "on":
                        // andrnosk: WORDSNET-12666 If value of this attribute cannot be read 
                        // then default (true) value will be set instead.
                        SetBoolAttribute(shape, ShapeAttr.ShadowOn, value, true);
                        break;
                    case "type":
                        shape.SetShapeAttrInternal(ShapeAttr.ShadowType, VmlEnum.VmlToShadowType(value));
                        break;
                    case "color":
                        shape.SetShapeAttrInternal(ShapeAttr.ShadowColor, VmlColor.VmlToColor(value));
                        break;
                    case "color2":
                        shape.SetShapeAttrInternal(ShapeAttr.ShadowHighlight, VmlColor.VmlToColor(value));
                        break;
                    case "opacity":
                        SetFixedAttribute(shape, ShapeAttr.ShadowOpacity, value);
                        break;
                    case "origin":
                        Set2DFixed(shape, ShapeAttr.ShadowOriginX, ShapeAttr.ShadowOriginY, value);
                        break;
                    case "offset":
                        Set2DEmus(shape, ShapeAttr.ShadowOffsetX, ShapeAttr.ShadowOffsetY, value);
                        break;
                    case "offset2":
                        Set2DEmus(shape, ShapeAttr.ShadowSecondOffsetX, ShapeAttr.ShadowSecondOffsetY, value);
                        break;
                    case "matrix":
                        SetShadowMatrix(shape, value);
                        break;
                    case "obscured":
                        // WORDSNET-19502 Added handling of invalid values.
                        SetBoolAttributeForce(shape, ShapeAttr.ShadowObscured, value);
                        break;
                    default:
                        reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                }
            }
        }

        private static void Set2DEmus(ShapeBase shape, int attrX, int attrY, string value)
        {
            VmlQuantity[] attrValues = VmlToQuantityArray(value);

            SetEmusAttribute(shape, attrX, attrValues[0]);

            if (attrValues.Length > 1)
                SetEmusAttribute(shape, attrY, attrValues[1]);
        }

        private static void SetShadowMatrix(ShapeBase shape, string matrix)
        {
            string[] values = matrix.Split(',');

            int index = 0;
            int maxIndex = values.Length - 1;

            for (int attr = ShapeAttr.ShadowScaleXtoX; attr <= ShapeAttr.ShadowScaleYtoY; attr++)
            {
                SetFixedAttribute(shape, attr, values[index]);

                index++;

                if (index > maxIndex)
                    return;
            }

            for (int attr = ShapeAttr.ShadowPerspectiveX; attr <= ShapeAttr.ShadowPerspectiveY; attr++)
            {
                string value = values[index];

                if (value != "")
                    shape.SetShapeAttrInternal(attr, VmlUtil.VmlToPerspective(value));

                index++;

                if (index > maxIndex)
                    return;
            }
        }
    }
}
