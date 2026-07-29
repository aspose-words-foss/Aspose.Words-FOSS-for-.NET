// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2009 by Dmitry Vorobyev

using System.Text;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Unescapes a field token and optionally trims double quotes if present.
    /// Field tokens are stored as is, i.e. with escaping backslashes and optionally double quoted.
    /// When accessing the tokens, we should get rid of them by replacing the escaping backslashes and
    /// double quotes at the beginning and end.
    /// </summary>
    internal class FieldTokenDecoder
    {
        /// <summary>
        /// Decodes the specified token represented by a string.
        /// </summary>
        /// <remarks>
        /// The <see cref="DecodeToken(string,NodeRange,bool)"/> and <see cref="DecodeToken(RichString,NodeRange,bool)"/> methods must be identical.
        /// </remarks>
        internal static string DecodeToken(string token, NodeRange tokenRange, bool trimDoubleQuotes)
        {
            // If an argument range ends with a field end then it is considered to be a field result range,
            // that is its content should not be modified (i.e. escaped, etc.).
            if (FieldUtil.EndsWithFieldEnd(tokenRange))
                return token;

            FieldTokenDecoderOptions options = FieldTokenDecoderOptions.EscapeChars;
            options = FieldTokenDecoderOptionsUtil.WithTrimDoubleQuotes(options, trimDoubleQuotes);
            FieldTokenDecoder decoder = new FieldTokenDecoder(options);

            return decoder.DecodeTokenPart(token, false, false);
        }

        /// <summary>
        /// Decodes the specified token represented by a string.
        /// </summary>
        /// <remarks>
        /// The <see cref="DecodeToken(string,NodeRange,bool)"/> and <see cref="DecodeToken(RichString,NodeRange,bool)"/> methods must be identical.
        /// </remarks>
        internal static RichString DecodeToken(RichString token, NodeRange tokenRange, bool trimDoubleQuotes)
        {
            // If an argument range ends with a field end then it is considered to be a field result range,
            // that is its content should not be modified (i.e. escaped, etc.).
            if (FieldUtil.EndsWithFieldEnd(tokenRange))
                return token;

            FieldTokenDecoderOptions options = FieldTokenDecoderOptions.EscapeChars;
            options = FieldTokenDecoderOptionsUtil.WithTrimDoubleQuotes(options, trimDoubleQuotes);
            FieldTokenDecoder decoder = new FieldTokenDecoder(options);

            return decoder.DecodeTokenPart(token, false, false);
        }

        internal FieldTokenDecoder(FieldTokenDecoderOptions fieldTokenDecoderOptions)
        {
            mFieldTokenDecoderOptions = fieldTokenDecoderOptions;
        }

        /// <summary>
        /// Unescapes the specified string and also trims double quotes if instructed.
        /// </summary>
        /// <remarks>
        /// The <see cref="DecodeTokenPart(string,bool,bool)"/> and <see cref="DecodeTokenPart(RichString,bool,bool)"/> methods must be identical.
        /// </remarks>
        internal string DecodeTokenPart(string tokenPart, bool isInField, bool isSymbolicFont)
        {
            StringBuilder builder = null;

            int tokenPartLength = tokenPart.Length;

            for (int i = 0; i < tokenPartLength; i++)
            {
                char c = tokenPart[i];
                if (c == '\\' && !isInField)
                {
                    if (IsTokenCharEscaped || (!EscapeChars && HasProcessedContent))
                        builder = ProcessDecodedTokenChar(builder, c, i);

                    IsTokenCharEscaped = !IsTokenCharEscaped;
                }
                else if (!NeedTrimDoubleQuote(c, i, tokenPartLength, isInField, isSymbolicFont))
                {
                    // Unescaped double quotes may only appear at the beginning or end of the token,
                    // so if we encounter them, it is a subject to trim.
                    builder = ProcessDecodedTokenChar(builder, c, i);

                    IsTokenCharEscaped = false;
                }
            }

            return builder != null
                ? builder.ToString()
                : string.Empty;
        }

        /// <summary>
        /// Unescapes the specified string and also trims double quotes if instructed.
        /// </summary>
        /// <remarks>
        /// The <see cref="DecodeTokenPart(string,bool,bool)"/> and <see cref="DecodeTokenPart(RichString,bool,bool)"/> methods must be identical.
        /// </remarks>
        private RichString DecodeTokenPart(RichString tokenPart, bool isInField, bool isSymbolicFont)
        {
            RichStringBuilder builder = null;

            int tokenPartLength = tokenPart.Length;

            for (int i = 0; i < tokenPartLength; i++)
            {
                RichChar c = tokenPart.GetInternal(i);
                if (c.ToSystemChar() == '\\' && !isInField)
                {
                    if (IsTokenCharEscaped || (!EscapeChars && HasProcessedContent))
                        builder = ProcessDecodedTokenChar(builder, c, i);

                    IsTokenCharEscaped = !IsTokenCharEscaped;
                }
                else if (!NeedTrimDoubleQuote(c.ToSystemChar(), i, tokenPartLength, isInField, isSymbolicFont))
                {
                    // Unescaped double quotes may only appear at the beginning or end of the token,
                    // so if we encounter them, it is a subject to trim.
                    builder = ProcessDecodedTokenChar(builder, c, i);

                    IsTokenCharEscaped = false;
                }
            }

            return builder != null
                ? builder.ToRichString()
                : RichString.Empty;
        }

        private bool NeedTrimDoubleQuote(char tokenChar, int tokenCharIndex, int tokenLength, bool isInField, bool isSymbolicFont)
        {
            if (IsTokenCharEscaped)
                return false;

            if (isInField)
                return false;

            if (isSymbolicFont && PreserveSymbolicQuotes)
                return false;

            return NeedTrimDoubleQuote(tokenChar, tokenCharIndex, tokenLength);
        }

        /// <summary>
        /// Processes the specified decoded token char typically by appending it to the current decoded token buffer.
        /// </summary>
        /// <param name="builder">
        /// The current decoded token buffer.
        /// </param>
        /// <param name="c">
        /// The decoded character.
        /// </param>
        /// <param name="positionInTokenPart">
        /// The position of the decoded character in the token part, i.e. the text of the corresponding run.</param>
        /// <returns>
        /// The current decoded token buffer. Typically, if the input value is null, it is initialized within the method
        /// and the output value is a new instance of <see cref="IStringBuilder"/> class.
        /// </returns>
        /// <remarks>
        /// The <see cref="ProcessDecodedTokenChar(StringBuilder,char,int)"/> and <see cref="ProcessDecodedTokenChar(RichStringBuilder,RichChar,int)"/> methods must be identical.
        /// </remarks>
        protected virtual StringBuilder ProcessDecodedTokenChar(StringBuilder builder, char c, int positionInTokenPart)
        {
            // Create on the first demand.
            if (builder == null)
                builder = new StringBuilder();

            OnContentProcessed();

            return builder.Append(c);
        }

        /// <summary>
        /// Processes the specified decoded token char typically by appending it to the current decoded token buffer.
        /// </summary>
        /// <param name="builder">
        /// The current decoded token buffer.
        /// </param>
        /// <param name="c">
        /// The decoded character.
        /// </param>
        /// <param name="positionInTokenPart">
        /// The position of the decoded character in the token part, i.e. the text of the corresponding run.</param>
        /// <returns>
        /// The current decoded token buffer. Typically, if the input value is null, it is initialized within the method
        /// and the output value is a new instance of <see cref="IStringBuilder"/> class.
        /// </returns>
        /// <remarks>
        /// The <see cref="ProcessDecodedTokenChar(StringBuilder,char,int)"/> and <see cref="ProcessDecodedTokenChar(RichStringBuilder,RichChar,int)"/> methods must be identical.
        /// </remarks>
        protected virtual RichStringBuilder ProcessDecodedTokenChar(RichStringBuilder builder, RichChar c, int positionInTokenPart)
        {
            // Create on the first demand.
            if (builder == null)
                builder = new RichStringBuilder();

            OnContentProcessed();

            return builder.AppendInternal(c);
        }

        protected void OnContentProcessed()
        {
            if (HasProcessedContent)
                return;

            if (IsHasProcessedContentLocked)
                return;

            HasProcessedContent = true;
        }

        protected virtual bool IsHasProcessedContentLocked
        {
            get { return false; }
        }

        private bool NeedTrimDoubleQuote(char tokenChar, int tokenCharIndex, int tokenLength)
        {
            if (!TrimDoubleQuotes)
                return false;

            if (tokenCharIndex != 0 && tokenCharIndex != tokenLength - 1)
                return false;

            return FieldCodeParser.IsDoubleQuote(tokenChar);
        }

        /// <summary>
        /// Gets a value indicating whether the current processed character is escaped.
        /// </summary>
        /// <remarks>
        /// The returned value is always <c>false</c> if the token char belongs to a run inside a nested field.
        /// </remarks>
        protected bool IsTokenCharEscaped { get; set; }

        protected bool HasProcessedContent { get; set; }

        private bool PreserveSymbolicQuotes
        {
            get { return FieldTokenDecoderOptionsUtil.HasPreserveSymbolicQuotes(mFieldTokenDecoderOptions); }
        }

        private bool TrimDoubleQuotes
        {
            get { return FieldTokenDecoderOptionsUtil.HasTrimDoubleQuotes(mFieldTokenDecoderOptions); }
        }

        private bool EscapeChars
        {
            get { return FieldTokenDecoderOptionsUtil.HasEscapeChars(mFieldTokenDecoderOptions); }
        }

        private readonly FieldTokenDecoderOptions mFieldTokenDecoderOptions;
    }
}
