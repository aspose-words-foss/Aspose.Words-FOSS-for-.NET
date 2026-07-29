// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/07/2020 by Anatoly Sidorenko

using System.Text;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Provides a failure-handling mechanism, called a fallback,
    /// for an encoded input byte sequence that cannot be converted to an output character.
    /// The fallback emits a user-specified replacement string instead of a decoded input byte sequence.  
    /// </summary>
    /// <remarks>
    /// If a Unicode code point cannot be represented using the selected encoding,
    /// this class replaces the replacement string instead of a decoded input byte sequence.
    /// </remarks>
    internal class DecoderReplacementFallBack : DecoderFallback
    {
        internal DecoderReplacementFallBack()
        {
            DefaultString = "?";
            mBuffer = new DecoderReplacementFallBackBuffer(DefaultString);
        }

        internal DecoderReplacementFallBack(string replacement)
        {
            DefaultString = replacement;
            mBuffer = new DecoderReplacementFallBackBuffer(DefaultString);
        }

        public string DefaultString
        {
            get;
            private set;
        }

        public override DecoderFallbackBuffer CreateFallbackBuffer()
        {
            return mBuffer;
        }

        public override int MaxCharCount
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return DefaultString.Length; }
        }

        private readonly DecoderReplacementFallBackBuffer mBuffer;
    }
}
