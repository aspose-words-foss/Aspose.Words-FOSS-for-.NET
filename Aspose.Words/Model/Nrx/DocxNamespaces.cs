// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2009 by Roman Korchagin

using System.Collections.Generic;

namespace Aspose.Words.Nrx
{
    internal class DocxNamespaces
    {
        internal DocxNamespaces(bool isStrict)
        {
            mIsStrict = isStrict;
        }

        /// <summary>
        /// Check this is MS Word 2010 namespace.
        /// </summary>
        internal static bool IsWord2010Namespace(string nameSpace)
        {
            return gXmlns2010.Contains(nameSpace);
        }

        /// <summary>
        /// Checks if this is a MS Word 2016 namespace.
        /// </summary>
        internal static bool IsWord2016Namespace(string nameSpace)
        {
            return gXmlns2016.Contains(nameSpace);
        }

        /// <summary>
        /// Check this is well known MS Word namespace.
        /// </summary>
        internal static bool IsWellKnownNamespace(string nameSpace)
        {
            return gWellKnownNamespaces.Contains(nameSpace);
        }

        /// <summary>
        /// Returns Docx namespace by name.
        /// </summary>
        private string GetNamespace(DocxNamespace docxNamespace)
        {
            return GetNamespace(docxNamespace, mIsStrict);
        }

        /// <summary>
        /// Returns Docx namespace by name and conformance type.
        /// </summary>
        internal static string GetNamespace(DocxNamespace docxNamespace, bool isStrict)
        {
            string[] docxNamespaces = isStrict ? gDocxNamespacesStrict : gDocxNamespacesTransitional;
            return docxNamespaces[(int)docxNamespace];
        }

        /// <summary>
        /// Converts ISO Strict namespace URL to corresponding ISO Transitional one. 
        /// If a specified URL is unknown, returns it without changes.
        /// </summary>
        internal static string ToIsoTransitional(string namespaceUrl)
        {
            for (int i = 0; i < gDocxNamespacesStrict.Length; i++)
            {
                if (gDocxNamespacesStrict[i] == namespaceUrl)
                    return gDocxNamespacesTransitional[i];
            }
            return namespaceUrl;
        }

        /// <summary>
        /// Converts ISO Transitional namespace URL to corresponding ISO Strict one. 
        /// If the specified URL is unknown, returns it without changes.
        /// </summary>
        internal static string ToIsoStrict(string namespaceUrl)
        {
            for (int i = 0; i < gDocxNamespacesTransitional.Length; i++)
            {
                if (gDocxNamespacesTransitional[i] == namespaceUrl)
                    return gDocxNamespacesStrict[i];
            }
            return namespaceUrl;
        }

