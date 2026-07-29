// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/08/2007 by Vladimir Averkin

using System;
using Aspose.Collections;
using Aspose.Words.Drawing;
using Aspose.Words.Fonts;
using Aspose.Words.Framesets;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Tables;

namespace Aspose.Words.Styles
{
    /// <summary>
    /// Performs conversions between enumerated types MS Word XML strings that are common to both DOCX and WordML.
    /// </summary>
    internal class StyleConvertUtil
    {
        internal static FramesetBorderType NrxToFramesetBorderType(string value)
        {
            switch (value)
            {
                case "flatBorders": return FramesetBorderType.Simple;
                case "noBorder": return FramesetBorderType.None;
                default: return FramesetBorderType.Raised;
            }
        }

        internal static string FramesetBorderTypeToNrx(FramesetBorderType value)
        {
            switch (value)
            {
                case FramesetBorderType.Simple: return "flatBorders";
                case FramesetBorderType.None: return "noBorder";
                default: return "";
            }
        }

        internal static FrameLayoutType NrxToFrameLayoutType(string value)
        {
            switch (value)
            {
                case "cols": return FrameLayoutType.Horizontal;
                case "rows": return FrameLayoutType.Vertical;
                default: return FrameLayoutType.None;
            }
        }

        internal static string FrameLayoutTypeToNrx(FrameLayoutType value)
        {
            switch (value)
            {
                case FrameLayoutType.Horizontal: return "cols";
                case FrameLayoutType.Vertical: return "rows";
                default: return "";
            }
        }

        internal static TableStyleOverrideType XmlToTableStyleOverrideType(string value)
        {
            switch (value)
            {
                case "band1Horz": return TableStyleOverrideType.OddRowBanding;
                case "band1Vert": return TableStyleOverrideType.OddColumnBanding;
                case "band2Horz": return TableStyleOverrideType.EvenRowBanding;
                case "band2Vert": return TableStyleOverrideType.EvenColumnBanding;
                case "firstCol": return TableStyleOverrideType.FirstColumn;
                case "firstRow": return TableStyleOverrideType.FirstRow;
                case "lastCol": return TableStyleOverrideType.LastColumn;
                case "lastRow": return TableStyleOverrideType.LastRow;
                case "neCell": return TableStyleOverrideType.TopRightCell;
                case "nwCell": return TableStyleOverrideType.TopLeftCell;
                case "seCell": return TableStyleOverrideType.BottomRightCell;
                case "swCell": return TableStyleOverrideType.BottomLeftCell;
                case "wholeTable": return TableStyleOverrideType.WholeTable;
                default: return TableStyleOverrideType.None;
            }
        }

        internal static string TableStyleOverrideTypeToXml(TableStyleOverrideType value)
        {
            switch (value)
            {
                case TableStyleOverrideType.OddRowBanding: return "band1Horz";
                case TableStyleOverrideType.OddColumnBanding: return "band1Vert";
                case TableStyleOverrideType.EvenRowBanding: return "band2Horz";
                case TableStyleOverrideType.EvenColumnBanding: return "band2Vert";
                case TableStyleOverrideType.FirstColumn: return "firstCol";
                case TableStyleOverrideType.FirstRow: return "firstRow";
                case TableStyleOverrideType.LastColumn: return "lastCol";
                case TableStyleOverrideType.LastRow: return "lastRow";
                case TableStyleOverrideType.TopRightCell: return "neCell";
                case TableStyleOverrideType.TopLeftCell: return "nwCell";
                case TableStyleOverrideType.BottomRightCell: return "seCell";
                case TableStyleOverrideType.BottomLeftCell: return "swCell";
                case TableStyleOverrideType.WholeTable: return "wholeTable";
                default: return "wholeTable";
            }
        }

        internal static FontPitch XmlToFontPitch(string value)
        {
            return (FontPitch)gFontPitchMap.GetValue(value, (int)FontPitch.Default);
        }

        internal static string FontPitchToXml(FontPitch value)
        {
            return gFontPitchMap.GetValue((int)value, "");
        }

        internal static RelativeVerticalSize XmlToRelativeHeight(string value)
        {
            return (RelativeVerticalSize)gRelativeVerticalSizeMap.GetValue(value, (int)RelativeVerticalSize.Default);
        }

        internal static string RelativeHeightToXml(RelativeVerticalSize value)
        {
            return gRelativeVerticalSizeMap.GetValue((int)value, "");
        }

        internal static RelativeHorizontalSize XmlToRelativeWidth(string value)
        {
            return (RelativeHorizontalSize)gRelativeHorizontalSizeMap.GetValue(value, (int)RelativeHorizontalSize.Default);
        }

        internal static string RelativeWidthToXml(RelativeHorizontalSize value)
        {
            return gRelativeHorizontalSizeMap.GetValue((int)value, "");
        }

        internal static RelativeVerticalPosition XmlToRelativeVerticalPosition(string value)
        {
            return (RelativeVerticalPosition)gRelativeVerticalPositionMap.GetValue(value, (int)RelativeVerticalPosition.TableDefault);
        }

        internal static RelativeVerticalPosition XmlToRelativeVerticalPosition(string value, RelativeVerticalPosition defaultValue)
        {
            return (RelativeVerticalPosition)gRelativeVerticalPositionMap.GetValue(value, (int)defaultValue);
        }

        internal static string RelativeVerticalPositionToXml(RelativeVerticalPosition value)
        {
            return gRelativeVerticalPositionMap.GetValue((int)value, "");
        }

        internal static RelativeHorizontalPosition XmlToRelativeHorizontalPosition(string value)
        {
            return (RelativeHorizontalPosition)gRelativeHorizontalPositionMap.GetValue(value, (int)RelativeHorizontalPosition.Default);
        }

        internal static RelativeHorizontalPosition XmlToTableRelativeHorizontalPosition(string value)
        {
            // Mimics MS Word behavior which
            // sets relative horizontal position to "page" if "column" position is specified in XML.
            // 'column' cannot be added to the map as the same map is used for reading VML shape position
            // and there is no such substitution for VML.
            return value.Equals("column", StringComparison.Ordinal)
                ? RelativeHorizontalPosition.Page
                : XmlToRelativeHorizontalPosition(value);
        }

        internal static string RelativeHorizontalPositionToXml(RelativeHorizontalPosition value)
        {
            return gRelativeHorizontalPositionMap.GetValue((int)value, "");
        }

        internal static HorizontalAlignment XmlToHorizontalAlignment(string value)
        {
            return (HorizontalAlignment)gHorizontalAlignmentMap.GetValue(value, (int)HorizontalAlignment.None);
        }

        internal static string HorizontalAlignmentToXml(HorizontalAlignment value)
        {
            return gHorizontalAlignmentMap.GetValue((int)value, "");
        }

        internal static VerticalAlignment XmlToVerticalAlignment(string value)
        {
            return (VerticalAlignment)gVerticalAlignmentMap.GetValue(value, (int)VerticalAlignment.None);
        }

        internal static string VerticalAlignmentToXml(VerticalAlignment value)
        {
            return gVerticalAlignmentMap.GetValue((int)value, "");
        }

        /// <summary>
        /// See below comment for StyleIdentifierToDocx.
        /// </summary>
        internal static StyleIdentifier XmlToStyleIdentifier(string value)
        {
            return (StyleIdentifier)gStyleIdentifierMap.GetValue(value, (int)StyleIdentifier.User);
        }

        /// <summary>
        /// Converts a built-in style identifier into a "standard" MS Word locale independent
        /// style name that is written to DOCX, WordML and probably RTF.
        ///
        /// Note that this name is suitable for writing into a file, but not suitable
        /// for using in the model because some names are different in the model.
        /// For example, this method will return "heading 2" because that's what MS Word
        /// writes into the above file formats. But in the model and in DOC it is "Heading 2".
        /// To convert between DOCX etc names and model/DOC names use DocxToStyleName.
        /// </summary>
        internal static string StyleIdentifierToXml(StyleIdentifier styleIdentifier, string defaultValue)
        {
            return gStyleIdentifierMap.GetValue((int)styleIdentifier, defaultValue);
        }

        /// <summary>
        /// Converts a locale independent "standard" MS Word style name
        /// that was read from DOCX, WordML and probably RTF into a name that is suitable
        /// for use in the model/DOC.
        /// If cannot find a mapping, just returns the original name.
        /// See comments above.
        /// </summary>
        internal static string XmlToStyleName(string value)
        {
            return gStyleNameMap.GetValue(value, value);
        }

        /// <summary>
        /// Converts a "model" style name into a "file" style name (standard locale independent).
        /// If cannot find a mapping, just returns the original name.
        /// See comments above.
        /// </summary>
        internal static string StyleNameToXml(string value)
        {
            return gStyleNameMap.GetKey(value, value);
        }

        internal static ScreenSize XmlToScreenSize(string value)
        {
            return (ScreenSize)gScreenSizeMap.GetValue(value, (int)ScreenSize.Size800x600);
        }

        internal static string ScreenSizeToXml(ScreenSize value)
        {
            return gScreenSizeMap.GetValue((int)value, "");
        }

        internal static PreferredWidthType XmlToLengthType(string value)
        {
            // WORDSNET-26396 Ignore case for preferred width type value.
            switch (value.ToLower())
            {
                case "pct": return PreferredWidthType.Percent;
                case "dxa": return PreferredWidthType.Points;
                case "auto": return PreferredWidthType.Auto;
                case "nil": return (PreferredWidthType)0;
                default: return PreferredWidthType.Auto;
            }
        }

        internal static string LengthTypeToXml(PreferredWidthType value)
        {
            switch (value)
            {
                case PreferredWidthType.Percent: return "pct";
                case PreferredWidthType.Points: return "dxa";
                case PreferredWidthType.Auto: return "auto";
                default: return "";
            }
        }

