// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2014 by Alexey Morozov

using System.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Nrx.Writer
{
    /// <summary>
    /// Implements writing 'shapeDefaults' and 'hdrShapeDefaults' elements.
    /// </summary>
    internal static class NrxShapeDefaultsWriter
    {
        internal static void Write(NrxXmlBuilder builder, SaveInfo saveInfo, bool isHeaderFooterShapes)
        {
            IList<ShapeBase> connectorShapes = isHeaderFooterShapes
                ? saveInfo.ConnectorsHdr
                : saveInfo.ConnectorsBody;
            string tagName = isHeaderFooterShapes ? "w:hdrShapeDefaults" : "w:shapeDefaults";

            if (connectorShapes.Count == 0)
                return;

            builder.StartElement(tagName);
            builder.StartElement("o:shapelayout");
            builder.StartElement("o:rules");

            int ruleIndex = 1;
            foreach(ShapeBase connectorShape in connectorShapes)
            {
                ConnectorRule rule = (ConnectorRule)connectorShape.ShapePr[ShapeAttr.ConnectorRule];

                builder.StartElement("o:r");
                builder.WriteAttribute("id", string.Format("V:Rule{0}", ruleIndex));
                builder.WriteAttribute("type", "connector");
                builder.WriteAttribute("idref", FormatIdRef(connectorShape));

                // proxy element for start.
                builder.StartElement("o:proxy");
                builder.WriteAttributeString("start", "");
                builder.WriteAttribute("idref", FormatIdRef(saveInfo.Shapes[rule.ShapeAId]));
                builder.WriteAttribute("connectloc", rule.ShapeASite);
                builder.EndElement("o:proxy");

                // proxy element for end.
                builder.StartElement("o:proxy");
                builder.WriteAttributeString("end", "");
                builder.WriteAttribute("idref", FormatIdRef(saveInfo.Shapes[rule.ShapeBId]));
                builder.WriteAttribute("connectloc", rule.ShapeBSite);
                builder.EndElement("o:proxy");

                builder.EndElement("o:r");

                ruleIndex++;
            }

            builder.EndElement("o:rules");
            builder.EndElement("o:shapelayout");
            builder.EndElement(tagName);
        }

        private static string FormatIdRef(ShapeBase shape)
        {
            string shapeName = (string)shape.ShapePr[ShapeAttr.ShapeName];
            return string.Format("#{0}", StringUtil.HasChars(shapeName) ? shapeName : NrxXmlUtil.GetShapeId(shape));
        }
    }
}
