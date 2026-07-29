// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/07/2007 by Vladimir Averkin

using System.Text;
using Aspose.Collections;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Fields;
using Aspose.Words.Fonts;
using Aspose.Words.Markup;
using Aspose.Words.Math;
using Aspose.Words.Notes;
using Aspose.Words.WebExtensions;

namespace Aspose.Words.Nrx
{
    /// <summary>
    /// Converts miscellaneous enumerated types between enum and DOCX string.
    /// </summary>
    internal class DocxEnum
    {
        internal static string ContentTypeToExtension(string contentType)
        {
            switch (contentType)
            {
                case DocxContentType.Doc: return ".doc";
                case DocxContentType.Xls: return ".xls";
                case DocxContentType.Ppt: return ".ppt";
                case DocxContentType.Vsd: return ".vsd";
                case DocxContentType.Docx: return ".docx";
                case DocxContentType.Dotx: return ".dotx";
                case DocxContentType.Docm: return ".docm";
                case DocxContentType.Dotm: return ".dotm";
                case DocxContentType.Xlsx: return ".xlsx";
                case DocxContentType.Xlsm: return ".xlsm";
                case DocxContentType.Xlsb: return ".xlsb";
                case DocxContentType.Pptx: return ".pptx";
                case DocxContentType.Pptm: return ".pptm";
                case DocxContentType.Ppsx: return ".ppsx";
                case DocxContentType.Ppsm: return ".ppsm";
                case DocxContentType.Sldx: return ".sldx";
                case DocxContentType.Sldm: return ".sldm";
                case DocxContentType.Vsdx: return ".vsdx";
                default: return ".bin";
            }
        }

        internal static Ligature XmlToLigature(string value)
        {
            if (value == "none")
                return Ligature.None;
            else if (value == "all")
                return Ligature.Standard | Ligature.Contextual | Ligature.Historical | Ligature.Discretional;

            Ligature ligature = Ligature.None;

            if (StringUtil.Contains(value, "standard", true))
                ligature |= Ligature.Standard;

            if (StringUtil.Contains(value, "contextual", true))
                ligature |= Ligature.Contextual;

            if (StringUtil.Contains(value, "historical", true))
                ligature |= Ligature.Historical;

            if (StringUtil.Contains(value, "discretional", true))
                ligature |= Ligature.Discretional;

            return ligature;
        }

        internal static string LigatureToXml(Ligature ligature)
        {
            if (ligature == Ligature.None)
                return "none";
            else if (ligature == Ligature.All)
                return "all";

            StringBuilder sb = new StringBuilder();

            if ((ligature & Ligature.Standard) != 0)
                sb.Append("Standard");

            if ((ligature & Ligature.Contextual) != 0)
                sb.Append("Contextual");

            if ((ligature & Ligature.Historical) != 0)
                sb.Append("Historical");

            if ((ligature & Ligature.Discretional) != 0)
                sb.Append("Discretional");

            string value = sb.ToString();

            // Make first letter lower.
            return string.Format("{0}{1}", value.Substring(0, 1).ToLower(), value.Substring(1));
        }

        internal static NumForm XmlToNumForm(string value)
        {
            switch (value)
            {
                case "default": return NumForm.Default;
                case "lining": return NumForm.Lining;
                case "oldStyle": return NumForm.OldStyle;
                default: return NumForm.Default;
            }
        }

        internal static string NumFormToXml(NumForm numForm)
        {
            switch (numForm)
            {
                case NumForm.Default: return "default";
                case NumForm.Lining: return "lining";
                case NumForm.OldStyle: return "oldStyle";
                default: return "";
            }
        }

        internal static NumSpacing XmlToNumSpacing(string value)
        {
            switch (value)
            {
                case "default": return NumSpacing.Default;
                case "proportional": return NumSpacing.Proportional;
                case "tabular": return NumSpacing.Tabular;
                default: return NumSpacing.Default;
            }
        }

        internal static string NumSpacingToXml(NumSpacing numSpacing)
        {
            switch (numSpacing)
            {
                case NumSpacing.Default: return "default";
                case NumSpacing.Proportional: return "proportional";
                case NumSpacing.Tabular: return "tabular";
                default: return "";
            }
        }

        /// <summary>
        /// Converts Xml value to StylisticSets.
        /// </summary>
        internal static StylisticSets XmlToStylisticSets(string value)
        {
            switch (value)
            {
                case "1": return StylisticSets.Set01;
                case "2": return StylisticSets.Set02;
                case "3": return StylisticSets.Set03;
                case "4": return StylisticSets.Set04;
                case "5": return StylisticSets.Set05;
                case "6": return StylisticSets.Set06;
                case "7": return StylisticSets.Set07;
                case "8": return StylisticSets.Set08;
                case "9": return StylisticSets.Set09;
                case "10": return StylisticSets.Set10;
                case "11": return StylisticSets.Set11;
                case "12": return StylisticSets.Set12;
                case "13": return StylisticSets.Set13;
                case "14": return StylisticSets.Set14;
                case "15": return StylisticSets.Set15;
                case "16": return StylisticSets.Set16;
                case "17": return StylisticSets.Set17;
                case "18": return StylisticSets.Set18;
                case "19": return StylisticSets.Set19;
                case "20": return StylisticSets.Set20;
                default: return StylisticSets.Default;
            }
        }

        /// <summary>
        /// Converts StylisticSets to Xml value.
        /// </summary>
        internal static string StylisticSetsToXml(StylisticSets stylisticSets)
        {
            switch (stylisticSets)
            {
                case StylisticSets.Set01: return "1";
                case StylisticSets.Set02: return "2";
                case StylisticSets.Set03: return "3";
                case StylisticSets.Set04: return "4";
                case StylisticSets.Set05: return "5";
                case StylisticSets.Set06: return "6";
                case StylisticSets.Set07: return "7";
                case StylisticSets.Set08: return "8";
                case StylisticSets.Set09: return "9";
                case StylisticSets.Set10: return "10";
                case StylisticSets.Set11: return "11";
                case StylisticSets.Set12: return "12";
                case StylisticSets.Set13: return "13";
                case StylisticSets.Set14: return "14";
                case StylisticSets.Set15: return "15";
                case StylisticSets.Set16: return "16";
                case StylisticSets.Set17: return "17";
                case StylisticSets.Set18: return "18";
                case StylisticSets.Set19: return "19";
                case StylisticSets.Set20: return "20";
                default: return "0";
            }
        }

        internal static TextFormFieldType DocxToTextFormFieldType(string value)
        {
            switch (value)
            {
                case "calculated": return TextFormFieldType.Calculated;
                case "currentDate": return TextFormFieldType.CurrentDate;
                case "currentTime": return TextFormFieldType.CurrentTime;
                case "date": return TextFormFieldType.Date;
                case "number": return TextFormFieldType.Number;
                case "": return TextFormFieldType.Regular;
                case "regular": return TextFormFieldType.Regular;
                default: return TextFormFieldType.Regular;
            }
        }

        internal static string TextFormFieldTypeToDocx(TextFormFieldType value)
        {
            switch (value)
            {
                case TextFormFieldType.Calculated: return "calculated";
                case TextFormFieldType.CurrentDate: return "currentDate";
                case TextFormFieldType.CurrentTime: return "currentTime";
                case TextFormFieldType.Date: return "date";
                case TextFormFieldType.Number: return "number";
                case TextFormFieldType.Regular: return "";
                default: return "";
            }
        }

        internal static NumberStyle DocxToNumberStyle(string value)
        {
            return (NumberStyle)gNumberStyleMap.GetValue(value, (int)NumberStyle.Arabic);
        }

        internal static string NumberStyleToDocx(NumberStyle value)
        {
            return gNumberStyleMap.GetValue((int)value, "");
        }