        internal static TextureIndex XmlToTextureIndex(string value)
        {
            switch (value)
            {
                case "clear": return TextureIndex.TextureNone;
                case "nil": return TextureIndex.TextureNil;
                case "solid": return TextureIndex.TextureSolid;
                case "horzStripe":
                case "horz-stripe": return TextureIndex.TextureDarkHorizontal;
                case "vertStripe":
                case "vert-stripe": return TextureIndex.TextureDarkVertical;
                case "reverseDiagStripe":
                case "reverse-diag-stripe": return TextureIndex.TextureDarkDiagonalDown;
                case "diagStripe":
                case "diag-stripe": return TextureIndex.TextureDarkDiagonalUp;
                case "horzCross":
                case "horz-cross": return TextureIndex.TextureDarkCross;
                case "diagCross":
                case "diag-cross": return TextureIndex.TextureDarkDiagonalCross;
                case "thinHorzStripe":
                case "thin-horz-stripe": return TextureIndex.TextureHorizontal;
                case "thinVertStripe":
                case "thin-vert-stripe": return TextureIndex.TextureVertical;
                case "thinReverseDiagStripe":
                case "thin-reverse-diag-stripe": return TextureIndex.TextureDiagonalDown;
                case "thinDiagStripe":
                case "thin-diag-stripe": return TextureIndex.TextureDiagonalUp;
                case "thinHorzCross":
                case "thin-horz-cross": return TextureIndex.TextureCross;
                case "thinDiagCross":
                case "thin-diag-cross": return TextureIndex.TextureDiagonalCross;
                case "pct5":
                case "pct-5": return TextureIndex.Texture5Percent;
                case "pct10":
                case "pct-10": return TextureIndex.Texture10Percent;
                case "pct12":
                case "pct-12": return TextureIndex.Texture12Pt5Percent;
                case "pct15":
                case "pct-15": return TextureIndex.Texture15Percent;
                case "pct20":
                case "pct-20": return TextureIndex.Texture20Percent;
                case "pct25":
                case "pct-25": return TextureIndex.Texture25Percent;
                case "pct30":
                case "pct-30": return TextureIndex.Texture30Percent;
                case "pct35":
                case "pct-35": return TextureIndex.Texture35Percent;
                case "pct37":
                case "pct-37": return TextureIndex.Texture37Pt5Percent;
                case "pct40":
                case "pct-40": return TextureIndex.Texture40Percent;
                case "pct45":
                case "pct-45": return TextureIndex.Texture45Percent;
                case "pct50":
                case "pct-50": return TextureIndex.Texture50Percent;
                case "pct55":
                case "pct-55": return TextureIndex.Texture55Percent;
                case "pct60":
                case "pct-60": return TextureIndex.Texture60Percent;
                case "pct62":
                case "pct-62": return TextureIndex.Texture62Pt5Percent;
                case "pct65":
                case "pct-65": return TextureIndex.Texture65Percent;
                case "pct70":
                case "pct-70": return TextureIndex.Texture70Percent;
                case "pct75":
                case "pct-75": return TextureIndex.Texture75Percent;
                case "pct80":
                case "pct-80": return TextureIndex.Texture80Percent;
                case "pct85":
                case "pct-85": return TextureIndex.Texture85Percent;
                case "pct87":
                case "pct-87": return TextureIndex.Texture87Pt5Percent;
                case "pct90":
                case "pct-90": return TextureIndex.Texture90Percent;
                case "pct95":
                case "pct-95": return TextureIndex.Texture95Percent;
                default: return TextureIndex.TextureNone;
            }
        }

        internal static string TextureIndexToXml(TextureIndex value, bool isDocx)
        {
            switch (value)
            {
                case TextureIndex.TextureNone: return "clear";
                case TextureIndex.TextureNil: return "nil";
                case TextureIndex.TextureSolid: return "solid";
                case TextureIndex.TextureDarkHorizontal: return (isDocx) ? "horzStripe" : "horz-stripe";
                case TextureIndex.TextureDarkVertical: return (isDocx) ? "vertStripe" : "vert-stripe";
                case TextureIndex.TextureDarkDiagonalDown: return (isDocx) ? "reverseDiagStripe" : "reverse-diag-stripe";
                case TextureIndex.TextureDarkDiagonalUp: return (isDocx) ? "diagStripe" : "diag-stripe";
                case TextureIndex.TextureDarkCross: return (isDocx) ? "horzCross" : "horz-cross";
                case TextureIndex.TextureDarkDiagonalCross: return (isDocx) ? "diagCross" : "diag-cross";
                case TextureIndex.TextureHorizontal: return (isDocx) ? "thinHorzStripe" : "thin-horz-stripe";
                case TextureIndex.TextureVertical: return (isDocx) ? "thinVertStripe" : "thin-vert-stripe";
                case TextureIndex.TextureDiagonalDown: return (isDocx) ? "thinReverseDiagStripe" : "thin-reverse-diag-stripe";
                case TextureIndex.TextureDiagonalUp: return (isDocx) ? "thinDiagStripe" : "thin-diag-stripe";
                case TextureIndex.TextureCross: return (isDocx) ? "thinHorzCross" : "thin-horz-cross";
                case TextureIndex.TextureDiagonalCross: return (isDocx) ? "thinDiagCross" : "thin-diag-cross";
                case TextureIndex.Texture5Percent: return (isDocx) ? "pct5" : "pct-5";
                case TextureIndex.Texture10Percent: return (isDocx) ? "pct10" : "pct-10";
                case TextureIndex.Texture12Pt5Percent: return (isDocx) ? "pct12" : "pct-12";
                case TextureIndex.Texture15Percent: return (isDocx) ? "pct15" : "pct-15";
                case TextureIndex.Texture20Percent: return (isDocx) ? "pct20" : "pct-20";
                case TextureIndex.Texture25Percent: return (isDocx) ? "pct25" : "pct-25";
                case TextureIndex.Texture30Percent: return (isDocx) ? "pct30" : "pct-30";
                case TextureIndex.Texture35Percent: return (isDocx) ? "pct35" : "pct-35";
                case TextureIndex.Texture37Pt5Percent: return (isDocx) ? "pct37" : "pct-37";
                case TextureIndex.Texture40Percent: return (isDocx) ? "pct40" : "pct-40";
                case TextureIndex.Texture45Percent: return (isDocx) ? "pct45" : "pct-45";
                case TextureIndex.Texture50Percent: return (isDocx) ? "pct50" : "pct-50";
                case TextureIndex.Texture55Percent: return (isDocx) ? "pct55" : "pct-55";
                case TextureIndex.Texture60Percent: return (isDocx) ? "pct60" : "pct-60";
                case TextureIndex.Texture62Pt5Percent: return (isDocx) ? "pct62" : "pct-62";
                case TextureIndex.Texture65Percent: return (isDocx) ? "pct65" : "pct-65";
                case TextureIndex.Texture70Percent: return (isDocx) ? "pct70" : "pct-70";
                case TextureIndex.Texture75Percent: return (isDocx) ? "pct75" : "pct-75";
                case TextureIndex.Texture80Percent: return (isDocx) ? "pct80" : "pct-80";
                case TextureIndex.Texture85Percent: return (isDocx) ? "pct85" : "pct-85";
                case TextureIndex.Texture87Pt5Percent: return (isDocx) ? "pct87" : "pct-87";
                case TextureIndex.Texture90Percent: return (isDocx) ? "pct90" : "pct-90";
                case TextureIndex.Texture95Percent: return (isDocx) ? "pct95" : "pct-95";

                // Some intermediate settings that exist in DOC do not exist in Office Open XML.
                // We round them to existing settings.
                case TextureIndex.Texture2Pt5Percent: return (isDocx) ? "pct5" : "pct-5";
                case TextureIndex.Texture7Pt5Percent: return (isDocx) ? "pct10" : "pct-10";
                case TextureIndex.Texture17Pt5Percent: return (isDocx) ? "pct20" : "pct-20";
                case TextureIndex.Texture22Pt5Percent: return (isDocx) ? "pct25" : "pct-25";
                case TextureIndex.Texture27Pt5Percent: return (isDocx) ? "pct30" : "pct-30";
                case TextureIndex.Texture32Pt5Percent: return (isDocx) ? "pct35" : "pct-35";
                case TextureIndex.Texture42Pt5Percent: return (isDocx) ? "pct40" : "pct-40";
                case TextureIndex.Texture47Pt5Percent: return (isDocx) ? "pct45" : "pct-45";
                case TextureIndex.Texture52Pt5Percent: return (isDocx) ? "pct50" : "pct-50";
                case TextureIndex.Texture57Pt5Percent: return (isDocx) ? "pct55" : "pct-55";
                case TextureIndex.Texture67Pt5Percent: return (isDocx) ? "pct65" : "pct-65";
                case TextureIndex.Texture72Pt5Percent: return (isDocx) ? "pct70" : "pct-70";
                case TextureIndex.Texture77Pt5Percent: return (isDocx) ? "pct80" : "pct-80";
                case TextureIndex.Texture82Pt5Percent: return (isDocx) ? "pct85" : "pct-85";
                case TextureIndex.Texture92Pt5Percent: return (isDocx) ? "pct90" : "pct-90";
                case TextureIndex.Texture97Pt5Percent: return (isDocx) ? "pct95" : "pct-95";

                default: return "nil";
            }
        }

        internal static TextOrientation XmlToTextOrientation(string value, OoxmlComplianceInfo cInfo)
        {
            switch (value)
            {
                case "lr":   // iso29500
                    {
                        cInfo.MarkAsIsoTransitional();
                        return TextOrientation.Upward;
                    }
                case "btLr":
                case "bt-lr":
                    return TextOrientation.Upward;
                case "tb":  // iso29500
                    {
                        cInfo.MarkAsIsoTransitional();
                        return TextOrientation.Horizontal;
                    }
                case "lrTb":
                case "lr-tb":
                    return TextOrientation.Horizontal;
                case "tbV": // iso29500
                    {
                        cInfo.MarkAsIsoTransitional();
                        return TextOrientation.HorizontalRotatedFarEast;
                    }
                case "lrTbV":
                case "lr-tb-v":
                    return TextOrientation.HorizontalRotatedFarEast;
                case "rlV": // iso29500
                    {
                        cInfo.MarkAsIsoTransitional();
                        return TextOrientation.VerticalFarEast;
                    }
                case "tbRlV":
                case "tb-rl-v":
                    return TextOrientation.VerticalFarEast;
                case "rl":  // iso29500
                    {
                        cInfo.MarkAsIsoTransitional();
                        return TextOrientation.Downward;
                    }
                case "tbRl":
                case "tb-rl":
                    return TextOrientation.Downward;
                case "tb-lr-v":
                case "tbLrV":
                    return TextOrientation.VerticalRotatedFarEast;
                case "lrV":  // iso29500
                    {
                        cInfo.MarkAsIsoTransitional();
                        return TextOrientation.VerticalRotatedFarEast;
                    }
                default:
                    return TextOrientation.Horizontal;
            }
        }

