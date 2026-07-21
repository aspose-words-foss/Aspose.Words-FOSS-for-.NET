// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2019 by Anton Savko

namespace Aspose.Words.RW.Html.Reader
{
    internal class HtmlNumberFormatPropertyValue : IHtmlModelListLevelPropertyValue
    {
        internal HtmlNumberFormatPropertyValue(string numberFormat)
        {
            mNumberFormat = numberFormat;
        }

        bool IHtmlModelListLevelPropertyValue.CanModifyListLevel(HtmlModelListLevel modelListLevel)
        {
            return !modelListLevel.IsNumberFormatSet || (mNumberFormat == modelListLevel.NumberFormat);
        }

        void IHtmlModelListLevelPropertyValue.ModifyListLevel(HtmlModelListLevel modelListLevel)
        {
            if (!modelListLevel.IsNumberFormatSet)
            {
                modelListLevel.NumberFormat = mNumberFormat;
            }
        }

        private readonly string mNumberFormat;
    }
}
