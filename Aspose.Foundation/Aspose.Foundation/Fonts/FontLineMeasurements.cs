// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/07/2016 by Konstantin Kornilov

namespace Aspose.Fonts
{
    /// <summary>
    /// Contains font line measurements like line spacing, ascent, descent.
    /// </summary>
    public class FontLineMeasurements
    {
        public FontLineMeasurements(int ascent, int descent, int lineSpacing)
        {
            mAscent = ascent;
            mDescent = descent;
            mLineSpacing = lineSpacing;
        }

        public int Ascent
        {
            get { return mAscent; }
        }

        public int Descent
        {
            get { return mDescent; }
        }

        public int LineSpacing
        {
            get { return mLineSpacing; }
        }

        public int CellHeight
        {
            get { return Ascent + Descent; }
        }

        private readonly int mAscent;
        private readonly int mDescent;
        private readonly int mLineSpacing;
    }
}
