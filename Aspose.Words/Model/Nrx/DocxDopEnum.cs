// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/07/2007 by Vladimir Averkin

using System;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Math;
using Aspose.Words.Revisions;
using Aspose.Words.Settings;
using Aspose.Words.Styles;

namespace Aspose.Words.Nrx
{
    /// <summary>
    /// Converts enumerated types related to document properties between enum and DOCX string.
    /// </summary>
    internal class DocxDopEnum
    {
        internal static ProtectionType DocxToProtectionType(string value)
        {
            return (ProtectionType)gProtectionTypeMap.GetValue(value, (int)ProtectionType.NoProtection);
        }

        internal static string ProtectionTypeToDocx(ProtectionType value)
        {
            return gProtectionTypeMap.GetValue((int)value, "none");
        }

        internal static JustificationMode DocxToKinsokuJustification(string value)
        {
            return (JustificationMode)gKinsokuJustificationMap.GetValue(value, (int)JustificationMode.Expand);
        }

        internal static string KinsokuJustificationToDocx(JustificationMode value)
        {
            return gKinsokuJustificationMap.GetValue((int)value, "DontCompress");
        }

        internal static ProofState DocxToProofState(string value)
        {
            switch (value)
            {
                case "clean":
                    return ProofState.Clean;
                default:
                    return ProofState.None;
            }
        }

        internal static string ProofStateToDocx(ProofState value)
        {
            switch (value)
            {
                case ProofState.Clean:
                    return "clean";
                default:
                    return "";
            }
        }

        internal static ViewType DocxToViewType(string value)
        {
            switch (value)
            {
                case "normal":
                    return ViewType.Normal;
                case "print":
                    return ViewType.PageLayout;
                case "outline":
                    return ViewType.Outline;
                case "masterPages":
                    return ViewType.Outline;
                case "web":
                    return ViewType.Web;
                default:
                    return ViewType.PageLayout;
            }
        }

        internal static string ViewTypeToDocx(ViewType value)
        {
            // ViewType.Reading setting seems to be ignored by MS Word now.
            switch (value)
            {
                case ViewType.Normal:
                    return "normal";
                case ViewType.Outline:
                    return "masterPages";
                case ViewType.Web:
                    return "web";
                default:
                    return "";
            }
        }
        
        /// <summary>
        /// Can convert XML values to <see cref="StylePaneSortMethod"/> as defined in 
        /// ISO29500 part1, 17.18.82 ST_StyleSort (Style Sort Settings),
        /// and ECMA-376, part4, 2.15.1.87 stylePaneSortMethod
        /// </summary>
        internal static StylePaneSortMethod DocxToStylePaneSortMethod(string value, OoxmlComplianceInfo ci)
        {
            if (Char.IsDigit(value[0])) // older pre ISO29500 way.
            {
                return StyleConvertUtil.IntToStylePaneSortMethod(FormatterPal.XmlToInt(value));
            }
            else // new ISO29500 and up.
            {
                ci.MarkAsIsoTransitional();
                return (StylePaneSortMethod)gStylePaneSortMethodMap.GetValue(value, (int)StylePaneSortMethod.Default);
            }
        }
        
        internal static string StylePaneSortMethodToDocx(StylePaneSortMethod value)
        {
            return gStylePaneSortMethodMap.GetValue((int)value, "default");
        }
        
        internal static ZoomType DocxToZoomType(string value)
        {
            return (ZoomType)gZoomTypeMap.GetValue(value, (int)ZoomType.None);
        }

        internal static string ZoomTypeToDocx(ZoomType value)
        {
            return gZoomTypeMap.GetValue((int)value, "none");
        }

        internal static AutoFormatDocumentType DocxToAutoFormatDocumentType(string value)
        {
            return (AutoFormatDocumentType)gAutoFormatDocumentTypeMap.GetValue(value, (int)AutoFormatDocumentType.Normal);
        }

        internal static string AutoFormatDocumentTypeToDocx(AutoFormatDocumentType value)
        {
            return gAutoFormatDocumentTypeMap.GetValue((int)value, "");
        }
        
        internal static WebTarget DocxToWebTarget(string value)
        {
            return (WebTarget)gWebTargetMap.GetValue(value, (int)WebTarget.None);
        }
        
        internal static string WebTargetToDocx(WebTarget value)
        {
            return gWebTargetMap.GetValue((int)value, "");
        }

        internal static MathBreakOnBinary DocxToMathBreakOnBinary(string value)
        {
            return (MathBreakOnBinary)gMathBreakOnBinaryMap.GetValue(value, (int)MathBreakOnBinary.Default);
        }
        
        internal static string MathBreakOnBynaryToDocx(MathBreakOnBinary value)
        {
            return gMathBreakOnBinaryMap.GetValue((int)value, "");
        }
        
        internal static MathBreakOnBinarySubtraction DocxToMathBreakOnBinarySubtraction(string value)
        {
            return (MathBreakOnBinarySubtraction)gMathBreakOnBinarySubtractionMap.GetValue(value, (int)MathBreakOnBinarySubtraction.Default);
        }
        
        internal static string MathBreakOnBinarySubtractionToDocx(MathBreakOnBinarySubtraction value)
        {
            return gMathBreakOnBinarySubtractionMap.GetValue((int)value, "");
        }

        internal static EditorType DocxToEditorType(string value)
        {
            // Fixed typo in default value: empty string -> EditorType.Unspecified.
            return (EditorType)gEditorTypeMap.GetValue(value, (int)EditorType.Unspecified);
        }

