// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/11/2015 by Andrey Noskov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Vml
{
    internal class VmlExtrusionReader : VmlShapeReaderBase
    {
        internal static void ReadExtrusion(ShapeBase shape, NrxXmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "ext": // v:ext
                        // Always "view", ignore.
                        break;
                    case "specularity":
                        SetFixedAttribute(shape, ShapeAttr.TDSpecularAmount, value);
                        break;
                    case "diffusity":
                        SetFixedAttribute(shape, ShapeAttr.TDDiffuseAmount, value);
                        break;
                    case "shininess":
                        shape.SetShapeAttrInternal(ShapeAttr.TDShininess, reader.ValueAsInt);
                        break;
                    case "edge":
                        SetEmusAttribute(shape, ShapeAttr.TDEdgeThickness, value);
                        break;
                    case "foredepth":
                        SetEmusAttribute(shape, ShapeAttr.TDExtrudeForward, value);
                        break;
                    case "backdepth":
                        SetEmusAttribute(shape, ShapeAttr.TDExtrudeBackward, value);
                        break;
                    case "plane":
                        shape.SetShapeAttrInternal(ShapeAttr.TDExtrudePlane, VmlEnum.VmlToPlaneType(value));
                        break;
                    case "color":
                        shape.SetShapeAttrInternal(ShapeAttr.TDExtrusionColor, VmlColor.VmlToColor(value));
                        break;
                    case "on":
                        SetBoolAttribute(shape, ShapeAttr.TDOn, value);
                        break;
                    case "metal":
                        SetBoolAttribute(shape, ShapeAttr.TDMetallic, value);
                        break;
                    case "lightface":
                        SetBoolAttribute(shape, ShapeAttr.TDLightFace, value);
                        break;
                    case "rotationangle":
                    {
                        SetRotationAngles(shape, value);
                        break;
                    }
                    case "orientation":
                        Set3DInt(shape, ShapeAttr.TDRotationAxisX, ShapeAttr.TDRotationAxisY, ShapeAttr.TDRotationAxisZ, value);
                        break;
                    case "orientationangle":
                        SetFixedDegreesAttribute(shape, ShapeAttr.TDRotationAngle, new VmlQuantity(value));
                        break;
                    case "rotationcenter":
                        SetRotationCenter(shape, value);
                        break;
                    case "render":
                        shape.SetShapeAttrInternal(ShapeAttr.TDRenderMode, VmlEnum.VmlToThreeDRenderMode(value));
                        break;
                    case "facet":
                        SetFixedAttribute(shape, ShapeAttr.TDTolerance, value);
                        break;
                    case "viewpoint":
                        Set3DEmus(shape, ShapeAttr.TDViewpointX, ShapeAttr.TDViewpointY, ShapeAttr.TDViewpointZ, value);
                        break;
                    case "viewpointorigin":
                        Set2DFixed(shape, ShapeAttr.TDOriginX, ShapeAttr.TDOriginY, value);
                        break;
                    case "skewangle":
                        SetFixedDegreesAttribute(shape, ShapeAttr.TDSkewAngle, new VmlQuantity(value));
                        break;
                    case "skewamt":
                        SetPercentAttribute(shape, ShapeAttr.TDSkewAmount, value);
                        break;
                    case "brightness":
                        SetFixedAttribute(shape, ShapeAttr.TDAmbientIntensity, value);
                        break;
                    case "lightposition":
                        Set3DInt(shape, ShapeAttr.TDKeyX, ShapeAttr.TDKeyY, ShapeAttr.TDKeyZ, value);
                        break;
                    case "lightlevel":
                        SetFixedAttribute(shape, ShapeAttr.TDKeyIntensity, value);
                        break;
                    case "lightposition2":
                        Set3DInt(shape, ShapeAttr.TDFillX, ShapeAttr.TDFillY, ShapeAttr.TDFillZ, value);
                        break;
                    case "lightlevel2":
                        SetFixedAttribute(shape, ShapeAttr.TDFillIntensity, value);
                        break;
                    case "colormode":
                        // Seems that MS Word does not use this attribute.
                        break;
                    case "lockrotationcenter":
                        SetBoolAttribute(shape, ShapeAttr.TDConstrainRotation, value);
                        break;
                    case "autorotationcenter":
                        SetBoolAttribute(shape, ShapeAttr.TDRotationCenterAuto, value);
                        break;
                    case "type":
                        if (value == "perspective")
                            shape.SetShapeAttrInternal(ShapeAttr.TDParallel, false);
                        break;
                    case "lightharsh":
                        SetBoolAttribute(shape, ShapeAttr.TDKeyHarsh, value);
                        break;
                    case "lightharsh2":
                        // WORDSNET-19213 Added handling of invalid values.
                        SetBoolAttributeForce(shape, ShapeAttr.TDFillHarsh, value);
                        break;
                    default:
                        reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                }
            }
        }

        private static void SetRotationCenter(ShapeBase shape, string value)
        {
            VmlQuantity[] attrValues = VmlToQuantityArray(value);

            SetFixedAttribute(shape, ShapeAttr.TDRotationCenterX, attrValues[0]);

            if (attrValues.Length > 1)
                SetFixedAttribute(shape, ShapeAttr.TDRotationCenterY, attrValues[1]);

            if (attrValues.Length > 2)
                SetEmusAttribute(shape, ShapeAttr.TDRotationCenterZ, attrValues[2]);
        }

        /// <summary>
        /// Sets rotation x and y angles.
        /// </summary>
        internal static void SetRotationAngles(ShapeBase shape, string value)
        {
            VmlQuantity[] attrValues = VmlToQuantityArray(value);

            SetFixedDegreesAttribute(shape, ShapeAttr.TDRotationAngleX, attrValues[0]);

            if (attrValues.Length > 1)
                SetFixedDegreesAttribute(shape, ShapeAttr.TDRotationAngleY, attrValues[1]);
        }

        private static void Set3DInt(ShapeBase shape, int attrX, int attrY, int attrZ, string value)
        {
            string[] attrValues = value.Split(',');

            SetIntAttribute(shape, attrX, attrValues[0]);

            if (attrValues.Length > 1)
                SetIntAttribute(shape, attrY, attrValues[1]);

            if (attrValues.Length > 2)
                SetIntAttribute(shape, attrZ, attrValues[2]);
        }

        private static void Set3DEmus(ShapeBase shape, int attrX, int attrY, int attrZ, string value)
        {
            VmlQuantity[] attrValues = VmlToQuantityArray(value);

            SetEmusAttribute(shape, attrX, attrValues[0]);

            if (attrValues.Length > 1)
                SetEmusAttribute(shape, attrY, attrValues[1]);

            if (attrValues.Length > 2)
                SetEmusAttribute(shape, attrZ, attrValues[2]);
        }
    }
}
