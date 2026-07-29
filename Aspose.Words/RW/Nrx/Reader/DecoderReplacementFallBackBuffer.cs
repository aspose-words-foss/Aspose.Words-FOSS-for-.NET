// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/07/2020 by Anatoly Sidorenko

using System.Text;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Represents a substitute output string that is emitted when the original input byte sequence cannot be decoded.
    /// </summary>
    /// <remarks>
    /// An instance of this class is created and used by an instance of the <see cref="Encoding"/> class.
    /// For details see the MSDN document: http://msdn.microsoft.com/ru-ru/library/system.text.encoderfallbackbuffer.aspx
    /// </remarks>
    internal sealed class DecoderReplacementFallBackBuffer : DecoderFallbackBuffer
    {
        public DecoderReplacementFallBackBuffer(string defaultString)
        {
            mBuffer = defaultString.ToCharArray();
            mIndex = 0;
        }

        public override bool Fallback(byte[] bytesUnknown, int index)
        {
            mIndex = 0;
            return true;
        }

        public override char GetNextChar()
        {
            if (mIndex < mBuffer.Length)
                return mBuffer[mIndex++];
            else
                return '\0';
        }

        public override bool MovePrevious()
        {
            return false;
        }

        public override int Remaining
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return mBuffer.Length - mIndex; }
        }

        private readonly char[] mBuffer;
        private int mIndex;
    }
}
