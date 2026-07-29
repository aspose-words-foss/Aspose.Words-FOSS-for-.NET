// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/09/2013 by Victor Chebotok

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// CSS box model. Translates positions of CSS boxes into indents and spacing of document text.
    /// </summary>
    /// <remarks>
    /// For details, see http://www.w3.org/TR/CSS2/box.html and http://www.w3.org/TR/CSS2/visuren.html.
    /// </remarks>
    internal class CssBoxModel
    {
        /// <summary>
        /// Creates and initializes a new instance of the class.
        /// </summary>
        /// <param name="documentMode">
        /// The mode of the HTML document being processed.
        /// </param>
        /// <param name="applyDefaultVerticalMargins">
        /// Instructs the box model to use vertical margins specified in the user agent style sheet for certain elements
        /// (paragraphs, lists, etc.)
        /// </param>
        /// <param name="applyFormattingAsMsWord">
        /// Indicates whether we should stick to MS Word's rules when applying CSS formatting to document model.
        /// </param>
        internal CssBoxModel(
            CssDocumentMode documentMode,
            bool applyDefaultVerticalMargins,
            bool applyFormattingAsMsWord)
        {
            mDocumentMode = documentMode;
            mApplyDefaultVerticalMargins = applyDefaultVerticalMargins;
            mApplyFormattingAsMsWord = applyFormattingAsMsWord;
            // An imaginary root box containing the whole document.
            CssBox rootBox = new CssBox();
            mBoxes.Push(rootBox);
            // Root formatting context.
            mContexts.Push(new CssBoxFormattingContext(rootBox, true));
        }

        /// <summary>
        /// Pushes a child HTML element into the model.
        /// </summary>
        /// <param name="element">An HTML element.</param>
        /// <param name="cssDeclarations">Element's CSS declarations.</param>
        /// <param name="displayType">Element display type based on CSS "display" property.</param>
        /// <param name="parentDisplayType">Parent element display type.</param>
        /// <param name="createsHtmlBlock">
        /// Indicates whether the HTML element creates a <see cref="HtmlBlock"/> instance in the document model.
        /// </param>
        internal void Push(IHtmlElementProvider element,
            CssDeclarationCollection cssDeclarations,
            CssDisplayType displayType,
            CssDisplayType parentDisplayType,
            bool createsHtmlBlock)
        {
            Debug.Assert(!element.IsImplicit);

            if (CurrentBox.StartsWithImplicitBox)
            {
                CurrentBox.ChildTopMarginCollapsing = CssBoxMarginCollapsing.Separate;
            }
            if (element.ElementName == "body")
            {
                CurrentBox.ChildTopMarginCollapsing = CssBoxMarginCollapsing.Collapse;
            }
            if (element.IsLastChildBox && (!CurrentBox.EndsWithImplicitBox))
            {
                CurrentBox.ChildBottomMarginCollapsing = CurrentBox.BottomMarginIsCollapsible
                    ? CssBoxMarginCollapsing.Collapse
                    : CssBoxMarginCollapsing.Add;
            }

            CssBox childBox;

            string name = element.ElementName;

            if ((name == "td") || (name == "th"))
            {
                // Table cells start AW model analogs of HTML boxes.
                childBox = ComputeTableCellBox(element);
            }
            else if (displayType == CssDisplayType.TableCell)
            {
                childBox = ComputeDisplayTableCellBox(element, cssDeclarations);
            }
            else if (name == "table")
            {
                // Tables in AW model are similar to HTML boxes.
                childBox = ComputeTableBox(element, cssDeclarations);
            }
            else if (name == "caption")
            {
                // Captions are imported as table cells, so they start boxes too.
                childBox = ComputeCaptionBox(element, cssDeclarations);
            }
            // WORDSNET-22044 A box of the list depends on display type.
            else if ((name == "ul") || (name == "ol"))
            {
                // For the following values of display type browser generates box without bottom margin
                // and without top margin for `inline` display type.
                bool isInlineBox =
                    displayType == CssDisplayType.Inline ||
                    displayType == CssDisplayType.TableHeaderGroup ||
                    displayType == CssDisplayType.TableCell ||
                    displayType == CssDisplayType.TableRow ||
                    displayType == CssDisplayType.TableRowGroup ||
                    displayType == CssDisplayType.TableFooterGroup;

                childBox = isInlineBox
                    ? ComputeInlineListBox(element, cssDeclarations, displayType)
                    : ComputeUsualBox(element, cssDeclarations);
            }
            else if (name == "li")
            {
                // WORDSNET-26553 If the list element is not the first then top margin is ignored.
                if (element.GetPreviousSiblingElement() != null)
                {
                    CurrentBox.ChildTopMarginCollapsing = CssBoxMarginCollapsing.Separate;
                }

                // WORDSNET-22044 A box of a list item depends on display type.
                childBox = IsBlock(displayType)
                    ? ComputeUsualBox(element, cssDeclarations)
                    : ComputeInlineBox(element);
            }
            // WORDSNET-23606 MS Word ignores the margins, the paddings and the border widths of `figure` elements
            // when loading HTML.
            else if ((name == "figure") && mApplyFormattingAsMsWord)
            {
                childBox = new CssBox();
            }
            else
            {
                // If the HTML element creates the HTML block then it shouldn't change the size of the box model.
                // All the margins of such HTML element should be stored in the corresponding HTML block.
                if (createsHtmlBlock)
                {
                    childBox = new CssBox();
                }
                else
                {
                    childBox = IsBlock(displayType)
                        ? ComputeUsualBox(element, cssDeclarations)
                        : ComputeInlineBox(element);
                }
            }

            CurrentContext.EmptyBlockMarginBefore = null;
            childBox.IsEmptyBlock = !element.IsElementContainsText && (displayType == CssDisplayType.Block);

            bool elementIsFloatingTable = IsFloatingTable(name, displayType, cssDeclarations);

            // Floating tables are excluded from the formatting context.
            // Their top margin never collapses with parent's top margin.
            if (element.IsElementContainsText && !elementIsFloatingTable)
            {
                // WORDSNET-22044 If the first list item is inline
                // and it is inside inline list then top margin is ignored.
                if ((element.GetPreviousSiblingElement() == null) &&
                    (element.ElementName == "li") &&
                    (displayType == CssDisplayType.Inline))
                {
                    if (parentDisplayType == CssDisplayType.Inline)
                        CurrentBox.ChildTopMarginCollapsing = CssBoxMarginCollapsing.Separate;
                }
                else
                {
                    CurrentBox.ChildTopMarginCollapsing = CssBoxMarginCollapsing.Separate;
                }
            }

            if (element.IsLastChildBox)
            {
                if ((element.ElementName == "li") && (parentDisplayType == CssDisplayType.Inline))
                {
                    CurrentBox.ChildTopMarginCollapsing = CssBoxMarginCollapsing.Separate;
                }
                else
                {
                    CurrentBox.ChildBottomMarginCollapsing = CssBoxMarginCollapsing.Collapse;
                }
            }

            // Floating tables start their own box formatting contexts.
            if (elementIsFloatingTable)
            {
                mContexts.Push(new CssBoxFormattingContext(childBox, IsFullWidthBox(cssDeclarations)));
            }

            if (element.ContainsChildBoxes)
            {
                childBox.ChildBottomMarginCollapsing = CssBoxMarginCollapsing.Separate;
            }

#if DEBUG
            childBox.DisplayType = displayType;
            childBox.Name = element.ElementName;
#endif
            mBoxes.Push(childBox);

            mRecalculationNeeded = true;
        }

        /// <summary>
        /// Pops the most inner child HTML element from the model.
        /// </summary>
        internal void Pop()
        {
            Debug.Assert(mBoxes.Count > 0);

            // The current formatting context closes when its root box is removed from the model.
            if (CurrentContext.RootBox == CurrentBox)
            {
                bool closedContextIsFullWidthBox = CurrentContext.IsFullWidthBox;
                mContexts.Pop();

                // There always exists a formatting context. At least the root context is never removed from the stack.
                Debug.Assert(mContexts.Count > 0);

                // WORDSNET-11202 A full-width floating box cannot have other boxes on its sides. A box after a full-width
                // floating box is pushed below the floating box, and the floating box occupies its top margin.
                // Here we do not know the exact height of the floating box but we assume it is big enough to occupy the whole
                // top margin of the next box and to effectively reduce its top margin value to zero.
                CurrentContext.CurrentBoxTopMarginManualCollapsing = closedContextIsFullWidthBox
                    ? CssBoxMarginManualCollapsing.Zero
                    : CssBoxMarginManualCollapsing.Collapse;
            }
            else
            {
                CurrentContext.CurrentBoxTopMarginManualCollapsing = CssBoxMarginManualCollapsing.Separate;
            }

            CurrentContext.PreviousBoxBottomMargin = CurrentContext.CurrentBoxBottomMargin;

            // WORDSNET-18824 Margins of an empty block are now collapsed with the top margin of the block right after
            // the empty one.
            if (CurrentBox.IsEmptyBlock)
            {
                // The calling code doesn't always recalculate margins of empty blocks, because they are normally ignored.
                // Let's make sure we know the margins of this empty block.
                RecalculateIfNeeded();

                CssLength emptyBlockMargin = new CssLength(
                    System.Math.Max(mTop.Value, mBottom.Value),
                    mTop.IsCustom || mBottom.IsCustom);

                // Margins of consequent empty blocks are collapsed too.
                if (CurrentContext.EmptyBlockMarginBefore == null)
                {
                    CurrentContext.EmptyBlockMarginBefore = emptyBlockMargin;
                }
                else
                {
                    CurrentContext.EmptyBlockMarginBefore = new CssLength(
                        System.Math.Max(CurrentContext.EmptyBlockMarginBefore.Value, emptyBlockMargin.Value),
                        CurrentContext.EmptyBlockMarginBefore.IsCustom || emptyBlockMargin.IsCustom);
                }
            }

            mBoxes.Pop();
            mRecalculationNeeded = true;
        }

        internal bool HasCustomParentLeft()
        {
            CssLength parentLeft = CalculateParentMarginValue(CssBoxMarginSide.Left, true);
            return parentLeft.IsCustom;
        }

        internal bool HasCustomParentRight()
        {
            CssLength parentRight = CalculateParentMarginValue(CssBoxMarginSide.Right, true);
            return parentRight.IsCustom;
        }

        /// <summary>
        /// Gets the current left indent value.
        /// </summary>
        internal CssLength Left
        {
            get
            {
                RecalculateIfNeeded();
                return mLeft;
            }
        }

        /// <summary>
        /// Gets the current right indent value.
        /// </summary>
        internal CssLength Right
        {
            get
            {
                RecalculateIfNeeded();
                return mRight;
            }
        }

        /// <summary>
        /// Gets the current top spacing value.
        /// </summary>
        internal CssLength Top
        {
            get
            {
                RecalculateIfNeeded();
                return mTop;
            }
        }

        /// <summary>
        /// Gets the current bottom spacing value.
        /// </summary>
        internal CssLength Bottom
        {
            get
            {
                RecalculateIfNeeded();
                return mBottom;
            }
        }

        /// <summary>
        /// Gets all current indent and spacing values.
        /// </summary>
        internal CssMargins Margins
        {
            get
            {
                CssMargins margins = new CssMargins(Top.Value, Right.Value, Bottom.Value, Left.Value);
                margins.UseTop = Top.IsCustom;
                margins.UseRight = Right.IsCustom;
                margins.UseBottom = Bottom.IsCustom;
                margins.UseLeft = Left.IsCustom;
                return margins;
            }
        }

        private static bool IsBlock(CssDisplayType displayType)
        {
            switch (displayType)
            {
                case CssDisplayType.Inline:
                case CssDisplayType.TableHeaderGroup:
                case CssDisplayType.TableRowGroup:
                case CssDisplayType.TableRow:
                case CssDisplayType.TableCell:
                case CssDisplayType.TableFooterGroup:
                    return false;
                case CssDisplayType.Block:
                case CssDisplayType.InlineBlock:
                case CssDisplayType.ListItem:
                case CssDisplayType.RunIn:
                case CssDisplayType.Table:
                case CssDisplayType.InlineTable:
                case CssDisplayType.TableCaption:
                case CssDisplayType.TableColumnGroup:
                case CssDisplayType.TableColumn:
                case CssDisplayType.None:
                    return true;
                default:
                    Debug.Assert(false, "Unknown display type.");
                    return false;
            }
        }

        private void RecalculateIfNeeded()
        {
            if (mRecalculationNeeded)
            {
                RecalculateLeft();
                RecalculateRight();
                RecalculateTop();
                RecalculateBottom();

                mRecalculationNeeded = false;
            }
        }

        private void RecalculateLeft()
        {
            mLeft = CalculateCurrentMarginValue(CssBoxMarginSide.Left, true);
        }

        private void RecalculateRight()
        {
            mRight = CalculateCurrentMarginValue(CssBoxMarginSide.Right, true);
        }

        private void RecalculateTop()
        {
            bool firstTextBoxOrElementItself = (!CurrentBox.CanContainImplicitTextBoxes) ||
                (CurrentBox.ChildTopMarginCollapsing != CssBoxMarginCollapsing.Separate);

            if (firstTextBoxOrElementItself)
            {
                // AW document model do not accept negative vertical margins (paragraph spacing).
                mTop = CalculateCurrentMarginValue(CssBoxMarginSide.Top, false);

                // We manually collapse adjacent vertical margins if they should collapse according to CSS rules
                // but MS Word refuses to do it.
                if (CurrentContext.CurrentBoxTopMarginManualCollapsing == CssBoxMarginManualCollapsing.Collapse)
                {
                    double correctedValue = System.Math.Max(mTop.Value - CurrentContext.PreviousBoxBottomMargin, 0);
                    mTop = new CssLength(correctedValue, mTop.IsCustom);
                }
                else if (CurrentContext.CurrentBoxTopMarginManualCollapsing == CssBoxMarginManualCollapsing.Zero)
                {
                    mTop = new CssLength(0, mTop.IsCustom);
                }
            }
            else
            {
                // Margin value of an implicit text box.
                mTop = CssLength.ZeroDefault;
            }
        }

        private void RecalculateBottom()
        {
            bool lastTextBoxOrElementItself = (!CurrentBox.CanContainImplicitTextBoxes) ||
                (CurrentBox.ChildBottomMarginCollapsing != CssBoxMarginCollapsing.Separate);

            if (lastTextBoxOrElementItself)
            {
                // AW document model do not accept negative vertical margins (paragraph spacing).
                mBottom = CalculateCurrentMarginValue(CssBoxMarginSide.Bottom, false);
            }
            else
            {
                // Margin value of an implicit text box.
                mBottom = CssLength.ZeroDefault;
            }

            // Remember the bottom margin of the current box in case we have to manually collapse it
            // with the top margin of the next box.
            CurrentContext.CurrentBoxBottomMargin = mBottom.Value;
        }

        private CssLength CalculateCurrentMarginValue(CssBoxMarginSide side, bool isNegativeResultAllowed)
        {
            return CalculateMarginValue(0, side, isNegativeResultAllowed);
        }

        private CssLength CalculateParentMarginValue(CssBoxMarginSide side, bool isNegativeResultAllowed)
        {
            // Note that only horizontal margins of an ancestor element can be recalculated at an arbitrary moment. This method
            // will return invalid results for vertical margins of ancestor boxes, because vertical margins may collapse.
            return CalculateMarginValue(1, side, isNegativeResultAllowed);
        }

        private CssLength CalculateMarginValue(
            int depth,
            CssBoxMarginSide side,
            bool isNegativeResultAllowed)
        {
            double sumMargin = 0;
            double maxPositiveCollapsedMargin = 0;
            double minNegativeCollapsedMargin = 0;
            bool onlyDefaultMarginsCollapsed = true;
            bool isCustom = false;

            // At the top level, adjacent bottom margins of elements are collapsed to zero if they all have default values.
            bool zeroDefaultCollapsedMargins = side == CssBoxMarginSide.Top;

            // Process boxes from innermost to outermost.
            int currentDepth = -1;
            foreach (CssBox box in mBoxes)
            {
                ++currentDepth;
                if (currentDepth < depth)
                {
                    // Skip inner elements we are not interested in.
                    continue;
                }

                CssBoxMargin margin = box.GetMargin(side);

                if (margin.Length.IsCustom)
                {
                    if (!MathUtil.IsZero(margin.Length.Value))
                    {
                        onlyDefaultMarginsCollapsed = false;
                    }
                    isCustom = true;
                }

                if ((margin.Length.Value > 0) && (margin.Length.Value > maxPositiveCollapsedMargin))
                {
                    maxPositiveCollapsedMargin = margin.Length.Value;
                }
                else if ((margin.Length.Value < 0) && (margin.Length.Value < minNegativeCollapsedMargin))
                {
                    minNegativeCollapsedMargin = margin.Length.Value;
                }

                if (margin.CollapsingWithParentMargin == CssBoxMarginCollapsing.Separate)
                {
                    // Stop margin calculation.
                    zeroDefaultCollapsedMargins = margin.ZeroDefaultCollapsedMargins;
                    break;
                }

                if (margin.CollapsingWithParentMargin != CssBoxMarginCollapsing.Collapse)
                {
                    // Restart collapsed margin calculation if the current margin does not collapse with the margin of its child.
                    sumMargin += maxPositiveCollapsedMargin + minNegativeCollapsedMargin;
                    maxPositiveCollapsedMargin = 0;
                    minNegativeCollapsedMargin = 0;
                    onlyDefaultMarginsCollapsed = true;
                }
            }

            // Some quirks require default vertical margins to collapse to zero at root element level.
            if (zeroDefaultCollapsedMargins && onlyDefaultMarginsCollapsed)
            {
                maxPositiveCollapsedMargin = 0;
                minNegativeCollapsedMargin = 0;
            }

            double value = sumMargin + maxPositiveCollapsedMargin + minNegativeCollapsedMargin;
            if ((!isNegativeResultAllowed) && (value < 0))
            {
                value = 0;
            }
            return new CssLength(value, isCustom);
        }

        private CssBox ComputeUsualBox(IHtmlElementProvider element, CssDeclarationCollection cssDeclarations)
        {
            CssLength leftValue = new CssLength(
                ComputeHorizontalMargin("margin-left", "padding-left", "border-left", cssDeclarations),
                IsCustomHorizontalMargin("margin-left", "padding-left", cssDeclarations));

            CssLength rightValue = new CssLength(
                ComputeHorizontalMargin("margin-right", "padding-right", "border-right", cssDeclarations),
                IsCustomHorizontalMargin("margin-right", "padding-right", cssDeclarations));

            CssLength topValue = new CssLength(
                ComputeVerticalMargin("margin-top", element, cssDeclarations),
                IsCustomVerticalMargin("margin-top", cssDeclarations));

            CssLength bottomValue = new CssLength(
                ComputeVerticalMargin("margin-bottom", element, cssDeclarations),
                IsCustomVerticalMargin("margin-bottom", cssDeclarations));

            // WORDSNET-18824 Collapse margins of an empty block with the top margin of the block right after the empty
            // one.
            if ((CurrentContext.EmptyBlockMarginBefore != null) && (CurrentContext.EmptyBlockMarginBefore.Value > topValue.Value))
            {
                topValue = CurrentContext.EmptyBlockMarginBefore;
            }

            // Margin collapsing quirk. In quirks mode, the last paragraph element in a table cell has zero bottom margin.
            // We cannot apply this quirk to <p> element (our CSS box model does not allow this), so we modify default bottom
            // margin of all elements in a cell except the last <p>.
            // See http://www.w3.org/TR/html5/rendering.html#margin-collapsing-quirks
            // Testing in browsers shows that this quirk also applies to preformatted elements (<pre>, <xmp>, and <listing>).
            if (CurrentBox.IsInsideTableCell &&
                !bottomValue.IsCustom &&
                !MathUtil.IsZero(bottomValue.Value) &&
                element.IsElementContainsText)
            {
                if ((!HasBottomMarginQuirkWhenLastInCell(element)) ||
                    (mDocumentMode != CssDocumentMode.Quirks) ||
                    (CurrentBox.ChildBottomMarginCollapsing == CssBoxMarginCollapsing.Separate))
                {
                    bottomValue = new CssLength(bottomValue.Value, true);
                }
            }

            CssDeclaration borderTopStyleDeclaration = cssDeclarations["border-top-style"];
            bool hasTopBorder = (borderTopStyleDeclaration != null) &&
                (!borderTopStyleDeclaration.Value.Equals(CssValue.None)) &&
                (!borderTopStyleDeclaration.Value.Equals(CssValue.Hidden));

            CssDeclaration borderBottomStyleDeclaration = cssDeclarations["border-bottom-style"];
            bool hasBottomBorder = (borderBottomStyleDeclaration != null) &&
                (!borderBottomStyleDeclaration.Value.Equals(CssValue.None)) &&
                (!borderBottomStyleDeclaration.Value.Equals(CssValue.Hidden));

            CssBoxMargin left = CssBoxMargin.Create(leftValue, CssBoxMarginCollapsing.Add, false);
            CssBoxMargin right = CssBoxMargin.Create(rightValue, CssBoxMarginCollapsing.Add, false);

            CssBoxMargin top = CssBoxMargin.Create(topValue, CurrentBox.ChildTopMarginCollapsing, false);
            CssBoxMargin bottom = CssBoxMargin.Create(bottomValue, CurrentBox.ChildBottomMarginCollapsing, false);

            CssBox box = new CssBox(left, right, top, bottom, element.StartsWithImplicitBox, element.EndsWithImplicitBox,
                !hasBottomBorder, CurrentBox.IsInsideTableCell, true);
            if (hasTopBorder)
            {
                box.ChildTopMarginCollapsing = CssBoxMarginCollapsing.Add;
            }
            if (hasBottomBorder)
            {
                box.ChildBottomMarginCollapsing = CssBoxMarginCollapsing.Add;
            }

            return box;
        }

        private CssBox ComputeInlineBox(IHtmlElementProvider element)
        {
            CssBoxMargin left = CssBoxMargin.Create(CssLength.ZeroDefault, CssBoxMarginCollapsing.Add, false);
            CssBoxMargin right = CssBoxMargin.Create(CssLength.ZeroDefault, CssBoxMarginCollapsing.Add, false);
            CssBoxMargin top = CssBoxMargin.Create(CssLength.ZeroDefault, CurrentBox.ChildTopMarginCollapsing, false);
            CssBoxMargin bottom = CssBoxMargin.Create(CssLength.ZeroDefault, CurrentBox.ChildBottomMarginCollapsing, false);

            return new CssBox(left, right, top, bottom, element.StartsWithImplicitBox, element.EndsWithImplicitBox, true,
                CurrentBox.IsInsideTableCell, true);
        }

        private CssBox ComputeCaptionBox(IHtmlElementProvider element, CssDeclarationCollection cssDeclarations)
        {
            CssLength leftLength = new CssLength(ComputeTableCellMargin("margin-left", cssDeclarations), true);
            CssBoxMargin left = CssBoxMargin.Create(leftLength, CssBoxMarginCollapsing.Separate, false);

            CssLength rightLength = new CssLength(ComputeTableCellMargin("margin-right", cssDeclarations), true);
            CssBoxMargin right = CssBoxMargin.Create(rightLength, CssBoxMarginCollapsing.Separate, false);

            double topValue = ComputeTableCellMargin("margin-top", cssDeclarations);
            CssLength topLength = new CssLength(topValue, !MathUtil.IsZero(topValue));
            CssBoxMargin top = CssBoxMargin.Create(topLength, CssBoxMarginCollapsing.Separate, true);

            double bottomValue = ComputeTableCellMargin("margin-bottom", cssDeclarations);
            CssLength bottomLength = new CssLength(bottomValue, !MathUtil.IsZero(bottomValue));
            CssBoxMargin bottom = CssBoxMargin.Create(bottomLength, CssBoxMarginCollapsing.Separate, true);

            return new CssBox(left, right, top, bottom, element.StartsWithImplicitBox, element.EndsWithImplicitBox, true,
                CurrentBox.IsInsideTableCell, true);
        }

        private CssBox ComputeTableBox(IHtmlElementProvider element, CssDeclarationCollection cssDeclarations)
        {
            CssLength leftLength = new CssLength(ComputeTableCellMargin("margin-left", cssDeclarations), true);
            CssBoxMargin left = CssBoxMargin.Create(leftLength, CssBoxMarginCollapsing.Add, false);

            CssLength rightLength = new CssLength(ComputeTableCellMargin("margin-right", cssDeclarations), true);
            CssBoxMargin right = CssBoxMargin.Create(rightLength, CssBoxMarginCollapsing.Add, false);

            double topValue = ComputeTableCellMargin("margin-top", cssDeclarations);
            CssLength topLength = new CssLength(topValue, !MathUtil.IsZero(topValue));
            CssBoxMargin top = CssBoxMargin.Create(topLength, CurrentBox.ChildTopMarginCollapsing, true);

            double bottomValue = ComputeTableCellMargin("margin-bottom", cssDeclarations);
            CssLength bottomLength = new CssLength(bottomValue, !MathUtil.IsZero(bottomValue));
            CssBoxMargin bottom = CssBoxMargin.Create(bottomLength, CurrentBox.ChildBottomMarginCollapsing, true);

            return new CssBox(left, right, top, bottom, element.StartsWithImplicitBox, element.EndsWithImplicitBox, true,
                CurrentBox.IsInsideTableCell, false);
        }

        private CssBox ComputeTableCellBox(IHtmlElementProvider element)
        {
            CssBoxMargin zeroCollapseDefault = CssBoxMargin.Create(CssLength.ZeroDefault, CssBoxMarginCollapsing.Separate, true);
            CssBoxMargin zeroKeepDefault = CssBoxMargin.Create(CssLength.ZeroDefault, CssBoxMarginCollapsing.Separate, false);

            CssBoxMargin left = zeroKeepDefault;
            CssBoxMargin right = zeroKeepDefault;
            CssBoxMargin bottom = zeroCollapseDefault;

            // Default vertical margins of an element inside a table cell collapse to zero when the element's box is the first
            // or the last box of the cell. See http://www.w3.org/TR/html5/rendering.html#margin-collapsing-quirks
            CssBoxMargin top = ((mDocumentMode == CssDocumentMode.Quirks) || !element.IsElementContainsText)
                ? zeroCollapseDefault
                : zeroKeepDefault;

            return new CssBox(left, right, top, bottom, element.StartsWithImplicitBox, element.EndsWithImplicitBox, true, true, true);
        }

        private CssBox ComputeInlineListBox(IHtmlElementProvider element, CssDeclarationCollection cssDeclarations, CssDisplayType displayType)
        {
            CssBoxMargin left = CssBoxMargin.Create(CssLength.ZeroDefault, CssBoxMarginCollapsing.Add, false);
            CssBoxMargin right = CssBoxMargin.Create(CssLength.ZeroDefault, CssBoxMarginCollapsing.Add, false);

            CssLength topValue = (displayType == CssDisplayType.Inline)
                ? new CssLength(ComputeVerticalMargin("margin-top", element, cssDeclarations),
                    IsCustomVerticalMargin("margin-top", cssDeclarations))
                : CssLength.ZeroDefault;

            CssBoxMargin top = CssBoxMargin.Create(topValue, CurrentBox.ChildTopMarginCollapsing, false);
            CssBoxMargin bottom = CssBoxMargin.Create(CssLength.ZeroDefault, CurrentBox.ChildBottomMarginCollapsing, false);

            return new CssBox(left, right, top, bottom, element.StartsWithImplicitBox, element.EndsWithImplicitBox, true,
                CurrentBox.IsInsideTableCell, true);
        }

        private static CssBox ComputeDisplayTableCellBox(IHtmlElementProvider element, CssDeclarationCollection cssDeclarations)
        {
            CssLength leftValue = new CssLength(ComputeHorizontalMargin("padding-left", "border-left-width", cssDeclarations),
                HasCustomValue("padding-left", cssDeclarations));

            CssLength rightValue = new CssLength(ComputeHorizontalMargin("padding-right", "border-right-width", cssDeclarations),
                HasCustomValue("padding-right", cssDeclarations));

            CssBoxMargin left = CssBoxMargin.Create(leftValue, CssBoxMarginCollapsing.Add, false);
            CssBoxMargin right = CssBoxMargin.Create(rightValue, CssBoxMarginCollapsing.Add, false);
            CssBoxMargin top = CssBoxMargin.Create(CssLength.ZeroDefault, CssBoxMarginCollapsing.Separate, false);
            CssBoxMargin bottom = CssBoxMargin.Create(CssLength.ZeroDefault, CssBoxMarginCollapsing.Separate, true);

            return new CssBox(left, right, top, bottom, element.StartsWithImplicitBox, element.EndsWithImplicitBox,
                true, true, true);
        }

        private static bool IsCustomHorizontalMargin(
            string marginProperty,
            string paddingProperty,
            CssDeclarationCollection cssDeclarations)
        {
            return HasCustomValue(marginProperty, cssDeclarations) ||
                HasCustomValue(paddingProperty, cssDeclarations);
        }

        private static bool HasCustomValue(string property, CssDeclarationCollection declarations)
        {
            Debug.Assert(StringUtil.HasChars(property));
            CssDeclaration declaration = declarations[property];
            // Values that come from the User Agent stylesheet are also considered default.
            return (declaration != null) && (!declarations.IsUserAgent(property));
        }

        private static bool IsCustomVerticalMargin(string marginPropertyName, CssDeclarationCollection cssDeclarations)
        {
            return cssDeclarations[marginPropertyName] != null;
        }

        private static double ComputeHorizontalMargin(string marginPropertyName, string paddingPropertyName,
            string borderPropertyPrefix, CssDeclarationCollection cssDeclarations)
        {
            return ComputeMargin(cssDeclarations, marginPropertyName) +
                   ComputePadding(cssDeclarations, paddingPropertyName) +
                   ComputeBorderLineWidth(cssDeclarations, borderPropertyPrefix);
        }

        private static double ComputeHorizontalMargin(string paddingPropertyName,
            string borderWidthPropertyName, CssDeclarationCollection cssDeclarations)
        {
            return ComputePadding(cssDeclarations, paddingPropertyName) +
                   ComputeBorderLineWidth(cssDeclarations, borderWidthPropertyName);
        }

        private static double ComputeBorderLineWidth(CssDeclarationCollection cssDeclarations, string borderPropertyPrefix)
        {
            Debug.Assert(!string.IsNullOrEmpty(borderPropertyPrefix));
            Debug.Assert(cssDeclarations != null);

            string borderStylePropertyName = borderPropertyPrefix + "-style";
            CssDeclaration borderStyleDeclaration = cssDeclarations[borderStylePropertyName];
            if ((borderStyleDeclaration != null) && (!borderStyleDeclaration.Value.Equals(CssValue.None)))
            {
                CssDeclaration borderWidthDeclaration = cssDeclarations[borderPropertyPrefix + "-width"];
                if (borderWidthDeclaration != null)
                {
                    return CssUtil.GetBorderLineWidth(borderWidthDeclaration.Value);
                }
            }
            return 0;
        }

        private static double ComputePadding(CssDeclarationCollection cssDeclarations, string paddingPropertyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(paddingPropertyName));
            Debug.Assert(cssDeclarations != null);

            CssDeclaration paddingDeclaration = cssDeclarations[paddingPropertyName];
            if (paddingDeclaration != null)
                return CssPaddingIndividualPropertyDefBase.GetPadding(paddingDeclaration.Value);
            return 0;
        }

        private static double ComputeMargin(CssDeclarationCollection cssDeclarations, string marginPropertyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(marginPropertyName));
            Debug.Assert(cssDeclarations != null);

            CssDeclaration marginDeclaration = cssDeclarations[marginPropertyName];
            if (marginDeclaration != null)
                return CssMarginIndividualPropertyDefBase.GetIndent(marginDeclaration.Value);
            return 0;
        }

        private double ComputeVerticalMargin(string marginPropertyName, IHtmlElementProvider element,
            CssDeclarationCollection cssDeclarations)
        {
            CssDeclaration marginDeclaration = cssDeclarations[marginPropertyName];
            if (marginDeclaration != null)
            {
                return CssMarginIndividualPropertyDefBase.GetIndent(marginDeclaration.Value);
            }

            double defaultMarginValue = GetDefaultVerticalMargins(element);
            // According to the CSS specification, relative values refer to the 'font-size' value of the corresponding HTML element.
            // In MS Word, however, such values refer to a fixed 'font-size' value that doesn't change from element to element.
            double fontSize = mApplyFormattingAsMsWord
                ? CssUtil.DefaultFontSize
                : CssUtil.ComputeFontSize(cssDeclarations);
            return defaultMarginValue * fontSize;
        }

        private static double ComputeTableCellMargin(string marginPropertyName, CssDeclarationCollection cssDeclarations)
        {
            CssDeclaration marginDeclaration = cssDeclarations[marginPropertyName];
            return (marginDeclaration != null)
                ? CssMarginIndividualPropertyDefBase.GetIndent(marginDeclaration.Value)
                : 0;
        }

        /// <summary>
        /// Returns default (user agent) vertical margins of HTML elements, in em.
        /// </summary>
        /// <param name="element">An HTML element.</param>
        /// <returns>
        /// Vertical margins of the HTML element as declared in the default (user agent) style sheet.
        /// </returns>
        /// <remarks>
        /// These margins cannot be declared directly in the user agent style sheet, because in this case we will not be able
        /// to tell where a CSS margin property came from: the author style sheet or the user agent style sheet.
        /// </remarks>
        private double GetDefaultVerticalMargins(IHtmlElementProvider element)
        {
            if (!mApplyDefaultVerticalMargins)
            {
                return 0;
            }

            return HtmlDefaultVerticalMargins.GetMargins(element);
        }

        private static bool HasBottomMarginQuirkWhenLastInCell(IHtmlElementProvider element)
        {
            return (element.ElementName == "p") ||
                (element.ElementName == "pre") ||
                (element.ElementName == "listing") ||
                (element.ElementName == "xmp") ||
                (element.ElementName == "plaintext");
        }

        private static bool IsFloatingTable(string elementName, CssDisplayType displayType, CssDeclarationCollection cssDeclarations)
        {
            if ((elementName != "table") || (displayType != CssDisplayType.Table))
            {
                return false;
            }

            CssDeclaration floatDeclaration = cssDeclarations["float"];
            if (floatDeclaration == null)
            {
                return false;
            }

            CssPropertyValue floatValue = floatDeclaration.Value;
            return floatValue.Equals(CssValue.Left) || floatValue.Equals(CssValue.Right);
        }

        /// <summary>
        /// Indicates whether a box is full-width (takes the whole width of an enclosing box).
        /// </summary>
        /// <remarks>
        /// This method is a quick and simple check for situations where a box is certainly full-width.
        /// We cannot do a more general check here, because it requires performing layout of boxes.
        /// </remarks>
        private static bool IsFullWidthBox(CssDeclarationCollection cssDeclarations)
        {
            CssDeclaration widthDeclaration = cssDeclarations["width"];
            return (widthDeclaration != null) && widthDeclaration.Value.Equals(new CssPercentageValue(100));
        }

        private CssBox CurrentBox
        {
            get
            {
                Debug.Assert(mBoxes.Count > 0);
                return mBoxes.Peek();
            }
        }

        private CssBoxFormattingContext CurrentContext
        {
            get
            {
                Debug.Assert(mContexts.Count > 0);
                return mContexts.Peek();
            }
        }

        private readonly CssDocumentMode mDocumentMode;

        private readonly bool mApplyDefaultVerticalMargins;

        /// <summary>
        /// Indicates whether we should stick to MS Word's rules when applying CSS formatting to document model.
        /// </summary>
        private readonly bool mApplyFormattingAsMsWord;

        private readonly Stack<CssBox> mBoxes = new Stack<CssBox>();

        private readonly Stack<CssBoxFormattingContext> mContexts = new Stack<CssBoxFormattingContext>();

        /// <summary>
        /// Indicates whether any boxes were added or removed from the model since last recalculation.
        /// </summary>
        /// <remarks>
        /// Model must be recalculated no more than once on each modification (addition or removal of an element).
        /// This flag is used to prevent additional recalculations.
        /// </remarks>
        private bool mRecalculationNeeded;

        private CssLength mLeft = CssLength.ZeroDefault;
        private CssLength mRight = CssLength.ZeroDefault;
        private CssLength mTop = CssLength.ZeroDefault;
        private CssLength mBottom = CssLength.ZeroDefault;
    }
}
