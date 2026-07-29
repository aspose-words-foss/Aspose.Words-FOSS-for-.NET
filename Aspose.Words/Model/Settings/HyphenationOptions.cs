// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/01/2016 by Alexander Zhiltsov

using System;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Allows to configure document hyphenation options.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-hyphenation/">Working with Hyphenation</a> documentation article.</para>
    /// </summary>
    public class HyphenationOptions
    {
        /// <summary>
        /// Makes a deep clone.
        /// </summary>
        internal HyphenationOptions Clone()
        {
            return (HyphenationOptions)this.MemberwiseClone();
        }

        /// <summary>
        /// Sets the <see cref="ConsecutiveHyphenLimit"/> property without checking input value.
        /// </summary>
        internal void SetConsecutiveHyphenLimitSafe(int value)
        {
            mConsecutiveHyphenLimit = value;
        }

        /// <summary>
        /// Sets the <see cref="HyphenationZone"/> property without checking input value.
        /// </summary>
        internal void SetHyphenationZoneSafe(int value)
        {
            mHyphenationZone = value;
        }

        /// <summary>
        /// Returns true if input value is acceptable value for <see cref="ConsecutiveHyphenLimit"/>.
        /// </summary>
        private static bool IsValidConsecutiveHyphenLimit(int value)
        {
            return (value >= 0) && (value <= 32767);
        }

        /// <summary>
        /// Returns true if input value is acceptable value for <see cref="HyphenationZone"/>.
        /// </summary>
        private static bool IsValidHyphenationZone(int value)
        {
            return (value > 0) && (value <= 31680);
        }

        /// <summary>
        /// Gets or sets value determining whether automatic hyphenation is turned on for the document.
        /// Default value for this property is <c>false</c>.
        /// </summary>
        public bool AutoHyphenation
        {
            get { return mAutoHyphenation; }
            set { mAutoHyphenation = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of consecutive lines that can end with hyphens.
        /// Default value for this property is 0.
        /// </summary>
        /// <remarks>
        /// <p>If value of this property is set to 0, any number of consecutive lines can end with hyphens.</p>
        /// <p>The property does not have effect when saving to fixed page formats e.g. PDF.</p>
        /// </remarks>
        public int ConsecutiveHyphenLimit
        {
            get { return mConsecutiveHyphenLimit; }
            set
            {
                if (!IsValidConsecutiveHyphenLimit(value))
                    throw new ArgumentOutOfRangeException("value");
                mConsecutiveHyphenLimit = value;
            }
        }

        /// <summary>        
        /// Gets or sets the distance in 1/20 of a point from the right margin within which you do not want
        /// to hyphenate words.
        /// Default value for this property is 360 (0.25 inch).
        /// </summary>
        /// <dev>
        /// MS Word does not hyphenate words starting inside the hyphenation zone. 
        /// A smaller zone reduces the raggedness of the right margin, but more words may require hyphens. 
        /// A larger zone increases the raggedness of the right margin, but fewer words may require hyphens.
        /// </dev>
        public int HyphenationZone
        {
            get { return mHyphenationZone; }
            set
            {
                if (!IsValidHyphenationZone(value))
                    throw new ArgumentOutOfRangeException("value");
                mHyphenationZone = value;
            }
        }

        /// <summary>
        /// Gets or sets value determining whether words written in all capital letters are hyphenated.
        /// Default value for this property is <c>true</c>.
        /// </summary>
        public bool HyphenateCaps
        {
            get { return mHyphenateCaps; }
            set { mHyphenateCaps = value; }
        }

        private bool mAutoHyphenation;
        private int mConsecutiveHyphenLimit = ConsecutiveHyphenLimitDefault;
        private int mHyphenationZone = HyphenationZoneDefault;
        private bool mHyphenateCaps = HyphenateCapsDefault;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int ConsecutiveHyphenLimitDefault = 0;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int HyphenationZoneDefault = 360;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const bool HyphenateCapsDefault = true;
    }
}
