// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2015 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Core.Dml.Text.Bullets
{
    internal class DmlTextBulletColor
    {
        /// <summary>
        /// Clones this instance of bullet color.
        /// </summary>
        internal DmlTextBulletColor Clone()
        {
            DmlTextBulletColor lhs = (DmlTextBulletColor)MemberwiseClone();

            if (mColor != null)
                lhs.mColor = mColor.Clone();

            return lhs;
        }

        /// <summary>
        /// Flag indicates that bullet color must follow text color.
        /// </summary>
        public bool FollowText
        {
            get { return (mColor == null); }
        }

        /// <summary>
        /// Gets or sets bullet color. Returns null if bullet color must follow text color.
        /// </summary>
        public DmlColor Color
        {
            get { return mColor; }
            set
            {
                if(value==null)
                    return;

                mColor = value;
            }
        }

        private DmlColor mColor;
    }
}
