// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/01/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Applies a field result represented by a string.
    /// </summary>
    internal class TextResultApplier : FieldResultApplier
    {
        internal TextResultApplier(Field field, string result)
            : this(field, result, true)
        {
        }

        internal TextResultApplier(Field field, string result, bool applyFormat)
            : this(field, new FieldFormattingResult(RichString.CreateFromString(result), false), applyFormat)
        {

        }

        internal TextResultApplier(Field field, FieldFormattingResult result)
            : this(field, result, true)
        {
        }

        internal TextResultApplier(Field field, FieldFormattingResult result, ParagraphBreakCharReplacement paragraphBreakCharReplacement)
            : this(field, result, true)
        {
            mParagraphBreakCharReplacement = paragraphBreakCharReplacement;
        }

        private TextResultApplier(Field field, FieldFormattingResult result, bool applyFormat)
            : base(field, applyFormat)
        {
            Debug.Assert(result != null);
            mResult = result;
        }

        protected override void ApplyResultCore()
        {
            Document document = Field.FetchDocument();
            // Use the builder because it inserts multi-paragraph text properly.
            DocumentBuilder builder = new DocumentBuilder(document);
            builder.ParagraphBreakCharReplacement = mParagraphBreakCharReplacement;

            // WORDSNET-4921 Set font attributes from the previous node.
            // WORDSNET-13345 Set font attributes from field code node.
            MoveAndSetFontAttributesFromPreviousNode(builder, Field.End, Field.Start);

            FieldTextHelper.WriteTextBidiAware(Field, builder, mResult);
        }

        /// <summary>
        /// Moves the builder to refNode, but resets the direct font attributes set by builder.MoveTo()
        /// with attributes from the following non whitespace inline node, if it is found.
        /// </summary>
        private void MoveAndSetFontAttributesFromPreviousNode(DocumentBuilder builder, Node refNode, FieldStart formattingStartpointNode)
        {
            builder.MoveTo(refNode);

            bool isStartHidden = IsHidden(formattingStartpointNode);

            // Originally, the inserted text should get the attributes from the first non whitespace node.
            // Later they may or may not be revised when applying the field format.
            // DocumentBuilder.MoveTo() sets the attributes from the next node.
            // So look for a following inline node to get the formatting attributes from.
            Node formattingSource = formattingStartpointNode;
            Inline inlineFormattingSource;
            do
            {
                formattingSource = formattingSource.NextSibling;
                inlineFormattingSource = formattingSource as Inline;
                if (inlineFormattingSource != null &&
                    NodeUtil.IsNonWhitespace(inlineFormattingSource) &&
                    (IsHidden(inlineFormattingSource) == isStartHidden))
                    break;
            } while (formattingSource != null);

            if (inlineFormattingSource != null)
            {
                // Use the attributes from the previous node, if they are present.
                builder.SetFont(inlineFormattingSource.RunPr, true);
            }
            // Do not change the attributes if a valid previous node not found.
            // They are still set from the refNode by DocumentBuilder.MoveTo().
        }

        private bool IsHidden(Inline inline)
        {
            return Field.IsUpdating
                ? Field.Updater.HiddenAttributeCache.GetHiddenAttribute(inline)
                : inline.IsHiddenOrDeleted;
        }

        private readonly FieldFormattingResult mResult;
        private readonly ParagraphBreakCharReplacement mParagraphBreakCharReplacement;
    }
}
