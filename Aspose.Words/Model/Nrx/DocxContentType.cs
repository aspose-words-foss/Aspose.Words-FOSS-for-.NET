// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/08/2007 by Vladimir Averkin

using Aspose.OpcPackaging;

namespace Aspose.Words.Nrx
{
    /// <summary>
    /// Helper class to map names of relation types to enum-like references.
    /// </summary>
    internal static class DocxContentType
    {
        /// <summary>
        /// Used for all other OLE objects (not Office binary, not OOXML).
        /// </summary>
        internal const string OleObject = "application/vnd.openxmlformats-officedocument.oleObject";

        /// <summary>
        /// Used for embedded OLE objects that are binary Excel documents.
        /// </summary>
        internal const string Xls = "application/vnd.ms-excel";

        /// <summary>
        /// Used for embedded OLE objects that are binary PowerPoint documents.
        /// </summary>
        internal const string Ppt = "application/vnd.ms-powerpoint";

        /// <summary>
        /// Used for embedded OLE objects that are binary Word documents.
        /// </summary>
        internal const string Doc = "application/msword";

        /// <summary>
        /// Used for embedded OLE objects that are binary Visio documents.
        /// </summary>
        internal const string Vsd = "application/vnd.visio";

        /// <summary>
        /// Used for the DOCX document as a whole or when embedded as a package.
        /// </summary>
        internal const string Docx = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        internal const string Dotx = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
        internal const string Docm = "application/vnd.ms-word.document.macroEnabled.12";
        internal const string Dotm = "application/vnd.ms-word.template.macroEnabled.12";

        internal const string Xlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        internal const string Xltx = "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
        internal const string Xlsm = "application/vnd.ms-excel.sheet.macroEnabled.12";
        internal const string Xltm = "application/vnd.ms-excel.template.macroEnabled.12";
        internal const string Xlsb = "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
        internal const string Pptx = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
        
        internal const string Pptm = "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
        internal const string Ppsx = "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
        internal const string Ppsm = "application/vnd.ms-powerpoint.slideshow.macroEnabled.12";

        internal const string Sldx = "application/vnd.openxmlformats-officedocument.presentationml.slide";
        internal const string Sldm = "application/vnd.ms-powerpoint.slide.macroEnabled.12";

        internal const string Vsdx = "application/vnd.ms-visio.drawing";

        internal const string CoreProperties = "application/vnd.openxmlformats-package.core-properties+xml";
        internal const string ExtendedProperties = "application/vnd.openxmlformats-officedocument.extended-properties+xml";
        internal const string CustomProperties = "application/vnd.openxmlformats-officedocument.custom-properties+xml";

        internal const string WebExtensionTaskPanes = "application/vnd.ms-office.webextensiontaskpanes+xml";
        internal const string WebExtension = "application/vnd.ms-office.webextension+xml";

        /// <summary>
        /// Used for macro-free documents.
        /// </summary>
        internal const string Document = "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml";
        /// <summary>
        /// Used for macro-enabled documents.
        /// </summary>
        internal const string DocumentMacroEnabled = "application/vnd.ms-word.document.macroEnabled.main+xml";
        /// <summary>
        /// Used for macro-free templates.
        /// </summary>
        internal const string Template = "application/vnd.openxmlformats-officedocument.wordprocessingml.template.main+xml";
        /// <summary>
        /// Used for macro-enabled templates.
        /// </summary>
        internal const string TemplateMacroEnabled = "application/vnd.ms-word.template.macroEnabledTemplate.main+xml";

        internal const string GlossaryDocument = "application/vnd.openxmlformats-officedocument.wordprocessingml.document.glossary+xml";

