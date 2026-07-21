// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/01/2007 by Roman Korchagin

using System;
using System.Runtime.Serialization;
using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Thrown during document load, when the document appears to be corrupted and impossible to load.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/programming-with-documents/">Programming with Documents</a> documentation article.</para>
    /// </summary>
#if !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class FileCorruptedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        internal FileCorruptedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <javaName>FileCorruptedException(java.lang.String message, java.lang.Exception innerException)</javaName>
        private FileCorruptedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class with the standard error message.
        /// </summary>
        internal FileCorruptedException(Exception innerException)
            : this("The document appears to be corrupted and cannot be loaded.", innerException)
        {
        }
#if !NET8_0_OR_GREATER
        /// <summary>
        /// WORDSNET-24522 This constructor is needed for serialization.
        /// </summary>
        [JavaDelete("No exception serialization on Java.")]
        internal FileCorruptedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
