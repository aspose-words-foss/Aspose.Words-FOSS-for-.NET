// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Path
{
    /// <summary>
    /// Represents an arcTo element.
    /// </summary>
    /// <remarks>
    /// 20.1.9.4 arcTo (Draw Arc To)
    /// This element specifies the existence of an arc within a shape path.
    /// It draws an arc with the specified parameters from the current pen position to the
    /// new point specified. An arc is a line that is bent based on the shape of a supposed circle.
    /// The length of this arc is determined by specifying both a start angle and an ending angle
    /// that act together to effectively specify an end point for the arc.
    /// </remarks>
    internal class DmlArcTo : IDmlPathPart
    {
        public DmlPathPartType PathPartType
        {
            get { return DmlPathPartType.ArcTo; }
        }

        /// <summary>
        /// Clones this instance of <see cref="DmlArcTo"/>.
        /// </summary>
        public IDmlPathPart Clone()
        {
            DmlArcTo lhs = (DmlArcTo)MemberwiseClone();

            if (mHeightRadius != null)
                lhs.mHeightRadius = mHeightRadius.Clone();

            if (mWidthRadius != null)
                lhs.mWidthRadius = mWidthRadius.Clone();

            // Cloning the DmlAdjustableAngle fields is skipped since they are immutable.

            return lhs;
        }

        private static double GetParametricAngle(double noScaledWidthRadius, double noScaledHeightRadius,  double angle)
        {
            return System.Math.Atan2(
                ((noScaledHeightRadius < 1) ? 1 : 1.0d / noScaledHeightRadius)  * System.Math.Sin(angle),
                ((noScaledWidthRadius < 1) ? 1 : 1.0d / noScaledWidthRadius) * System.Math.Cos(angle));
        }

        public void Write(NrxXmlBuilder builder)
        {
            builder.WriteElementWithAttributes("a:arcTo",
                "hR", HeightRadius.String,
                "wR", WidthRadius.String,
                "stAng", StartAngle.String,
                "swAng", SwingAngle.String);
        }

        /// <summary>
        /// Gets or sets the height radius.
        /// </summary>
        /// <remarks>
        /// This attribute specifies the height radius of the supposed circle being used to
        ///  draw the arc. This gives the circle a total height of (2 * hR). This total height
        /// could also be called it's vertical diameter as it is the diameter for the y axis only.
        /// </remarks>
        /// <value>The height radius.</value>
        internal DmlAdjustableCoordinate HeightRadius
        {
            get { return mHeightRadius; }
            set { mHeightRadius = value; }
        }

        /// <summary>
        /// Gets or sets the width radius.
        /// </summary>
        /// <remarks>
        /// This attribute specifies the width radius of the supposed circle being used to draw
        /// the arc. This gives the circle a total width of (2 * wR). This total width could also
        /// be called it's horizontal diameter as it is the diameter for the x axis only.
        /// </remarks>
        /// <value>The width radius.</value>
        internal DmlAdjustableCoordinate WidthRadius
        {
            get { return mWidthRadius; }
            set { mWidthRadius = value; }
        }

        /// <summary>
        /// Gets or sets the start angle.
        /// </summary>
        /// <remarks>
        /// Specifies the start angle for an arc. This angle specifies what angle along the supposed
        /// circle path is used as the start position for drawing the arc. This start angle is locked
        /// to the last known pen position in the shape path. Thus guaranteeing a continuos shape path.
        /// </remarks>
        /// <value>The start angle.</value>
        internal DmlAdjustableAngle StartAngle
        {
            get { return mStartAngle; }
            set { mStartAngle = value; }
        }

        /// <summary>
        /// Gets or sets the swing angle.
        /// </summary>
        /// <remarks>
        /// Specifies the swing angle for an arc. This angle specifies how far angle-wise along the
        /// supposed circle path the arc is extended. The extension from the start angle is always in
        /// the clockwise direction around the supposed circle.
        /// </remarks>
        /// <value>The swing angle.</value>
        internal DmlAdjustableAngle SwingAngle
        {
            get { return mSwingAngle; }
            set { mSwingAngle = value; }
        }

        private DmlAdjustableCoordinate mHeightRadius;
        private DmlAdjustableAngle mStartAngle;
        private DmlAdjustableAngle mSwingAngle;
        private DmlAdjustableCoordinate mWidthRadius;
    }
}
