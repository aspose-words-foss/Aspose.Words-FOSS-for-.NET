// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2019 by Anton Savko

namespace Aspose.Words.RW.Html.Reader
{
    internal class HtmlNumberStylePropertyValue : IHtmlModelListLevelPropertyValue
    {
        internal HtmlNumberStylePropertyValue(NumberStyle numberStyle)
        {
            mNumberStyle = numberStyle;
        }

        bool IHtmlModelListLevelPropertyValue.CanModifyListLevel(HtmlModelListLevel modelListLevel)
        {
            return !modelListLevel.IsNumberStyleSet || (mNumberStyle == modelListLevel.NumberStyle);
        }

        void IHtmlModelListLevelPropertyValue.ModifyListLevel(HtmlModelListLevel modelListLevel)
        {
            if (!modelListLevel.IsNumberStyleSet)
            {
                modelListLevel.NumberStyle = mNumberStyle;
            }
        }

        private readonly NumberStyle mNumberStyle;
    }
}
