// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/01/2011 by Alexey Titov

using System;
using System.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Effects;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Represents a class building DmlFill objects from xml.
    /// </summary>
    internal class DmlFillReader : DmlReaderBase
    {
        private DmlFillReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
        }

        /// <summary>
        /// Reads shape fill from a specified reader.
        /// </summary>
        internal static DmlFill Read(DocxDocumentReaderBase reader)
        {
            DmlFillReader fillReader = new DmlFillReader(reader);
            return fillReader.ReadCore();
        }

        /// <summary>
        /// Reads text fill from a specified reader.
        /// </summary>
        internal static DmlFill ReadTextFill(DocxDocumentReaderBase reader)
        {
            DmlFillReader fillReader = new DmlFillReader(reader);

            string element = reader.XmlReader.LocalName;
            while (reader.XmlReader.ReadChild(element))
            {
                DmlFill fill = fillReader.ReadTextFillCore();

                if (fill != null)
                    return fill;
            }

            return null;
            }

        /// <summary>
        /// Reads text outline fill from a specified reader.
        /// </summary>
        internal static DmlFill ReadTextOutlineFill(DocxDocumentReaderBase reader)
        {
            DmlFillReader fillReader = new DmlFillReader(reader);
                return fillReader.ReadTextFillCore();
        }

        /// <summary>
        /// Reads text fill core.
        /// </summary>
        private DmlFill ReadTextFillCore()
        {
            switch (XmlReader.LocalName)
            {
                case "gradFill":
                    return ReadGradientFill();
                case "noFill":
                    return ReadNoFill();
                case "solidFill":
                    return ReadSolidFill();
                default:
                    WarnUnexpectedAndIgnoreElement(XmlReader);
                    return null;
            }
        }

        /// <summary>
        /// Reads shape fill core.
        /// </summary>
        private DmlFill ReadCore()
        {
            switch (XmlReader.LocalName)
            {
                case "blipFill":
                    return ReadBlipFill();
                case "gradFill":
                    return ReadGradientFill();
                case "grpFill":
                    return ReadGroupFill();
                case "noFill":
                    return ReadNoFill();
                case "pattFill":
                    return ReadPatternFill();
                case "solidFill":
                    return ReadSolidFill();
                default:
                    WarnUnexpectedAndIgnoreElement(XmlReader);
                    return null;
            }
        }

        /// <summary>
        /// Reads shape solid fill.
        /// </summary>
        private DmlSolidFill ReadSolidFill()
        {
            // WORDSNET-10911 If color of solid fill is not specified, treat as there is no fill at all.
            DmlColor fillColor = DmlColorReader.ReadColor(XmlReader, ComplianceInfo);
            if (fillColor == null)
                return null;

            DmlSolidFill fill = new DmlSolidFill();
            fill.Color = fillColor;
            return fill;
        }

        /// <summary>
        /// Reads patterned fill.
        /// </summary>
        private DmlPatternFill ReadPatternFill()
        {
            DmlPatternFill fill = new DmlPatternFill();
            fill.FillPresetPattern = ReadPatternType();
            while (XmlReader.ReadChild("pattFill"))
            {
                switch (XmlReader.LocalName)
                {
                    case "bgClr":
                        fill.BackgroundColor =
                            DmlColorReader.ReadColor(XmlReader, ComplianceInfo); // Children tags are colors
                        break;
                    case "fgClr":
                        fill.ForegroundColor =
                            DmlColorReader.ReadColor(XmlReader, ComplianceInfo); // Children tags are colors
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
            return fill;
        }

        /// <summary>
        /// Reads shape 'NoFill' fill.
        /// </summary>
        private static DmlNoFill ReadNoFill()
        {
            return new DmlNoFill();
        }

        /// <summary>
        /// Reads group fill.
        /// </summary>
        private static DmlGroupFill ReadGroupFill()
        {
            return new DmlGroupFill();
        }

        /// <summary>
        /// Reads shape gradient fill.
        /// </summary>
        private DmlGradientFill ReadGradientFill()
        {
            DmlGradientFill fill = new DmlGradientFill();
            fill.TileFlipMode = ReadTileFlipAttribute();

            // WORDSNET-17952 The default value for "rotate with shape" flag is "true".
            fill.RotateWithShape = XmlReader.ReadBoolAttribute("rotWithShape", true);

            ReadGradientFillChild(fill);

            return fill;
        }
        /// <summary>
        /// Reads children of 'gradFill' element.
        /// </summary>
        private void ReadGradientFillChild(DmlGradientFill fill)
        {
            while (XmlReader.ReadChild("gradFill"))
            {
                switch (XmlReader.LocalName)
                {
                    case "gsLst":
                        fill.GradientStops = ReadGradientStops();
                        break;
                    case "lin":
                        fill.Gradient = ReadLinearGradient();
                        break;
                    case "path":
                        fill.Gradient = ReadPathGradient();
                        break;
                    case "tileRect":
                        fill.TileRectangle = ReadPercentageOffsetRectangle();
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }

        private DmlPathGradient ReadPathGradient()
        {
            DmlPathGradient pathGradient = new DmlPathGradient();
            pathGradient.Path = ReadPathShadeType();
            while (XmlReader.ReadChild("path"))
            {
                switch (XmlReader.LocalName)
                {
                    case "fillToRect":
                        pathGradient.FillToRectangle = ReadPercentageOffsetRectangle();
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
            return pathGradient;
        }

        private DmlPercentageOffsetRectangle ReadPercentageOffsetRectangle()
        {
            DmlPercentageOffsetRectangle fillToRectangle = new DmlPercentageOffsetRectangle();
            fillToRectangle.BottomOffset =
                DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.ReadAttribute("b", ""), ComplianceInfo);
            fillToRectangle.TopOffset =
                DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.ReadAttribute("t", ""), ComplianceInfo);
            fillToRectangle.LeftOffset =
                DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.ReadAttribute("l", ""), ComplianceInfo);
            fillToRectangle.RightOffset =
                DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.ReadAttribute("r", ""), ComplianceInfo);
            return fillToRectangle;
        }

        private DmlPathShadeType ReadPathShadeType()
        {
            string value = XmlReader.ReadAttribute("path", String.Empty);
            return DmlEnum.DmlToPathShadeType(value);
        }

        private DmlLinearGradient ReadLinearGradient()
        {
            DmlLinearGradient fill = new DmlLinearGradient();
            fill.Angle = new DmlAngle(XmlReader.ReadDoubleAttribute("ang", 0.0));
            fill.IsScaled = XmlReader.ReadBoolAttribute("scaled", false);
            return fill;
        }

        private IList<DmlGradientStop> ReadGradientStops()
        {
            List<DmlGradientStop> gradientStops = new List<DmlGradientStop>();
            while (XmlReader.ReadChild("gsLst"))
            {
                switch (XmlReader.LocalName)
                {
                    case "gs":
                        DmlGradientStop gs = ReadGradientStop();
                        gs.OriginalOrder = gradientStops.Count;
                        gradientStops.Add(gs);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            // WORDSNET-9594 Gradient stops are in incorrect order, that causes problems upon creating GDI gradient brush,
            // fixed by sorting stops by position.
            // WORDSNET-22003 Actually, Word VBA uses original order to determine gradient styles and variants and as a result
            // the ForeColor and BackColor. In addition, Word seems always inserts new gradient stops at the very start of
            // gradient stop collection. As the result, it is more convenient and natural to keep in model gradient
            // stops unsorted and sort it only just before Rendering.
            // So, please do not sort gradient stops here!

            return gradientStops;
        }

        private DmlGradientStop ReadGradientStop()
        {
            DmlGradientStop gradientStop = new DmlGradientStop();
            gradientStop.Position =
                DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.ReadAttribute("pos", ""), ComplianceInfo);
            gradientStop.Color =
                DmlColorReader.ReadColor(XmlReader, ComplianceInfo); // Children tags are colors
            return gradientStop;
        }

        private DmlTileFlipMode ReadTileFlipAttribute()
        {
            string value = XmlReader.ReadAttribute("flip", String.Empty);
            return DmlEnum.DmlToTileFlipMode(value);
        }

        private DmlBlipFill ReadBlipFill()
        {
            DmlBlipFill fill = new DmlBlipFill();
            fill.DotsPerInch = XmlReader.ReadUIntAttribute("dpi", 0);
            fill.RotateWithShape = XmlReader.ReadBoolAttribute("rotWithShape", true);

            while (XmlReader.ReadChild("blipFill"))
            {
                switch (XmlReader.LocalName)
                {
                    case "blip":
                        fill.Blip = ReadBlip(mDocumentReader);
                        break;
                    case "srcRect":
                        fill.SourceRectangle = ReadPercentageOffsetRectangle();
                        break;
                    case "stretch":
                        fill.BlipFillMode = ReadStretch();
                        break;
                    case "tile":
                        fill.BlipFillMode = ReadTile();
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
            return fill;
        }

        private DmlBlipFillTile ReadTile()
        {
            DmlBlipFillTile tile = new DmlBlipFillTile();
            tile.Alignment = ReadAlignment();
            tile.TileFlipMode = ReadTileFlipAttribute();
            tile.HorizontalRatio =
                DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.ReadAttribute("sx", ""), 1.0, ComplianceInfo);
            tile.VerticalRatio =
                DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.ReadAttribute("sy", ""), 1.0, ComplianceInfo);
            tile.HorizontalOffset = XmlReader.ReadAttributeAsEmus("tx", 0.0, ComplianceInfo);
            tile.VerticalOffset = XmlReader.ReadAttributeAsEmus("ty", 0.0, ComplianceInfo);
            return tile;
        }

        private DmlRectangleAlignment ReadAlignment()
        {
            return DmlEnum.DmlToRectangleAlignment(XmlReader.ReadAttribute("algn", string.Empty));
        }

        private DmlBlipFillStretch ReadStretch()
        {
            DmlBlipFillStretch blipFillStretch = new DmlBlipFillStretch();
            while (XmlReader.ReadChild("stretch"))
            {
                switch (XmlReader.LocalName)
                {
                    case "fillRect":
                        blipFillStretch.FillRectangle = ReadPercentageOffsetRectangle();
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
            return blipFillStretch;
        }

        internal static DmlBlip ReadBlip(DocxDocumentReaderBase reader)
        {
            DmlBlip blip = new DmlBlip();
            blip.CompressionState = ReadCompressionState(reader.XmlReader);
            blip.Document = reader.Document;

            string embed = reader.XmlReader.ReadAttribute("embed", String.Empty);
            if (StringUtil.HasChars(embed))
                blip.EmbedImage = reader.GetBinData(embed);

            string link = reader.XmlReader.ReadAttribute("link", String.Empty);
            if (StringUtil.HasChars(link))
                blip.ImageLink = reader.GetRelationshipTarget(link);

            ReadBlipChildren(blip, reader);
            return blip;
        }

        private static void ReadBlipChildren(DmlBlip blip, DocxDocumentReaderBase reader)
        {
            IList<DmlEffect> effects = new List<DmlEffect>();
            NrxXmlReader xmlReader = reader.XmlReader;

            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "extLst":
                        blip.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    case "biLevel":
                        effects.Add(DmlEffectReader.ReadBiLevelEffect(reader));
                        break;
                    case "clrChange":
                        effects.Add(DmlEffectReader.ReadColorChangeEffect(reader));
                        break;
                    case "grayscl":
                        effects.Add(DmlEffectReader.ReadGrayScaleEffect());
                        break;
                    case "lum":
                        effects.Add(DmlEffectReader.ReadLuminanceEffect(reader));
                        break;
                    case "duotone":
                        effects.Add(DmlEffectReader.ReadDuotoneEffect(reader));
                        break;
                    case "blur":
                        effects.Add(DmlEffectReader.ReadBlurEffect(reader));
                        break;
                    case "alphaBiLevel":
                        effects.Add(DmlEffectReader.ReadAlphaBiLevelEffect(reader));
                        break;
                    case "alphaCeiling":
                        effects.Add(new DmlAlphaCeilingEffect());
                        break;
                    case "alphaFloor":
                        effects.Add(new DmlAlphaFloorEffect());
                        break;
                    case "alphaInv":
                        effects.Add(DmlEffectReader.ReadAlphaInveseEffect(reader));
                        break;
                    case "alphaMod":
                        effects.Add(DmlEffectReader.ReadAlphaModulateEffect(reader));
                        break;
                    case "alphaModFix":
                        effects.Add(DmlEffectReader.ReadAlphaModulateFixedEffect(reader));
                        break;
                    case "alphaRepl":
                        effects.Add(DmlEffectReader.ReadAlphaReplaceEffect(reader));
                        break;
                    case "clrRepl":
                        effects.Add(DmlEffectReader.ReadColorReplaceEffect(reader));
                        break;
                    case "fillOverlay":
                        effects.Add(DmlEffectReader.ReadFillOverlayEffect(reader));
                        break;
                    case "hsl":
                        effects.Add(DmlEffectReader.ReadHueSaturationLuminanceEffect(reader));
                        break;
                    case "tint":
                        effects.Add(DmlEffectReader.ReadTintEffect(reader));
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            blip.Effects = effects;
        }

        private static DmlCompressionState ReadCompressionState(NrxXmlReader xmlReader)
        {
            return DmlEnum.DmlToCompressionState(xmlReader.ReadAttribute("cstate", string.Empty));
        }

        private PatternType ReadPatternType()
        {
            string value = XmlReader.ReadAttribute("prst", String.Empty);
            return DmlEnum.DmlToPatternType(value);
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
    }
}
