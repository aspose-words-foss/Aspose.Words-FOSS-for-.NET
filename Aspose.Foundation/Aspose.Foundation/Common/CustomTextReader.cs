// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2012 by Andrey Soldatov
// 11/02/2016 by Vyacheslav Durin

using System;
using System.IO;
using System.Text;
using Aspose.Charset;

namespace Aspose.Common
{
    /// <summary>
    /// <para>
    /// The class is helper for detecting file format,
    /// it wraps a stream and provides convenient operations needed for a process of detection of a stream format.
    /// </para>
    /// <para>
    /// It's used both for textual and binary file formats. Although operations needed for textual and binary formats are
    /// quite different, there are some common operations which makes this combination acceptable.
    /// </para>
    /// </summary>
    public class CustomTextReader
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public CustomTextReader(Stream stream)
            : this(stream, null)
        {
        }

        /// <summary>
        /// Ctor. Allows specifying encoding of the input stream. Otherwise it will be autodetected.
        /// </summary>
        public CustomTextReader(Stream stream, Encoding encoding)
        {
            ResetReadLimit();
            mStream = stream;
            mStreamInitPos = stream.Position;
            mEncoding = encoding ?? CharsetDetector.Detect(mStream);

            // WORDSNET-24468 Fallback to UTF8, when encoding cannot be properly detected by CharsetDetector.
            if (mEncoding == null)
                mEncoding = Encoding.UTF8;

            ReadMore();

            // Detect BOM at the beginning to ignore it when reading even single-byte encodings.
            mBom = CodePage.GetBom(mDetectionBuffer);

            mTextBuffer = Encoding.GetString(mDetectionBuffer, mBom.Length, mBytesRead - mBom.Length);
        }

        /// <summary>
        /// Reverts the stream to initial position to make possible specific
        /// format readers to read a document from the current position.
        /// </summary>
        public void RewindStream()
        {
            mStream.Position = mStreamInitPos + Utf7BomShift;
        }

        #region Binary Operations

