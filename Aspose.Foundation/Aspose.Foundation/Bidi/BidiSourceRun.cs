// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2014 by Victor Chebotok

namespace Aspose.Bidi
{
    /// <summary>
    /// Represents a run of text with corresponding bidi levels.
    /// This information is needed for the Unicode bidirectional algorithm.
    /// </summary>
    public class BidiSourceRun
    {
        public BidiSourceRun(string text)
            : this(text, new BidiLevelList())
        {
            // Empty constructor.
        }

        public BidiSourceRun(string text, BidiLevel level)
            : this(text, new BidiLevelList(level))
        {
            // Empty constructor.
        }

        public BidiSourceRun(string text, BidiLevelList levels)
            : this (text, levels, true, false)
        {
            // Empty constructor.
        }

        public BidiSourceRun(string text, bool hebrewScript, bool arabicScript)
            : this (text, new BidiLevelList(), hebrewScript, arabicScript)
        {
            // Empty constructor.
        }

        public BidiSourceRun(string text, BidiLevelList levels, bool useHebrewNumberSeparators, bool ignoreBidiTypeET)
        {
            Debug.Assert(levels != null);

            mText = text;
            mLevels = levels;
            mUseHebrewNumberSeparators = useHebrewNumberSeparators;
            mIgnoreBidiTypeET = ignoreBidiTypeET;
        }

        public string Text
        {
            get { return mText; }
        }

        /// <summary>
        /// Nested bidi levels opened for this run. From outermost to innermost.
        /// </summary>
        public BidiLevelList Levels
        {
            get { return mLevels; }
        }

        /// <summary>
        /// Indicates whether characters '+', '-' and '/' should be treated as number separators in RTL paragraphs.
        /// </summary>
        public bool UseHebrewNumberSeparators
        {
            get { return mUseHebrewNumberSeparators; }
        }

        /// <summary>
        /// Indicates whether characters of the <see cref="BidiCharacterType.ET"/> type
        /// should be treated as <see cref="BidiCharacterType.ON"/> characters in RTL paragraphs.
        /// </summary>
        public bool IgnoreBidiTypeET
        {
            get { return mIgnoreBidiTypeET; }
        }

        private readonly string mText;
        private readonly BidiLevelList mLevels;
        private readonly bool mUseHebrewNumberSeparators;
        private readonly bool mIgnoreBidiTypeET;
    }
}