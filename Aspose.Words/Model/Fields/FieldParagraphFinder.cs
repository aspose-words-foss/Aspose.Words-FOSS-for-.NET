// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/12/2013 by Ivan Lyagin

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// A base class used to find a paragraph required to update a particular field.
    /// </summary>
    internal abstract class FieldParagraphFinder
    {
        /// <summary>
        /// Seeks for the first paragraph which is confirmed by <see cref="ConfirmParagraphNode(Aspose.Words.Node)"/>
        /// for a field located in header/footer story.
        /// </summary>
        protected Paragraph FindParagraphHeaderFooter(Field field)
        {
            // Ensure that the field's update is initiated by layout. Otherwise, we could not use the display context.
            Debug.Assert(field.Updater.IsUpdateInitiatedByLayout);

            // 1. Scan pages backward from the field's page to the first page.
            Paragraph paragraph = field.Updater.DisplayContext.FindParagraph(field.Start, this);

            if (paragraph == null)
            {
                OnAllPagesScanned();

                // 2. Nothing has been found before the field, so we need to search for a paragraph after it.
                //    Scan the document forward starting from the field's section not to bother layout once again.
                Node parentSection = field.Start.GetAncestor(NodeType.Section);
                paragraph = FindParagraph(parentSection, true, true);
            }

            return paragraph;
        }

        /// <summary>
        /// Notifies the finder that a whole page has been scanned.
        /// </summary>
        internal void NotifyPageScanned()
        {
            OnPageScanned();
        }

        /// <summary>
        /// Called when a whole page has been scanned.
        /// </summary>
        protected virtual void OnPageScanned()
        {
            // Do nothing.
        }

        /// <summary>
        /// Called when all of the interesting pages have been scanned.
        /// </summary>
        protected virtual void OnAllPagesScanned()
        {
            // Do nothing.
        }

        /// <summary>
        /// Seeks for the first paragraph which is confirmed by <see cref="ConfirmParagraphNode(Aspose.Words.Node)"/>
        /// starting from the specified node in forward or backward direction. Optionally includes the start node
        /// and its children (if any) to the search.
        /// </summary>
        protected Paragraph FindParagraph(Node startNode, bool isForward, bool includeStart)
        {
            DocumentPosition position = (isForward ^ includeStart)
                ? DocumentPosition.CreatePositionAfter(startNode)
                : DocumentPosition.CreatePositionBefore(startNode);

            // We should skip the first move if the start node should be included to the search.
            bool skipMove = includeStart;

            while (skipMove || position.Move(null, isForward, true, true, false, true))
            {
                Paragraph paragraph = ConfirmParagraphNode(position.Node);
                if (paragraph != null)
                    return paragraph;

                skipMove = false;
            }

            return null;
        }

        /// <summary>
        /// Checks whether the specified candidate node is a paragraph satisfying some conditions defined by
        /// the derived class and if it is, returns it as a paragraph, otherwise returns <c>null</c>.
        /// </summary>
        internal Paragraph ConfirmParagraphNode(Node candidateNode)
        {
            if ((candidateNode == null) || (candidateNode.NodeType != NodeType.Paragraph))
                return null;

            return ConfirmParagraph((Paragraph)candidateNode);
        }

        /// <summary>
        /// When implemented in a derived class, checks whether the specified candidate paragraph satisfies some
        /// conditions defined by the derived class and if it is, returns it, otherwise returns <c>null</c>.
        /// </summary>
        [JavaThrows(true)]
        internal abstract Paragraph ConfirmParagraph(Paragraph candidateParagraph);

        /// <summary>
        /// When implemented in a derived class, gets a value indicating whether a page should be scanned in
        /// forward direction while searching for a paragraph.
        /// </summary>
        internal abstract bool IsForwardPageScan { get; }

        internal virtual bool NeedCachePageScanResult(Paragraph paragraph)
        {
            return false;
        }
    }
}
