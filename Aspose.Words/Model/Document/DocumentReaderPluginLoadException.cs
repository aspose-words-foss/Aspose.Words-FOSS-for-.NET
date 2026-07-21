// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/01/2020 by Alexey Noskov

using System;
using System.Runtime.Serialization;
using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Thrown during document load, when the plugin required for reading the document format cannot be loaded.
    /// </summary>
#if !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class DocumentReaderPluginLoadException : Exception
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        internal DocumentReaderPluginLoadException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <javaName>DocumentReaderPluginLoadException(java.lang.String message, java.lang.Exception innerException)</javaName>
        internal DocumentReaderPluginLoadException(string message, Exception innerException) : base(message, innerException)
        {
        }
#if !NET8_0_OR_GREATER
        /// <summary>
        /// WORDSNET-24522 This constructor is needed for serialization.
        /// </summary>
        [JavaDelete("No exception serialization on Java.")]
        protected DocumentReaderPluginLoadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