        internal static string TextOrientationToXml(TextOrientation value, bool isDocx, OoxmlComplianceCore compliance)
        {
            bool isIsoStrict = isDocx && (compliance == OoxmlComplianceCore.IsoStrict);

            switch (value)
            {
                case TextOrientation.Upward:
                    return (isDocx) ? ((isIsoStrict) ? "lr" : "btLr") : "bt-lr";
                case TextOrientation.Horizontal:
                    return (isDocx) ? ((isIsoStrict) ? "tb" : "lrTb") : "lr-tb";
                case TextOrientation.HorizontalRotatedFarEast:
                    return (isDocx) ? ((isIsoStrict) ? "tbV" : "lrTbV") : "lr-tb-v";
                case TextOrientation.VerticalFarEast:
                    return (isDocx) ? ((isIsoStrict) ? "rlV" : "tbRlV") : "tb-rl-v";
                case TextOrientation.Downward:
                    return (isDocx) ? ((isIsoStrict) ? "rl" : "tbRl") : "tb-rl";
                case TextOrientation.VerticalRotatedFarEast:
                    return (isDocx) ? ((isIsoStrict) ? "lrV" : "tbLrV") : "tb-lr-v";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Converts int values into <see cref="StylePaneSortMethod"/>.
        /// Is used for docx as well as for rtf. See RK comments and constants in DocPr.
        /// </summary>
        internal static StylePaneSortMethod IntToStylePaneSortMethod(int value)
        {
            switch (value)
            {
                case DocPr.StylePaneSortMethodNames:
                    return StylePaneSortMethod.Name;
                case DocPr.StylePaneSortMethodDefault:
                    return StylePaneSortMethod.Default;
                case DocPr.StylePaneSortMethodFont:
                    return StylePaneSortMethod.Font;
                case DocPr.StylePaneSortMethodStyle:
                    return StylePaneSortMethod.BasedOn;
                case DocPr.StylePaneSortMethodType:
                    return StylePaneSortMethod.StyleType;
                default:
                    return StylePaneSortMethod.Default;
            }
        }

        /// <summary>
        /// Converts <see cref="StylePaneSortMethod"/> values in int.
        /// Is used for docx as well as for rtf. See RK comments and constants in DocPr.
        /// </summary>
        internal static int StylePaneSortMethodToInt(StylePaneSortMethod value)
        {
            switch (value)
            {
                case StylePaneSortMethod.Name:
                    return DocPr.StylePaneSortMethodNames;
                case StylePaneSortMethod.Font:
                    return DocPr.StylePaneSortMethodFont;
                case StylePaneSortMethod.BasedOn:
                    return DocPr.StylePaneSortMethodStyle;
                case StylePaneSortMethod.StyleType:
                    return DocPr.StylePaneSortMethodType;
                case StylePaneSortMethod.Default:
                case StylePaneSortMethod.Priority:
                default:
                    return DocPr.StylePaneSortMethodDefault;
            }
        }

        private static readonly StringToIntBidirectionalMap gFontPitchMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gRelativeVerticalSizeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gRelativeHorizontalSizeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gRelativeVerticalPositionMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gRelativeHorizontalPositionMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gHorizontalAlignmentMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gVerticalAlignmentMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gStyleIdentifierMap = new StringToIntBidirectionalMap();
        private static readonly StringToObjBidirectionalMap<string> gStyleNameMap = new StringToObjBidirectionalMap<string>();
        private static readonly StringToIntBidirectionalMap gScreenSizeMap = new StringToIntBidirectionalMap();
        static StyleConvertUtil()
        {
            gFontPitchMap.AddEntry("default", (int)FontPitch.Default);
            gFontPitchMap.AddEntry("fixed", (int)FontPitch.Fixed);
            gFontPitchMap.AddEntry("variable", (int)FontPitch.Variable);

            gRelativeVerticalSizeMap.AddEntry("margin", (int)RelativeVerticalSize.Margin);
            gRelativeVerticalSizeMap.AddEntry("page", (int)RelativeVerticalSize.Page);
            gRelativeVerticalSizeMap.AddEntry("outer-margin-area", (int)RelativeVerticalSize.OuterMargin);
            gRelativeVerticalSizeMap.AddEntry("inner-margin-area", (int)RelativeVerticalSize.InnerMargin);
            gRelativeVerticalSizeMap.AddEntry("top-margin-area", (int)RelativeVerticalSize.TopMargin);
            gRelativeVerticalSizeMap.AddEntry("bottom-margin-area", (int)RelativeVerticalSize.BottomMargin);

            gRelativeHorizontalSizeMap.AddEntry("margin", (int)RelativeHorizontalSize.Margin);
            gRelativeHorizontalSizeMap.AddEntry("page", (int)RelativeHorizontalSize.Page);
            gRelativeHorizontalSizeMap.AddEntry("outer-margin-area", (int)RelativeHorizontalSize.OuterMargin);
            gRelativeHorizontalSizeMap.AddEntry("inner-margin-area", (int)RelativeHorizontalSize.InnerMargin);
            gRelativeHorizontalSizeMap.AddEntry("left-margin-area", (int)RelativeHorizontalSize.LeftMargin);
            gRelativeHorizontalSizeMap.AddEntry("right-margin-area", (int)RelativeHorizontalSize.RightMargin);

            gRelativeVerticalPositionMap.AddEntry("margin", (int)RelativeVerticalPosition.Margin);
            gRelativeVerticalPositionMap.AddEntry("page", (int)RelativeVerticalPosition.Page);
            gRelativeVerticalPositionMap.AddEntry("text", (int)RelativeVerticalPosition.Paragraph);
            gRelativeVerticalPositionMap.AddEntry("line", (int)RelativeVerticalPosition.Line);
            gRelativeVerticalPositionMap.AddEntry("outer-margin-area", (int)RelativeVerticalPosition.OutsideMargin);
            gRelativeVerticalPositionMap.AddEntry("inner-margin-area", (int)RelativeVerticalPosition.InsideMargin);
            gRelativeVerticalPositionMap.AddEntry("top-margin-area", (int)RelativeVerticalPosition.TopMargin);
            gRelativeVerticalPositionMap.AddEntry("bottom-margin-area", (int)RelativeVerticalPosition.BottomMargin);

            gRelativeHorizontalPositionMap.AddEntry("margin", (int)RelativeHorizontalPosition.Margin);
            gRelativeHorizontalPositionMap.AddEntry("page", (int)RelativeHorizontalPosition.Page);
            gRelativeHorizontalPositionMap.AddEntry("text", (int)RelativeHorizontalPosition.Column);
            gRelativeHorizontalPositionMap.AddEntry("char", (int)RelativeHorizontalPosition.Character);
            gRelativeHorizontalPositionMap.AddEntry("outer-margin-area", (int)RelativeHorizontalPosition.OutsideMargin);
            gRelativeHorizontalPositionMap.AddEntry("inner-margin-area", (int)RelativeHorizontalPosition.InsideMargin);
            gRelativeHorizontalPositionMap.AddEntry("left-margin-area", (int)RelativeHorizontalPosition.LeftMargin);
            gRelativeHorizontalPositionMap.AddEntry("right-margin-area", (int)RelativeHorizontalPosition.RightMargin);

            gHorizontalAlignmentMap.AddEntry("absolute", (int)HorizontalAlignment.None);
            gHorizontalAlignmentMap.AddEntry("left", (int)HorizontalAlignment.Left);
            gHorizontalAlignmentMap.AddEntry("center", (int)HorizontalAlignment.Center);
            gHorizontalAlignmentMap.AddEntry("right", (int)HorizontalAlignment.Right);
            gHorizontalAlignmentMap.AddEntry("inside", (int)HorizontalAlignment.Inside);
            gHorizontalAlignmentMap.AddEntry("outside", (int)HorizontalAlignment.Outside);

            gVerticalAlignmentMap.AddEntry("absolute", (int)VerticalAlignment.None);
            gVerticalAlignmentMap.AddEntry("top", (int)VerticalAlignment.Top);
            gVerticalAlignmentMap.AddEntry("center", (int)VerticalAlignment.Center);
            gVerticalAlignmentMap.AddEntry("bottom", (int)VerticalAlignment.Bottom);
            gVerticalAlignmentMap.AddEntry("inside", (int)VerticalAlignment.Inside);
            gVerticalAlignmentMap.AddEntry("outside", (int)VerticalAlignment.Outside);

            // This defines all built-in styles as per MS Word 2007.
            // Style names here are as they appear in the file.

            // Character styles.
            gStyleIdentifierMap.AddEntry("Book Title", (int)StyleIdentifier.BookTitle);
            gStyleIdentifierMap.AddEntry("annotation reference", (int)StyleIdentifier.CommentReference);
            gStyleIdentifierMap.AddEntry("Default Paragraph Font", (int)StyleIdentifier.DefaultParagraphFont);
            gStyleIdentifierMap.AddEntry("Emphasis", (int)StyleIdentifier.Emphasis);
            gStyleIdentifierMap.AddEntry("endnote reference", (int)StyleIdentifier.EndnoteReference);
            gStyleIdentifierMap.AddEntry("FollowedHyperlink", (int)StyleIdentifier.FollowedHyperlink);
            gStyleIdentifierMap.AddEntry("footnote reference", (int)StyleIdentifier.FootnoteReference);
            gStyleIdentifierMap.AddEntry("HTML Acronym", (int)StyleIdentifier.HtmlAcronym);
            gStyleIdentifierMap.AddEntry("HTML Cite", (int)StyleIdentifier.HtmlCite);
            gStyleIdentifierMap.AddEntry("HTML Code", (int)StyleIdentifier.HtmlCode);
            gStyleIdentifierMap.AddEntry("HTML Definition", (int)StyleIdentifier.HtmlDefinition);
            gStyleIdentifierMap.AddEntry("HTML Keyboard", (int)StyleIdentifier.HtmlKeyboard);
            gStyleIdentifierMap.AddEntry("HTML Sample", (int)StyleIdentifier.HtmlSample);
            gStyleIdentifierMap.AddEntry("HTML Typewriter", (int)StyleIdentifier.HtmlTypewriter);
            gStyleIdentifierMap.AddEntry("HTML Variable", (int)StyleIdentifier.HtmlVariable);
            gStyleIdentifierMap.AddEntry("Hyperlink", (int)StyleIdentifier.Hyperlink);
            gStyleIdentifierMap.AddEntry("Intense Emphasis", (int)StyleIdentifier.IntenseEmphasis);
            gStyleIdentifierMap.AddEntry("Intense Reference", (int)StyleIdentifier.IntenseReference);
            gStyleIdentifierMap.AddEntry("line number", (int)StyleIdentifier.LineNumber);
            gStyleIdentifierMap.AddEntry("page number", (int)StyleIdentifier.PageNumber);
            gStyleIdentifierMap.AddEntry("Placeholder Text", (int)StyleIdentifier.PlaceholderText);
            // WORDSNET-21757 Added style used in MS Word 2019, Office 365.
            gStyleIdentifierMap.AddEntry("Smart Link", (int)StyleIdentifier.SmartLink);
            gStyleIdentifierMap.AddEntry("Strong", (int)StyleIdentifier.Strong);
            gStyleIdentifierMap.AddEntry("Subtle Emphasis", (int)StyleIdentifier.SubtleEmphasis);
            gStyleIdentifierMap.AddEntry("Subtle Reference", (int)StyleIdentifier.SubtleReference);

            // Paragraph styles that have linked character styles.
            // RK I'm not really doing anything about linked styles yet.
            gStyleIdentifierMap.AddEntry("Balloon Text", (int)StyleIdentifier.BalloonText);
            gStyleIdentifierMap.AddEntry("Body Text", (int)StyleIdentifier.BodyText);
            gStyleIdentifierMap.AddEntry("Body Text 2", (int)StyleIdentifier.BodyText2);
            gStyleIdentifierMap.AddEntry("Body Text 3", (int)StyleIdentifier.BodyText3);
            gStyleIdentifierMap.AddEntry("Body Text First Indent", (int)StyleIdentifier.BodyText1I);
            gStyleIdentifierMap.AddEntry("Body Text First Indent 2", (int)StyleIdentifier.BodyText1I2);
            gStyleIdentifierMap.AddEntry("Body Text Indent", (int)StyleIdentifier.BodyTextInd);
            gStyleIdentifierMap.AddEntry("Body Text Indent 2", (int)StyleIdentifier.BodyTextInd2);
            gStyleIdentifierMap.AddEntry("Body Text Indent 3", (int)StyleIdentifier.BodyTextInd3);
            gStyleIdentifierMap.AddEntry("Closing", (int)StyleIdentifier.Closing);
            gStyleIdentifierMap.AddEntry("annotation subject", (int)StyleIdentifier.CommentSubject);
            gStyleIdentifierMap.AddEntry("annotation text", (int)StyleIdentifier.CommentText);
            gStyleIdentifierMap.AddEntry("Date", (int)StyleIdentifier.Date);
            gStyleIdentifierMap.AddEntry("Document Map", (int)StyleIdentifier.DocumentMap);
            gStyleIdentifierMap.AddEntry("E-mail Signature", (int)StyleIdentifier.EmailSignature);
            gStyleIdentifierMap.AddEntry("endnote text", (int)StyleIdentifier.EndnoteText);
            gStyleIdentifierMap.AddEntry("footer", (int)StyleIdentifier.Footer);
            gStyleIdentifierMap.AddEntry("footnote text", (int)StyleIdentifier.FootnoteText);
            gStyleIdentifierMap.AddEntry("header", (int)StyleIdentifier.Header);
            gStyleIdentifierMap.AddEntry("heading 1", (int)StyleIdentifier.Heading1);
            gStyleIdentifierMap.AddEntry("heading 2", (int)StyleIdentifier.Heading2);
            gStyleIdentifierMap.AddEntry("heading 3", (int)StyleIdentifier.Heading3);
            gStyleIdentifierMap.AddEntry("heading 4", (int)StyleIdentifier.Heading4);
            gStyleIdentifierMap.AddEntry("heading 5", (int)StyleIdentifier.Heading5);
            gStyleIdentifierMap.AddEntry("heading 6", (int)StyleIdentifier.Heading6);
            gStyleIdentifierMap.AddEntry("heading 7", (int)StyleIdentifier.Heading7);
            gStyleIdentifierMap.AddEntry("heading 8", (int)StyleIdentifier.Heading8);
            gStyleIdentifierMap.AddEntry("heading 9", (int)StyleIdentifier.Heading9);
            gStyleIdentifierMap.AddEntry("HTML Address", (int)StyleIdentifier.HtmlAddress);
            gStyleIdentifierMap.AddEntry("HTML Bottom of Form", (int)StyleIdentifier.HtmlBottomOfForm);
            gStyleIdentifierMap.AddEntry("HTML Preformatted", (int)StyleIdentifier.HtmlPreformatted);
            gStyleIdentifierMap.AddEntry("HTML Top of Form", (int)StyleIdentifier.HtmlTopOfForm);
            gStyleIdentifierMap.AddEntry("Intense Quote", (int)StyleIdentifier.IntenseQuote);
            gStyleIdentifierMap.AddEntry("macro", (int)StyleIdentifier.Macro);
            gStyleIdentifierMap.AddEntry("Message Header", (int)StyleIdentifier.MessageHeader);
            gStyleIdentifierMap.AddEntry("Note Heading", (int)StyleIdentifier.NoteHeading);
            gStyleIdentifierMap.AddEntry("Plain Text", (int)StyleIdentifier.PlainText);
            gStyleIdentifierMap.AddEntry("Quote", (int)StyleIdentifier.Quote);
            gStyleIdentifierMap.AddEntry("Salutation", (int)StyleIdentifier.Salutation);
            gStyleIdentifierMap.AddEntry("Signature", (int)StyleIdentifier.Signature);
            gStyleIdentifierMap.AddEntry("Subtitle", (int)StyleIdentifier.Subtitle);
            gStyleIdentifierMap.AddEntry("Title", (int)StyleIdentifier.Title);

            // Paragraph styles.
            gStyleIdentifierMap.AddEntry("Bibliography", (int)StyleIdentifier.Bibliography);
            gStyleIdentifierMap.AddEntry("Block Text", (int)StyleIdentifier.BlockText);
            gStyleIdentifierMap.AddEntry("caption", (int)StyleIdentifier.Caption);
            gStyleIdentifierMap.AddEntry("envelope address", (int)StyleIdentifier.EnvelopeAddress);
            gStyleIdentifierMap.AddEntry("envelope return", (int)StyleIdentifier.EnvelopeReturn);
            gStyleIdentifierMap.AddEntry("index 1", (int)StyleIdentifier.Index1);
            gStyleIdentifierMap.AddEntry("index 2", (int)StyleIdentifier.Index2);
            gStyleIdentifierMap.AddEntry("index 3", (int)StyleIdentifier.Index3);
            gStyleIdentifierMap.AddEntry("index 4", (int)StyleIdentifier.Index4);
            gStyleIdentifierMap.AddEntry("index 5", (int)StyleIdentifier.Index5);
            gStyleIdentifierMap.AddEntry("index 6", (int)StyleIdentifier.Index6);
            gStyleIdentifierMap.AddEntry("index 7", (int)StyleIdentifier.Index7);
            gStyleIdentifierMap.AddEntry("index 8", (int)StyleIdentifier.Index8);
            gStyleIdentifierMap.AddEntry("index 9", (int)StyleIdentifier.Index9);
            gStyleIdentifierMap.AddEntry("index heading", (int)StyleIdentifier.IndexHeading);
            gStyleIdentifierMap.AddEntry("List", (int)StyleIdentifier.List);
            gStyleIdentifierMap.AddEntry("List 2", (int)StyleIdentifier.List2);
            gStyleIdentifierMap.AddEntry("List 3", (int)StyleIdentifier.List3);
            gStyleIdentifierMap.AddEntry("List 4", (int)StyleIdentifier.List4);
            gStyleIdentifierMap.AddEntry("List 5", (int)StyleIdentifier.List5);
            gStyleIdentifierMap.AddEntry("List Bullet", (int)StyleIdentifier.ListBullet);
            gStyleIdentifierMap.AddEntry("List Bullet 2", (int)StyleIdentifier.ListBullet2);
            gStyleIdentifierMap.AddEntry("List Bullet 3", (int)StyleIdentifier.ListBullet3);
            gStyleIdentifierMap.AddEntry("List Bullet 4", (int)StyleIdentifier.ListBullet4);
            gStyleIdentifierMap.AddEntry("List Bullet 5", (int)StyleIdentifier.ListBullet5);
            gStyleIdentifierMap.AddEntry("List Continue", (int)StyleIdentifier.ListContinue);
            gStyleIdentifierMap.AddEntry("List Continue 2", (int)StyleIdentifier.ListContinue2);
            gStyleIdentifierMap.AddEntry("List Continue 3", (int)StyleIdentifier.ListContinue3);
            gStyleIdentifierMap.AddEntry("List Continue 4", (int)StyleIdentifier.ListContinue4);
            gStyleIdentifierMap.AddEntry("List Continue 5", (int)StyleIdentifier.ListContinue5);
            gStyleIdentifierMap.AddEntry("List Number", (int)StyleIdentifier.ListNumber);
            gStyleIdentifierMap.AddEntry("List Number 2", (int)StyleIdentifier.ListNumber2);
            gStyleIdentifierMap.AddEntry("List Number 3", (int)StyleIdentifier.ListNumber3);
            gStyleIdentifierMap.AddEntry("List Number 4", (int)StyleIdentifier.ListNumber4);
            gStyleIdentifierMap.AddEntry("List Number 5", (int)StyleIdentifier.ListNumber5);
            gStyleIdentifierMap.AddEntry("List Paragraph", (int)StyleIdentifier.ListParagraph);
            gStyleIdentifierMap.AddEntry("No Spacing", (int)StyleIdentifier.NoSpacing);
            gStyleIdentifierMap.AddEntry("Normal", (int)StyleIdentifier.Normal);
            gStyleIdentifierMap.AddEntry("Normal (Web)", (int)StyleIdentifier.NormalWeb);
            gStyleIdentifierMap.AddEntry("Normal Indent", (int)StyleIdentifier.NormalIndent);
            gStyleIdentifierMap.AddEntry("Revision", (int)StyleIdentifier.Revision);
            gStyleIdentifierMap.AddEntry("table of authorities", (int)StyleIdentifier.TableOfAuthorities);
            gStyleIdentifierMap.AddEntry("table of figures", (int)StyleIdentifier.TableOfFigures);
            gStyleIdentifierMap.AddEntry("toa heading", (int)StyleIdentifier.ToaHeading);
            gStyleIdentifierMap.AddEntry("toc 1", (int)StyleIdentifier.Toc1);
            gStyleIdentifierMap.AddEntry("toc 2", (int)StyleIdentifier.Toc2);
            gStyleIdentifierMap.AddEntry("toc 3", (int)StyleIdentifier.Toc3);
            gStyleIdentifierMap.AddEntry("toc 4", (int)StyleIdentifier.Toc4);
            gStyleIdentifierMap.AddEntry("toc 5", (int)StyleIdentifier.Toc5);
            gStyleIdentifierMap.AddEntry("toc 6", (int)StyleIdentifier.Toc6);
            gStyleIdentifierMap.AddEntry("toc 7", (int)StyleIdentifier.Toc7);
            gStyleIdentifierMap.AddEntry("toc 8", (int)StyleIdentifier.Toc8);
            gStyleIdentifierMap.AddEntry("toc 9", (int)StyleIdentifier.Toc9);
            gStyleIdentifierMap.AddEntry("TOC Heading", (int)StyleIdentifier.TocHeading);

            // List styles.
            gStyleIdentifierMap.AddEntry("Outline List 1", (int)StyleIdentifier.OutlineList1);
            gStyleIdentifierMap.AddEntry("Outline List 2", (int)StyleIdentifier.OutlineList2);
            gStyleIdentifierMap.AddEntry("Outline List 3", (int)StyleIdentifier.OutlineList3);
            gStyleIdentifierMap.AddEntry("No List", (int)StyleIdentifier.NoList);

            // Table styles.
            gStyleIdentifierMap.AddEntry("Colorful Grid", (int)StyleIdentifier.ColorfulGrid);
            gStyleIdentifierMap.AddEntry("Colorful Grid Accent 1", (int)StyleIdentifier.ColorfulGridAccent1);
            gStyleIdentifierMap.AddEntry("Colorful Grid Accent 2", (int)StyleIdentifier.ColorfulGridAccent2);
            gStyleIdentifierMap.AddEntry("Colorful Grid Accent 3", (int)StyleIdentifier.ColorfulGridAccent3);
            gStyleIdentifierMap.AddEntry("Colorful Grid Accent 4", (int)StyleIdentifier.ColorfulGridAccent4);
            gStyleIdentifierMap.AddEntry("Colorful Grid Accent 5", (int)StyleIdentifier.ColorfulGridAccent5);
            gStyleIdentifierMap.AddEntry("Colorful Grid Accent 6", (int)StyleIdentifier.ColorfulGridAccent6);
            gStyleIdentifierMap.AddEntry("Colorful List", (int)StyleIdentifier.ColorfulList);
            gStyleIdentifierMap.AddEntry("Colorful List Accent 1", (int)StyleIdentifier.ColorfulListAccent1);
            gStyleIdentifierMap.AddEntry("Colorful List Accent 2", (int)StyleIdentifier.ColorfulListAccent2);
            gStyleIdentifierMap.AddEntry("Colorful List Accent 3", (int)StyleIdentifier.ColorfulListAccent3);
            gStyleIdentifierMap.AddEntry("Colorful List Accent 4", (int)StyleIdentifier.ColorfulListAccent4);
            gStyleIdentifierMap.AddEntry("Colorful List Accent 5", (int)StyleIdentifier.ColorfulListAccent5);
            gStyleIdentifierMap.AddEntry("Colorful List Accent 6", (int)StyleIdentifier.ColorfulListAccent6);
            gStyleIdentifierMap.AddEntry("Colorful Shading", (int)StyleIdentifier.ColorfulShading);
            gStyleIdentifierMap.AddEntry("Colorful Shading Accent 1", (int)StyleIdentifier.ColorfulShadingAccent1);
            gStyleIdentifierMap.AddEntry("Colorful Shading Accent 2", (int)StyleIdentifier.ColorfulShadingAccent2);
            gStyleIdentifierMap.AddEntry("Colorful Shading Accent 3", (int)StyleIdentifier.ColorfulShadingAccent3);
            gStyleIdentifierMap.AddEntry("Colorful Shading Accent 4", (int)StyleIdentifier.ColorfulShadingAccent4);
            gStyleIdentifierMap.AddEntry("Colorful Shading Accent 5", (int)StyleIdentifier.ColorfulShadingAccent5);
            gStyleIdentifierMap.AddEntry("Colorful Shading Accent 6", (int)StyleIdentifier.ColorfulShadingAccent6);
            gStyleIdentifierMap.AddEntry("Dark List", (int)StyleIdentifier.DarkList);
            gStyleIdentifierMap.AddEntry("Dark List Accent 1", (int)StyleIdentifier.DarkListAccent1);
            gStyleIdentifierMap.AddEntry("Dark List Accent 2", (int)StyleIdentifier.DarkListAccent2);
            gStyleIdentifierMap.AddEntry("Dark List Accent 3", (int)StyleIdentifier.DarkListAccent3);
            gStyleIdentifierMap.AddEntry("Dark List Accent 4", (int)StyleIdentifier.DarkListAccent4);
            gStyleIdentifierMap.AddEntry("Dark List Accent 5", (int)StyleIdentifier.DarkListAccent5);
            gStyleIdentifierMap.AddEntry("Dark List Accent 6", (int)StyleIdentifier.DarkListAccent6);
            gStyleIdentifierMap.AddEntry("Light Grid", (int)StyleIdentifier.LightGrid);
            gStyleIdentifierMap.AddEntry("Light Grid Accent 1", (int)StyleIdentifier.LightGridAccent1);
            gStyleIdentifierMap.AddEntry("Light Grid Accent 2", (int)StyleIdentifier.LightGridAccent2);
            gStyleIdentifierMap.AddEntry("Light Grid Accent 3", (int)StyleIdentifier.LightGridAccent3);
            gStyleIdentifierMap.AddEntry("Light Grid Accent 4", (int)StyleIdentifier.LightGridAccent4);
            gStyleIdentifierMap.AddEntry("Light Grid Accent 5", (int)StyleIdentifier.LightGridAccent5);
            gStyleIdentifierMap.AddEntry("Light Grid Accent 6", (int)StyleIdentifier.LightGridAccent6);
            gStyleIdentifierMap.AddEntry("Light List", (int)StyleIdentifier.LightList);
            gStyleIdentifierMap.AddEntry("Light List Accent 1", (int)StyleIdentifier.LightListAccent1);
            gStyleIdentifierMap.AddEntry("Light List Accent 2", (int)StyleIdentifier.LightListAccent2);
            gStyleIdentifierMap.AddEntry("Light List Accent 3", (int)StyleIdentifier.LightListAccent3);
            gStyleIdentifierMap.AddEntry("Light List Accent 4", (int)StyleIdentifier.LightListAccent4);
            gStyleIdentifierMap.AddEntry("Light List Accent 5", (int)StyleIdentifier.LightListAccent5);
            gStyleIdentifierMap.AddEntry("Light List Accent 6", (int)StyleIdentifier.LightListAccent6);
            gStyleIdentifierMap.AddEntry("Light Shading", (int)StyleIdentifier.LightShading);
            gStyleIdentifierMap.AddEntry("Light Shading Accent 1", (int)StyleIdentifier.LightShadingAccent1);
            gStyleIdentifierMap.AddEntry("Light Shading Accent 2", (int)StyleIdentifier.LightShadingAccent2);
            gStyleIdentifierMap.AddEntry("Light Shading Accent 3", (int)StyleIdentifier.LightShadingAccent3);
            gStyleIdentifierMap.AddEntry("Light Shading Accent 4", (int)StyleIdentifier.LightShadingAccent4);
            gStyleIdentifierMap.AddEntry("Light Shading Accent 5", (int)StyleIdentifier.LightShadingAccent5);
            gStyleIdentifierMap.AddEntry("Light Shading Accent 6", (int)StyleIdentifier.LightShadingAccent6);
            gStyleIdentifierMap.AddEntry("Medium Grid 1", (int)StyleIdentifier.MediumGrid1);
            gStyleIdentifierMap.AddEntry("Medium Grid 1 Accent 1", (int)StyleIdentifier.MediumGrid1Accent1);
            gStyleIdentifierMap.AddEntry("Medium Grid 1 Accent 2", (int)StyleIdentifier.MediumGrid1Accent2);
            gStyleIdentifierMap.AddEntry("Medium Grid 1 Accent 3", (int)StyleIdentifier.MediumGrid1Accent3);
            gStyleIdentifierMap.AddEntry("Medium Grid 1 Accent 4", (int)StyleIdentifier.MediumGrid1Accent4);
            gStyleIdentifierMap.AddEntry("Medium Grid 1 Accent 5", (int)StyleIdentifier.MediumGrid1Accent5);
            gStyleIdentifierMap.AddEntry("Medium Grid 1 Accent 6", (int)StyleIdentifier.MediumGrid1Accent6);
            gStyleIdentifierMap.AddEntry("Medium Grid 2", (int)StyleIdentifier.MediumGrid2);
            gStyleIdentifierMap.AddEntry("Medium Grid 2 Accent 1", (int)StyleIdentifier.MediumGrid2Accent1);
            gStyleIdentifierMap.AddEntry("Medium Grid 2 Accent 2", (int)StyleIdentifier.MediumGrid2Accent2);
            gStyleIdentifierMap.AddEntry("Medium Grid 2 Accent 3", (int)StyleIdentifier.MediumGrid2Accent3);
            gStyleIdentifierMap.AddEntry("Medium Grid 2 Accent 4", (int)StyleIdentifier.MediumGrid2Accent4);
            gStyleIdentifierMap.AddEntry("Medium Grid 2 Accent 5", (int)StyleIdentifier.MediumGrid2Accent5);
            gStyleIdentifierMap.AddEntry("Medium Grid 2 Accent 6", (int)StyleIdentifier.MediumGrid2Accent6);
            gStyleIdentifierMap.AddEntry("Medium Grid 3", (int)StyleIdentifier.MediumGrid3);
            gStyleIdentifierMap.AddEntry("Medium Grid 3 Accent 1", (int)StyleIdentifier.MediumGrid3Accent1);
            gStyleIdentifierMap.AddEntry("Medium Grid 3 Accent 2", (int)StyleIdentifier.MediumGrid3Accent2);
            gStyleIdentifierMap.AddEntry("Medium Grid 3 Accent 3", (int)StyleIdentifier.MediumGrid3Accent3);
            gStyleIdentifierMap.AddEntry("Medium Grid 3 Accent 4", (int)StyleIdentifier.MediumGrid3Accent4);
            gStyleIdentifierMap.AddEntry("Medium Grid 3 Accent 5", (int)StyleIdentifier.MediumGrid3Accent5);
            gStyleIdentifierMap.AddEntry("Medium Grid 3 Accent 6", (int)StyleIdentifier.MediumGrid3Accent6);
            gStyleIdentifierMap.AddEntry("Medium List 1", (int)StyleIdentifier.MediumList1);
            gStyleIdentifierMap.AddEntry("Medium List 1 Accent 1", (int)StyleIdentifier.MediumList1Accent1);
            gStyleIdentifierMap.AddEntry("Medium List 1 Accent 2", (int)StyleIdentifier.MediumList1Accent2);
            gStyleIdentifierMap.AddEntry("Medium List 1 Accent 3", (int)StyleIdentifier.MediumList1Accent3);
            gStyleIdentifierMap.AddEntry("Medium List 1 Accent 4", (int)StyleIdentifier.MediumList1Accent4);
            gStyleIdentifierMap.AddEntry("Medium List 1 Accent 5", (int)StyleIdentifier.MediumList1Accent5);
            gStyleIdentifierMap.AddEntry("Medium List 1 Accent 6", (int)StyleIdentifier.MediumList1Accent6);
            gStyleIdentifierMap.AddEntry("Medium List 2", (int)StyleIdentifier.MediumList2);
            gStyleIdentifierMap.AddEntry("Medium List 2 Accent 1", (int)StyleIdentifier.MediumList2Accent1);
            gStyleIdentifierMap.AddEntry("Medium List 2 Accent 2", (int)StyleIdentifier.MediumList2Accent2);
            gStyleIdentifierMap.AddEntry("Medium List 2 Accent 3", (int)StyleIdentifier.MediumList2Accent3);
            gStyleIdentifierMap.AddEntry("Medium List 2 Accent 4", (int)StyleIdentifier.MediumList2Accent4);
            gStyleIdentifierMap.AddEntry("Medium List 2 Accent 5", (int)StyleIdentifier.MediumList2Accent5);
            gStyleIdentifierMap.AddEntry("Medium List 2 Accent 6", (int)StyleIdentifier.MediumList2Accent6);
            gStyleIdentifierMap.AddEntry("Medium Shading 1", (int)StyleIdentifier.MediumShading1);
            gStyleIdentifierMap.AddEntry("Medium Shading 1 Accent 1", (int)StyleIdentifier.MediumShading1Accent1);
            gStyleIdentifierMap.AddEntry("Medium Shading 1 Accent 2", (int)StyleIdentifier.MediumShading1Accent2);
            gStyleIdentifierMap.AddEntry("Medium Shading 1 Accent 3", (int)StyleIdentifier.MediumShading1Accent3);
            gStyleIdentifierMap.AddEntry("Medium Shading 1 Accent 4", (int)StyleIdentifier.MediumShading1Accent4);
            gStyleIdentifierMap.AddEntry("Medium Shading 1 Accent 5", (int)StyleIdentifier.MediumShading1Accent5);
            gStyleIdentifierMap.AddEntry("Medium Shading 1 Accent 6", (int)StyleIdentifier.MediumShading1Accent6);
            gStyleIdentifierMap.AddEntry("Medium Shading 2", (int)StyleIdentifier.MediumShading2);
            gStyleIdentifierMap.AddEntry("Medium Shading 2 Accent 1", (int)StyleIdentifier.MediumShading2Accent1);
            gStyleIdentifierMap.AddEntry("Medium Shading 2 Accent 2", (int)StyleIdentifier.MediumShading2Accent2);
            gStyleIdentifierMap.AddEntry("Medium Shading 2 Accent 3", (int)StyleIdentifier.MediumShading2Accent3);
            gStyleIdentifierMap.AddEntry("Medium Shading 2 Accent 4", (int)StyleIdentifier.MediumShading2Accent4);
            gStyleIdentifierMap.AddEntry("Medium Shading 2 Accent 5", (int)StyleIdentifier.MediumShading2Accent5);
            gStyleIdentifierMap.AddEntry("Medium Shading 2 Accent 6", (int)StyleIdentifier.MediumShading2Accent6);
            gStyleIdentifierMap.AddEntry("Table 3D effects 1", (int)StyleIdentifier.Table3DEffects1);
            gStyleIdentifierMap.AddEntry("Table 3D effects 2", (int)StyleIdentifier.Table3DEffects2);
            gStyleIdentifierMap.AddEntry("Table 3D effects 3", (int)StyleIdentifier.Table3DEffects3);
            gStyleIdentifierMap.AddEntry("Table Classic 1", (int)StyleIdentifier.TableClassic1);
            gStyleIdentifierMap.AddEntry("Table Classic 2", (int)StyleIdentifier.TableClassic2);
            gStyleIdentifierMap.AddEntry("Table Classic 3", (int)StyleIdentifier.TableClassic3);
            gStyleIdentifierMap.AddEntry("Table Classic 4", (int)StyleIdentifier.TableClassic4);
            gStyleIdentifierMap.AddEntry("Table Colorful 1", (int)StyleIdentifier.TableColorful1);
            gStyleIdentifierMap.AddEntry("Table Colorful 2", (int)StyleIdentifier.TableColorful2);
            gStyleIdentifierMap.AddEntry("Table Colorful 3", (int)StyleIdentifier.TableColorful3);
            gStyleIdentifierMap.AddEntry("Table Columns 1", (int)StyleIdentifier.TableColumns1);
            gStyleIdentifierMap.AddEntry("Table Columns 2", (int)StyleIdentifier.TableColumns2);
            gStyleIdentifierMap.AddEntry("Table Columns 3", (int)StyleIdentifier.TableColumns3);
            gStyleIdentifierMap.AddEntry("Table Columns 4", (int)StyleIdentifier.TableColumns4);
            gStyleIdentifierMap.AddEntry("Table Columns 5", (int)StyleIdentifier.TableColumns5);
            gStyleIdentifierMap.AddEntry("Table Contemporary", (int)StyleIdentifier.TableContemporary);
            gStyleIdentifierMap.AddEntry("Table Elegant", (int)StyleIdentifier.TableElegant);
            gStyleIdentifierMap.AddEntry("Table Grid", (int)StyleIdentifier.TableGrid);
            gStyleIdentifierMap.AddEntry("Table Grid 1", (int)StyleIdentifier.TableGrid1);
            gStyleIdentifierMap.AddEntry("Table Grid 2", (int)StyleIdentifier.TableGrid2);
            gStyleIdentifierMap.AddEntry("Table Grid 3", (int)StyleIdentifier.TableGrid3);
            gStyleIdentifierMap.AddEntry("Table Grid 4", (int)StyleIdentifier.TableGrid4);
            gStyleIdentifierMap.AddEntry("Table Grid 5", (int)StyleIdentifier.TableGrid5);
            gStyleIdentifierMap.AddEntry("Table Grid 6", (int)StyleIdentifier.TableGrid6);
            gStyleIdentifierMap.AddEntry("Table Grid 7", (int)StyleIdentifier.TableGrid7);
            gStyleIdentifierMap.AddEntry("Table Grid 8", (int)StyleIdentifier.TableGrid8);
            gStyleIdentifierMap.AddEntry("Table List 1", (int)StyleIdentifier.TableList1);
            gStyleIdentifierMap.AddEntry("Table List 2", (int)StyleIdentifier.TableList2);
            gStyleIdentifierMap.AddEntry("Table List 3", (int)StyleIdentifier.TableList3);
            gStyleIdentifierMap.AddEntry("Table List 4", (int)StyleIdentifier.TableList4);
            gStyleIdentifierMap.AddEntry("Table List 5", (int)StyleIdentifier.TableList5);
            gStyleIdentifierMap.AddEntry("Table List 6", (int)StyleIdentifier.TableList6);
            gStyleIdentifierMap.AddEntry("Table List 7", (int)StyleIdentifier.TableList7);
            gStyleIdentifierMap.AddEntry("Table List 8", (int)StyleIdentifier.TableList8);
            gStyleIdentifierMap.AddEntry("Normal Table", (int)StyleIdentifier.TableNormal);
            gStyleIdentifierMap.AddEntry("Table Professional", (int)StyleIdentifier.TableProfessional);
            gStyleIdentifierMap.AddEntry("Table Simple 1", (int)StyleIdentifier.TableSimple1);
            gStyleIdentifierMap.AddEntry("Table Simple 2", (int)StyleIdentifier.TableSimple2);
            gStyleIdentifierMap.AddEntry("Table Simple 3", (int)StyleIdentifier.TableSimple3);
            gStyleIdentifierMap.AddEntry("Table Subtle 1", (int)StyleIdentifier.TableSubtle1);
            gStyleIdentifierMap.AddEntry("Table Subtle 2", (int)StyleIdentifier.TableSubtle2);
            gStyleIdentifierMap.AddEntry("Table Theme", (int)StyleIdentifier.TableTheme);
            gStyleIdentifierMap.AddEntry("Table Web 1", (int)StyleIdentifier.TableWeb1);
            gStyleIdentifierMap.AddEntry("Table Web 2", (int)StyleIdentifier.TableWeb2);
            gStyleIdentifierMap.AddEntry("Table Web 3", (int)StyleIdentifier.TableWeb3);

            // New table styles
            gStyleIdentifierMap.AddEntry("Plain Table 1", (int)StyleIdentifier.PlainTable1);
            gStyleIdentifierMap.AddEntry("Plain Table 2", (int)StyleIdentifier.PlainTable2);
            gStyleIdentifierMap.AddEntry("Plain Table 3", (int)StyleIdentifier.PlainTable3);
            gStyleIdentifierMap.AddEntry("Plain Table 4", (int)StyleIdentifier.PlainTable4);
            gStyleIdentifierMap.AddEntry("Plain Table 5", (int)StyleIdentifier.PlainTable5);
            gStyleIdentifierMap.AddEntry("Grid Table Light", (int)StyleIdentifier.TableGridLight);
            gStyleIdentifierMap.AddEntry("Grid Table 1 Light", (int)StyleIdentifier.GridTable1Light);
            gStyleIdentifierMap.AddEntry("Grid Table 2", (int)StyleIdentifier.GridTable2);
            gStyleIdentifierMap.AddEntry("Grid Table 3", (int)StyleIdentifier.GridTable3);
            gStyleIdentifierMap.AddEntry("Grid Table 4", (int)StyleIdentifier.GridTable4);
            gStyleIdentifierMap.AddEntry("Grid Table 5 Dark", (int)StyleIdentifier.GridTable5Dark);
            gStyleIdentifierMap.AddEntry("Grid Table 6 Colorful", (int)StyleIdentifier.GridTable6Colorful);
            gStyleIdentifierMap.AddEntry("Grid Table 7 Colorful", (int)StyleIdentifier.GridTable7Colorful);
            gStyleIdentifierMap.AddEntry("Grid Table 1 Light Accent 1", (int)StyleIdentifier.GridTable1LightAccent1);
            gStyleIdentifierMap.AddEntry("Grid Table 2 Accent 1", (int)StyleIdentifier.GridTable2Accent1);
            gStyleIdentifierMap.AddEntry("Grid Table 3 Accent 1", (int)StyleIdentifier.GridTable3Accent1);
            gStyleIdentifierMap.AddEntry("Grid Table 4 Accent 1", (int)StyleIdentifier.GridTable4Accent1);
            gStyleIdentifierMap.AddEntry("Grid Table 5 Dark Accent 1", (int)StyleIdentifier.GridTable5DarkAccent1);
            gStyleIdentifierMap.AddEntry("Grid Table 6 Colorful Accent 1", (int)StyleIdentifier.GridTable6ColorfulAccent1);
            gStyleIdentifierMap.AddEntry("Grid Table 7 Colorful Accent 1", (int)StyleIdentifier.GridTable7ColorfulAccent1);
            gStyleIdentifierMap.AddEntry("Grid Table 1 Light Accent 2", (int)StyleIdentifier.GridTable1LightAccent2);
            gStyleIdentifierMap.AddEntry("Grid Table 2 Accent 2", (int)StyleIdentifier.GridTable2Accent2);
            gStyleIdentifierMap.AddEntry("Grid Table 3 Accent 2", (int)StyleIdentifier.GridTable3Accent2);
            gStyleIdentifierMap.AddEntry("Grid Table 4 Accent 2", (int)StyleIdentifier.GridTable4Accent2);
            gStyleIdentifierMap.AddEntry("Grid Table 5 Dark Accent 2", (int)StyleIdentifier.GridTable5DarkAccent2);
            gStyleIdentifierMap.AddEntry("Grid Table 6 Colorful Accent 2", (int)StyleIdentifier.GridTable6ColorfulAccent2);
            gStyleIdentifierMap.AddEntry("Grid Table 7 Colorful Accent 2", (int)StyleIdentifier.GridTable7ColorfulAccent2);
            gStyleIdentifierMap.AddEntry("Grid Table 1 Light Accent 3", (int)StyleIdentifier.GridTable1LightAccent3);
            gStyleIdentifierMap.AddEntry("Grid Table 2 Accent 3", (int)StyleIdentifier.GridTable2Accent3);
            gStyleIdentifierMap.AddEntry("Grid Table 3 Accent 3", (int)StyleIdentifier.GridTable3Accent3);
            gStyleIdentifierMap.AddEntry("Grid Table 4 Accent 3", (int)StyleIdentifier.GridTable4Accent3);
            gStyleIdentifierMap.AddEntry("Grid Table 5 Dark Accent 3", (int)StyleIdentifier.GridTable5DarkAccent3);
            gStyleIdentifierMap.AddEntry("Grid Table 6 Colorful Accent 3", (int)StyleIdentifier.GridTable6ColorfulAccent3);
            gStyleIdentifierMap.AddEntry("Grid Table 7 Colorful Accent 3", (int)StyleIdentifier.GridTable7ColorfulAccent3);
            gStyleIdentifierMap.AddEntry("Grid Table 1 Light Accent 4", (int)StyleIdentifier.GridTable1LightAccent4);
            gStyleIdentifierMap.AddEntry("Grid Table 2 Accent 4", (int)StyleIdentifier.GridTable2Accent4);
            gStyleIdentifierMap.AddEntry("Grid Table 3 Accent 4", (int)StyleIdentifier.GridTable3Accent4);
            gStyleIdentifierMap.AddEntry("Grid Table 4 Accent 4", (int)StyleIdentifier.GridTable4Accent4);
            gStyleIdentifierMap.AddEntry("Grid Table 5 Dark Accent 4", (int)StyleIdentifier.GridTable5DarkAccent4);
            gStyleIdentifierMap.AddEntry("Grid Table 6 Colorful Accent 4", (int)StyleIdentifier.GridTable6ColorfulAccent4);
            gStyleIdentifierMap.AddEntry("Grid Table 7 Colorful Accent 4", (int)StyleIdentifier.GridTable7ColorfulAccent4);
            gStyleIdentifierMap.AddEntry("Grid Table 1 Light Accent 5", (int)StyleIdentifier.GridTable1LightAccent5);
            gStyleIdentifierMap.AddEntry("Grid Table 2 Accent 5", (int)StyleIdentifier.GridTable2Accent5);
            gStyleIdentifierMap.AddEntry("Grid Table 3 Accent 5", (int)StyleIdentifier.GridTable3Accent5);
            gStyleIdentifierMap.AddEntry("Grid Table 4 Accent 5", (int)StyleIdentifier.GridTable4Accent5);
            gStyleIdentifierMap.AddEntry("Grid Table 5 Dark Accent 5", (int)StyleIdentifier.GridTable5DarkAccent5);
            gStyleIdentifierMap.AddEntry("Grid Table 6 Colorful Accent 5", (int)StyleIdentifier.GridTable6ColorfulAccent5);
            gStyleIdentifierMap.AddEntry("Grid Table 7 Colorful Accent 5", (int)StyleIdentifier.GridTable7ColorfulAccent5);
            gStyleIdentifierMap.AddEntry("Grid Table 1 Light Accent 6", (int)StyleIdentifier.GridTable1LightAccent6);
            gStyleIdentifierMap.AddEntry("Grid Table 2 Accent 6", (int)StyleIdentifier.GridTable2Accent6);
            gStyleIdentifierMap.AddEntry("Grid Table 3 Accent 6", (int)StyleIdentifier.GridTable3Accent6);
            gStyleIdentifierMap.AddEntry("Grid Table 4 Accent 6", (int)StyleIdentifier.GridTable4Accent6);
            gStyleIdentifierMap.AddEntry("Grid Table 5 Dark Accent 6", (int)StyleIdentifier.GridTable5DarkAccent6);
            gStyleIdentifierMap.AddEntry("Grid Table 6 Colorful Accent 6", (int)StyleIdentifier.GridTable6ColorfulAccent6);
            gStyleIdentifierMap.AddEntry("Grid Table 7 Colorful Accent 6", (int)StyleIdentifier.GridTable7ColorfulAccent6);
            gStyleIdentifierMap.AddEntry("List Table 1 Light", (int)StyleIdentifier.ListTable1Light);
            gStyleIdentifierMap.AddEntry("List Table 2", (int)StyleIdentifier.ListTable2);
            gStyleIdentifierMap.AddEntry("List Table 3", (int)StyleIdentifier.ListTable3);
            gStyleIdentifierMap.AddEntry("List Table 4", (int)StyleIdentifier.ListTable4);
            gStyleIdentifierMap.AddEntry("List Table 5 Dark", (int)StyleIdentifier.ListTable5Dark);
            gStyleIdentifierMap.AddEntry("List Table 6 Colorful", (int)StyleIdentifier.ListTable6Colorful);
            gStyleIdentifierMap.AddEntry("List Table 7 Colorful", (int)StyleIdentifier.ListTable7Colorful);
            gStyleIdentifierMap.AddEntry("List Table 1 Light Accent 1", (int)StyleIdentifier.ListTable1LightAccent1);
            gStyleIdentifierMap.AddEntry("List Table 2 Accent 1", (int)StyleIdentifier.ListTable2Accent1);
            gStyleIdentifierMap.AddEntry("List Table 3 Accent 1", (int)StyleIdentifier.ListTable3Accent1);
            gStyleIdentifierMap.AddEntry("List Table 4 Accent 1", (int)StyleIdentifier.ListTable4Accent1);
            gStyleIdentifierMap.AddEntry("List Table 5 Dark Accent 1", (int)StyleIdentifier.ListTable5DarkAccent1);
            gStyleIdentifierMap.AddEntry("List Table 6 Colorful Accent 1", (int)StyleIdentifier.ListTable6ColorfulAccent1);
            gStyleIdentifierMap.AddEntry("List Table 7 Colorful Accent 1", (int)StyleIdentifier.ListTable7ColorfulAccent1);
            gStyleIdentifierMap.AddEntry("List Table 1 Light Accent 2", (int)StyleIdentifier.ListTable1LightAccent2);
            gStyleIdentifierMap.AddEntry("List Table 2 Accent 2", (int)StyleIdentifier.ListTable2Accent2);
            gStyleIdentifierMap.AddEntry("List Table 3 Accent 2", (int)StyleIdentifier.ListTable3Accent2);
            gStyleIdentifierMap.AddEntry("List Table 4 Accent 2", (int)StyleIdentifier.ListTable4Accent2);
            gStyleIdentifierMap.AddEntry("List Table 5 Dark Accent 2", (int)StyleIdentifier.ListTable5DarkAccent2);
            gStyleIdentifierMap.AddEntry("List Table 6 Colorful Accent 2", (int)StyleIdentifier.ListTable6ColorfulAccent2);
            gStyleIdentifierMap.AddEntry("List Table 7 Colorful Accent 2", (int)StyleIdentifier.ListTable7ColorfulAccent2);
            gStyleIdentifierMap.AddEntry("List Table 1 Light Accent 3", (int)StyleIdentifier.ListTable1LightAccent3);
            gStyleIdentifierMap.AddEntry("List Table 2 Accent 3", (int)StyleIdentifier.ListTable2Accent3);
            gStyleIdentifierMap.AddEntry("List Table 3 Accent 3", (int)StyleIdentifier.ListTable3Accent3);
            gStyleIdentifierMap.AddEntry("List Table 4 Accent 3", (int)StyleIdentifier.ListTable4Accent3);
            gStyleIdentifierMap.AddEntry("List Table 5 Dark Accent 3", (int)StyleIdentifier.ListTable5DarkAccent3);
            gStyleIdentifierMap.AddEntry("List Table 6 Colorful Accent 3", (int)StyleIdentifier.ListTable6ColorfulAccent3);
            gStyleIdentifierMap.AddEntry("List Table 7 Colorful Accent 3", (int)StyleIdentifier.ListTable7ColorfulAccent3);
            gStyleIdentifierMap.AddEntry("List Table 1 Light Accent 4", (int)StyleIdentifier.ListTable1LightAccent4);
            gStyleIdentifierMap.AddEntry("List Table 2 Accent 4", (int)StyleIdentifier.ListTable2Accent4);
            gStyleIdentifierMap.AddEntry("List Table 3 Accent 4", (int)StyleIdentifier.ListTable3Accent4);
            gStyleIdentifierMap.AddEntry("List Table 4 Accent 4", (int)StyleIdentifier.ListTable4Accent4);
            gStyleIdentifierMap.AddEntry("List Table 5 Dark Accent 4", (int)StyleIdentifier.ListTable5DarkAccent4);
            gStyleIdentifierMap.AddEntry("List Table 6 Colorful Accent 4", (int)StyleIdentifier.ListTable6ColorfulAccent4);
            gStyleIdentifierMap.AddEntry("List Table 7 Colorful Accent 4", (int)StyleIdentifier.ListTable7ColorfulAccent4);
            gStyleIdentifierMap.AddEntry("List Table 1 Light Accent 5", (int)StyleIdentifier.ListTable1LightAccent5);
            gStyleIdentifierMap.AddEntry("List Table 2 Accent 5", (int)StyleIdentifier.ListTable2Accent5);
            gStyleIdentifierMap.AddEntry("List Table 3 Accent 5", (int)StyleIdentifier.ListTable3Accent5);
            gStyleIdentifierMap.AddEntry("List Table 4 Accent 5", (int)StyleIdentifier.ListTable4Accent5);
            gStyleIdentifierMap.AddEntry("List Table 5 Dark Accent 5", (int)StyleIdentifier.ListTable5DarkAccent5);
            gStyleIdentifierMap.AddEntry("List Table 6 Colorful Accent 5", (int)StyleIdentifier.ListTable6ColorfulAccent5);
            gStyleIdentifierMap.AddEntry("List Table 7 Colorful Accent 5", (int)StyleIdentifier.ListTable7ColorfulAccent5);
            gStyleIdentifierMap.AddEntry("List Table 1 Light Accent 6", (int)StyleIdentifier.ListTable1LightAccent6);
            gStyleIdentifierMap.AddEntry("List Table 2 Accent 6", (int)StyleIdentifier.ListTable2Accent6);
            gStyleIdentifierMap.AddEntry("List Table 3 Accent 6", (int)StyleIdentifier.ListTable3Accent6);
            gStyleIdentifierMap.AddEntry("List Table 4 Accent 6", (int)StyleIdentifier.ListTable4Accent6);
            gStyleIdentifierMap.AddEntry("List Table 5 Dark Accent 6", (int)StyleIdentifier.ListTable5DarkAccent6);
            gStyleIdentifierMap.AddEntry("List Table 6 Colorful Accent 6", (int)StyleIdentifier.ListTable6ColorfulAccent6);
            gStyleIdentifierMap.AddEntry("List Table 7 Colorful Accent 6", (int)StyleIdentifier.ListTable7ColorfulAccent6);
            gStyleIdentifierMap.AddEntry("Mention", (int)StyleIdentifier.Mention);
            gStyleIdentifierMap.AddEntry("Smart Hyperlink", (int)StyleIdentifier.SmartHyperlink);
            gStyleIdentifierMap.AddEntry("Hashtag", (int)StyleIdentifier.Hashtag);
            gStyleIdentifierMap.AddEntry("Unresolved Mention", (int)StyleIdentifier.UnresolvedMention);

            // This contains all style names that are different between DOCX/WordML and model/DOC/RTF.
            // First entry is name in DOCX/WordML, second entry is name in the Model/DOC/RTF.

            // Character styles.
            gStyleNameMap.AddEntry("annotation reference", "Comment Reference");
            gStyleNameMap.AddEntry("endnote reference", "Endnote Reference");
            gStyleNameMap.AddEntry("footnote reference", "Footnote Reference");
            gStyleNameMap.AddEntry("line number", "Line Number");
            gStyleNameMap.AddEntry("page number", "Page Number");

            // Paragraph styles.
            gStyleNameMap.AddEntry("annotation subject", "Comment Subject");
            gStyleNameMap.AddEntry("annotation text", "Comment Text");
            gStyleNameMap.AddEntry("caption", "Caption");
            gStyleNameMap.AddEntry("endnote text", "Endnote Text");
            gStyleNameMap.AddEntry("envelope address", "Envelope Address");
            gStyleNameMap.AddEntry("envelope return", "Envelope Return");
            gStyleNameMap.AddEntry("footer", "Footer");
            gStyleNameMap.AddEntry("footnote text", "Footnote Text");
            gStyleNameMap.AddEntry("header", "Header");
            gStyleNameMap.AddEntry("heading 1", "Heading 1");
            gStyleNameMap.AddEntry("heading 2", "Heading 2");
            gStyleNameMap.AddEntry("heading 3", "Heading 3");
            gStyleNameMap.AddEntry("heading 4", "Heading 4");
            gStyleNameMap.AddEntry("heading 5", "Heading 5");
            gStyleNameMap.AddEntry("heading 6", "Heading 6");
            gStyleNameMap.AddEntry("heading 7", "Heading 7");
            gStyleNameMap.AddEntry("heading 8", "Heading 8");
            gStyleNameMap.AddEntry("heading 9", "Heading 9");
            gStyleNameMap.AddEntry("index 1", "Index 1");
            gStyleNameMap.AddEntry("index 2", "Index 2");
            gStyleNameMap.AddEntry("index 3", "Index 3");
            gStyleNameMap.AddEntry("index 4", "Index 4");
            gStyleNameMap.AddEntry("index 5", "Index 5");
            gStyleNameMap.AddEntry("index 6", "Index 6");
            gStyleNameMap.AddEntry("index 7", "Index 7");
            gStyleNameMap.AddEntry("index 8", "Index 8");
            gStyleNameMap.AddEntry("index 9", "Index 9");
            gStyleNameMap.AddEntry("index heading", "Index Heading");
            gStyleNameMap.AddEntry("macro", "Macro Text");
            gStyleNameMap.AddEntry("note heading", "Note Heading");
            gStyleNameMap.AddEntry("table of authorities", "Table of Authorities");
            gStyleNameMap.AddEntry("table of figures", "Table of Figures");
            gStyleNameMap.AddEntry("toa heading", "TOA Heading");
            gStyleNameMap.AddEntry("toc 1", "TOC 1");
            gStyleNameMap.AddEntry("toc 2", "TOC 2");
            gStyleNameMap.AddEntry("toc 3", "TOC 3");
            gStyleNameMap.AddEntry("toc 4", "TOC 4");
            gStyleNameMap.AddEntry("toc 5", "TOC 5");
            gStyleNameMap.AddEntry("toc 6", "TOC 6");
            gStyleNameMap.AddEntry("toc 7", "TOC 7");
            gStyleNameMap.AddEntry("toc 8", "TOC 8");
            gStyleNameMap.AddEntry("toc 9", "TOC 9");

            // List styles.
            gStyleNameMap.AddEntry("Outline List 1", "1 / a / i");
            gStyleNameMap.AddEntry("Outline List 2", "1 / 1.1 / 1.1.1");
            gStyleNameMap.AddEntry("Outline List 3", "Article / Section");

            // Table styles.
            gStyleNameMap.AddEntry("Normal Table", "Table Normal"); //TableNormal

            gScreenSizeMap.AddEntry("1024x768", (int)ScreenSize.Size1024x768);
            gScreenSizeMap.AddEntry("1152x882", (int)ScreenSize.Size1152x882);
            gScreenSizeMap.AddEntry("1152x900", (int)ScreenSize.Size1152x900);
            gScreenSizeMap.AddEntry("1280x1024", (int)ScreenSize.Size1280x1024);
            gScreenSizeMap.AddEntry("1600x1200", (int)ScreenSize.Size1600x1200);
            gScreenSizeMap.AddEntry("1800x1440", (int)ScreenSize.Size1800x1440);
            gScreenSizeMap.AddEntry("1920x1200", (int)ScreenSize.Size1920x1200);
            gScreenSizeMap.AddEntry("544x376", (int)ScreenSize.Size544x376);
            gScreenSizeMap.AddEntry("640x480", (int)ScreenSize.Size640x480);
            gScreenSizeMap.AddEntry("720x512", (int)ScreenSize.Size720x512);
            gScreenSizeMap.AddEntry("800x600", (int)ScreenSize.Size800x600);

        }
    }
}
