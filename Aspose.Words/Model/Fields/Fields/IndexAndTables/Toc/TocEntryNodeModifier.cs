// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2010 by Dmitry Vorobyev

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words.Drawing;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Removes or preserves tabs and line breaks while copying TOC entry contents.
    /// </summary>
    internal class TocEntryNodeModifier : INodeModifier
    {
        internal TocEntryNodeModifier(bool preserveTabs, bool preserveLineBreaks, int entryLevel, bool isTableOfFigures)
        {
            mPreserveTabs = preserveTabs;
            mPreserveLineBreaks = preserveLineBreaks;
            mEntryLevel = entryLevel;
            mIsTableOfFigures = isTableOfFigures;
        }

        Node INodeModifier.Modify(Node referenceNode, Node nodeToModify, bool modifyChildren, INodeCloningListener cloningListener)
        {
            // WORDSNET-26724 Skip deleted nodes.
            // WORDSNET-20677 Skip hidden nodes.
            // WORDSNET-8839 Use referenceNode instead of nodeToModify in this fix, because nodeToModify have no parent,
            // which leads to incorrect determining of toggle attributes.
            Inline inline = referenceNode as Inline;
            if ((inline != null) && inline.IsHiddenOrDeleted)
                return null;

            switch (nodeToModify.NodeType)
            {
                case NodeType.Run:
                    Run run = (Run)nodeToModify;
                    run = TrimWhiteSpaces(run);
                    ModifyRunText(run);
                    return run;

                // WORDSNET-26724 Skip comment ranges and comments.
                case NodeType.CommentRangeStart:
                case NodeType.Comment:
                case NodeType.CommentRangeEnd:
                // WORDSNET-5485 Skip footnote nodes.
                case NodeType.Footnote:
                    return null;

                case NodeType.Shape:
                case NodeType.GroupShape:
                    ShapeBase shape = (ShapeBase)nodeToModify;
                    if (!shape.IsInline)
                        return null;

                    ProcessNotWhiteSpace();
                    return nodeToModify;

                case NodeType.SpecialChar:
                    ProcessNotWhiteSpace();
                    return null;

                default:
                    // Do not modify other nodes
                    ProcessNotWhiteSpace();
                    return nodeToModify;
            }
        }

        internal void FinalizeEntry()
        {
            TrimTrailingWhitespaces();
            EnsureEntryTabStop();
        }

        private void TrimTrailingWhitespaces()
        {
            for (int index = 0; index < mTrailingWhitespaceRuns.Count; index++)
            {
                Run run = mTrailingWhitespaceRuns[index];
                if (index != 0 || mFirstTrailingWhitespaceCharPosition == 0)
                    run.Remove();
                else
                    run.CutText(0, mFirstTrailingWhitespaceCharPosition, false);
            }
        }

        private void EnsureEntryTabStop()
        {
            if (mRunWithTab != null && mRunWithTab.ParentNode != null)
            {
                int tabIndex = mRunWithTab.Text.IndexOf(ControlChar.Tab, StringComparison.Ordinal);
                mRunWithTab.SplitAfter(tabIndex + 1);
                mRunWithTab.SplitBefore(tabIndex);

                if (mRunWithTab.Font.StyleIdentifier == StyleIdentifier.Hyperlink)
                    mRunWithTab.RunPr.Remove(FontAttr.Istd);

                FieldIndexAndTablesUtil.EnsureTocEntryTabStop(mRunWithTab, mEntryLevel, mIsTableOfFigures);
            }
        }

        private void ModifyRunText(Run run)
        {
            if (run == null)
                return;

            StringBuilder builder = new StringBuilder();

            foreach (char c in run.Text)
            {
                // WORDSNET-15774 Do not copy page breaks to the TOC.
                // WORDSNET-24890 Do not copy column breaks either.
                if ((c == ControlChar.PageBreakChar) || (c == ControlChar.ColumnBreakChar))
                    continue;

                // OOXML specs says "Preserves tab entries within table entries" while Word always copies tab characters,
                // but when \w is specified, it does not copy tab properties. Strange behavior, I prefer simply replace
                // with a space or preserve, just like for line breaks.
                bool shouldReplaceTab = ShouldReplaceTab(c, run);
                bool shouldReplaceLineBreak = ShouldReplaceLineBreak(c);

                if (shouldReplaceTab || shouldReplaceLineBreak)
                    builder.Append(' ');
                else
                    builder.Append(c);
            }

            run.Text = builder.ToString();
        }

        private Run TrimWhiteSpaces(Run run)
        {
            run = TrimLeadingWhitespaces(run);
            FindPossibleTrailingWhitespaces(run);
            return run;
        }

        private Run TrimLeadingWhitespaces(Run run)
        {
            if (mNotWhitespaceReached)
                return run;

            int leadingWhitespaceCharCount = 0;
            foreach (char c in run.Text)
            {
                if (StringUtil.IsWhiteSpace(c))
                {
                    leadingWhitespaceCharCount++;
                }
                else
                {
                    ProcessNotWhiteSpace();
                    break;
                }
            }

            if (leadingWhitespaceCharCount == run.Text.Length)
                return null;

            if (leadingWhitespaceCharCount > 0)
                run.Text = run.Text.Remove(0, leadingWhitespaceCharCount);

            return run;
        }

        private void FindPossibleTrailingWhitespaces(Run run)
        {
            if (!mNotWhitespaceReached)
                return;

            bool isTrailingWhitespace = false;
            for (int i = 0; i < run.Text.Length; i++)
            {
                char c = run.Text[i];
                if (StringUtil.IsWhiteSpace(c))
                {
                    isTrailingWhitespace = true;
                    if (mFirstTrailingWhitespaceCharPosition == -1)
                        mFirstTrailingWhitespaceCharPosition = i;
                }
                else
                {
                    isTrailingWhitespace = false;
                    ProcessNotWhiteSpace();
                }
            }

            if (isTrailingWhitespace)
                mTrailingWhitespaceRuns.Add(run);
        }

        private void ProcessNotWhiteSpace()
        {
            mNotWhitespaceReached = true;
            mTrailingWhitespaceRuns.Clear();
            mFirstTrailingWhitespaceCharPosition = -1;
        }

        private bool ShouldReplaceTab(char c, Run run)
        {
            if (c != ControlChar.TabChar)
                return false;

            if (mPreserveTabs)
                return false;

            if (mRunWithTab != null)
                return true;

            mRunWithTab = run;
            return false;
        }

        private bool ShouldReplaceLineBreak(char c)
        {
            return (c == ControlChar.LineBreakChar) && !mPreserveLineBreaks;
        }

        private Run mRunWithTab;

        private bool mNotWhitespaceReached;
        private readonly List<Run> mTrailingWhitespaceRuns = new List<Run>();
        private int mFirstTrailingWhitespaceCharPosition = -1;

        private readonly bool mPreserveTabs;
        private readonly bool mPreserveLineBreaks;
        private readonly int mEntryLevel;
        private readonly bool mIsTableOfFigures;
    }
}
