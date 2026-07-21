// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/17/2014 by Alexey Noskov

using System;
using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Effects;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlEffectsWriter
    {
        internal static void Write(IList<DmlEffect> effects, IDmlShapeWriterContext writer)
        {
            foreach (DmlEffect effect in effects)
            {
                switch (effect.Type)
                {
                    case DmlEffectType.AlphaBiLevel:
                        WriteAlphaBiLevel((DmlAlphaBiLevelEffect)effect, writer);
                        break;
                    case DmlEffectType.AlphaCeiling:
                        WriteAlphaCeiling(writer);
                        break;
                    case DmlEffectType.AlphaFloor:
                        WriteAlphaFloor(writer);
                        break;
                    case DmlEffectType.AlphaInverse:
                        WriteAlphaInverse((DmlAlphaInverseEffect)effect, writer);
                        break;
                    case DmlEffectType.AlphaModulate:
                        WriteAlphaModulate((DmlAlphaModulateEffect)effect, writer);
                        break;
                    case DmlEffectType.AlphaModulateFixed:
                        WriteAlphaModulateFixed((DmlAlphaModulateFixedEffect)effect, writer);
                        break;
                    case DmlEffectType.AlphaReplace:
                        WriteAlphaReplace((DmlAlphaReplaceEffect)effect, writer);
                        break;
                    case DmlEffectType.BiLevel:
                        WriteBiLevel((DmlBiLevelEffect)effect, writer);
                        break;
                    case DmlEffectType.Blur:
                        WriteBlur((DmlBlurEffect)effect, writer);
                        break;
                    case DmlEffectType.ColorChange:
                        WriteColorChange((DmlColorChangeEffect)effect, writer);
                        break;
                    case DmlEffectType.ColorReplace:
                        WriteColorReplace((DmlColorReplaceEffect)effect, writer);
                        break;
                    case DmlEffectType.Duotone:
                        WriteDuotone((DmlDuotoneEffect)effect, writer);
                        break;
                    case DmlEffectType.FillOverlay:
                        WriteFillOverlay((DmlFillOverlayEffect)effect, writer);
                        break;
                    case DmlEffectType.GrayScale:
                        WriteGrayScale(writer);
                        break;
                    case DmlEffectType.HueSaturationLuminance:
                        WriteHueSaturationLuminance((DmlHueSaturationLuminanceEffect)effect, writer);
                        break;
                    case DmlEffectType.Luminance:
                        WriteLuminance((DmlLuminanceEffect)effect, writer);
                        break;
                    case DmlEffectType.Tint:
                        WriteTint((DmlTintEffect)effect, writer);
                        break;
                    default:
                        throw new ArgumentException("Unexpected Dml effect type.");
                }
            }
        }

        private static void WriteAlphaBiLevel(DmlAlphaBiLevelEffect effect, IDmlShapeWriterContext writer)
        {
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:alphaBiLevel");
            builder.WriteAttribute("thresh", DmlPercentageUtil.ToPercentOrDmlPercent(effect.Threshold, isIsoStrict));
            builder.EndElement("a:alphaBiLevel");
        }

        private static void WriteAlphaCeiling(IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.WriteEmptyElement("a:alphaCeiling");
        }

        private static void WriteAlphaFloor(IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.WriteEmptyElement("a:alphaFloor");
        }

        private static void WriteAlphaInverse(DmlAlphaInverseEffect effect, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:alphaInv");
            DmlColorWriter.Write(effect.Color, writer);
            builder.EndElement("a:alphaInv");
        }

        private static void WriteAlphaModulate(DmlAlphaModulateEffect effect, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:alphaMod");
            builder.StartElement("a:cont");
            Write(effect.EffectsContainer, writer);
            builder.EndElement("a:cont");
            builder.EndElement("a:alphaMod");
        }

        private static void WriteAlphaModulateFixed(DmlAlphaModulateFixedEffect effect, IDmlShapeWriterContext writer)
        {
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            double amount = effect.Amount;

            if (isIsoStrict)
            {
                // According to spec the type of the alpha value is "ST_PositivePercentage", and
                // value have to be matched for mask "[0-9]+(\.[0-9]+)?%". Mimic MSW behavior
                // and take only integer part of the value.
                amount = System.Math.Round(amount, 2, MidpointRounding.AwayFromZero);
            }

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:alphaModFix");
            builder.WriteAttribute("amt", DmlPercentageUtil.ToPercentOrDmlPercent(amount, isIsoStrict));
            builder.EndElement("a:alphaModFix");
        }

        private static void WriteAlphaReplace(DmlAlphaReplaceEffect effect, IDmlShapeWriterContext writer)
        {
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:alphaRepl");
            builder.WriteAttribute("a", DmlPercentageUtil.ToPercentOrDmlPercent(effect.Alpha, isIsoStrict));
            builder.EndElement("a:alphaRepl");
        }

        private static void WriteBiLevel(DmlBiLevelEffect effect, IDmlShapeWriterContext writer)
        {
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:biLevel");
            builder.WriteAttribute("thresh", DmlPercentageUtil.ToPercentOrDmlPercent(effect.Threshold, isIsoStrict));
            builder.EndElement("a:biLevel");
        }

        internal static void WriteBlur(DmlBlurEffect effect, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:blur");
            builder.WriteAttribute("rad", effect.Radius);
            builder.WriteAttribute("grow", effect.Grow);
            builder.EndElement("a:blur");
        }

        private static void WriteColorChange(DmlColorChangeEffect effect, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:clrChange");

            if (!effect.ConsiderAlphaValues)
                builder.WriteAttribute("useA", effect.ConsiderAlphaValues);

            builder.StartElement("a:clrFrom");
            DmlColorWriter.Write(effect.SourceColor, writer);
            builder.EndElement("a:clrFrom");

            builder.StartElement("a:clrTo");
            DmlColorWriter.Write(effect.DestinationColor, writer);
            builder.EndElement("a:clrTo");

            builder.EndElement("a:clrChange");
        }

        private static void WriteColorReplace(DmlColorReplaceEffect effect, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:clrRepl");
            DmlColorWriter.Write(effect.Color, writer);
            builder.EndElement("a:clrRepl");
        }

        private static void WriteDuotone(DmlDuotoneEffect effect, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:duotone");
            DmlColorWriter.Write(effect.Color1, writer);
            DmlColorWriter.Write(effect.Color2, writer);
            builder.EndElement("a:duotone");
        }

        internal static void WriteFillOverlay(DmlFillOverlayEffect effect, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:fillOverlay");
            builder.WriteAttribute("blend", DmlEnum.EffectBlendModeToDml(effect.Blend));
            DmlFillWriter.Write(effect.Fill, writer, false);
            builder.EndElement("a:fillOverlay");
        }

        private static void WriteGrayScale(IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.WriteEmptyElement("a:grayscl");
        }

        private static void WriteHueSaturationLuminance(DmlHueSaturationLuminanceEffect effect,
            IDmlShapeWriterContext writer)
        {
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:hsl");
            builder.WriteAttribute("hue", effect.Hue.Value);
            builder.WriteAttribute("lum", DmlPercentageUtil.ToPercentOrDmlPercent(effect.Luminance, isIsoStrict));
            builder.WriteAttribute("sat", DmlPercentageUtil.ToPercentOrDmlPercent(effect.Saturation, isIsoStrict));
            builder.EndElement("a:hsl");
        }

        private static void WriteLuminance(DmlLuminanceEffect effect, IDmlShapeWriterContext writer)
        {
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:lum");

            // Change contrast/bright ranges from (0, 1) to (-1, 1).
            double brightness = (effect.Brightness * 2) - 1;
            if (MathUtil.DoubleToInt(DmlPercentageUtil.ToDmlPercent(brightness)) != 0)
                builder.WriteAttribute("bright", DmlPercentageUtil.ToPercentOrDmlPercent(brightness, isIsoStrict));
            double contrast = (effect.Contrast * 2) - 1;
            if (MathUtil.DoubleToInt(DmlPercentageUtil.ToDmlPercent(contrast)) != 0)
                builder.WriteAttribute("contrast", DmlPercentageUtil.ToPercentOrDmlPercent(contrast, isIsoStrict));

            builder.EndElement("a:lum");
        }

        private static void WriteTint(DmlTintEffect effect, IDmlShapeWriterContext writer)
        {
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:tint");
            builder.WriteAttribute("amt", DmlPercentageUtil.ToPercentOrDmlPercent(effect.Amount, isIsoStrict));
            builder.WriteAttribute("hue", DmlPercentageUtil.ToPercentOrDmlPercent(effect.Hue, isIsoStrict));
            builder.EndElement("a:tint");
        }
    }
}
