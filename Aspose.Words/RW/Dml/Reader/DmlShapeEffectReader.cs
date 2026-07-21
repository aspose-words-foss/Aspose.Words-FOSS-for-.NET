// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/24/2013 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    internal class DmlShapeEffectReader : DmlReaderBase
    {
        private DmlShapeEffectReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
        }

        /// <summary>
        /// Reads by specified XML reader and returns effects collection.
        /// </summary>
        internal static DmlShapeEffectsCollection ReadEffects(DocxDocumentReaderBase reader, bool isTheme, bool isEffectDag)
        {
            DmlShapeEffectsCollection effects = new DmlShapeEffectsCollection(isTheme, isEffectDag);
            NrxXmlReader xmlReader = reader.XmlReader;

            string tagName = xmlReader.LocalName;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "name":
                        effects.DagName = xmlReader.Value;
                        break;
                    case "type":
                        effects.DagType = DmlEnum.DmlToEffectDagType(xmlReader.Value);
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }

            while (xmlReader.ReadChild(tagName))
            {
                // Children tag are effects tag.
                // Processing is delegated to the ShapeEffectBuilder.
                DmlShapeEffect dmlEffect = Read(reader);
                if (dmlEffect != null)
                    effects.AddEffect(dmlEffect);
            }

            return effects;
        }

        /// <summary>
        /// Builds an effect object by specified XML reader.
        /// </summary>
        internal static DmlShapeEffect Read(DocxDocumentReaderBase reader)
        {
            DmlShapeEffectReader shapeEffectReader = new DmlShapeEffectReader(reader);

            switch (reader.XmlReader.LocalName)
            {
                case "blur":
                    return shapeEffectReader.ReadBlur();
                case "fillOverlay":
                    return shapeEffectReader.ReadFillOverlay();
                case "glow":
                    return shapeEffectReader.ReadGlow();
                case "innerShdw":
                    return shapeEffectReader.ReadInnerShadow();
                case "shadow": // This is shadow definition for text effect shadow.
                case "outerShdw":
                    return shapeEffectReader.ReadOuterShadow();
                case "prstShdw":
                    return shapeEffectReader.ReadPresetShadow();
                case "reflection":
                    return shapeEffectReader.ReadReflection();
                case "softEdge":
                    return shapeEffectReader.ReadSoftEdge();
                default:
                    WarnUnexpectedAndIgnoreElement(reader.XmlReader);
                    return null;
            }
        }

        private DmlShapeBlurEffect ReadBlur()
        {
            DmlShapeBlurEffect blurEffect = new DmlShapeBlurEffect();
            // When blur is set in effectsLst radius is set in emus instead of pixels.
            blurEffect.Radius = ConvertUtilCore.EmuToPixel((int)XmlReader.ReadDoubleAttribute("rad", 0.0));
            blurEffect.Grow = XmlReader.ReadBoolAttribute("grow", true);
            return blurEffect;
        }

        private DmlShapeFillOverlayEffect ReadFillOverlay()
        {
            DmlShapeFillOverlayEffect fillOverlayEffect = new DmlShapeFillOverlayEffect();
            fillOverlayEffect.Blend = DmlEnum.DmlToEffectBlendMode(XmlReader.ReadAttribute("blend", ""));

            while (XmlReader.ReadChild("fillOverlay"))
            {
                switch (XmlReader.LocalName)
                {
                    case "blipFill":
                    case "gradFill":
                    case "grpFill":
                    case "pattFill":
                    case "noFill":
                    case "solidFill":
                        fillOverlayEffect.Fill = DmlFillReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            return fillOverlayEffect;
        }

        private DmlShapeGlowEffect ReadGlow()
        {
            DmlShapeGlowEffect glowEffect = new DmlShapeGlowEffect();
            glowEffect.Radius = Validate(XmlReader.ReadDoubleAttribute("rad", 0.0), MaxGlowBlurValue);
            XmlReader.MoveToElement();
            glowEffect.Color = DmlColorReader.ReadColor(XmlReader, ComplianceInfo);
            return glowEffect;
        }

        private DmlShapeEffect ReadInnerShadow()
        {
            DmlShapeInnerShadowEffect innerShadowEffect = new DmlShapeInnerShadowEffect();

            while (XmlReader.MoveToNextAttribute())
            {
                switch (XmlReader.LocalName)
                {
                    case "blurRad":
                        innerShadowEffect.BlurRadius = Validate(XmlReader.ValueAsDouble, MaxShadowBlurValue);
                        break;
                    case "dist":
                        innerShadowEffect.Distance = Validate(XmlReader.ValueAsDouble, MaxShadowDistanceValue);
                        break;
                    case "dir":
                        innerShadowEffect.Direction = new DmlAngle(XmlReader.ValueAsDouble);
                        break;
                    default:
                        WarnUnexpected(XmlReader);
                        break;
                }
            }

            XmlReader.MoveToElement();
            innerShadowEffect.Color = DmlColorReader.ReadColor(XmlReader, ComplianceInfo);

            return innerShadowEffect;
        }

        private DmlShapeOuterShadowEffect ReadOuterShadow()
        {
            DmlShapeOuterShadowEffect outerShadowEffect = new DmlShapeOuterShadowEffect();

            while (XmlReader.MoveToNextAttribute())
            {
                switch (XmlReader.LocalName)
                {
                    case "blurRad":
                        outerShadowEffect.BlurRadius = Validate(XmlReader.ValueAsDouble, MaxShadowBlurValue);
                        break;
                    case "dist":
                        outerShadowEffect.Distance = Validate(XmlReader.ValueAsDouble, MaxShadowDistanceValue);
                        break;
                    case "dir":
                        outerShadowEffect.Direction = new DmlAngle(XmlReader.ValueAsDouble);
                        break;
                    case "sx":
                        outerShadowEffect.HorizontalScale =
                            ValidateScaleValue(DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.Value, ComplianceInfo));
                        break;
                    case "sy":
                        outerShadowEffect.VerticalScale =
                            ValidateScaleValue(DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.Value, ComplianceInfo));
                        break;
                    case "kx":
                        outerShadowEffect.HorizontalSkew = new DmlAngle(XmlReader.ValueAsDouble);
                        break;
                    case "ky":
                        outerShadowEffect.VerticalSkew = new DmlAngle(XmlReader.ValueAsDouble);
                        break;
                    case "algn":
                        outerShadowEffect.Alignment = ValueToAlignment(XmlReader.Value);
                        break;
                    case "rotWithShape":
                        outerShadowEffect.RotateWithShape = XmlReader.ValueAsBool;
                        break;
                    default:
                        WarnUnexpected(XmlReader);
                        break;
                }
            }

            XmlReader.MoveToElement();

            // WORDSNET-20570 Added resilience to not allow 'null' color for non-empty shadow effect in model.
            DmlColor color = DmlColorReader.ReadColor(XmlReader, ComplianceInfo);
            outerShadowEffect.Color = (color == null) ? DmlColor.Empty : color;

            return outerShadowEffect;
        }

        private DmlShapePresetShadowEffect ReadPresetShadow()
        {
            DmlShapePresetShadowEffect prstShadowEffect = new DmlShapePresetShadowEffect();

            while (XmlReader.MoveToNextAttribute())
            {
                switch (XmlReader.LocalName)
                {
                    case "dist":
                        prstShadowEffect.Distance = Validate(XmlReader.ValueAsDouble, MaxShadowDistanceValue);
                        break;
                    case "dir":
                        prstShadowEffect.Direction = new DmlAngle(XmlReader.ValueAsDouble);
                        break;
                    case "prst":
                        prstShadowEffect.PresetShadow = DmlEnum.DmlToPresetShadow(XmlReader.Value);
                        break;
                    default:
                        WarnUnexpected(XmlReader);
                        break;
                }
            }

            XmlReader.MoveToElement();
            prstShadowEffect.Color = DmlColorReader.ReadColor(XmlReader, ComplianceInfo);

            return prstShadowEffect;
        }

        private DmlShapeEffect ReadReflection()
        {
            DmlShapeReflectionEffect reflectionEffect = new DmlShapeReflectionEffect();

            while (XmlReader.MoveToNextAttribute())
            {
                switch (XmlReader.LocalName)
                {
                    case "blurRad":
                        reflectionEffect.BlurRadius = Validate(XmlReader.ValueAsDouble, MaxShadowBlurValue);
                        break;
                    case "stA":
                        reflectionEffect.StartAlpha =
                            DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.Value, ComplianceInfo);
                        break;
                    case "stPos":
                        reflectionEffect.StartPosition =
                            DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.Value, ComplianceInfo);
                        break;
                    case "endA":
                        reflectionEffect.EndAlpha =
                            DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.Value, ComplianceInfo);
                        break;
                    case "endPos":
                        reflectionEffect.EndPosition =
                            DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.Value, ComplianceInfo);
                        break;
                    case "dist":
                        reflectionEffect.Distance = Validate(XmlReader.ValueAsDouble, MaxReflectionDistanceValue);
                        break;
                    case "dir":
                        reflectionEffect.Direction = new DmlAngle(XmlReader.ValueAsDouble);
                        break;
                    case "fadeDir":
                        reflectionEffect.FadeDirection = new DmlAngle(XmlReader.ValueAsDouble);
                        break;
                    case "sx":
                        reflectionEffect.HorizontalScale =
                            ValidateScaleValue(DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.Value, ComplianceInfo));
                        break;
                    case "sy":
                        reflectionEffect.VerticalScale =
                            ValidateScaleValue(DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.Value, ComplianceInfo));
                        break;
                    case "kx":
                        reflectionEffect.HorizontalSkew = new DmlAngle(XmlReader.ValueAsDouble);
                        break;
                    case "ky":
                        reflectionEffect.VerticalSkew = new DmlAngle(XmlReader.ValueAsDouble);
                        break;
                    case "algn":
                        reflectionEffect.Alignment = ValueToAlignment(XmlReader.Value);
                        break;
                    case "rotWithShape":
                        reflectionEffect.RotateWithShape = XmlReader.ValueAsBool;
                        break;
                    default:
                        WarnUnexpected(XmlReader);
                        break;
                }
            }

            XmlReader.MoveToElement();

            return reflectionEffect;
        }

        private static double Validate(double srcValue, double maxValue)
        {
            if (srcValue < 0.0)
                srcValue = srcValue * (-1.0);

            if (srcValue > maxValue)
                srcValue = maxValue;

            return srcValue;
        }

        private static double ValidateScaleValue(double srcValue)
        {
            if (srcValue > MaxScaleValue)
                srcValue = 0.0;

            return srcValue;
        }

        private DmlShapeSoftEdgeEffect ReadSoftEdge()
        {
            DmlShapeSoftEdgeEffect softEdgeEffect = new DmlShapeSoftEdgeEffect();
            softEdgeEffect.Radius = XmlReader.ReadDoubleAttribute("rad", 0.0);
            return softEdgeEffect;
        }

        private static DmlRectangleAlignment ValueToAlignment(string value)
        {
            switch (value)
            {
                case "tl":
                    return DmlRectangleAlignment.TopLeft;
                case "t":
                    return DmlRectangleAlignment.Top;
                case "tr":
                    return DmlRectangleAlignment.TopRight;
                case "l":
                    return DmlRectangleAlignment.Left;
                case "ctr":
                    return DmlRectangleAlignment.Center;
                case "r":
                    return DmlRectangleAlignment.Right;
                case "bl":
                    return DmlRectangleAlignment.BottomLeft;
                case "b":
                    return DmlRectangleAlignment.Bottom;
                case "br":
                    return DmlRectangleAlignment.BottomRight;
                case "none": // appears in Dml Text Effects
                    return DmlRectangleAlignment.None;
                default:
                    return DmlRectangleAlignment.TopLeft;
            }
        }

        private NrxXmlReader XmlReader
        {
            get { return mDocumentReader.XmlReader; }
        }

        private OoxmlComplianceInfo ComplianceInfo
        {
            get { return mDocumentReader.ComplianceInfo; }
        }

        private readonly DocxDocumentReaderBase mDocumentReader;

        // Values ​​are taken based on valid values ​​in Word.
        private const double MaxShadowBlurValue = 1270000.0;
        private const double MaxGlowBlurValue = 1905000.0;
        private const double MaxShadowDistanceValue = 2540000.0;
        private const double MaxReflectionDistanceValue = 1270000.0;
        private const double MaxScaleValue = 2.0;

    }
}
