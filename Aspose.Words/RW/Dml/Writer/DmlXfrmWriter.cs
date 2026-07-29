// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/07/2014 by Andrey Noskov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml.Transforms;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlXfrmWriter
    {
        /// <summary>
        /// Writes 2D Transform for Grouped Objects.
        /// </summary>
        internal static void Write(DmlTransform dmlTransform, IDmlShapeWriterContext writer)
        {
            Write("a:xfrm", dmlTransform, writer);
        }

        internal static void Write(string tagName, DmlTransform dmlTransform, IDmlShapeWriterContext writer)
        {
            if (dmlTransform == null)
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(tagName);

            // Write only non-zero rotation.
            // Value stores without fractional part in output XML.
            int rotVal = MathUtil.DoubleToInt(dmlTransform.Rotation.Value);
            builder.WriteAttributeIfNotZero("rot", rotVal);

            FlipOrientation orientation = dmlTransform.FlipOrientation;
            bool flipH = (orientation == FlipOrientation.Horizontal) || (orientation == FlipOrientation.Both);
            bool flipV = (orientation == FlipOrientation.Vertical) || (orientation == FlipOrientation.Both);

            builder.WriteAttributeIfTrue("flipH", flipH);
            builder.WriteAttributeIfTrue("flipV", flipV);

            DmlBasicsWriter.WriteXY("a:off", MathUtil.DoubleToInt(dmlTransform.X),
                MathUtil.DoubleToInt(dmlTransform.Y), builder);
            DmlBasicsWriter.WriteCxCy("a:ext", MathUtil.DoubleToInt(dmlTransform.Width),
                MathUtil.DoubleToInt(dmlTransform.Height), builder);

            DmlGroupTransform dmlGrpTransform = dmlTransform as DmlGroupTransform;
            if (dmlGrpTransform != null)
            {
                DmlBasicsWriter.WriteXY("a:chOff", MathUtil.DoubleToInt(dmlGrpTransform.ChildX),
                    MathUtil.DoubleToInt(dmlGrpTransform.ChildY), builder);
                DmlBasicsWriter.WriteCxCy("a:chExt", MathUtil.DoubleToInt(dmlGrpTransform.ChildWidth),
                    MathUtil.DoubleToInt(dmlGrpTransform.ChildHeight), builder);
            }

            builder.EndElement(tagName);
        }
    }
}
