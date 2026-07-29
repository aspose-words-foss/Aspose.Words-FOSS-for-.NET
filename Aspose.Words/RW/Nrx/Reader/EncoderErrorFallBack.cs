// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/06/2018 by Victor Moskvitin

using System.Text;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// A simple fallback testing that all characters can be correctly encoded
    /// using the selected encoding.
    /// </summary>
    /// <remarks>
    /// If a Unicode code point cannot be represented using the selected encoding,
    /// this class sets a flag showing that encoding operation has failed.
    /// </remarks>
    internal class EncoderErrorFallBack : EncoderFallback
    {
        internal EncoderErrorFallBack()
        {
            buffer = new EncoderErrorFallBackBuffer();
        }

        public override EncoderFallbackBuffer CreateFallbackBuffer()
        {
            return buffer;
        }

        public override int MaxCharCount
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return 0; }
        }

        /// <summary>
        /// Whether an error has occurred during encoding process or not.
        /// </summary>
        internal bool IsError
        {
            get
            {
                return buffer.IsError;
            }
        }

        private readonly EncoderErrorFallBackBuffer buffer;
    }
}
