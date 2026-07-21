// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2019 by Anton Savko

using Aspose.Words.Lists;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// List level identifier.
    /// </summary>
    internal class HtmlListLevelId
    {
        internal HtmlListLevelId(int htmlListLevelNumber)
        {
            Debug.Assert(htmlListLevelNumber >= ListLevel.MinLevel);
            Debug.Assert(ListLevel.MinLevel == 0);

            ListLevelNumber = htmlListLevelNumber % ListLevel.MaxLevels;
            HtmlListLevelNumber = htmlListLevelNumber;
            ListNestedLevel = htmlListLevelNumber / ListLevel.MaxLevels;
        }

        /// <summary>
        /// List level number at which list item should be added.
        /// </summary>
        internal int ListLevelNumber { get; }

        /// <summary>
        /// List level number which was specified by HTML list.
        /// </summary>
        internal int HtmlListLevelNumber { get; }

        /// <summary>
        /// List nested level.
        /// </summary>
        /// <remarks>
        /// When list exhausts all its list levels, another list is created. So we get sequence of nested lists.
        /// Each list has its list nested level.
        /// </remarks>
        internal int ListNestedLevel { get; }
    }
}
