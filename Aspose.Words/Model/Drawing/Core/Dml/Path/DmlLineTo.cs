// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Path
{
    /// <summary>
    /// Represents a "line to" path part.
    /// </summary>
    /// <remarks>
    /// 20.1.9.13 lnTo (Draw Line To)
    /// This element specifies the drawing of a straight line from the current pen position to the
    /// new point specified. This line becomes part of the shape geometry, representing a side of the shape.
    /// The coordinate system used when specifying this line is the path coordinate system.
    /// </remarks>
    internal class DmlLineTo : IDmlPathPart
    {
        internal DmlLineTo()
        {
        }

        internal DmlLineTo(DmlAdjustablePoint point)
        {
            mPoint = point;
        }

        /// <summary>
        /// Clones this instance of <see cref="DmlLineTo"/>.
        /// </summary>
        public IDmlPathPart Clone()
        {
            DmlLineTo lhs = (DmlLineTo)MemberwiseClone();

            if (mPoint != null)
                lhs.mPoint = mPoint.Clone();

            return lhs;
        }

        public void Write(NrxXmlBuilder builder)
        {
            builder.StartElement("a:lnTo");
            DmlWriterUtil.WriteDmlAdjustablePoint(Point, builder);
            builder.EndElement("a:lnTo");
        }

        public DmlPathPartType PathPartType
        {
            get { return DmlPathPartType.LineTo; }
        }

        internal DmlAdjustablePoint Point
        {
            get { return mPoint; }
            set { mPoint = value; }
        }

        private DmlAdjustablePoint mPoint;
    }
}
