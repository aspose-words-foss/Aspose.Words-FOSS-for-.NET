// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Class used to read DML diagram drawing.
    /// </summary>
    internal class DmlDiagramDrawingReader : DmlReaderBase
    {
        private DmlDiagramDrawingReader()
        {
        }

        internal static GroupShape Read(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            if (xmlReader.LocalName != "drawing")
                return null;

            GroupShape diagramDrawing = null;
            while (xmlReader.ReadChild("drawing"))
            {
                switch (xmlReader.LocalName)
                {
                    case "spTree":
                        diagramDrawing = DmlCompositeNodeReader.ReadGroup(reader);
                        break;
                    default:
                        WarnNotSupportedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return diagramDrawing;
        }
    }
}
