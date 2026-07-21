// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2012 by Alexey Noskov

using System.Xml;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Class used to read DML diagrams.
    /// </summary>
    internal class DmlDiagramReader : DmlReaderBase
    {
        private DmlDiagramReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
        }

        /// <summary>
        /// Builds <see cref="DmlDiagram"/>, current node must be 21.4.2.22 relIds (Explicit Relationships to Diagram Parts).
        /// </summary>
        internal static Shape Read(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            if (xmlReader.LocalName != "relIds")
                return null;

            DmlDiagramReader diagramReader = new DmlDiagramReader(reader);

            // Read id of the chart part and get it.
            Shape diagram = new Shape(reader.Document, ShapeMarkupLanguage.Dml);
            reader.AddAndPushContainer(diagram);
            DmlDiagram diagramDml = new DmlDiagram();
            diagram.DmlNode = diagramDml;
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "cs":
                        diagramDml.ColorTransformDefinition = diagramReader.ReadColors(xmlReader.Value);
                        break;
                    case "dm":
                        diagramDml.Data = diagramReader.ReadDataModel(xmlReader.Value);
                        break;
                    case "lo":
                        diagramDml.Layout = diagramReader.ReadLayout(xmlReader.Value);
                        break;
                    case "qs":
                        diagramDml.StyleDefinition = diagramReader.ReadStyles(xmlReader.Value);
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }

            if (diagramDml.Data.Drawing!= null)
                diagramDml.Data.Drawing.SetParent(diagram);

            reader.PopContainer(NodeType.Shape);

            return diagram;
        }

        private DmlDiagramLayout ReadLayout(string dataPartId)
        {
            mDocumentReader.SwitchToPartReaderByRelId(dataPartId);
            DmlDiagramLayout layout = DmlDiagramLayoutReader.Read(mDocumentReader, dataPartId);
            mDocumentReader.RestorePartReader();

            return layout;
        }

        private DmlDiagramDataModel ReadDataModel(string dataPartId)
        {
            mDocumentReader.SwitchToPartReaderByRelId(dataPartId);
            DmlDiagramDataModel dataModel = DmlDiagramDataReader.Read(mDocumentReader);
            mDocumentReader.RestorePartReader();

            dataModel.Drawing = ReadDiagramDrawing(GetDrawingRelId(dataModel));

            return dataModel;
        }

        /// <summary>
        /// Gets DrawingRelId from dataModel extension.
        /// </summary>
        private static string GetDrawingRelId(DmlDiagramDataModel dataModel)
        {
            string drawingId = dataModel.DrawingId;

            // If dataModel extension is set, DrawingRelId from this extension have to be used.
            // Try to get DrawingRelId from dataModel extension.
            // If there are no extensions, there is nothing to do here.
            if (dataModel.HasExtensions)
            {
                DmlExtension dataModelExt = dataModel.Extensions[DmlExtensionUri.DataModelExt];

                if (dataModelExt != null)
                    drawingId = dataModelExt.DrawingRelId;
            }

            return drawingId;
        }

        private ShapeBase ReadDiagramDrawing(string drawingRelId)
        {
            if (!StringUtil.HasChars(drawingRelId))
                return null;

            // WORDSNET-6787 Skip reading drawing part if there is no rel target for it.
            string relTarget = mDocumentReader.GetRelationshipTarget(drawingRelId);
            if (!StringUtil.HasChars(relTarget) || mDocumentReader.IsExternalPackage(drawingRelId))
                return null;

            ShapeBase drawing = ReadDiagramDrawingContent(drawingRelId);

            // We immediately remove diagram drawing from parent drawingML, because it should not be present in the model.
            // This is simply last time successfully rendered diagram cache, we must use it for rendering only.
            if (drawing != null)
            {
                drawing.Remove();
            }

            return drawing;
        }

        /// <summary>
        /// Read content of the diagram drawing object.
        /// </summary>
        /// <param name="drawingRelId">Link identifier to diagram drawing.</param>
        /// <returns>Object of the drawing layer.</returns>
        private ShapeBase ReadDiagramDrawingContent(string drawingRelId)
        {
            ShapeBase drawing = null;
            const string eMsgPref = "Detail message";
            const string eMsg = "Can not parse content some of diagram drawing object from specified path. {0}: {1}";

            try
            {
                mDocumentReader.SwitchToPartReaderByRelId(drawingRelId);
                // "drawingReader" does not need check for null, when package part can not be found
                // "InvalidOperationException" will throw. Package part always has content data stream of the part.
                drawing = DmlDiagramDrawingReader.Read(mDocumentReader);
            }
#if JAVA
            // In Java here is XMLStreamException wrapped by IllegalStateException because
            // original XMLStreamException is thrown by external lib.
            catch (System.InvalidOperationException ex)
#else
            catch (XmlException ex)
#endif
            {
                // WORDSNET-13443 Skip any errors which occur while parsing diagram drawing object.
                string msg = string.Format(eMsg, eMsgPref, ex.Message);
                mDocumentReader.Warn(WarningType.MinorFormattingLoss, WarningSource.DrawingML, msg);
            }

            mDocumentReader.RestorePartReader();
            return drawing;
        }

        private DmlDiagramColorTransform ReadColors(string dataPartId)
        {
            mDocumentReader.SwitchToPartReaderByRelId(dataPartId);
            DmlDiagramColorTransform colorTransform =
                DmlDiagramColorTransformReader.Read(mDocumentReader, mDocumentReader.ComplianceInfo);
            mDocumentReader.RestorePartReader();

            return colorTransform;
        }

        private DmlDiagramStyleDefinition ReadStyles(string dataPartId)
        {
            mDocumentReader.SwitchToPartReaderByRelId(dataPartId);
            DmlDiagramStyleDefinition styleDefinition =
                DmlDiagramStylesReader.Read(mDocumentReader, mDocumentReader.ComplianceInfo);
            mDocumentReader.RestorePartReader();

            return styleDefinition;
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
    }
}
