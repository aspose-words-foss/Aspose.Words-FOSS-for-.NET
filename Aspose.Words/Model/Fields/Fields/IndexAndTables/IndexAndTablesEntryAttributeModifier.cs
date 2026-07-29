// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2011 by Dmitry Matveenko

using Aspose.JavaAttributes;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// A base class for modifying INDEX, TOC and TOA entry font attributes when copying to the INDEX, TOC or TOA.
    /// </summary>
    internal abstract class IndexAndTablesEntryAttributeModifier
    {
        /// <summary>
        /// Initializes base class members.
        /// </summary>
        /// <param name="styleId"></param>
        /// <param name="styles"></param>
        protected IndexAndTablesEntryAttributeModifier(StyleIdentifier styleId, StyleCollection styles)
        {
            EntryStyle = styles.GetBySti(styleId, true);
        }

        /// <summary>
        /// When implemented in a derived class, modifies the attributes of inline node to be placed in the INDEX, TOC or TOA.
        /// </summary>
        /// <param name="sourceInline"></param>
        /// <param name="modifiedInline"></param>
        [JavaThrows(true)]
        protected abstract void ModifyInlineNode(Inline sourceInline, Inline modifiedInline);

        /// <summary>
        /// Implements common parameter checks to perform before node modification.
        /// </summary>
        /// <param name="referenceNode"></param>
        /// <param name="nodeToModify"></param>
        /// <returns></returns>
        protected Node Modify(Node referenceNode, Node nodeToModify)
        {
            Debug.Assert(referenceNode != null);
            Debug.Assert(nodeToModify != null);

            if (!ModifyInlineNode(referenceNode, nodeToModify))
            {
                // WORDSNET-18798 modify SDT nested inline nodes.
                if (referenceNode.NodeType == NodeType.StructuredDocumentTag)
                {
                    NodeEnumerator referenceRange = (NodeEnumerator) new NodeRange(referenceNode, referenceNode).GetEnumerator();
                    NodeEnumerator modifiedRange = (NodeEnumerator) new NodeRange(nodeToModify, nodeToModify).GetEnumerator();

                    while (referenceRange.MoveToNextNode() && modifiedRange.MoveToNextNode())
                        ModifyInlineNode(referenceRange.CurrentNode, modifiedRange.CurrentNode);
                }
            }

            return nodeToModify;
        }

        private bool ModifyInlineNode(Node referenceNode, Node nodeToModify)
        {
            Inline referenceInline = referenceNode as Inline;
            Inline modifiedInline = nodeToModify as Inline;

            if (referenceInline == null || modifiedInline == null)
                return false;

            // Derived classes process nodes differently for different field types.
            AcceptRevisions(modifiedInline);
            ModifyInlineNode(referenceInline, modifiedInline);

            return true;
        }

        private static void AcceptRevisions(Inline node)
        {
            node.RunPr.AcceptFormatRevision();
            node.RunPr.Remove(RevisionAttr.InsertRevision);
        }

        /// <summary>
        /// Removes extra direct formatting attributes that should not be placed in the INDEX, TOC or TOA.
        /// </summary>
        /// <param name="sourceInline">The node on which duplicated attributes are detected.</param>
        /// <param name="modifiedInline">The node from which the detected attributes are stripped. Can be other than sourceInline.</param>
        protected void StripExtraDirectAttributes(Inline sourceInline, Inline modifiedInline)
        {
            int[] extraDirectAttributes = ListExtraDirectAttributes(sourceInline);
            RemoveListOfAttrs(modifiedInline.RunPr, extraDirectAttributes);
        }

        /// <summary>
        /// Builds a collection of direct formatting attributes
        /// that must be removed when copying the node to the INDEX, TOC or TOA.
        /// </summary>
        /// <param name="inline"></param>
        protected int[] ListExtraDirectAttributes(Inline inline)
        {
            RunPr runPr = inline.RunPr;

            // Something like List would be cleaner, but I don't like boxing/un-boxing in this case.
            // It's a pity we can't use generics.
            int[] directAttributes = new int[runPr.Count];

            for (int attrIdx = 0; attrIdx < runPr.Count; ++attrIdx)
            {
                int directAttrKey = runPr.GetKey(attrIdx);
                object directAttrValue = runPr.GetDirectAttr(directAttrKey);

                directAttributes[attrIdx] = IsExtraDirectAttribute(directAttrKey, directAttrValue, inline)
                    ? directAttrKey
                    // We could place all attributes to delete in the beginning of the array without gaps.
                    // But I don't think it is worth complicating the logic.
                    : KeepThisAttribute;
            }

            return directAttributes;
        }

        /// <summary>
        /// Implements the logic of deciding if the given attribute should be stripped when copying to the INDEX, TOC or TOA.
        /// </summary>
        /// <param name="directAttrKey"></param>
        /// <param name="directAttrValue"></param>
        /// <param name="sourceInline"></param>
        /// <returns></returns>
        protected virtual bool IsExtraDirectAttribute(int directAttrKey, object directAttrValue, Inline sourceInline)
        {
            return IsStrippedUncoditionally(directAttrKey)
                || DirectAttrDuplicatesInheritedValue(directAttrKey, directAttrValue, sourceInline)
                || IsExtraToggleAttribute(directAttrKey, directAttrValue, sourceInline);
        }

        protected static bool IsMinorHAnsiThemeFontAttr(int attrKey, object attrValue)
        {
            switch (attrKey)
            {
                case FontAttr.NameAscii:
                case FontAttr.NameOther:
                    return IsMinorHAnsiThemeFontAttr(attrValue);
                case FontAttr.NameBi:
                case FontAttr.NameFarEast:
                    // MS Word doesn't strip `cstheme` and `eastAsiaTheme` attrs. Let's declare it explicitly.
                    return false;
                default:
                    return false;
            }
        }

        private static bool IsMinorHAnsiThemeFontAttr(object attrValue)
        {
            ComplexFontName fontName = attrValue as ComplexFontName;
            if (fontName == null)
                return false;

            if (!fontName.IsThemeFont)
                return false;

            return fontName.ThemeFontCore == ThemeFontCore.MinorHAnsi;
        }

        /// <summary>
        /// Checks if a certain attribute type should go to INDEX, TOC or TOA.
        /// </summary>
        /// <param name="attrKey"></param>
        /// <returns></returns>
        protected static bool IsStrippedUncoditionally(int attrKey)
        {
            // It seems like font size specified directly is always ignored when building an INDEX, TOC or TOA.
            return (attrKey == FontAttr.Size) || (attrKey == FontAttr.SizeBi)
                // WORDSNET-25674 TOC items inherit style formatting after update fields.
                || (attrKey == FontAttr.Istd);
        }

        /// <summary>
        /// Checks if the given attribute has the same value as inherited from the styles or defaults.
        /// </summary>
        /// <remarks>
        /// Normally, such attributes should not be present in a document.
        /// They just duplicate the value obtained from the styles or defaults.
        /// However, when the node is copied to the INDEX, TOC or TOA, styles change.
        /// So in a new context such attribute may result in a different formatting.
        /// </remarks>
        protected static bool DirectAttrDuplicatesInheritedValue(int directAttrKey, object directAttribute, Inline sourceInline)
        {
            object inheritedAttribute = InlineHelper.FetchInheritedAttr(sourceInline, directAttrKey);
            return EnumUtilPal.EnumOrIntEquals(directAttribute, inheritedAttribute);
        }

        /// <summary>
        /// Checks if a toggle attribute should go to INDEX, TOC or TOA.
        /// </summary>
        /// <param name="directAttrKey"></param>
        /// <param name="directAttrValue"></param>
        /// <param name="sourceInline"></param>
        /// <returns></returns>
        protected bool IsExtraToggleAttribute(int directAttrKey, object directAttrValue, Inline sourceInline)
        {
            // Check toggle attributes only.
            if (!(directAttrValue is AttrBoolEx))
                return false;

            // DM: Oddly enough, Word seems to clear a direct toggle attribute when copying to TOC...
            // ...if the same attribute is set via character style.
            // It looks like this happens regardless of the actual resolved attribute value for the source or destination run.
            // I cannot explain it logically. Just trying to reproduce it.
            AttrBoolEx resolvedCharStyleAttr = (AttrBoolEx)sourceInline.CharacterStyle.GetFontAttr(directAttrKey, false);
            bool isToggledOnViaCharStyle = (resolvedCharStyleAttr == AttrBoolEx.True);

            return isToggledOnViaCharStyle
                || ResolvedAttrValueMatchesParagraphStyle(directAttrKey, sourceInline);

        }

        /// <summary>
        /// Checks if the resolved attribute of the source node has the same value
        /// that is derived from the INDEX, TOC or TOA paragraph style.
        /// </summary>
        /// <param name="attrKey"></param>
        /// <param name="sourceInline"></param>
        /// <returns></returns>
        protected bool ResolvedAttrValueMatchesParagraphStyle(int attrKey, Inline sourceInline)
        {
            if (EntryStyle == null)
                return false;

            // That will be the attribute value in the INDEX, TOC or TOA in the absence of direct formatting.
            object resolvedParagraphStyleAttr = EntryStyle.GetFontAttr(attrKey, true);

            // This is the resolved attribute value of the source node.
            object resolvedSourceAttr = sourceInline.Font.FetchAttr(attrKey);

            // Do not add direct formatting in INDEX, TOC or TOA if the resolved attributes match without it.
            return resolvedSourceAttr.Equals(resolvedParagraphStyleAttr);
        }

        /// <summary>
        /// Removes a list of attributes from a collection.
        /// </summary>
        /// <param name="attrCollection"></param>
        /// <param name="attrsToRemove"></param>
        protected static void RemoveListOfAttrs(AttrCollection attrCollection, int[] attrsToRemove)
        {
            // Remove duplicated attributes.
            foreach (int attr in attrsToRemove)
            {
                if (attr != KeepThisAttribute)
                    attrCollection.Remove(attr);
            }
        }

        /// <summary>
        /// A placeholder for the attributes that should not be removed from the modified node.
        /// </summary>
        private const int KeepThisAttribute = -1;

        protected Style EntryStyle { get; }
    }
}
