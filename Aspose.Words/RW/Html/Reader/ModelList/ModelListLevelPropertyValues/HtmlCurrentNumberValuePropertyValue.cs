// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2019 by Anton Savko

namespace Aspose.Words.RW.Html.Reader
{
    internal class HtmlCurrentNumberValuePropertyValue : IHtmlModelListLevelPropertyValue
    {
        internal HtmlCurrentNumberValuePropertyValue(int currentNumberValue)
        {
            mCurrentNumberValue = currentNumberValue;
        }

        bool IHtmlModelListLevelPropertyValue.CanModifyListLevel(HtmlModelListLevel modelListLevel)
        {
            return !modelListLevel.IsNumberValueSet || (mCurrentNumberValue == modelListLevel.NumberValue);
        }

        void IHtmlModelListLevelPropertyValue.ModifyListLevel(HtmlModelListLevel modelListLevel)
        {
            if (!modelListLevel.IsNumberValueSet)
            {
                modelListLevel.NumberValue = mCurrentNumberValue;
            }
        }

        private readonly int mCurrentNumberValue;
    }
}