        internal string Relationships { get { return GetNamespace(DocxNamespace.Relationships); } }
        internal string Main { get { return GetNamespace(DocxNamespace.Main); } }
        internal string WmlBeta { get { return GetNamespace(DocxNamespace.WmlBeta); } }
        internal string SchemaLibrary { get { return GetNamespace(DocxNamespace.SchemaLibrary); } }
        internal string CustomXml { get { return GetNamespace(DocxNamespace.CustomXml); } }
        internal string DrawingML { get { return GetNamespace(DocxNamespace.DrawingML); } }
        internal string DrawingMLMain { get { return GetNamespace(DocxNamespace.DrawingMLMain); } }
        internal string DrawingMLPicture { get { return GetNamespace(DocxNamespace.DrawingMLPicture); } }
        internal string DrawingMLDiagram { get { return GetNamespace(DocxNamespace.DrawingMLDiagram); } }
        internal string DrawingMLChart { get { return GetNamespace(DocxNamespace.DrawingMLChart); } }
        internal string DrawingMLLockedCanvas { get { return GetNamespace(DocxNamespace.DrawingMLLockedCanvas); } }
        internal string ActiveX { get { return GetNamespace(DocxNamespace.ActiveX); } }
        internal string DigitalSignature { get { return GetNamespace(DocxNamespace.DigitalSignature); } }
        internal string MicrosoftDigitalSignature { get { return GetNamespace(DocxNamespace.MicrosoftDigitalSignature); } }
        internal string MarkupCompatibility { get { return GetNamespace(DocxNamespace.MarkupCompatibility); } }
        internal string WordprocessingCanvas { get { return GetNamespace(DocxNamespace.WordprocessingCanvas); } }
        internal string DrawingMLIso29500 { get { return GetNamespace(DocxNamespace.DrawingMLIso29500); } }
        internal string WordprocessingGroup { get { return GetNamespace(DocxNamespace.WordprocessingGroup); } }
        internal string WordprocessingInk { get { return GetNamespace(DocxNamespace.WordprocessingInk); } }
        internal string WordrocessingShape { get { return GetNamespace(DocxNamespace.WordrocessingShape); } }
        internal string Math { get { return GetNamespace(DocxNamespace.Math); } }
        internal string W14Markup { get { return GetNamespace(DocxNamespace.W14Markup); } }
        internal string W15Markup { get { return GetNamespace(DocxNamespace.W15Markup); } }
        internal string ChartEx { get { return GetNamespace(DocxNamespace.DrawingMLChartEx); } }
        internal string ChartEx1 { get { return GetNamespace(DocxNamespace.DrawingMLChartEx1); } }
        internal string ChartEx2 { get { return GetNamespace(DocxNamespace.DrawingMLChartEx2); } }
        internal string ChartEx3 { get { return GetNamespace(DocxNamespace.DrawingMLChartEx3); } }
        internal string ChartEx4 { get { return GetNamespace(DocxNamespace.DrawingMLChartEx4); } }
        internal string ChartEx5 { get { return GetNamespace(DocxNamespace.DrawingMLChartEx5); } }
        internal string W16Symex { get { return GetNamespace(DocxNamespace.W16Symex); } }
        internal string W16Cid { get { return GetNamespace(DocxNamespace.W16Cid); } }
        internal string W16Markup { get { return GetNamespace(DocxNamespace.W16Markup); } }
        internal string W16Cex { get { return GetNamespace(DocxNamespace.W16Cex); } }
        internal string W16Sdtdh { get { return GetNamespace(DocxNamespace.W16Sdtdh); } }
        internal string CommentReaction { get { return GetNamespace(DocxNamespace.CommentReaction); } }

