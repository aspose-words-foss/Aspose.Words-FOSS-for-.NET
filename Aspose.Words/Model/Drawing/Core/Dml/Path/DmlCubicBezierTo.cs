// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Path
{
    /// <summary>
    /// Represents a cubic bezier part.
    /// </summary>
    /// <remarks>
    /// 20.1.9.7 cubicBezTo (Draw Cubic Bezier Curve To)
    /// This element specifies to draw a cubic bezier curve along the specified points.
    /// To specify a cubic bezier curve there needs to be 3 points specified. The first
    /// two are control points used in the cubic bezier calculation and the last is the
    /// ending point for the curve. The coordinate system used for this kind of curve is the
    /// path coordinate system as this element is path specific.
    /// </remarks>
    internal class DmlCubicBezierTo : IDmlPathPart
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pathPoints">The path points.</param>
        internal DmlCubicBezierTo(DmlAdjustablePoint[] pathPoints)
        {
            if (pathPoints.Length > 0)
                ControlPoint1 = (pathPoints[0] != null) ? pathPoints[0] : new DmlAdjustablePoint();
            if (pathPoints.Length > 1)
                ControlPoint2 = (pathPoints[1] != null) ? pathPoints[1] : new DmlAdjustablePoint();
            if (pathPoints.Length > 2)
                EndPoint = (pathPoints[2] != null) ? pathPoints[2] : new DmlAdjustablePoint();
        }

        /// <summary>
        /// Clones this instance of <see cref="DmlCubicBezierTo"/>.
        /// </summary>
        public IDmlPathPart Clone()
        {
            DmlCubicBezierTo lhs = (DmlCubicBezierTo)MemberwiseClone();

            if (mControlPoint1 != null)
                lhs.mControlPoint1 = mControlPoint1.Clone();

            if (mControlPoint2 != null)
                lhs.mControlPoint2 = mControlPoint2.Clone();

            if (mEndPoint != null)
                lhs.mEndPoint = mEndPoint.Clone();

            return lhs;
        }

        public void Write(NrxXmlBuilder builder)
        {
            builder.StartElement("a:cubicBezTo");
            DmlWriterUtil.WriteDmlAdjustablePoint(ControlPoint1, builder);
            DmlWriterUtil.WriteDmlAdjustablePoint(ControlPoint2, builder);
            DmlWriterUtil.WriteDmlAdjustablePoint(EndPoint, builder);
            builder.EndElement("a:cubicBezTo");
        }

        public DmlPathPartType PathPartType
        {
            get { return DmlPathPartType.CubicBezierTo; }
        }

        /// <summary>
        /// Gets or sets the control point1.
        /// </summary>
        internal DmlAdjustablePoint ControlPoint1
        {
            get { return mControlPoint1; }
            set { mControlPoint1 = value; }
        }

        /// <summary>
        /// Gets or sets the control point2.
        /// </summary>
        internal DmlAdjustablePoint ControlPoint2
        {
            get { return mControlPoint2; }
            set { mControlPoint2 = value; }
        }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        internal DmlAdjustablePoint EndPoint
        {
            get { return mEndPoint; }
            set { mEndPoint = value; }
        }

        private DmlAdjustablePoint mControlPoint1;
        private DmlAdjustablePoint mControlPoint2;
        private DmlAdjustablePoint mEndPoint;
    }
}
