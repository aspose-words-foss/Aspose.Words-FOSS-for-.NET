// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/09/2024 by Edward Voronov

using System.Collections.Generic;
using System.Text;
using Aspose.Words.Notes;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Validates footnote and endnote references.
    /// </summary>
    internal class FootnoteValidator
    {
        internal void VisitFootnote(Footnote footnote)
        {
            if (!string.IsNullOrEmpty(footnote.ReferenceMark) || footnote.IsAuto)
                return;

            mMissedCustomMarkFootnotes.Enqueue(footnote);
        }

        internal void VisitRun(Run run, StoryTypeStack storyTypeStack)
        {
            if (run.IsRemoved)
                return;

            if (storyTypeStack.Current != StoryType.MainText)
                return;

            Footnote footnote = MoveAndJoinFootnotes(run);
            if (footnote == null)
                return;

            if (!footnote.IsAuto)
                footnote.ReferenceMark = CollectRunTextWithSameFormatting(run);
        }

        private static string CollectRunTextWithSameFormatting(Run run)
        {
            const RunPrExpandFlags expandFlags = RunPrExpandFlags.DocumentDefaults | RunPrExpandFlags.GlobalDefaults;

            Node curNode = run.NextSibling;
            RunPr runPrExp = null;
            StringBuilder sb = null;

            while (curNode != null)
            {
                if (curNode.NodeType != NodeType.Run)
                    break;

                Run curRun = (Run)curNode;

                RunPr curRunPrExp = curRun.GetExpandedRunPr(expandFlags);
                if (runPrExp == null)
                    runPrExp = run.GetExpandedRunPr(expandFlags);

                if (!RunPr.IsSameFormatting(curRunPrExp, runPrExp))
                    break;

                if (sb == null)
                    sb = new StringBuilder(run.Text);

                sb.Append(curRun.Text);

                curNode = curNode.NextSibling;

                curRun.Remove();
            }

            run.Remove();

            return sb != null
                ? sb.ToString()
                : run.Text;
        }

        private Footnote MoveAndJoinFootnotes(Run run)
        {
            Footnote result = null;

            while (true)
            {
                if (mMissedCustomMarkFootnotes.Count == 0)
                    return result;

                Footnote footnote = mMissedCustomMarkFootnotes.Peek();

                if (footnote.ParentNode == run.ParentNode)
                    return result;

                if (result != null)
                {
                    result.IsAuto = false;
                    result.InsertAfter(footnote.FirstChild, null, null);
                    footnote.Remove();
                }
                else
                {
                    run.InsertPrevious(footnote);
                    footnote.IsAuto = IsAutoFootnote(footnote);
                    footnote.RunPr = run.RunPr.Clone();

                    result = footnote;
                }

                mMissedCustomMarkFootnotes.Dequeue();
            }
        }

        private static bool IsAutoFootnote(Footnote footnote)
        {
            if (footnote.FootnoteType == FootnoteType.Endnote)
                return false;

            foreach (SpecialChar specialChar in footnote.GetChildNodes(NodeType.SpecialChar, true))
            {
                if (specialChar.Char == ControlChar.FootnoteRefChar)
                    return true;
            }

            return false;
        }

        private readonly Queue<Footnote> mMissedCustomMarkFootnotes = new Queue<Footnote>();
    }
}
