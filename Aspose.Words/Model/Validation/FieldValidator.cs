// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/07/2010 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Words.Fields;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// This class is invoked by <see cref="DocumentValidator"/> to correct any invalid fields before saving a document.
    /// As I just have started this class it only contains some basics checks and corrections.
    /// Add more checks and corrections here as needed.
    /// </summary>
    internal class FieldValidator
    {
        internal FieldValidator(IWarningCallback warningCallback, SaveFormat saveFormat)
        {
            mSaveFormat = saveFormat;
            mWarningCallback = warningCallback;
        }

        internal void VisitDocumentEnd()
        {
            // One situation when we can have some field chars in the stack is when a field did not have a field end.
            while (mFieldChars.Count > 0)
            {
                FieldBundle bundle = PopFieldBundleSafe();

                FieldStart start = bundle.Start;
                FieldSeparator separator = bundle.Separator;
                FieldEnd end = bundle.End;

                if ((start != null) && (end == null))
                {
                    // Field has no end. Mmm what should we do.
                    if (start.FieldType != FieldType.FieldNone)
                    {
                        // If field type is valid, there is a possibility we can correct the field.
                        // Let's insert an end node. The most difficult question is where to insert.
                        // Let's insert it at the end of the separator or start node's parent.
                        // Hit by TestDefect12929.
                        bool hasSeparator = separator != null;
                        CompositeNode parent = hasSeparator ? separator.ParentNode : start.FirstNonMarkupParentNode;
                        end = new FieldEnd(start.Document,  start.RunPr.Clone(), start.FieldType, hasSeparator);
                        parent.AppendChild(end);
                    }
                    else
                    {
                        // Field type is none. This is some really bugged field, lets delete the field chars.
                        // Any field code or result will remain as plain text which is okay for us.
                        // Hit by TestDefect5221.
                        Warn(WarningType.DataLoss, WarningSource.Validator, WarningStrings.FieldValidatorFieldNone);
                        bundle.RemoveFieldNodes();
                    }
                }
                else if ((start == null) && (end == null))
                {
                    // andrnosk: RESILIENCY WORDSNET-5330 Remove standalone FieldSeparators.
                    Warn(WarningType.DataLoss, WarningSource.Validator, WarningStrings.FieldValidatorNoStartEnd);
                    bundle.RemoveFieldNodes();
                }
            }
        }

        internal void VisitFieldStart(FieldStart fieldStart)
        {
            if (NeedReplaceWithResult(fieldStart))
            {
                FieldBundle bibliographyBundle = FieldBundle.GetFieldBundle(fieldStart);
                FieldReplacer.ReplaceWithResult(new Field(bibliographyBundle));
                return;
            }

            mFieldChars.Push(fieldStart);

            if (FieldUtil.IsFormField(fieldStart.FieldType))
                mFormFieldValidationCounter++;
        }

        internal void VisitFieldSeparator(FieldSeparator fieldSeparator)
        {
            mFieldChars.Push(fieldSeparator);
        }

        internal void VisitFormField(FormField formField)
        {
            if (mFormFieldValidationCounter != 0)
                return;

            // WORDSNET-8448 mFormFieldValidationCounter is 0, because there was no FieldStart.
            // We should remove formField, like it happens in MSWord.
            Warn(WarningType.DataLoss, WarningSource.Validator, WarningStrings.FieldValidatorRemovedInvalidFormField);
            formField.Remove();
        }

        internal void VisitFieldEnd(FieldEnd fieldEnd)
        {
            mFieldChars.Push(fieldEnd);

            FieldBundle bundle = PopFieldBundleSafe();

            // This is a simplistic validation that deletes field nodes if the field has no start node.
            // If there was any field code or result text, it just remains in the document as plain text.
            // Hit by TestDefect5221.
            if (bundle.Start == null)
            {
                Warn(WarningType.DataLoss, WarningSource.Validator, WarningStrings.FieldValidatorNoStart);
                bundle.RemoveFieldNodes();
            }

            if (FieldUtil.IsFormField(fieldEnd.FieldType))
                mFormFieldValidationCounter--;

            if ((bundle.Start != null) && (bundle.End != null))
            {
                // WORDSNET-7346 Update dirty/locked flags for all field parts.
                bundle.UpdateDirtyLocked();

                DetermineFieldType(bundle);

                RemoveCrossStoryFieldNodes(bundle);
            }

            // Add more validations here when needed.
        }

        private void RemoveCrossStoryFieldNodes(FieldBundle bundle)
        {
            // WORDSNET-7641 Field starts in header and ends in body which causes Office Validator complains.
            // AM. It seems that field must not go outside header/footer.
            // Because it's just assumption type is limited to FieldType.None as it is in problematic document.
            // andrnosk: WORDSNET-7910 One more problematic document with FieldType.FieldPage.
            // WORDSNET-7913 Lets do this correction for any field type.
            Node startStory = bundle.Start.GetStoryAncestor(NodeType.Any);

            // WORDSNET-23439 Document contains field which spreads for two shapes.
            if ((startStory.NodeType != NodeType.HeaderFooter) && (startStory.NodeType != NodeType.Shape))
                return;

            Node endStory = bundle.End.GetStoryAncestor(NodeType.Any);
            if (startStory == endStory)
                return;

            Warn(WarningType.DataLoss, WarningSource.Validator, WarningStrings.FieldValidatorDifferentStoriesField);
            bundle.RemoveFieldNodes();
        }

        private void DetermineFieldType(FieldBundle bundle)
        {
            bundle.DetermineFieldType();

            if (mSaveFormat != SaveFormat.Doc)
                return;

            if (bundle.FieldType != FieldType.FieldNone)
                return;

            FieldUnknown field = (FieldUnknown)bundle.GetField();
            if (!field.IsBookmarkRef())
                return;

            bundle.FieldType = FieldType.FieldRefNoKeyword;
        }

        private FieldBundle PopFieldBundleSafe()
        {
            FieldEnd fieldEnd = (FieldEnd)mFieldChars.PopIfInstanceOf(typeof(FieldEnd));
            FieldSeparator fieldSeparator = (FieldSeparator)mFieldChars.PopIfInstanceOf(typeof(FieldSeparator));
            FieldStart fieldStart = (FieldStart)mFieldChars.PopIfInstanceOf(typeof(FieldStart));

            return new FieldBundle(fieldStart, fieldSeparator, fieldEnd);
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private void Warn(WarningType type, WarningSource source, string description)
        {
            if (mWarningCallback != null)
                mWarningCallback.Warning(new WarningInfo(type, source, description));
        }

        /// <summary>
        /// Returns "true" when field must be replaced with it's result.
        /// </summary>
        private bool NeedReplaceWithResult(FieldChar fieldChar)
        {
            // WORDSNET-21460 Replace "Bibliography" field with it's result.
            // DS: Strange behavior, but the Word replaces problematic field with the result while saving to DOC/WML.
            // It's better to do it in writers and do not change the model. But lets postpone for a while.
            if (fieldChar.FieldType != FieldType.FieldBibliography)
                return false;

            switch (mSaveFormat)
            {
                case SaveFormat.Doc:
                case SaveFormat.WordML:
                    return true;
                default:
                    return false;
            }
        }

        private readonly SaveFormat mSaveFormat;
        private readonly IWarningCallback mWarningCallback;
        private readonly Stack<FieldChar> mFieldChars = new Stack<FieldChar>();

        /// <summary>
        /// Used to verify that form fields contain FormField node inside.
        /// </summary>
        private int mFormFieldValidationCounter = 0;
    }
}
