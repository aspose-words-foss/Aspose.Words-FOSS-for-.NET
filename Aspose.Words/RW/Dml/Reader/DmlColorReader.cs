// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2011 by Alexey Titov

using System;
using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Represents a class building DmlColor objects from xml.
    /// </summary>
    internal class DmlColorReader : DmlReaderBase
    {
        private DmlColorReader(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            mReader = reader;
            mComplianceInfo = complianceInfo;
        }

        /// <summary>
        /// Reads CT_Color complex type value of the DrawingML element from the XML reader.
        /// Returns the color value of the latest child element in the current element.
        /// </summary>
        public static DmlColor ReadColor(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            string tagName = reader.LocalName;
            DmlColor result = null;
            while (reader.ReadChild(tagName))
            {
                switch (reader.LocalName)
                {
                    case "hslClr":
                    case "prstClr":
                    case "schemeClr":
                    case "scrgbClr":
                    case "srgbClr":
                    case "sysClr":
                        // If the property is already initialized
                        // then overwrite it. It gives a possibility to
                        // use color defined by the latest color tag.
                        DmlColor tmp = Read(reader, complianceInfo);
                        if (tmp != null)
                            result = tmp;
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// Builds a color by specified XML reader.
        /// </summary>
        public static DmlColor Read(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            DmlColorReader colorReader = new DmlColorReader(reader, complianceInfo);
            return colorReader.ReadCore();
        }

        private DmlColor ReadCore()
        {
            switch (mReader.LocalName)
            {
                case "hslClr":
                    return ReadHslColor();
                case "prstClr":
                    return ReadPresetColor();
                case "schemeClr":
                    return ReadSchemeColor();
                case "scrgbClr":
                    return ReadPercentageRgbColor();
                case "srgbClr":
                    return ReadHexRgbColor();
                case "sysClr":
                    return ReadSystemColor();
                default:
                    WarnUnexpectedAndIgnoreElement(mReader);
                    return null;
            }
        }

        private DmlColor ReadSystemColor()
        {
            DmlSystemColor color = new DmlSystemColor();
            color.Value = mReader.ReadAttribute(String.Empty);
            color.LastColor = mReader.ReadAttribute("lastClr", String.Empty);
            color.ColorModifiers = ReadModifiers();
            return color;
        }

        /// <summary>
        /// Reads color modifiers.
        /// </summary>
        internal static List<IDmlColorModifier> ReadModifiers(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            DmlColorReader colorReader = new DmlColorReader(reader, complianceInfo);
            return colorReader.ReadModifiers();
        }

        private List<IDmlColorModifier> ReadModifiers()
        {
            List<IDmlColorModifier> modifiers = new List<IDmlColorModifier>();
            string tagName = mReader.LocalName;
            while (mReader.ReadChild(tagName))
            {
                switch (mReader.LocalName)
                {
                    case "alpha":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlAlpha()));
                        break;
                    case "alphaMod":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlAlphaModulation()));
                        break;
                    case "alphaOff":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlAlphaOffset()));
                        break;
                    case "blue":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlBlue()));
                        break;
                    case "blueMod":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlBlueModulation()));
                        break;
                    case "blueOff":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlBlueOffset()));
                        break;
                    case "comp":
                        modifiers.Add(new DmlComplement());
                        break;
                    case "gamma":
                        modifiers.Add(new DmlGamma());
                        break;
                    case "gray":
                        modifiers.Add(new DmlGray());
                        break;
                    case "green":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlGreen()));
                        break;
                    case "greenMod":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlGreenModulation()));
                        break;
                    case "greenOff":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlGreenOffset()));
                        break;
                    case "hue":
                        ReadHue(modifiers);
                        break;
                    case "hueMod":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlHueModulation()));
                        break;
                    case "hueOff":
                        ReadHueOffset(modifiers);
                        break;
                    case "inv":
                        modifiers.Add(new DmlInverse());
                        break;
                    case "invGamma":
                        modifiers.Add(new DmlInverseGamma());
                        break;
                    case "lum":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlLuminance()));
                        break;
                    case "lumMod":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlLuminanceModulation()));
                        break;
                    case "lumOff":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlLuminanceOffset()));
                        break;
                    case "red":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlRed()));
                        break;
                    case "redMod":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlRedModulation()));
                        break;
                    case "redOff":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlRedOffset()));
                        break;
                    case "sat":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlSaturation()));
                        break;
                    case "satMod":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlSaturationModulation()));
                        break;
                    case "satOff":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlSaturationOffset()));
                        break;
                    case "shade":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlShade()));
                        break;
                    case "tint":
                        modifiers.Add(ReadPercentageBasedModifier(new DmlTint()));
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                }
            }
            return modifiers;
        }

        private DmlPercentageBasedColorModifier ReadPercentageBasedModifier(DmlPercentageBasedColorModifier modifier)
        {
            modifier.Value = DmlPercentageUtil.FromPercentOrDmlPercent(mReader.ReadAttribute(""), mComplianceInfo);
            return modifier;
        }

        private void ReadHueOffset(IList<IDmlColorModifier> colorModifiers)
        {
            DmlHueOffset modificator = new DmlHueOffset();
            modificator.Value = new DmlAngle(mReader.ReadDoubleAttribute(0));
            colorModifiers.Add(modificator);
        }

        private void ReadHue(IList<IDmlColorModifier> colorModifiers)
        {
            DmlHue modificator = new DmlHue();
            modificator.Value = new DmlAngle(mReader.ReadDoubleAttribute(0));
            colorModifiers.Add(modificator);
        }

        private DmlColor ReadHexRgbColor()
        {
            DmlHexRgbColor color = new DmlHexRgbColor();
            color.Value = mReader.ReadAttribute(String.Empty);
            color.ColorIndex = mReader.ReadAttribute("legacySpreadsheetColorIndex", null);
            color.ColorModifiers = ReadModifiers();
            return color;
        }

        private DmlColor ReadPercentageRgbColor()
        {
            DmlPercentageRgbColor color = new DmlPercentageRgbColor();
            color.R =
                DmlPercentageUtil.FromPercentOrDmlPercent(mReader.ReadAttribute("r", String.Empty), mComplianceInfo);
            color.G =
                DmlPercentageUtil.FromPercentOrDmlPercent(mReader.ReadAttribute("g", String.Empty), mComplianceInfo);
            color.B =
                DmlPercentageUtil.FromPercentOrDmlPercent(mReader.ReadAttribute("b", String.Empty), mComplianceInfo);
            color.ColorModifiers = ReadModifiers();
            return color;
        }

        private DmlColor ReadSchemeColor()
        {
            string value = mReader.ReadAttribute(String.Empty);
            if (value == "phClr")
            {
                // This color can appear in theme.
                DmlPlaceholderColor placeholderColor = new DmlPlaceholderColor();
                placeholderColor.ColorModifiers = ReadModifiers();
                return placeholderColor;
            }

            DmlSchemeColor color = new DmlSchemeColor();
            color.Value = ThemeColorConverter.FromString(value);
            color.ColorModifiers = ReadModifiers();
            return color;
        }

        private DmlColor ReadPresetColor()
        {
            DmlPresetColor color = new DmlPresetColor();
            color.Value = mReader.ReadAttribute(String.Empty);
            color.ColorModifiers = ReadModifiers();
            return color;
        }

        private DmlColor ReadHslColor()
        {
            DmlHslColor color = new DmlHslColor();
            color.Hue = new DmlAngle(mReader.ReadIntAttribute("hue", 0));
            color.Luminance =
                DmlPercentageUtil.FromPercentOrDmlPercent(mReader.ReadAttribute("lum", String.Empty), mComplianceInfo);
            color.Saturation =
                DmlPercentageUtil.FromPercentOrDmlPercent(mReader.ReadAttribute("sat", String.Empty), mComplianceInfo);
            color.ColorModifiers = ReadModifiers();
            return color;
        }

        private readonly NrxXmlReader mReader;
        private readonly OoxmlComplianceInfo mComplianceInfo;
    }
}
