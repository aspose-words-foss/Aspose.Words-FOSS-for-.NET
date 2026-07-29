// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/06/2019 by Anton Savko

using Aspose.Collections;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// A base class for classes that represent list items.
    /// </summary>
    internal abstract class HtmlListItemBase
    {
        protected HtmlListItemBase(HtmlListLevelId listLevelId, HtmlMarkerType markerType, string listLabelString)
        {
            Debug.Assert(listLevelId != null);

            ListLevelId = listLevelId;
            MarkerType = markerType;
            ListLabelString = listLabelString;

            mPropertyValues = new IntToObjDictionary<HtmlModelListLevelPropertyValueCollection>();
        }

        /// <summary>
        /// Formats the list.
        /// </summary>
        [JavaAttributes.JavaThrows(true)]
        internal abstract void PostModifyList(HtmlModelList modelList);

        protected static void RemoveFontAttributesForOrderedList(RunPr runPr)
        {
            // WORDSNET-3456 Ordered lists shouldn't have any specified fonts for labels. They are applicable for bullets.
            runPr.Remove(FontAttr.NameAscii);
            runPr.Remove(FontAttr.NameFarEast);
            runPr.Remove(FontAttr.NameOther);
            runPr.Remove(FontAttr.NameBi);
        }

        /// <summary>
        /// Determines if list item can be added to the specified list.
        /// </summary>
        internal bool CanAddToList(HtmlModelList modelList)
        {
            IntToObjDictionary<HtmlModelListLevelPropertyValueCollection>.Enumerator enumerator =
                mPropertyValues.GetEnumerator();

            while (enumerator.MoveNext())
            {
                HtmlModelListLevel modelListLevel = modelList.GetListLevel(enumerator.CurrentKey);
                HtmlModelListLevelPropertyValueCollection propertyValues = enumerator.CurrentValue;
                if (!propertyValues.CanModifyListLevel(modelListLevel))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Adds list item to the specified list.
        /// </summary>
        [JavaAttributes.JavaThrows(true)]
        internal void AddToList(HtmlModelList modelList)
        {
            modelList.AddListItem(ListLevelId.ListLevelNumber);

            IntToObjDictionary<HtmlModelListLevelPropertyValueCollection>.Enumerator enumerator =
                mPropertyValues.GetEnumerator();

            while (enumerator.MoveNext())
            {
                HtmlModelListLevel modelListLevel = modelList.GetListLevel(enumerator.CurrentKey);
                HtmlModelListLevelPropertyValueCollection propertyValues = enumerator.CurrentValue;
                propertyValues.ModifyListLevel(modelListLevel);
            }

            PostModifyList(modelList);
        }

        internal HtmlListLevelId ListLevelId { get; }

        internal string ListLabelString { get; }

        /// <summary>
        /// Determines if list item has a marker specified by ::before or ::after pseudo-elements.
        /// </summary>
        internal bool IsPseudoElement
        {
            get { return MarkerType == HtmlMarkerType.PseudoElement; }
        }

        protected void SetListLevelType(HtmlModelListLevelType modelListLevelType)
        {
            GetPropertyValues(ListLevelId.ListLevelNumber).Add(new HtmlListLevelTypePropertyValue(modelListLevelType));
        }

        protected void SetNumberFormat(string numberFormat)
        {
            GetPropertyValues(ListLevelId.ListLevelNumber).Add(new HtmlNumberFormatPropertyValue(numberFormat));
        }

        protected void SetNumberStyle(int listLevelNumber, NumberStyle numberStyle)
        {
            GetPropertyValues(listLevelNumber).Add(new HtmlNumberStylePropertyValue(numberStyle));
        }

        protected void SetNumberValue(int listLevelNumber, int numberValue)
        {
            if (listLevelNumber != ListLevelId.ListLevelNumber)
            {
                GetPropertyValues(listLevelNumber).Add(new HtmlCurrentNumberValuePropertyValue(numberValue));
            }
            else
            {
                GetPropertyValues(listLevelNumber).Add(new HtmlNextNumberValuePropertyValue(numberValue));
            }
        }

        protected void SetPictureBulletId(int pictureBulletId)
        {
            GetPropertyValues(ListLevelId.ListLevelNumber).Add(new HtmlPictureBulletIdPropertyValue(pictureBulletId));
        }

        protected HtmlMarkerType MarkerType { get; }

        private HtmlModelListLevelPropertyValueCollection GetPropertyValues(int modelListLevelNumber)
        {
            if (!mPropertyValues.ContainsKey(modelListLevelNumber))
            {
                HtmlModelListLevelPropertyValueCollection propertyValues =
                    new HtmlModelListLevelPropertyValueCollection();

                mPropertyValues.Add(modelListLevelNumber, propertyValues);

                return propertyValues;
            }

            return mPropertyValues[modelListLevelNumber];
        }

        private readonly IntToObjDictionary<HtmlModelListLevelPropertyValueCollection> mPropertyValues;
    }
}
