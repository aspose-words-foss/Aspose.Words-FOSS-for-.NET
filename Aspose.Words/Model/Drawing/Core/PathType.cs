// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/08/2008 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Specifies how the individual pieces of a path should be interpreted.
    /// 
    /// Corresponds to 2.4.30 MSOPATHTYPE in the DOC spec.
    /// </summary>
    internal enum PathType
    {
        Unknown = -1,
        /// <summary>
        /// Add a straight line segment from the current end point to the new point.
        /// The number of points to process is equal to the number of segments. 
        /// The last point in the array becomes the new end point.
        /// </summary>
        LineTo = 0,
        /// <summary>
        /// For each segment, three points are used to draw a cubic Bezier curve. 
        /// The first two points are control points and the last point is the new end point. 
        /// The number of points consumed is three times the number of segments.
        /// </summary>
        CurveTo = 1,
        /// <summary>
        /// Start a new sub-path using a single point. The starting point becomes the current end point.
        /// The value of the segment field MUST be zero. The number of points used is one.
        /// </summary>
        MoveTo = 2,
        /// <summary>
        /// If the starting point and the end point are not the same a single straight line is drawn to 
        /// connect the starting point and ending point of the path.
        /// The number of segments MUST be one. The number of points used is zero.
        /// </summary>
        Close = 3,
        /// <summary>
        /// The end of the current path. All consecutive lines and fill values MUST be drawn before any 
        /// following path or line is drawn. 
        /// The number of segments MUST be zero. The number of points used is zero.
        /// </summary>
        End = 4,

        /// <summary>
        /// Escape path types are defined below and this is their starting value.
        /// 
        /// RK The spec says that escape path types store editing information (like whether or not to 
        /// allow the control points to be adjusted on a bezier segment), but they are NOT needed to 
        /// understand how to draw the path. But it seems to be some escape path types need to be drawn.
        /// </summary>
        EscapeBase = 0xa0,

        /// <summary>
        /// Adds additional points to the escape code that follows msopathEscapeExtension.
        /// RK No idea what this means.
        /// </summary>
        EscapeExtension =             EscapeBase + 0x00,
        /// <summary>
        /// The first point specifies the center of the ellipse.
        /// The second point specifies the starting radius in x and the ending radius in y. 
        /// The third point specifies the starting angle in the x value and the ending angle in the y value. 
        /// Angles are in degrees. The number of ellipse segments drawn is equal to the segment count 
        /// divided by three.
        /// </summary>
        AngleEllipseTo =        EscapeBase + 0x01,
        /// <summary>
        /// The first point specifies the center of the ellipse. 
        /// The second point specifies the starting radius in x and the ending radius in y. 
        /// The third point specifies the starting angle in the x value and the ending angle in the y value. 
        /// Angles are in degrees. The number of ellipse segments drawn is equal to the segment count 
        /// divided by three. The first point of the ellipse becomes the first point of a new path.
        /// </summary>
        AngleEllipse =          EscapeBase + 0x02,
        /// <summary>
        /// The first two points specify the bounding rectangle of the ellipse. 
        /// The second two points specify the radial vectors for the ellipse. 
        /// The radial vectors are cast from the center of the bounding rectangle. 
        /// The path will start at the point where the first radial vector intersects the bounding 
        /// rectangle to the point where the second radial vector intersects the bounding rectangle. 
        /// The drawing direction is always counterclockwise. If the path has already been started, 
        /// a line is drawn from the last point to the starting point of the arc; otherwise a new 
        /// path is started. The number of arc segments drawn is equal to the segment count 
        /// divided by four.
        /// </summary>
        ArcTo =                 EscapeBase + 0x03,
        /// <summary>
        /// The first two points specify the bounding rectangle of the ellipse. 
        /// The second two points specify the radial vectors for the ellipse. 
        /// The radial vectors are cast from the center of the bounding rectangle. 
        /// The path will start at the point where the first radial vector intersects the 
        /// bounding rectangle to the point where the second radial vector intersects the bounding rectangle. 
        /// The drawing direction is always counterclockwise. The number of arc segments drawn is equal 
        /// to the segment count divided by four.
        /// </summary>
        Arc =                   EscapeBase + 0x04,
        /// <summary>
        /// The first two points specify the bounding rectangle of the ellipse. The second two points 
        /// specify the radial vectors for the ellipse. The radial vectors are cast from the center 
        /// of the bounding rectangle. The path will start at the point where the first radial vector intersects
        /// the bounding rectangle to the point where the second radial vector intersects the bounding rectangle.
        /// The drawing direction is always clockwise. If the path has already been started, a line is 
        /// drawn from the last point to the starting point of the arc; otherwise a new path is started. 
        /// The number of arc segments drawn is equal to the segment count divided by four.
        /// </summary>
        ClockwiseArcTo =        EscapeBase + 0x05,
        /// <summary>
        /// The first two points specify the bounding rectangle of the ellipse. 
        /// The second two points specify the radial vectors for the ellipse. 
        /// The radial vectors are cast from the center of the bounding rectangle. 
        /// The path will start at the point where the first radial vector intersects the bounding 
        /// rectangle to the point where the second radial vector intersects the bounding rectangle. 
        /// The drawing direction is always clockwise. The number of arc segments drawn is equal to 
        /// the segment count divided by four. This escape code always starts a new path.
        /// </summary>
        ClockwiseArc =          EscapeBase + 0x06,
        /// <summary>
        /// Add an ellipse to the path from the current point to the next point starting. 
        /// The ellipse is drawn as a quadrant that starts as a tangent to the X axis. 
        /// Multiple elliptical quadrants are joined by a straight line. 
        /// The number of elliptical quadrants drawn is equal to the segment count.
        /// </summary>
        EllipticalQuadrantX =   EscapeBase + 0x07,
        /// <summary>
        /// Add an ellipse to the path from the current point to the next point starting. 
        /// The ellipse is drawn as a quadrant that starts as a tangent to the Y axis. 
        /// Multiple elliptical quadrants are joined by a straight line. 
        /// The number of elliptical quadrants drawn is equal to the segment count.
        /// </summary>
        EllipticalQuadrantY =   EscapeBase + 0x08,
        /// <summary>
        /// Each point defines a control point for a quadratic Bezier curve. 
        /// The number of control points is defined by the segment count.
        /// </summary>
        QuadraticBezier =       EscapeBase + 0x09,
        /// <summary>
        /// The path SHOULD not be filled, even if passed to a rendering routine that would 
        /// normally fill the path.
        /// </summary>
        NoFill =                EscapeBase + 0x0a,
        /// <summary>
        /// The path SHOULD not be drawn, even if passed to a rendering routine that 
        /// would normally draw the path.
        /// </summary>
        NoLine =                EscapeBase + 0x0b,
        /// <summary>
        /// For Bezier curve editing, vertex joints are calculated, are equal lengths, and are collinear. 
        /// The segment after the point is a line. The tangent is not visible.
        /// </summary>
        EscapeAutoLine =              EscapeBase + 0x0c,
        /// <summary>
        /// For Bezier curve editing, vertex joints are calculated, are equal lengths, and are collinear. 
        /// The segment after the point is a curve. The tangent is not visible.
        /// </summary>
        EscapeAutoCurve =             EscapeBase + 0x0d,
        /// <summary>
        /// For Bezier curve editing, vertex joints are not calculated, are not equal lengths, 
        /// and are not collinear. The segment after the point is a line. The tangent is visible.
        /// </summary>
        EscapeCornerLine =            EscapeBase + 0x0e,
        /// <summary>
        /// For Bezier curve editing, vertex joints are not calculated, are not equal lengths, 
        /// and are not collinear. The segment after the point is a line. The tangent is visible.
        /// </summary>
        EscapeCornerCurve =           EscapeBase + 0x0f,
        /// <summary>
        /// For Bezier curve editing, vertex joints are not calculated, are not equal lengths, 
        /// and are not collinear. The segment after the point is a line. The tangent is visible.
        /// </summary>
        EscapeSmoothLine =            EscapeBase + 0x10,
        /// <summary>
        /// For Bezier curve editing, vertex joints are not calculated, are not equal lengths, 
        /// and are not collinear. The segment after the point is a curve. The tangent is visible.
        /// </summary>
        EscapeSmoothCurve =           EscapeBase + 0x11,
        /// <summary>
        /// For Bezier curve editing, vertex joints are not calculated, are equal lengths, 
        /// and are not collinear. The segment after the point is a line. The tangent is visible.
        /// </summary>
        EscapeSymmetricLine =         EscapeBase + 0x12,
        /// <summary>
        /// For Bezier curve editing vertex joints are not calculated, are equal lengths, 
        /// and are not collinear. The segment after the point is a curve. The tangent is visible.
        /// </summary>
        EscapeSymmetricCurve =        EscapeBase + 0x13,
        /// <summary>
        /// For Bezier curve editing, vertex joints are calculated, are equal lengths and are collinear. 
        /// The tangent is not visible.
        /// </summary>
        EscapeFreeForm =              EscapeBase + 0x14,
        /// <summary>
        /// Sets a new fill color. A single point is used to represent the colors. 
        /// X is an OfficeArtCOLORREF that specifies the new foreground color. 
        /// Y is an OfficeArtCOLORREF that specifies the new background color.
        /// </summary>
        FillColor =             EscapeBase + 0x15,
        /// <summary>
        /// Sets a new line drawing color. A single point is used to represent the colors. 
        /// X is an OfficeArtCOLORREF that specifies the new foreground color. 
        /// Y is an OfficeArtCOLORREF that specifies the new background color.
        /// </summary>
        LineColor =             EscapeBase + 0x16
    }
}
