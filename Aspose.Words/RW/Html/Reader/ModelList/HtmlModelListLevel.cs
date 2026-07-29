// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/06/2019 by Anton Savko

using Aspose.Words.Lists;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Helps to work with model list levels, track numbering, etc.
    /// </summary>
    internal class HtmlModelListLevel
    {
        internal HtmlModelListLevel(ListLevel listLevel)
        {
            Debug.Assert(listLevel != null);

            ListLevel = listLevel;
        }

        internal void AddListItem()
        {
            mListItemCount++;
        }

        internal void DeleteAllListItems()
        {
            mListItemCount = 0;
        }

        internal ListLevel ListLevel { get; private set; }

        internal bool HasListItems
        {
            get { return mListItemCount > 0; }
        }

        internal HtmlModelListLevelType ListLevelType
        {
            get { return mListLevelType; }
            set
            {
                Debug.Assert(!IsListLevelTypeSet);

                mListLevelType = value;
                IsListLevelTypeSet = true;
            }
        }

        internal bool IsListLevelTypeSet { get; private set; }

        internal string NumberFormat
        {
            get { return ListLevel.NumberFormat; }
            set
            {
                Debug.Assert(!IsNumberFormatSet);

                ListLevel.SetNumberFormatSafe(value);
                IsNumberFormatSet = true;
            }
        }

        internal bool IsNumberFormatSet { get; private set; }

        internal int NumberValue
        {
            get { return ListLevel.StartAt + mListItemCount - 1; }
            set
            {
                Debug.Assert(!IsNumberValueSet);

                // WORDSNET-20444 Ignore "start at" values that are not accepted by MS Word. This is what MS Word does when
                // it loads HTML.
                int startAtValue = value - mListItemCount + 1;
                if (ListLevel.IsStartAtValid(startAtValue))
                {
                    ListLevel.StartAt = startAtValue;
                    IsNumberValueSet = true;
                }
            }
        }

        internal bool IsNumberValueSet { get; private set; }

        internal NumberStyle NumberStyle
        {
            get { return ListLevel.NumberStyle; }
            set
            {
                Debug.Assert(!IsNumberStyleSet);

                ListLevel.NumberStyle = value;
                IsNumberStyleSet = true;
            }
        }

        internal bool IsNumberStyleSet { get; private set; }

        internal int PictureBulletId
        {
            get { return ListLevel.PictureBulletId; }
            set
            {
                Debug.Assert(!IsPictureBulletIdSet);

                ListLevel.PictureBulletId = value;
                IsPictureBulletIdSet = true;
            }
        }

        internal bool IsPictureBulletIdSet { get; private set; }

        private HtmlModelListLevelType mListLevelType;
        private int mListItemCount;
    }
}
