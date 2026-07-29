// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/08/2005 by Roman Korchagin

using System.Text.RegularExpressions;
using Aspose.JavaAttributes;

namespace Aspose.Words.Replacing
{
    /// <summary>
    /// Provides data for a custom replace operation.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/find-and-replace/">Find and Replace</a> documentation article.</para>
    /// </summary>
    /// <seealso cref="IReplacingCallback"/>
    /// <seealso cref="Range"/>
    /// <seealso cref="Range.Replace(string, string, FindReplaceOptions)"/>
    public class ReplacingArgs
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal ReplacingArgs(Match match, int matchOffset, Node matchStartNode, Node matchEndNode, string replacement)
        {
            mMatch = match;
            MatchNode = matchStartNode;
            MatchEndNode = matchEndNode;
            mMatchOffset = matchOffset;
            mReplacement = replacement;
        }

        /// <summary>
        /// The <see cref="System.Text.RegularExpressions.Match"/> resulting from a single regular
        /// expression match during a <b>Replace</b>.
        /// </summary>
        /// <remarks>
        /// <p><ms><b>Match.Index"</b></ms><java><tt>Matcher.start()</tt></java><cpp><b>Match.Index"</b></cpp> gets the zero-based starting
        /// position of the match from the start of the find and replace range.</p>
        /// </remarks>
        public Match Match
        {
            get { return mMatch; }
        }

        /// <summary>
        /// Gets the node that contains the beginning of the match.
        /// </summary>
        public Node MatchNode { get; }

        /// <summary>
        /// Gets the node that contains the end of the match.
        /// </summary>
        public Node MatchEndNode { get; }

        /// <summary>
        /// Gets the zero-based starting position of the match from the start of
        /// the node that contains the beginning of the match.
        /// </summary>
        public int MatchOffset
        {
            get { return mMatchOffset; }
        }

        /// <summary>
        /// Gets or sets the replacement string.
        /// </summary>
        public string Replacement
        {
            get { return mReplacement; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mReplacement = value;
            }
        }

        /// <summary>
        /// Identifies, by name, a captured group in the <see cref="Match"/>
        /// that is to be replaced with the <see cref="Replacement"/> string.
        /// </summary>
        /// <remarks>
        /// <p>When group name is <c>null</c>, <see cref="GroupIndex"/> is used to identify the group.</p>
        /// <p>Default is <c>null</c>.</p>
        /// </remarks>
        /// <msonly>Remove this from Java public API.</msonly>
        [JavaDelete("Java regex does not support group names, it supports only group indexes.")]
        public string GroupName
        {
            get { return mGroupName; }
            set { mGroupName = value; }
        }
        private string mGroupName;

        /// <summary>
        /// Identifies, by index, a captured group in the <see cref="Match"/>
        /// that is to be replaced with the <see cref="Replacement"/> string.
        /// </summary>
        /// <remarks>
        /// <ms><p><see cref="GroupIndex"/> has effect only when <see cref="GroupName"/> is <c>null</c>.</p></ms>
        /// <cpp><p><see cref="GroupIndex"/> has effect only when <see cref="GroupName"/> is <c>null</c>.</p></cpp>
        /// <p>Default is zero.</p>
        /// </remarks>
        public int GroupIndex
        {
            get { return mGroupIndex; }
            set { mGroupIndex = value; }
        }

        private readonly Match mMatch;
        private readonly int mMatchOffset;
        private string mReplacement;
        private int mGroupIndex;
    }
}
