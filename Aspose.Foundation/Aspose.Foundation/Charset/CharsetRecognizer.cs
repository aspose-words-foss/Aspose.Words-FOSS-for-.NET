// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2016 by Anatoly Sidorenko

/**
 * ******************************************************************************
 * Copyright (C) 2005-2014, International Business Machines Corporation and    *
 * others. All Rights Reserved.                                                *
 * ******************************************************************************
 */
// 28/03/16 port to C# by Anatoly Sidorenko

namespace Aspose.Charset
{
    /// <summary>
    /// Abstract class for recognizing a single charset.
    /// Part of the implementation of ICU's CharsetDetector.
    /// <p/>
    /// Each specific charset that can be recognized will have an instance
    /// of some subclass of this class.  All interaction between the overall
    /// CharsetDetector and the stuff specific to an individual charset happens
    /// via the interface provided here.
    /// <p/>
    /// Instances of CharsetDetector DO NOT have or maintain
    /// state pertaining to a specific match or detect operation.
    /// The WILL be shared by multiple instances of CharsetDetector.
    /// They encapsulate const charset-specific information.
    /// </summary>
    public abstract class CharsetRecognizer
    {
        /// <summary>
        /// Get the IANA name of this charset.
        ///
        /// <returns>the charset name.</returns>
        /// </summary>
        public abstract string GetName();

        /// <summary>
        /// Get the ISO language code for this charset.
        ///
        /// <returns>the language code, or <code>null</code> if the language cannot be determined.</returns> 
        /// </summary>
        public virtual string GetLanguage()
        {
            return null;
        }

        /// <summary>
        /// Test the match of this charset with the input text data
        /// which is obtained via the CharsetDetector object.
        ///
        /// <param name="det">The CharsetDetector, which contains the input text
        /// to be checked for being in this charset.</param>
        /// 
        /// <returns>A CharsetMatch object containing details of match
        /// with this charset, or null if there was no match.</returns> 
        /// </summary>
        public abstract CharsetMatch Match(CharsetDetector det);
    }
}
