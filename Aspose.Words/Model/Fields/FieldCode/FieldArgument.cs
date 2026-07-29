// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/11/2009 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a field argument with original formatting preserved.
    /// Field argument may sometimes be a source for field result (TrueText or FalseText in IF fields), so
    /// it implements the <see cref="IFieldResult"/> interface.
    /// </summary>
    internal class FieldArgument : IFieldArgument, IFieldResult
    {
        internal FieldArgument(
            Field field,
            string text,
            RichString richText,
            NodeRange range,
            NodeRange completeFieldRange,
            bool isInDoubleQuotes)
        {
            Debug.Assert(text != null);

            mField = field;
            mText = text;
            mRichText = richText;
            Range = range;
            CompleteFieldRange = completeFieldRange;
            IsInDoubleQuotes = isInDoubleQuotes;
        }

        /// <summary>
        /// Gets the value of the argument as a plain text. Also decodes it, translates to invariant language
        /// if it is translatable and trims double quotes.
        /// </summary>
        internal string GetNormalizedText()
        {
            return GetNormalizedText(true);
        }

        /// <summary>
        /// Gets the value of the argument as a plain text. Also decodes it and translates to invariant language
        /// if it is translatable.
        /// Double quotes should be trimmed in most cases except of in comparison expression operands because
        /// they are used there to determine whether to compare the operands as strings.
        /// </summary>
        /// <param name="trimDoubleQuotes">True to trim double quotes if present.</param>
        internal string GetNormalizedText(bool trimDoubleQuotes)
        {
            return FieldTokenDecoder.DecodeToken(Text, Range, trimDoubleQuotes && IsInDoubleQuotes);
        }

        internal RichString GetNormalizedRichText()
        {
            return GetNormalizedRichText(true);
        }

        internal RichString GetNormalizedRichText(bool trimDoubleQuotes)
        {
            return FieldTokenDecoder.DecodeToken(RichText, Range, trimDoubleQuotes && IsInDoubleQuotes);
        }

        /// <summary>
        /// Returns a value, indicating contains this argument child fields or not
        /// </summary>
        internal bool ContainsChildFields()
        {
            foreach (Node node in Range)
            {
                if (node.NodeType == NodeType.FieldStart)
                    return true;
            }

            return false;
        }

        Constant IFieldResult.GetFieldResultValue()
        {
            return new StringConstant(GetNormalizedText());
        }

        NodeRange IFieldResult.GetFieldResultRange()
        {
            return Range;
        }

        void IFieldArgument.InvalidateText()
        {
            mText = null;
            mRichText = null;

            // WORDSNET-18544 Adjust argument range to nested field result, if nested field did not have
            // separator when field code was parsed.
            AdjustNestedFieldArgumentRangeWithoutSeparator();
        }

        private void AdjustNestedFieldArgumentRangeWithoutSeparator()
        {
            if (!Range.IsSameNode)
                return;

            if (Range.End.Node.NodeType != NodeType.FieldEnd)
                return;

            FieldEnd fieldEnd = (FieldEnd)Range.End.Node;
            if (!fieldEnd.HasSeparator)
                return;

            Range = fieldEnd.GetField().GetFieldResultRange();
        }

        internal void EnsureText()
        {
            if (mText == null)
            {
                mText = FieldCodeParser.GetArgumentTextByRange(this);
                Debug.Assert(mText != null);
            }
        }

        /// <summary>
        /// Gets a string representation of this argument.
        /// </summary>
        internal string Text
        {
            get
            {
                EnsureText();
                return mText;
            }
        }

        /// <summary>
        /// Gets a rich string representation of this argument.
        /// </summary>
        internal RichString RichText
        {
            get
            {
                if (mRichText == null)
                {
                    mRichText = FieldCodeParser.GetArgumentRichTextByRange(this);
                    Debug.Assert(mRichText != null);
                }

                return mRichText;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this argument is enclosed in double quotes.
        /// This relates to the original argument content, i.e. before any translation.
        /// </summary>
        internal bool IsInDoubleQuotes { get; }

        /// <summary>
        /// Gets a value indicating whether this argument is represented by a single field's result.
        /// </summary>
        internal bool IsSingleFieldResult
        {
            get { return FieldUtil.EndsWithFieldEnd(Range); }
        }

        /// <summary>
        /// Gets a node range of the field argument.
        /// </summary>
        internal NodeRange Range { get; private set; }

        internal Field Field
        {
            get { return mField; }
        }

        internal NodeRange CompleteFieldRange { get; }

        internal FieldArgument Clone(NodeRange range)
        {
            return new FieldArgument(
                mField,
                mText,
                mRichText,
                range,
                CompleteFieldRange,
                IsInDoubleQuotes);
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Field mField;
        private string mText;
        private RichString mRichText;
    }
}