        internal static FontFamily DocxToFontFamily(string value)
        {
            return (FontFamily)gFontFamilyMap.GetValue(value, (int)FontFamily.Auto);
        }

        internal static string FontFamilyToDocx(FontFamily value)
        {
            return gFontFamilyMap.GetValue((int)value, "");
        }

        internal static LineStyle DocxToLineStyle(string value)
        {
            return (LineStyle)gLineStyleMap.GetValue(value, (int)LineStyle.None);
        }

        internal static string LineStyleToDocx(LineStyle value)
        {
            switch (value)
            {
                // andrnosk: WORDSNET-7167 Hairline is not in spec and it seems Hairline made equivalent to Single
                // Made in the same way as WmlEnum.LineStyleToWml.
                case LineStyle.Hairline: return "single";
                default:
                    return gLineStyleMap.GetValue((int)value, "none");
            }
        }

        internal static StyleType DocxToStyleType(string value)
        {
            return (StyleType)gStyleTypeMap.GetValue(value, (int)StyleType.Paragraph);
        }

        internal static string StyleTypeToDocx(StyleType value)
        {
            return gStyleTypeMap.GetValue((int)value, "paragraph");
        }

        internal static FootnoteSeparatorType DocxToFootnoteSeparatorType(string value, bool isEndnote)
        {
            StringToIntBidirectionalMap map = isEndnote ? gEndnoteSeparatorTypeMap : gFootnoteSeparatorTypeMap;
            return (FootnoteSeparatorType)map.GetValue(value, -1);
        }

        internal static string FootnoteSeparatorTypeToDocx(FootnoteSeparatorType value, bool isEndnote)
        {
            StringToIntBidirectionalMap map = isEndnote ? gEndnoteSeparatorTypeMap : gFootnoteSeparatorTypeMap;
            return map.GetValue((int)value, "");
        }

        internal static string DocPartBehaviorToDocx(BuildingBlockBehavior value)
        {
            return gDocPartBehaviorMap.GetValue((int)value, "");
        }

        internal static BuildingBlockBehavior DocxToDocPartBehavior(string value)
        {
            return (BuildingBlockBehavior)gDocPartBehaviorMap.GetValue(value, (int)BuildingBlockBehavior.Default);
        }

        internal static string DocPartGalleryToDocx(BuildingBlockGallery value)
        {
            return gDocPartGalleryMap.GetValue((int)value, "any");
        }

        internal static BuildingBlockGallery DocxToDocPartGallery(string value)
        {
            return (BuildingBlockGallery)gDocPartGalleryMap.GetValue(value, (int)BuildingBlockGallery.All);
        }

        internal static string DocPartTypeToDocx(BuildingBlockType value)
        {
            return gDocPartTypeMap.GetValue((int)value, "");
        }

        internal static BuildingBlockType DocxToDocPartType(string value)
        {
            return (BuildingBlockType)gDocPartTypeMap.GetValue(value, (int)BuildingBlockType.None);
        }

        internal static string CalendarTypeToDocx(SdtCalendarType value)
        {
            return gCalendarTypeMap.GetValue((int)value, "");
        }

        internal static SdtCalendarType DocxToCaledarType(string value)
        {
            return (SdtCalendarType)gCalendarTypeMap.GetValue(value, (int)SdtCalendarType.None);
        }

        internal static string SdtLockTypeToDocx(SdtLockType value)
        {
            return gSdtLockTypeMap.GetValue((int)value, "");
        }

        internal static SdtLockType DocxToSdtLockType(string value)
        {
            return (SdtLockType)gSdtLockTypeMap.GetValue(value, (int)SdtLockType.Unlocked);
        }

        internal static SdtDateStorageFormat DocxToSdtDateStorageFormat(string value)
        {
            return (SdtDateStorageFormat)gSdtDateStorageFormatMap.GetValue(value, (int)SdtDateStorageFormat.Default);
        }

        internal static string SdtDateStorageFormatToDocx(SdtDateStorageFormat value)
        {
            return gSdtDateStorageFormatMap.GetValue((int)value, "dateTime");
        }

        internal static SdtAppearance DocxToSdtAppearance(string value)
        {
            return (SdtAppearance)gSdtAppearanceMap.GetValue(value, (int)SdtAppearance.Default);
        }

        internal static string SdtAppearanceToDocx(SdtAppearance value)
        {
            return gSdtAppearanceMap.GetValue((int)value, "boundingBox");
        }

        internal static string MathJustificationToDocx(OfficeMathJustification value)
        {
            return gMathJustificationMap.GetValue((int)value, "");
        }

        internal static OfficeMathJustification DocxToMathJustification(string value)
        {
            return (OfficeMathJustification)gMathJustificationMap.GetValue(value, (int)OfficeMathJustification.Default);
        }

        internal static string MathScriptTypeToDocx(MathScript value)
        {
            return gMathScriptMap.GetValue((int)value, "");
        }

        internal static MathScript DocxToMathScriptType(string value)
        {
            return (MathScript)gMathScriptMap.GetValue(value, (int)MathScript.Default);
        }

        internal static string MathStyleTypeToDocx(MathStyle value)
        {
            return gMathStyleMap.GetValue((int)value, "");
        }

        internal static MathStyle DocxToMathStyleType(string value)
        {
            return (MathStyle)gMathStyleMap.GetValue(value, (int)MathStyle.Default);
        }

        internal static string MathPositionTypeToDocx(MathPosition value)
        {
            return gMathPositionMap.GetValue((int)value, "");
        }

        internal static MathPosition DocxToMathPositionType(string value)
        {
            return (MathPosition)gMathPositionMap.GetValue(value, (int)MathPosition.Default);
        }

        internal static string MathDelimiterShapeToDocx(MathDelimiterShape value)
        {
            return gMathDelimiterShapeMap.GetValue((int)value, "");
        }

        internal static MathDelimiterShape DocxToMathDelimiterShape(string value)
        {
            return (MathDelimiterShape)gMathDelimiterShapeMap.GetValue(value, (int)MathDelimiterShape.Default);
        }

        internal static string MathBaseJustificationToDocx(MathBaseJustification value, bool isIso29500)
        {

            switch (value)
            {
                case MathBaseJustification.Top:
                    return "top";
                case MathBaseJustification.Bottom:
                    return (isIso29500) ? "bottom" : "bot";
                default:
                    return "center";
            }
        }

        internal static MathBaseJustification DocxToMathBaseJustification(string value,
            OoxmlComplianceInfo complianceInfo)
        {
            switch (value)
            {
                case "top":
                    return MathBaseJustification.Top;
                case "bottom":
                    if (complianceInfo != null)
                        complianceInfo.MarkAsIsoTransitional();
                    return MathBaseJustification.Bottom;
                case "bot":
                    return MathBaseJustification.Bottom;
                case "center":
                    return MathBaseJustification.Center;
                default:
                    return MathBaseJustification.Default;
            }
        }

        internal static string MathVerticalJustificationTypeToDocx(MathVerticalJustification value)
        {
            return gMathVerticalJustificationTypeMap.GetValue((int)value, "");
        }

        internal static MathVerticalJustification DocxToMathVerticalJustificationType(string value)
        {
            return (MathVerticalJustification)gMathVerticalJustificationTypeMap.GetValue(value, (int)MathVerticalJustification.Bottom);
        }

        internal static string MathFractionTypeToDocx(MathFractionType value)
        {
            return gMathFractionTypeMap.GetValue((int)value, "");
        }

        internal static MathFractionType DocxToMathFractionType(string value)
        {
            return (MathFractionType)gMathFractionTypeMap.GetValue(value, (int)MathFractionType.Default);
        }

        internal static string MathLimitLocationToDocx(MathLimitLocation value)
        {
            return gMathLimitLocationMap.GetValue((int)value, "");
        }

