// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/06/2018 by Victor Moskvitin

using System.Text;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Encapsulates a property indicating that all characters
    /// can be correctly encoded using the selected encoding.
    /// </summary>
    /// <remarks>
    /// An instance of this class is created and used by an instance of the <see cref="Encoding"/> class.
    /// For details see the MSDN document: http://msdn.microsoft.com/ru-ru/library/system.text.encoderfallbackbuffer.aspx
    /// </remarks>
    internal class EncoderErrorFallBackBuffer : EncoderFallbackBuffer
    {
        public override bool Fallback(char charUnknown, int index)
        {
            // this method is executed only if error occurs.
            mIsError = true;
            return false;
        }

        public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
        {
            mIsError = true;
            return false;
        }

        public override char GetNextChar()
        {
            return '\0';
        }

        public override bool MovePrevious()
        {
            return false;
        }

        public override int Remaining {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return 0; }
        }

        /// <summary>
        /// Whether an error has occurred during encoding process or not.
        /// </summary>
        internal bool IsError
        {
            get { return mIsError; }
        }

        private bool mIsError;
    }
}