        internal static string EditorTypeToDocx(EditorType value)
        {
            return gEditorTypeMap.GetValue((int)value, "");
        }
        
        internal static DisplacedByType DocxToDisplacedByType(string value)
        {
            return (DisplacedByType)gDisplacedByTypeMap.GetValue(value, (int)DisplacedByType.Unspecified);
        }

        internal static string DisplacedByTypeToDocx(DisplacedByType value)
        {
            return gDisplacedByTypeMap.GetValue((int)value, "");
        }
        
        //JAVA: declarations moved here to exclude java's illegal forward reference.

        private static readonly StringToIntBidirectionalMap gProtectionTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gKinsokuJustificationMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gZoomTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gAutoFormatDocumentTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gWebTargetMap = new StringToIntBidirectionalMap();        
        private static readonly StringToIntBidirectionalMap gStylePaneSortMethodMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gMathBreakOnBinaryMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gMathBreakOnBinarySubtractionMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gEditorTypeMap = new StringToIntBidirectionalMap(); 
        private static readonly StringToIntBidirectionalMap gDisplacedByTypeMap = new StringToIntBidirectionalMap();          
        static DocxDopEnum()
        {
            gZoomTypeMap.AddEntry("none", (int)ZoomType.None);
            gZoomTypeMap.AddEntry("fullPage", (int)ZoomType.FullPage);
            gZoomTypeMap.AddEntry("bestFit", (int)ZoomType.PageWidth);
            gZoomTypeMap.AddEntry("textFit", (int)ZoomType.TextFit);

            gAutoFormatDocumentTypeMap.AddEntry("letter", (int)AutoFormatDocumentType.Letter);
            gAutoFormatDocumentTypeMap.AddEntry("eMail", (int)AutoFormatDocumentType.Email);

            gKinsokuJustificationMap.AddEntry("compressPunctuation", (int)JustificationMode.Compress);
            gKinsokuJustificationMap.AddEntry("compressPunctuationAndJapaneseKana", (int)JustificationMode.CompressKana);
            gKinsokuJustificationMap.AddEntry("doNotCompress", (int)JustificationMode.Expand);

            gProtectionTypeMap.AddEntry("trackedChanges", (int)ProtectionType.AllowOnlyRevisions);
            gProtectionTypeMap.AddEntry("comments", (int)ProtectionType.AllowOnlyComments);
            gProtectionTypeMap.AddEntry("forms", (int)ProtectionType.AllowOnlyFormFields);
            gProtectionTypeMap.AddEntry("readOnly", (int)ProtectionType.ReadOnly);
            gProtectionTypeMap.AddEntry("none", (int)ProtectionType.NoProtection);

            gWebTargetMap.AddEntry("W3C XHTML+CSS1", (int)WebTarget.XhtmlPlusCss1);
            gWebTargetMap.AddEntry("W3C HTML4+CSS1", (int)WebTarget.Html4PlusCss1);
            gWebTargetMap.AddEntry("W3C XHTML+CSS2", (int)WebTarget.XhtmlPlusCss2);
            gWebTargetMap.AddEntry("W3C HTML4+CSS2", (int)WebTarget.Html4PlusCss2);

            gStylePaneSortMethodMap.AddEntry("basedOn", (int)StylePaneSortMethod.BasedOn);
            gStylePaneSortMethodMap.AddEntry("font", (int)StylePaneSortMethod.Font);
            gStylePaneSortMethodMap.AddEntry("name", (int)StylePaneSortMethod.Name);
            gStylePaneSortMethodMap.AddEntry("priority", (int)StylePaneSortMethod.Priority);
            gStylePaneSortMethodMap.AddEntry("type", (int)StylePaneSortMethod.StyleType);
            gStylePaneSortMethodMap.AddEntry("default", (int)StylePaneSortMethod.Default);

            gMathBreakOnBinaryMap.AddEntry("after", (int)MathBreakOnBinary.After);
            gMathBreakOnBinaryMap.AddEntry("before", (int)MathBreakOnBinary.Before);
            gMathBreakOnBinaryMap.AddEntry("repeat", (int)MathBreakOnBinary.Repeat);

            gMathBreakOnBinarySubtractionMap.AddEntry("+-", (int)MathBreakOnBinarySubtraction.PlusMinus);
            gMathBreakOnBinarySubtractionMap.AddEntry("-+", (int)MathBreakOnBinarySubtraction.MinusPlus);
            gMathBreakOnBinarySubtractionMap.AddEntry("--", (int)MathBreakOnBinarySubtraction.MinusMinus);

            gEditorTypeMap.AddEntry("", (int)EditorType.Unspecified);
            gEditorTypeMap.AddEntry("administrators", (int)EditorType.Administrators);
            gEditorTypeMap.AddEntry("contributors", (int)EditorType.Contributors);
            gEditorTypeMap.AddEntry("current", (int)EditorType.Current);
            gEditorTypeMap.AddEntry("editors", (int)EditorType.Editors);
            gEditorTypeMap.AddEntry("everyone", (int)EditorType.Everyone);
            gEditorTypeMap.AddEntry("none", (int)EditorType.None);
            gEditorTypeMap.AddEntry("owners", (int)EditorType.Owners);

            gDisplacedByTypeMap.AddEntry("next", (int)DisplacedByType.Next);
            gDisplacedByTypeMap.AddEntry("prev", (int)DisplacedByType.Prev);
            gDisplacedByTypeMap.AddEntry("", (int)DisplacedByType.Unspecified);
        }
    }
}
