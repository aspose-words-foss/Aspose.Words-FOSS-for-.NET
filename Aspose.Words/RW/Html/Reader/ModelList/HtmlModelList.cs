// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/06/2019 by Anton Savko

using Aspose.Words.Lists;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Helps to work with document model lists, track numbering, etc.
    /// </summary>
    internal class HtmlModelList
    {
        internal HtmlModelList(List list, int startedAtListLevelNumber)
        {
            Debug.Assert(list != null);
            Debug.Assert(ListLevel.IsLevelNumberValid(startedAtListLevelNumber));

            List = list;
            StartedAtListLevelNumber = startedAtListLevelNumber;

            Debug.Assert(ListLevel.MinLevel == 0);
            mModelListLevels = new HtmlModelListLevel[ListLevel.MaxLevels];

            for (int i = ListLevel.MinLevel; i < ListLevel.MaxLevels; i++)
            {
                HtmlModelListLevel modelListLevel = new HtmlModelListLevel(List.ListLevels[i]);
                mModelListLevels[i] = modelListLevel;
            }
        }

        internal void AddListItem(int listLevelNumber)
        {
            Debug.Assert(ListLevel.IsLevelNumberValid(listLevelNumber));

            for (int i = ListLevel.MinLevel; i < listLevelNumber; i++)
            {
                HtmlModelListLevel modelListLevel = GetListLevel(i);
                if (!modelListLevel.HasListItems)
                {
                    modelListLevel.AddListItem();
                }
            }

            HtmlModelListLevel currentModelListLevel = GetListLevel(listLevelNumber);
            currentModelListLevel.AddListItem();

            for (int i = listLevelNumber + 1; i < ListLevel.MaxLevels; i++)
            {
                HtmlModelListLevel modelListLevel = GetListLevel(i);
                modelListLevel.DeleteAllListItems();
            }
        }

        internal HtmlModelListLevel GetListLevel(int listLevelNumber)
        {
            Debug.Assert(ListLevel.IsLevelNumberValid(listLevelNumber));

            return mModelListLevels[listLevelNumber];
        }

        internal List List { get; }

        internal int StartedAtListLevelNumber { get; }

        private readonly HtmlModelListLevel[] mModelListLevels;
    }
}