        internal const string Comments = "application/vnd.openxmlformats-officedocument.wordprocessingml.comments+xml";
        internal const string CommentsExtended = "application/vnd.openxmlformats-officedocument.wordprocessingml.commentsExtended+xml";
        internal const string CommentsIds = 
            "application/vnd.openxmlformats-officedocument.wordprocessingml.commentsIds+xml";
        internal const string CommentsExtensible = 
            "application/vnd.openxmlformats-officedocument.wordprocessingml.commentsExtensible+xml";
        internal const string FontTable = "application/vnd.openxmlformats-officedocument.wordprocessingml.fontTable+xml";
        internal const string Footnotes = "application/vnd.openxmlformats-officedocument.wordprocessingml.footnotes+xml";
        internal const string Endnotes = "application/vnd.openxmlformats-officedocument.wordprocessingml.endnotes+xml";
        internal const string Numbering = "application/vnd.openxmlformats-officedocument.wordprocessingml.numbering+xml";
        internal const string Styles = "application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml";
        internal const string Settings = "application/vnd.openxmlformats-officedocument.wordprocessingml.settings+xml";
        internal const string WebSettings = "application/vnd.openxmlformats-officedocument.wordprocessingml.webSettings+xml";
        internal const string Header = "application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml";
        internal const string Footer = "application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml";
        internal const string People = "application/vnd.openxmlformats-officedocument.wordprocessingml.people+xml";

        /// <summary>
        /// For /word/vbaProject.bin
        /// MS Word actually writes this as a default content type for bin.
        /// </summary>
        internal const string Vba = "application/vnd.ms-office.vbaProject";
        /// <summary>
        /// For /word/vbaData.xml
        /// </summary>
        internal const string VbaData = "application/vnd.ms-word.vbaData+xml";
        /// <summary>
        /// For /word/vbaProjectSignature.bin
        /// </summary>
        internal const string VbaProjectSignature = "application/vnd.ms-office.vbaProjectSignature";
        /// <summary>
        /// For /word/customizations.xml
        /// </summary>
        internal const string Customizations = "application/vnd.ms-word.keyMapCustomizations+xml";
        /// <summary>
        /// For /word/attachedToolbars.bin
        /// </summary>
        internal const string AttachedToolbars = "application/vnd.ms-word.attachedToolbars";

        /// <summary>
        /// For /word/theme/theme1.xml
        /// </summary>
        internal const string Theme = "application/vnd.openxmlformats-officedocument.theme+xml";

        /// <summary>
        /// For /word/theme/themeOverride{0}.xml
        /// </summary>
        internal const string ThemeOverride = "application/vnd.openxmlformats-officedocument.themeOverride+xml";

        /// <summary>
        /// For recipientData.xml
        /// </summary>
        internal const string MailMergeRecipientData = "application/vnd.openxmlformats-officedocument.wordprocessingml.mailMergeRecipientData+xml";

        /// <summary>
        /// Relationship from a Custom XML Data Storage part to its Custom XML Data Storage Properties.
        /// </summary>
        internal const string CustomXmlProperties = "application/vnd.openxmlformats-officedocument.customXmlProperties+xml";

        internal const string DiagramData = "application/vnd.openxmlformats-officedocument.drawingml.diagramData+xml";
        internal const string DiagramLayout = "application/vnd.openxmlformats-officedocument.drawingml.diagramLayout+xml";
        internal const string DiagramStyle = "application/vnd.openxmlformats-officedocument.drawingml.diagramStyle+xml";
        internal const string DiagramColors = "application/vnd.openxmlformats-officedocument.drawingml.diagramColors+xml";
        internal const string DiagramDrawing = "application/vnd.ms-office.drawingml.diagramDrawing+xml";

        internal const string Chart = "application/vnd.openxmlformats-officedocument.drawingml.chart+xml";
        internal const string ChartDrawing = "application/vnd.openxmlformats-officedocument.drawingml.chartshapes+xml";
        internal const string ChartColorStyle = "application/vnd.ms-office.chartcolorstyle+xml";
        internal const string ChartStyle = "application/vnd.ms-office.chartstyle+xml";

        internal const string ChartEx = "application/vnd.ms-office.chartex+xml";

        /// <summary>
        /// Content type of XML part of ActiveX control.
        /// </summary>
        internal const string Control = "application/vnd.ms-office.activeX+xml";
        /// <summary>
        /// Content type of binary part of ActiveX control.
        /// </summary>
        internal const string ControlBinary = "application/vnd.ms-office.activeX";

        /// <summary>
        /// For /word/ink/ink.xml
        /// </summary>
        internal const string InkML = "application/inkml+xml";

        internal const string OctetStream = OpcContentType.OctetStream;

        internal const string ImagePictCompressed = OpcContentType.ImagePictCompressed;
    }
}
