// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2009 by Dmitry Vorobyev

using System;
using Aspose.Fonts;

namespace Aspose.Words.Fields
{
    internal class FieldTokenDecoderNodeModifier : FieldTokenDecoder, INodeModifier
    {
        internal FieldTokenDecoderNodeModifier(NodeRange tokenRange)
            : this(tokenRange, true)
        {
        }

        internal FieldTokenDecoderNodeModifier(NodeRange tokenRange, bool trimDoubleQuotes)
            : this(tokenRange, FieldTokenDecoderOptionsUtil.WithTrimDoubleQuotes(FieldTokenDecoderOptions.EscapeChars, trimDoubleQuotes))
        {
        }

        internal FieldTokenDecoderNodeModifier(NodeRange tokenRange, FieldTokenDecoderOptions options)
            : base(options)
        {
            mFieldCharCounter = new FieldCharCounter(FieldUtil.EndsWithFieldEnd(tokenRange));
        }

        Node INodeModifier.Modify(Node referenceNode, Node nodeToModify, bool modifyChildren, INodeCloningListener cloningListener)
        {
            ProcessNode(nodeToModify);

            // Reject the node if needed, but process it in a common way, as it may influence the internal state of the decoder.
            bool needRejectNode = !PrepareModify(referenceNode, nodeToModify);

            switch (TryDecodeRunText(nodeToModify, false))
            {
                case DecodeRunTextResult.Decoded:
                {
                    // Do nothing.
                    break;
                }
                case DecodeRunTextResult.EmptyText:
                {
                    needRejectNode = true;
                    break;
                }
                case DecodeRunTextResult.NotRun:
                {
                    // WORDSNET-20760 Previously modifyChildren only affected field char calculation and didn't actually modify child nodes.
                    if (nodeToModify.IsComposite && modifyChildren)
                        ModifyChildNodes(nodeToModify, false);

                    // Reset escaping if present.
                    IsTokenCharEscaped = false;
                    OnContentProcessed(nodeToModify);

                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            return (needRejectNode ? null : nodeToModify);
        }

        private void ModifyChildNodes(Node nodeToModify, bool processNode)
        {
            if (processNode)
                ProcessNode(nodeToModify);

            if (nodeToModify.IsComposite)
            {
                for (Node childNode = ((CompositeNode)nodeToModify).FirstChild; childNode != null; childNode = childNode.NextSibling)
                    ModifyChildNodes(childNode, true);
            }
            else
            {
                // We don't really care about decode result here.
                TryDecodeRunText(nodeToModify, true);
            }
        }

        private void ProcessNode(Node node)
        {
            mFieldCharCounter.VisitNode(node);
            TryProcessCell(node);
        }

        private DecodeRunTextResult TryDecodeRunText(Node node, bool setIfEmpty)
        {
            if (node.NodeType != NodeType.Run)
                return DecodeRunTextResult.NotRun;

            Run run = (Run)node;

            // It looks like decode logic is similar when located in a field or in a table cell.
            string decodedText = DecodeTokenPart(
                run.Text,
                mFieldCharCounter.IsInField || IsInCell,
                FontUtil.IsSymbolic(run.Font.Name));
            bool isEmpty = string.IsNullOrEmpty(decodedText);

            if (!isEmpty || setIfEmpty)
                run.Text = decodedText;

            return isEmpty ? DecodeRunTextResult.EmptyText : DecodeRunTextResult.Decoded;
        }

        private void OnContentProcessed(Node node)
        {
            // This check is redundant, but it allows to avoid cast below in most cases.
            if (HasProcessedContent)
                return;

            if (node is FieldChar)
                return;

            OnContentProcessed();
        }

        private void TryProcessCell(Node node)
        {
            // No the best way of detecting if we are still in the same cell. However we don't have ability to track the end of the container
            // like when we use a visitor.
            if ((mLastCell != null) && !node.IsAncestorNode(mLastCell))
                mLastCell = null;

            if (node.NodeType == NodeType.Cell)
                mLastCell = node;
        }

        protected override bool IsHasProcessedContentLocked
        {
            get { return IsInFieldCode; }
        }

        /// <summary>
        /// Performs initialization of a node modifying process.
        /// </summary>
        /// <remarks>
        /// An implementation of this method in a derived class should return false to reject the specified node.
        /// </remarks>
        protected virtual bool PrepareModify(Node referenceNode, Node nodeToModify)
        {
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the current processed character is inside a field code.
        /// </summary>
        protected bool IsInFieldCode
        {
            get { return mFieldCharCounter.IsInFieldCode; }
        }

        /// <summary>
        /// Gets a value indicating if the current position is inside a table cell.
        /// </summary>
        private bool IsInCell
        {
            get { return (mLastCell != null); }
        }

        private readonly FieldCharCounter mFieldCharCounter;
        private Node mLastCell;

        private enum DecodeRunTextResult
        {
            Decoded,
            EmptyText,
            NotRun
        }
    }
}
