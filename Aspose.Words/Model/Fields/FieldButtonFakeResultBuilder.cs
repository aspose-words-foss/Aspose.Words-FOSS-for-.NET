// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2022 by Edward Voronov

using Aspose.Words.Drawing;
using Aspose.Words.Tables;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Generate <see cref="FieldMacroButton"/> and <see cref="FieldGoToButton"/> field's fake results.
    /// </summary>
    internal class FieldButtonFakeResultBuilder : INodeModifier
    {
        private FieldButtonFakeResultBuilder(Paragraph fieldParagraph)
        {
            mDocument = fieldParagraph.Document;
            Section section = new Section(mDocument);
            mBody = section.AppendChild(new Body(mDocument));
            Paragraph paragraph = (Paragraph)mBody.AppendChild(fieldParagraph.Clone(false));
            mRun = paragraph.AppendChild(new Run(mDocument));
        }

        internal static NodeRange BuildFieldButtonFakeResult(FieldArgument displayTextArgument)
        {
            if (displayTextArgument == null)
                return null;

            FieldButtonFakeResultBuilder builder = new FieldButtonFakeResultBuilder(displayTextArgument.Field.Start.ParentParagraph);
            builder.CopyArgumentRange(displayTextArgument.Range);
            builder.FinalizeResult();
            return builder.GetResult();
        }

        private void CopyArgumentRange(NodeRange range)
        {
            NodeCopier.Copy(range, mRun, this);
            mRun.Remove();
        }

        private void FinalizeResult()
        {
            if (mIsError)
                return;

            foreach (Node node in mBody.GetChildNodes(NodeType.Any, false))
            {
                if (MergeNode(node))
                    node.Remove();
            }

            Debug.Assert(mBody.Paragraphs.Count == 1);
        }

        private bool MergeNode(Node node)
        {
            if (node == mBody.FirstParagraph)
                return false;

            switch (node.NodeType)
            {
                case NodeType.Paragraph:
                    return MergeParagraph((Paragraph)node);
                case NodeType.Table:
                    return MergeTable((Table)node);
                default:
                    return false;
            }
        }

        private bool MergeParagraph(Paragraph source)
        {
            mBody.FirstParagraph.InsertBefore(source.FirstChild, null, null);
            return true;
        }

        private bool MergeTable(Table source)
        {
            foreach (Row row in source.Rows)
            {
                foreach (Cell cell in row.Cells)
                {
                    foreach (Node node in cell.GetChildNodes(NodeType.Any, false))
                        MergeNode(node);

                    mBody.FirstParagraph.AppendChild(new Run(mDocument, ControlChar.Cell));
                }

                if (!row.IsLastRow)
                    mBody.FirstParagraph.AppendChild(new Run(mDocument, ControlChar.Cell));
            }

            return true;
        }

        private NodeRange GetResult()
        {
            if (mIsError)
            {
                Run run = new Run(mDocument, "DisplayText cannot span more than one line!");
                run.Font.Bold = true;
                return new NodeRange(run, run);
            }

            Paragraph paragraph = mBody.FirstParagraph;
            return FieldUtil.BuildFieldResultNodeRange(paragraph, paragraph);
        }

        Node INodeModifier.Modify(Node referenceNode, Node nodeToModify, bool modifyChildren, INodeCloningListener cloningListener)
        {
            return ModifyNode(nodeToModify, modifyChildren);
        }

        private bool IsValidNode(Node node)
        {
            if (mIsError)
                return false;

            Run run = node as Run;
            if (run == null)
                return true;

            if (run.Text != ControlChar.PageBreak)
                return true;

            mIsError = true;
            return false;
        }

        private Node ModifyNode(Node node, bool modifyChildren)
        {
            if (!IsValidNode(node))
                return null;

            switch (node.NodeType)
            {
                case NodeType.Run:
                    return ModifyRun((Run)node);
                case NodeType.Shape:
                    return ModifyShape((Shape)node);
                default:
                   break;
            }

            if (modifyChildren && node.IsComposite)
            {
                CompositeNode compositeNode = (CompositeNode)node;
                foreach (Node child in compositeNode.GetChildNodes(NodeType.Any, false))
                {
                    if (ModifyNode(child, true) == null)
                        child.Remove();
                }
            }

            return node;
        }

        private static Node ModifyRun(Run run)
        {
            run.Text = run.Text.Replace(ControlChar.LineBreak, string.Empty);
            return run;
        }

        private static Shape ModifyShape(Shape shape)
        {
            if (shape.WrapType != WrapType.Inline)
                return null;

            shape.Stroke.Visible = false;
            shape.Fill.Visible = false;

            shape.GetChildNodes(NodeType.Any, false).Clear();
            return shape;
        }

        private readonly DocumentBase mDocument;
        private readonly Body mBody;
        private readonly Run mRun;

        private bool mIsError;
    }
}