        internal static MathLimitLocation DocxToMathLimitLocation(string value)
        {
            return (MathLimitLocation)gMathLimitLocationMap.GetValue(value, (int)MathLimitLocation.UnderOver);
        }

        internal static string AbsolutePositionTabAlignmentToDocx(AbsolutePositionTabAlignment value)
        {
            return gAbsolutePositionTabAlignmentMap.GetValue((int)value, "");
        }

        internal static AbsolutePositionTabAlignment DocxToAbsolutePositionTabAlignment(string value)
        {
            return (AbsolutePositionTabAlignment)gAbsolutePositionTabAlignmentMap.GetValue(value, (int)AbsolutePositionTabAlignment.Default);
        }

        internal static string AbsolutePositionTabLeaderCharToDocx(AbsolutePositionTabLeaderChar value)
        {
            return gAbsolutePositionTabLeaderCharMap.GetValue((int)value, "");
        }

        internal static AbsolutePositionTabLeaderChar DocxToAbsolutePositionTabLeaderChar(string value)
        {
            return (AbsolutePositionTabLeaderChar)gAbsolutePositionTabLeaderCharMap.GetValue(value, (int)AbsolutePositionTabLeaderChar.Default);
        }

        internal static string AbsolutePositionTabPositioningBaseToDocx(AbsolutePositionTabPositioningBase value)
        {
            return gAbsolutePositionTabRelativeToMap.GetValue((int)value, "");
        }

        internal static AbsolutePositionTabPositioningBase DocxToAbsolutePositionTabPositioningBase(string value)
        {
            return (AbsolutePositionTabPositioningBase)gAbsolutePositionTabRelativeToMap.GetValue(value, (int)AbsolutePositionTabPositioningBase.Default);
        }

        internal static string ContactIdentityProviderToDocx(ContactIdentityProvider value)
        {
            return gContactIdentityProviderMap.GetValue((int)value, "None");
        }

        internal static ContactIdentityProvider DocxToContactIdentityProvider(string value)
        {
            return (ContactIdentityProvider)gContactIdentityProviderMap.GetValue(value,
                (int)ContactIdentityProvider.None);
        }

        internal static TaskPaneDockState DocxToTaskPaneDockState(string value)
        {
            return (TaskPaneDockState)gTaskPaneDockStateMap.GetValue(value, (int)TaskPaneDockState.Right);
        }

        internal static string TaskPaneDockStateToDocx(TaskPaneDockState dockState)
        {
            return gTaskPaneDockStateMap.GetValue((int)dockState, "right");
        }

        internal static WebExtensionStoreType DocxToWebExtensionStoreType(string value)
        {
            return (WebExtensionStoreType)gWebExtensionStoreTypeMap.GetValue(value, (int)WebExtensionStoreType.Default);
        }

        internal static string WebExtensionStoreTypeToDocx(WebExtensionStoreType storeType)
        {
            return gWebExtensionStoreTypeMap.GetValue((int)storeType, "spcatalog");
        }

        internal static WebExtensionBindingType DocxToWebExtensionBindingType(string value)
        {
            return (WebExtensionBindingType)gWebExtensionBindingTypeMap.GetValue(value, (int)WebExtensionBindingType.Default);
        }

        internal static string WebExtensionBindingTypeToDocx(WebExtensionBindingType bindingType)
        {
            return gWebExtensionBindingTypeMap.GetValue((int)bindingType, "matrix");
        }

        //JAVA: declarations moved here to exclude java's illegal forward reference.

        private static readonly StringToIntBidirectionalMap gNumberStyleMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gFontFamilyMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gLineStyleMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gStyleTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gFootnoteSeparatorTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gEndnoteSeparatorTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gDocPartBehaviorMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gDocPartGalleryMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gDocPartTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gCalendarTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gSdtLockTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gSdtDateStorageFormatMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gSdtAppearanceMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gMathJustificationMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gMathScriptMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gMathStyleMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gMathPositionMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gMathDelimiterShapeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gMathVerticalJustificationTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gMathFractionTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gMathLimitLocationMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gAbsolutePositionTabAlignmentMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gAbsolutePositionTabRelativeToMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gAbsolutePositionTabLeaderCharMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gContactIdentityProviderMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTaskPaneDockStateMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gWebExtensionStoreTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gWebExtensionBindingTypeMap = new StringToIntBidirectionalMap();

