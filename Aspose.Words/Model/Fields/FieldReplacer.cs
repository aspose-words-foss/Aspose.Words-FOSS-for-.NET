// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/08/2013 by Ivan Lyagin

using System;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides methods for replacing of an individual field with its result and its completely removal
    /// considering that it can be a part of a parent field's code.
    /// </summary>
    internal static class FieldReplacer
    {
        /// <summary>
        /// Completely removes the specified field considering that it can be a part of a parent field's code.
        /// Returns a node right after the field. If the field's end is the last child of its parent node, returns
        /// its parent paragraph. If the field is already removed, returns <c>null</c>.
        /// </summary>
        internal static Node Remove(Field field)
        {
            if (field.IsRemoved)
                return null;

            SeparateFieldCharsIfNeeded(field, RemainingFieldPart.None);

            // We can obtain a proper reference node only after the possible separation above is done.
            // Field start is used because of NodeJoinMode.JoinToPreviousSibling.
            Node refNode = field.End.NextSibling ?? field.Start.ParentParagraph;

            NodeRemover.Remove(field.Start, field.End);

            NotifyUpdaterIfNeeded(field, true);

            return refNode;
        }

        /// <summary>
        /// Replaces the specified field with its result considering that it can be a part of a parent field's code.
        /// </summary>
        internal static void ReplaceWithResult(Field field)
        {
            ReplaceWithResult(field, EmptyFieldReplaceListener.Instance, NodeJoinMode.JoinToPreviousSibling);
        }

        /// <summary>
        /// Replaces the specified field with its result considering that it can be a part of a parent field's code.
        /// </summary>
        internal static void ReplaceWithResult(Field field, IFieldReplaceListener listener, NodeJoinMode joinMode)
        {
            if (field.IsRemoved)
                return;

            if (FieldUtil.IsFieldResultInvisible(field))
            {
                // WORDSNET-7552 Completely remove the field if its result is preserved from being viewed
                // after all of the dependent fields are updated.
                if (field.IsUpdating && FieldUtil.ProducesBookmarkAsResult(field.Type))
                {
                    field.Updater.DeferRemove(field);
                }
                else
                {
                    Remove(field);
                }
            }
            else
            {
                NodeRange fakeResult = field.GetFakeResult();

                bool hasFakeResult = (fakeResult != null) && !fakeResult.IsVoid && !fakeResult.IsEmpty;

                if (!field.HasSeparator && !hasFakeResult)
                {
                    // No result - remove the field completely.
                    Remove(field);
                }
                else
                {
                    if (hasFakeResult)
                    {
                        field.RemoveFieldResult();
                        field.EnsureSeparator(true);
                        NodeCopier.Copy(fakeResult, field.End);
                    }

                    RemoveFieldCode(field, listener, joinMode);
                }
            }
        }

        private static void RemoveFieldCode(Field field, IFieldReplaceListener listener, NodeJoinMode joinMode)
        {
            listener.BeforeReplaceWithResult();

            SeparateFieldCharsIfNeeded(field, RemainingFieldPart.Result);

            // Remove field start, field code and field separator.
            NodeRemover.Remove(field.Start, true, field.Separator, true, joinMode);
            // Remove field end.
            field.End.Remove();

            NotifyUpdaterIfNeeded(field, false);
        }

        /// <summary>
        /// If the specified field is a separate token of a parent field's code (i.e. it is not double quoted) and its
        /// replacement leads its remainder to join with other tokens, separates the remainder with appropriate separators.
        /// </summary>
        private static void SeparateFieldCharsIfNeeded(Field field, RemainingFieldPart part)
        {
            FieldCharSeparators separators = DefineFieldCharSeparators(field, part);
            if (separators == FieldCharSeparators.None)
                return;

            Inline fieldCharBeforeSeparator = GetFieldCharBeforeSeparator(field, part, separators);
            InsertSeparator(fieldCharBeforeSeparator, separators, true);

            Inline fieldCharAfterSeparator = GetFieldCharAfterSeparator(field, part, separators);
            InsertSeparator(fieldCharAfterSeparator, separators, false);
        }

        /// <summary>
        /// Returns appropriate separators to use while replacing of the specified field.
        /// </summary>
        private static FieldCharSeparators DefineFieldCharSeparators(Field field, RemainingFieldPart part)
        {
            // At the moment this method works properly only for fields being updated, since we can not easily determine
            // whether the field is a part of a parent field's code as well as whether it is double quoted, when it is not
            // being updated (or is being updated selectively). But let's leave it as is until any related client request.
            if (!field.IsUpdating)
                return FieldCharSeparators.None;

            // If the field is located in a place where it can be safely replaced (i.e. it is topmost, or double quoted, or
            // a part of another field's result), do not separate it.
            if (field.UpdateContext.IsSafeReplacingPossible)
                return FieldCharSeparators.None;

            // If the field is a part of a parent formula field's code, then its result must not be enclosed in double quotes.
            // Moreover, in this case the field is surrounded by operator symbols which act like separators themselves,
            // so we do not need to add extra whitespaces either. It is a bit hacky to handle this here, but it works.
            if (field.UpdateContext.ParentContext.Field.Type == FieldType.FieldFormula)
                return FieldCharSeparators.None;

            // If the field is to be completely removed (which is equal to it to be replaced with an empty result),
            // then it should be enclosed in double quotes.
            if (part == RemainingFieldPart.None)
                return FieldCharSeparators.DoubleQuotes;

            // If the field is to be replaced with an empty or multi-word (i.e. containing whitespaces) result,
            // then it should be enclosed in double quotes.
            string fieldResult = field.Result;
            if (!StringUtil.HasChars(fieldResult) || StringUtil.ContainsAnyWhitespaces(fieldResult))
                return FieldCharSeparators.DoubleQuotes;

            // WORDSNET-7614, 7390 REWORKED.
            // If the field's result is to be joined with other parent field's code tokens, separate it with whitespaces.
            FieldCharSeparators separators = FieldCharSeparators.None;
            if (!IsSeparateFieldBoundary(field.Start))
                separators |= FieldCharSeparators.LeadingWhiteSpace;

            if (!IsSeparateFieldBoundary(field.End))
                separators |= FieldCharSeparators.TrailingWhiteSpace;

            return separators;
        }

        /// <summary>
        /// Returns a value indicating whether the specified field boundary (i.e. field start or field end) node is separated
        /// from other parent field's code tokens by any separator other than the boundary node itself.
        /// </summary>
        private static bool IsSeparateFieldBoundary(Node boundary)
        {
            // Go forward from an end boundary, go backward from a start boundary.
            bool isForward = (boundary.NodeType == NodeType.FieldEnd);

            for (Node node = boundary.GetNearestSibling(isForward); node != null; node = node.GetNearestSibling(isForward))
            {
                // Skip zero-length nodes.
                if (NodeUtil.IsZln(node))
                    continue;

                switch (node.NodeType)
                {
                    case NodeType.Run:
                    {
                        string text = node.GetText();

                        // Skip empty runs.
                        if (!StringUtil.HasChars(text))
                            continue;

                        // Check the first or the last character of the run depending on the movement direction.
                        int charIndexToCheck = isForward ? 0 : (text.Length - 1);
                        char charToCheck = text[charIndexToCheck];

                        // Only double quote and whitespace characters are usually considered as field code token separators.
                        return (FieldCodeParser.IsDoubleQuote(charToCheck) || char.IsWhiteSpace(charToCheck));
                    }
                    case NodeType.FieldStart:
                    case NodeType.FieldSeparator:
                    case NodeType.FieldEnd:
                        // Other fields' boundary nodes are considered as field code token separators either.
                        return true;
                    default:
                        // Other nodes are considered as parts of field code tokens.
                        return false;
                }
            }

            // If we have reached a parent paragraph boundary, this means that the specified field boundary is separated
            // by a paragraph break.
            return true;
        }

        /// <summary>
        /// Returns a field char belonging to the given field which should be followed by a separator according to
        /// the specified <see cref="RemainingFieldPart"/>. Returns <c>null</c> if the separation is not required.
        /// </summary>
        private static Inline GetFieldCharBeforeSeparator(Field field, RemainingFieldPart part, FieldCharSeparators separators)
        {
            switch (part)
            {
                case RemainingFieldPart.None:
                    return HasTrailingFieldCharSeparator(separators) ? field.End : null;
                case RemainingFieldPart.Result:
                    return HasLeadingFieldCharSeparator(separators) ? field.Separator : null;
                default:
                    throw new ArgumentOutOfRangeException("part");
            }
        }

        /// <summary>
        /// Returns a field char belonging to the given field which should be preceded by a separator according to
        /// the specified <see cref="RemainingFieldPart"/>. Returns <c>null</c> if the separation is not required.
        /// </summary>
        private static Inline GetFieldCharAfterSeparator(Field field, RemainingFieldPart part, FieldCharSeparators separators)
        {
            switch (part)
            {
                case RemainingFieldPart.None:
                    return HasLeadingFieldCharSeparator(separators) ? field.Start : null;
                case RemainingFieldPart.Result:
                    return HasTrailingFieldCharSeparator(separators) ? field.End : null;
                default:
                    throw new ArgumentOutOfRangeException("part");
            }
        }

        /// <summary>
        /// Inserts a separator run before or after the specified field char.
        /// </summary>
        private static void InsertSeparator(Inline fieldChar, FieldCharSeparators separators, bool isAfter)
        {
            if (fieldChar == null)
                return;

            string separatorText = GetFieldCharSeparatorText(separators);
            RunPr separatorRunPr = GetFieldCharSeparatorRunPr(fieldChar, fieldChar.GetNearestSibling(isAfter));
            Run separatorRun = new Run(fieldChar.Document, separatorText, separatorRunPr);

            fieldChar.ParentNode.Insert(separatorRun, fieldChar, isAfter);
        }

        private static RunPr GetFieldCharSeparatorRunPr(Inline fieldChar, Node sibling)
        {
            Inline inline = sibling as Inline ?? fieldChar;
            return inline.RunPr.Clone();
        }

        private static string GetFieldCharSeparatorText(FieldCharSeparators separators)
        {
            if (IsDoubleQuotes(separators))
                return "\"";

            if (HasLeadingWhiteSpace(separators) || HasTrailingWhiteSpace(separators))
                return ControlChar.Space;

            throw new ArgumentOutOfRangeException("separators");
        }

        private static bool HasLeadingFieldCharSeparator(FieldCharSeparators separators)
        {
            return (IsDoubleQuotes(separators) || HasLeadingWhiteSpace(separators));
        }

        private static bool HasTrailingFieldCharSeparator(FieldCharSeparators separators)
        {
            return (IsDoubleQuotes(separators) || HasTrailingWhiteSpace(separators));
        }

        private static bool IsDoubleQuotes(FieldCharSeparators separators)
        {
            return (separators == FieldCharSeparators.DoubleQuotes);
        }

        private static bool HasLeadingWhiteSpace(FieldCharSeparators separators)
        {
            return ((separators & FieldCharSeparators.LeadingWhiteSpace) != 0);
        }

        private static bool HasTrailingWhiteSpace(FieldCharSeparators separators)
        {
            return ((separators & FieldCharSeparators.TrailingWhiteSpace) != 0);
        }

        private static void NotifyUpdaterIfNeeded(Field field, bool isFieldResultRemoved)
        {
            if (field.IsUpdating)
                field.Updater.NotifyFieldReplaced(field, isFieldResultRemoved);
        }

        /// <summary>
        /// Specifies what characters should be used to separate a field remainder after replacing of
        /// the corresponding field.
        /// </summary>
        [Flags]
        private enum FieldCharSeparators
        {
            None = 0,
            DoubleQuotes = 1,
            LeadingWhiteSpace = 2,
            TrailingWhiteSpace = 4
        }

        /// <summary>
        /// Specifies a field part that should stay in a document instead of the corresponding field
        /// after its replacing.
        /// </summary>
        private enum RemainingFieldPart
        {
            None,
            Result
        }
    }
}
