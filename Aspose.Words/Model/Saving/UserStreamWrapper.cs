// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/04/2011 by Roman Korchagin

using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// This class exists to help resolve the problem of exposing Stream objects in the public API of event handler arguments
    /// <see cref="ImageSavingArgs"/> etc. In these classes the user can provide a Stream object
    /// and AW will need to write data into that stream object. On Java these streams need to be exposed as java.io.OutputStream.
    /// 
    /// To resolve this, the Java version of this class first writes into a MemoryStream and then, when writing is finished,
    /// it copies to the user-provided java.io.OutputStream. There is also a need to automatically close the stream if the
    /// user has specified this option. So this class abstracts (to make the calling code autoportable) all these concepts:
    /// 1. Creation of a temporary MemoryStream.
    /// 2. Copying from the MemoryStream to java.io.OutputStream.
    /// 3. Closing the user stream optionally.
    /// 
    /// The .NET version of this class is straightforward.
    /// </summary>
    internal class UserStreamWrapper
    {
#if PLAIN_JAVA
        //Java-added for java.io.OutputStream.
        UserStreamWrapper(java.io.OutputStream javaUserStream, boolean keepStreamOpen)
        {
            mJavaUserStream = javaUserStream;
            mKeepStreamOpen = keepStreamOpen;
        }
#endif

        internal UserStreamWrapper(Stream userStream, bool keepStreamOpen)
        {
            mUserStream = userStream;
            mKeepStreamOpen = keepStreamOpen;
        }

        /// <summary>
        /// Gets the user-provided stream into which the data needs to be written.
        /// Exists to make calling code autoportable.
        /// </summary>
        internal Stream BeginUserStream()
        {
#if PLAIN_JAVA
            // Java: create a temporary MemoryStream so our AW code can write to it first.
            if (mJavaUserStream != null)
                mUserStream = new com.aspose.ms.System.IO.MemoryStream();

#endif
            return mUserStream;
        }

        /// <summary>
        /// Performs whatever is necessary after data was written to the user-provided stream.
        /// Exists to make calling code autoportable.
        /// </summary>
        [JavaThrows(true)]
        internal void EndUserStream()
        {
#if PLAIN_JAVA
            if (mJavaUserStream != null)
            {
                // Java: if we created a temporary MemoryStream, then it is time to copy to the user-provided 
                // Java stream and optionally close.
                if (mUserStream != null)
                {
                    mUserStream.setPosition(0);
                    com.aspose.ms.java.IO.JavaOnlyStreamUtil.copyStream(mUserStream, mJavaUserStream);
                }
            
                if (!mKeepStreamOpen)
                    mJavaUserStream.close();
            }
            else
            {
                /* This handles the case where the output stream is not user-provided, but our own like when 
                    handling first document part of HTML or EPUB writing. */
                if (!mKeepStreamOpen)
                    mUserStream.close();
            }
#else
            if (!mKeepStreamOpen)
                mUserStream.Close();
#endif
        }

#if PLAIN_JAVA
        private java.io.OutputStream mJavaUserStream;
#endif
        private readonly Stream mUserStream;

        private readonly bool mKeepStreamOpen;
    }
}
