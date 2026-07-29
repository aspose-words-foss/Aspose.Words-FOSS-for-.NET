// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2003 by Roman Korchagin

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Bidi;
using Aspose.Fonts;
using Aspose.JavaAttributes;
using Aspose.Words.Fields;
using Aspose.Words.Lists;
using Aspose.Words.Math;
using Aspose.Words.Saving;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Txt.Writer
{
    /// <summary>
    /// Responsible for saving document or a fragment of a document in plain text format.
    /// </summary>
    internal class TxtWriter : TxtWriterBase
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal TxtWriter()
        {
            mCachedListLabelsStack = new Stack<ListLabel>();
            // WORDSNET-455 and 4821 When user calls Run.ToTxt, then this stack is empty and it causes an exception.
            // The list was populated only when processing paragraph start and of course customer can just call Run.ToTxt.
            // So we push just one dummy item to the stack and it works.
            mCachedListLabelsStack.Push(null);
        }

        public override VisitorAction VisitParagraphStart(Paragraph paragraph)
        {
            mBlockQuoteLevel = paragraph.HtmlBlockQuoteLevel;

            // Don't write ListLabel immediately because paragraph may start with
            // PageBreakChar or ColumnBreakChar which must be written before.

            // ListLabel is written to the stack to handle the case with nested paragraphs.
            // Topmost element of mCachedListLabelsStack is set to 'null' if there is no need to write label for
            // the paragraph, null value is also used as a flag that list label has been already written to file.
            mCachedListLabelsStack.Push(paragraph.HasListLabel ? paragraph.ListLabel : null);

            // WORDSNET-19050 Start accumulating text for the paragraph inside a table.
            if (SaveOptions.PreserveTableLayout && IsInTable)
                TableBuilder.StartParagraph(paragraph);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitRun(Run run)
        {
            if (!IsLocked)
            {
                ListLabel listLabel = mCachedListLabelsStack.Peek();

                string text = GetText(run);

                if (listLabel != null)
                {
                    // This while loop is ended with i set to first not-breaking (page break or column break) character
                    // in run.Text. If all characters in the run are breaks, i is set to position after last character.
                    int i = 0;
                    while (i < text.Length)
                    {
                        char ch = text[i];
                        if ((ch != ControlChar.PageBreakChar) && (ch != ControlChar.ColumnBreakChar))
                            break;

                        i++;
                    }

                    // If run.Text starts with one or several breaks, write this breaks (before list label).
                    if (i > 0)
                        AppendText(text.Substring(0, i));

                    // If there are any not-breaking character in the run, write list label and then these characters.
                    if (i < text.Length)
                    {
                        AppendText(BuildListLabel(listLabel));
                        // Add space after label. It's possible to use
                        // paragraph.ListLabel.TrailingCharacter
                        // as separator but space looks the best for plain text.
                        AppendText(" ");
                        text = text.Substring(i);

                        // Paragraph can have several runs so we need to mark the paragraph list label written for
                        // that this list label must not be written for this paragraph anymore.
                        // 'null' means that we have written label for the paragraph and do not need to do it anymore.
                        mCachedListLabelsStack.Pop();
                        mCachedListLabelsStack.Push(null);
                    }
                }

                // WORDSNET-8213 We should add ControlChar.RightToLeftMarkChar before each RtL run.
                // WORDSNET-16814 AddBidiMarks option is introduced to add RTL control marks only when this option is enabled.
                // WORDSNET-17853 The algorithm of adding BiDi marks is improved, so that BiDi marks now are wrapping current
                // run and their type is chosen depending on BiDi property of this and previous/next run and BiDi category
                // of first/last character of this run.
                if (SaveOptions.AddBidiMarks)
                    AppendTextBiDiAware(text, run);
                else
                    AppendText(text);
            }

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldStart(FieldStart fieldStart)
        {
            if (fieldStart.FieldType == FieldType.FieldMacroButton)
                AppendText(((FieldMacroButton)fieldStart.GetField()).DisplayText);

            Lock();
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldSeparator(FieldSeparator fieldSeparator)
        {
            if (IsLocked) // WORDSNET-11447
                Unlock();

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
        {
            // If the field has separator, the output was already unlocked.
            if (!fieldEnd.HasSeparator &&
                IsLocked) // WORDSNET-11447
                Unlock();

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitParagraphEnd(Paragraph paragraph)
        {
            if (!IsLocked)
            {
                ListLabel listLabel = mCachedListLabelsStack.Peek();

                if (listLabel != null)
                    AppendText(BuildListLabel(listLabel));

                // WORDSNET-8796 MS Word writes the end of table cell as CR only (not CR+LF).
                AppendText((paragraph.IsEndOfCell && !SaveOptions.PreserveTableLayout)
                    ? ControlChar.Cr
                    : SaveOptions.ParagraphBreak);
            }

            mCachedListLabelsStack.Pop();
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitTableStart(Table table)
        {
            mTableBuilderStack.Push(new TxtTableBuilder(table, TableBuilder, SaveOptions));
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitTableEnd(Table table)
        {
            if ((SaveOptions.PreserveTableLayout) && (mTableBuilderStack.Count == 1))
            {
                // Add table lines.
                for (int i = 0; i < TableBuilder.Lines.Count; i++)
                {
                    StringBuilder line = TableBuilder.Lines[i];
                    AppendTextFinal(line.ToString());
                    AppendTextFinal(SaveOptions.ParagraphBreak);
                }
            }

            mTableBuilderStack.Pop();
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitRowStart(Row row)
        {
            if (SaveOptions.PreserveTableLayout)
                TableBuilder.StartRow();
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitRowEnd(Row row)
        {
            if (SaveOptions.PreserveTableLayout)
                TableBuilder.EndRow();
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCellStart(Cell cell)
        {
            if (SaveOptions.PreserveTableLayout)
            {
                // WORDSNET-6949 The problem occurs because "previous" cells (HorizontalMerge == CellMerge.Previous) were skipped
                // but nested tables inside these cells were processed.
                // Disable processing of the "previous" cells content.
                if (!TableBuilder.StartCell(cell))
                    return VisitorAction.SkipThisNode;
            }
            else if (cell.CellPr.IsMergedToPrevious)
            {
                // WORDSNET-7020 The problem occurs because merged cells aren't skipped when PreserveTableLayout option is false.
                // All the merged cells in a range are processed and saved to output. It causes data duplication.
                // Skip the merged cells to avoid data duplication in an output.
                return VisitorAction.SkipThisNode;
            }
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCellEnd(Cell cell)
        {
            if (SaveOptions.PreserveTableLayout)
                TableBuilder.EndCell();
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCommentStart(Comment comment)
        {
            // WORDSNET-25287 Put comment into a separate line.
            if ((Builder.Length > 0) && (Builder[Builder.Length - 1] != '\r') && (Builder[Builder.Length - 1] != '\n'))
                AppendText(SaveOptions.ParagraphBreak);

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a Office Math object has started.
        /// </summary>
        public override VisitorAction VisitOfficeMathStart(OfficeMath officeMath)
        {
            // FOSS
            return VisitorAction.SkipThisNode;
        }

        protected override void BeforeNodeAccepted(Node node)
        {
            base.BeforeNodeAccepted(node);

            if (!node.IsRemoved && SaveOptions.PreserveTableLayout &&
                ((node.NodeType == NodeType.Cell) || (node.NodeType == NodeType.Row)))
            {
                Table table = node.NodeType == NodeType.Cell
                    ? ((Cell)node).ParentTable
                    : ((Row)node).ParentTable;
                VisitTableStart(table);
            }
        }

        [JavaThrows(false)]
        protected override void AfterNodeAccepted(Node node)
        {
            base.AfterNodeAccepted(node);

            if (!node.IsRemoved && SaveOptions.PreserveTableLayout &&
                ((node.NodeType == NodeType.Cell) || (node.NodeType == NodeType.Row)))
            {
                Table table = node.NodeType == NodeType.Cell
                    ? ((Cell)node).ParentTable
                    : ((Row)node).ParentTable;
                VisitTableEnd(table);
            }
        }

        /// <summary>
        /// Appends a specified text.
        /// </summary>
        private void AppendText(string text)
        {
            // WORDSNET-13651 Field argument passed as text parameter is null.
            // It is raises "ArgumentNullException" while trying to find matches using regular expressions.
            if (string.IsNullOrEmpty(text))
                return;

            MatchCollection matches = gRegexBreaks.Matches(text);

            foreach (Match match in matches)
            {
                GroupCollection matchGroups = match.Groups;
                int index = GetFirstNumberedGroup(matchGroups);
                index = index > 0 ? index : matchGroups.Count - 1;
                bool isTextGroup = index == matchGroups.Count - 1;

                if (isTextGroup)
                    AppendTextValue(matchGroups[matchGroups.Count - 1].Value);
                else
                    AppendBreak(matchGroups[index].Value);
            }
        }

        /// <summary>
        /// Appends text to <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">Text to append.</param>
        private void AppendTextValue(string value)
        {
            if (!StringUtil.HasChars(value))
                return;

            if (SaveOptions.PreserveTableLayout && IsInTable)
                TableBuilder.AppendText(value);
            else
                AppendTextFinal(value);
        }

        /// <summary>
        /// <para>Writes text to <see cref="StringBuilder"/> calling <see cref="FontUtil.UnicodeToSymbol(char)"/>
        /// for each its character. See <see cref="FontUtil.UnicodeToSymbol(char)"/> for explanation.</para>
        /// </summary>
        private void AppendTextFinal(string text)
        {
            if (text == SaveOptions.ParagraphBreak)
            {
                FlushCurWord();
                Builder.Append(text);
                mCurPosition = 0;
            }
            else
            {
                // WORDSNET-19780 Append BlockQuote defined in HtmlBlock of the paragraph, if any.
                // IN. I think we should write it only once even when line is split due to limit of max page size.
                if (IsAtStart && (mBlockQuoteLevel > 0))
                    Builder.Append(string.Format("{0} ", new string('>', mBlockQuoteLevel)));

                foreach (char c in text)
                {
                    char curChar = FontUtil.UnicodeToSymbol(c);
                    if (SaveOptions.MaxCharactersPerLine < 1)
                    {
                        // The number of characters in line is not limited, so just append "as is".
                        Builder.Append(curChar);
                    }
                    else
                    {
                        // The number of characters in line is limited,
                        // so let's first split by words to try fit them in a single line.
                        if (char.IsLetterOrDigit(curChar))
                        {
                            mCurWord.Append(curChar);
                        }
                        else
                        {
                            FlushCurWord();
                            if (mCurPosition == SaveOptions.MaxCharactersPerLine)
                            {
                                Builder.Append(SaveOptions.ParagraphBreak);
                                mCurPosition = 0;
                            }

                            Builder.Append(curChar);
                            mCurPosition++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Flushes current word into a builder considering a maximum allowed length of line.
        /// </summary>
        private void FlushCurWord()
        {
            if (mCurWord.Length == 0)
                return;

            // If number of characters in line is not limited, then just append a whole word.
            if (SaveOptions.MaxCharactersPerLine < 1)
            {
                Builder.Append(mCurWord);
                return;
            }

            // Otherwise, check if the word can fit entirely in a current line.
            if ((mCurPosition + mCurWord.Length) <= SaveOptions.MaxCharactersPerLine)
            {
                // If so, then just append it.
                Builder.Append(mCurWord);
                mCurPosition += mCurWord.Length;
            }
            else
            {
                // Otherwise, check if word can fit in the line at all.
                if (mCurWord.Length <= SaveOptions.MaxCharactersPerLine)
                {
                    // As the word can not fit in the current line,
                    // then we start a new one, where the word can definitely fit.
                    Builder.Append(SaveOptions.ParagraphBreak);
                    Builder.Append(mCurWord);
                    mCurPosition = mCurWord.Length;
                }
                else
                {
                    // If we here, then the word can not fit in the line at all (its length greater,
                    // than allowed length of line). In this case, write word by characters as much,
                    // as can fit in the line.
                    int i = 0;
                    while (i < mCurWord.Length)
                    {
                        if (mCurPosition == SaveOptions.MaxCharactersPerLine)
                        {
                            Builder.Append(SaveOptions.ParagraphBreak);
                            mCurPosition = 0;
                        }

                        Builder.Append(mCurWord[i]);
                        i++;
                        mCurPosition++;
                    }
                }
            }

            mCurWord.Length = 0;
        }

        /// <summary>
        /// Appends text, considering BiDi properties.
        /// </summary>
        /// <param name="text">The text to append.</param>
        /// <param name="run">The run of the text that is being appended.</param>
        private void AppendTextBiDiAware(string text, Run run)
        {
            if (!StringUtil.HasChars(text))
                return;

            char bidiMark = (InlineHelper.GetBool(run, FontAttr.Bidi))
                ? ControlChar.RightToLeftMarkChar
                : ControlChar.LeftToRightMarkChar;

            if (IsNeedBiDiMark(text, run, true))
                Builder.Append(bidiMark);

            AppendText(text);

            if (IsNeedBiDiMark(text, run, false))
                Builder.Append(bidiMark);
        }


        /// <summary>
        /// Returns true, if a BiDi control character mark should be added to a specified text.
        /// </summary>
        private static bool IsNeedBiDiMark(string text, Run run, bool isBefore)
        {
            Debug.Assert(StringUtil.HasChars(run.Text));

            // Don't add a BiDi mark, if edge character of text has Strong Unicode type.
            char c = (isBefore) ? text[0] : text[run.Text.Length - 1];
            if (UnicodeCharacterDataResolver.IsStrongBidiCharacterType(c))
                return false;

            Run siblingRun = (isBefore)
                ? (Run)run.PreviousSiblingOfType(NodeType.Run)
                : (Run)run.NextSiblingOfType(NodeType.Run);

            // Add a BiDi mark to a very first/last run.
            if (siblingRun == null)
                return true;

            bool isBiDiRun = InlineHelper.GetBool(run, FontAttr.Bidi);
            bool isBiDiSiblingRun = InlineHelper.GetBool(siblingRun, FontAttr.Bidi);

            // Add a BiDi mark, if run of text and its sibling have different BiDi.
            return (isBiDiRun != isBiDiSiblingRun);
        }

        /// <summary>
        /// Appends break control characters to <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">String value with control character.</param>
        private void AppendBreak(string value)
        {
            if (!StringUtil.HasChars(value))
                return;

            FlushCurWord();

            switch (value[0])
            {
                case ControlChar.LineBreakChar:
                    AppendTextValue(ControlChar.CrLf);
                    break;
                case ControlChar.PageBreakChar:
                    // WORDSNET-13598 Remove 0x0C (page break) character from output while
                    // saving to text file.
                    // WORDSNET-14405 Page breaks are written optionally.
                    if (SaveOptions.ForcePageBreaks)
                        AppendTextValue(ControlChar.PageBreak);
                    break;
                default:
                    IWarningCallback callBack = Document.WarningCallback;
                    WarningUtil.WarnUnexpected(callBack, WarningSource.Text, WarningStrings.UnexpectedBreak);
                    break;
            }

            mCurPosition = 0;
        }

        /// <summary>
        /// Obtains index of the first numbered group which was captured successfully,
        /// otherwise return zero.
        /// </summary>
        /// <param name="groups">Collection with groups.</param>
        /// <returns>Index of the numbered group or zero.</returns>
        private static int GetFirstNumberedGroup(GroupCollection groups)
        {
            // There are expected initialized group object which contains
            // at least one numbered (unnamed) group.
            Debug.Assert((groups != null) && (groups.Count > 1));

            int index = 0;

            for (int i = 1; i < groups.Count; ++i)
                index = groups[i].Success ? i : index;

            return index;
        }

        /// <summary>
        /// Gets the complete list label if normal one is needed or calls building of
        /// customized list label if SimplifyListLabels option chosen.
        /// </summary>
        /// <remarks>
        /// Considers a list indentation specified in SaveOptions of the writer.
        /// </remarks>
        private string BuildListLabel(ListLabel listLabel)
        {
            // WORDSNET-17778 The ListIndentation option is introduced.
            string indent = SaveOptions.ListIndentation.GetIndent(listLabel.ListLevel.LevelNumber);

            string label = (SaveOptions.SimplifyListLabels)
                ? BuildSimplifiedListLabel(listLabel.NumberState)
                : listLabel.LabelStringOriginal;

            return string.Format("{0}{1}", indent, label);
        }

        /// <summary>
        /// <para>Builds the complete list label if SimplifyListLabels option chosen.</para>
        /// <para>In this case list label format from the model is ignored and some common rules of list labels generation
        /// are applied for all list items in a document.</para>
        ///
        /// <para>There are two rules:</para>
        /// <para>1. If current list level specifies a bulleted label, one of standard characters is inserted as list label.
        /// See <see cref="gSimplifiedBulletsCharacters"/> for list of used characters. This character is indented
        /// proportionally to list level number.</para>
        ///
        /// <para>Example:</para>
        ///
        /// <para>Original list items:</para>
        /// <para>$ First level</para>
        /// <para>        % Second level</para>
        /// <para>  * Third level</para>
        ///
        /// <para>List items with simplified labels:</para>
        /// <para>* First level</para>
        /// <para>  > Second level</para>
        /// <para>    + Third level</para>
        ///
        /// <para>2. If current list level specifies a numbered label, a sequence of numbers divided by dots is
        /// inserted as list label. The sequence is always contains numbers of all levels up to current.</para>
        ///
        /// <para>Example:</para>
        ///
        /// <para>Original list items:</para>
        /// <para>1. First level</para>
        /// <para>3) Second level</para>
        /// <para>d) Third level</para>
        ///
        /// <para>List items with simplified labels:</para>
        /// <para>1. First level</para>
        /// <para>1.3. Second level</para>
        /// <para>1.3.4. Third level</para>
        /// </summary>
        /// <remarks>There is enough place to improve the algorithm, e.g. skip numbers for list levels which are not used
        /// in current list.</remarks>
        private static string BuildSimplifiedListLabel(ListNumberState state)
        {
            StringBuilder builder = new StringBuilder();

            ListLevel levelPr = state.GetListLevel();

            if (levelPr.NumberStyle == NumberStyle.Bullet)
            {
                for (int j = 0; j < levelPr.LevelNumber; j++)
                {
                    builder.Append("  ");
                }
                builder.Append(gSimplifiedBulletsCharacters[levelPr.LevelNumber]);
            }
            else // If curListLevel.NumberStyle is any numbered format.
            {
                for (int j = 0; j <= levelPr.LevelNumber; j++)
                {
                    builder.Append(state.GetNumber(j));
                    builder.Append('.');
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets a boolean value indicating either current position in Builder is at very start of a new line.
        /// </summary>
        private bool IsAtStart
        {
            get
            {
                if (Builder.Length == 0)
                    return true;

                char lastChar = Builder[Builder.Length - 1];
                return (lastChar == '\n') || (lastChar == '\v') || (lastChar == '\f');
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether a cursor is inside a Table.
        /// </summary>
        private bool IsInTable
        {
            get { return (mTableBuilderStack.Count > 0); }
        }

        /// <summary>
        /// Gets <see cref="TxtTableBuilder"/> object.
        /// </summary>
        private TxtTableBuilder TableBuilder
        {
            get { return mTableBuilderStack.Top(); }
        }

        /// <summary>
        /// Gets <see cref="TxtSaveOptions"/> object specified for this writer.
        /// </summary>
        private TxtSaveOptions SaveOptions
        {
            get { return (TxtSaveOptions)SaveOptionsBase; }
        }

        private readonly Stack<TxtTableBuilder> mTableBuilderStack = new Stack<TxtTableBuilder>();

        /// <summary>
        /// This stack is used to store information whether we need to write list label for a paragraph.
        /// </summary>
        private readonly Stack<ListLabel> mCachedListLabelsStack;

        /// <summary>
        /// The cursor position inside a current line.
        /// </summary>
        private int mCurPosition;

        /// <summary>
        /// The current word being written.
        /// </summary>
        private readonly StringBuilder mCurWord = new StringBuilder();

        /// <summary>
        /// Regular expression for searching line and page breaks symbols.
        /// </summary>
        /// <remarks>
        /// There are expected mutually exclusive conditions.
        /// </remarks>
        private static readonly Regex gRegexBreaks = new Regex(
            "(" + ControlChar.LineBreak + ")|" +
            "(" + ControlChar.PageBreak + ")|" +
            "([^" + string.Format("{0}{1}", ControlChar.LineBreak, ControlChar.PageBreak) + "]*)",
            RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// List of 9 ASCII replacements of list bullets for each level.
        /// Used TxtSaveOptions.SimplifyListLabels option is chosen.
        /// </summary>
        private static readonly char[] gSimplifiedBulletsCharacters = { '*', '>', '+', '-', 'o', '>', '+', '-', 'o' };

        /// <summary>
        /// The BlockQuote level of <see cref="HtmlBlock"/> the current paragraph belongs.
        /// </summary>
        private int mBlockQuoteLevel;
    }
}
