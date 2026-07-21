// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/02/2013 by Andrey Noskov

namespace Aspose.Words.Nrx
{
    /// <summary>
    /// Factory for Docx relationship types.
    /// </summary>
    internal class DocxRelationshipTypes
    {
        internal DocxRelationshipTypes(bool isStrict)
        {
            mIsStrict = isStrict;
        }

        /// <summary>
        /// Returns Docx relationship type by name.
        /// </summary>
        private string GetRelType(DocxRelationshipType relationshipTypeName)
        {
            return GetType(relationshipTypeName, mIsStrict);
        }

        /// <summary>
        /// Returns Docx relationship type by name and conformance type.
        /// </summary>
        internal static string GetType(DocxRelationshipType relationshipTypeName, bool isStrict)
        {
            string[] docxRelationshipTypes = isStrict ? gDocxRelationshipTypesStrict : gDocxRelationshipTypesTransitional;
            return docxRelationshipTypes[(int)relationshipTypeName];
        }

        internal string OfficeDocument { get { return GetRelType(DocxRelationshipType.OfficeDocument); } }
        internal string CoreProperties { get { return GetRelType(DocxRelationshipType.CoreProperties); } }
        internal string ExtendedProperties { get { return GetRelType(DocxRelationshipType.ExtendedProperties); } }
        internal string CustomProperties { get { return GetRelType(DocxRelationshipType.CustomProperties); } }
        internal string Thumbnail { get { return GetRelType(DocxRelationshipType.Thumbnail); } }
        internal string FontTable { get { return GetRelType(DocxRelationshipType.FontTable); } }
        internal string Styles { get { return GetRelType(DocxRelationshipType.Styles); } }
        internal string Numbering { get { return GetRelType(DocxRelationshipType.Numbering); } }
        internal string Settings { get { return GetRelType(DocxRelationshipType.Settings); } }
        internal string WebSettings { get { return GetRelType(DocxRelationshipType.WebSettings); } }
        internal string Theme { get { return GetRelType(DocxRelationshipType.Theme); } }
        internal string ThemeOverride { get { return GetRelType(DocxRelationshipType.ThemeOverride); } }
        internal string Header { get { return GetRelType(DocxRelationshipType.Header); } }
        internal string Footer { get { return GetRelType(DocxRelationshipType.Footer); } }
        internal string Comments { get { return GetRelType(DocxRelationshipType.Comments); } }
        internal string CommentsEx { get { return GetRelType(DocxRelationshipType.CommentsEx); } }
        internal string CommentsIds { get { return GetRelType(DocxRelationshipType.CommentsIds); } }
        internal string CommentsExtensible { get { return GetRelType(DocxRelationshipType.CommentsExtensible); } }
        internal string Footnotes { get { return GetRelType(DocxRelationshipType.Footnotes); } }
        internal string Endnotes { get { return GetRelType(DocxRelationshipType.Endnotes); } }
        internal string GlossaryDocument { get { return GetRelType(DocxRelationshipType.GlossaryDocument); } }
        internal string CustomXml { get { return GetRelType(DocxRelationshipType.CustomXml); } }
        internal string AlternativeFormatChunk { get { return GetRelType(DocxRelationshipType.AlternativeFormatChunk); } }
        internal string Chart { get { return GetRelType(DocxRelationshipType.Chart); } }
        internal string DiagramColors { get { return GetRelType(DocxRelationshipType.DiagramColors); } }
        internal string DiagramData { get { return GetRelType(DocxRelationshipType.DiagramData); } }
        internal string DiagramLayout { get { return GetRelType(DocxRelationshipType.DiagramLayout); } }
        internal string DiagramStyle { get { return GetRelType(DocxRelationshipType.DiagramStyle); } }
        internal string PrinterSettings { get { return GetRelType(DocxRelationshipType.PrinterSettings); } }
        internal string SubDocument { get { return GetRelType(DocxRelationshipType.SubDocument); } }
        internal string Image { get { return GetRelType(DocxRelationshipType.Image); } }
        internal string Video { get { return GetRelType(DocxRelationshipType.Video); } }
        internal string OleObject { get { return GetRelType(DocxRelationshipType.OleObject); } }
        internal string Control { get { return GetRelType(DocxRelationshipType.Control); } }
        internal string Package { get { return GetRelType(DocxRelationshipType.Package); } }
        internal string Hyperlink { get { return GetRelType(DocxRelationshipType.Hyperlink); } }
        internal string AttachedTemplate { get { return GetRelType(DocxRelationshipType.AttachedTemplate); } }
        internal string Vba { get { return GetRelType(DocxRelationshipType.Vba); } }
        internal string Customizations { get { return GetRelType(DocxRelationshipType.Customizations); } }
        internal string People { get { return GetRelType(DocxRelationshipType.People); } }
        internal string ChartDrawing { get { return GetRelType(DocxRelationshipType.ChartDrawing); } }
        internal string DiagramDrawing { get { return GetRelType(DocxRelationshipType.DiagramDrawing); } }
        internal string CustomXmlProperties { get { return GetRelType(DocxRelationshipType.CustomXmlProperties); } }
        internal string VbaData { get { return GetRelType(DocxRelationshipType.VbaData); } }
        internal string VbaProjectSignature { get { return GetRelType(DocxRelationshipType.VbaProjectSignature); } }
        internal string AttachedToolbars { get { return GetRelType(DocxRelationshipType.AttachedToolbars); } }
        internal string MailMergeDataSource { get { return GetRelType(DocxRelationshipType.MailMergeDataSource); } }
        internal string MailMergeHeaderSource { get { return GetRelType(DocxRelationshipType.MailMergeHeaderSource); } }
        internal string MailMergeRecipientData { get { return GetRelType(DocxRelationshipType.MailMergeRecipientData); } }
        internal string Font { get { return GetRelType(DocxRelationshipType.Font); } }
        internal string Frameset { get { return GetRelType(DocxRelationshipType.Frameset); } }
        internal string ControlBinary { get { return GetRelType(DocxRelationshipType.ControlBinary); } }
        internal string SaveThroughXslt { get { return GetRelType(DocxRelationshipType.SaveThroughXslt); } }
        internal string WebExtensionTaskPanes { get { return GetRelType(DocxRelationshipType.WebExtensionTaskPanes); } }
        internal string WebExtension { get { return GetRelType(DocxRelationshipType.WebExtension); } }

