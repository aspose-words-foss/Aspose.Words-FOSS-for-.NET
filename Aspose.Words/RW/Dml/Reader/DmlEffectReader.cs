// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/11/2011 by Alexey Kachalov

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Effects;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Represents a class that builds effects objects from XML.
    /// </summary>
    internal class DmlEffectReader : DmlReaderBase
    {
        private DmlEffectReader()
        {
        }

        /// <summary>
        /// Builds an effect object by specified XML reader.
        /// </summary>
        internal static DmlEffect Read(DocxDocumentReaderBase reader)
        {
            switch (reader.XmlReader.LocalName)
            {
                case "biLevel":
                    return ReadBiLevelEffect(reader);
                case "clrChange":
                    return ReadColorChangeEffect(reader);
                case "grayscl":
                    return ReadGrayScaleEffect();
                case "lum":
                    return ReadLuminanceEffect(reader);
                case "duotone":
                    return ReadDuotoneEffect(reader);
                case "blur":
                    return ReadBlurEffect(reader);
                case "alphaBiLevel":
                    return ReadAlphaBiLevelEffect(reader);
                case "alphaCeiling":
                    return new DmlAlphaCeilingEffect();
                case "alphaFloor":
                    return new DmlAlphaFloorEffect();
                case "alphaInv":
                    return ReadAlphaInveseEffect(reader);
                case "alphaMod":
                    return ReadAlphaModulateEffect(reader);
                case "alphaModFix":
                    return ReadAlphaModulateFixedEffect(reader);
                case "alphaRepl":
                    return ReadAlphaReplaceEffect(reader);
                case "clrRepl":
                    return ReadColorReplaceEffect(reader);
                case "fillOverlay":
                    return ReadFillOverlayEffect(reader);
                case "hsl":
                    return ReadHueSaturationLuminanceEffect(reader);
                case "tint":
                    return ReadTintEffect(reader);
                default:
                    WarnUnexpectedAndIgnoreElement(reader.XmlReader);
                    return null;
            }
        }

        internal static DmlTintEffect ReadTintEffect(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            DmlTintEffect tint = new DmlTintEffect();
            tint.Amount = DmlPercentageUtil.FromPercentOrDmlPercent(xmlReader.ReadAttribute("amt", ""), complianceInfo);
            tint.Hue = DmlPercentageUtil.FromPercentOrDmlPercent(xmlReader.ReadAttribute("hue", ""), complianceInfo);
            return tint;
        }

        internal static DmlHueSaturationLuminanceEffect ReadHueSaturationLuminanceEffect(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            DmlHueSaturationLuminanceEffect hsl = new DmlHueSaturationLuminanceEffect();
            hsl.Hue = new DmlAngle(xmlReader.ReadDoubleAttribute("hue", 0.0));
            hsl.Luminance =
                DmlPercentageUtil.FromPercentOrDmlPercent(xmlReader.ReadAttribute("lum", ""), complianceInfo);
            hsl.Saturation =
                DmlPercentageUtil.FromPercentOrDmlPercent(xmlReader.ReadAttribute("sat", ""), complianceInfo);
            return hsl;
        }

        internal static DmlFillOverlayEffect ReadFillOverlayEffect(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DmlFillOverlayEffect fillOverlay = new DmlFillOverlayEffect();
            fillOverlay.Blend = DmlEnum.DmlToEffectBlendMode(xmlReader.ReadAttribute("blend", ""));

            while (xmlReader.ReadChild("fillOverlay"))
            {
                switch (xmlReader.LocalName)
                {
                    case "blipFill":
                    case "gradFill":
                    case "grpFill":
                    case "pattFill":
                    case "noFill":
                    case "solidFill":
                        fillOverlay.Fill = DmlFillReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return fillOverlay;
        }

        internal static DmlColorReplaceEffect ReadColorReplaceEffect(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            DmlColorReplaceEffect colorReplace = new DmlColorReplaceEffect();
            colorReplace.Color = DmlColorReader.Read(xmlReader, complianceInfo);
            return colorReplace;
        }

        internal static DmlAlphaReplaceEffect ReadAlphaReplaceEffect(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            DmlAlphaReplaceEffect alphaReplace = new DmlAlphaReplaceEffect();
            alphaReplace.Alpha =
                DmlPercentageUtil.FromPercentOrDmlPercent(xmlReader.ReadAttribute("a", ""), complianceInfo);
            return alphaReplace;
        }

        internal static DmlAlphaModulateFixedEffect ReadAlphaModulateFixedEffect(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            DmlAlphaModulateFixedEffect alphaModulateFixed = new DmlAlphaModulateFixedEffect();

            // andrnosk: WORDSNET-11656 Default value for "amt" equals 100000, please see spec ECMA 376 Part4 p.5.1.10.6
            alphaModulateFixed.Amount =
                DmlPercentageUtil.FromPercentOrDmlPercent(xmlReader.ReadAttribute("amt", ""), 1.0, complianceInfo);
            return alphaModulateFixed;
        }

        internal static DmlAlphaModulateEffect ReadAlphaModulateEffect(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DmlAlphaModulateEffect alphaModulate = new DmlAlphaModulateEffect();

            while (xmlReader.ReadChild("alphaMod"))
            {
                switch (xmlReader.LocalName)
                {
                    case "cont":
                    {
                        while (xmlReader.ReadChild("cont"))
                        {
                            // Children tag are effects tag.
                            // According to the specification in the container might be picture effects as well as shape effects,
                            // lets read only picture effects for now.
                            DmlEffect dmlEffect = Read(reader);
                            if (dmlEffect != null)
                                alphaModulate.EffectsContainer.Add(dmlEffect);
                        }
                        break;
                    }
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return alphaModulate;
        }

        internal static DmlAlphaInverseEffect ReadAlphaInveseEffect(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            DmlAlphaInverseEffect alphaInverse = new DmlAlphaInverseEffect();
            alphaInverse.Color = DmlColorReader.Read(xmlReader, complianceInfo);
            return alphaInverse;
        }

        internal static DmlAlphaBiLevelEffect ReadAlphaBiLevelEffect(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            DmlAlphaBiLevelEffect alphaBiLevel = new DmlAlphaBiLevelEffect();
            alphaBiLevel.Threshold =
                DmlPercentageUtil.FromPercentOrDmlPercent(xmlReader.ReadAttribute("thresh", ""), complianceInfo);
            return alphaBiLevel;
        }

        internal static DmlBlurEffect ReadBlurEffect(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DmlBlurEffect blurEffect = new DmlBlurEffect();
            blurEffect.Radius = xmlReader.ReadDoubleAttribute("rad", 0.0);
            blurEffect.Grow = xmlReader.ReadBoolAttribute("grow", true);
            return blurEffect;
        }

        internal static DmlBiLevelEffect ReadBiLevelEffect(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            DmlBiLevelEffect biLevelEffect = new DmlBiLevelEffect();
            biLevelEffect.Threshold =
                DmlPercentageUtil.FromPercentOrDmlPercent(xmlReader.ReadAttribute("thresh", ""), complianceInfo);
            return biLevelEffect;
        }

        internal static DmlColorChangeEffect ReadColorChangeEffect(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            DmlColorChangeEffect colorChangeEffect = new DmlColorChangeEffect();
            colorChangeEffect.ConsiderAlphaValues = xmlReader.ReadBoolAttribute("useA", true);
            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "clrFrom":
                        colorChangeEffect.SourceColor = DmlColorReader.ReadColor(xmlReader, complianceInfo);
                        break;
                    case "clrTo":
                        colorChangeEffect.DestinationColor = DmlColorReader.ReadColor(xmlReader, complianceInfo);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
            return colorChangeEffect;
        }

        internal static DmlGrayScaleEffect ReadGrayScaleEffect()
        {
            return new DmlGrayScaleEffect();
        }

        internal static DmlLuminanceEffect ReadLuminanceEffect(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            DmlLuminanceEffect luminanceEffect = new DmlLuminanceEffect();
            luminanceEffect.Brightness = ReadBrightness(xmlReader, complianceInfo);
            luminanceEffect.Contrast = ReadContrast(xmlReader, complianceInfo);
            return luminanceEffect;
        }

        internal static DmlDuotoneEffect ReadDuotoneEffect(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            DmlDuotoneEffect duotoneEffect = new DmlDuotoneEffect();

            string tagName = xmlReader.LocalName;

            // Read the first color.
            if (xmlReader.ReadChild(tagName))
                duotoneEffect.Color1 = DmlColorReader.Read(xmlReader, complianceInfo);

            // Read the second color.
            if (xmlReader.ReadChild(tagName))
                duotoneEffect.Color2 = DmlColorReader.Read(xmlReader, complianceInfo);

            return duotoneEffect;
        }

        private static double ReadContrast(NrxXmlReader xmlReader, OoxmlComplianceInfo complianceInfo)
        {
            // Change contrast ranges from (-1, 1) to (0, 1).
            double percentVal =
                DmlPercentageUtil.FromPercentOrDmlPercent(xmlReader.ReadAttribute("contrast", ""), complianceInfo);
            return (percentVal + 1)/2;
        }

        private static double ReadBrightness(NrxXmlReader xmlReader, OoxmlComplianceInfo complianceInfo)
        {
            // Change brightness ranges from (-1, 1) to (0, 1).
            double percentVal =
                DmlPercentageUtil.FromPercentOrDmlPercent(xmlReader.ReadAttribute("bright", ""), complianceInfo);
            return (percentVal + 1)/2;
        }

    }
}
