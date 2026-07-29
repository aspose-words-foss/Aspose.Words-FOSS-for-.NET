// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2019 by Anton Savko

namespace Aspose.Words.RW.Html.Reader
{
    internal interface IHtmlModelListLevelPropertyValue
    {
        /// <summary>
        /// Determines if this value can modify the specified list level.
        /// </summary>
        bool CanModifyListLevel(HtmlModelListLevel modelListLevel);

        /// <summary>
        /// Modifies the specified list level.
        /// </summary>
        void ModifyListLevel(HtmlModelListLevel modelListLevel);
    }
}
