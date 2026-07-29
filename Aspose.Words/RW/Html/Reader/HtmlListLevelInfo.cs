// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/04/2014 by Anton Savko

using Aspose.Words.Lists;
using Aspose.Words.RW.Html.HtmlList;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Stores some information about list level.
    /// </summary>
    internal class HtmlListLevelInfo
    {
        internal HtmlListLevelInfo(int listLevelNumber, bool isImplicit)
        {
            ListLevelNumber = listLevelNumber;
            IsImplicit = isImplicit;

            StartValue = 1;
            ListTemplate = ListTemplate.BulletDefault;
        }

        internal bool IsImplicit { get; }

        internal ListLevel CurrentListLevel { get; set; }

        internal int ListLevelNumber { get; set; }

        internal int StartValue { get; set; }

        internal int ListLevelNumberValue
        {
            get { return StartValue + ListItemCount - 1; }
        }

        internal HtmlListItemMarker DefaultListItemMarker { get; set; }

        internal ListTemplate ListTemplate { get; set; }

        internal int ListItemCount { get; set; }

        internal HtmlListItemMarker ListItemMarker { get; set; }

        internal bool IsNoneListStyle { get; set; }
    }
}
