// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2015 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.Text.Bullets
{
    internal class DmlTextBulletFont
    {
        /// <summary>
        /// Clones this instance of bullet font.
        /// </summary>
        internal DmlTextBulletFont Clone()
        {
            DmlTextBulletFont lhs = (DmlTextBulletFont)MemberwiseClone();

            if (mFont != null)
                lhs.mFont = mFont.Clone();

            return lhs;
        }

        /// <summary>
        /// Flag indicates that bullet font must follow text font.
        /// </summary>
        public bool FollowText
        {
            get { return (mFont == null); }
        }

        /// <summary>
        /// Gets or sets bullet font. Returns null if bullet font must follow text font.
        /// </summary>
        public DmlFont Font
        {
            get { return mFont; }
            set
            {
                if (value == null)
                    return;

                mFont = value;
            }
        }

        private DmlFont mFont;
    }
}
