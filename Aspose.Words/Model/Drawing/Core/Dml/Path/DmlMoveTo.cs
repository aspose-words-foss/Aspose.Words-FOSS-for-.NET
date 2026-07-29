// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Path
{
    /// <summary>
    /// Represents a "move to" path part.
    /// </summary>
    /// <remarks>
    /// 20.1.9.14 moveTo (Move Path To)
    /// This element specifies a set of new coordinates to move the shape cursor to. This element is only
    /// used for drawing a custom geometry. When this element is utilized the pt element is used to specify
    /// a new set of shape coordinates that the shape cursor should be moved to. This does not draw a line
    /// or curve to this new position from the old position but simply move the cursor to a new starting
    /// position. It is only when a path drawing element such as lnTo is used that a portion of the path is drawn.
    /// </remarks>
    internal class DmlMoveTo : IDmlPathPart
    {
        internal DmlMoveTo()
        {
        }

        internal DmlMoveTo(DmlAdjustablePoint point)
        {
            mPoint = point;
        }

        /// <summary>
        /// Clones this instance of <see cref="DmlMoveTo"/>.
        /// </summary>
        public IDmlPathPart Clone()
        {
            DmlMoveTo lhs = (DmlMoveTo)MemberwiseClone();

            if (mPoint != null)
                lhs.mPoint = mPoint.Clone();

            return lhs;
        }

        public void Write(NrxXmlBuilder builder)
        {
            builder.StartElement("a:moveTo");
            DmlWriterUtil.WriteDmlAdjustablePoint(Point, builder);
            builder.EndElement("a:moveTo");
        }

        public DmlPathPartType PathPartType
        {
            get { return DmlPathPartType.MoveTo; }
        }

        internal DmlAdjustablePoint Point
        {
            get { return mPoint; }
            set { mPoint = value; }
        }

        private DmlAdjustablePoint mPoint;
    }
}
