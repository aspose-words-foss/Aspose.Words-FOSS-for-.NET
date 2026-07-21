// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/07/2020 by Anatoly Sidorenko

using System.Text;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Provides a failure handling mechanism, called a fallback, 
    /// for an input character that cannot be converted to an output byte sequence. 
    /// The fallback uses a user-specified replacement string instead of the original input character. 
    /// </summary>
    /// <remarks>
    /// If a Unicode code point cannot be represented using the selected encoding,
    /// this class replaces the replacement string instead of the original input character.
    /// </remarks>
    internal class EncoderReplacementFallBack : EncoderFallback
    {
        internal EncoderReplacementFallBack()
        {
            DefaultString = "?";
            mBuffer = new EncoderReplacementFallBackBuffer(DefaultString);
        }

        internal EncoderReplacementFallBack(string replacement)
        {
            DefaultString = replacement;
            mBuffer = new EncoderReplacementFallBackBuffer(DefaultString);
        }

        public string DefaultString
        {
            get;
            private set;
        }

        public override EncoderFallbackBuffer CreateFallbackBuffer()
        {
            return mBuffer;
        }

        public override int MaxCharCount
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return DefaultString.Length; }
        }

        private readonly EncoderReplacementFallBackBuffer mBuffer;
    }
}
