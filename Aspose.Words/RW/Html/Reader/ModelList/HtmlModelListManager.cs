// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/06/2019 by Anton Savko

using System.Collections.Generic;
using Aspose.Words.Lists;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Helps to work with created lists.
    /// </summary>
    internal class HtmlModelListManager
    {
        internal HtmlModelListManager(Document document)
        {
            Debug.Assert(document != null);

            mDocument = document;
            mModelLists = new Dictionary<int, List<HtmlModelList>>();
        }

        /// <summary>
        /// Adds a new list item to a model list. The model list is chosen automatically. If no list fits, a new one is created.
        /// </summary>
        /// <returns>The chosen model list.</returns>
        internal HtmlModelList AddListItem(ListTemplate listTemplate, HtmlListItemBase listItem)
        {
            HtmlModelList modelList = GetList(listTemplate, listItem);
            listItem.AddToList(modelList);

            return modelList;
        }

        internal void DeleteAllLists()
        {
            mModelLists.Clear();
        }

        private HtmlModelList GetList(ListTemplate listTemplate, HtmlListItemBase listItem)
        {
            List<HtmlModelList> modelLists;
            if (!mModelLists.TryGetValue(listItem.ListLevelId.ListNestedLevel, out modelLists))
            {
                modelLists = new List<HtmlModelList>();
                mModelLists.Add(listItem.ListLevelId.ListNestedLevel, modelLists);
            }

            // Currently we aren't concerned about preserving list structure, when importing lists.
            // But using latest created list first should provide better result, so using reverse enumeration here.
            for (int i = modelLists.Count - 1; i >= 0; i--)
            {
                HtmlModelList modelList = modelLists[i];
                if ((listItem.ListLevelId.ListLevelNumber >= modelList.StartedAtListLevelNumber) &&
                    listItem.CanAddToList(modelList))
                {
                    // WORDSNET-27415 - Raise priority of the last used list.
                    modelLists.Remove(modelList);
                    modelLists.Add(modelList);

                    return modelList;
                }
            }

            List newList = mDocument.Lists.Add(listTemplate);
            HtmlModelList newModelList = new HtmlModelList(newList, listItem.ListLevelId.ListLevelNumber);
            modelLists.Add(newModelList);

            return newModelList;
        }

        private readonly Document mDocument;

        /// <summary>
        /// Maps list nested level <see cref="HtmlListLevelId.ListNestedLevel"/> to list of model lists.
        /// </summary>
        private readonly Dictionary<int, List<HtmlModelList>> mModelLists;
    }
}
