// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2016 by Denis Shvydkiy

namespace Aspose.Drawing.Fonts
{
    /// <summary>
    /// This interface allows to specify alternative font metrics in DrFont class.
    /// </summary>
    public interface IDrFontMetrics
    {
        float GetCharWidthPoints(int c, float sizePoints);
        float GetRawCharWidthPoints(int c, float sizePoints);
        float GetTextWidthPoints(string text, float sizePoints);
        float AscentPoints { get; set; }
        float DescentPoints { get; set; }
        float LineSpacingPoints { get; set; }
        float AscentRawPoints { get; set; }
        float DescentRawPoints { get; set; }
    }
}
