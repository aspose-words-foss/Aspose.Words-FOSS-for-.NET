// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/04/2013 by Ivan Lyagin

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Modifies INDEX entry font attributes.
    /// </summary>
    internal class IndexEntryAttributeModifier : IndexAndTablesEntryAttributeModifier, INodeModifier
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal IndexEntryAttributeModifier(StyleIdentifier styleId, StyleCollection styles)
            : base(styleId, styles)
        {
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
            return IsIndexExtraDirectAttribute(directAttrKey)
                || base.IsExtraDirectAttribute(directAttrKey, directAttrValue, sourceInline)
                || IsMinorHAnsiThemeFontAttr(directAttrKey, directAttrValue);
        }

        /// <summary>
        /// Returns a value indicating whether the font attribute with the specified key is an INDEX-specific extra attr.
        /// </summary>
        private static bool IsIndexExtraDirectAttribute(int key)
        {
            return (key == FontAttr.Hidden);
        }
    }
}
