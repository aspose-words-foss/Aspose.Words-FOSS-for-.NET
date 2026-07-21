// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

using System.Text;

namespace Aspose.Bidi
{
    /// <summary>
    /// Run of Bidi text
    /// </summary>
    public class BidiRun
    {
        public BidiRun(string text, int level)
            : this(text, level, 0)
        {
            // Empty constructor.
        }

        public BidiRun(string text, int level, int sourceRunIndex)
        {
            mRunLevel = level;
            mRunText = text;
            mSourceRunIndex = sourceRunIndex;
        }

        public string Text
        {
            get { return mRunText; }
        }

        public bool Rtl
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return (mRunLevel % 2) != 0; }
        }

        public int Level
        {
            get { return mRunLevel; }
        }

        public int SourceRunIndex
        {
            get { return mSourceRunIndex; }
        }

        public bool IsNumber
        {
            get { return mIsNumber; }
            set { mIsNumber = value; }
        }

        public bool HasHebrewNumberSeparators
        {
            get { return mHasHebrewNumberSeparators; }
            set { mHasHebrewNumberSeparators = value; }
        }

        public bool RequiresHebrewLocaleBi
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return Rtl && mIsNumber && mHasHebrewNumberSeparators; }
        }

        public BidiCharacterType BidiCharacterType
        {
            get { return mBidiCharacterType; }
            set { mBidiCharacterType = value; }
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string direction = (Rtl)
                ? "RTL"
                : "LTR";
            sb.Append(direction);
            sb.Append(": |");
            sb.Append(mRunText);
            sb.Append("|");

            if (RequiresHebrewLocaleBi)
            {
                sb.Append(" - Hebrew");
            }

            return sb.ToString();
        }
#endif

        private readonly string mRunText;

        private readonly int mRunLevel;

        private readonly int mSourceRunIndex;

        private bool mIsNumber;

        private bool mHasHebrewNumberSeparators;

        private BidiCharacterType mBidiCharacterType;
    }
}
