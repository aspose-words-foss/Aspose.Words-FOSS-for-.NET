// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2013 by Alexey Morozov

using Aspose.Collections.Generic;
using Aspose.Words.Lists;
using Aspose.Words.Settings;
using Aspose.Words.Tables;

namespace  Aspose.Words.Validation
{
    /// <summary>
    /// Removes unused styles, lists, list definitions from the document.
    /// </summary>
    internal class DocumentCleaner : IstdVisitor
    {
        private DocumentCleaner(CleanupOptions options)
        {
            mOptions = options;
        }

        internal static void Execute(DocumentBase doc, CleanupOptions options)
        {
            DocumentCleaner cleaner = new DocumentCleaner(options);
            cleaner.ExecuteCore(doc);
        }

        protected override void OnRunPr(RunPr runPr)
        {
            AddStyle(runPr.Istd);
        }

        protected override void OnParaPr(ParaPr paraPr)
        {
            AddStyle(paraPr.Istd);

            if (paraPr.ListId != 0)
                AddList(paraPr.ListId);
        }

        protected override void OnRowPr(TablePr tablePr)
        {
            AddStyle(tablePr.Istd);
        }

        protected override void OnDocPr(DocPr docPr)
        {
            Style style = mDoc.Styles.GetByIstd(docPr.ClickTypeParaStyleIstd, false);
            if (style != null)
                AddStyle(style.Istd);
            style = mDoc.Styles.GetByIstd(docPr.DefaultTableStyleIstd, false);
            if (style != null)
                AddStyle(style.Istd);
        }

        private void ExecuteCore(DocumentBase doc)
        {
            mDoc = doc;

            // Visit all document content and collect used styles/lists.
            Run(doc);

            if(!mOptions.UnusedBuiltinStyles)
                AddBuiltInStyles();

            AddListsReferredByStyle();

            if (mOptions.UnusedLists)
            {
                RemoveUnusedLists();
                RemoveUnusedListDefs();
            }

            if (mOptions.UnusedStyles)
                RemoveUnusedStyles();

            if (mOptions.DuplicateStyle)
                DuplicateStyleRemover.Execute(doc);

            if (mOptions.UnusedStyles || mOptions.DuplicateStyle)
                StyleIstdNormalizer.Execute(doc);
        }

        /// <summary>
        /// Marks style and all related items (list, base style, etc) as used.
        /// </summary>
        private void AddStyle(int istd)
        {
            if (mUsedStyles.Contains(istd))
                return;

            Style style = mDoc.Styles.GetByIstd(istd, false);

            mUsedStyles.Add(istd);

            if (style == null)
                return;

            // Add referred list if needed.
            if (style.ParaPr.ListId != 0)
                AddList(style.ParaPr.ListId);

            // Add linked style if needed.
            if (style.LinkedIstd != StyleIndex.Nil)
                AddStyle(style.LinkedIstd);

            // Recursively add all based styles.
            if (style.BasedOnIstd != StyleIndex.Nil)
                AddStyle(style.BasedOnIstd);
        }

        /// <summary>
        /// Marks list and all related items (list definitions, referred styles, etc) as used.
        /// </summary>
        /// <param name="listId"></param>
        private void AddList(int listId)
        {
            mUsedLists.Add(listId);

            List list = mDoc.Lists.GetListByListId(listId);

            if (list != null)
            {
                ListDef listDef = list.ListDef;
                AddStyle(listDef.ListStyleIstd);
                mUsedListDefs.Add(listDef.ListDefId);
            }
        }

        /// <summary>
        /// Marks all builtin styles as used.
        /// </summary>
        /// <remarks>
        /// AM. I think that we can be more aggressive and delete not used built-in styles as well. 
        /// </remarks>
        private void AddBuiltInStyles()
        {
            foreach (Style style in mDoc.Styles)
                if (style.BuiltIn)
                    AddStyle(style.Istd);
        }

        /// <summary>
        /// Marks all list that refers to used styles.
        /// </summary>
        private void AddListsReferredByStyle()
        {
            foreach (List list in mDoc.Lists)
                if ((list.Style != null) && (mUsedStyles.Contains(list.Style.Istd)))
                    AddList(list.ListId);
        }

        /// <summary>
        /// Removes all styles which are not marked as used.
        /// </summary>
        private void RemoveUnusedStyles()
        {
            int i = 0; 
            while (i < mDoc.Styles.Count)
            {
                Style style = mDoc.Styles[i];
                if (!mUsedStyles.Contains(style.Istd))
                {
                    mDoc.Styles.RemoveCore(style);
                }
                else
                {
                    i++;
                }
            }
        }

        /// <summary>
        /// Removes all lists which are not marked as used.
        /// </summary>
        private void RemoveUnusedLists()
        {
            int i = 0;
            while (i < mDoc.Lists.Count)
            {
                List list = mDoc.Lists[i];
                if (CanRemoveList(list.ListId))
                {
                    mDoc.Lists.RemoveCore(list.ListId);
                }
                else
                {
                    i++;
                }
            }
        }

        /// <summary>
        /// Checks if list is unused and can be safely removed from document.
        /// </summary>
        private bool CanRemoveList(int listId)
        {
            // WORDSNET-18579 If there is only one list and some content has reference to list (even not this one)
            // treat this list as indirectly referred and do not remove from document.
            if (mUsedLists.Count > 0 && (mDoc.Lists.Count == 1))
                return false;

            return !mUsedLists.Contains(listId);
        }

        /// <summary>
        /// Removes all list definitions which are not marked as used.
        /// </summary>
        private void RemoveUnusedListDefs()
        {
            int i = 0;
            while (i < mDoc.Lists.ListDefs.Count)
            {
                ListDef listDef = mDoc.Lists.ListDefs[i];
                if (!mUsedListDefs.Contains(listDef.ListDefId))
                {
                    mDoc.Lists.ListDefs.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        private DocumentBase mDoc;

        /// <summary>
        /// Style indexes are marked as used.
        /// </summary>
        private readonly HashSetGeneric<int> mUsedStyles = new HashSetGeneric<int>();

        /// <summary>
        /// List Ids are marked as used.
        /// </summary>
        private readonly HashSetGeneric<int> mUsedLists = new HashSetGeneric<int>();

        /// <summary>
        /// List definition Ids are marked as used.
        /// </summary>
        private readonly HashSetGeneric<int> mUsedListDefs = new HashSetGeneric<int>();

        private readonly CleanupOptions mOptions;
    }
}
