// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/02/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Themes
{
    /// <summary>
    /// Interface providing theme objects.
    /// </summary>
    internal interface IThemeProvider
    {
        string GetFontName(ThemeFontCore themeFont);
        DmlColor GetThemeColor(ThemeColor color);
        DmlFill GetBackgroundFillStyle(int index);
        DmlFill GetFillStyle(int index);
        DmlOutline GetLineStyle(int index);
        EffectStyle GetEffectStyle(int index);

        /// <summary>
        /// Handles theme changes.
        /// </summary>
        void OnChange();
    }
}
