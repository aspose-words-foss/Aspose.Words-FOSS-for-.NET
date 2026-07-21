// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/02/2013 by Alexey Morozov

using Aspose.Collections.Generic;
using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// Implements collection for HtmlBlock items.
    /// </summary>
    /// <remarks>
    /// AM. This collection MUST be written sorted by HtmlBlock.Id for DOC and RTF formats. 
    /// That why it's a sorted list.
    /// </remarks>
    internal class HtmlBlockCollection
    {
        internal void Add(HtmlBlock htmlBlock)
        {
            // WORDSNET-11139 Ignore items with duplicate ids.
            if (mBlocks[htmlBlock.Id] != null)
                return;

            mBlocks.Add(htmlBlock.Id, htmlBlock);
            htmlBlock.HtmlBlockCollection = this;
        }

        internal void Add(HtmlBlockCollection blocks)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                HtmlBlock block = blocks.GetHtmlBlockByIndex(i);
                Add(block);
            }
        }

        internal int IndexOfKey(int id)
        {
            return mBlocks.IndexOfKey(id);
        }

        /// <summary>
        /// Expands HTML related formatting specified in the source paragraph properties into the destination paragraph properties.
        /// </summary>
        internal void Expand(ParaPr srcParaPr, ParaPr dstParaPr, bool expandParent)
        {
            object htmlBlockId = srcParaPr.GetDirectAttr(ParaAttr.HtmlBlockId);

            if (htmlBlockId != null)
                GetExpandedParaPr((int)htmlBlockId, expandParent).ExpandTo(dstParaPr);
        }

        /// <summary>
        /// Expands HTML related formatting specified in the source table row properties into the destination row properties.
        /// </summary>
        internal void Expand(TablePr dstTablePr, int htmlBlockId, bool expandParent)
        {
            ParaPr dstParaPr = new ParaPr();
            GetExpandedParaPr(htmlBlockId, expandParent).ExpandTo(dstParaPr);

            // Take in attention only margin attributes for a while.
            int startIndex = ParaAttr.HtmlMarginLeft;
            for (int i = startIndex; i <= ParaAttr.HtmlMarginBottom; ++i)
            {
                if (dstParaPr[i] != null)
                    dstTablePr[i - startIndex + TableAttr.HtmlMarginLeft] = dstParaPr[i];
            }
        }

        /// <summary>
        /// Removes HTML related formatting specified in the table row properties.
        /// </summary>
        internal void RemoveFormatting(TablePr tablePr)
        {
            // Take in attention only margin attributes for a while.
            for (int i = TableAttr.HtmlMarginBottom; i >= TableAttr.HtmlMarginLeft; --i)
            {
                if (tablePr.Contains(i))
                    tablePr.Remove(i);
            }
        }

        /// <summary>
        /// Creates a deep copy of this collection.
        /// </summary>
        internal HtmlBlockCollection Clone()
        {
            HtmlBlockCollection lhs = new HtmlBlockCollection();

            for (int i = 0; i < mBlocks.Count; i++)
                lhs.Add(this.GetHtmlBlockByIndex(i).Clone());

            return lhs;
        }

        internal HtmlBlock GetHtmlBlockById(int id)
        {
            return mBlocks[id];
        }

        internal HtmlBlock GetHtmlBlockByIndex(int index)
        {
            return mBlocks.GetByIndex(index);
        }

        private ParaPr GetExpandedParaPr(int htmlBlockId, bool expandParent)
        {
            HtmlBlock htmlBlock = GetHtmlBlockById(htmlBlockId);

            if(htmlBlock == null)
                return new ParaPr();

            if (expandParent)
            {
                ParaPr expParaPr = GetExpandedParaPr(htmlBlock.ParentId, true);

                // Expand formatting of this block.
                htmlBlock.ParaPr.ExpandTo(expParaPr);
                return expParaPr;
            }
            else
            {
                return htmlBlock.ParaPr;
            }
        }

        internal int Count
        {
            get { return mBlocks.Count; }
        }

        private readonly SortedIntegerListGeneric<HtmlBlock> mBlocks = new SortedIntegerListGeneric<HtmlBlock>();
    }
}
