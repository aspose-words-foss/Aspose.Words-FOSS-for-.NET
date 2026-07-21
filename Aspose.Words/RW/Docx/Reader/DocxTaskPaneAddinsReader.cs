// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2019 by Dmitry Sokolov

using Aspose.OpcPackaging;
using Aspose.Words.Nrx;
using Aspose.Words.WebExtensions;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides ability to read a collection of task pane add-ins ("2.2.9 CT_OsfTaskpanes" of the MS-OWEXML spec).
    /// </summary>
    internal class DocxTaskPaneAddinsReader
    {
        /// <summary>
        /// Ctr.
        /// </summary>
        private DocxTaskPaneAddinsReader(DocxDocumentReader reader)
        {
            mDocument = reader.Document;
            mReader = reader;
        }

        /// <summary>
        /// Reads task pane add-ins.
        /// </summary>
        internal static void Read(DocxDocumentReader reader)
        {
            DocxTaskPaneAddinsReader taskPaneReader = new DocxTaskPaneAddinsReader(reader);
            taskPaneReader.Read();
        }

        /// <summary>
        /// Reads "2.2.9 CT_OsfTaskpanes" element.
        /// </summary>
        private void Read()
        {
            DocxXmlReader taskPanesReader = mReader.SwitchToPartReaderByRelType(null, mReader.RelTypes.WebExtensionTaskPanes);
            if (taskPanesReader == null)
                return;

            TaskPaneCollection taskPanes = mDocument.WebExtensionTaskPanes;

            while (taskPanesReader.ReadChild("taskpanes")) // wetp:taskpanes
            {
                switch (taskPanesReader.LocalName)
                {
                    case "taskpane": // wetp:taskpane
                    {
                        TaskPane taskPane = ReadTaskPane(taskPanesReader);
                        if (taskPane != null)
                            taskPanes.Add(taskPane);
                        break;
                    }
                    default:
                        taskPanesReader.IgnoreElement();
                        break;
                }
            }

            mReader.RestorePartReader();
        }

        /// <summary>
        /// Reads "2.2.8 CT_OsfTaskpane" element.
        /// </summary>
        private TaskPane ReadTaskPane(DocxXmlReader xmlReader)
        {
            TaskPane taskPane = new TaskPane();

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "row":
                        // Actually it is "unsignedInt" attribute according to spec.
                        taskPane.Row = xmlReader.ValueAsInt;
                        break;
                    case "width":
                        taskPane.Width = xmlReader.ValueAsDouble;
                        break;
                    case "locked":
                        taskPane.IsLocked = xmlReader.ValueAsBool;
                        break;
                    case "visibility":
                        taskPane.IsVisible = xmlReader.ValueAsBool;
                        break;
                    case "dockstate":
                        taskPane.DockState = DocxEnum.DocxToTaskPaneDockState(xmlReader.Value.ToLower());
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            while (xmlReader.ReadChild("taskpane")) // wetp:taskpane
            {
                switch (xmlReader.LocalName)
                {
                    case "webextensionref":
                    {
                        string relId = xmlReader.ReadAttribute("id", "");
                        ReadWebExtensionReference(relId, xmlReader.Part, taskPane.WebExtension);
                        break;
                    }
                    default:
                        // For example "extLst", which may be ignored according to spec.
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            return taskPane;
        }

        /// <summary>
        /// Reads content which referenced by "2.2.10 CT_WebExtensionPartRef" element.
        /// </summary>
        private void ReadWebExtensionReference(string relId, OpcPackagePart part, WebExtension extension)
        {
            OpcRelationship relationship = part.Rels.GetById(relId);
            if (relationship.IsExternal)
            {
                mReader.WarnUnexpected(WarningStrings.TaskPaneAddinHasUnexpectedReferenceType);
                return;
            }

            DocxWebExtensionReader.ReadWebExtension(relId, mReader, extension);
        }

        private readonly Document mDocument;
        private readonly DocxDocumentReader mReader;
    }
}
