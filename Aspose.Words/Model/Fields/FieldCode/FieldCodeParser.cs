// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/12/2009 by Dmitry Vorobyev

using System;
using Aspose.Fonts;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Parses field code to switches and arguments.
    /// </summary>
    internal sealed class FieldCodeParser : IDisposable
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldCodeParser(Field field, IFieldCodeTokenInfoProvider tokenInfoProvider)
            : this(new NodeRangeFieldCodeTokenizer(field, true), field.UpdateContext, tokenInfoProvider)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldCodeParser(string fieldCode, IFieldCodeTokenInfoProvider tokenInfoProvider)
            : this(new TextFieldCodeTokenizer(fieldCode, true), null, tokenInfoProvider)
        {
        }

        [JavaAttributes.JavaConvertCheckedExceptions]
        public void Dispose()
        {
            IDisposable disposable = mTokenizer as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        /// <summary>
        /// Ctor to use solely in <see cref="GetArgumentTextByRange"/>.
        /// </summary>
        private FieldCodeParser(FieldArgument argument)
            : this(new NodeRangeFieldCodeTokenizer(argument, true))
        {
            // If the argument refers to a standalone field, then its range corresponds to the field result.
            // So we need to set the parser's state as if it encountered a field separator at this point.
            if (argument.IsSingleFieldResult)
            {
                mIsInChildFieldResult = true;
                mIsInToken = true;
            }
        }

        /// <summary>
        /// Ctor to use solely in <see cref="GetFieldType"/>.
        /// </summary>
        private FieldCodeParser(FieldStart fieldStart, FieldChar fieldCodeEnd)
            : this(new NodeRangeFieldCodeTokenizer(fieldStart, fieldCodeEnd, true))
        {
        }

        /// <summary>
        /// Ctor for helper routines. Makes language ids' calculation to be omitted.
        /// </summary>
        private FieldCodeParser(IFieldCodeTokenizer tokenizer)
            : this(tokenizer, null, null)
        {
            // Language ids are not used in helper routines, set them here to avoid their calculation.
            LanguageId = RunPr.ProcessOrUserDefaultLanguageId;
            LanguageIdFarEast = RunPr.ProcessOrUserDefaultLanguageId;
            LanguageIdBi = RunPr.ProcessOrUserDefaultLanguageId;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        private FieldCodeParser(
            IFieldCodeTokenizer tokenizer,
            FieldUpdateContext updateContext,
            IFieldCodeTokenInfoProvider tokenInfoProvider)
        {
            LanguageIdBi = LanguageNotSet;
            LanguageIdFarEast = LanguageNotSet;
            LanguageId = LanguageNotSet;
            mTokenizer = tokenizer;
            mUpdateContext = updateContext;
            mTokenInfoProvider = tokenInfoProvider;
        }

        /// <summary>
        /// Returns a string representation of the field type for a field which code is strictly contained
        /// between the specified nodes.
        /// </summary>
        internal static string GetFieldType(FieldStart fieldStart, FieldChar fieldCodeEnd)
        {
            using (FieldCodeParser parser = new FieldCodeParser(fieldStart, fieldCodeEnd))
            {
                return parser.MoveToNextToken(false, false)
                    ? parser.CurrentToken
                    : string.Empty;
            }
        }

        private static bool IsVerbatimArgument(FieldArgument argument)
        {
            Field field = argument.Field;

            if (!field.HasFieldCodeCache)
                return false;

            int argumentIndex = field.FieldCodeCache.Arguments.IndexOf(argument);
            if (argumentIndex == -1)
                return false;

            return argumentIndex == FieldUtil.GetVerbatimArgumentIndex(field.Type);
        }

        private static bool IsIgnoreSymbolicQuotesArgument(FieldArgument argument)
        {
            Field field = argument.Field;

            if (!field.HasFieldCodeCache)
                return false;

            int argumentIndex = field.FieldCodeCache.Arguments.IndexOf(argument);
            if (argumentIndex == -1)
                return false;

            return FieldUtil.IgnoreSymbolicQuotes(field.Type, argumentIndex);
        }

        /// <summary>
        /// Collects text of the specified field argument using its node range.
        /// </summary>
        internal static string GetArgumentTextByRange(FieldArgument argument)
        {
            using (FieldCodeParser parser = new FieldCodeParser(argument))
            {
                // If the parser can not move, then the argument range is empty. Return an empty string in this case.
                if (!parser.MoveToNextToken(IsVerbatimArgument(argument), IsIgnoreSymbolicQuotesArgument(argument)))
                    return string.Empty;

                string argumentText = parser.CurrentToken;

                // An argument range should be parsed as a single token. If this condition is not met, then the range
                // probably became invalid because of some node changes. Throw to indicate this.
                //
                // If you have encountered a scenario where this exception is thrown, then you should invalidate the whole
                // field code right after the argument range became invalid to avoid this.
                //
                if (parser.MoveToNextToken(false, false))
                    throw new InvalidOperationException();

                return argumentText;
            }
        }

        /// <summary>
        /// Collects rich text of the specified field argument using its node range.
        /// </summary>
        internal static RichString GetArgumentRichTextByRange(FieldArgument argument)
        {
            using (FieldCodeParser parser = new FieldCodeParser(argument))
            {
                if (!parser.MoveToNextToken(IsVerbatimArgument(argument), IsIgnoreSymbolicQuotesArgument(argument)))
                    return RichString.Empty;

                RichString argumentRichText = parser.CurrentRichToken;

                if (parser.MoveToNextToken(false, false))
                    throw new InvalidOperationException();

                return argumentRichText;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified character is considered to be a double quote by field code parser.
        /// </summary>
        internal static bool IsDoubleQuote(char c)
        {
            return StringUtil.IsDoubleQuote(c);
        }

        /// <summary>
        /// Returns a value indicating whether the specified token is enclosed in double quotes.
        /// </summary>
        internal static bool IsTokenInDoubleQuotes(string token)
        {
            // An argument is considered to be enclosed in the double quotes by the MS Word
            // even if there is the opening double quote only ({ QUOTE "97 } is the example).
            // Also it is assumed that a field code parser does not include leading white spaces
            // to a token, so a double quote may appear only at the beginning of a token.
            return StringUtil.HasChars(token) && IsDoubleQuote(token[0]);
        }

        /// <summary>
        /// Proceeds to the first field token.
        /// </summary>
        internal bool MoveToFirstToken(bool ignoreSwitches)
        {
            return MoveToNextToken(false, ignoreSwitches, false);
        }

        /// <summary>
        /// Proceeds to a next field token.
        /// </summary>
        /// <param name="parseUntilEnd">True to parse next token until the end of field code.</param>
        /// <param name="ignoreSymbolicQuotes">True to consider quote as regular char if symbolic font is used.</param>
        /// <returns>True if successful, false if passed the end of field code.</returns>
        internal bool MoveToNextToken(bool parseUntilEnd, bool ignoreSymbolicQuotes)
        {
            return MoveToNextToken(parseUntilEnd, false, ignoreSymbolicQuotes);
        }

        private bool MoveToNextToken(bool parseUntilEnd, bool ignoreSwitches, bool ignoreSymbolicQuotes)
        {
            IsCurrentTokenInDoubleQuotes = false;
            mIgnoreSwitches = ignoreSwitches;

            if (mIsEof)
                return false;

            // We may have a pending switch to return (i.e. a switch which is not preceded by a delimiter).
            // Note, that it can not have any corresponding first field start and last field end as it can be
            // nothing but a piece of plain (i.e. not preserved (see the summary for IsTextCharPreserved)) text.
            if (!IsSwitch && mSwitchBuilder.HasContent)
            {
                IsSwitch = true;
                return true;
            }

            mArgumentBuilder.Clear();
            mSwitchBuilder.Clear();
            mIsInToken = false;
            IsSwitch = false;

            bool isEscaped = false;

            if (mIsInDoubleQuotes)
            {
                // The only situation when we may have stopped at the beginning of the next token is when
                // an opening double quote was encountered, so add it and register a start point.
                AppendToken();
            }

            if (parseUntilEnd)
            {
                if (ParseUntilEnd())
                    return true;
                mIsEof = true;
                return mArgumentBuilder.HasContent;
            }

            while (!mTokenizer.IsEof)
            {
                if (!IsEndOfRun)
                {
                    if (ProcessCurrentToken())
                    {
                        // Found the end of the current token, exit.
                        EndProbingSwitch();
                        // WORDSNET-26152 set the end position after the last token.
                        FinalizeToken();
                        // This can probably move beyond the range.
                        mTokenizer.MoveToNextToken();

                        return true;
                    }

                    if (mTokenizer.CurrentToken == FieldCodeToken.TextChar)
                    {
                        if (ProcessTextChar(isEscaped, ignoreSymbolicQuotes))
                            return true;

                        isEscaped = (!mIsInChildFieldResult) && (!isEscaped) && (mTokenizer.CurrentChar == '\\');
                    }
                    else
                    {
                        ProcessNonTextChar();
                    }
                }

                mTokenizer.MoveToNextToken();
            }

            // WORDSNET-26152 set the end position after the last token.
            FinalizeToken();

            mIsEof = true;
            EndProbingSwitch();

            return mArgumentBuilder.HasContent;
        }

        private bool ParseUntilEnd()
        {
            bool switchIsPossible = true;
            bool isLeadingWhitespaceSkipped = false;
            FieldCodeTokenBuilder whitespaceBuilder = new FieldCodeTokenBuilder();

            while (!mTokenizer.IsEof)
            {
                if (!IsEndOfRun)
                {
                    if ((mTokenizer.CurrentToken == FieldCodeToken.TextChar) && char.IsWhiteSpace(mTokenizer.CurrentChar))
                    {
                        if (isLeadingWhitespaceSkipped)
                            whitespaceBuilder.AppendToken(mTokenizer);
                    }
                    else
                    {
                        isLeadingWhitespaceSkipped = true;

                        // WORDSNET-11718 Try to parse switch first.
                        if (switchIsPossible && (mTokenizer.CurrentToken == FieldCodeToken.TextChar) && mTokenizer.CurrentChar == '\\')
                        {
                            StartProbingSwitch();
                        }
                        else
                        {
                            if (whitespaceBuilder.HasContent)
                            {
                                FieldCodeTokenBuilder builder = mSwitchBuilder.HasContent
                                    ? mSwitchBuilder
                                    : mArgumentBuilder;
                                builder.MergeWith(whitespaceBuilder);
                            }

                            if (ProcessRegularTextChar())
                                return true;
                        }

                        switchIsPossible = false;
                    }
                }

                mTokenizer.MoveToNextToken();
            }

            FinalizeToken();
            return false;
        }

        /// <summary>
        /// Returns true if found the end of the current token.
        /// </summary>
        private bool ProcessCurrentToken()
        {
            switch (mTokenizer.CurrentToken)
            {
                case FieldCodeToken.TextChar:
                    return false;
                case FieldCodeToken.Paragraph:
                    return !mTokenizer.IsEndOfToken && ProcessBreak(ControlChar.ParagraphBreakChar, ResolveParagraphBreakFormatting(mTokenizer.CurrentNode));
                case FieldCodeToken.Section:
                    return !mTokenizer.IsEndOfToken && ProcessBreak(ControlChar.SectionBreakChar, null);
                case FieldCodeToken.NonTextNode:
                    AppendToken();
                    return false;
                case FieldCodeToken.ChildFieldStart:
                    return (mIsInToken && !mIsInDoubleQuotes);
                case FieldCodeToken.ChildFieldSeparator:
                    mIsInChildFieldResult = true;
                    return false;
                case FieldCodeToken.ChildFieldEnd:
                    mIsInChildFieldResult = false;
                    return !mIsInDoubleQuotes;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static RunPr ResolveParagraphBreakFormatting(Node currentParagraph)
        {
            if (currentParagraph == null)
                return null;

            Paragraph previousParagraph = currentParagraph.PreviousSibling as Paragraph;
            return previousParagraph != null
                ? previousParagraph.ParagraphBreakRunPr
                : null;
        }

        /// <summary>
        /// Processes a token other than a text character (shape, field start/end etc).
        /// </summary>
        private void ProcessNonTextChar()
        {
            // We need to continue parsing, but the current token is not a text character.
            // We just skip it as the further logic requires a character to work.
            // Also, any token except a character breaks a switch.

            EndProbingSwitch();

            switch (mTokenizer.CurrentToken)
            {
                case FieldCodeToken.ChildFieldStart:
                    VisitFieldStart();
                    break;
                case FieldCodeToken.ChildFieldSeparator:
                case FieldCodeToken.ChildFieldEnd:
                    AppendToken();
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }

        /// <summary>
        /// Processes a text character token.
        /// </summary>
        /// <returns>True if the character delimits a token, false to continue parse.</returns>
        private bool ProcessTextChar(bool isEscaped, bool ignoreSymbolicQuotes)
        {
            // WORDSNET-4241 the first character of the field code sets the field language.
            // WORDSNET-4848 Support Japanese formatting picture elements.
            SetLanguages();

            char c = mTokenizer.CurrentChar;

            if (char.IsWhiteSpace(c))
                return ProcessWhiteSpace();

            if (IsDoubleQuoteSymbolicFontAware(c, ignoreSymbolicQuotes))
                return ProcessDoubleQuotes(isEscaped);

            if (c == '\\')
            {
                ProcessBackslash(isEscaped);
                return false;
            }

            return ProcessRegularTextChar();
        }

        private bool IsDoubleQuoteSymbolicFontAware(char c, bool ignoreSymbolicQuotes)
        {
            if (!IsDoubleQuote(c))
                return false;

            if (!ignoreSymbolicQuotes)
                return true;

            return !IsSymbolicFont();
        }

        private bool IsSymbolicFont()
        {
            string fontName = mTokenizer.CurrentFontName;
            return !string.IsNullOrEmpty(fontName) && FontUtil.IsSymbolic(fontName);
        }

        private void SetLanguages()
        {
            // SPEED Request the value if it is not set yet.
            if (mIsLanguagesInitialized)
                return;

            LanguageId = mTokenizer.CurrentLocaleId;
            LanguageIdFarEast = mTokenizer.CurrentLocaleIdFarEast;
            LanguageIdBi = mTokenizer.CurrentLocaleIdBi;
            Bidi = mTokenizer.CurrentBidi;

            LanguageNode = mTokenizer.CurrentNode as Inline;

            mIsLanguagesInitialized = true;
        }

        /// <summary>
        /// Processes a whitespace character token.
        /// </summary>
        /// <returns>True if the character delimits a token, false to continue parse.</returns>
        private bool ProcessWhiteSpace()
        {
            if (IsTextCharPreserved)
            {
                // The token is in double quotes or we are reading a child field's result,
                // so spaces are respected.
                AppendToken();
                return false;
            }
            else if (!mIsInToken)
            {
                // Leading space, skip.
                return false;
            }
            else
            {
                // A whitespace after a token, exit.
                EndProbingSwitch();
                // WORDSNET-26152 set the end position after the last token.
                FinalizeToken();

                return true;
            }
        }

        /// <summary>
        /// Processes a double quotes character token.
        /// </summary>
        /// <returns>True if the character delimits a token, false to continue parse.</returns>
        private bool ProcessDoubleQuotes(bool isEscaped)
        {
            if (IgnoreDoubleQuotes())
                return false;

            EndProbingSwitch();

            if (mIsInToken)
            {
                if (isEscaped || mIsInChildFieldResult)
                {
                    // Escaped double quote.
                    AppendToken();
                }
                else
                {
                    if (mIsInDoubleQuotes)
                    {
                        // Closing double quote.
                        AppendToken();
                        CloseDoubleQuotes();

                        // WORDSNET-26152 set the end position after the last token.
                        MoveNextAndFinalizeToken();
                    }
                    else
                    {
                        // Starting double quote will be processed as a part of the next token.
                        FinalizeToken();
                    }

                    return true;
                }
            }
            else
            {
                // Opening double quote.
                AppendToken();
                OpenDoubleQuotes();
            }

            return false;
        }

        /// <summary>
        /// Processes a backslash character token.
        /// </summary>
        private void ProcessBackslash(bool isEscaped)
        {
            EndProbingSwitch();

            if (!isEscaped)
            {
                // This is where a switch may begin.
                // WORDSNET-14447 Ignore switches before field type token.
                if (!IsTextCharPreserved && !mIgnoreSwitches)
                {
                    // WORDSNET-13773 Set the end position of the current token.
                    if (mArgumentBuilder.HasContent)
                        FinalizeToken();

                    StartProbingSwitch();
                }
                else
                {
                    AppendToken();
                }
            }
            else
            {
                // An escaped backslash.
                AppendToken();
            }
        }

        /// <summary>
        /// Processes a regular text character token.
        /// </summary>
        /// <returns>True if the character delimits a token, false to continue parse.</returns>
        private bool ProcessRegularTextChar()
        {
            AppendToken();

            if (!mSwitchBuilder.HasContent)
                return false;

            // We are trying to recognize a switch.
            // According to the OOXML spec, a switch may be 1 or 2 chars long (plus a backslash).
            const int maxSwitchLength = 3;
            if (mSwitchBuilder.Length <= maxSwitchLength)
            {
                if (IsValidSwitch)
                {
                    // If we have not collected any argument chars before the switch, make the current token
                    // a switch right away. Otherwise, return the argument first.
                    IsSwitch = !mArgumentBuilder.HasContent;
                    // WORDSNET-26152 set the end position after the last token.
                    MoveNextAndFinalizeToken();

                    return true;
                }
            }
            else
            {
                // The possible switch is not recognized, stop probing it.
                EndProbingSwitch();
            }

            return false;
        }

        /// <summary>
        /// Processes a paragraph or section break.
        /// </summary>
        /// <returns>True if the break delimits a token, false to continue parse.</returns>
        private bool ProcessBreak(char breakChar, RunPr runPr)
        {
            if (IsTextCharPreserved)
            {
                mArgumentBuilder.AppendChar(breakChar, runPr);
                return false;
            }
            else
            {
                return mIsInToken;
            }
        }

        private void VisitFieldStart()
        {
            FieldCodeTokenBuilder builder = (!mSwitchBuilder.HasContent) ? mArgumentBuilder : mSwitchBuilder;
            builder.VisitFieldStart(mTokenizer);
        }

        private void AppendToken()
        {
            FieldCodeTokenBuilder builder = (!mSwitchBuilder.HasContent) ? mArgumentBuilder : mSwitchBuilder;
            builder.AppendToken(mTokenizer);
            mIsInToken = true;
        }

        private void MoveNextAndFinalizeToken()
        {
            // Get the position after the current position.
            mTokenizer.MoveToNextToken();
            FinalizeToken();
        }

        private void FinalizeToken()
        {
            FieldCodeTokenBuilder builder = (!mSwitchBuilder.HasContent) ? mArgumentBuilder : mSwitchBuilder;
            builder.FinalizeToken(mTokenizer);
        }

        private void StartProbingSwitch()
        {
            mSwitchBuilder.Clear();
            mSwitchBuilder.AppendToken(mTokenizer);
            mIsInToken = true;
        }

        private void EndProbingSwitch()
        {
            mArgumentBuilder.MergeWith(mSwitchBuilder);
        }

        private void OpenDoubleQuotes()
        {
            IsCurrentTokenInDoubleQuotes = true;
            mIsInDoubleQuotes = true;
            if (mUpdateContext != null)
                mUpdateContext.IsInDoubleQuotes = true;

            mOpeningDoubleQuotePosition = mTokenizer.GetCurrentPosition();
        }

        private void CloseDoubleQuotes()
        {
            mIsInDoubleQuotes = false;
            if (mUpdateContext != null)
                mUpdateContext.IsInDoubleQuotes = false;

            mOpeningDoubleQuotePosition = null;
        }

        private bool IgnoreDoubleQuotes()
        {
            if (mOpeningDoubleQuotePosition == null)
                return false;

            // This is a rude way of handling double quotes located e.g. in a table that is enclosed into
            // double quotes to designate an argument.
            if (NodeUtil.GetNestingLevel(mOpeningDoubleQuotePosition.Node) ==
                NodeUtil.GetNestingLevel(mTokenizer.GetCurrentPosition().Node))
                return false;

            return true;
        }

        /// <summary>
        /// Returns a node range corresponding to the current token.
        /// </summary>
        /// <returns></returns>
        internal NodeRange GetCurrentNodeRange()
        {
            return CurrentBuilder.GetNodeRange();
        }

        /// <summary>
        /// Returns a node range corresponding to the current complete field token.
        /// </summary>
        internal NodeRange GetCompleteFieldNodeRange()
        {
            return CurrentBuilder.GetCompleteFieldNodeRange();
        }

        private bool IsEndOfRun
        {
            get { return (mTokenizer.CurrentToken == FieldCodeToken.TextChar) && (mTokenizer.IsEndOfToken); }
        }

        /// <summary>
        /// Returns a value indicating whether a special text character is preserved from being used as a special,
        /// i.e. it should be treated as a regular character (i.e. not a delimiter, etc.). A special text character
        /// is considered to be preserved, when it is contained within double quotes or a child field result.
        /// </summary>
        private bool IsTextCharPreserved
        {
            get { return (mIsInDoubleQuotes || mIsInChildFieldResult); }
        }

        /// <summary>
        /// Gets whether the possible switch is valid for the field whose code we are parsing.
        /// </summary>
        private bool IsValidSwitch
        {
            get
            {
                return ((mTokenInfoProvider != null) &&
                        (mTokenInfoProvider.GetSwitchType(mSwitchBuilder.Text.ToLower()) != FieldSwitchType.Unknown));
            }
        }

        private FieldCodeTokenBuilder CurrentBuilder
        {
            get { return (!IsSwitch) ? mArgumentBuilder : mSwitchBuilder; }
        }

        /// <summary>
        /// Gets the current token, whether it is an argument or a valid switch.
        /// </summary>
        internal string CurrentToken
        {
            get { return CurrentBuilder.Text; }
        }

        /// <summary>
        /// Gets the current rich token.
        /// </summary>
        internal RichString CurrentRichToken
        {
            get { return CurrentBuilder.RichText; }
        }

        /// <summary>
        /// Gets whether the current token is a switch.
        /// </summary>
        internal bool IsSwitch { get; private set; }

        /// <summary>
        /// Gets the language id of the field code specified via the properties of the first character of the field code.
        /// </summary>
        internal int LanguageId { get; private set; }

        /// <summary>
        /// Gets the East Asian language id of the field code specified via the properties of the first character of the field code.
        /// </summary>
        internal int LanguageIdFarEast { get; private set; }

        /// <summary>
        /// Gets the RTL language id of the field code specified via the properties of the first character of the field code.
        /// </summary>
        internal int LanguageIdBi { get; private set; }

        /// <summary>
        /// A boolean value indicating if RTL is specified via the properties of the first character of the field code.
        /// </summary>
        internal bool Bidi { get; private set; }

        internal Inline LanguageNode { get; private set; }

        internal bool IsCurrentTokenInDoubleQuotes { get; private set; }

        private readonly IFieldCodeTokenizer mTokenizer;
        private readonly IFieldCodeTokenInfoProvider mTokenInfoProvider;
        private readonly FieldUpdateContext mUpdateContext;
        private readonly FieldCodeTokenBuilder mArgumentBuilder = new FieldCodeTokenBuilder();
        private readonly FieldCodeTokenBuilder mSwitchBuilder = new FieldCodeTokenBuilder();
        private bool mIsInToken;
        private bool mIsInDoubleQuotes;
        private DocumentPosition mOpeningDoubleQuotePosition;
        private bool mIsInChildFieldResult;
        private bool mIgnoreSwitches;
        private bool mIsEof;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int LanguageNotSet = -1;
        private bool mIsLanguagesInitialized;
    }
}
