// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2019 by Anton Savko

namespace Aspose.Words.RW.Html.Reader
{
    internal class HtmlListLevelTypePropertyValue : IHtmlModelListLevelPropertyValue
    {
        internal HtmlListLevelTypePropertyValue(HtmlModelListLevelType listLevelType)
        {
            mListLevelType = listLevelType;
        }

        bool IHtmlModelListLevelPropertyValue.CanModifyListLevel(HtmlModelListLevel modelListLevel)
        {
            return !modelListLevel.IsListLevelTypeSet || (mListLevelType == modelListLevel.ListLevelType);
        }

        void IHtmlModelListLevelPropertyValue.ModifyListLevel(HtmlModelListLevel modelListLevel)
        {
            if (!modelListLevel.IsListLevelTypeSet)
            {
                modelListLevel.ListLevelType = mListLevelType;
            }
        }

        private readonly HtmlModelListLevelType mListLevelType;
    }
}
