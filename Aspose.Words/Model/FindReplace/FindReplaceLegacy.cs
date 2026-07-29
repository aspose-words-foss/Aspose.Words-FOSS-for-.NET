// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/08/2005 by Roman Korchagin

using System;
using System.Text.RegularExpressions;
using Aspose.Words.Markup;

namespace Aspose.Words.Replacing
{
    //TODO 2 Allow regex substitution patterns to be used in the replacement text.

    //TODO 3 Allow replacing replace paragraph, section and page breaks, maybe even cell breaks.

    //TODO 2 Create a sample that shows how to replace using a group name or index.
    //TODO 2 Create a sample to include increasing numbers into text.
    //TODO 2 Create a sample that shows how to replace including or excluding field codes.
    //TODO 2 Create a sample that shows how to replace in body only or in headers only.

    /// <summary>
    /// Performs replace operations over the document tree.
    /// Maybe later can make a public class.
    /// </summary>
    /// <dev>
    /// This is legacy find/replace algorithm, we had to return it after customer complained. See WORDSNET-20007
    /// </dev>
    internal class FindReplaceLegacy
    {
        internal FindReplaceLegacy(Node rootNode, Regex pattern, string replacement, IReplacingCallback handler, bool isForward)
        {
            this.RootNode = rootNode;
            this.Pattern = pattern;
            this.Replacement = replacement;
            this.ReplacingCallback = handler;
            this.IsForward = isForward;

            mDoc = rootNode.Document;
        }

        internal FindReplaceLegacy(NodeRange nodeRange, Regex pattern, string replacement, IReplacingCallback handler, bool isForward)
        {
            mNodeRange = nodeRange;
            this.Pattern = pattern;
            this.Replacement = replacement;
            this.ReplacingCallback = handler;
            this.IsForward = isForward;

            mDoc = nodeRange.Document;
        }

        internal FindReplaceLegacy(Node rootNode, string oldValue, string newValue, bool isMatchCase, bool isMatchWholeWord)
            : this(rootNode, TextToPattern(oldValue, isMatchCase, isMatchWholeWord), newValue, null, false)
        {
            //Do nothing.
        }

        internal FindReplaceLegacy(NodeRange nodeRange, string oldValue, string newValue, bool isMatchCase, bool isMatchWholeWord)
            : this(nodeRange, TextToPattern(oldValue, isMatchCase, isMatchWholeWord), newValue, null, false)
        {
            //Do nothing.
        }

        /// <summary>
        /// Gets or sets the node that is the root for the replace operation.
        /// </summary>
        internal Node RootNode
        {
            get { return mRootNode; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                mRootNode = value;
            }
        }

        /// <summary>
        /// Gets or sets the regular expression used to find matches.
        /// </summary>
        internal Regex Pattern
        {
            get { return mPattern; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.ToString() == "")
                    throw new ArgumentException("The search string cannot be empty.");

                //No pointing checking for break characters in the regex because the pattern
                //can capture them even if they are not specified explicitly.

                mPattern = value;
            }
        }

