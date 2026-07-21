// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/06/2012 by Alexey Butalov

using System;
using System.Text.RegularExpressions;

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Base class for bullet styles
    /// </summary>
    internal abstract class BulletTxtNumberingStyleBase : TxtNumberingStyle
    {
        protected BulletTxtNumberingStyleBase(char bulletChar, bool onlySpaceSeparateBullet)
            : base(true, false, NumberStyle.Bullet)
        {
            mBulletChar = bulletChar;

            string delimiterPattern = (onlySpaceSeparateBullet)
                ? @"\s"
                : @"(\w|\s)";
            mRegex = new Regex(string.Format(@"^\u{0:X4}{1}", (int)mBulletChar, delimiterPattern), RegexOptions.Compiled);
        }

        protected BulletTxtNumberingStyleBase(char bulletChar)
            : this(bulletChar, false)
        {
        }

        internal override TxtNumberingInfo DetectNumbering(string text)
        {
            Match match = mRegex.Match(text);
            if (match.Success)
            {
                string[] numbers = new string[1];
                numbers[0] = Convert.ToString(mBulletChar);
                return new TxtNumberingInfo(numbers, numbers[0]);
            }
            return null;
        }

        internal override bool IsStartNumber(string value)
        {
            Debug.Assert(false, "Bullet style doesn't support this operation!");
            return (value.Length == 1) && (value[0] == mBulletChar);
        }

        internal override string GetNextNumber(string prevNumber)
        {
            Debug.Assert(false, "Bullet style doesn't support this operation!");
            return Convert.ToString(mBulletChar);
        }

        internal override string GetNumberFormat(int level)
        {
            Debug.Assert(level == 0, "Only level 0 is allowed!");
            return Convert.ToString(mBulletChar);
        }

        private readonly char mBulletChar;
        private readonly Regex mRegex;
    }
}