        private static void InitDocxNamespacesTransitional()
        {
            //From package.
            gDocxNamespacesTransitional[(int)DocxNamespace.Relationships] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
            gDocxNamespacesTransitional[(int)DocxNamespace.Main] = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            gDocxNamespacesTransitional[(int)DocxNamespace.WmlBeta] = "http://schemas.microsoft.com/office/word/2006/wordml";
            gDocxNamespacesTransitional[(int)DocxNamespace.SchemaLibrary] = "http://schemas.openxmlformats.org/schemaLibrary/2006/main";
            gDocxNamespacesTransitional[(int)DocxNamespace.CustomXml] = "http://schemas.openxmlformats.org/officeDocument/2006/customXml";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingML] = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLMain] = "http://schemas.openxmlformats.org/drawingml/2006/main";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLPicture] = "http://schemas.openxmlformats.org/drawingml/2006/picture";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLDiagram] = "http://schemas.openxmlformats.org/drawingml/2006/diagram";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLChart] = "http://schemas.openxmlformats.org/drawingml/2006/chart";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLLockedCanvas] = "http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas";
            gDocxNamespacesTransitional[(int)DocxNamespace.ActiveX] = "http://schemas.microsoft.com/office/2006/activeX";
            gDocxNamespacesTransitional[(int)DocxNamespace.DigitalSignature] = "http://schemas.openxmlformats.org/package/2006/digital-signature";
            gDocxNamespacesTransitional[(int)DocxNamespace.MicrosoftDigitalSignature] = "http://schemas.microsoft.com/office/2006/digsig";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLChartDrawing] = "http://schemas.openxmlformats.org/drawingml/2006/chartDrawing";
            gDocxNamespacesTransitional[(int)DocxNamespace.Math] = NrxNamespaces.Math;

            // Word 2010 additional namespaces.
            gDocxNamespacesTransitional[(int)DocxNamespace.MarkupCompatibility] = "http://schemas.openxmlformats.org/markup-compatibility/2006";
            gDocxNamespacesTransitional[(int)DocxNamespace.WordprocessingCanvas] = "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLIso29500] = "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing";
            gDocxNamespacesTransitional[(int)DocxNamespace.WordprocessingGroup] = "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup";
            gDocxNamespacesTransitional[(int)DocxNamespace.WordprocessingInk] = "http://schemas.microsoft.com/office/word/2010/wordprocessingInk";
            gDocxNamespacesTransitional[(int)DocxNamespace.WordrocessingShape] = "http://schemas.microsoft.com/office/word/2010/wordprocessingShape";
            gDocxNamespacesTransitional[(int)DocxNamespace.W14Markup] = "http://schemas.microsoft.com/office/word/2010/wordml";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLDiagram2008] = "http://schemas.microsoft.com/office/drawing/2008/diagram";

            // Word 2012
            gDocxNamespacesTransitional[(int)DocxNamespace.W15Markup] = "http://schemas.microsoft.com/office/word/2012/wordml";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLChart2007] = "http://schemas.microsoft.com/office/drawing/2007/8/2/chart";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLChartColorStyle] = "http://schemas.microsoft.com/office/2011/relationships/chartColorStyle";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLChartStyle] = "http://schemas.microsoft.com/office/2011/relationships/chartStyle";

            // Word 2016
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLChartEx] = "http://schemas.microsoft.com/office/drawing/2014/chartex";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLChartEx1] = "http://schemas.microsoft.com/office/drawing/2015/9/8/chartex";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLChartEx2] = "http://schemas.microsoft.com/office/drawing/2015/10/21/chartex";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLChartEx3] = "http://schemas.microsoft.com/office/drawing/2016/5/9/chartex";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLChartEx4] = "http://schemas.microsoft.com/office/drawing/2016/5/10/chartex";
            gDocxNamespacesTransitional[(int)DocxNamespace.DrawingMLChartEx5] = "http://schemas.microsoft.com/office/drawing/2016/5/11/chartex";
            gDocxNamespacesTransitional[(int)DocxNamespace.W16Symex] =
                "http://schemas.microsoft.com/office/word/2015/wordml/symex";
            gDocxNamespacesTransitional[(int)DocxNamespace.W16Cid] =
                "http://schemas.microsoft.com/office/word/2016/wordml/cid";
            gDocxNamespacesTransitional[(int)DocxNamespace.W16Markup] =
                "http://schemas.microsoft.com/office/word/2018/wordml";
            gDocxNamespacesTransitional[(int)DocxNamespace.W16Cex] =
                "http://schemas.microsoft.com/office/word/2018/wordml/cex";
            gDocxNamespacesTransitional[(int)DocxNamespace.W16Sdtdh] =
                "http://schemas.microsoft.com/office/word/2020/wordml/sdtdatahash";
            gDocxNamespacesTransitional[(int)DocxNamespace.Chart16Ac] =
                "http://schemas.microsoft.com/office/drawing/2014/chart/ac";
            gDocxNamespacesTransitional[(int)DocxNamespace.CommentReaction] =
                "http://schemas.microsoft.com/office/comments/2020/reactions";
        }

        private static void InitDocxNamespacesStrict()
        {
            //From package.
            gDocxNamespacesStrict[(int)DocxNamespace.Relationships] = "http://purl.oclc.org/ooxml/officeDocument/relationships";
            gDocxNamespacesStrict[(int)DocxNamespace.Main] = "http://purl.oclc.org/ooxml/wordprocessingml/main";
            gDocxNamespacesStrict[(int)DocxNamespace.WmlBeta] = "http://schemas.microsoft.com/office/word/2006/wordml";
            gDocxNamespacesStrict[(int)DocxNamespace.SchemaLibrary] = "http://schemas.openxmlformats.org/schemaLibrary/2006/main"; // Is the same for strict.
            gDocxNamespacesStrict[(int)DocxNamespace.CustomXml] = "http://purl.oclc.org/ooxml/officeDocument/customXml";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingML] = "http://purl.oclc.org/ooxml/drawingml/wordprocessingDrawing";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLMain] = "http://purl.oclc.org/ooxml/drawingml/main";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLPicture] = "http://purl.oclc.org/ooxml/drawingml/picture";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLDiagram] = "http://purl.oclc.org/ooxml/drawingml/diagram";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLChart] = "http://purl.oclc.org/ooxml/drawingml/chart";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLLockedCanvas] = "http://purl.oclc.org/ooxml/drawingml/lockedCanvas";
            gDocxNamespacesStrict[(int)DocxNamespace.ActiveX] = "http://schemas.microsoft.com/office/2006/activeX";
            gDocxNamespacesStrict[(int)DocxNamespace.DigitalSignature] = "http://schemas.openxmlformats.org/package/2006/digital-signature"; // Is the same for strict.
            gDocxNamespacesStrict[(int)DocxNamespace.MicrosoftDigitalSignature] = "http://schemas.microsoft.com/office/2006/digsig"; // Is the same for strict.
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLChartDrawing] = "http://purl.oclc.org/ooxml/drawingml/chartDrawing";
            gDocxNamespacesStrict[(int)DocxNamespace.Math] = "http://purl.oclc.org/ooxml/officeDocument/math";

            // Word 2010 additional namespaces.
            gDocxNamespacesStrict[(int)DocxNamespace.MarkupCompatibility] = "http://schemas.openxmlformats.org/markup-compatibility/2006";
            gDocxNamespacesStrict[(int)DocxNamespace.WordprocessingCanvas] = "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLIso29500] = "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing";
            gDocxNamespacesStrict[(int)DocxNamespace.WordprocessingGroup] = "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup";
            gDocxNamespacesStrict[(int)DocxNamespace.WordprocessingInk] = "http://schemas.microsoft.com/office/word/2010/wordprocessingInk";
            gDocxNamespacesStrict[(int)DocxNamespace.WordrocessingShape] = "http://schemas.microsoft.com/office/word/2010/wordprocessingShape";
            gDocxNamespacesStrict[(int)DocxNamespace.W14Markup] = "http://schemas.microsoft.com/office/word/2010/wordml";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLDiagram2008] = "http://schemas.microsoft.com/office/drawing/2008/diagram";

            // Word 2012
            gDocxNamespacesStrict[(int)DocxNamespace.W15Markup] = "http://schemas.microsoft.com/office/word/2012/wordml";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLChart2007] = "http://schemas.microsoft.com/office/drawing/2007/8/2/chart";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLChartColorStyle] = "http://schemas.microsoft.com/office/2011/relationships/chartColorStyle";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLChartStyle] = "http://schemas.microsoft.com/office/2011/relationships/chartStyle";

            // Word 2016
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLChartEx] = "http://schemas.microsoft.com/office/drawing/2014/chartex";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLChartEx1] = "http://schemas.microsoft.com/office/drawing/2015/9/8/chartex";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLChartEx2] = "http://schemas.microsoft.com/office/drawing/2015/10/21/chartex";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLChartEx3] = "http://schemas.microsoft.com/office/drawing/2016/5/9/chartex";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLChartEx4] = "http://schemas.microsoft.com/office/drawing/2016/5/10/chartex";
            gDocxNamespacesStrict[(int)DocxNamespace.DrawingMLChartEx5] = "http://schemas.microsoft.com/office/drawing/2016/5/11/chartex";
            gDocxNamespacesStrict[(int)DocxNamespace.W16Symex] =
                "http://schemas.microsoft.com/office/word/2015/wordml/symex";
            gDocxNamespacesStrict[(int)DocxNamespace.W16Cid] =
                "http://schemas.microsoft.com/office/word/2016/wordml/cid";
            gDocxNamespacesStrict[(int)DocxNamespace.W16Markup] =
                "http://schemas.microsoft.com/office/word/2018/wordml";
            gDocxNamespacesStrict[(int)DocxNamespace.W16Cex] =
                "http://schemas.microsoft.com/office/word/2018/wordml/cex";
            gDocxNamespacesStrict[(int)DocxNamespace.W16Sdtdh] =
                "http://schemas.microsoft.com/office/word/2020/wordml/sdtdatahash";
            gDocxNamespacesStrict[(int)DocxNamespace.Chart16Ac] =
                "http://schemas.microsoft.com/office/drawing/2014/chart/ac";
            gDocxNamespacesStrict[(int)DocxNamespace.CommentReaction] =
                "http://schemas.microsoft.com/office/comments/2020/reactions";
        }

        private readonly bool mIsStrict;
        private static readonly List<string> gXmlns2010 = new List<string>();
        private static readonly List<string> gXmlns2016 = new List<string>();
        private static readonly List<string> gWellKnownNamespaces = new List<string>();
        private static readonly string[] gDocxNamespacesStrict;
        private static readonly string[] gDocxNamespacesTransitional;
        
        /// <summary>
        /// Static ctor.
        /// </summary>
        static DocxNamespaces()
        {
            int docxNamespaceCount = EnumUtilPal.GetEffectiveArrayLength(DocxNamespace.Main.GetType(), 41);

            gDocxNamespacesStrict = new string[docxNamespaceCount];
            InitDocxNamespacesStrict();

            gDocxNamespacesTransitional = new string[docxNamespaceCount];
            InitDocxNamespacesTransitional();

            // Populate gXmlns2010 array list with Word 2010 namespaces, which we are using to check 
            // whether Choice can be matched upon reading AlternateContent.
            gXmlns2010.Add(GetNamespace(DocxNamespace.WordprocessingCanvas, false));
            gXmlns2010.Add(GetNamespace(DocxNamespace.DrawingMLIso29500, false));
            gXmlns2010.Add(GetNamespace(DocxNamespace.W14Markup, false));
            gXmlns2010.Add(GetNamespace(DocxNamespace.WordprocessingGroup, false));
            gXmlns2010.Add(GetNamespace(DocxNamespace.WordprocessingInk, false));
            gXmlns2010.Add(GetNamespace(DocxNamespace.WordrocessingShape, false));
            gXmlns2010.Add(GetNamespace(DocxNamespace.MarkupCompatibility, false));

            // The gXmlns2016 list contains Word 2016 namespaces and is used to check 
            // whether Choice can be matched upon reading AlternateContent.
            gXmlns2016.Add(GetNamespace(DocxNamespace.DrawingMLChartEx, false));
            gXmlns2016.Add(GetNamespace(DocxNamespace.DrawingMLChartEx1, false));
            gXmlns2016.Add(GetNamespace(DocxNamespace.DrawingMLChartEx2, false));
            gXmlns2016.Add(GetNamespace(DocxNamespace.DrawingMLChartEx3, false));
            gXmlns2016.Add(GetNamespace(DocxNamespace.DrawingMLChartEx4, false));
            gXmlns2016.Add(GetNamespace(DocxNamespace.DrawingMLChartEx5, false));

            gWellKnownNamespaces.Add(NrxNamespaces.Vml);
            gWellKnownNamespaces.Add(NrxNamespaces.Office);
            gWellKnownNamespaces.Add(NrxNamespaces.Word);
            gWellKnownNamespaces.Add(NrxNamespaces.Math);
        }
    }
}
