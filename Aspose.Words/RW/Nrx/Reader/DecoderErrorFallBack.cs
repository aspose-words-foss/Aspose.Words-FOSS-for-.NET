// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/06/2018 by Victor Moskvitin

using System.Text;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// A simple fallback testing that all characters can be decoded.
    /// </summary>
    internal class DecoderErrorFallBack : DecoderFallback
    {
        internal DecoderErrorFallBack()
        {
            buffer = new DecoderErrorFallbackBuffer();
        }

        public override DecoderFallbackBuffer CreateFallbackBuffer()
        {
            return buffer;
        }

        public override int MaxCharCount
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return 0; }
        }

        /// <summary>
        /// Whether an error has occurred during decoding process or not.
        /// </summary>
        internal bool IsError
        {
            get { return buffer.IsError; }
        }

        private readonly DecoderErrorFallbackBuffer buffer;
    }
}
