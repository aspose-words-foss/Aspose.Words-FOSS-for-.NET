// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/07/2014 by Andrey Noskov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Guides;
using Aspose.Words.Drawing.Core.Dml.Path;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlGeomertyWriter
    {
        internal static void Write(DmlGeometry geometry, IDmlShapeWriterContext writer)
        {
            if (geometry == null)
                return;

            NrxXmlBuilder builder = writer.Builder;
            DmlGuides guides = geometry.Guides;
            // If preset name of geometry is specified write it as prstGeom otherwise as custGeom.
            string presetName = geometry.PresetName;
            if (StringUtil.HasChars(presetName))
            {
                // In MS Word, only the adjustable values that are not preset are updated and written.
                // If any preset adjustable values are changed, we need to create a custom adjustable values
                // with the same name but with the changed values in order to mimic this behavior.
                // Additionally, MS Word creates a copy of all preset values, even if only one value has been changed.
                if (!guides.HasCustomAdjustableValues() && guides.IsEditedPresetAdjustableValues(presetName))
                    guides.CreateCustomAdjustableValues();

                builder.StartElement("a:prstGeom");
                builder.WriteAttribute("prst", presetName);
                WriteGuides(guides.AdjustableValues, builder, "a:avLst");
                builder.EndElement("a:prstGeom");
            }
            else
            {
                builder.StartElement("a:custGeom");
                WriteGuides(guides.AdjustableValues, builder, "a:avLst");
                WriteGuides(guides.Guides, builder, "a:gdLst");
                WriteConnectionSites(geometry.ConnectionSites, builder, "a:cxnLst");

                // andrnosk: WORDSNET-12188 Write a:rect element to preserve text box appearance.
                if (geometry.DmlTextboxRect != null)
                {
                    WriteTextboxRect(geometry.DmlTextboxRect, builder);
                }

                WritePaths(geometry.Paths, builder);
                builder.EndElement("a:custGeom");
            }
        }

        /// <summary>
        /// Writes Text Rectangle (L.4.9.4.4.2)
        /// The text rectangle defines where text resides within the shape.
        /// </summary>
        private static void WriteTextboxRect(DmlTextBoxRect dmlTextboxRec, NrxXmlBuilder builder)
        {
            builder.StartElement("a:rect");
            builder.WriteAttribute("l", dmlTextboxRec.Left);
            builder.WriteAttribute("t", dmlTextboxRec.Top);
            builder.WriteAttribute("r", dmlTextboxRec.Right);
            builder.WriteAttribute("b", dmlTextboxRec.Bottom);
            builder.EndElement("a:rect");
        }

        internal static void WriteGuides(IList<DmlGuide> guides, NrxXmlBuilder builder, string rootName)
        {
            builder.StartElement(rootName);

            foreach (DmlGuide guide in guides)
                WriteGuide(guide, builder);

            builder.EndElement(rootName);
        }

        /// <summary>
        /// Writes connection sites of the shape.
        /// </summary>
        /// <param name="connectionSites">Connection sites collection.</param>
        /// <param name="builder">Builder which writes to output file.</param>
        /// <param name="rootName">Root name for connection sites collection.</param>
        private static void WriteConnectionSites(IList<DmlConnectionSite> connectionSites, NrxXmlBuilder builder,
            string rootName)
        {
            Debug.Assert((connectionSites != null) && (builder != null));

            if (connectionSites.Count == 0)
                return;

            const string siteRoot = "a:cxn";
            builder.StartElement(rootName);

            foreach (DmlConnectionSite site in connectionSites)
            {
                builder.StartElement(siteRoot);
                builder.WriteAttribute("ang", site.Angle.String);
                WriteShapePosition(site.Coordinates, builder, "a:pos");
                builder.EndElement(siteRoot);
            }

            builder.EndElement(rootName);
        }

        /// <summary>
        /// Writes position coordinate within the shape bounding box.
        /// </summary>
        /// <param name="coord">Shape position coordinate.</param>
        /// <param name="builder">Builder which writes to output file.</param>
        /// <param name="rootName">Root name for position coordinate.</param>
        private static void WriteShapePosition(DmlAdjustablePoint coord, NrxXmlBuilder builder, string rootName)
        {
            Debug.Assert(coord != null);

            builder.StartElement(rootName);
            builder.WriteAttribute("x", coord.X.String);
            builder.WriteAttribute("y", coord.Y.String);
            builder.EndElement(rootName);
        }

        private static void WriteGuide(DmlGuide guide, NrxXmlBuilder builder)
        {
            if (!guide.IsPreset)
                builder.WriteElementWithAttributes("a:gd", "name", guide.Name, "fmla", guide.Formula.Source);
        }

        private static void WritePaths(IList<DmlPath> paths, NrxXmlBuilder builder)
        {
            builder.StartElement("a:pathLst");

            foreach (DmlPath path in paths)
                WritePath(path, builder);

            builder.EndElement("a:pathLst");
        }

        private static void WritePath(DmlPath path, NrxXmlBuilder builder)
        {
            builder.StartElement("a:path");
            builder.WriteAttribute("fill", DmlEnum.PathFillModeToDml(path.FillMode));

            if (path.HeightDefined)
                builder.WriteAttribute("h", path.Height);
            if (path.WidthDefined)
                builder.WriteAttribute("w", path.Width);

            builder.WriteAttribute("stroke", path.Stroke);

            // According to spec: if this attribute is omitted then a value of 0,
            // or false is assumed. So, write "extrusionOk" when value is true.
            builder.WriteAttributeIfTrue("extrusionOk", path.ExtrusionOk);

            foreach (IDmlPathPart part in path.PathParts)
                part.Write(builder);

            builder.EndElement("a:path");
        }
    }
}
