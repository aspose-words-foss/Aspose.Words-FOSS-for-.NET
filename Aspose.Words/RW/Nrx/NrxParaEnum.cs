// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2009 by Roman Korchagin

using Aspose.Words.Drawing;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Nrx
{
    internal static class NrxParaEnum
    {
        internal static TextboxTightWrap XmlToTextboxTightWrap(string value)
        {
            switch (value)
            {
                case "allLines": return TextboxTightWrap.AllLines;
                case "firstAndLastLine": return TextboxTightWrap.FirstAndLastLine;
                case "firstLineOnly": return TextboxTightWrap.FirstLineOnly;
                case "lastLineOnly": return TextboxTightWrap.LastLineOnly;
                case "none": return TextboxTightWrap.None;
                default: return TextboxTightWrap.Default;
            }
        }

        internal static string TextboxTightWrapToXml(TextboxTightWrap value)
        {
            switch (value)
            {
                case TextboxTightWrap.AllLines: return "allLines";
                case TextboxTightWrap.FirstAndLastLine : return "firstAndLastLine";
                case TextboxTightWrap.FirstLineOnly: return "firstLineOnly";
                case TextboxTightWrap.LastLineOnly: return "lastLineOnly";
                case TextboxTightWrap.None : return "none";
                default: return "";
            }
        }

        internal static BaselineAlignment XmlToBaselineAlignment(string value)
        {
            switch (value)
            { 
                case "top": return BaselineAlignment.Top;
                case "center": return BaselineAlignment.Center;
                case "baseline": return BaselineAlignment.Baseline;
                case "bottom": return BaselineAlignment.Bottom;
                case "auto": return BaselineAlignment.Auto;
                default: return BaselineAlignment.Auto;
            }
        }

        internal static string BaselineAlignmentToXml(BaselineAlignment value)
        {
            switch (value)
            {
                case BaselineAlignment.Top: return "top";
                case BaselineAlignment.Center: return "center";
                case BaselineAlignment.Baseline: return "baseline";
                case BaselineAlignment.Bottom: return "bottom";
                case BaselineAlignment.Auto: return "auto";
                default: return "";
            }
        }

        internal static DropCapPosition XmlToDropCapPosition(string value)
        {
            switch (value)
            {
                case "none": return DropCapPosition.None;
                case "drop": return DropCapPosition.Normal;
                case "margin": return DropCapPosition.Margin;
                default: return DropCapPosition.None;
            }
        }

        internal static string DropCapPositionToXml(DropCapPosition value)
        {
            switch (value)
            {
                case DropCapPosition.None: return "none";
                case DropCapPosition.Normal: return "drop";
                case DropCapPosition.Margin: return "margin";
                default: return "";
            }
        }

        internal static VerticalAlignment XmlToVerticalAlignment(string value)
        {
            switch (value)
            {
                case "inline": return VerticalAlignment.Inline;
                case "top": return VerticalAlignment.Top;
                case "center": return VerticalAlignment.Center;
                case "bottom": return VerticalAlignment.Bottom;
                case "inside": return VerticalAlignment.Inside;
                case "outside": return VerticalAlignment.Outside;
                default: return VerticalAlignment.None;
            }
        }

        internal static string VerticalAlignmentToXml(VerticalAlignment value)
        {
            switch (value)
            {
                case VerticalAlignment.Inline: return "inline";
                case VerticalAlignment.Top: return "top";
                case VerticalAlignment.Center: return "center";
                case VerticalAlignment.Bottom: return "bottom";
                case VerticalAlignment.Inside: return "inside";
                case VerticalAlignment.Outside: return "outside";
                default: return "";
            }
        }

        internal static HorizontalAlignment XmlToHorizontalAlignment(string value)
        {
            switch (value)
            {
                case "left": return HorizontalAlignment.Left;
                case "center": return HorizontalAlignment.Center;
                case "right": return HorizontalAlignment.Right;
                case "inside": return HorizontalAlignment.Inside;
                case "outside": return HorizontalAlignment.Outside;
                default: return HorizontalAlignment.None;
            }
        }

        internal static string HorizontalAlignmentToXml(HorizontalAlignment value)
        {
            switch (value)
            {
                case HorizontalAlignment.Left: return "left";
                case HorizontalAlignment.Center: return "center";
                case HorizontalAlignment.Right: return "right";
                case HorizontalAlignment.Inside: return "inside";
                case HorizontalAlignment.Outside: return "outside";
                default: return "";
            }
        }

        internal static RelativeHorizontalPosition XmlToRelativeHorizontalPosition(string value)
        {
            switch (value)
            {
                case "text": return RelativeHorizontalPosition.Column;
                case "margin": return RelativeHorizontalPosition.Margin;
                case "page": return RelativeHorizontalPosition.Page;
                default: return RelativeHorizontalPosition.Default;
            }
        }

        internal static string RelativeHorizontalPositionToXml(RelativeHorizontalPosition value)
        {
            switch (value)
            {
                case RelativeHorizontalPosition.Column: return "text";
                case RelativeHorizontalPosition.Margin: return "margin";
                case RelativeHorizontalPosition.Page: return "page";
                default: return "";
            }
        }

        internal static RelativeVerticalPosition XmlToRelativeVerticalPosition(string value)
        {
            switch (value)
            {
                case "text": return RelativeVerticalPosition.Paragraph;
                case "margin": return RelativeVerticalPosition.Margin;
                case "page": return RelativeVerticalPosition.Page;
                default: return RelativeVerticalPosition.TableDefault;
            }
        }

        internal static string RelativeVerticalPositionToXml(RelativeVerticalPosition value)
        {
            switch (value)
            {
                case RelativeVerticalPosition.Paragraph: return "text";
                case RelativeVerticalPosition.Margin: return "margin";
                case RelativeVerticalPosition.Page: return "page";
                default: return "";
            }
        }

        internal static WrapType XmlToTextFrameWrapType(string value)
        {
            switch (value)
            {
                case "auto": return WrapType.Inline;    // This is arguable. Some claim that this should mean "around".
                case "notBeside": return WrapType.TopBottom;
                case "not-beside": return WrapType.TopBottom;   // WML
                case "around": return WrapType.Square;
                case "tight": return WrapType.Tight;
                case "through": return WrapType.Through;
                case "none": return WrapType.None;
                default: return WrapType.None;
            }
        }

        internal static string TextFrameWrapTypeToXml(WrapType value, bool isDocx)
        {
            switch (value)
            {
                case WrapType.Inline: return "auto";
                case WrapType.TopBottom: return (isDocx) ? "notBeside" : "not-beside";
                case WrapType.Square: return "around";
                case WrapType.Tight: return "tight";
                case WrapType.Through: return "through";
                case WrapType.None: return "none";
                default: return "";
            }
        }

        internal static HeightRule XmlToHeightRule(string value)
        {
            switch (value)
            {
                case "auto": return HeightRule.Auto;
                case "exact": return HeightRule.Exactly;
                case "atLeast": return HeightRule.AtLeast;
                case "at-least": return HeightRule.AtLeast; // WML
                default: return HeightRule.Auto;
            }
        }

        internal static string HeightRuleToXml(HeightRule value, bool isDocx)
        {
            switch (value)
            {
                case HeightRule.Auto: return "auto";
                case HeightRule.Exactly: return "exact";
                case HeightRule.AtLeast: return (isDocx) ? "atLeast" : "at-least";
                default: return "";
            }
        }

        internal static LineSpacingRule XmlToLineSpacingRule(string value)
        {
            switch (value)
            {
                case "auto": return LineSpacingRule.Multiple;
                case "exact": return LineSpacingRule.Exactly;
                case "atLeast": return LineSpacingRule.AtLeast;
                case "at-least": return LineSpacingRule.AtLeast;    // WML
                default: return LineSpacingRule.AtLeast;    
            }
        }

        internal static string LineSpacingRuleToXml(LineSpacingRule value, bool isDocx)
        {
            switch (value)
            {
                case LineSpacingRule.Multiple: return "auto";
                case LineSpacingRule.Exactly: return "exact";
                case LineSpacingRule.AtLeast: return (isDocx) ? "atLeast" : "at-least";
                default: return "";
            }
        }

        internal static ParagraphAlignment XmlToParagraphAlignment(string value, OoxmlComplianceInfo cInfo)
        {
            switch (value)
            {
                case "start": 
                    cInfo.MarkAsIsoTransitional();
                    return ParagraphAlignment.Left;
                case "left": 
                    return ParagraphAlignment.Left;
                case "center": 
                    return ParagraphAlignment.Center;
                case "right": 
                    return ParagraphAlignment.Right;
                case "end": 
                    cInfo.MarkAsIsoTransitional();
                    return ParagraphAlignment.Right;
                case "both": 
                    return ParagraphAlignment.Justify;
                case "distribute": 
                    return ParagraphAlignment.Distributed;
                // WORDSNET-4870 Arabic alignment roundtrip support. See TestJira4870 for details.
                case "lowKashida": 
                case "low-kashida":
                    return ParagraphAlignment.ArabicLowKashida;
                case "mediumKashida":
                case "medium-kashida":
                    return ParagraphAlignment.ArabicMediumKashida;
                case "highKashida":
                case "high-kashida":
                    return ParagraphAlignment.ArabicHighKashida;
                case "thaiDistribute": 
                case "thai-distribute":
                    return ParagraphAlignment.ThaiDistributed;
                //"listTab", ParagraphAlignment.,
                //"list-tab", ParagraphAlignment.,  // WML
                default: return ParagraphAlignment.Left;
            }
        }

        internal static string ParagraphAlignmentToXml(ParagraphAlignment value, bool isDocx, 
            OoxmlComplianceCore compliance)
        {
            bool isIsoStrict = isDocx && (compliance == OoxmlComplianceCore.IsoStrict);

            switch (value)
            {
                case ParagraphAlignment.Left: return (isIsoStrict) ? "start" : "left";
                case ParagraphAlignment.Center: return "center";
                case ParagraphAlignment.Right: return (isIsoStrict) ? "end" : "right";
                case ParagraphAlignment.Justify: return "both";
                case ParagraphAlignment.Distributed: return "distribute";
                // WORDSNET-4870 Arabic alignment roundtrip support.
                case ParagraphAlignment.ArabicLowKashida: return isDocx ? "lowKashida" : "low-kashida";
                case ParagraphAlignment.ArabicMediumKashida: return isDocx ? "mediumKashida" : "medium-kashida";
                case ParagraphAlignment.ArabicHighKashida: return isDocx ? "highKashida" : "high-kashida";
                case ParagraphAlignment.ThaiDistributed: return isDocx ? "thaiDistribute" : "thai-distribute";
                //"listTab", ParagraphAlignment.,
                //"list-tab", ParagraphAlignment.,  // WML
                default: return "";
            }
        }

        internal static TabLeader XmlToTabLeader(string value)
        {
            switch (value)
            {
                case "none": return TabLeader.None;
                case "dot": return TabLeader.Dots;
                case "hyphen": return TabLeader.Dashes;
                case "underscore": return TabLeader.Line;
                case "heavy": return TabLeader.Heavy;
                case "middleDot": return TabLeader.MiddleDot;
                case "middle-dot": return TabLeader.MiddleDot;   // WML
                default: return TabLeader.None;
            }
        }

        internal static string TabLeaderToXml(TabLeader value, bool isDocx)
        {
            switch (value)
            {
                case TabLeader.None: return "none";
                case TabLeader.Dots: return "dot";
                case TabLeader.Dashes: return "hyphen";
                case TabLeader.Line: return "underscore";
                case TabLeader.Heavy: return "heavy";
                case TabLeader.MiddleDot: return (isDocx) ? "middleDot" : "middle-dot";
                default: return "";
            }
        }

        internal static TabAlignment XmlToTabAlignment(string value, OoxmlComplianceInfo complianceInfo)
        {
            switch (value)
            {
                case "clear": 
                    return TabAlignment.Clear;
                case "left":
                case "l": // DML
                    return TabAlignment.Left;
                case "start": 
                    if (complianceInfo != null) 
                        complianceInfo.MarkAsIsoTransitional();
                    return TabAlignment.Left;
                case "center":
                case "ctr": // DML
                    return TabAlignment.Center;
                case "right":
                case "r": // DML
                    return TabAlignment.Right;
                case "end":
                    if (complianceInfo != null)
                        complianceInfo.MarkAsIsoTransitional();
                    return TabAlignment.Right;
                case "decimal":
                case "dec": // DML
                    return TabAlignment.Decimal;
                case "bar": 
                    return TabAlignment.Bar;
                case "num": 
                    return TabAlignment.List;
                case "list": 
                    return TabAlignment.List;   // WML
                default: return TabAlignment.Clear;
            }
        }

        internal static string TabAlignmentToXml(TabAlignment value, bool isDocx, bool isIsoStrict)
        {
            switch (value)
            {
                case TabAlignment.Clear: return "clear";
                case TabAlignment.Left: return (isDocx && isIsoStrict) ? "start" : "left";
                case TabAlignment.Center: return "center";
                case TabAlignment.Right: return (isDocx && isIsoStrict) ? "end" : "right";
                case TabAlignment.Decimal: return "decimal";
                case TabAlignment.Bar: return "bar";
                case TabAlignment.List: return (isDocx) ? "num" : "list";
                default: return "";
            }
        }
    }
}
