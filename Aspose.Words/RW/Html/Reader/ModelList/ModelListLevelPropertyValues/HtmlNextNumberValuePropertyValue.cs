// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2019 by Anton Savko

namespace Aspose.Words.RW.Html.Reader
{
    internal class HtmlNextNumberValuePropertyValue : IHtmlModelListLevelPropertyValue
    {
        internal HtmlNextNumberValuePropertyValue(int nextNumberValue)
        {
            mNextNumberValue = nextNumberValue;
        }

        bool IHtmlModelListLevelPropertyValue.CanModifyListLevel(HtmlModelListLevel modelListLevel)
        {
            return !modelListLevel.IsNumberValueSet || (mNextNumberValue == (modelListLevel.NumberValue + 1));
        }

        void IHtmlModelListLevelPropertyValue.ModifyListLevel(HtmlModelListLevel modelListLevel)
        {
            if (!modelListLevel.IsNumberValueSet)
            {
                modelListLevel.NumberValue = mNextNumberValue;
            }
        }

        private readonly int mNextNumberValue;
    }
}
