// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/07/2007 by Vladimir Averkin

using Aspose.Collections;
using Aspose.Words.Lists;

namespace Aspose.Words.RW.Docx
{
    /// <summary>
    /// Converts list enumerated types between enum and DOCX string.
    /// </summary>
    internal class DocxNumberingEnum
    {
        internal static ListLevelAlignment DocxToListLevelAlignment(string value, OoxmlComplianceInfo complianceInfo)
        {
            switch (value)
            {
                case "left":
                    return ListLevelAlignment.Left;
                case "start":
                    complianceInfo.MarkAsIsoTransitional();
                    return ListLevelAlignment.Left;
                case "center":
                    return ListLevelAlignment.Center;
                case "right":
                    return ListLevelAlignment.Right;
                case "end":
                    complianceInfo.MarkAsIsoTransitional();
                    return ListLevelAlignment.Right;
                default:
                    return ListLevelAlignment.Left;
            }
            // TODO 2 Implement remaining ListLevelAlignment members:
            //left
            //center
            //right
            //both
            //mediumKashida
            //distribute
            //listTab
            //highKashida
            //lowKashida
            //thaiDistribute
        }

        internal static string ListLevelAlignmentToDocx(ListLevelAlignment value, bool isIsoStrict)
        {
            switch (value)
            {
                case ListLevelAlignment.Left:
                    return (isIsoStrict) ? "start" : "left";
                case ListLevelAlignment.Center:
                    return "center";
                case ListLevelAlignment.Right:
                    return (isIsoStrict) ? "end" : "right";
                default: 
                    return "";
            }
        }

        internal static ListTrailingCharacter DocxToListTrailingCharacter(string value)
        {
            return (ListTrailingCharacter)gListTrailingCharacterMap.GetValue(value, (int)ListTrailingCharacter.Tab);
        }

        internal static string ListTrailingCharacterToDocx(ListTrailingCharacter value)
        {
            return gListTrailingCharacterMap.GetValue((int)value, "");
        }

        internal static ListType DocxToListType(string value)
        {
            // WORDSNET-26739 An incorrect value in the WML file, we need to default to multi-level list to read such files reliably.
            return (ListType)gListTypeMap.GetValue(value, (int)ListType.HybridMultiLevel);
        }

        internal static string ListTypeToDocx(ListType value)
        {
            return gListTypeMap.GetValue((int)value, "");
        }

        //JAVA: declarations moved here to exclude java's illegal forward reference.

        private static readonly StringToIntBidirectionalMap gListTrailingCharacterMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gListTypeMap = new StringToIntBidirectionalMap();
        static DocxNumberingEnum()
        {

            gListTypeMap.AddEntry("singleLevel", (int)ListType.SingleLevel);
            gListTypeMap.AddEntry("multilevel", (int)ListType.MultiLevel);
            gListTypeMap.AddEntry("hybridMultilevel", (int)ListType.HybridMultiLevel);

            gListTrailingCharacterMap.AddEntry("tab", (int)ListTrailingCharacter.Tab);
            gListTrailingCharacterMap.AddEntry("space", (int)ListTrailingCharacter.Space);
            gListTrailingCharacterMap.AddEntry("nothing", (int)ListTrailingCharacter.Nothing);
        }
    }
}
