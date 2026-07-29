// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/05/2016 by Edward Voronov

using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Performs default cleanup strategy.
    /// </summary>
    internal class DefaultFieldCleaner : IFieldCleaner
    {
        bool IFieldCleaner.RemoveEmptyParagraph(Paragraph paragraph)
        {
            if (NeedPostponeParagraphRemoval(paragraph))
            {
                mParagraphsToRemove.Add(paragraph);
                return false;
            }

            paragraph.Remove();
            return true;
        }

        bool IFieldCleaner.RemoveFieldCode(FieldUpdateContext context)
        {
            return RemoveFieldCodeInternal(context);
        }

        void IFieldCleaner.FinalizeCleanup()
        {
            foreach (Field field in mFieldsToReplaceWithResult)
                FieldReplacer.ReplaceWithResult(field);

            foreach (Paragraph paragraph in mParagraphsToRemove)
            {
                if (paragraph.ParentNode != null)
                    paragraph.Remove();
            }
        }

        protected virtual bool RemoveFieldCodeInternal(FieldUpdateContext context)
        {
            if (NeedPostponeReplaceWithResult(context))
            {
                PostponeFieldReplaceWithResult(context.Field);
                return false;
            }

            FieldReplacer.ReplaceWithResult(context.Field);
            return true;
        }

        protected virtual bool NeedPostponeParagraphRemoval(Paragraph paragraph)
        {
            if (paragraph.GetChild(NodeType.BookmarkStart, 0, true) != null)
                return true;

            if (paragraph.GetChild(NodeType.BookmarkEnd, 0, true) != null)
                return true;

            return false;
        }

        protected virtual bool NeedPostponeReplaceWithResult(FieldUpdateContext context)
        {
            return false;
        }

        protected void PostponeFieldReplaceWithResult(Field field)
        {
            mFieldsToReplaceWithResult.Add(field);
        }

        private readonly IList<Paragraph> mParagraphsToRemove = new List<Paragraph>();
        private readonly IList<Field> mFieldsToReplaceWithResult = new List<Field>();
    }
}