        private readonly bool mIsStrict;

        private static void InitDocxRelationshipTypeStrict()
        {
            //From package.
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.OfficeDocument] = "http://purl.oclc.org/ooxml/officeDocument/relationships/officeDocument";

            //From package.
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.CoreProperties] = "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties";

            //From package.
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.ExtendedProperties] = "http://purl.oclc.org/ooxml/officeDocument/relationships/extendedProperties";

            //From package.
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.CustomProperties] = "http://purl.oclc.org/ooxml/officeDocument/relationships/customProperties";

            //From package.
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.WebExtensionTaskPanes] = "http://schemas.microsoft.com/office/2011/relationships/webextensiontaskpanes";

            //From package, from document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Thumbnail] = "http://schemas.openxmlformats.org/package/2006/relationships/metadata/thumbnail";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.FontTable] = "http://purl.oclc.org/ooxml/officeDocument/relationships/fontTable";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Styles] = "http://purl.oclc.org/ooxml/officeDocument/relationships/styles";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Numbering] = "http://purl.oclc.org/ooxml/officeDocument/relationships/numbering";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Settings] = "http://purl.oclc.org/ooxml/officeDocument/relationships/settings";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.WebSettings] = "http://purl.oclc.org/ooxml/officeDocument/relationships/webSettings";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Theme] = "http://purl.oclc.org/ooxml/officeDocument/relationships/theme";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.ThemeOverride] = @"http://purl.oclc.org/ooxml/officeDocument/relationships/themeOverride";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Header] = "http://purl.oclc.org/ooxml/officeDocument/relationships/header";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Footer] = "http://purl.oclc.org/ooxml/officeDocument/relationships/footer";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Comments] = "http://purl.oclc.org/ooxml/officeDocument/relationships/comments";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.CommentsEx] = "http://schemas.microsoft.com/office/2011/relationships/commentsExtended";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.CommentsIds] =
                "http://schemas.microsoft.com/office/2016/09/relationships/commentsIds";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.CommentsExtensible] =
                "http://schemas.microsoft.com/office/2018/08/relationships/commentsExtensible";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Footnotes] = "http://purl.oclc.org/ooxml/officeDocument/relationships/footnotes";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Endnotes] = "http://purl.oclc.org/ooxml/officeDocument/relationships/endnotes";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.GlossaryDocument] = "http://purl.oclc.org/ooxml/officeDocument/relationships/glossaryDocument";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.CustomXml] = "http://purl.oclc.org/ooxml/officeDocument/relationships/customXml";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.AlternativeFormatChunk] = "http://purl.oclc.org/ooxml/officeDocument/relationships/aFChunk";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Chart] = "http://purl.oclc.org/ooxml/officeDocument/relationships/chart";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.DiagramColors] = "http://purl.oclc.org/ooxml/officeDocument/relationships/diagramColors";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.DiagramData] = "http://purl.oclc.org/ooxml/officeDocument/relationships/diagramData";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.DiagramLayout] = "http://purl.oclc.org/ooxml/officeDocument/relationships/diagramLayout";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.DiagramStyle] = "http://purl.oclc.org/ooxml/officeDocument/relationships/diagramQuickStyle";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.PrinterSettings] = "http://purl.oclc.org/ooxml/officeDocument/relationships/printerSettings";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.SubDocument] = "http://purl.oclc.org/ooxml/officeDocument/relationships/subDocument";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Image] = "http://purl.oclc.org/ooxml/officeDocument/relationships/image";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Video] = "http://purl.oclc.org/ooxml/officeDocument/relationships/video";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.OleObject] = "http://purl.oclc.org/ooxml/officeDocument/relationships/oleObject";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Control] = "http://purl.oclc.org/ooxml/officeDocument/relationships/control";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Package] = "http://purl.oclc.org/ooxml/officeDocument/relationships/package";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Hyperlink] = "http://purl.oclc.org/ooxml/officeDocument/relationships/hyperlink";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.AttachedTemplate] = "http://purl.oclc.org/ooxml/officeDocument/relationships/attachedTemplate";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.ChartEx] = "http://schemas.microsoft.com/office/2014/relationships/chartEx";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Vba] = "http://schemas.microsoft.com/office/2006/relationships/vbaProject";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Customizations] = "http://schemas.microsoft.com/office/2006/relationships/keyMapCustomizations";

            //From document.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.People] = "http://schemas.microsoft.com/office/2011/relationships/people";

            //From Chart.
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.ChartDrawing] = "http://purl.oclc.org/ooxml/officeDocument/relationships/chartUserShapes";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.ChartStyle] = "http://schemas.microsoft.com/office/2011/relationships/chartStyle";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.ChartColorStyle] = "http://schemas.microsoft.com/office/2011/relationships/chartColorStyle";

            //From Diagram Data.
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.DiagramDrawing] = "http://schemas.microsoft.com/office/2007/relationships/diagramDrawing";

            //From Custom XML Data Storage Part to its properties.
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.CustomXmlProperties] = "http://purl.oclc.org/ooxml/officeDocument/relationships/customXmlProps";

            //From vbaProject.bin
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.VbaData] = "http://schemas.microsoft.com/office/2006/relationships/wordVbaData";

            //From vbaProject.bin
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.VbaProjectSignature] = "http://schemas.microsoft.com/office/2006/relationships/vbaProjectSignature";

            //From customizations.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.AttachedToolbars] = "http://schemas.microsoft.com/office/2006/relationships/attachedToolbars";

            //From settings.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.MailMergeDataSource] = "http://purl.oclc.org/ooxml/officeDocument/relationships/mailMergeSource";

            //From settings.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.MailMergeHeaderSource] = "http://purl.oclc.org/ooxml/officeDocument/relationships/mailMergeHeaderSource";

            //From settings.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.MailMergeRecipientData] = "http://purl.oclc.org/ooxml/officeDocument/relationships/mailMergeRecipientData";

            //From fontTable.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Font] = "http://purl.oclc.org/ooxml/officeDocument/relationships/font";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.Frameset] = "http://purl.oclc.org/ooxml/officeDocument/relationships/frame";
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.ControlBinary] = "http://schemas.microsoft.com/office/2006/relationships/activeXControlBinary";

            //From settings.xml
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.SaveThroughXslt] = "http://purl.oclc.org/ooxml/officeDocument/relationships/transform";

            //From taskpanes.xml.
            gDocxRelationshipTypesStrict[(int)DocxRelationshipType.WebExtension] = "http://schemas.microsoft.com/office/2011/relationships/webextension";
        }

        private static void InitDocxRelationshipTypeTransitional()
        {
            //From package.
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.OfficeDocument] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";

            //From package.
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.CoreProperties] = "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties";

            //From package.
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.ExtendedProperties] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties";

            //From package.
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.CustomProperties] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties";

            //From package.
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.WebExtensionTaskPanes] = "http://schemas.microsoft.com/office/2011/relationships/webextensiontaskpanes";

            //From package, from document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Thumbnail] = "http://schemas.openxmlformats.org/package/2006/relationships/metadata/thumbnail";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.FontTable] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/fontTable";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Styles] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Numbering] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/numbering";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Settings] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.WebSettings] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/webSettings";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Theme] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.ThemeOverride] = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/themeOverride";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Header] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/header";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Footer] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Comments] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/comments";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.CommentsEx] = "http://schemas.microsoft.com/office/2011/relationships/commentsExtended";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.CommentsIds] = 
                "http://schemas.microsoft.com/office/2016/09/relationships/commentsIds";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.CommentsExtensible] =
                "http://schemas.microsoft.com/office/2018/08/relationships/commentsExtensible";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Footnotes] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footnotes";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Endnotes] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/endnotes";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.GlossaryDocument] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/glossaryDocument";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.CustomXml] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/customXml";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.AlternativeFormatChunk] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/aFChunk";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Chart] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/chart";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.DiagramColors] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/diagramColors";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.DiagramData] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/diagramData";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.DiagramLayout] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/diagramLayout";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.DiagramStyle] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/diagramQuickStyle";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.PrinterSettings] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/printerSettings";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.SubDocument] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/subDocument";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Image] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Video] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/video";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.OleObject] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/oleObject";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Control] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/control";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Package] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/package";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Hyperlink] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.AttachedTemplate] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/attachedTemplate";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.ChartEx] = "http://schemas.microsoft.com/office/2014/relationships/chartEx";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Vba] = "http://schemas.microsoft.com/office/2006/relationships/vbaProject";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Customizations] = "http://schemas.microsoft.com/office/2006/relationships/keyMapCustomizations";

            //From document.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.People] = "http://schemas.microsoft.com/office/2011/relationships/people";

            //From Chart.
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.ChartDrawing] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/chartUserShapes";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.ChartStyle] = "http://schemas.microsoft.com/office/2011/relationships/chartStyle";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.ChartColorStyle] = "http://schemas.microsoft.com/office/2011/relationships/chartColorStyle";

            //From Diagram Data.
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.DiagramDrawing] = "http://schemas.microsoft.com/office/2007/relationships/diagramDrawing";

            //From Custom XML Data Storage Part to its properties.
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.CustomXmlProperties] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/customXmlProps";

            //From vbaProject.bin
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.VbaData] = "http://schemas.microsoft.com/office/2006/relationships/wordVbaData";

            //From vbaProject.bin
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.VbaProjectSignature] = "http://schemas.microsoft.com/office/2006/relationships/vbaProjectSignature";

            //From customizations.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.AttachedToolbars] = "http://schemas.microsoft.com/office/2006/relationships/attachedToolbars";

            //From settings.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.MailMergeDataSource] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/mailMergeSource";

            //From settings.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.MailMergeHeaderSource] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/mailMergeHeaderSource";

            //From settings.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.MailMergeRecipientData] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/recipientData";

            //From fontTable.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Font] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/font";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.Frameset] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/frame";
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.ControlBinary] = "http://schemas.microsoft.com/office/2006/relationships/activeXControlBinary";

            //From settings.xml
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.SaveThroughXslt] = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/transform";

            //From taskpanes.xml.
            gDocxRelationshipTypesTransitional[(int)DocxRelationshipType.WebExtension] = "http://schemas.microsoft.com/office/2011/relationships/webextension";
        }

        private static readonly string[] gDocxRelationshipTypesStrict;
        private static readonly string[] gDocxRelationshipTypesTransitional;

        static DocxRelationshipTypes()
        {
            int docxRelationshipTypeCount = EnumUtilPal.GetEffectiveArrayLength(DocxRelationshipType.AlternativeFormatChunk.GetType(), 58);

            gDocxRelationshipTypesStrict = new string[docxRelationshipTypeCount];
            InitDocxRelationshipTypeStrict();

            gDocxRelationshipTypesTransitional = new string[docxRelationshipTypeCount];
            InitDocxRelationshipTypeTransitional();
        }
    }
}