        /// <summary>
        /// Whether the stream has given byte prefix.
        /// String is used as parameter for convenience of prefix definition in code.
        /// </summary>
        public bool HasBinaryPrefix(string asciiString)
        {
            if (mBytesRead < asciiString.Length)
                return false;

            for (int i = 0; i < asciiString.Length; i++)
                if (mDetectionBuffer[i] != asciiString[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Whether the stream has given byte prefix.
        /// Byte array is used as parameter for convenience of prefix definition in code.
        /// </summary>
        public bool HasBinaryPrefix(byte[] bytes)
        {
            if (mBytesRead < bytes.Length)
                return false;

            for (int i = 0; i < bytes.Length; i++)
                if (mDetectionBuffer[i] != bytes[i])
                    return false;

            return true;
        }

        public bool ContainsBinary(byte[] bytes)
        {
            if (mBytesRead < bytes.Length)
                return false;

            bool found = false;

            for (int i = 0; i < mBytesRead - bytes.Length && !found; i++)
            {
                for (int j = 0; j < bytes.Length; j++)
                {
                    if (mDetectionBuffer[j + i] != bytes[j])
                    {
                        found = false;
                        break;
                    }

                    found = true;
                }
            }

            return found;
        }

        #endregion

        #region Text Operations

        /// <summary>
        /// <para>Sets the current encoding of textual stream to Windows Latin1 code page preparing
        /// the reader for <see cref="ReadChar"/> and other textual operations.</para>
        /// </summary>
        /// <remarks>
        /// <para>This mode is used by MhtmlFormatDetector.</para>
        /// </remarks>
        public void SetEncodingLatin()
        {
            ResetState();
            mTextBuffer = Encoding.GetString(mDetectionBuffer, mBom.Length, mBytesRead - mBom.Length);
        }

        /// <summary>
        /// <para>Sets the current encoding of textual stream to one determined by BOM (Byte Order Mark) preparing
        /// the reader for <see cref="ReadChar"/> and other textual operations.</para>
        /// </summary>
        /// <remarks>
        /// <para>This mode is used by FileFormatDetector.</para>
        /// </remarks>
        public void SetEncodingByBom()
        {
            ResetState();

            // WORDSNET-23607 Set encoding by BOM.
            int codePage = CodePage.GetByBom(mBom);
            if (codePage != CodePage.Unknown)
                mEncoding = EncodingUtil.GetEncoding(codePage);

            mTextBuffer = Encoding.GetString(mDetectionBuffer, mBom.Length, mBytesRead - mBom.Length);
        }

        /// <summary>
        /// <para>Increases a reading limit.</para>
        /// <para>One of goals of the class is to avoid reading of whole file during process of detection of the stream
        /// format. To achieve this, a reading limit is implemented. But there are some cases (which are responsibility
        /// of format detectors) when the reading limit should be increased.</para>
        /// </summary>
        public void ResetReadLimit()
        {
            if (mIsResetReadLimitAvailable)
                mReadLimit = 1024;
        }

        /// <summary>
        /// Checks whether prefix of the stream contains given value. Uses a part of the stream which is already read
        /// to internal buffer of the reader but not more than 512 characters.
        /// </summary>
        public bool Contains(string value)
        {
            return mTextBuffer.IndexOf(value, 0, Math.Min(mTextBuffer.Length, 512), StringComparison.Ordinal) != -1;
        }

        /// <summary>
        /// <para>Locks possibility to reset a reading limit.</para>
        /// <para>One of goals of the class is to avoid reading of whole file during process of detection of the stream format.
        /// To achieve this, a reading limit is implemented. But there are some cases (which are responsibility of format
        /// detectors) when the reading limit should not be increased even if <see cref="ResetReadLimit"/> is called.</para>
        /// <para>This command has a precedence over <see cref="ResetReadLimit"/>.</para>
        /// </summary>
        public void LockResetReadLimit()
        {
            mIsResetReadLimitAvailable = false;
        }

        /// <summary>
        /// <para>Reads one character from the stream.</para>
        /// <para>Calling side is responsible for check
        /// whether there is a character to read (use <see cref="HasChars"/>).</para>
        /// </summary>
        public char ReadChar()
        {
            if (!HasChars)
            {
                Debug.Fail("Cannot read behind a read limit.");
                return '\0';
            }

            mReadLimit--;
            return mTextBuffer[mTextCursor++];
        }

        /// <summary>
        /// <para>Rewind the reader one character back.</para>
        /// <para>Calling side is responsible for calling the method only after at least one <see cref="ReadChar"/>.</para>
        /// </summary>
        public void StepBack()
        {
            if (mTextCursor == 0)
                Debug.Fail("Cannot step back.");

            mReadLimit++;
            mTextCursor--;
        }

        /// <summary>
        /// <para>Rewind the reader to beginning.</para>
        /// </summary>
        public void StepFirst()
        {
            mTextCursor = 0;
            mIsResetReadLimitAvailable = true;
            ResetReadLimit();
        }

        /// <summary>
        /// Whether a character can be read by <see cref="ReadChar"/>.
        /// </summary>
        public bool HasChars
        {
            get
            {
                Debug.Assert(mTextCursor <= mTextBuffer.Length);

                if (mReadLimit <= 0)
                    return false;

                if (mTextCursor < mTextBuffer.Length)
                    return true;

                if (!IsReadCompletely && ReadMore())
                    mTextBuffer = Encoding.GetString(mDetectionBuffer, mBom.Length, mBytesRead - mBom.Length);

                return mTextCursor < mTextBuffer.Length;
            }
        }

        /// <summary>
        /// Returns the underlying stream. I hope it is okay and does not violate any encapsulation.
        /// </summary>
        public Stream Stream
        {
            get { return mStream; }
        }

        /// <summary>
        /// Whether whole stream was read to the buffer.
        /// </summary>
        public bool IsReadCompletely
        {
            get { return mBytesRead != mDetectionBuffer.Length; }
        }

        /// <summary>
        /// Encoding used to convert stream data to characters.
        /// </summary>
        public Encoding Encoding
        {
            get { return mEncoding; }
        }

        /// <summary>
        /// Gets a total number of read characters.
        /// </summary>
        public int TotalCharsRead
        {
            get { return mTextCursor; }
        }

        /// <summary>
        /// Gets character at the current cursor position within a buffer.
        /// </summary>
        public char CurChar
        {
            get
            {
                return (mTextCursor < mTextBuffer.Length) ? mTextBuffer[mTextCursor] : '\0';
            }
        }

        /// <summary>
        /// Gets character at the next cursor position within a buffer.
        /// </summary>
        public char NextChar
        {
            get
            {
                return (mTextCursor + 1 < mTextBuffer.Length) ? mTextBuffer[mTextCursor + 1] : '\0';
            }
        }

        #endregion

        public void ResetState()
        {
            mStream.Position = mStreamInitPos;
            mTextBuffer = null;
            mTextCursor = 0;
            mIsResetReadLimitAvailable = true;
            ResetReadLimit();
        }

        /// <summary>
        /// Invalidates current detection buffer and reads from the handled stream a new one which is 4 times longer.
        /// Reads 512-byte buffer for the first time.
        /// Returns whether any new data was read after increasing the buffer.
        /// </summary>
        /// <remarks>
        /// For the most test files, buffers of 512 and 2048 bytes are used.
        /// There is no limit for buffer length used during the detection process. However, a simple test shows that most
        /// of our tests require a buffer of 512 or 2048 bytes. Only TestJira5527 and TestJira5760 require a larger buffer.
        /// </remarks>
        private bool ReadMore()
        {
            if (IsReadCompletely)
                return false;

            int oldBytesRead = mBytesRead;
            mDetectionBuffer = new byte[(mDetectionBuffer.Length == 0) ? BufferInitialSize : mDetectionBuffer.Length * 4];
            ReadToExistingBuffer();
            return mBytesRead > oldBytesRead;
        }

        /// <summary>
        /// Reads data from the stream to the existing buffer
        /// until the end of the buffer or the end of the stream.
        /// </summary>
        private void ReadToExistingBuffer()
        {
            mStream.Position = mStreamInitPos;
            mBytesRead = 0;

            while (true)
            {
                int bytesRead = mStream.Read(mDetectionBuffer, mBytesRead, mDetectionBuffer.Length - mBytesRead);
                if (bytesRead == 0)
                    break;
                mBytesRead += bytesRead;
            }
        }

        /// <summary>
        /// .NET readers (used for loading files of specific formats) cannot recognize Utf7 BOM, so we shift the stream's
        /// position right after the Utf7 BOM when <see cref="RewindStream"/> is called.
        /// </summary>
        private int Utf7BomShift
        {
            get
            {
                // UTF7 BOM has unique length  among other BOMs, so we can use it here.
                return (mBom.Length == CodePage.Utf7Bom.Length) ? mBom.Length : 0;
            }
        }

        private readonly Stream mStream;
        private readonly long mStreamInitPos;

        private byte[] mDetectionBuffer = ArrayUtil.EmptyByteArray;
        private int mBytesRead;

        private string mTextBuffer;
        private int mTextCursor;

        private readonly byte[] mBom;

        private int mReadLimit;
        private bool mIsResetReadLimitAvailable = true;

        /// <summary>
        /// Size of the trial buffer that is read at the beginning of the stream. HTML 4.01 states that "meta charset"
        /// declaration "should appear as early as possible", however in HTML 5.0 it is restricted to the first 512 bytes.
        /// We saw a document containing the "meta charset" declaration truncated by 512-bytes edge (WORDSNET-9285).
        /// try-catch block was added to absorb the exception.
        /// References:
        /// http://www.w3.org/TR/REC-html40/charset.html#spec-char-encoding
        /// http://www.w3.org/TR/html5-diff/#character-encoding
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int BufferInitialSize = 512;
        private Encoding mEncoding;
    }
}
