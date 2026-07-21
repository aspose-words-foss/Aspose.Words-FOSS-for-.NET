// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/09/2014 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Stores element background information.
    /// This information allows specifying background colors taken from html element for their counterpart of the AW model.
    /// </summary>
    /// /// <remarks>
    /// This class is immutable (read-only).
    /// </remarks>
    internal class ElementBackgroundInfo
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="blockBackgroundDeclaration"><see cref="CssDeclaration"/> for block element.</param>
        /// <param name="inlineBackgroundDeclaration"><see cref="CssDeclaration"/> for inline element.</param>
        /// <param name="isTopLevelContainer"><c>true</c> if it's top level element as a 'html' or 'body'.</param>
        /// <param name="parent">Parent background.</param>
        /// <param name="isContainer">True if the element may contain another element.</param>
        internal ElementBackgroundInfo(
            CssComputedDeclaration blockBackgroundDeclaration,
            CssComputedDeclaration inlineBackgroundDeclaration,
            bool isTopLevelContainer,
            ElementBackgroundInfo parent,
            bool isContainer)
        {
            mBlockBackgroundDeclaration = blockBackgroundDeclaration;
            mIsTopLevelContainer = isTopLevelContainer;
            mParent = parent;
            mIsContainer = isContainer;
            mInlineBackgroundDeclaration = inlineBackgroundDeclaration;
        }

        internal CssDeclaration GetParagraphBackgroundColor()
        {
            return ((!IsBlockBackgroundEmpty) && (!IsBlockBackgroundSameAsContainer))
                ? BlockBackgroundDeclaration
                : null;
        }

        internal CssDeclaration GetFontBackgroundColor()
        {
            return ((!IsInlineBackgroundEmpty) && (!IsBlockBackgroundSameAsInlineBackground))
                ? InlineBackgroundDeclaration
                : null;
        }

        internal CssDeclaration GetTableBackgroundColor()
        {
            return ((!IsBlockBackgroundEmpty) && (!IsBlockBackgroundSameAsContainer))
                ? BlockBackgroundDeclaration
                : null;
        }

        private CssComputedDeclaration GetContainerBackgroundColor()
        {
            ElementBackgroundInfo item = mParent;
            while (item != null)
            {
                if (item.mIsContainer)
                    return item.BlockBackgroundDeclaration;
                item = item.mParent;
            }
            return null;
        }

        internal bool IsBlockBackgroundEmpty
        {
            get
            {
                return BlockBackgroundDeclaration == null;
            }
        }

        /// <summary>
        /// Determines if the block background for this item is the same as the container.
        /// </summary>
        internal bool IsBlockBackgroundSameAsContainer
        {
            get
            {
                CssComputedDeclaration containerColor = GetContainerBackgroundColor();
                return ((BlockBackgroundDeclaration == null) && (containerColor == null)) ||
                    ((BlockBackgroundDeclaration != null) && (containerColor != null) &&
                    BlockBackgroundDeclaration.Value.Equals(containerColor.Value));
            }
        }

        internal bool IsInlineBackgroundEmpty
        {
            get
            {
                return InlineBackgroundDeclaration == null;
            }
        }

        internal bool IsBlockBackgroundSameAsInlineBackground
        {
            get
            {
                return !IsInlineBackgroundEmpty &&
                   !IsBlockBackgroundEmpty &&
                   BlockBackgroundDeclaration.Value.Equals(InlineBackgroundDeclaration.Value);
            }
        }

        /// <summary>
        /// <see cref="CssDeclaration"/> of background declaration for block element
        /// </summary>
        internal CssComputedDeclaration BlockBackgroundDeclaration
        {
            get { return mBlockBackgroundDeclaration; }
        }
        /// <summary>
        /// <see cref="CssDeclaration"/> of background declaration for inline element
        /// </summary>
        internal CssComputedDeclaration InlineBackgroundDeclaration
        {
            get { return mInlineBackgroundDeclaration; }
        }
        /// <summary>
        /// Returns <c>true</c> if this top level element as a 'html' or 'body'
        /// </summary>
        internal bool IsTopLevelContainer
        {
            get { return mIsTopLevelContainer; }
        }

        /// <summary>
        /// Represents an empty top level container element.
        /// </summary>
        /// <remarks>
        /// Used to reduce memory allocations.
        /// </remarks>
        internal static ElementBackgroundInfo EmptyTopLevelContainer
        {
            get { return gEmptyTopLevelContainer; }
        }

        private readonly CssComputedDeclaration mBlockBackgroundDeclaration;
        private readonly CssComputedDeclaration mInlineBackgroundDeclaration;
        private readonly bool mIsTopLevelContainer;
        private readonly ElementBackgroundInfo mParent;
        private readonly bool mIsContainer;

        private static readonly ElementBackgroundInfo gEmptyTopLevelContainer = new ElementBackgroundInfo(null, null, true, null, true);
    }
}
