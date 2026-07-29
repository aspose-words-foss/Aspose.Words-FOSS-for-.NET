// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Path
{
    /// <summary>
    /// Represents a quadratic bezier part.
    /// </summary>
    /// <remarks>
    /// 20.1.9.21 quadBezTo (Draw Quadratic Bezier Curve To)
    /// This element specifies to draw a quadratic bezier curve along the specified points.
    /// To specify a quadratic bezier curve there needs to be 2 points specified.
    /// The first is a control point used in the quadratic bezier calculation and
    /// the last is the ending point for the curve. The coordinate system used for
    /// this type of curve is the path coordinate system as this element is path specific.
    /// </remarks>
    internal class DmlQuadraticBezierTo : IDmlPathPart
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pathPoints">The path points.</param>
        internal DmlQuadraticBezierTo(DmlAdjustablePoint[] pathPoints)
        {
            if (pathPoints.Length > 0)
                ControlPoint = (pathPoints[0] != null) ? pathPoints[0] : new DmlAdjustablePoint();
            if (pathPoints.Length > 1)
                EndPoint = (pathPoints[1] != null) ? pathPoints[1] : new DmlAdjustablePoint();
        }

        /// <summary>
        /// Clones this instance of <see cref="DmlQuadraticBezierTo"/>.
        /// </summary>
        public IDmlPathPart Clone()
        {
            DmlQuadraticBezierTo lhs = (DmlQuadraticBezierTo)MemberwiseClone();

            if (mControlPoint != null)
                lhs.mControlPoint = mControlPoint.Clone();

            if (mEndPoint != null)
                lhs.mEndPoint = mEndPoint.Clone();

            return lhs;
        }

        public void Write(NrxXmlBuilder builder)
        {
            builder.StartElement("a:quadBezTo");
            DmlWriterUtil.WriteDmlAdjustablePoint(ControlPoint, builder);
            DmlWriterUtil.WriteDmlAdjustablePoint(EndPoint, builder);
            builder.EndElement("a:quadBezTo");
        }

        public DmlPathPartType PathPartType
        {
            get { return DmlPathPartType.QuadraticBezierTo; }
        }

        /// <summary>
        /// Gets or sets the control point.
        /// </summary>
        internal DmlAdjustablePoint ControlPoint
        {
            get { return mControlPoint; }
            set { mControlPoint = value; }
        }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        internal DmlAdjustablePoint EndPoint
        {
            get { return mEndPoint; }
            set { mEndPoint = value; }
        }

        private DmlAdjustablePoint mControlPoint;
        private DmlAdjustablePoint mEndPoint;
    }
}
