// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

namespace Aspose.Bidi
{
    /// <summary>
    /// Represents BIDI-relative character data.
    /// </summary>
    /// <remarks>
    /// In original sources class name was CharData and it was private nested to <see cref="BidiParagraph"/>.
    /// It has been moved due to new use cases for usability.
    /// </remarks>
    public class BidiCharData
    {
        public char Char
        {
            get { return mChar; }
            set { mChar = value; }
        }

        public int EmbLevel // 0-62 => 6
        {
            get { return mEl; }
            set { mEl = value; }
        }

        public BidiCharacterType CharType // 0-18 => 5
        {
            get { return mCharType; }
            set { mCharType = value; }
        }

        private char mChar;
        private int mEl; // 0-62 => 6
        private BidiCharacterType mCharType; // 0-18 => 5
    }
}
