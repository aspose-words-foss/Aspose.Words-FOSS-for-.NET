// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2011 by Dmitry Matveenko

using Aspose.Words.Drawing;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Modifies TOC entry font attributes for a hyperlink TOC entry (TOC with \\h switch).
    /// </summary>
    internal class TocHyperlinkEntryAttributeModifier : IndexAndTablesEntryAttributeModifier, INodeModifier
    {
        internal TocHyperlinkEntryAttributeModifier(StyleIdentifier tocStyleId, StyleCollection styles)
            : base(tocStyleId, styles)
        {
        }

        Node INodeModifier.Modify(Node referenceNode, Node nodeToModify, bool modifyChildren, INodeCloningListener cloningListener)
        {
            Node modifiedNode = Modify(referenceNode, nodeToModify);

            if (!SetHyperlinkStyle(modifiedNode))
            {
                foreach (Node node in new NodeRange(modifiedNode, modifiedNode))
                    SetHyperlinkStyle(node);
            }

            return modifiedNode;
        }

        private static bool SetHyperlinkStyle(Node node)
        {
            Font font = ExtractFont(node);
            if (font == null)
                return false;

            // It's interesting that font attributes defined by Hyperlink style are ignored by MS Word in TOC.
            // "Reveal formatting" shows these attributes, but the text does not get them.
            // This Word behavior breaks if character style other than Hyperlink is used, even if it is just cloned from Hyperlink.
            font.StyleIdentifier = StyleIdentifier.Hyperlink;
            return true;
        }

        private static Font ExtractFont(Node node)
        {
            Inline inline = node as Inline;
            if (inline != null)
                return inline.Font;

            ShapeBase shape = node as ShapeBase;
            if (shape != null && shape.IsTopLevel && shape.FallbackShape != null)
                return shape.Font;

            return null;
        }

        /// <summary>
        /// Implements Toc entry node font attributes modification for a TOC with \\h switch.
        /// </summary>
        protected override void ModifyInlineNode(Inline sourceInline, Inline modifiedInline)
        {
            StripExtraDirectAttributes(sourceInline, modifiedInline);
        }

        /// <summary>
        /// Checks if a direct font attributes should be stripped when copying node to the TOC with \\h switch.
        /// Adds additional checks to the base implementation.
        /// </summary>
        /// <param name="directAttrKey"></param>
        /// <param name="directAttrValue"></param>
        /// <param name="sourceInline"></param>
        /// <returns></returns>
        protected override bool IsExtraDirectAttribute(int directAttrKey, object directAttrValue, Inline sourceInline)
        {
            return IsSpecificHyperlinkAttribute(directAttrKey)
                || base.IsExtraDirectAttribute(directAttrKey, directAttrValue, sourceInline)
                || IsDefinedViaStyle(directAttrKey, sourceInline)
                || IsMinorHAnsiThemeFontAttr(directAttrKey, directAttrValue);
        }

        /// <summary>
        /// Checks if the given attribute type should go to the TOC with \\h switch.
        /// </summary>
        /// <param name="attrKey"></param>
        /// <returns></returns>
        private static bool IsSpecificHyperlinkAttribute(int attrKey)
        {
            // Normally, color and underline are defined by Hyperlink style.
            // Word strips them regardless of the value.
            // And it even does not matter if they are really defined in Hyperlink style.
            switch (attrKey)
            {
                case FontAttr.Color:
                case FontAttr.ThemeColor:
                case FontAttr.ThemeShade:
                case FontAttr.ThemeTint:
                case FontAttr.Underline:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if the given is actually defined by TOC or Hyperlink style.
        /// </summary>
        /// <param name="attrKey"></param>
        /// <param name="sourceInline"></param>
        /// <returns></returns>
        private bool IsDefinedViaStyle(int attrKey, Inline sourceInline)
        {
            Style hyperlinkStyle = GetStyle(StyleIdentifier.Hyperlink);

            return StyleAttrValueDiffersFromDefault(hyperlinkStyle, attrKey, sourceInline)
                || StyleAttrValueDiffersFromDefault(EntryStyle, attrKey, sourceInline);
        }

        /// <summary>
        /// Checks if an attribute is defined by style and the definition differs from default and inline.
        /// </summary>
        /// <remarks>
        /// However toggles are considered defined even if they resolve to the default definition.
        /// </remarks>
        /// <param name="style"></param>
        /// <param name="attrKey"></param>
        /// <param name="sourceInline"></param>
        /// <returns></returns>
        private bool StyleAttrValueDiffersFromDefault(Style style, int attrKey, Inline sourceInline)
        {
            if (style == null)
                return false;

            object attrValue = style.GetFontAttr(attrKey, false);
            if (attrValue == null)
            {
                // It's not defined. Hence, does not differ from default.
                return false;
            }
            else if (attrValue is AttrBoolEx)
            {
                // Always clear direct toggle attributes if they are defined in the TOC or HyperLink style.
                return true;
            }
            else
            {
                // Otherwise, compare with the default value and inline value.
                object defaultAttrValue = GetAttributeFromDefaultStyle(attrKey);
                if (attrValue.Equals(defaultAttrValue))
                {
                    // WORDSNET-10392 Clear direct attribute only if inline attribute equals to style attribute.
                    object inlineAttrValue = sourceInline.RunPr.FetchAttr(attrKey);
                    return attrValue.Equals(inlineAttrValue);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets an attribute value from the default style.
        /// </summary>
        /// <param name="attrKey"></param>
        /// <returns></returns>
        private object GetAttributeFromDefaultStyle(int attrKey)
        {
            Style defaultStyle = GetStyle(StyleIdentifier.DefaultParagraphFont);

            return (defaultStyle != null)
                ? defaultStyle.GetFontAttr(attrKey, HasDefaultAttributeValue(attrKey))
                : null;
        }

        private static bool HasDefaultAttributeValue(int attrKey)
        {
            switch (attrKey)
            {
                // Revision keys
                case RevisionAttr.FormatRevision:
                case RevisionAttr.InsertRevision:
                case RevisionAttr.DeleteRevision:
                case RevisionAttr.MoveFromRevision:
                case RevisionAttr.MoveToRevision:

                // Internal keys
                case FontAttr.Sys_Symbol:
                    return false;

                default:
                    return true;
            }
        }

        /// <summary>
        /// Gets a style via Styles collection of TocEntryStyle.
        /// Returns null if TocEntryStyle is null;
        /// </summary>
        /// <param name="styleIdentifier"></param>
        /// <returns></returns>
        private Style GetStyle(StyleIdentifier styleIdentifier)
        {
            return EntryStyle != null
                ? EntryStyle.Styles.GetBySti(styleIdentifier, true)
                : null;
        }

    }
}