        static DocxEnum()
        {
            // It seems that all border styles that are available via Borders and Shading menu in MS Word 2003 are here.
            gLineStyleMap.AddEntry("none", (int)LineStyle.None);
            gLineStyleMap.AddEntry("single", (int)LineStyle.Single);
            gLineStyleMap.AddEntry("thick", (int)LineStyle.Thick);
            gLineStyleMap.AddEntry("double", (int)LineStyle.Double);
            gLineStyleMap.AddEntry("dotted", (int)LineStyle.Dot);
            gLineStyleMap.AddEntry("dashed", (int)LineStyle.DashLargeGap);
            gLineStyleMap.AddEntry("dotDash", (int)LineStyle.DotDash);
            gLineStyleMap.AddEntry("dotDotDash", (int)LineStyle.DotDotDash);
            gLineStyleMap.AddEntry("triple", (int)LineStyle.Triple);
            gLineStyleMap.AddEntry("thinThickSmallGap", (int)LineStyle.ThinThickSmallGap);
            gLineStyleMap.AddEntry("thickThinSmallGap", (int)LineStyle.ThickThinSmallGap);
            gLineStyleMap.AddEntry("thinThickThinSmallGap", (int)LineStyle.ThinThickThinSmallGap);
            gLineStyleMap.AddEntry("thinThickMediumGap", (int)LineStyle.ThinThickMediumGap);
            gLineStyleMap.AddEntry("thickThinMediumGap", (int)LineStyle.ThickThinMediumGap);
            gLineStyleMap.AddEntry("thinThickThinMediumGap", (int)LineStyle.ThinThickThinMediumGap);
            gLineStyleMap.AddEntry("thinThickLargeGap", (int)LineStyle.ThinThickLargeGap);
            gLineStyleMap.AddEntry("thickThinLargeGap", (int)LineStyle.ThickThinLargeGap);
            gLineStyleMap.AddEntry("thinThickThinLargeGap", (int)LineStyle.ThinThickThinLargeGap);
            gLineStyleMap.AddEntry("wave", (int)LineStyle.Wave);
            gLineStyleMap.AddEntry("doubleWave", (int)LineStyle.DoubleWave);
            gLineStyleMap.AddEntry("dashSmallGap", (int)LineStyle.DashSmallGap);
            gLineStyleMap.AddEntry("dashDotStroked", (int)LineStyle.DashDotStroker);
            gLineStyleMap.AddEntry("threeDEmboss", (int)LineStyle.Emboss3D);
            gLineStyleMap.AddEntry("threeDEngrave", (int)LineStyle.Engrave3D);
            gLineStyleMap.AddEntry("outset", (int)LineStyle.Outset);
            gLineStyleMap.AddEntry("inset", (int)LineStyle.Inset);
            // page border art
            gLineStyleMap.AddEntry("apples", (int)PageBorderArt.Apples);
            gLineStyleMap.AddEntry("archedScallops", (int)PageBorderArt.ArchedScallops);
            gLineStyleMap.AddEntry("babyPacifier", (int)PageBorderArt.BabyPacifier);
            gLineStyleMap.AddEntry("babyRattle", (int)PageBorderArt.BabyRattle);
            gLineStyleMap.AddEntry("balloons3Colors", (int)PageBorderArt.Balloons3Colors);
            gLineStyleMap.AddEntry("balloonsHotAir", (int)PageBorderArt.BalloonsHotAir);
            gLineStyleMap.AddEntry("basicBlackDashes", (int)PageBorderArt.BasicBlackDashes);
            gLineStyleMap.AddEntry("basicBlackDots", (int)PageBorderArt.BasicBlackDots);
            gLineStyleMap.AddEntry("basicBlackSquares", (int)PageBorderArt.BasicBlackSquares);
            gLineStyleMap.AddEntry("basicThinLines", (int)PageBorderArt.BasicThinLines);
            gLineStyleMap.AddEntry("basicWhiteDashes", (int)PageBorderArt.BasicWhiteDashes);
            gLineStyleMap.AddEntry("basicWhiteDots", (int)PageBorderArt.BasicWhiteDots);
            gLineStyleMap.AddEntry("basicWhiteSquares", (int)PageBorderArt.BasicWhiteSquares);
            gLineStyleMap.AddEntry("basicWideInline", (int)PageBorderArt.BasicWideInline);
            gLineStyleMap.AddEntry("basicWideMidline", (int)PageBorderArt.BasicWideMidline);
            gLineStyleMap.AddEntry("basicWideOutline", (int)PageBorderArt.BasicWideOutline);
            gLineStyleMap.AddEntry("bats", (int)PageBorderArt.Bats);
            gLineStyleMap.AddEntry("birds", (int)PageBorderArt.Birds);
            gLineStyleMap.AddEntry("birdsFlight", (int)PageBorderArt.BirdsFlight);
            gLineStyleMap.AddEntry("cabins", (int)PageBorderArt.Cabins);
            gLineStyleMap.AddEntry("cakeSlice", (int)PageBorderArt.CakeSlice);
            gLineStyleMap.AddEntry("candyCorn", (int)PageBorderArt.CandyCorn);
            gLineStyleMap.AddEntry("celticKnotwork", (int)PageBorderArt.CelticKnotwork);
            gLineStyleMap.AddEntry("certificateBanner", (int)PageBorderArt.CertificateBanner);
            gLineStyleMap.AddEntry("chainLink", (int)PageBorderArt.ChainLink);
            gLineStyleMap.AddEntry("champagneBottle", (int)PageBorderArt.ChampagneBottle);
            gLineStyleMap.AddEntry("checkedBarBlack", (int)PageBorderArt.CheckedBarBlack);
            gLineStyleMap.AddEntry("checkedBarColor", (int)PageBorderArt.CheckedBarColor);
            gLineStyleMap.AddEntry("checkered", (int)PageBorderArt.Checkered);
            gLineStyleMap.AddEntry("christmasTree", (int)PageBorderArt.ChristmasTree);
            gLineStyleMap.AddEntry("circlesLines", (int)PageBorderArt.CirclesLines);
            gLineStyleMap.AddEntry("circlesRectangles", (int)PageBorderArt.CirclesRectangles);
            gLineStyleMap.AddEntry("classicalWave", (int)PageBorderArt.ClassicalWave);
            gLineStyleMap.AddEntry("clocks", (int)PageBorderArt.Clocks);
            gLineStyleMap.AddEntry("compass", (int)PageBorderArt.Compass);
            gLineStyleMap.AddEntry("confetti", (int)PageBorderArt.Confetti);
            gLineStyleMap.AddEntry("confettiGrays", (int)PageBorderArt.ConfettiGrays);
            gLineStyleMap.AddEntry("confettiOutline", (int)PageBorderArt.ConfettiOutline);
            gLineStyleMap.AddEntry("confettiStreamers", (int)PageBorderArt.ConfettiStreamers);
            gLineStyleMap.AddEntry("confettiWhite", (int)PageBorderArt.ConfettiWhite);
            gLineStyleMap.AddEntry("cornerTriangles", (int)PageBorderArt.CornerTriangles);
            gLineStyleMap.AddEntry("couponCutoutDashes", (int)PageBorderArt.CouponCutoutDashes);
            gLineStyleMap.AddEntry("couponCutoutDots", (int)PageBorderArt.CouponCutoutDots);
            gLineStyleMap.AddEntry("crazyMaze", (int)PageBorderArt.CrazyMaze);
            gLineStyleMap.AddEntry("creaturesButterfly", (int)PageBorderArt.CreaturesButterfly);
            gLineStyleMap.AddEntry("creaturesFish", (int)PageBorderArt.CreaturesFish);
            gLineStyleMap.AddEntry("creaturesInsects", (int)PageBorderArt.CreaturesInsects);
            gLineStyleMap.AddEntry("creaturesLadyBug", (int)PageBorderArt.CreaturesLadyBug);
            gLineStyleMap.AddEntry("crossStitch", (int)PageBorderArt.CrossStitch);
            gLineStyleMap.AddEntry("cup", (int)PageBorderArt.Cup);
            gLineStyleMap.AddEntry("decoArch", (int)PageBorderArt.DecoArch);
            gLineStyleMap.AddEntry("decoArchColor", (int)PageBorderArt.DecoArchColor);
            gLineStyleMap.AddEntry("decoBlocks", (int)PageBorderArt.DecoBlocks);
            gLineStyleMap.AddEntry("diamondsGray", (int)PageBorderArt.DiamondsGray);
            gLineStyleMap.AddEntry("doubleD", (int)PageBorderArt.DoubleD);
            gLineStyleMap.AddEntry("doubleDiamonds", (int)PageBorderArt.DoubleDiamonds);
            gLineStyleMap.AddEntry("earth1", (int)PageBorderArt.Earth1);
            gLineStyleMap.AddEntry("earth2", (int)PageBorderArt.Earth2);
            gLineStyleMap.AddEntry("eclipsingSquares1", (int)PageBorderArt.EclipsingSquares1);
            gLineStyleMap.AddEntry("eclipsingSquares2", (int)PageBorderArt.EclipsingSquares2);
            gLineStyleMap.AddEntry("eggsBlack", (int)PageBorderArt.EggsBlack);
            gLineStyleMap.AddEntry("fans", (int)PageBorderArt.Fans);
            gLineStyleMap.AddEntry("film", (int)PageBorderArt.Film);
            gLineStyleMap.AddEntry("firecrackers", (int)PageBorderArt.Firecrackers);
            gLineStyleMap.AddEntry("flowersBlockPrint", (int)PageBorderArt.FlowersBlockPrint);
            gLineStyleMap.AddEntry("flowersDaisies", (int)PageBorderArt.FlowersDaisies);
            gLineStyleMap.AddEntry("flowersModern1", (int)PageBorderArt.FlowersModern1);
            gLineStyleMap.AddEntry("flowersModern2", (int)PageBorderArt.FlowersModern2);
            gLineStyleMap.AddEntry("flowersPansy", (int)PageBorderArt.FlowersPansy);
            gLineStyleMap.AddEntry("flowersRedRose", (int)PageBorderArt.FlowersRedRose);
            gLineStyleMap.AddEntry("flowersRoses", (int)PageBorderArt.FlowersRoses);
            gLineStyleMap.AddEntry("flowersTeacup", (int)PageBorderArt.FlowersTeacup);
            gLineStyleMap.AddEntry("flowersTiny", (int)PageBorderArt.FlowersTiny);
            gLineStyleMap.AddEntry("gems", (int)PageBorderArt.Gems);
            gLineStyleMap.AddEntry("gingerbreadMan", (int)PageBorderArt.GingerbreadMan);
            gLineStyleMap.AddEntry("gradient", (int)PageBorderArt.Gradient);
            gLineStyleMap.AddEntry("handmade1", (int)PageBorderArt.Handmade1);
            gLineStyleMap.AddEntry("handmade2", (int)PageBorderArt.Handmade2);
            gLineStyleMap.AddEntry("heartBalloon", (int)PageBorderArt.HeartBalloon);
            gLineStyleMap.AddEntry("heartGray", (int)PageBorderArt.HeartGray);
            gLineStyleMap.AddEntry("hearts", (int)PageBorderArt.Hearts);
            gLineStyleMap.AddEntry("heebieJeebies", (int)PageBorderArt.HeebieJeebies);
            gLineStyleMap.AddEntry("holly", (int)PageBorderArt.Holly);
            gLineStyleMap.AddEntry("houseFunky", (int)PageBorderArt.HouseFunky);
            gLineStyleMap.AddEntry("hypnotic", (int)PageBorderArt.Hypnotic);
            gLineStyleMap.AddEntry("iceCreamCones", (int)PageBorderArt.IceCreamCones);
            gLineStyleMap.AddEntry("lightBulb", (int)PageBorderArt.LightBulb);
            gLineStyleMap.AddEntry("lightning1", (int)PageBorderArt.Lightning1);
            gLineStyleMap.AddEntry("lightning2", (int)PageBorderArt.Lightning2);
            gLineStyleMap.AddEntry("mapPins", (int)PageBorderArt.MapPins);
            gLineStyleMap.AddEntry("mapleLeaf", (int)PageBorderArt.MapleLeaf);
            gLineStyleMap.AddEntry("mapleMuffins", (int)PageBorderArt.MapleMuffins);
            gLineStyleMap.AddEntry("marquee", (int)PageBorderArt.Marquee);
            gLineStyleMap.AddEntry("marqueeToothed", (int)PageBorderArt.MarqueeToothed);
            gLineStyleMap.AddEntry("moons", (int)PageBorderArt.Moons);
            gLineStyleMap.AddEntry("mosaic", (int)PageBorderArt.Mosaic);
            gLineStyleMap.AddEntry("musicNotes", (int)PageBorderArt.MusicNotes);
            gLineStyleMap.AddEntry("northwest", (int)PageBorderArt.Northwest);
            gLineStyleMap.AddEntry("ovals", (int)PageBorderArt.Ovals);
            gLineStyleMap.AddEntry("packages", (int)PageBorderArt.Packages);
            gLineStyleMap.AddEntry("palmsBlack", (int)PageBorderArt.PalmsBlack);
            gLineStyleMap.AddEntry("palmsColor", (int)PageBorderArt.PalmsColor);
            gLineStyleMap.AddEntry("paperClips", (int)PageBorderArt.PaperClips);
            gLineStyleMap.AddEntry("papyrus", (int)PageBorderArt.Papyrus);
            gLineStyleMap.AddEntry("partyFavor", (int)PageBorderArt.PartyFavor);
            gLineStyleMap.AddEntry("partyGlass", (int)PageBorderArt.PartyGlass);
            gLineStyleMap.AddEntry("pencils", (int)PageBorderArt.Pencils);
            gLineStyleMap.AddEntry("people", (int)PageBorderArt.People);
            gLineStyleMap.AddEntry("peopleWaving", (int)PageBorderArt.PeopleWaving);
            gLineStyleMap.AddEntry("peopleHats", (int)PageBorderArt.PeopleHats);
            gLineStyleMap.AddEntry("poinsettias", (int)PageBorderArt.Poinsettias);
            gLineStyleMap.AddEntry("postageStamp", (int)PageBorderArt.PostageStamp);
            gLineStyleMap.AddEntry("pumpkin1", (int)PageBorderArt.Pumpkin1);
            gLineStyleMap.AddEntry("pushPinNote1", (int)PageBorderArt.PushPinNote1);
            gLineStyleMap.AddEntry("pushPinNote2", (int)PageBorderArt.PushPinNote2);
            gLineStyleMap.AddEntry("pyramids", (int)PageBorderArt.Pyramids);
            gLineStyleMap.AddEntry("pyramidsAbove", (int)PageBorderArt.PyramidsAbove);
            gLineStyleMap.AddEntry("quadrants", (int)PageBorderArt.Quadrants);
            gLineStyleMap.AddEntry("rings", (int)PageBorderArt.Rings);
            gLineStyleMap.AddEntry("safari", (int)PageBorderArt.Safari);
            gLineStyleMap.AddEntry("sawtooth", (int)PageBorderArt.Sawtooth);
            gLineStyleMap.AddEntry("sawtoothGray", (int)PageBorderArt.SawtoothGray);
            gLineStyleMap.AddEntry("scaredCat", (int)PageBorderArt.ScaredCat);
            gLineStyleMap.AddEntry("seattle", (int)PageBorderArt.Seattle);
            gLineStyleMap.AddEntry("shadowedSquares", (int)PageBorderArt.ShadowedSquares);
            gLineStyleMap.AddEntry("sharksTeeth", (int)PageBorderArt.SharksTeeth);
            gLineStyleMap.AddEntry("shorebirdTracks", (int)PageBorderArt.ShorebirdTracks);
            gLineStyleMap.AddEntry("skyrocket", (int)PageBorderArt.Skyrocket);
            gLineStyleMap.AddEntry("snowflakeFancy", (int)PageBorderArt.SnowflakeFancy);
            gLineStyleMap.AddEntry("snowflakes", (int)PageBorderArt.Snowflakes);
            gLineStyleMap.AddEntry("sombrero", (int)PageBorderArt.Sombrero);
            gLineStyleMap.AddEntry("southwest", (int)PageBorderArt.Southwest);
            gLineStyleMap.AddEntry("stars", (int)PageBorderArt.Stars);
            gLineStyleMap.AddEntry("starsTop", (int)PageBorderArt.StarsTop);
            gLineStyleMap.AddEntry("stars3d", (int)PageBorderArt.Stars3D);
            gLineStyleMap.AddEntry("starsBlack", (int)PageBorderArt.StarsBlack);
            gLineStyleMap.AddEntry("starsShadowed", (int)PageBorderArt.StarsShadowed);
            gLineStyleMap.AddEntry("sun", (int)PageBorderArt.Sun);
            gLineStyleMap.AddEntry("swirligig", (int)PageBorderArt.Swirligig);
            gLineStyleMap.AddEntry("tornPaper", (int)PageBorderArt.TornPaper);
            gLineStyleMap.AddEntry("tornPaperBlack", (int)PageBorderArt.TornPaperBlack);
            gLineStyleMap.AddEntry("trees", (int)PageBorderArt.Trees);
            gLineStyleMap.AddEntry("triangleParty", (int)PageBorderArt.TriangleParty);
            gLineStyleMap.AddEntry("triangles", (int)PageBorderArt.Triangles);
            gLineStyleMap.AddEntry("tribal1", (int)PageBorderArt.Tribal1);
            gLineStyleMap.AddEntry("tribal2", (int)PageBorderArt.Tribal2);
            gLineStyleMap.AddEntry("tribal3", (int)PageBorderArt.Tribal3);
            gLineStyleMap.AddEntry("tribal4", (int)PageBorderArt.Tribal4);
            gLineStyleMap.AddEntry("tribal5", (int)PageBorderArt.Tribal5);
            gLineStyleMap.AddEntry("tribal6", (int)PageBorderArt.Tribal6);
            gLineStyleMap.AddEntry("twistedLines1", (int)PageBorderArt.TwistedLines1);
            gLineStyleMap.AddEntry("twistedLines2", (int)PageBorderArt.TwistedLines2);
            gLineStyleMap.AddEntry("vine", (int)PageBorderArt.Vine);
            gLineStyleMap.AddEntry("waveline", (int)PageBorderArt.Waveline);
            gLineStyleMap.AddEntry("weavingAngles", (int)PageBorderArt.WeavingAngles);
            gLineStyleMap.AddEntry("weavingBraid", (int)PageBorderArt.WeavingBraid);
            gLineStyleMap.AddEntry("weavingRibbon", (int)PageBorderArt.WeavingRibbon);
            gLineStyleMap.AddEntry("weavingStrips", (int)PageBorderArt.WeavingStrips);
            gLineStyleMap.AddEntry("whiteFlowers", (int)PageBorderArt.WhiteFlowers);
            gLineStyleMap.AddEntry("woodwork", (int)PageBorderArt.Woodwork);
            gLineStyleMap.AddEntry("xIllusions", (int)PageBorderArt.XIllusions);
            gLineStyleMap.AddEntry("zanyTriangles", (int)PageBorderArt.ZanyTriangles);
            gLineStyleMap.AddEntry("zigZag", (int)PageBorderArt.ZigZag);
            gLineStyleMap.AddEntry("zigZagStitch", (int)PageBorderArt.ZigZagStitch);
            gLineStyleMap.AddEntry("earth3", (int)PageBorderArt.Earth3);
            gLineStyleMap.AddEntry("triangle1", (int)PageBorderArt.Triangle1);
            gLineStyleMap.AddEntry("triangle2", (int)PageBorderArt.Triangle2);
            gLineStyleMap.AddEntry("triangleCircle1", (int)PageBorderArt.TriangleCircle1);
            gLineStyleMap.AddEntry("triangleCircle2", (int)PageBorderArt.TriangleCircle2);
            gLineStyleMap.AddEntry("shapes1", (int)PageBorderArt.Shapes1);
            gLineStyleMap.AddEntry("shapes2", (int)PageBorderArt.Shapes2);
            gLineStyleMap.AddEntry("custom", (int)PageBorderArt. Custom);

            gNumberStyleMap.AddEntry("decimal", (int)NumberStyle.Arabic);
            gNumberStyleMap.AddEntry("upperRoman", (int)NumberStyle.UppercaseRoman);
            gNumberStyleMap.AddEntry("lowerRoman", (int)NumberStyle.LowercaseRoman);
            gNumberStyleMap.AddEntry("upperLetter", (int)NumberStyle.UppercaseLetter);
            gNumberStyleMap.AddEntry("lowerLetter", (int)NumberStyle.LowercaseLetter);
            gNumberStyleMap.AddEntry("ordinal", (int)NumberStyle.Ordinal);
            gNumberStyleMap.AddEntry("cardinalText", (int)NumberStyle.Number);
            gNumberStyleMap.AddEntry("ordinalText", (int)NumberStyle.OrdinalText);
            gNumberStyleMap.AddEntry("hex", (int)NumberStyle.Hex);
            gNumberStyleMap.AddEntry("chicago", (int)NumberStyle.ChicagoManual);
            gNumberStyleMap.AddEntry("ideographDigital", (int)NumberStyle.Kanji);
            gNumberStyleMap.AddEntry("japaneseCounting", (int)NumberStyle.KanjiDigit);
            gNumberStyleMap.AddEntry("aiueo", (int)NumberStyle.AiueoHalfWidth);
            gNumberStyleMap.AddEntry("iroha", (int)NumberStyle.IrohaHalfWidth);
            gNumberStyleMap.AddEntry("decimalFullWidth", (int)NumberStyle.ArabicFullWidth);
            gNumberStyleMap.AddEntry("decimalHalfWidth", (int)NumberStyle.ArabicHalfWidth);
            gNumberStyleMap.AddEntry("japaneseLegal", (int)NumberStyle.KanjiTraditional);
            gNumberStyleMap.AddEntry("japaneseDigitalTenThousand", (int)NumberStyle.KanjiTraditional2);
            gNumberStyleMap.AddEntry("decimalEnclosedCircle", (int)NumberStyle.NumberInCircle);
            gNumberStyleMap.AddEntry("decimalFullWidth2", (int)NumberStyle.DecimalFullWidth);
            gNumberStyleMap.AddEntry("aiueoFullWidth", (int)NumberStyle.Aiueo);
            gNumberStyleMap.AddEntry("irohaFullWidth", (int)NumberStyle.Iroha);
            gNumberStyleMap.AddEntry("decimalZero", (int)NumberStyle.LeadingZero);
            gNumberStyleMap.AddEntry("bullet", (int)NumberStyle.Bullet);
            gNumberStyleMap.AddEntry("ganada", (int)NumberStyle.Ganada);
            gNumberStyleMap.AddEntry("chosung", (int)NumberStyle.Chosung);
            gNumberStyleMap.AddEntry("decimalEnclosedFullstop", (int)NumberStyle.GB1);
            gNumberStyleMap.AddEntry("decimalEnclosedParen", (int)NumberStyle.GB2);
            gNumberStyleMap.AddEntry("decimalEnclosedCircleChinese", (int)NumberStyle.GB3);
            gNumberStyleMap.AddEntry("ideographEnclosedCircle", (int)NumberStyle.GB4);
            gNumberStyleMap.AddEntry("ideographTraditional", (int)NumberStyle.Zodiac1);
            gNumberStyleMap.AddEntry("ideographZodiac", (int)NumberStyle.Zodiac2);
            gNumberStyleMap.AddEntry("ideographZodiacTraditional", (int)NumberStyle.Zodiac3);
            gNumberStyleMap.AddEntry("taiwaneseCounting", (int)NumberStyle.TradChinNum1);
            gNumberStyleMap.AddEntry("ideographLegalTraditional", (int)NumberStyle.TradChinNum2);
            gNumberStyleMap.AddEntry("taiwaneseCountingThousand", (int)NumberStyle.TradChinNum3);
            gNumberStyleMap.AddEntry("taiwaneseDigital", (int)NumberStyle.TradChinNum4);
            gNumberStyleMap.AddEntry("chineseCounting", (int)NumberStyle.SimpChinNum1);
            gNumberStyleMap.AddEntry("chineseLegalSimplified", (int)NumberStyle.SimpChinNum2);
            gNumberStyleMap.AddEntry("chineseCountingThousand", (int)NumberStyle.SimpChinNum3);
            gNumberStyleMap.AddEntry("koreanDigital", (int)NumberStyle.HanjaRead);
            gNumberStyleMap.AddEntry("koreanCounting", (int)NumberStyle.HanjaReadDigit);
            gNumberStyleMap.AddEntry("koreanLegal", (int)NumberStyle.Hangul);
            gNumberStyleMap.AddEntry("koreanDigital2", (int)NumberStyle.Hanja);
            gNumberStyleMap.AddEntry("vietnameseCounting", (int)NumberStyle.VietCardinalText);
            gNumberStyleMap.AddEntry("russianLower", (int)NumberStyle.LowercaseRussian);
            gNumberStyleMap.AddEntry("russianUpper", (int)NumberStyle.UppercaseRussian);
            gNumberStyleMap.AddEntry("none", (int)NumberStyle.None);
            gNumberStyleMap.AddEntry("numberInDash", (int)NumberStyle.NumberInDash);
            gNumberStyleMap.AddEntry("hebrew1", (int)NumberStyle.Hebrew1);
            gNumberStyleMap.AddEntry("hebrew2", (int)NumberStyle.Hebrew2);
            gNumberStyleMap.AddEntry("arabicAlpha", (int)NumberStyle.Arabic1);
            gNumberStyleMap.AddEntry("arabicAbjad", (int)NumberStyle.Arabic2);
            gNumberStyleMap.AddEntry("hindiVowels", (int)NumberStyle.HindiLetter1);
            gNumberStyleMap.AddEntry("hindiConsonants", (int)NumberStyle.HindiLetter2);
            gNumberStyleMap.AddEntry("hindiNumbers", (int)NumberStyle.HindiArabic);
            gNumberStyleMap.AddEntry("hindiCounting", (int)NumberStyle.HindiCardinalText);
            gNumberStyleMap.AddEntry("thaiLetters", (int)NumberStyle.ThaiLetter);
            gNumberStyleMap.AddEntry("thaiNumbers", (int)NumberStyle.ThaiArabic);
            gNumberStyleMap.AddEntry("thaiCounting", (int)NumberStyle.ThaiCardinalText);
            gNumberStyleMap.AddEntry("custom", (int)NumberStyle.Custom);

            gStyleTypeMap.AddEntry("paragraph", (int)StyleType.Paragraph);
            gStyleTypeMap.AddEntry("character", (int)StyleType.Character);
            gStyleTypeMap.AddEntry("table", (int)StyleType.Table);
            gStyleTypeMap.AddEntry("numbering", (int)StyleType.List);

            gFontFamilyMap.AddEntry("auto", (int)FontFamily.Auto);
            gFontFamilyMap.AddEntry("decorative", (int)FontFamily.Decorative);
            gFontFamilyMap.AddEntry("modern", (int)FontFamily.Modern);
            gFontFamilyMap.AddEntry("roman", (int)FontFamily.Roman);
            gFontFamilyMap.AddEntry("script", (int)FontFamily.Script);
            gFontFamilyMap.AddEntry("swiss", (int)FontFamily.Swiss);

            gFootnoteSeparatorTypeMap.AddEntry("normal", -1);   // FootnoteSeparatorType.Normal.
            gFootnoteSeparatorTypeMap.AddEntry("separator", (int)FootnoteSeparatorType.FootnoteSeparator);
            gFootnoteSeparatorTypeMap.AddEntry("continuationSeparator", (int)FootnoteSeparatorType.FootnoteContinuationSeparator);
            gFootnoteSeparatorTypeMap.AddEntry("continuationNotice", (int)FootnoteSeparatorType.FootnoteContinuationNotice);

            gEndnoteSeparatorTypeMap.AddEntry("normal", -1);    // FootnoteSeparatorType.Normal.
            gEndnoteSeparatorTypeMap.AddEntry("separator", (int)FootnoteSeparatorType.EndnoteSeparator);
            gEndnoteSeparatorTypeMap.AddEntry("continuationSeparator", (int)FootnoteSeparatorType.EndnoteContinuationSeparator);
            gEndnoteSeparatorTypeMap.AddEntry("continuationNotice", (int)FootnoteSeparatorType.EndnoteContinuationNotice);

            gDocPartBehaviorMap.AddEntry("content", (int)BuildingBlockBehavior.Content);
            gDocPartBehaviorMap.AddEntry("p", (int)BuildingBlockBehavior.Paragraph);
            gDocPartBehaviorMap.AddEntry("pg", (int)BuildingBlockBehavior.Page);

            gDocPartGalleryMap.AddEntry("any", (int)BuildingBlockGallery.All);
            gDocPartGalleryMap.AddEntry("autoTxt", (int)BuildingBlockGallery.AutoText);
            gDocPartGalleryMap.AddEntry("bib", (int)BuildingBlockGallery.Bibliography);
            gDocPartGalleryMap.AddEntry("coverPg", (int)BuildingBlockGallery.CoverPage);
            gDocPartGalleryMap.AddEntry("custAutoTxt", (int)BuildingBlockGallery.CustomAutoText);
            gDocPartGalleryMap.AddEntry("custBib", (int)BuildingBlockGallery.CustomBibliography);
            gDocPartGalleryMap.AddEntry("custCoverPg", (int)BuildingBlockGallery.CustomCoverPage);
            gDocPartGalleryMap.AddEntry("custEq", (int)BuildingBlockGallery.CustomEquations);
            gDocPartGalleryMap.AddEntry("custFtrs", (int)BuildingBlockGallery.CustomFooters);
            gDocPartGalleryMap.AddEntry("custHdrs", (int)BuildingBlockGallery.CustomHeaders);
            gDocPartGalleryMap.AddEntry("custom1", (int)BuildingBlockGallery.Custom1);
            gDocPartGalleryMap.AddEntry("custom2", (int)BuildingBlockGallery.Custom2);
            gDocPartGalleryMap.AddEntry("custom3", (int)BuildingBlockGallery.Custom3);
            gDocPartGalleryMap.AddEntry("custom4", (int)BuildingBlockGallery.Custom4);
            gDocPartGalleryMap.AddEntry("custom5", (int)BuildingBlockGallery.Custom5);
            gDocPartGalleryMap.AddEntry("custPgNum", (int)BuildingBlockGallery.CustomPageNumber);
            gDocPartGalleryMap.AddEntry("custPgNumB", (int)BuildingBlockGallery.CustomPageNumberAtBottom);
            gDocPartGalleryMap.AddEntry("custPgNumMargins", (int)BuildingBlockGallery.CustomPageNumberAtMargin);
            gDocPartGalleryMap.AddEntry("custPgNumT", (int)BuildingBlockGallery.CustomPageNumberAtTop);
            gDocPartGalleryMap.AddEntry("custQuickParts", (int)BuildingBlockGallery.CustomQuickParts);
            gDocPartGalleryMap.AddEntry("custTblOfContents", (int)BuildingBlockGallery.CustomTableOfContents);
            gDocPartGalleryMap.AddEntry("custTbls", (int)BuildingBlockGallery.CustomTables);
            gDocPartGalleryMap.AddEntry("custTxtBox", (int)BuildingBlockGallery.CustomTextBox);
            gDocPartGalleryMap.AddEntry("custWatermarks", (int)BuildingBlockGallery.CustomWatermarks);
            gDocPartGalleryMap.AddEntry("default", (int)BuildingBlockGallery.NoGallery);
            gDocPartGalleryMap.AddEntry("docParts", (int)BuildingBlockGallery.QuickParts);
            gDocPartGalleryMap.AddEntry("eq", (int)BuildingBlockGallery.Equations);
            gDocPartGalleryMap.AddEntry("ftrs", (int)BuildingBlockGallery.Footers);
            gDocPartGalleryMap.AddEntry("hdrs", (int)BuildingBlockGallery.Headers);
            gDocPartGalleryMap.AddEntry("pgNum", (int)BuildingBlockGallery.PageNumber);
            gDocPartGalleryMap.AddEntry("pgNumB", (int)BuildingBlockGallery.PageNumberAtBottom);
            gDocPartGalleryMap.AddEntry("pgNumMargins", (int)BuildingBlockGallery.PageNumberAtMargin);
            gDocPartGalleryMap.AddEntry("pgNumT", (int)BuildingBlockGallery.PageNumberAtTop);
            gDocPartGalleryMap.AddEntry("placeholder", (int)BuildingBlockGallery.StructuredDocumentTagPlaceholderText);
            gDocPartGalleryMap.AddEntry("tblOfContents", (int)BuildingBlockGallery.TableOfContents);
            gDocPartGalleryMap.AddEntry("tbls", (int)BuildingBlockGallery.Tables);
            gDocPartGalleryMap.AddEntry("txtBox", (int)BuildingBlockGallery.TextBox);
            gDocPartGalleryMap.AddEntry("watermarks", (int)BuildingBlockGallery.Watermarks);

            gDocPartTypeMap.AddEntry("autoExp", (int)BuildingBlockType.AutomaticallyReplaceNameWithContent);
            gDocPartTypeMap.AddEntry("bbPlcHdr", (int)BuildingBlockType.StructuredDocumentTagPlaceholderText);
            gDocPartTypeMap.AddEntry("formFld", (int)BuildingBlockType.FormFieldHelpText);
            gDocPartTypeMap.AddEntry("none", (int)BuildingBlockType.None);
            gDocPartTypeMap.AddEntry("normal", (int)BuildingBlockType.Normal);
            gDocPartTypeMap.AddEntry("speller", (int)BuildingBlockType.AutoCorrect);
            gDocPartTypeMap.AddEntry("toolbar", (int)BuildingBlockType.AutoText);

            gCalendarTypeMap.AddEntry("gregorian", (int)SdtCalendarType.Gregorian);
            gCalendarTypeMap.AddEntry("gregorianArabic", (int)SdtCalendarType.GregorianArabic);
            gCalendarTypeMap.AddEntry("gregorianMeFrench", (int)SdtCalendarType.GregorianMeFrench);
            gCalendarTypeMap.AddEntry("gregorianUs", (int)SdtCalendarType.GregorianUs);
            gCalendarTypeMap.AddEntry("gregorianXlitEnglish", (int)SdtCalendarType.GregorianXlitEnglish);
            gCalendarTypeMap.AddEntry("gregorianXlitFrench", (int)SdtCalendarType.GregorianXlitFrench);
            gCalendarTypeMap.AddEntry("hebrew", (int)SdtCalendarType.Hebrew);
            gCalendarTypeMap.AddEntry("hijri", (int)SdtCalendarType.Hijri);
            gCalendarTypeMap.AddEntry("japan", (int)SdtCalendarType.Japan);
            gCalendarTypeMap.AddEntry("korea", (int)SdtCalendarType.Korea);
            gCalendarTypeMap.AddEntry("none", (int)SdtCalendarType.None);
            gCalendarTypeMap.AddEntry("saka", (int)SdtCalendarType.Saka);
            gCalendarTypeMap.AddEntry("taiwan", (int)SdtCalendarType.Taiwan);
            gCalendarTypeMap.AddEntry("thai", (int)SdtCalendarType.Thai);

            gSdtLockTypeMap.AddEntry("contentLocked", (int)SdtLockType.ContentLocked);
            gSdtLockTypeMap.AddEntry("sdtContentLocked", (int)SdtLockType.SdtAndContentLocked);
            gSdtLockTypeMap.AddEntry("sdtLocked", (int)SdtLockType.SdtLocked);
            gSdtLockTypeMap.AddEntry("unlocked", (int)SdtLockType.Unlocked);

            gSdtDateStorageFormatMap.AddEntry("date", (int)SdtDateStorageFormat.Date);
            gSdtDateStorageFormatMap.AddEntry("dateTime", (int)SdtDateStorageFormat.DateTime);
            gSdtDateStorageFormatMap.AddEntry("text", (int)SdtDateStorageFormat.Text);

            gSdtAppearanceMap.AddEntry("boundingBox", (int)SdtAppearance.BoundingBox);
            gSdtAppearanceMap.AddEntry("tags", (int)SdtAppearance.Tags);
            gSdtAppearanceMap.AddEntry("hidden", (int)SdtAppearance.Hidden);

            gMathJustificationMap.AddEntry("center", (int)OfficeMathJustification.Center);
            gMathJustificationMap.AddEntry("centerGroup", (int)OfficeMathJustification.CenterGroup);
            gMathJustificationMap.AddEntry("left", (int)OfficeMathJustification.Left);
            gMathJustificationMap.AddEntry("right", (int)OfficeMathJustification.Right);

            gMathScriptMap.AddEntry("double-struck", (int)MathScript.DoubleStruck);
            gMathScriptMap.AddEntry("fraktur", (int)MathScript.Fraktur);
            gMathScriptMap.AddEntry("monospace", (int)MathScript.Monospace);
            gMathScriptMap.AddEntry("roman", (int)MathScript.Roman);
            gMathScriptMap.AddEntry("sans-serif", (int)MathScript.SansSerif);
            gMathScriptMap.AddEntry("script", (int)MathScript.Script);

            gMathStyleMap.AddEntry("p", (int)MathStyle.Plain);
            gMathStyleMap.AddEntry("b", (int)MathStyle.Bold);
            gMathStyleMap.AddEntry("i", (int)MathStyle.Italic);
            gMathStyleMap.AddEntry("bi", (int)MathStyle.BoldItalic);

            gMathPositionMap.AddEntry("top", (int)MathPosition.Top);
            gMathPositionMap.AddEntry("bot", (int)MathPosition.Bottom);

            gMathDelimiterShapeMap.AddEntry("centered", (int)MathDelimiterShape.Centered);
            gMathDelimiterShapeMap.AddEntry("match", (int)MathDelimiterShape.Match);

            gMathVerticalJustificationTypeMap.AddEntry("top", (int)MathVerticalJustification.Top);
            gMathVerticalJustificationTypeMap.AddEntry("bot", (int)MathVerticalJustification.Bottom);

            gMathFractionTypeMap.AddEntry("bar", (int)MathFractionType.Bar);
            gMathFractionTypeMap.AddEntry("lin", (int)MathFractionType.Linear);
            gMathFractionTypeMap.AddEntry("noBar", (int)MathFractionType.NoBar);
            gMathFractionTypeMap.AddEntry("skw", (int)MathFractionType.Skewed);

            gMathLimitLocationMap.AddEntry("subSup", (int)MathLimitLocation.SubscriptSuperscript);
            gMathLimitLocationMap.AddEntry("undOvr", (int)MathLimitLocation.UnderOver);

            gAbsolutePositionTabAlignmentMap.AddEntry("center", (int)AbsolutePositionTabAlignment.Center);
            gAbsolutePositionTabAlignmentMap.AddEntry("left", (int)AbsolutePositionTabAlignment.Left);
            gAbsolutePositionTabAlignmentMap.AddEntry("right", (int)AbsolutePositionTabAlignment.Right);

            gAbsolutePositionTabLeaderCharMap.AddEntry("dot", (int)AbsolutePositionTabLeaderChar.Dot);
            gAbsolutePositionTabLeaderCharMap.AddEntry("hyphen", (int)AbsolutePositionTabLeaderChar.Hyphen);
            gAbsolutePositionTabLeaderCharMap.AddEntry("middleDot", (int)AbsolutePositionTabLeaderChar.MiddleDot);
            gAbsolutePositionTabLeaderCharMap.AddEntry("none", (int)AbsolutePositionTabLeaderChar.None);
            gAbsolutePositionTabLeaderCharMap.AddEntry("underscore", (int)AbsolutePositionTabLeaderChar.Underscore);

            gAbsolutePositionTabRelativeToMap.AddEntry("indent", (int)AbsolutePositionTabPositioningBase.Indent);
            gAbsolutePositionTabRelativeToMap.AddEntry("margin", (int)AbsolutePositionTabPositioningBase.Margin);

            gContactIdentityProviderMap.AddEntry("None", (int)ContactIdentityProvider.None);
            gContactIdentityProviderMap.AddEntry("AD", (int)ContactIdentityProvider.ActiveDirectory);
            gContactIdentityProviderMap.AddEntry("Windows Live", (int)ContactIdentityProvider.WindowsLiveId);

            gTaskPaneDockStateMap.AddEntry("right", (int)TaskPaneDockState.Right);
            gTaskPaneDockStateMap.AddEntry("left", (int)TaskPaneDockState.Left);

            gWebExtensionStoreTypeMap.AddEntry("omex", (int)WebExtensionStoreType.OMEX);
            gWebExtensionStoreTypeMap.AddEntry("spapp", (int)WebExtensionStoreType.SPApp);
            gWebExtensionStoreTypeMap.AddEntry("registry", (int)WebExtensionStoreType.Registry);
            gWebExtensionStoreTypeMap.AddEntry("exchange", (int)WebExtensionStoreType.Exchange);
            gWebExtensionStoreTypeMap.AddEntry("spcatalog", (int)WebExtensionStoreType.SPCatalog);
            gWebExtensionStoreTypeMap.AddEntry("excatalog", (int)WebExtensionStoreType.ExCatalog);
            gWebExtensionStoreTypeMap.AddEntry("filesystem", (int)WebExtensionStoreType.FileSystem);

            gWebExtensionBindingTypeMap.AddEntry("text", (int)WebExtensionBindingType.Text);
            gWebExtensionBindingTypeMap.AddEntry("table", (int)WebExtensionBindingType.Table);
            gWebExtensionBindingTypeMap.AddEntry("matrix", (int)WebExtensionBindingType.Matrix);
        }
    }
}
