// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2015 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Core.Dml.Text.Bullets
{
    internal class DmlTextBulletSize
    {
        public DmlTextBulletSize(double value, DmlTextBulletSizeType sizeType)
        {
            mValue = value;
            mSizeType = sizeType;
        }

        /// <summary>
        /// Clones this instance of bullet size.
        /// </summary>
        internal DmlTextBulletSize Clone()
        {
            return (DmlTextBulletSize)MemberwiseClone();
        }

        /// <summary>
        /// Returns bullet font size in half-points.
        /// </summary>
        public int GetBulletSize(RunPr runPr)
        {
            switch (mSizeType)
            {
                case DmlTextBulletSizeType.Points:
                {
                    DmlTextPoints points = new DmlTextPoints((int)Value);
                    return (int)(points.ValueInPoints * 2);
                }
                case DmlTextBulletSizeType.Percentage:
                {
                    double fraction = DmlPercentageUtil.FromDmlPercent(Value);
                    return (int)(fraction * runPr.Size);
                }
                case DmlTextBulletSizeType.FolowText:
                default:
                {
                    return runPr.Size;
                }
            }
        }

        public DmlTextBulletSizeType SizeType
        {
            get { return mSizeType; }
        }

        /// <summary>
        /// Flag indicates that bullet size must follow text size.
        /// </summary>
        public bool FollowText
        {
            get { return (mSizeType == DmlTextBulletSizeType.FolowText); }
        }

        /// <summary>
        /// Size value.
        /// </summary>
        public double Value
        {
            get { return mValue; }
        }

        private readonly DmlTextBulletSizeType mSizeType;
        private readonly double mValue;
    }
}
