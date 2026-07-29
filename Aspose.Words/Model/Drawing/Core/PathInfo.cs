// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2015 by Andrey Noskov

using System;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Specifies how a series of points are to be interpreted to construct a path.
    /// 
    /// 2.2.53 MSOPATHINFO in the DOC SPEC
    /// 2.2.54 MSOPATHESCAPEINFO in the DOC SPEC
    /// </summary>
    internal class PathInfo
    {
        internal PathInfo() : this(Aspose.Words.Drawing.Core.PathType.Unknown, 0)
        {
        }

        internal PathInfo(PathType pathType, int segmentCount)
        {
            mPathType = pathType;
            mSegmentCount = segmentCount;
        }

        /// <summary>
        /// Specifies how this part of a path is to be drawn.
        /// </summary>
        internal PathType PathType
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mPathType; }
        }

        /// <summary>
        /// An unsigned integer that specifies the number of segments to process.
        /// For escape path types this is the number of points (not number of segments).
        /// </summary>
        internal int SegmentCount
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mSegmentCount; }
        }

        public override string ToString()
        {
            return string.Format("PathType: {0}, Count: {1}", PathType, SegmentCount);
        }

        /// <summary>
        /// Gets the total number of points this path info uses.
        /// </summary>
        internal int GetPointCount()
        {
            return GetPointsAlways() + GetPointsPerSegment() * SegmentCount;
        }

        /// <summary>
        /// Gets the number of points this path info uses regardless of the segment count value.
        /// </summary>
        internal int GetPointsAlways()
        {
            switch (PathType)
            {
                case PathType.MoveTo:
                    // This command uses 1 point, but the segment count is 0.
                    return 1;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the number of points this path info uses per segment.
        /// </summary>
        internal int GetPointsPerSegment()
        {
            switch (PathType)
            {
                case PathType.LineTo:
                    return 1;
                case PathType.CurveTo:
                    return 3;
                case PathType.MoveTo:
                case PathType.Close:
                case PathType.End:
                    return 0;
                case PathType.AngleEllipseTo:
                case PathType.AngleEllipse:
                case PathType.ArcTo:
                case PathType.Arc:
                case PathType.ClockwiseArcTo:
                case PathType.ClockwiseArc:
                case PathType.EllipticalQuadrantX:
                case PathType.EllipticalQuadrantY:
                case PathType.QuadraticBezier:
                    // Although some of these commands require 3 or 4 points, they always have segment count 
                    // equal to the number of points because for escape commands it is the point count.
                    return 1;
                case PathType.NoFill:
                case PathType.NoLine:
                case PathType.EscapeAutoLine:
                case PathType.EscapeAutoCurve:
                case PathType.EscapeCornerLine:
                case PathType.EscapeCornerCurve:
                case PathType.EscapeSmoothLine:
                case PathType.EscapeSmoothCurve:
                case PathType.EscapeSymmetricLine:
                case PathType.EscapeSymmetricCurve:
                case PathType.EscapeFreeForm:
                    return 0;
                case PathType.FillColor:
                case PathType.LineColor:
                    // My guess.
                    return 1;
                default:
                    throw new InvalidOperationException("Unrecognized command in geometry path.");
            }
        }

        private readonly PathType mPathType;
        private readonly int mSegmentCount;
    }
}
