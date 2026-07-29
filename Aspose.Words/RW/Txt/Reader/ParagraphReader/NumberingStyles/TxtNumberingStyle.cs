// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/06/2012 by Alexey Butalov

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Base numbering style class
    /// </summary>
    internal abstract class TxtNumberingStyle
    {
        protected TxtNumberingStyle(bool isBullet, bool isLevelsSupported, NumberStyle numberStyle)
        {
            IsBullet = isBullet;
            IsLevelsSupported = isLevelsSupported;
            NumberStyle = numberStyle;
        }

        /// <summary>
        /// Returns true is the value is first in the numbering set
        /// </summary>
        internal abstract bool IsStartNumber(string value);

        /// <summary>
        /// Returns the next numbering value in the sequence. Returns empty string for an illegal or last value in sequence.
        /// </summary>
        internal abstract string GetNextNumber(string prevNumber);

        /// <summary>
        /// Returns information about numbering if the numbering is detected, or null if it is not.
        /// </summary>
        internal abstract TxtNumberingInfo DetectNumbering(string text);

        /// <summary>
        /// Returns AW NumberFormat. level is zero-based.
        /// </summary>
        internal abstract string GetNumberFormat(int level);

        /// <summary>
        /// The numbering is bullet style (*,-)
        /// </summary>
        internal bool IsBullet { get; }

        /// <summary>
        /// The numbering is set style (1) 2) .. or a. b. c. ...)
        /// </summary>
        internal bool IsSet
        {
            get { return !IsBullet; }
        }

        /// <summary>
        /// Numbering supports levels (1.1.2. 1.1.3.)
        /// </summary>
        internal bool IsLevelsSupported { get; }

        /// <summary>
        /// AW number style
        /// </summary>
        internal NumberStyle NumberStyle { get; }

        /// <summary>
        /// Allows control the way of numbering detection. See <see cref="Aspose.Words.Loading.TxtLoadOptions.DetectNumberingWithWhitespaces"/> for more details.
        /// </summary>
        internal bool DetectNumberingWithWhitespaces
        {
            get { return mDetectNumberingWithWhitespaces; }
            set { mDetectNumberingWithWhitespaces = value; }
        }

        private bool mDetectNumberingWithWhitespaces = true;
    }
}
