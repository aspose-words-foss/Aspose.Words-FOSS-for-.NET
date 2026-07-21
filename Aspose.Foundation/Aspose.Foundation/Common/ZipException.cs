// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/01/2017 by Konstantin Sidorenko

using System;
using System.Runtime.Serialization;
using Aspose.JavaAttributes;

namespace Aspose.Common
{
    /// <summary>
    /// Base class for all exceptions defined by and throw by the Aspose.Zip library.
    /// </summary>
    [JavaDelete("java.util.zip.ZipException is used instead. But the class is needed here for mapping.")]
    public class ZipException : Exception
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public ZipException() { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        public ZipException(String message) : base(message) { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        /// <param name="innerException">The innerException for this exception.</param>
        public ZipException(String message, Exception innerException)
            : base(message, innerException)
        { }
#if !NET8_0_OR_GREATER
        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="info">The serialization info for the exception.</param>
        /// <param name="context">The streaming context from which to deserialize.</param>
        protected ZipException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
#endif
    }
}