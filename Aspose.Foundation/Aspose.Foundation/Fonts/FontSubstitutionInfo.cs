// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2016 by Victor Sotnikov

using Aspose.Fonts.TrueType;

namespace Aspose.Fonts
{
    /// <summary>
    /// Represents font information used in font substitution algorithms.
    /// </summary>
    public sealed class FontSubstitutionInfo
    {
        public FontSubstitutionInfo(FontPanose panose, FontUnicodeRanges unicodeRanges, 
            FontCodepageRanges codepageRanges, FontFamilyCore family, 
            FontPitchCore pitch, int charset, int weight)
        {
            Debug.Assert(panose != null);
            Debug.Assert(unicodeRanges != null);
            Debug.Assert(codepageRanges != null);

            mPanose = panose;
            mUnicodeRanges = unicodeRanges;
            mCodepageRanges = codepageRanges;
            mFamily = family;
            mPitch = pitch;
            mCharset = charset;
            mWeight = weight;
        }

        public FontPanose Panose
        {
            get { return mPanose; }
        }

        public FontUnicodeRanges UnicodeRanges
        {
            get { return mUnicodeRanges; }
        }

        public FontCodepageRanges CodepageRanges
        {
            get { return mCodepageRanges; }
        }

        public FontFamilyCore Family
        {
            get { return mFamily; }
        }

        public FontPitchCore Pitch
        {
            get { return mPitch; }
        }

        public int Charset
        {
            get { return mCharset; }
        }

        public int Weight
        {
            get { return mWeight; }
        }

        #region Equality members

        private bool Equals(FontSubstitutionInfo other)
        {
            return mPanose.Equals(other.mPanose) && mUnicodeRanges.Equals(other.mUnicodeRanges) &&
                   mCodepageRanges.Equals(other.mCodepageRanges) && (mFamily == other.mFamily) &&
                   (mPitch == other.mPitch) && (mCharset == other.mCharset) && (mWeight == other.mWeight);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((FontSubstitutionInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = mPanose.GetHashCode();
                hashCode = (hashCode * 397) ^ mUnicodeRanges.GetHashCode();
                hashCode = (hashCode * 397) ^ mCodepageRanges.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)mFamily;
                hashCode = (hashCode * 397) ^ (int)mPitch;
                hashCode = (hashCode * 397) ^ mCharset;
                hashCode = (hashCode * 397) ^ mWeight;
                return hashCode;
            }
        }

        #endregion

        private readonly FontPanose mPanose;
        private readonly FontUnicodeRanges mUnicodeRanges;
        private readonly FontCodepageRanges mCodepageRanges;
        private readonly FontFamilyCore mFamily;
        private readonly FontPitchCore mPitch;
        private readonly int mCharset;
        private readonly int mWeight;
    }
}