        /// <summary>
        /// Gets or sets the replacement string.
        /// </summary>
        /// <remarks>
        /// <p><b>Replacement</b> is used when no custom <b>ReplaceEvaluator</b> is specified.</p>
        /// </remarks>
        internal string Replacement
        {
            get { return mReplacement; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.IndexOfAny(gCannotInsertChars) != -1)
                    throw new ArgumentException("The replace string cannot contain special or break characters.");

                mReplacement = value;
            }
        }

        /// <summary>
        /// Gets or sets the method that will be called for each match, allows the user to control how the replace is made.
        /// </summary>
        /// <remarks>
        /// <p>If null, the <b>Replacement</b> string will be used to replace all found matches.</p>
        /// </remarks>
        internal IReplacingCallback ReplacingCallback
        {
            get { return mReplacingCallback; }
            set { mReplacingCallback = value; }
        }

        /// <summary>
        /// Indicates whether the find and replace will be performed in forward or backward direction inside the <b>RootNode</b>.
        /// </summary>
        /// <remarks>
        /// <p>Default is false.</p>
        /// </remarks>
        internal bool IsForward
        {
            get { return mIsForward; }
            set { mIsForward = value; }
        }

        private MatchCollection GetMatches()
        {
            if (mNodeRange != null)
            {
                NodeTextCollectorOptions options = new NodeTextCollectorOptions();
                options.SkipCommentText = false;
                options.SkipFootnoteText = false;

                return mPattern.Matches(NodeTextCollector.GetText(mNodeRange, options));
            }

            return mPattern.Matches(mRootNode.GetText());
        }

        private NodeIndexer GetNodeIndexer()
        {
            return mNodeRange == null
                ? new NodeIndexer(mRootNode)
                : new NodeIndexer(mNodeRange);
        }

        private CompositeNode GetCommonParent(Node node1, Node node2)
        {
            int nodeDepth1 = NodeUtil.GetDepth(node1);
            int nodeDepth2 = NodeUtil.GetDepth(node2);

            CompositeNode parent1 = (nodeDepth2 > nodeDepth1)
                ? node1.ParentNode
                : node2.ParentNode;

            while (parent1 != null)
            {
                CompositeNode parent2 = (nodeDepth2 > nodeDepth1)
                    ? node2.ParentNode
                    : node1.ParentNode;

                while (parent2 != null)
                {
                    if (parent1 == parent2)
                        return parent1;

                    parent2 = parent2.ParentNode;
                }

                parent1 = parent1.ParentNode;
            }

            return mDoc;
        }

        private Node GetParentNode()
        {
            return mNodeRange == null
                ? RootNode
                : GetCommonParent(mNodeRange.Start.Node, mNodeRange.End.Node);
        }

        /// <summary>
        /// Executes the find and replace operation.
        /// </summary>
        /// <returns>The number of replacements made.</returns>
        internal int Replace()
        {
            MatchCollection matches = GetMatches();

            NodeIndexer nodeIndexer = GetNodeIndexer();
            nodeIndexer.Update();

            int replaceCount = 0;

            //This loop works both for forward and backward replace.
            int remainingMatchesCount = matches.Count;
            int step = (mIsForward) ? 1 : -1;                //Step of the loop, different depending on direction.
            int i = (mIsForward) ? 0 : matches.Count - 1;    //Initialize the current match index depending on direction.
            ReplacementInfo lastReplacementInfo = null;      //Information about the last replacement
            int runPosDelta = 0;                             //Total position delta in the last run, in which
                                                             //replacement was. (For several replacements in one run.)
            while (remainingMatchesCount > 0)
            {
                Match match = matches[i];

                //I'm not sure why, but some patterns yield matches of zero length.
                //I don't think I need to try to replace them.
                if (match.Length > 0)
                {
                    //Local position of the match in the first run.
                    int matchNodeStart = nodeIndexer.GetNodeStartByPosition(match.Index);
                    int matchOffset = match.Index - matchNodeStart;
                    Node matchNode = nodeIndexer.GetNodeByPosition(match.Index);
                    if ((mIsForward) &&
                        (lastReplacementInfo != null) &&
                        (lastReplacementInfo.LastRun == matchNode))
                    {
                        // The previous replacement was in the same run: the offset should be corrected
                        runPosDelta += lastReplacementInfo.PosDelta;
                        matchOffset += runPosDelta;
                    }
                    else
                    {
                        runPosDelta = 0;
                    }

                    // WORDSNET-28070 Added a new replacing arg to get match end node.
                    int endIndex = matchOffset + matches[i].Length;
                    Node matchEndNode = nodeIndexer.GetNodeByPosition(endIndex);

                    ReplacingArgs args = new ReplacingArgs(match, matchOffset, matchNode, matchEndNode, mReplacement);
                    ReplaceAction action = (mReplacingCallback != null) ? DoReplacingCallback(args, nodeIndexer) :
                        ReplaceAction.Replace;

                    switch (action)
                    {
                        case ReplaceAction.Replace:
                        {
                            lastReplacementInfo = ReplaceOccurrence(args);

                            if (lastReplacementInfo.LastRun != null)
                                replaceCount++;

                            break;
                        }
                        case ReplaceAction.Skip:
                            break;
                        case ReplaceAction.Stop:
                            return replaceCount;
                        default:
                            throw new InvalidOperationException("Unknown replace action.");
                    }
                }

                i += step;
                remainingMatchesCount--;
            }

            return replaceCount;
        }

        /// <summary>
        /// Calls replacing callback to get action for a match.
        /// </summary>
        /// <remarks>May rebuild the node index if document tree will be changed in the callback.</remarks>
        private ReplaceAction DoReplacingCallback(ReplacingArgs args, NodeIndexer nodeIndexer)
        {
            int treeChangeCount = mDoc.TreeChangeCount;

            ReplaceAction action = mReplacingCallback.Replacing(args);

            if (mDoc.TreeChangeCount != treeChangeCount)
            {
                if (!mIsForward)
                {
                    // WORDSNET-9902 Should rebuild indexes after document tree was changed.
                    nodeIndexer.Update();
                }
                else
                {
                    // Further replacements are disabled because wrong result may be got.
                    throw new InvalidOperationException(
                        "Cannot perform further replacements since the document has been changed.");
                }
            }

            return action;
        }

        /// <summary>
        /// Replaces a single occurrence according to the parameters specified in the args.
        /// </summary>
        /// <returns>An object that contains the last modified run and the difference between the number
        /// of inserted and deleted characters in the last run.
        /// If inserted more characters than deleted, the number is positive.</returns>
        private ReplacementInfo ReplaceOccurrence(ReplacingArgs args)
        {
            ReplacementInfo infoToReturn = new ReplacementInfo();

            Group group;

            // FindReplace tracking is quite complex task so postpone it till later and
            // suspend tracking during this operation.
            using (new SuspendTrackRevisionsDocument(mDoc))
            {
#if !JAVA
                // Java regex does not support group names, it supports only group indexes.
                if (args.GroupName != null)
                    group = args.Match.Groups[args.GroupName];
                else
#endif
                    group = args.Match.Groups[args.GroupIndex];

                if (group.Value.IndexOfAny(gCannotReplaceChars) != -1)
                    throw new NotSupportedException("The match includes one or more special or break characters and cannot be replaced.");

                // We shall ignore SpecialChar node.
                if (args.MatchNode.NodeType == NodeType.SpecialChar)
                    return infoToReturn;

                Run firstRun = (Run)args.MatchNode;

                // We shall replace CustomXML property content to avoid a false positive replacement of SDT content
                // which will be rolled back by a subsequent UpdateDataBoundContent().
                if (firstRun.ParentNode.NodeType == NodeType.StructuredDocumentTag)
                {
                    StructuredDocumentTag sdt = (StructuredDocumentTag)firstRun.ParentNode;
                    if (!SdtContentHelper.ReplaceDataBoundContent(sdt, mPattern, mReplacement))
                        return infoToReturn;
                }

                //Keep a counter how much is still to delete.
                int remainingMatchLength = group.Length;
                //Where the remove starts in the current run.
                int removeStartInCurRun = args.MatchOffset;
                Run curRun = firstRun;

                //Loop because the match can span more than one node.
                while (remainingMatchLength > 0)
                {
                    int availableToRemoveInCurRun = curRun.GetTextLength() - removeStartInCurRun;
                    int removeCountInCurRun = System.Math.Min(availableToRemoveInCurRun, remainingMatchLength);

                    curRun.Text = curRun.Text.Remove(removeStartInCurRun, removeCountInCurRun);

                    remainingMatchLength -= removeCountInCurRun;
                    removeStartInCurRun = 0;

                    infoToReturn.LastRun = curRun;
                    infoToReturn.PosDelta = -removeCountInCurRun;

                    // Select the next node because we might need to remove the current run.
                    Node nextNode = curRun;
                    do
                    {
                        // We could get runs, bookmarks, smart tags etc here. Skip all except runs.
                        nextNode = nextNode.NextPreOrder(GetParentNode());
                    } while ((nextNode != null) && (nextNode.NodeType != NodeType.Run));

                    // We normally delete the run node if it becomes empty, but
                    // keep the first run for now since it might have to stay to keep the formatting.
                    if ((curRun != firstRun) && (curRun.Text == ""))
                        curRun.Remove();

                    curRun = (Run)nextNode;
                }

                //Insert new text.
                firstRun.Text = firstRun.Text.Insert(args.MatchOffset, args.Replacement);
                //Remove the empty first run if it was completely cleared.
                if (firstRun.Text == "")
                    firstRun.Remove();

                if (infoToReturn.LastRun == firstRun)
                {
                    // The replacement is in one run.
                    infoToReturn.PosDelta = args.Replacement.Length - group.Length;
                }
            }

            return infoToReturn;
        }

        /// <summary>
        /// Creates a regex pattern for a simple text search string.
        /// </summary>
        private static Regex TextToPattern(string oldValue, bool isMatchCase, bool isMatchWholeWord)
        {
            string pattern = Regex.Escape(oldValue);
            if (isMatchWholeWord)
                pattern = "\\b" + pattern + "\\b";    //This means extract at the word boundary.

            RegexOptions options = (isMatchCase) ? RegexOptions.None : RegexOptions.IgnoreCase;

            return new Regex(pattern, options);
        }

        /// <summary>
        /// At the moment cannot replace any of these chars.
        /// </summary>
        private static readonly char[] gCannotReplaceChars = new char[]
        {
            ControlChar.PictureChar,
            ControlChar.FootnoteRefChar,
            ControlChar.FootnoteSeparatorChar,
            ControlChar.FootnoteContinuationChar,
            ControlChar.CellChar,
            //TODO 2 It is unfortunate that page break and section break symbols are same.
            //This prevents us from letting user to search for page break characters.
            ControlChar.SectionBreakChar,
            ControlChar.ParagraphBreakChar,
            ControlChar.FieldStartChar,
            ControlChar.FieldSeparatorChar,
            ControlChar.FieldEndChar
        };

        /// <summary>
        /// Cannot insert any of these chars. Similar to the chars that we cannot replace,
        /// with the exception of page break character (actually same code as section break).
        /// </summary>
        private static readonly char[] gCannotInsertChars = new char[]
        {
            ControlChar.PictureChar,
            ControlChar.FootnoteRefChar,
            ControlChar.FootnoteSeparatorChar,
            ControlChar.FootnoteContinuationChar,
            ControlChar.CellChar,
            ControlChar.ParagraphBreakChar,
            ControlChar.FieldStartChar,
            ControlChar.FieldSeparatorChar,
            ControlChar.FieldEndChar
        };


        private readonly DocumentBase mDoc;
        private Node mRootNode;
        private readonly NodeRange mNodeRange;
        private Regex mPattern;
        private string mReplacement;
        private IReplacingCallback mReplacingCallback;
        private bool mIsForward;

        /// <summary>
        /// Information about performed replacement.
        /// </summary>
        private class ReplacementInfo
        {
            /// <summary>
            /// Returns the last run, in which the replacement has been performed.
            /// </summary>
            internal Run LastRun
            {
                get { return mLastRun; }
                set { mLastRun = value; }
            }

            /// <summary>
            /// Returns position delta for the further replacement in the <see cref="LastRun"/>
            /// </summary>
            internal int PosDelta
            {
                get { return mPosDelta; }
                set { mPosDelta = value; }
            }

            private Run mLastRun;
            private int mPosDelta;
        }

    }
}
