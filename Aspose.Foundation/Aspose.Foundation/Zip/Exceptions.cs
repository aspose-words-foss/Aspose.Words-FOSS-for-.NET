// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// Exceptions.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2008, 2009 Dino Chiesa and Microsoft Corporation.  
// All rights reserved.
//
// This code module is part of DotNetZip, a zipfile class library.
//
// ------------------------------------------------------------------
//
// This code is licensed under the Microsoft Public License. 
// See the file License.txt for the license details.
// More info on: http://dotnetzip.codeplex.com
//
// ------------------------------------------------------------------
//
// last saved (in emacs): 
// Time-stamp: <2009-August-14 00:52:07>
//
// ------------------------------------------------------------------
//
// This module defines exceptions used in the class library.
//

using System;

using System.Runtime.Serialization;
using Aspose.Common;

using Aspose.JavaAttributes;

namespace Aspose.Zip
{
    /// <summary>
    /// Issued when an <c>ZipEntry.ExtractWithPassword()</c> method is invoked
    /// with an incorrect password.
    /// </summary>
    [JavaDelete("For ZIP on Java we use a completely different implementation.")]
#if !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class BadPasswordException : ZipException
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public BadPasswordException() { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        public BadPasswordException(String message)
            : base(message)
        { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        /// <param name="innerException">The innerException for this exception.</param>
        public BadPasswordException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
#if !NET8_0_OR_GREATER
        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="info">The serialization info for the exception.</param>
        /// <param name="context">The streaming context from which to deserialize.</param>
        protected BadPasswordException(SerializationInfo info, StreamingContext context)
            : base(info, context)
          {  }
#endif
    }

    /// <summary>
    /// Indicates that a read was attempted on a stream, and bad or incomplete data was
    /// received.  
    /// </summary>
    [JavaDelete("Using java zip instead.")]
#if !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class BadReadException : ZipException
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public BadReadException() { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        public BadReadException(string message)
            : base(message)
        { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        /// <param name="innerException">The innerException for this exception.</param>
        public BadReadException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
#if !NET8_0_OR_GREATER
        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="info">The serialization info for the exception.</param>
        /// <param name="context">The streaming context from which to deserialize.</param>
        protected BadReadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
          {  }
#endif
    }

    /// <summary>
    /// Issued when an CRC check fails upon extracting an entry from a zip archive.
    /// </summary>
    [JavaDelete("Using java zip instead.")]
#if !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class BadCrcException : ZipException
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public BadCrcException() { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        public BadCrcException(String message)
            : base(message)
        { }

#if NOTUSED
        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        /// <param name="innerException">The innerException for this exception.</param>
        public BadCrcException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
#endif
#if !NET8_0_OR_GREATER
        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="info">The serialization info for the exception.</param>
        /// <param name="context">The streaming context from which to deserialize.</param>
        protected BadCrcException(SerializationInfo info, StreamingContext context)
            : base(info, context)
          {  }
#endif
    }

    /// <summary>
    /// Issued when errors occur saving a self-extracting archive.
    /// </summary>
    [JavaDelete("Using java zip instead.")]
#if !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class SfxGenerationException : ZipException
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public SfxGenerationException() { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        public SfxGenerationException(String message)
            : base(message)
        { }

#if NOTUSED
        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        /// <param name="innerException">The innerException for this exception.</param>
        public SfxGenerationException(String message, Exception innerException)
            : base(message, innerException)
        { }
#endif
#if !NET8_0_OR_GREATER
        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="info">The serialization info for the exception.</param>
        /// <param name="context">The streaming context from which to deserialize.</param>
        protected SfxGenerationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
          {  }
#endif
    }

    /// <summary>
    /// Indicates that an operation was attempted on a ZipFile which was not possible
    /// given the state of the instance. For example, if you call <c>Save()</c> on a ZipFile 
    /// which has no filename set, you can get this exception. 
    /// </summary>
    [JavaDelete("Using java zip instead.")]
#if !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class BadStateException : ZipException
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public BadStateException() { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        public BadStateException(String message)
            : base(message)
        { }

        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        /// <param name="innerException">The innerException for this exception.</param>
        public BadStateException(String message, Exception innerException)
            : base(message, innerException)
        {}
#if !NET8_0_OR_GREATER
        /// <summary>
        /// Come on, you know how exceptions work. Why are you looking at this documentation?
        /// </summary>
        /// <param name="info">The serialization info for the exception.</param>
        /// <param name="context">The streaming context from which to deserialize.</param>
        protected BadStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
          {  }
#endif
    }
}
