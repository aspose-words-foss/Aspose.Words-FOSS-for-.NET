// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2013 by Victor Chebotok

using System;
using System.Text;
using Aspose.Bidi;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Parses HTML text into tokens according to HTML 5 rules.
    /// </summary>
    /// <remarks>
    /// This class implements the tokenizer described here:
    /// http://www.w3.org/TR/html5/syntax.html#tokenization
    /// </remarks>
    internal class HtmlTokenizer
    {
        /// <summary>
        /// Creates and initializes a new instance of the class.
        /// </summary>
        /// <param name="html">HTML text that will be parsed into tokens.</param>
        /// <param name="supportedFeatures">
        /// Features that are considered supported when reading IE conditional comments.
        /// Setting this parameter to <c>null</c> disables support for conditional comments, like in modern browers.
        /// </param>
        internal HtmlTokenizer(string html, Features supportedFeatures)
        {
            mStream = new HtmlStream(html);
            mState = ParseState.Data;
            mSupportedFeatures = supportedFeatures;
        }

        /// <summary>
        /// Parses the HTML text and returns the next token.
        /// </summary>
        /// <param name="htmlContent">When this flag is <c>true</c>, CDATA sections are parsed as comments and are eventually
        /// ignored (parsing of HTML content). When this flag is <c>false</c>, CDATA sections are parsed as text (parsing of XML
        /// fragments: SVG and MathML).
        /// </param>
        /// <returns>
        /// The next parsed token. If the end of the HTML text is reached, an instance of <see cref="HtmlEndOfFileToken"/>
        /// is returned.
        /// </returns>
        internal HtmlToken NextToken(bool htmlContent)
        {
            Debug.Assert(mText.Length == 0);

            if (mEmittedToken == null)
            {
                Parse(htmlContent);
            }

            // The parser does not stop on every character token. Instead it accumulates all the text characters and returns
            // only on markup tokens. Here we check if some characters should be emitted before the markup token. The token
            // itself will be stored and emitted on the next call to the method.
            if (mText.Length > 0)
            {
                string text = mText.ToString();
                mText.Length = 0;
                return new HtmlTextToken(text);
            }
            Debug.Assert(mEmittedToken != null);

            // Switching parser to special states such as RCDATA, RAWTEXT, and others is only allowed
            // after a start tag has been emitted. Remember that a start tag token has just been emitted.
            HtmlTagToken tagToken = mEmittedToken as HtmlTagToken;
            mJustEmittedStartTagName = ((tagToken != null) && tagToken.IsStart)
                ? tagToken.Name
                : string.Empty;

            // Emit the next token and 'forget' it, but do not 'forget' an EOF token 
            // to prevent reading beyond the end of file.
            HtmlToken result = mEmittedToken;
            if (mEmittedToken.Type != HtmlTokenType.EndOfFile)
            {
                mEmittedToken = null;
            }
            return result;
        }

        /// <summary>
        /// Switches the tokenizer to the RCDATA state.
        /// </summary>
        /// <remarks>
        /// The state can be switched only just after a start tag has been returned by the tokenizer.
        /// </remarks>
        internal void SwitchToRcdataState()
        {
            SwitchToState(ParseState.Rcdata);
        }

        /// <summary>
        /// Switches the tokenizer to the PLAINTEXT state.
        /// </summary>
        /// <remarks>
        /// The state can be switched only just after a start tag has been returned by the tokenizer.
        /// </remarks>
        internal void SwitchToPlaintextState()
        {
            SwitchToState(ParseState.Plaintext);
        }

        /// <summary>
        /// Switches the tokenizer to the RAWTEXT state.
        /// </summary>
        /// <remarks>
        /// The state can be switched only just after a start tag has been returned by the tokenizer.
        /// </remarks>
        internal void SwitchToRawtextState()
        {
            SwitchToState(ParseState.Rawtext);
        }

        /// <summary>
        /// Switches the tokenizer to the SCRIPT state.
        /// </summary>
        /// <remarks>
        /// The state can be switched only just after a start tag has been returned by the tokenizer.
        /// </remarks>
        internal void SwitchToScriptDataState()
        {
            SwitchToState(ParseState.ScriptData);
        }

        /// <summary>
        /// States of the finite-state automaton used to parse core HTML markup.
        /// </summary>
        private enum ParseState
        {
            AfterAttributeValueQuoted,
            AfterAttributeName,
            AttributeName,
            AttributeValueDoubleQuoted,
            AttributeValueUnquoted,
            AttributeValueSingleQuoted,
            BeforeAttributeName,
            BeforeAttributeValue,
            Comment,
            BogusComment,
            UnmatchedRevealedConditionalComment,
            CdataSection,
            Data,
            Doctype,
            EndTagOpen,
            MarkupDeclarationOpen,
            TagName,
            TagOpen,
            Rcdata,
            Rawtext,
            Plaintext,
            ScriptData,
            SelfClosingStartTag
        }

        /// <summary>
        /// States of the finite-state automaton used to parse DOCTYPE tokens.
        /// </summary>
        private enum DoctypeParseState
        {
            BeforeName,
            Name,
            AfterName,
            AfterPublicIdentifier,
            AfterPublicKeyword,
            AfterSystemIdentifier,
            AfterSystemKeyword,
            Bogus
        }

        /// <summary>
        /// States of the finite-state automaton used to parse ID strings of DOCTYPE tokens.
        /// </summary>
        private enum DoctypeIdParseState
        {
            Initial,
            DoubleQuoted,
            SingleQuoted
        }

        /// <summary>
        /// States of the finite-state automaton used to parse text of SCRIPT areas.
        /// </summary>
        private enum ScriptParseState
        {
            Data,
            DoubleEscaped,
            DoubleEscapedDash,
            DoubleEscapedDashDash,
            DoubleEscapedLessThanSign,
            DoubleEscapeEnd,
            DoubleEscapeStart,
            EndTagName,
            EndTagOpen,
            Escaped,
            EscapedDash,
            EscapedDashDash,
            EscapedLessThanSign,
            EscapedEndTagOpen,
            EscapedEndTagName,
            EscapeStart,
            EscapeStartDash,
            LessThanSign
        }

        /// <summary>
        /// States of the finite-state automaton used to parse text of RCDATA and RAWTEXT areas.
        /// </summary>
        private enum TextParseState
        {
            Text,
            LessThanSign,
            EndTagOpen,
            EndTagName
        }

        /// <summary>
        /// States of the finite-state automaton used to parse text of comments.
        /// </summary>
        private enum CommentParseState
        {
            Start,
            StartDash,
            Text,
            End,
            EndDash,
            EndBang,
        }

        /// <summary>
        /// A result of parsing and matching a conditional expression.
        /// </summary>
        private enum ConditionalExpressionConsumeResult
        {
            /// <summary>
            /// Not a conditional comment (no '[if...' part).
            /// </summary>
            NotFound,
            /// <summary>
            /// A conditional comment with an invalid expression that we failed to parse.
            /// </summary>
            Invalid,
            /// <summary>
            /// A conditional comment with an expression that doesn't match the set of features we support.
            /// </summary>
            NotMatched,
            /// <summary>
            /// A conditional comment with an expression that matches the set of features we support.
            /// </summary>
            Matched
        }

        /// <summary>
        /// Switches the tokenizer to the specified state.
        /// </summary>
        /// <remarks>
        /// The state can be switched only just after a start tag has been returned by the tokenizer.
        /// </remarks>
        private void SwitchToState(ParseState state)
        {
            Debug.Assert(mJustEmittedStartTagName.Length > 0);
            mState = state;
        }

        /// <summary>
        /// Reads the HTML text until a token is parsed or the end of the text is reached.
        /// </summary>
        private void Parse(bool htmlContent)
        {
            do
            {
                mStream.ConsumeChar();

                switch (mState)
                {
                    case ParseState.Data:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '&':
                                mText.Append(ConsumeCharacterReference(false));
                                break;
                            case '<':
                                mState = ParseState.TagOpen;
                                break;
                            case HtmlStream.EofChar:
                                EmitEof();
                                return;
                            default:
                            {
                                // NULL characters (U+0000) are not replaced in text outside HTML markup.
                                mText.Append(mStream.CurrentChar);
                                break;
                            }
                        }
                        break;
                    }
                    case ParseState.TagOpen:
                    {
                        if (StringUtil.IsLetter(mStream.CurrentChar))
                        {
                            CreateTag(true);
                            mTagName.Append(StringUtil.AsciiLowerCase(mStream.CurrentChar));
                            mState = ParseState.TagName;
                        }
                        else
                        {
                            switch (mStream.CurrentChar)
                            {
                                case '!':
                                    mState = ParseState.MarkupDeclarationOpen;
                                    break;
                                case '/':
                                    mState = ParseState.EndTagOpen;
                                    break;
                                case '?':
                                {
                                    mState = ParseState.BogusComment;
                                    mStream.UnconsumeChar();
                                    break;
                                }
                                default:
                                {
                                    mState = ParseState.Data;
                                    mText.Append('<');
                                    mStream.UnconsumeChar();
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    case ParseState.MarkupDeclarationOpen:
                    {
                        HtmlStreamBookmark bookmark = mStream.SetBookmark();
                        mStream.UnconsumeChar();
                        if (TryConsumeOrdinal("--"))
                        {
                            if (TryConsumeConditionalExpression() == ConditionalExpressionConsumeResult.Matched)
                            {
                                EmitToken(CreateComment(mStream.GetBookmarkedText(bookmark)));
                                mState = ParseState.Data;
                                return;
                            }
                            mState = ParseState.Comment;
                        }
                        else if (TryConsumeAsciiCaseInsensitive("DOCTYPE"))
                        {
                            mState = ParseState.Doctype;
                        }
                        else if ((!htmlContent) && TryConsumeOrdinal("[CDATA["))
                        {
                            mState = ParseState.CdataSection;
                        }
                        else
                        {
                            switch (TryConsumeConditionalExpression())
                            {
                                case ConditionalExpressionConsumeResult.NotFound:
                                    mState = ParseState.BogusComment;
                                    break;
                                case ConditionalExpressionConsumeResult.Invalid:
                                case ConditionalExpressionConsumeResult.NotMatched:
                                    // Unlike IE, Word hides revealed comments that contain invalid expressions. So do we.
                                    mState = ParseState.UnmatchedRevealedConditionalComment;
                                    break;
                                case ConditionalExpressionConsumeResult.Matched:
                                    EmitToken(CreateComment(mStream.GetBookmarkedText(bookmark)));
                                    mState = ParseState.Data;
                                    return;
                                default:
                                    // Error. Unexpected result.
                                    throw new InvalidOperationException();
                            }
                        }
                        break;
                    }
                    case ParseState.Comment:
                    {
                        EmitToken(ParseComment());
                        mState = ParseState.Data;
                        return;
                    }
                    case ParseState.BogusComment:
                    {
                        EmitToken(ParseBogusComment());
                        mState = ParseState.Data;
                        return;
                    }
                    case ParseState.UnmatchedRevealedConditionalComment:
                    {
                        EmitToken(ParseUnmatchedRevealedComment());
                        mState = ParseState.Data;
                        return;
                    }
                    case ParseState.EndTagOpen:
                    {
                        if (StringUtil.IsLetter(mStream.CurrentChar))
                        {
                            CreateTag(false);
                            mTagName.Append(StringUtil.AsciiLowerCase(mStream.CurrentChar));
                            mState = ParseState.TagName;
                        }
                        else
                        {
                            switch (mStream.CurrentChar)
                            {
                                case '>':
                                    mState = ParseState.Data;
                                    break;
                                case HtmlStream.EofChar:
                                {
                                    mState = ParseState.Data;
                                    mText.Append("</");
                                    mStream.UnconsumeChar();
                                    break;
                                }
                                default:
                                {
                                    mState = ParseState.BogusComment;
                                    mStream.UnconsumeChar();
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    case ParseState.CdataSection:
                    {
                        ParseCData();
                        mState = ParseState.Data;
                        break;
                    }
                    case ParseState.TagName:
                    {
                        char lowerCaseChar = StringUtil.AsciiLowerCase(mStream.CurrentChar);
                        switch (lowerCaseChar)
                        {
                            case '\t':
                            case '\n':
                            case '\f':
                            case ' ':
                                mState = ParseState.BeforeAttributeName;
                                break;
                            case '/':
                                mState = ParseState.SelfClosingStartTag;
                                break;
                            case '>':
                            {
                                mState = ParseState.Data;
                                EmitTag();
                                return;
                            }
                            case HtmlStream.EofChar:
                            {
                                mState = ParseState.Data;
                                mStream.UnconsumeChar();
                                break;
                            }
                            default:
                                mTagName.Append(ReplaceNull(lowerCaseChar));
                                break;
                        }
                        break;
                    }
                    case ParseState.BeforeAttributeName:
                    {
                        char lowerCaseChar = StringUtil.AsciiLowerCase(mStream.CurrentChar);
                        switch (lowerCaseChar)
                        {
                            case '\t':
                            case '\n':
                            case '\f':
                            case ' ':
                                break;
                            case '/':
                                mState = ParseState.SelfClosingStartTag;
                                break;
                            case '>':
                            {
                                mState = ParseState.Data;
                                EmitTag();
                                return;
                            }
                            case HtmlStream.EofChar:
                            {
                                mState = ParseState.Data;
                                mStream.UnconsumeChar();
                                break;
                            }
                            default:
                            {
                                CreateAttribute();
                                mAttributeName.Append(ReplaceNull(lowerCaseChar));
                                mState = ParseState.AttributeName;
                                break;
                            }
                        }
                        break;
                    }
                    case ParseState.AttributeName:
                    {
                        char lowerCaseChar = StringUtil.AsciiLowerCase(mStream.CurrentChar);
                        switch (lowerCaseChar)
                        {
                            case '\t':
                            case '\n':
                            case '\f':
                            case ' ':
                                mState = ParseState.AfterAttributeName;
                                break;
                            case '/':
                                mState = ParseState.SelfClosingStartTag;
                                break;
                            case '=':
                                mState = ParseState.BeforeAttributeValue;
                                break;
                            case '>':
                            {
                                mState = ParseState.Data;
                                EmitTag();
                                return;
                            }
                            case HtmlStream.EofChar:
                            {
                                mState = ParseState.Data;
                                mStream.UnconsumeChar();
                                break;
                            }
                            default:
                            {
                                mAttributeName.Append(ReplaceNull(lowerCaseChar));
                                mState = ParseState.AttributeName;
                                break;
                            }
                        }
                        break;
                    }
                    case ParseState.AfterAttributeName:
                    {
                        char lowerCaseChar = StringUtil.AsciiLowerCase(mStream.CurrentChar);
                        switch (lowerCaseChar)
                        {
                            case '\t':
                            case '\n':
                            case '\f':
                            case ' ':
                                break;
                            case '/':
                                mState = ParseState.SelfClosingStartTag;
                                break;
                            case '=':
                                mState = ParseState.BeforeAttributeValue;
                                break;
                            case '>':
                            {
                                mState = ParseState.Data;
                                EmitTag();
                                return;
                            }
                            case HtmlStream.EofChar:
                            {
                                mState = ParseState.Data;
                                mStream.UnconsumeChar();
                                break;
                            }
                            default:
                            {
                                CreateAttribute();
                                mAttributeName.Append(ReplaceNull(lowerCaseChar));
                                mState = ParseState.AttributeName;
                                break;
                            }
                        }
                        break;
                    }
                    case ParseState.BeforeAttributeValue:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '\t':
                            case '\n':
                            case '\f':
                            case ' ':
                                break;
                            case '"':
                                mState = ParseState.AttributeValueDoubleQuoted;
                                break;
                            case '&':
                            {
                                mState = ParseState.AttributeValueUnquoted;
                                mStream.UnconsumeChar();
                                break;
                            }
                            case '\'':
                                mState = ParseState.AttributeValueSingleQuoted;
                                break;
                            case '>':
                            {
                                mState = ParseState.Data;
                                EmitTag();
                                return;
                            }
                            case HtmlStream.EofChar:
                            {
                                mState = ParseState.Data;
                                mStream.UnconsumeChar();
                                break;
                            }
                            default:
                            {
                                mAttributeValue.Append(ReplaceNull(mStream.CurrentChar));
                                mState = ParseState.AttributeValueUnquoted;
                                break;
                            }
                        }
                        break;
                    }
                    case ParseState.AttributeValueDoubleQuoted:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '"':
                                mState = ParseState.AfterAttributeValueQuoted;
                                break;
                            case '&':
                                mAttributeValue.Append(ConsumeCharacterReference(true));
                                break;
                            case HtmlStream.EofChar:
                            {
                                mState = ParseState.Data;
                                mStream.UnconsumeChar();
                                break;
                            }
                            default:
                                mAttributeValue.Append(ReplaceNull(mStream.CurrentChar));
                                break;
                        }
                        break;
                    }
                    case ParseState.AttributeValueSingleQuoted:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '\'':
                                mState = ParseState.AfterAttributeValueQuoted;
                                break;
                            case '&':
                                mAttributeValue.Append(ConsumeCharacterReference(true));
                                break;
                            case HtmlStream.EofChar:
                            {
                                mState = ParseState.Data;
                                mStream.UnconsumeChar();
                                break;
                            }
                            default:
                                mAttributeValue.Append(ReplaceNull(mStream.CurrentChar));
                                break;
                        }
                        break;
                    }
                    case ParseState.AttributeValueUnquoted:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '\t':
                            case '\n':
                            case '\f':
                            case ' ':
                                mState = ParseState.BeforeAttributeName;
                                break;
                            case '&':
                                mAttributeValue.Append(ConsumeCharacterReference(true));
                                break;
                            case '>':
                            {
                                mState = ParseState.Data;
                                EmitTag();
                                return;
                            }
                            case HtmlStream.EofChar:
                            {
                                mState = ParseState.Data;
                                mStream.UnconsumeChar();
                                break;
                            }
                            default:
                                mAttributeValue.Append(ReplaceNull(mStream.CurrentChar));
                                break;
                        }
                        break;
                    }
                    case ParseState.AfterAttributeValueQuoted:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '\t':
                            case '\n':
                            case '\f':
                            case ' ':
                                mState = ParseState.BeforeAttributeName;
                                break;
                            case '/':
                                mState = ParseState.SelfClosingStartTag;
                                break;
                            case '>':
                            {
                                mState = ParseState.Data;
                                EmitTag();
                                return;
                            }
                            case HtmlStream.EofChar:
                            {
                                mState = ParseState.Data;
                                mStream.UnconsumeChar();
                                break;
                            }
                            default:
                            {
                                mState = ParseState.BeforeAttributeName;
                                mStream.UnconsumeChar();
                                break;
                            }
                        }
                        break;
                    }
                    case ParseState.SelfClosingStartTag:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '>':
                            {
                                mState = ParseState.Data;
                                mTagIsSelfClosing = true;
                                EmitTag();
                                return;
                            }
                            case HtmlStream.EofChar:
                            {
                                mState = ParseState.Data;
                                mStream.UnconsumeChar();
                                break;
                            }
                            default:
                            {
                                mState = ParseState.BeforeAttributeName;
                                mStream.UnconsumeChar();
                                break;
                            }
                        }
                        break;
                    }
                    case ParseState.Doctype:
                    {
                        EmitToken(ParseDoctype());
                        mState = ParseState.Data;
                        return;
                    }
                    case ParseState.Rcdata:
                    {
                        ParseTextArea(true);
                        mState = ParseState.Data;
                        break;
                    }
                    case ParseState.Plaintext:
                    {
                        while (mStream.CurrentChar != HtmlStream.EofChar)
                        {
                            mText.Append(ReplaceNull(mStream.CurrentChar));
                            mStream.ConsumeChar();
                        }
                        EmitEof();
                        return;
                    }
                    case ParseState.Rawtext:
                    {
                        ParseTextArea(false);
                        mState = ParseState.Data;
                        break;
                    }
                    case ParseState.ScriptData:
                    {
                        ParseScript();
                        mState = ParseState.Data;
                        break;
                    }
                    default:
                        // SQ fix: nothing to do.
                        break;
                }
            } while (mStream.CurrentChar != HtmlStream.EofChar);
        }

        /// <summary>
        /// Parses a DOCTYPE token.
        /// </summary>
        /// <returns>
        /// The parsed DOCTYPE token, which is never <c>null</c>.
        /// </returns>
        private HtmlDoctypeToken ParseDoctype()
        {
            StringBuilder name = new StringBuilder();
            string publicId = null;
            string systemId = null;
            DoctypeParseState state = DoctypeParseState.BeforeName;

            while ((mStream.CurrentChar != HtmlStream.EofChar) && (mStream.CurrentChar != '>'))
            {
                switch (state)
                {
                    case DoctypeParseState.BeforeName:
                    {
                        if (!IsWhitespace(mStream.CurrentChar))
                        {
                            name.Append(ReplaceNull(StringUtil.AsciiLowerCase(mStream.CurrentChar)));
                            state = DoctypeParseState.Name;
                        }
                        break;
                    }
                    case DoctypeParseState.Name:
                    {
                        if (IsWhitespace(mStream.CurrentChar))
                        {
                            state = DoctypeParseState.AfterName;
                        }
                        else
                        {
                            name.Append(ReplaceNull(StringUtil.AsciiLowerCase(mStream.CurrentChar)));
                        }
                        break;
                    }
                    case DoctypeParseState.AfterName:
                    {
                        if (!IsWhitespace(mStream.CurrentChar))
                        {
                            mStream.UnconsumeChar();
                            if (TryConsumeAsciiCaseInsensitive("PUBLIC"))
                            {
                                state = DoctypeParseState.AfterPublicKeyword;
                            }
                            else if (TryConsumeAsciiCaseInsensitive("SYSTEM"))
                            {
                                state = DoctypeParseState.AfterSystemKeyword;
                            }
                            else
                            {
                                state = DoctypeParseState.Bogus;
                            }
                        }
                        break;
                    }
                    case DoctypeParseState.AfterPublicKeyword:
                    {
                        HtmlParsedDoctypeId parsedId = ParseDoctypeId();
                        publicId = parsedId.Value;
                        // There must be a public ID after the PUBLIC keyword.
                        state = ((parsedId.Value != null) && parsedId.Correct)
                            ? DoctypeParseState.AfterPublicIdentifier
                            : DoctypeParseState.Bogus;
                        break;
                    }
                    case DoctypeParseState.AfterPublicIdentifier:
                    {
                        HtmlParsedDoctypeId parsedId = ParseDoctypeId();
                        systemId = parsedId.Value;
                        // There may be no system ID after a public ID.
                        state = parsedId.Correct
                            ? DoctypeParseState.AfterSystemIdentifier
                            : DoctypeParseState.Bogus;
                        break;
                    }
                    case DoctypeParseState.AfterSystemKeyword:
                    {
                        HtmlParsedDoctypeId parsedId = ParseDoctypeId();
                        systemId = parsedId.Value;
                        // There must be a system ID after the PUBLIC keyword.
                        state = ((parsedId.Value != null) && parsedId.Correct)
                            ? DoctypeParseState.AfterSystemIdentifier
                            : DoctypeParseState.Bogus;
                        break;
                    }
                    case DoctypeParseState.AfterSystemIdentifier:
                    case DoctypeParseState.Bogus:
                        // These states do nothing but waiting for the DOCTYPE tag to be closed, and waiting is performed
                        // by the outer loop.
                        break;
                    default:
                        // SQ fix: nothing to do.
                        break;
                }

                // Consume next character.
                mStream.ConsumeChar();
            }

            if (mStream.CurrentChar == HtmlStream.EofChar)
            {
                // End of HTML stream is reached before a DOCTYPE tag is ended.
                mStream.UnconsumeChar();
                string name1 = name.ToString();
                return new HtmlDoctypeToken(name1, publicId, systemId, true);
            }
            else
            {
                // The DOCTYPE tag is ended (the '>' character has been consumed).
                bool tagClosedPrematurely = (state != DoctypeParseState.Name) &&
                                            (state != DoctypeParseState.AfterName) &&
                                            (state != DoctypeParseState.AfterPublicIdentifier) &&
                                            (state != DoctypeParseState.AfterSystemIdentifier);
                string name1 = name.ToString();
                return new HtmlDoctypeToken(name1, publicId, systemId, tagClosedPrematurely);
            }
        }

        /// <summary>
        /// Parses either public or system ID of a DOCTYPE token.
        /// </summary>
        /// <returns>
        /// Both the parsed ID string (can be <c>null</c>) and a value indicating whether any parsing errors occurred.
        /// </returns>
        private HtmlParsedDoctypeId ParseDoctypeId()
        {
            StringBuilder id = null;
            DoctypeIdParseState state = DoctypeIdParseState.Initial;

            while ((mStream.CurrentChar != HtmlStream.EofChar) && (mStream.CurrentChar != '>'))
            {
                switch (state)
                {
                    case DoctypeIdParseState.Initial:
                    {
                        if (!IsWhitespace(mStream.CurrentChar))
                        {
                            switch (mStream.CurrentChar)
                            {
                                case '"':
                                {
                                    id = new StringBuilder();
                                    state = DoctypeIdParseState.DoubleQuoted;
                                    break;
                                }
                                case '\'':
                                {
                                    id = new StringBuilder();
                                    state = DoctypeIdParseState.SingleQuoted;
                                    break;
                                }
                                default:
                                    mStream.UnconsumeChar();
                                    return new HtmlParsedDoctypeId(null, false);
                            }
                        }
                        break;
                    }
                    case DoctypeIdParseState.DoubleQuoted:
                    {
                        if (mStream.CurrentChar == '"')
                        {
                            return new HtmlParsedDoctypeId(id.ToString(), true);
                        }
                        else
                        {
                            id.Append(ReplaceNull(mStream.CurrentChar));
                        }
                        break;
                    }
                    case DoctypeIdParseState.SingleQuoted:
                    {
                        if (mStream.CurrentChar == '\'')
                        {
                            return new HtmlParsedDoctypeId(id.ToString(), true);
                        }
                        else
                        {
                            id.Append(ReplaceNull(mStream.CurrentChar));
                        }
                        break;
                    }
                    default:
                        // SQ fix: nothing to do.
                        break;
                }

                mStream.ConsumeChar();
            }

            mStream.UnconsumeChar();
            string idString = (id != null)
                ? id.ToString()
                : null;
            bool endOfIdIsExpected = state == DoctypeIdParseState.Initial;
            return new HtmlParsedDoctypeId(idString, endOfIdIsExpected);
        }

        /// <summary>
        /// Parses text of a SCRIPT area. Stops just before the end tag of the area.
        /// </summary>
        private void ParseScript()
        {
            ScriptParseState state = ScriptParseState.Data;

            // The following two buffers can contain letters only. NULL characters (U+0000) never appear in them.
            StringBuilder endTagName = new StringBuilder();
            StringBuilder temporaryBuffer = new StringBuilder();

            HtmlStreamBookmark mostRecentLessThanSignBookmark = null;

            while ((mStream.CurrentChar != HtmlStream.EofChar) || (state != ScriptParseState.Data))
            {
                // Remember the position just before the most recent less-than sign.
                // If the less-than sign is the first character of an end tag that closes the SCRIPT area,
                // we will return to the remembered position to reparse the end tag.
                if (mStream.CurrentChar == '<')
                {
                    mStream.UnconsumeChar();
                    mostRecentLessThanSignBookmark = mStream.SetBookmark();
                    mStream.ConsumeChar();
                }

                switch (state)
                {
                    case ScriptParseState.Data:
                    {
                        if (mStream.CurrentChar == '<')
                        {
                            state = ScriptParseState.LessThanSign;
                        }
                        else
                        {
                            mText.Append(ReplaceNull(mStream.CurrentChar));
                        }
                        break;
                    }
                    case ScriptParseState.LessThanSign:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '/':
                            {
                                temporaryBuffer.Length = 0;
                                state = ScriptParseState.EndTagOpen;
                                break;
                            }
                            case '!':
                            {
                                state = ScriptParseState.EscapeStart;
                                mText.Append("<!");
                                break;
                            }
                            default:
                            {
                                state = ScriptParseState.Data;
                                mText.Append('<');
                                mStream.UnconsumeChar();
                                break;
                            }
                        }
                        break;
                    }
                    case ScriptParseState.EndTagOpen:
                    {
                        if (StringUtil.IsLetter(mStream.CurrentChar))
                        {
                            endTagName.Length = 0;
                            endTagName.Append(StringUtil.AsciiLowerCase(mStream.CurrentChar));
                            temporaryBuffer.Length = 0;
                            temporaryBuffer.Append(mStream.CurrentChar);
                            state = ScriptParseState.EndTagName;
                        }
                        else
                        {
                            state = ScriptParseState.Data;
                            mText.Append("</");
                            mStream.UnconsumeChar();
                        }
                        break;
                    }
                    case ScriptParseState.EndTagName:
                    {
                        if (StringUtil.IsLetter(mStream.CurrentChar))
                        {
                            // Tag name continues.
                            endTagName.Append(StringUtil.AsciiLowerCase(mStream.CurrentChar));
                            temporaryBuffer.Append(mStream.CurrentChar);
                        }
                        else if (IsValidAfterTagName(mStream.CurrentChar) && IsAppropriateEndTag(endTagName.ToString()))
                        {
                            // Tag name ended. This end tag closes the script area.
                            mStream.UnconsumeToBookmark(mostRecentLessThanSignBookmark);
                            return;
                        }
                        else
                        {
                            // Tag name ended. This is not a valid end tag, or this end tag is valid, but it does not match
                            // the start tag of the script area.
                            mText.Append("</").Append(temporaryBuffer.ToString());
                            temporaryBuffer.Length = 0;
                            endTagName.Length = 0;
                            mStream.UnconsumeChar();
                            state = ScriptParseState.Data;
                        }
                        break;
                    }
                    case ScriptParseState.EscapeStart:
                    {
                        if (mStream.CurrentChar == '-')
                        {
                            state = ScriptParseState.EscapeStartDash;
                            mText.Append('-');
                        }
                        else
                        {
                            state = ScriptParseState.Data;
                            mStream.UnconsumeChar();
                        }
                        break;
                    }
                    case ScriptParseState.EscapeStartDash:
                    {
                        if (mStream.CurrentChar == '-')
                        {
                            mText.Append('-');
                            state = ScriptParseState.EscapedDashDash;
                        }
                        else
                        {
                            mStream.UnconsumeChar();
                            state = ScriptParseState.Data;
                        }
                        break;
                    }
                    case ScriptParseState.Escaped:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '-':
                            {
                                state = ScriptParseState.EscapedDash;
                                mText.Append('-');
                                break;
                            }
                            case '<':
                                state = ScriptParseState.EscapedLessThanSign;
                                break;
                            case HtmlStream.EofChar:
                            {
                                mStream.UnconsumeChar();
                                state = ScriptParseState.Data;
                                break;
                            }
                            default:
                                mText.Append(ReplaceNull(mStream.CurrentChar));
                                break;
                        }
                        break;
                    }
                    case ScriptParseState.EscapedDash:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '-':
                            {
                                mText.Append('-');
                                state = ScriptParseState.EscapedDashDash;
                                break;
                            }
                            case '<':
                                state = ScriptParseState.EscapedLessThanSign;
                                break;
                            case HtmlStream.EofChar:
                            {
                                mStream.UnconsumeChar();
                                state = ScriptParseState.Data;
                                break;
                            }
                            default:
                            {
                                mText.Append(ReplaceNull(mStream.CurrentChar));
                                state = ScriptParseState.Escaped;
                                break;
                            }
                        }
                        break;
                    }
                    case ScriptParseState.EscapedDashDash:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '-':
                                mText.Append('-');
                                break;
                            case '<':
                                state = ScriptParseState.EscapedLessThanSign;
                                break;
                            case '>':
                            {
                                state = ScriptParseState.Data;
                                mText.Append('>');
                                break;
                            }
                            case HtmlStream.EofChar:
                            {
                                mStream.UnconsumeChar();
                                state = ScriptParseState.Data;
                                break;
                            }
                            default:
                            {
                                state = ScriptParseState.Escaped;
                                mText.Append(ReplaceNull(mStream.CurrentChar));
                                break;
                            }
                        }
                        break;
                    }
                    case ScriptParseState.EscapedLessThanSign:
                    {
                        if (StringUtil.IsLetter(mStream.CurrentChar))
                        {
                            temporaryBuffer.Length = 0;
                            temporaryBuffer.Append(StringUtil.AsciiLowerCase(mStream.CurrentChar));
                            mText.Append('<').Append(mStream.CurrentChar);
                            state = ScriptParseState.DoubleEscapeStart;
                        }
                        else
                        {
                            if (mStream.CurrentChar == '/')
                            {
                                temporaryBuffer.Length = 0;
                                state = ScriptParseState.EscapedEndTagOpen;
                            }
                            else
                            {
                                mText.Append('<');
                                mStream.UnconsumeChar();
                                state = ScriptParseState.Escaped;
                            }
                        }
                        break;
                    }
                    case ScriptParseState.EscapedEndTagOpen:
                    {
                        if (StringUtil.IsLetter(mStream.CurrentChar))
                        {
                            CreateTag(false);
                            endTagName.Append(StringUtil.AsciiLowerCase(mStream.CurrentChar));
                            temporaryBuffer.Append(mStream.CurrentChar);
                            state = ScriptParseState.EscapedEndTagName;
                        }
                        else
                        {
                            mText.Append("</");
                            mStream.UnconsumeChar();
                            state = ScriptParseState.Escaped;
                        }
                        break;
                    }
                    case ScriptParseState.EscapedEndTagName:
                    {
                        if (StringUtil.IsLetter(mStream.CurrentChar))
                        {
                            endTagName.Append(StringUtil.AsciiLowerCase(mStream.CurrentChar));
                            temporaryBuffer.Append(mStream.CurrentChar);
                        }
                        else if (IsValidAfterTagName(mStream.CurrentChar) && IsAppropriateEndTag(endTagName.ToString()))
                        {
                            mStream.UnconsumeToBookmark(mostRecentLessThanSignBookmark);
                            return;
                        }
                        else
                        {
                            mText.Append("</").Append(temporaryBuffer.ToString());
                            temporaryBuffer.Length = 0;
                            endTagName.Length = 0;
                            mStream.UnconsumeChar();
                            state = ScriptParseState.Escaped;
                        }
                        break;
                    }
                    case ScriptParseState.DoubleEscapeStart:
                    {
                        if (StringUtil.IsLetter(mStream.CurrentChar))
                        {
                            temporaryBuffer.Append(StringUtil.AsciiLowerCase(mStream.CurrentChar));
                            mText.Append(mStream.CurrentChar);
                        }
                        else if (IsValidAfterTagName(mStream.CurrentChar))
                        {
                            mText.Append(mStream.CurrentChar);
                            state = (temporaryBuffer.ToString() == "script")
                                ? ScriptParseState.DoubleEscaped
                                : ScriptParseState.Escaped;
                        }
                        else
                        {
                            mStream.UnconsumeChar();
                            state = ScriptParseState.Escaped;
                        }
                        break;
                    }
                    case ScriptParseState.DoubleEscaped:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '-':
                            {
                                mText.Append('-');
                                state = ScriptParseState.DoubleEscapedDash;
                                break;
                            }
                            case '<':
                            {
                                mText.Append('<');
                                state = ScriptParseState.DoubleEscapedLessThanSign;
                                break;
                            }
                            case HtmlStream.EofChar:
                            {
                                mStream.UnconsumeChar();
                                state = ScriptParseState.Data;
                                break;
                            }
                            default:
                                mText.Append(ReplaceNull(mStream.CurrentChar));
                                break;
                        }
                        break;
                    }
                    case ScriptParseState.DoubleEscapedDash:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '-':
                            {
                                mText.Append('-');
                                state = ScriptParseState.DoubleEscapedDashDash;
                                break;
                            }
                            case '<':
                            {
                                mText.Append('<');
                                state = ScriptParseState.DoubleEscapedLessThanSign;
                                break;
                            }
                            case HtmlStream.EofChar:
                            {
                                mStream.UnconsumeChar();
                                state = ScriptParseState.Data;
                                break;
                            }
                            default:
                            {
                                mText.Append(ReplaceNull(mStream.CurrentChar));
                                state = ScriptParseState.DoubleEscaped;
                                break;
                            }
                        }
                        break;
                    }
                    case ScriptParseState.DoubleEscapedDashDash:
                        switch (mStream.CurrentChar)
                        {
                            case '-':
                                mText.Append('-');
                                break;
                            case '<':
                            {
                                mText.Append('<');
                                state = ScriptParseState.DoubleEscapedLessThanSign;
                                break;
                            }
                            case '>':
                            {
                                mText.Append('>');
                                state = ScriptParseState.Data;
                                break;
                            }
                            case HtmlStream.EofChar:
                            {
                                mStream.UnconsumeChar();
                                state = ScriptParseState.Data;
                                break;
                            }
                            default:
                            {
                                mText.Append(ReplaceNull(mStream.CurrentChar));
                                state = ScriptParseState.DoubleEscaped;
                                break;
                            }
                        }
                        break;
                    case ScriptParseState.DoubleEscapedLessThanSign:
                    {
                        if (mStream.CurrentChar == '/')
                        {
                            temporaryBuffer.Length = 0;
                            mText.Append('/');
                            state = ScriptParseState.DoubleEscapeEnd;
                        }
                        else
                        {
                            mStream.UnconsumeChar();
                            state = ScriptParseState.DoubleEscaped;
                        }
                        break;
                    }
                    case ScriptParseState.DoubleEscapeEnd:
                    {
                        if (StringUtil.IsLetter(mStream.CurrentChar))
                        {
                            temporaryBuffer.Append(StringUtil.AsciiLowerCase(mStream.CurrentChar));
                            mText.Append(mStream.CurrentChar);
                        }
                        else if (IsValidAfterTagName(mStream.CurrentChar))
                        {
                            mText.Append(mStream.CurrentChar);
                            state = (temporaryBuffer.ToString() == "script")
                                ? ScriptParseState.Escaped
                                : ScriptParseState.DoubleEscaped;
                        }
                        else
                        {
                            mStream.UnconsumeChar();
                            state = ScriptParseState.DoubleEscaped;
                        }
                        break;
                    }
                    default:
                        // SQ fix: nothing to do.
                        break;
                }

                mStream.ConsumeChar();
            }

            // Unconsume the end-of-file character.
            mStream.UnconsumeChar();
        }

        /// <summary>
        /// Parses either RAWTEXT or RCDATA text area. Returns just before the end tag that closes the area.
        /// </summary>
        /// <param name="resolveCharacterReferences">
        /// A flag indicating whether named and numeric character references should be replaced with corresponding Unicode
        /// characters.
        /// </param>
        private void ParseTextArea(bool resolveCharacterReferences)
        {
            TextParseState state = TextParseState.Text;

            // The following two buffers can contain letters only. NULL characters (U+0000) never appear in them.
            StringBuilder endTagName = new StringBuilder();
            StringBuilder temporaryBuffer = new StringBuilder();

            HtmlStreamBookmark mostRecentLessThanSignBookmark = null;

            while ((mStream.CurrentChar != HtmlStream.EofChar) || (state != TextParseState.Text))
            {
                // Remember the position just before the most recent less-than sign.
                // If the less-than sign is the first character of an end tag that closes the text area,
                // we will return to the remembered position to reparse the end tag.
                if (mStream.CurrentChar == '<')
                {
                    mStream.UnconsumeChar();
                    mostRecentLessThanSignBookmark = mStream.SetBookmark();
                    mStream.ConsumeChar();
                }

                switch (state)
                {
                    case TextParseState.Text:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '&':
                                if (resolveCharacterReferences)
                                {
                                    mText.Append(ConsumeCharacterReference(false));
                                }
                                else
                                {
                                    mText.Append('&');
                                }
                                break;
                            case '<':
                                state = TextParseState.LessThanSign;
                                break;
                            default:
                                mText.Append(ReplaceNull(mStream.CurrentChar));
                                break;
                        }
                        break;
                    }
                    case TextParseState.LessThanSign:
                    {
                        if (mStream.CurrentChar == '/')
                        {
                            temporaryBuffer.Length = 0;
                            state = TextParseState.EndTagOpen;
                        }
                        else
                        {
                            mText.Append('<');
                            mStream.UnconsumeChar();
                            state = TextParseState.Text;
                        }
                        break;
                    }
                    case TextParseState.EndTagOpen:
                    {
                        if (StringUtil.IsLetter(mStream.CurrentChar))
                        {
                            endTagName.Length = 0;
                            endTagName.Append(StringUtil.AsciiLowerCase(mStream.CurrentChar));
                            temporaryBuffer.Append(mStream.CurrentChar);
                            state = TextParseState.EndTagName;
                        }
                        else
                        {
                            mText.Append("</");
                            mStream.UnconsumeChar();
                            state = TextParseState.Text;
                        }
                        break;
                    }
                    case TextParseState.EndTagName:
                    {
                        if (StringUtil.IsLetter(mStream.CurrentChar))
                        {
                            endTagName.Append(StringUtil.AsciiLowerCase(mStream.CurrentChar));
                            temporaryBuffer.Append(mStream.CurrentChar);
                        }
                        else if (IsValidAfterTagName(mStream.CurrentChar) && IsAppropriateEndTag(endTagName.ToString()))
                        {
                            mStream.UnconsumeToBookmark(mostRecentLessThanSignBookmark);
                            return;
                        }
                        else
                        {
                            mText.Append("</").Append(temporaryBuffer.ToString());
                            temporaryBuffer.Length = 0;
                            endTagName.Length = 0;
                            mStream.UnconsumeChar();
                            state = TextParseState.Text;
                        }
                        break;
                    }
                    default:
                        // SQ fix: nothing to do.
                        break;
                }

                mStream.ConsumeChar();
            }

            // Unconsume the end-of-file character.
            mStream.UnconsumeChar();
        }

        /// <summary>
        /// Parses a CDATA text section. Returns after the "]]&gt;" character sequence is read.
        /// </summary>
        private void ParseCData()
        {
            while (mStream.CurrentChar != HtmlStream.EofChar)
            {
                if ((mStream.CurrentChar == ']') && TryConsumeOrdinal("]>"))
                {
                    // The end character sequence is read. Parsing of CDATA is finished.
                    break;
                }

                // NULL characters (U+0000) are not replaced in CDATA sections.
                mText.Append(mStream.CurrentChar);

                mStream.ConsumeChar();
            }

            // The end-of-file character will be reprocessed at a higher level.
            if (mStream.CurrentChar == HtmlStream.EofChar)
            {
                mStream.UnconsumeChar();
            }
        }

        /// <summary>
        /// Parses an HTML comment. Returns after the "--!>" sequence is parsed.
        /// </summary>
        /// <returns>
        /// The parsed comment token.
        /// </returns>
        /// <remarks>
        /// This method should be called after the initial "&lt;!--" sequence is read.
        /// </remarks>
        private HtmlCommentToken ParseComment()
        {
            StringBuilder commentText = new StringBuilder();
            CommentParseState state = CommentParseState.Start;

            while (mStream.CurrentChar != HtmlStream.EofChar)
            {
                switch (state)
                {
                    case CommentParseState.Start:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '-':
                                state = CommentParseState.StartDash;
                                break;
                            case '>':
                                return new HtmlCommentToken(commentText.ToString());
                            default:
                            {
                                commentText.Append(ReplaceNull(mStream.CurrentChar));
                                state = CommentParseState.Text;
                                break;
                            }
                        }
                        break;
                    }
                    case CommentParseState.StartDash:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '-':
                                state = CommentParseState.End;
                                break;
                            case '>':
                                return new HtmlCommentToken(commentText.ToString());
                            default:
                            {
                                commentText.Append('-').Append(ReplaceNull(mStream.CurrentChar));
                                state = CommentParseState.Text;
                                break;
                            }
                        }
                        break;
                    }
                    case CommentParseState.Text:
                    {
                        if (mStream.CurrentChar == '-')
                        {
                            state = CommentParseState.EndDash;
                        }
                        else
                        {
                            commentText.Append(ReplaceNull(mStream.CurrentChar));
                        }
                        break;
                    }
                    case CommentParseState.EndDash:
                    {
                        if (mStream.CurrentChar == '-')
                        {
                            state = CommentParseState.End;
                        }
                        else
                        {
                            commentText.Append('-').Append(ReplaceNull(mStream.CurrentChar));
                            state = CommentParseState.Text;
                        }
                        break;
                    }
                    case CommentParseState.End:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '>':
                                return new HtmlCommentToken(commentText.ToString());
                            case '!':
                                state = CommentParseState.EndBang;
                                break;
                            case '-':
                                commentText.Append('-');
                                break;
                            default:
                            {
                                commentText.Append("--").Append(ReplaceNull(mStream.CurrentChar));
                                state = CommentParseState.Text;
                                break;
                            }
                        }
                        break;
                    }
                    case CommentParseState.EndBang:
                    {
                        switch (mStream.CurrentChar)
                        {
                            case '-':
                            {
                                commentText.Append("--!");
                                state = CommentParseState.EndDash;
                                break;
                            }
                            case '>':
                                return new HtmlCommentToken(commentText.ToString());
                            default:
                            {
                                commentText.Append("--!").Append(ReplaceNull(mStream.CurrentChar));
                                state = CommentParseState.Text;
                                break;
                            }
                        }
                        break;
                    }
                    default:
                        // SQ fix: nothing to do.
                        break;
                }

                mStream.ConsumeChar();
            }

            // Unconsume the end-of-file character.
            mStream.UnconsumeChar();
            return new HtmlCommentToken(commentText.ToString());
        }

        /// <summary>
        /// Parses a bogus HTML comment.
        /// </summary>
        /// <returns>
        /// The parsed comment token.
        /// </returns>
        /// <remarks>
        /// This method should be called after the sequence that starts the comment is read.
        /// </remarks>
        private HtmlCommentToken ParseBogusComment()
        {
            HtmlStreamBookmark bookmark = mStream.SetBookmark();
            while ((mStream.CurrentChar != '>') && (mStream.CurrentChar != HtmlStream.EofChar))
            {
                mStream.ConsumeChar();
            }
            string commentText = mStream.GetBookmarkedText(bookmark);

            // These characters will be reprocessed at a higher level.
            if ((mStream.CurrentChar == HtmlStream.EofChar) || (mStream.CurrentChar == '<'))
            {
                mStream.UnconsumeChar();
            }

            return CreateComment(commentText);
        }

        /// <summary>
        /// Parses a revealed conditional comment as a normal comment.
        /// </summary>
        /// <remarks>
        /// If a revealed conditinal comment doesn't match the set of features we support, we must parse it as a normal comment
        /// and, as a result, hide its contents.
        /// </remarks>
        private HtmlCommentToken ParseUnmatchedRevealedComment()
        {
            HtmlStreamBookmark bookmark = mStream.SetBookmark();
            // Seek to the nearest '<![endif]>' tag.
            while (mStream.CurrentChar != HtmlStream.EofChar)
            {
                if (TryConsumeRevealedCommentEndTag())
                {
                    break;
                }
                mStream.ConsumeChar();
            }
            string commentText = mStream.GetBookmarkedText(bookmark);

            // The end-of-file character will be reprocessed at higher level.
            if (mStream.CurrentChar == HtmlStream.EofChar)
            {
                mStream.UnconsumeChar();
            }

            return CreateComment(commentText);
        }

        /// <summary>
        /// Consumes next characters if they constitute a valid end tag of a revealed conditional comment (&lt;![endif]&gt;).
        /// </summary>
        private bool TryConsumeRevealedCommentEndTag()
        {
            if (!TryConsumeAsciiCaseInsensitive("<![endif]"))
            {
                return false;
            }

            mStream.ConsumeChar();
            return mStream.CurrentChar == '>';
        }

        /// <summary>
        /// Consume next characters if they are equal to the specified string. Ordinal case-sensitive comparison is used.
        /// </summary>
        /// <param name="str">The string that will be consumed in the case of a match.</param>
        /// <returns><c>true</c> if the string has been consumed; <c>false</c> otherwise.</returns>
        private bool TryConsumeOrdinal(string str)
        {
            if (mStream.NextStringEquals(str, false))
            {
                mStream.ConsumeChar(str.Length);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Consume next characters if they are equal to the specified string. ASCII case-insensitive comparison is used.
        /// </summary>
        /// <param name="str">The string that will be consumed in the case of a match.</param>
        /// <returns><c>true</c> if the string has been consumed; <c>false</c> otherwise.</returns>
        private bool TryConsumeAsciiCaseInsensitive(string str)
        {
            if (mStream.NextStringEquals(str, true))
            {
                mStream.ConsumeChar(str.Length);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries to parse an expression of a conditional comment and to match it against the set of features we support.
        /// </summary>
        private ConditionalExpressionConsumeResult TryConsumeConditionalExpression()
        {
            // The fact that the list of supported features is null indicates that the calling code explicitly asks
            // the HTML parser not to support conditional comments. Note that no modern browsers support them.
            if (mSupportedFeatures == null)
            {
                return ConditionalExpressionConsumeResult.NotFound;
            }

            // Set a bookmark so that we can unconsume input in case the condition is not met.
            HtmlStreamBookmark expressionStart = mStream.SetBookmark();

            ConditionalExpressionConsumeResult consumeResult = ConditionalExpressionConsumeResult.NotFound;

            if (TryConsumeAsciiCaseInsensitive("[if"))
            {
                StringBuilder expressionText = new StringBuilder();

                mStream.ConsumeChar();
                while (mStream.CurrentChar != HtmlStream.EofChar)
                {
                    // Conditional expressions end with ']>'
                    if (mStream.CurrentChar == ']')
                    {
                        mStream.ConsumeChar();
                        if (mStream.CurrentChar == '>')
                        {
                            consumeResult = ConditionalExpressionConsumeResult.Invalid;
                            break;
                        }
                        mStream.UnconsumeChar();
                    }

                    expressionText.Append(ReplaceNull(mStream.CurrentChar));
                    mStream.ConsumeChar();
                }

                // The end-of-file character will be reprocessed at a higher level.
                if (mStream.CurrentChar == HtmlStream.EofChar)
                {
                    mStream.UnconsumeChar();
                }

                if (consumeResult != ConditionalExpressionConsumeResult.NotFound)
                {
                    // It is a conditional comment. Let's try to parse and match its expression.
                    ConditionalExpression expression = ConditionalExpression.Parse(expressionText.ToString());
                    if (expression != null)
                    {
                        // Ths expression is parsed sucessfully. Let's check whether available features match the expression.
                        consumeResult = expression.Matches(mSupportedFeatures)
                            ? ConditionalExpressionConsumeResult.Matched
                            : ConditionalExpressionConsumeResult.NotMatched;
                    }
                }
            }

            // It is not a conditional comment or its expression doesn't match the features. The text will be reprocessed.
            if (consumeResult != ConditionalExpressionConsumeResult.Matched)
            {
                mStream.UnconsumeToBookmark(expressionStart);
            }

            return consumeResult;
        }

        /// <summary>
        /// Consumes a character reference (either numeric or named).
        /// </summary>
        /// <param name="isInsideAttribute">Indicates whether the currently parsed HTML part is an attribute value.</param>
        /// <returns>
        /// The referenced characters, if the reference has been parsed sucessfully;
        /// the ampersand character otherwise.
        /// </returns>
        private string ConsumeCharacterReference(bool isInsideAttribute)
        {
            string characters;

            // Additional allowed characters, which the HTML 5 Specification mentions, are processed correctly
            // when a symbolic reference is parsed, because no reference name starts with any of the additional allowed
            // characters.
            switch (mStream.NextChar)
            {
                case '\t':
                case '\n':
                case '\f':
                case ' ':
                case '<':
                case '&':
                case HtmlStream.EofChar:
                    characters = string.Empty;
                    break;
                case '#':
                    characters = ConsumeNumericCharacterReference();
                    break;
                default:
                    characters = ConsumeSymbolicCharacterReference(isInsideAttribute);
                    break;
            }

            return (characters.Length > 0)
                ? characters
                : "&";
        }

        /// <summary>
        /// Consumes a numeric character reference (either decimal or hexadecimal).
        /// </summary>
        /// <returns>
        /// The referenced characters, if the reference has been parsed sucessfully;
        /// an empty string otherwise.
        /// </returns>
        private string ConsumeNumericCharacterReference()
        {
            HtmlStreamBookmark positionBeforeReference = mStream.SetBookmark();
            mStream.ConsumeChar(); // '#'

            int value = 0;
            int valueLength = 0;

            // Consume digits of the character reference.
            if ((mStream.NextChar == 'x') || (mStream.NextChar == 'X'))
            {
                mStream.ConsumeChar(); // 'x' or 'X'
                while (StringUtil.IsHexDigit(mStream.NextChar))
                {
                    ++valueLength;
                    mStream.ConsumeChar();
                    // If the value is already too big, stop parsing its digits to prevent overflow.
                    // The value will not be used anyway.
                    if (value <= 0x10FFFF)
                    {
                        value <<= 4;
                        value += StringUtil.HexCharToDigit(mStream.CurrentChar);
                    }
                }
            }
            else
            {
                while (StringUtil.IsDigit(mStream.NextChar))
                {
                    ++valueLength;
                    mStream.ConsumeChar();
                    // If the value is already too big, stop parsing its digits to prevent overflow.
                    // The value will not be used anyway.
                    if (value <= 0x10FFFF)
                    {
                        value *= 10;
                        value += mStream.CurrentChar - '0';
                    }
                }
            }

            // Consume a semicolon, if there is any.
            if (mStream.NextChar == ';')
            {
                mStream.ConsumeChar();
            }

            if (valueLength == 0)
            {
                mStream.UnconsumeToBookmark(positionBeforeReference);
                return string.Empty;
            }
            else
            {
                return HtmlCharacterReference.Convert(value);
            }
        }

        /// <summary>
        /// Consumes a named character reference.
        /// </summary>
        /// <returns>
        /// The referenced characters, if the reference has been parsed sucessfully (name is known);
        /// an empty string otherwise.
        /// </returns>
        private string ConsumeSymbolicCharacterReference(bool isInsideAttribute)
        {
            StringBuilder name = new StringBuilder();
            string longestNameCharacters = string.Empty;

            HtmlStreamBookmark positionBeforeReference = mStream.SetBookmark();
            bool entityNameEndsWithSemicolon = false;

            HtmlStreamBookmark positionAfterMatchedName = mStream.SetBookmark();
            while (HtmlCharacterReference.LongerNamedReferenceExists(name.ToString()))
            {
                mStream.ConsumeChar();
                if (mStream.CurrentChar == HtmlStream.EofChar)
                {
                    break;
                }
                name.Append(mStream.CurrentChar);
                string characters = HtmlCharacterReference.Convert(name.ToString());
                if (characters.Length > 0)
                {
                    // Current name is known.
                    longestNameCharacters = characters;
                    entityNameEndsWithSemicolon = mStream.CurrentChar == ';';
                    positionAfterMatchedName = mStream.SetBookmark();
                }
            }

            mStream.UnconsumeToBookmark(positionAfterMatchedName);

            // For historical reasons, there is a quirk for named character references in attribute values.
            if (isInsideAttribute && (!entityNameEndsWithSemicolon))
            {
                if (StringUtil.IsLetterOrDigit(mStream.NextChar) || (mStream.NextChar == '='))
                {
                    mStream.UnconsumeToBookmark(positionBeforeReference);
                    longestNameCharacters = string.Empty;
                }
            }

            return longestNameCharacters;
        }

        /// <summary>
        /// Initializes a new tag token.
        /// </summary>
        /// <param name="isStartTag">
        /// Whether the token will be a start tag (<c>true</c>) or an end tag (<c>false</c>).
        /// </param>
        private void CreateTag(bool isStartTag)
        {
            mTagName.Length = 0;
            mTagIsStartTag = isStartTag;
            mTagIsSelfClosing = false;
            mAttributes = new HtmlAttributeCollection();

            Debug.Assert(mAttributeName.Length == 0);
            Debug.Assert(mAttributeValue.Length == 0);
        }

        /// <summary>
        /// Initializes a new attributes.
        /// </summary>
        private void CreateAttribute()
        {
            AddCurrentAttributeToCollectionIfExists();

            mAttributeName.Length = 0;
            mAttributeValue.Length = 0;
        }

        /// <summary>
        /// If a current attributes exists, adds it to the collection of attributes.
        /// </summary>
        /// <remarks>
        /// A filled attribute is added to the current tag's collection of attributes only when another attribute starts,
        /// or when the current tag is emitted.
        /// </remarks>
        private void AddCurrentAttributeToCollectionIfExists()
        {
            if (mAttributeName.Length > 0)
            {
                mAttributes.Add(new HtmlAttribute(mAttributeName.ToString(), mAttributeValue.ToString()));
                mAttributeName.Length = 0;
                mAttributeValue.Length = 0;
            }
        }

        /// <summary>
        /// Creates a new comment token. Replaces null characters in the specified text.
        /// </summary>
        private static HtmlCommentToken CreateComment(string comment)
        {
            return new HtmlCommentToken(ReplaceNull(comment));
        }

        /// <summary>
        /// Emits the current tag token.
        /// </summary>
        private void EmitTag()
        {
            AddCurrentAttributeToCollectionIfExists();
            EmitToken(new HtmlTagToken(mTagName.ToString(), mTagIsStartTag, mTagIsSelfClosing, mAttributes));
        }

        /// <summary>
        /// Emits an end-of-file token.
        /// </summary>
        private void EmitEof()
        {
            EmitToken(new HtmlEndOfFileToken());
        }

        /// <summary>
        /// Emits the token.
        /// </summary>
        /// <param name="token">A token to be emitted.</param>
        private void EmitToken(HtmlToken token)
        {
            Debug.Assert(token != null);
            Debug.Assert(mEmittedToken == null);

            mEmittedToken = token;
        }

        /// <summary>
        /// Checks whether the specified tag name matches the name of the last emitted start tag.
        /// </summary>
        private bool IsAppropriateEndTag(string tagName)
        {
            // Ordinal case-sensitive comparison. Tag names are lowercase anyway.
            return tagName == mJustEmittedStartTagName;
        }

        /// <summary>
        /// Replaces the null character U+0000 with the replacement character U+FFFD. Leaves other characters unchanged.
        /// </summary>
        private static char ReplaceNull(char c)
        {
            return (c == '\0')
                ? UnicodeUtil.ReplacementChar
                : c;
        }

        /// <summary>
        /// Replaces the null character U+0000 with the replacement character U+FFFD. Leaves other characters unchanged.
        /// </summary>
        private static string ReplaceNull(string s)
        {
            return s.Replace('\0', UnicodeUtil.ReplacementChar);
        }

        /// <summary>
        /// Gets a value indicating whether the specified character is a whitespace character.
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns><c>true</c> if the specified character is a whitespace character. <c>false</c> otherwise.</returns>
        private static bool IsWhitespace(char c)
        {
            // The HTML whitespace characters are listed here: http://www.w3.org/TR/html5/infrastructure.html#space-character
            // The CR character is not included, because it must be removed from the HTML stream during the preprocessing phase.
            return (c == ' ') ||
                   (c == '\t') ||
                   (c == '\n') ||
                   (c == '\f');
        }

        /// <summary>
        /// Gets a value indicating whether the specified character can follow a tag name.
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns><c>true</c> if the specified character can follow a tag name. <c>false</c> otherwise.</returns>
        private static bool IsValidAfterTagName(char c)
        {
            return IsWhitespace(c) ||
                   (c == '/') ||
                   (c == '>');
        }

        /// <summary>
        /// The current state of the tokenizer.
        /// </summary>
        private ParseState mState;

        /// <summary>
        /// The HTML text being parsed.
        /// </summary>
        private readonly HtmlStream mStream;

        /// <summary>
        /// Features that are considered supported when loading IE conditional comments.
        /// </summary>
        private readonly Features mSupportedFeatures;

        /// <summary>
        /// Contents of the current text token.
        /// </summary>
        private readonly StringBuilder mText = new StringBuilder();

        /// <summary>
        /// The emitted token that waits while the caller processes a text token lazily emitted before it.
        /// </summary>
        private HtmlToken mEmittedToken;

        /// <summary>
        /// Name of the current tag token.
        /// </summary>
        private readonly StringBuilder mTagName = new StringBuilder();

        /// <summary>
        /// Whether the current tag token is a start or an end tag.
        /// </summary>
        private bool mTagIsStartTag;

        /// <summary>
        /// 'self-closing' flag of the current tag token.
        /// </summary>
        private bool mTagIsSelfClosing;

        /// <summary>
        /// Name of the current attribute.
        /// </summary>
        private readonly StringBuilder mAttributeName = new StringBuilder();

        /// <summary>
        /// Value of the current attribute.
        /// </summary>
        private readonly StringBuilder mAttributeValue = new StringBuilder();

        /// <summary>
        /// Attributes of the current tag token.
        /// </summary>
        private HtmlAttributeCollection mAttributes;

        /// <summary>
        /// The name of a start tag that has just been emitted. If the last emitted token is not a start tag,
        /// this string is empty, and a tag name cannot be empty.
        /// </summary>
        private string mJustEmittedStartTagName = string.Empty;
    }
}
