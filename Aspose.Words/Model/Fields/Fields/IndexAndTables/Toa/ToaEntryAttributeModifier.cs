// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2017 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Modifies TOA entry font attributes.
    /// </summary>
    internal class ToaEntryAttributeModifier : IndexAndTablesEntryAttributeModifier, INodeModifier
    {
        public ToaEntryAttributeModifier(StyleCollection styles, bool clearDirectFormatting)
            : base(StyleIdentifier.TableOfAuthorities, styles)
        {
            mClearDirectFormatting = clearDirectFormatting;
        }

        Node INodeModifier.Modify(Node referenceNode, Node nodeToModify, bool modifyChildren, INodeCloningListener cloningListener)
        {
            return Modify(referenceNode, nodeToModify);
        }

        protected override void ModifyInlineNode(Inline sourceInline, Inline modifiedInline)
        {
            StripExtraDirectAttributes(sourceInline, modifiedInline);
        }

        protected override bool IsExtraDirectAttribute(int directAttrKey, object directAttrValue, Inline sourceInline)
        {
            return mClearDirectFormatting
                || base.IsExtraDirectAttribute(directAttrKey, directAttrValue, sourceInline)
                || IsMinorHAnsiThemeFontAttr(directAttrKey, directAttrValue);
        }

        private readonly bool mClearDirectFormatting;
    }
}
