// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/07/2014 by Andrey Noskov

using System;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Instance class for writing DrawingML shape properties.
    /// </summary>
    internal static class DmlShapePropertiesWriter
    {
        internal static void Write(DmlShapeBase dmlShapeBase, IDmlShapeWriterContext writer)
        {
            string prefix;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            // Use the same writer for writing legacy grpSp and wgp.
            // Determine root tag name by group shape type.
            switch (dmlShapeBase.DmlNodeType)
            {
                case DmlNodeType.Picture:
                    prefix = "pic";
                    break;
                case DmlNodeType.Shape:
                case DmlNodeType.ConnectorShape:
                    prefix = "a";
                    break;
                case DmlNodeType.WordprocessingShape:
                    prefix = isIsoStrict ? "wp" : "wps";
                    break;
                default:
                    throw new ArgumentException("Unexpected Dml node type.");
            }

            Write(prefix, dmlShapeBase, writer);
        }

        internal static void Write(string prefix, IDmlShapePrSource prSource, IDmlShapeWriterContext writer)
        {
            Write(prefix, prSource, writer, false);
        }

        /// <summary>
        /// Writes the shape properties with the option to specify whether placeholder colors that occur in chart style
        /// parts can be written to the current document part. Such a color has no particular color definition, and
        /// colors without a definition are not written to the document by default.
        /// </summary>
        internal static void Write(string prefix, IDmlShapePrSource prSource, IDmlShapeWriterContext writer,
            bool writePlaceholderColor)
        {
            if (prSource == null)
                return;

            NrxXmlBuilder builder = writer.Builder;

            string rootTagName = string.Format("{0}:spPr", prefix);

            builder.StartElement(rootTagName);

            builder.WriteAttribute("bwMode", DmlEnum.BWModeToDml(prSource.BWMode));

            DmlXfrmWriter.Write(prSource.Transform, writer);
            DmlGeomertyWriter.Write(prSource.Geometry, writer);
            DmlFillWriter.Write(((IDmlCommonShapePrSource)prSource).Fill, writer, false, writePlaceholderColor);
            DmlOutlineWriter.Write("a:ln", ((IDmlCommonShapePrSource)prSource).Outline, writer);
            DmlShapeEffectsWriter.Write(prSource.Effects, writer, false);
            Dml3DPropertiesWriter.WriteScene3D(prSource.Scene3DProperties, writer, false);
            Dml3DPropertiesWriter.WriteShape3D(prSource.Shape3DProperties, writer, false);
            DmlExtensionListWriter.Write(prSource.SpPrExtensions, writer);

            builder.EndElement(rootTagName);
        }
    }
}
