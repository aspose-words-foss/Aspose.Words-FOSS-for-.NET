// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/05/2023 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Modifies HYPERLINK field results inside IF field result.
    /// </summary>
    internal class FieldIfHyperlinkFormattingModifier : INodeModifier
    {
        internal FieldIfHyperlinkFormattingModifier(Document document)
        {
            mDocument = document;
        }

        Node INodeModifier.Modify(Node referenceNode, Node nodeToModify, bool modifyChildren, INodeCloningListener cloningListener)
        {
            Modify(nodeToModify, modifyChildren);
            return nodeToModify;
        }

        private void Modify(Node node, bool modifyChildren)
        {
            mFieldCharCounter.VisitNode(node);

            ModifyHyperlinkFieldResult(node);

            if (!modifyChildren || !node.IsComposite)
                return;

            foreach (Node child in ((CompositeNode)node))
                Modify(child, true);
        }

        private void ModifyHyperlinkFieldResult(Node node)
        {
            Inline inline = node as Inline;
            if (inline == null)
                return;

            if (!IsHyperlinkFieldResult())
                return;

            RemoveHyperlinkSpecificAttributes(inline.RunPr);
            AddNotHyperlinkSpecificAttributes(inline.RunPr);

            inline.RunPr.Remove(FontAttr.Istd);
        }

        private bool IsHyperlinkFieldResult()
        {
            return mFieldCharCounter.IsInFieldResult &&
                   mFieldCharCounter.CurrentField.FieldType == FieldType.FieldHyperlink;
        }

        private static void RemoveHyperlinkSpecificAttributes(RunPr runPr)
        {
            foreach (int key in runPr.GetKeys())
            {
                if (IsSpecificHyperlinkAttribute(key))
                    runPr.Remove(key);
            }
        }

        private void AddNotHyperlinkSpecificAttributes(RunPr runPr)
        {
            foreach (int key in NotHyperlinkSpecificAttributes.GetKeys())
            {
                if (runPr.ContainsKey(key))
                {
                    runPr.Remove(key);
                }
                else
                {
                    runPr.Add(key, NotHyperlinkSpecificAttributes[key]);
                }
            }
        }

        private RunPr NotHyperlinkSpecificAttributes
        {
            get
            {
                if (mNotHyperlinkSpecificAttributes == null)
                {
                    Style hyperlinkStyle = mDocument.Styles.GetBySti(StyleIdentifier.Hyperlink, false);
                    if (hyperlinkStyle != null)
                    {
                        mNotHyperlinkSpecificAttributes = hyperlinkStyle.GetExpandedRunPr(RunPrExpandFlags.Normal);
                        foreach (int key in mNotHyperlinkSpecificAttributes.GetKeys())
                        {
                            if (IsSpecificHyperlinkAttribute(key))
                                mNotHyperlinkSpecificAttributes.Remove(key);
                        }
                    }
                    else
                    {
                        mNotHyperlinkSpecificAttributes = new RunPr();
                    }
                }

                return mNotHyperlinkSpecificAttributes;
            }
        }

        private static bool IsSpecificHyperlinkAttribute(int attrKey)
        {
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

        private readonly FieldCharCounter mFieldCharCounter = new FieldCharCounter();
        private readonly Document mDocument;
        private RunPr mNotHyperlinkSpecificAttributes;
    }
}
