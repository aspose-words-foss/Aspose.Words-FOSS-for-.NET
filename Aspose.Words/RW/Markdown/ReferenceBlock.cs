// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/05/2023 by Vadim Saltykov

using System.Collections.Generic;
using System.Globalization;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown ReferenceBlock block.
    /// </summary>
    internal abstract class ReferenceBlock : ContainerBlock
    {
        /// <summary>
        /// Resolve destination link.
        /// </summary>
        internal bool Resolve(Dictionary<string, LinkDefinitionBlock> linkDefinitions, Stack<ReferenceBlock> unresolvedLinks)
        {
            if (ResolveCore(Label, linkDefinitions))
            {
                if (IsFullReferenceText() && (unresolvedLinks.Count > 0))
                {
                    ReferenceBlock linkLabelBlock = unresolvedLinks.Pop();
                    linkLabelBlock.ResolveCore(Label, linkDefinitions);
                    linkLabelBlock.SetRawText(mRawText);
                    return true;
                }
            }
            else if (IsCollapsedReferenceText())
                return true;
            else
            {
                unresolvedLinks.Push(this);
                mRawText = string.Empty;
            }

            return false;
        }

        /// <summary>
        /// Returns true, if appropriate LinkDestinationBlock is not null.
        /// </summary>
        internal bool HasLinkDestination()
        {
            return (LinkDestinationBlock != null);
        }

        /// <summary>
        /// Raw text containing all special characters.
        /// </summary>
        /// <remarks>
        /// Used only for mapping LinkText and LinkDefinition.
        /// </remarks>
        internal void SetRawText(string text)
        {
            mRawText = text;
        }

        /// <summary>
        /// Expands the text content of the child block that was lost when the current block was parsed.
        /// </summary>
        internal void ExpandInlineLinkText()
        {
            if (Count == 0)
                Add(new InlineBlock(string.Empty));

            InlineBlock inline = this[0] as InlineBlock;
            if (inline != null)
                inline.TryParse(string.Format("{0}{1}{2}", OpeningStringDelimiter, inline.ContentLine,
                    LinkTextBlock.ClosingDelimiter), 0);
        }

        /// <summary>
        /// Gets a string value representing a label of the LinkDefinition block.
        /// </summary>
        internal string DefinitionLabel
        {
            get
            {
                return (StringUtil.HasChars(mRawText))
                    ? string.Format("{0}{1}{2}", LinkTextOpeningDelimiter.Character, mRawText, LinkTextClosingDelimiter.Character)
                    : string.Empty;
            }
        }

        /// <summary>
        /// Resolve destination link.
        /// </summary>
        private bool ResolveCore(string label, Dictionary<string, LinkDefinitionBlock> linkDefinitions)
        {
            bool retValue = (InlineLinkDestinationBlock != null) ||
                            linkDefinitions.TryGetValue(label, out mLinkDefinition);

            return retValue;
        }

        /// <summary>
        /// True if the current reference block is the link text part of the full reference link.
        /// </summary>
        private bool IsFullReferenceText()
        {
            return (IsReferenceLink() && StringUtil.HasChars(Text));
        }

        /// <summary>
        /// True if the current reference block is the link text part of the collapsed reference link.
        /// </summary>
        private bool IsCollapsedReferenceText()
        {
            return (IsReferenceLink() && !StringUtil.HasChars(Text));
        }

        /// <summary>
        /// True if the current reference block is the reference link.
        /// </summary>
        private bool IsReferenceLink()
        {
            if (InlineLinkDestinationBlock != null)
                return false;

            LinkTextBlock prevLinkDesc = PreviousSibling as LinkTextBlock;
            ImageDescriptionBlock prevImageDesc = PreviousSibling as ImageDescriptionBlock;
            LinkTextBlock next = NextSibling as LinkTextBlock;
            if ((((prevLinkDesc != null) || (prevImageDesc != null)) && (next != null)) ||
                ((prevLinkDesc == null) && (prevImageDesc == null) && (next == null)))
                return false;

            return ((prevLinkDesc != null) || (prevImageDesc != null));
        }

        /// <summary>
        /// Gets a corresponding LinkDestination block, or <c>null</c> if not exist.
        /// </summary>
        internal LinkDestinationBlock LinkDestinationBlock
        {
            get
            {
                return (InlineLinkDestinationBlock != null)
                    ? InlineLinkDestinationBlock
                    : (mLinkDefinition != null) ? mLinkDefinition.LinkDestination : null;
            }
        }

        /// <summary>
        /// Gets a corresponding inline LinkDestination block, or <c>null</c> if not exist.
        /// </summary>
        private LinkDestinationBlock InlineLinkDestinationBlock
        {
            get { return NextSibling as LinkDestinationBlock; }
        }

        /// <summary>
        /// Gets a string value representing a label of the current reference block.
        /// </summary>
        private string Label
        {
            get
            {
                return (StringUtil.HasChars(mRawText))
                    ? mRawText.ToUpper(CultureInfo.InvariantCulture)
                    : Text.ToUpper(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Opening string delimiter.
        /// </summary>
        protected abstract string OpeningStringDelimiter { get; }

        private string mRawText;

        private LinkDefinitionBlock mLinkDefinition;
    }

}
