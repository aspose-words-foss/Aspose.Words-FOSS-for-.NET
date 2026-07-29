// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/07/2009 by Roman Korchagin

using Aspose.Words.Saving;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Nrx
{
    internal class NrxTableEnum
    {
        internal static TableAlignment XmlToTableAlignment(string value, OoxmlComplianceInfo cInfo)
        {
            switch (value)
            {
                case "left": return TableAlignment.Left;
                case "start":
                    {
                        cInfo.MarkAsIsoTransitional();
                        return TableAlignment.Left; // iso29500
                    }
                case "center": return TableAlignment.Center;
                case "right": return TableAlignment.Right;
                case "end":
                    {
                        cInfo.MarkAsIsoTransitional();
                        return TableAlignment.Right; // iso29500
                    }
                default: return TableAlignment.Left;
                // RK These are not supported (have not seen) for tables.
                //    both
                //    medium-kashida
                //    mediumKashida
                //    distribute
                //    list-tab
                //    listTab
                //    high-kashida
                //    highKashida
                //    low-kashida
                //    lowKashida
                //    thai-distribute
                //    thaiDistribute
            }
        }

        internal static string TableAlignmentToXml(TableAlignment value, bool isDocx, OoxmlComplianceCore compliance)
        {
            bool isIsoStrict = isDocx && (compliance == OoxmlComplianceCore.IsoStrict);

            switch (value)
            {
                case TableAlignment.Left: return (isIsoStrict) ? "start" : "left";
                case TableAlignment.Center: return "center";
                case TableAlignment.Right: return (isIsoStrict) ? "end" : "right";
                default: return "";
            }
        }

        internal static HeightRule XmlToHeightRule(string value)
        {
            switch (value)
            {
                case "auto": return HeightRule.Auto;
                case "exact": return HeightRule.Exactly;
                case "atLeast": 
                case "at-least":
                    return HeightRule.AtLeast;
                default:
                    return HeightRule.Auto;
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

        internal static CellMerge XmlToCellMerge(string value)
        {
            switch (value)
            {
                case "continue": return CellMerge.Previous;
                case "restart": return CellMerge.First;
                default: return CellMerge.None;
            }
        }

        internal static string CellMergeToXml(CellMerge value)
        {
            switch (value)
            {
                case CellMerge.Previous: return "continue";
                case CellMerge.First: return "restart";
                default: return "";
            }
        }

        internal static CellVerticalAlignment XmlToCellVerticalAlignment(string value)
        {
            switch (value)
            {
                case "top": return CellVerticalAlignment.Top;
                case "center": return CellVerticalAlignment.Center;
                case "bottom": return CellVerticalAlignment.Bottom;
                default: return CellVerticalAlignment.Top;
            }
        }

        internal static string CellVerticalAlignmentToXml(CellVerticalAlignment value)
        {
            switch (value)
            {
                case CellVerticalAlignment.Top: return "top";
                case CellVerticalAlignment.Center: return "center";
                case CellVerticalAlignment.Bottom: return "bottom";
                default: return "";
            }
        }
    }
}
