// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/03/2016 by Anatoly Sidorenko

/**
 * ******************************************************************************
 * Copyright (C) 2005-2014, International Business Machines Corporation and    *
 * others. All Rights Reserved.                                                *
 * ******************************************************************************
 */
// 25/03/16 port to C# by Anatoly Sidorenko

using System;

namespace Aspose.Charset
{
    /// <summary>
    /// This class represents a charset that has been identified by a CharsetDetector
    /// as a possible encoding for a set of input data.
    /// From an instance of this class, you can ask for a confidence level in the charset identification,
    /// or for Java Reader or string to access the original byte data in Unicode form.
    /// <para>Instances of this class are created only by CharsetDetectors.</para>
    /// Note:  this class has a natural ordering that is inconsistent with equals.
    /// The natural ordering is based on the match confidence value.
    /// stable version ICU 3.4
    /// </summary>
    public class CharsetMatch : IComparable<CharsetMatch>
    {
        /// <summary>
        /// Get an indication of the confidence in the charset detected.
        /// Confidence values range from 0-100, with larger numbers indicating
        /// a better match of the input data to the characteristics of the charset.
        /// <returns>Return the confidence in the charset match</returns>
        /// </summary>
        public int GetConfidence()
        {
            return mConfidence;
        }

        /// <summary>
        ///  Get the name of the detected charset.
        ///  The name will be one that can be used with other APIs on the
        ///  platform that accept charset names.  It is the "Canonical name"
        ///  as defined by the class java.nio.charset.Charset; for
        ///  charsets that are registered with the IANA charset registry,
        ///  this is the MIME-preferred registered name.
        ///
        ///  <returns> The name of the charset.</returns>
        ///  <see cref="java.nio.charset.Charset"/>
        ///  <see cref="java.io.InputStreamReader"/>
        /// </summary>
        public string GetName()
        {
            return mCharsetName;
        }

        /// <summary>
        ///  Get the ISO code for the language of the detected charset.
        ///
        ///  <returns>The ISO code for the language or <code>null</code> if the language cannot be determined.</returns>
        /// </summary>
        public string GetLanguage()
        {
            return mLanguage;
        }

        /// <summary>
        ///  Compare to other CharsetMatch objects.
        ///  Comparison is based on the match confidence value, which
        ///  allows CharsetDetector.detectAll() to order its results.
        ///
        ///  <para>other the CharsetMatch object to compare against.</para>
        ///  <returns>a negative integer, zero, or a positive integer as the
        ///  confidence level of this CharsetMatch
        ///  is less than, equal to, or greater than that of
        ///  the argument.</returns>
        /// </summary>
        public int CompareTo(CharsetMatch other)
        {
            if (other == null)
               return 1;

            int compareResult = 0;

            if (this.mConfidence > other.mConfidence)
                compareResult = 1;
            else if (this.mConfidence < other.mConfidence)
                compareResult = -1;

            return compareResult;
        }

        /// <summary>
        /// Constructor.  Implementation internal
        /// </summary>
        internal CharsetMatch(CharsetRecognizer rec, int conf)
        {
            mConfidence = conf;
            mCharsetName = rec.GetName();
            mLanguage = rec.GetLanguage();
        }

        /// <summary>
        /// Constructor. Implementation internal
        /// </summary>
        internal CharsetMatch(int conf, string csName, string lang)
        {
            mConfidence = conf;
            mCharsetName = csName;
            mLanguage = lang;
        }

        private readonly int mConfidence;
        private readonly string mCharsetName; // The name of the charset this CharsetMatch represents.  Filled in by the recognizer.
        private readonly string mLanguage; // The language, if one was determined by the recognizer during the detect operation.
    }
}
