// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2011 by Dmitry Matveenko

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Modifies TOC entry font attributes for a not-hyperlink TOC entry (TOC without \\h switch).
    /// </summary>
    internal class TocNormalEntryAttributeModifier : IndexAndTablesEntryAttributeModifier, INodeModifier
    {
        internal TocNormalEntryAttributeModifier(
            StyleIdentifier tocStyleId,
            StyleCollection styles,
            bool copyCharStyleAttributes)
            : base(tocStyleId, styles)
        {
            mCopyCharStyleAttributes = copyCharStyleAttributes;
        }

        Node INodeModifier.Modify(Node referenceNode, Node nodeToModify, bool modifyChildren, INodeCloningListener cloningListener)
        {
            return Modify(referenceNode, nodeToModify);
        }

        /// <summary>
        /// Implements Toc entry node font attributes modification for a TOC without \\h switch.
        /// </summary>
        protected override void ModifyInlineNode(Inline sourceInline, Inline modifiedInline)
        {
            StripExtraDirectAttributes(sourceInline, modifiedInline);
            AddDirectAttributesForCharStyle(sourceInline, modifiedInline);
        }

        /// <summary>
        /// Currently, it is used for testing only.
        /// Strips extra direct formatting attributes from the current node (if it is inline) and all inline children.
        /// </summary>
        internal static void StripInlineFontAttrsForToc(Node node)
        {
            NodeRange nodeRange = new NodeRange(node, node);

            TocNormalEntryAttributeModifier stripper = new TocNormalEntryAttributeModifier(
                StyleIdentifier.DefaultParagraphFont,
                node.Document.Styles,
                true);

            stripper.StripInlineFontAttrsForToc(nodeRange);
        }

        /// <summary>
        /// Currently, it is used for testing only.
        /// Strips extra direct formatting from all inline nodes in the given range.
        /// </summary>
        private void StripInlineFontAttrsForToc(NodeRange strippingRange)
        {
            using (NodeEnumerator strippingEnumerator = new NodeEnumerator(strippingRange))
            {
                while (strippingEnumerator.MoveToNextNode())
                {
                    Inline currentNode = strippingEnumerator.CurrentNode as Inline;
                    if (currentNode != null)
                    {
                        StripExtraDirectAttributes(currentNode, currentNode);
                    }
                }
            }
        }

        /// <summary>
        /// Adds direct attributes to the copied entry in TOC.
        /// This is needed when an attribute is set via character style and no direct formatting is present.
        /// As the character style is not copied to the TOC, to make TOC entry look the same as the source,
        /// direct formatting must be added.
        /// </summary>
        private void AddDirectAttributesForCharStyle(Inline sourceInline, Inline modifiedInline)
        {
            if (!mCopyCharStyleAttributes)
                return;

            if (EntryStyle == null)
                return;

            RunPr charStylePr = sourceInline.CharacterStyle.GetExpandedRunPr(RunPrExpandFlags.Normal);
            RunPr sourceRunPr = sourceInline.RunPr;
            RunPr modifiedRunPr = modifiedInline.RunPr;

            for (int attrIdx = 0; attrIdx < charStylePr.Count; ++attrIdx)
            {
                int charStyleAttrKey = charStylePr.GetKey(attrIdx);
                object charStyleAttrValue = charStylePr[charStyleAttrKey];

                if (IsStrippedUncoditionally(charStyleAttrKey))
                    continue;

                bool isHandledByKeepingDirectFormatting = (sourceRunPr[charStyleAttrKey] != null);
                if (isHandledByKeepingDirectFormatting)
                    continue;

                // Add a direct formatting so that entry formatting in TOC
                // matches source entry formatting via character style.
                if (charStyleAttrValue is AttrBoolEx)
                {
                    // If the toggle is turned off via character style it does not differ from the case when it is not set at all.
                    // Ignore toggled off AttrBoolEx.
                    // It is assumed that all AttrBoolEx'es are False by default.
                    bool attrToggledOn = (charStyleAttrValue == AttrBoolEx.True);
                    if (attrToggledOn && !ResolvedAttrValueMatchesParagraphStyle(charStyleAttrKey, sourceInline))
                    {
                        // For toggle attributes, add a toggle to TOC paragraph style value.
                        // Resolved value from the source won't work.
                        modifiedRunPr.Add(charStyleAttrKey, AttrBoolEx.Toggle);
                    }
                }
                else if (!ResolvedAttrValueMatchesParagraphStyle(charStyleAttrKey, sourceInline))
                {
                    // For normal attributes, just add the attribute value to keep formatting set by character style in TOC.
                    modifiedRunPr.Add(charStyleAttrKey, charStyleAttrValue);
                }
            }
        }

        private readonly bool mCopyCharStyleAttributes;
    }
}
