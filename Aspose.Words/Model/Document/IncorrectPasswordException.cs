// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/11/2008 by Roman Korchagin

using System;
using System.Runtime.Serialization;
using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Thrown if a document is encrypted with a password and the password specified when opening the document is incorrect or missing.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/programming-with-documents/">Programming with Documents</a> documentation article.</para>
    /// </summary>
#if !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class IncorrectPasswordException : Exception
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        internal IncorrectPasswordException(string message) : base(message)
        {
        }
#if !NET8_0_OR_GREATER
        /// <summary>
        /// WORDSNET-24522 This constructor is needed for serialization.
        /// </summary>
        [JavaDelete("No exception serialization on Java.")]
        internal IncorrectPasswordException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
