// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2019 by Dmitry Sokolov

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;
using Aspose.Words.WebExtensions;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Exports task pane add-ins collection.
    /// </summary>
    internal static class DocxTaskPaneAddinsWriter
    {
        /// <summary>
        /// Writes task pane add-ins to the package.
        /// </summary>
        internal static void Write(DocxDocumentWriter writer)
        {
            TaskPaneCollection taskPanes = writer.Document.WebExtensionTaskPanes;
            if (taskPanes.Count == 0)
                return;

            WriteTaskPanes(writer, taskPanes);
        }

        /// <summary>
        /// Writes "2.2.9 CT_OsfTaskpanes" element.
        /// </summary>
        private static void WriteTaskPanes(DocxDocumentWriter writer, TaskPaneCollection taskPanes)
        {
            string relId;
            // Prepare appropriate package part.
            DocxBuilder builder = writer.CreateChildPartAndBuilder(null, @"/word/webextensions/taskpanes.xml",
                DocxContentType.WebExtensionTaskPanes, writer.RelTypes.WebExtensionTaskPanes, out relId);
            writer.PushBuilder(builder);

            // Write task panes.
            builder.StartDocument("wetp:taskpanes");
            builder.WriteAttributeString("xmlns:wetp", "http://schemas.microsoft.com/office/webextensions/taskpanes/2010/11");

            foreach(TaskPane taskPane in taskPanes)
            {
                relId = DocxWebExtensionWriter.Write(writer, builder.Part, taskPane.WebExtension);
                WriteTaskPane(builder, taskPane, relId);
            }

            builder.EndDocument();
            writer.PopBuilder();
        }

        /// <summary>
        /// Writes "2.2.8 CT_OsfTaskpane" element.
        /// </summary>
        private static void WriteTaskPane(DocxBuilder builder, TaskPane taskPane, string relToWebExtension)
        {
            const string taksPaneElement = "wetp:taskpane";
            builder.StartElement(taksPaneElement);

            builder.WriteAttribute("dockstate", DocxEnum.TaskPaneDockStateToDocx(taskPane.DockState));
            builder.WriteAttributeIfTrue("locked", taskPane.IsLocked);
            builder.WriteAttribute("visibility", taskPane.IsVisible);
            builder.WriteAttribute("width", taskPane.Width);
            builder.WriteAttribute("row", taskPane.Row);

            bool isStrict = builder.OoxmlCompliance == OoxmlComplianceCore.IsoStrict;
            builder.WriteElementWithAttributes("wetp:webextensionref",
                "xmlns:r", DocxNamespaces.GetNamespace(DocxNamespace.Relationships, isStrict),
                "r:id", relToWebExtension);

            builder.EndElement(taksPaneElement);
        }
    }
}
