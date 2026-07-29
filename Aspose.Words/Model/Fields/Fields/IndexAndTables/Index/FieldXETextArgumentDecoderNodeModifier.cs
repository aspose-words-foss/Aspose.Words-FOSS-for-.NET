// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/04/2013 by Ivan Lyagin

using System.Text;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Unescapes XE field text argument performing its validity check and trimming of double quotes if present.
    /// </summary>
    internal class FieldXETextArgumentDecoderNodeModifier : FieldTokenDecoderNodeModifier
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldXETextArgumentDecoderNodeModifier(NodeRange textRange)
            : base(textRange, FieldTokenDecoderOptions.All)
        {
        }

        protected override bool PrepareModify(Node referenceNode, Node nodeToModify)
        {
            if (NodeUtil.IsZln(nodeToModify) || (nodeToModify is InlineStory))
                return false;

            return true;
        }

        protected override StringBuilder ProcessDecodedTokenChar(StringBuilder builder, char c, int positionInTokenPart)
        {
            // Replace any whitespace character by the space as MS Word does.
            char modifiedChar = char.IsWhiteSpace(c)
                ? ControlChar.SpaceChar
                : c;
            return base.ProcessDecodedTokenChar(builder, modifiedChar, positionInTokenPart);
        }

        protected override RichStringBuilder ProcessDecodedTokenChar(RichStringBuilder builder, RichChar c, int positionInTokenPart)
        {
            Debug.Fail("Rich strings are not supported");

            return base.ProcessDecodedTokenChar(builder, c, positionInTokenPart);
        }
    }
}
