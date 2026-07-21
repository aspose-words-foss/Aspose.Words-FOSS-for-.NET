// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/01/2007 by Roman Korchagin

using System;
using System.Runtime.Serialization;
using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Thrown during document load, when the document format is not recognized or not supported by Aspose.Words.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/programming-with-documents/">Programming with Documents</a> documentation article.</para>
    /// </summary>
#if !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class UnsupportedFileFormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        internal UnsupportedFileFormatException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <javaName>UnsupportedFileFormatException(java.lang.String message, java.lang.Exception innerException)</javaName>
        internal UnsupportedFileFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
#if !NET8_0_OR_GREATER
        /// <summary>
        /// WORDSNET-24522 This constructor is needed for serialization.
        /// </summary>
        [JavaDelete("No exception serialization on Java.")]
        internal UnsupportedFileFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
