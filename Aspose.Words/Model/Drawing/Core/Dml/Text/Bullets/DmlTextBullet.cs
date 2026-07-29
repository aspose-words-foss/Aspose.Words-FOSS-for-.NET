// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2015 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Fills;

namespace Aspose.Words.Drawing.Core.Dml.Text.Bullets
{
    internal class DmlTextBullet
    {
        /// <summary>
        /// Clones this instance of bullet.
        /// </summary>
        internal DmlTextBullet Clone()
        {
            DmlTextBullet lhs = (DmlTextBullet)MemberwiseClone();

            if (mPictureBullet != null)
                lhs.mPictureBullet = mPictureBullet.Clone();

            return lhs;
        }

        /// <summary>
        /// Returns true if paragraph has bullet.
        /// </summary>
        public bool HasBullet
        {
            get { return (mBulletType != DmlTextBulletType.None); }
        }

        /// <summary>
        /// Returns true if bullet is picture bullet.
        /// </summary>
        public bool IsPictureBullet
        {
            get { return (mBulletType == DmlTextBulletType.Picture); }
        }

        /// <summary>
        /// Returns true if bullet is character bullet.
        /// </summary>
        public bool IsCharBullet
        {
            get { return (mBulletType == DmlTextBulletType.Char); }
        }

        /// <summary>
        /// Returns true if bullet is auto num bullet.
        /// </summary>
        public bool IsAutoNumBullet
        {
            get { return HasBullet && !IsPictureBullet && !IsCharBullet; }
        }

        /// <summary>
        /// Gets or sets bullet type.
        /// </summary>
        public DmlTextBulletType BulletType
        {
            get { return mBulletType; }
            set { mBulletType = value; }
        }

        /// <summary>
        /// Gets or sets picture bullet.
        /// </summary>
        public DmlBlip PictureBullet
        {
            get { return mPictureBullet; }
            set
            {
                if(value == null)
                    return;

                mPictureBullet = value;
                mBulletType = DmlTextBulletType.Picture;
            }
        }

        /// <summary>
        /// Gets or sets char bullet character.
        /// </summary>
        public string BulletChar
        {
            get { return mBulletChar; }
            set
            {
                if (!StringUtil.HasChars(value))
                    return;

                mBulletChar = value;
                mBulletType = DmlTextBulletType.Char;
            }
        }

        /// <summary>
        /// Specifies the number that starts a given sequence of automatically numbered bullets. 
        /// When the numbering is alphabetical, the number should map to the appropriate letter. 
        /// For instance 1 maps to 'a', 2 to 'b' and so on. If the numbers are larger than 26, 
        /// then multiple letters should be used. For instance 27 should be represented as 'aa' 
        /// and similarly 53 should be 'aaa'.
        /// </summary>
        public int StartAt
        {
            get { return mStartAt; }
            set { mStartAt = value; }
        }

        private DmlTextBulletType mBulletType = DmlTextBulletType.None;
        private DmlBlip mPictureBullet;
        private string mBulletChar;
        private int mStartAt = 1;
    }
}
