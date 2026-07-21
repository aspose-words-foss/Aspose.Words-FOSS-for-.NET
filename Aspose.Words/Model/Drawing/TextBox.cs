// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/07/2006 by Roman Korchagin

using System;
using Aspose.Collections.Generic;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Defines attributes that specify how a text is displayed inside a shape.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-shapes/">Working with Shapes</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Use the <see cref="Shape.TextBox"/> property to access text properties of a shape.
    /// You do not create instances of the <see cref="TextBox"/> class directly.</p>
    ///
    /// <seealso cref="Shape.TextBox"/>
    /// </remarks>
    public class TextBox
    {
        internal TextBox(Shape parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Specifies the inner left margin in points for a shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1/10 inch.</p>
        /// </remarks>
        public double InternalMarginLeft
        {
            get { return mParent.GraphicData.InternalMarginLeft; }
            set { mParent.GraphicData.InternalMarginLeft = value; }
        }

        /// <summary>
        /// Specifies the inner right margin in points for a shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1/10 inch.</p>
        /// </remarks>
        public double InternalMarginRight
        {
            get { return mParent.GraphicData.InternalMarginRight; }
            set { mParent.GraphicData.InternalMarginRight = value; }
        }

        /// <summary>
        /// Specifies the inner top margin in points for a shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1/20 inch.</p>
        /// </remarks>
        public double InternalMarginTop
        {
            get { return mParent.GraphicData.InternalMarginTop; }
            set { mParent.GraphicData.InternalMarginTop = value; }
        }

        /// <summary>
        /// Specifies the inner bottom margin in points for a shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1/20 inch.</p>
        /// </remarks>
        public double InternalMarginBottom
        {
            get { return mParent.GraphicData.InternalMarginBottom; }
            set { mParent.GraphicData.InternalMarginBottom = value; }
        }

        /// <summary>
        /// Determines whether Microsoft Word will grow the shape to fit text.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool FitShapeToText
        {
            get { return mParent.GraphicData.FitShapeToText; }
            set { mParent.GraphicData.FitShapeToText = value; }
        }

        /// <summary>
        /// Determines the flow of the text layout in a shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.LayoutFlow.Horizontal"/>.</p>
        /// </remarks>
        public LayoutFlow LayoutFlow
        {
            get { return mParent.GraphicData.LayoutFlow; }
            set { mParent.GraphicData.LayoutFlow = value; }
        }

        /// <summary>
        /// Determines how text wraps inside a shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Words.Drawing.TextBoxWrapMode.Square"/>.</p>
        /// </remarks>
        public TextBoxWrapMode TextBoxWrapMode
        {
            get { return mParent.GraphicData.TextBoxWrapMode; }
            set { mParent.GraphicData.TextBoxWrapMode = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating either text of the TextBox should not rotate when the shape is rotated.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c></p>
        /// </remarks>
        public bool NoTextRotation
        {
            get { return mParent.GraphicData.TextBoxNoTextRotation; }
            set { mParent.GraphicData.TextBoxNoTextRotation = value; }
        }

        /// <summary>
        /// Specifies the vertical alignment of the text within a shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.TextBoxAnchor.Top"/>.</p>
        /// </remarks>
        public TextBoxAnchor VerticalAnchor
        {
            get
            {
                return TextBoxAnchor;
            }

            set
            {
                switch (value)
                {
                    case TextBoxAnchor.Top:
                    case TextBoxAnchor.Middle:
                    case TextBoxAnchor.Bottom:
                        TextBoxAnchor = value;
                        break;
                    default:
                        TextBoxAnchor = TextBoxAnchor.Top;
                        break;
                }
            }
        }

        /// <summary>
        /// Determines whether this <see cref="TextBox"/> can be linked to the target <see cref="TextBox"/>.
        /// </summary>
        public bool IsValidLinkTarget(TextBox target)
        {
            return string.IsNullOrEmpty(ValidateLinkTarget(target));
        }

        /// <summary>
        /// Returns or sets a <see cref="TextBox"/> that represents the next <see cref="TextBox"/> in a sequence of shapes.
        /// </summary>
        public TextBox Next
        {
            get
            {
                // Check cached value is valid (shape can be removed, for example).
                if (!IsNext(this, mNext))
                {
                    // We don't hold Shape reference in DOM and have to iterate all shape to find next in chain.
                    mNext = null;
                    ShapeCollection shapes = new ShapeCollection(mParent.Document);

                    foreach (Shape shape in shapes)
                        if (IsNext(mParent, shape))
                        {
                            mNext = shape.TextBox;
                            break;
                        }
                }

                return mNext;
            }
            set
            {
                if (value == null)
                {
                    BreakForwardLink();
                }
                else
                {
                    AddLink(value);
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="TextBox"/> that represents the previous <see cref="TextBox"/> in a sequence of shapes.
        /// </summary>
        public TextBox Previous
        {
            get
            {
                // Check cached value is valid (shape can be removed, for example).
                if (!IsNext(mPrevious, this))
                {
                    // We don't hold Shape reference in DOM and have to iterate all shape to find next in chain.
                    mPrevious = null;
                    ShapeCollection shapes = new ShapeCollection(mParent.Document);

                    foreach (Shape shape in shapes)
                        if (IsNext(shape, mParent))
                        {
                            mPrevious = shape.TextBox;
                            break;
                        }
                }

                return mPrevious;
            }
        }

        /// <summary>
        /// Breaks the link to the next <see cref="TextBox"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="BreakForwardLink"/> doesn't break all other links in the current sequence of shapes.
        /// For example: 1-2-3-4 sequence and <see cref="BreakForwardLink"/> at the 2-nd textbox will create
        /// two sequences 1-2, 3-4.
        /// </remarks>
        public void BreakForwardLink()
        {
            if (mParent.MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                // int key - shape's 0-based index in a chain, value - shape.
                SortedIntegerListGeneric<ShapeBase> linkedChain = new SortedIntegerListGeneric<ShapeBase>();

                int id = (mParent.TextboxId > 0) ? mParent.TextboxId : mParent.LinkedTextboxId;

                ShapeCollection shapes = new ShapeCollection(mParent.Document);
                foreach (Shape shape in shapes)
                {
                    if ((shape.LinkedTextboxId == id) || (shape.TextboxId == id))
                    {
                        int index = shape.TextboxId > 0 ? 0 : shape.LinkedTextboxSeq;
                        linkedChain.Add(index, shape);
                    }
                }

                int currentIndex = mParent.TextboxId > 0 ? 0 : mParent.LinkedTextboxSeq;

                if (linkedChain.Count <= 1)
                    return;

                // We have a chain, which consists of more than two textboxes. Split it into two parts.
                CreateNewChain(linkedChain, 0, currentIndex);
                CreateNewChain(linkedChain, currentIndex + 1, linkedChain.Count - 1);

                mParent.RemoveShapeAttrInternal(ShapeAttr.TextboxNextShapeId);

                Shape fallBack = (Shape)mParent.FallbackShape;

                if (fallBack != null)
                    fallBack.TextBox.BreakForwardLink();
            }
            else
            {
                mParent.RemoveShapeAttrInternal(ShapeAttr.TextboxNextShapeId);
            }
        }

        /// <summary>
        /// Gets a parent shape for the <see cref="TextBox"/>.
        /// </summary>
        public Shape Parent
        {
            get { return mParent; }
        }

        /// <summary>
        /// Defines the vertical anchoring of text in a textbox. Word 2007 only.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Words.Drawing.TextBoxAnchor.Top"/>.</p>
        /// </remarks>
        internal TextBoxAnchor TextBoxAnchor
        {
            get { return mParent.GraphicData.TextBoxAnchor; }
            set { mParent.GraphicData.TextBoxAnchor = value; }
        }

        /// <summary>
        /// Creates a new chain of linked textboxes.
        /// </summary>
        /// <param name="linkedChain"> Initial chain of textboxes. int Key - sequence index, value - textbox</param>
        /// <param name="firstIndex"> Index of a textbox, which will be a head of new chain. </param>
        /// <param name="lastIndex"> Index of a textbox, which will be a tail of new chain</param>
        private void CreateNewChain(SortedIntegerListGeneric<ShapeBase> linkedChain, int firstIndex, int lastIndex)
        {
            if (firstIndex == lastIndex)
            {
                // Dettach a single shape from the chain.
                ShapeBase shape = linkedChain[firstIndex];
                shape.TextboxId = 0;
                shape.LinkedTextboxId = 0;
                shape.LinkedTextboxSeq = 0;
            }
            else
            {
                int id = mParent.Document.GetNextDmlTextBoxId();

                ShapeBase shape = linkedChain[firstIndex];

                // Create a head for a new chain.
                shape.TextboxId = id;
                shape.LinkedTextboxId = 0;
                shape.LinkedTextboxSeq = 0;

                // Mimic MSW, add an empty paragraph to a newly created head of the chain.
                if (!shape.HasChildNodes)
                {
                    Paragraph para = new Paragraph(mParent.Document);
                    shape.AppendChild(para);
                }

                for (int i = 1; i <= lastIndex - firstIndex; i++)
                {
                    // Create a chain.
                    shape = linkedChain[firstIndex + i];
                    shape.TextboxId = 0;
                    shape.LinkedTextboxId = id;
                    shape.LinkedTextboxSeq = i;
                }
            }
        }

        /// <summary>
        /// Verifies if link from source TextBox to target TextBox is valid.
        /// </summary>
        private static bool IsNext(TextBox source, TextBox target)
        {
            return !ArgumentUtil.OneIsNull(source, target) && IsNext(source.Parent, target.Parent);
        }

        /// <summary>
        /// Verifies that link from source shape to target shape is valid.
        /// </summary>
        internal static bool IsNext(Shape sourceShape, Shape targetShape)
        {
            if (sourceShape.MarkupLanguage != targetShape.MarkupLanguage)
                return false;

            // Make sure, that both shapes are still in the document.
            if ((sourceShape.Document != null) && (sourceShape.Document != targetShape.Document))
                return false;

            if (sourceShape.MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                int textboxId = sourceShape.TextboxId;
                int linkedTextboxId = sourceShape.LinkedTextboxId;

                if ((textboxId > 0) || (linkedTextboxId > 0))
                {
                    int id = (textboxId > 0) ? textboxId : linkedTextboxId;
                    int seq = (textboxId > 0) ? 1 : sourceShape.LinkedTextboxSeq + 1;

                    return (targetShape.LinkedTextboxId == id) && (targetShape.LinkedTextboxSeq == seq);
                }

                return false;
            }
            else
            {
                return (sourceShape.TextboxNextShapeId == targetShape.Id);
            }
        }

        /// <summary>
        /// Creates a link between textboxes.
        /// </summary>
        private void AddLink(TextBox target)
        {
            string errorMessage = ValidateLinkTarget(target);

            if (StringUtil.HasChars(errorMessage))
                throw new ArgumentException(errorMessage);

            Shape sourceShape = mParent;
            Shape targetShape = target.Parent;

            if (sourceShape.MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                int textboxId = mParent.TextboxId;
                int linkedTextboxId = mParent.LinkedTextboxId;

                if (textboxId > 0)
                {
                    // Head of the linked chain.
                    targetShape.LinkedTextboxId = textboxId;
                    targetShape.LinkedTextboxSeq = 1;
                }
                else if (linkedTextboxId > 0)
                {
                    // Tail of the linked chain.
                    targetShape.LinkedTextboxId = linkedTextboxId;
                    targetShape.LinkedTextboxSeq = mParent.LinkedTextboxSeq + 1;
                }
                else
                {
                    // Create a new link.
                    textboxId = mParent.Document.GetNextDmlTextBoxId();
                    mParent.TextboxId = textboxId;
                    targetShape.LinkedTextboxId = textboxId;
                    targetShape.LinkedTextboxSeq = 1;
                }

                targetShape.RemoveAllChildren();

                TextBox sourceFallbackTextbox = GetFallBackTextBox(sourceShape);
                TextBox targetFallbackTextbox = GetFallBackTextBox(targetShape);

                if ((sourceFallbackTextbox != null) && (targetFallbackTextbox != null))
                    sourceFallbackTextbox.Next = targetFallbackTextbox;
            }
            else
            {
                sourceShape.TextboxNextShapeId = targetShape.Id;
            }
        }

        /// <summary>
        /// Determines whether the TextBox can be linked to the target Textbox, and returns error message
        /// if it's not possible. Returns empty string, if everything is Ok.
        /// </summary>
        private string ValidateLinkTarget(TextBox target)
        {
            Shape sourceShape = mParent;
            Shape targetShape = target.Parent;

            // Check for the target textbox.
            if ((mParent == null) || (target.Parent == null))
                return WarningStrings.LinkTextboxInvalidTarget;

            // Check for the target textbox.
            if (mParent.Id == targetShape.Id)
                return WarningStrings.LinkTextboxInvalidTarget;

            // Check for the document.
            if ((sourceShape.Document == null) || (sourceShape.Document != targetShape.Document))
                return WarningStrings.LinkTextboxSameDocument;

            // Linked shapes must be able to store text.
            if (!ShapeCanStoreText(sourceShape))
                return WarningStrings.LinkTextboxBothShapesText;

            if (!ShapeCanStoreText(targetShape))
                return WarningStrings.LinkTextboxBothShapesText;

            // Target textbox must be empty. The case with a single empty paragraph is good enough for linking.
            string targetText = targetShape.GetText();
            if ((targetText != string.Empty) && (targetText != "\r"))
                return WarningStrings.LinkTextboxEmptyTarget;

            // Source and target must be one type DML/VML.
            if (sourceShape.MarkupLanguage != targetShape.MarkupLanguage)
                return WarningStrings.LinkTextboxSameMarkup;

            // Verify if textbox already has a link.
            if (Next != null)
                return WarningStrings.LinkTextboxHasLink;

            // Verify if target textbox is linked.
            if (target.Previous != null)
                return WarningStrings.LinkTextboxTargetIsLinked;

            // Verify if textboxes are from different story types.
            if (DifferentStoryTypes(targetShape, NodeType.Body) || DifferentStoryTypes(targetShape, NodeType.HeaderFooter))
                return WarningStrings.LinkTextboxDifferentStoryTypes;

            // Get fallback ValidateLinkTarget.
            if (mParent.MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                TextBox sourceFallbackTextbox = GetFallBackTextBox(sourceShape);
                TextBox targetFallbackTextbox = GetFallBackTextBox(targetShape);

                if ((sourceFallbackTextbox != null) && (targetFallbackTextbox != null))
                    return sourceFallbackTextbox.ValidateLinkTarget(targetFallbackTextbox);
            }

            return string.Empty;
        }

        /// <summary>
        /// If exists, gets a Textbox for a fallback shape, returns null otherwise.
        /// </summary>
        private static TextBox GetFallBackTextBox(Shape shape)
        {
            return (shape.FallbackShape == null) ? null : ((Shape)shape.FallbackShape).TextBox;
        }

        /// <summary>
        /// Verifies story type for the linked textboxes. Returns true if we should throw an exception.
        /// </summary>
        /// <remarks>
        /// There are several story types which are suitable for linking. For example, textboxes from MainText are good
        /// for linking. Textboxes from the different types of Headers/Footers are also good for linking. But when one textbox
        /// belongs to MainText and other textbox belongs to Headers/Footers - MSW refuses linking.
        /// </remarks>
        private bool DifferentStoryTypes(ShapeBase targetShape, NodeType nodeType)
        {
            bool source = mParent.GetAncestor(nodeType) != null;
            bool target = targetShape.GetAncestor(nodeType) != null;

            return source != target;
        }

        /// <summary>
        /// Determines if this shape is able to store a text.
        /// </summary>
        private static bool ShapeCanStoreText(ShapeBase shape)
        {
            return (shape.ShapeType == ShapeType.TextBox) || (shape.HasChildNodes);
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Shape mParent;

        private TextBox mPrevious;
        private TextBox mNext;
    }
}
