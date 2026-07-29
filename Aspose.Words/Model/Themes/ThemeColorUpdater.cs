// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/10/2017 by Artem Shabarshin

using System.Collections.Generic;
using Aspose.Drawing;
using Aspose.Words.Tables;

namespace Aspose.Words.Themes
{
    /// <summary>
    /// Provides methods for updating document content colors from theme.
    /// </summary>
    /// <remarks>
    /// AM. So far only few document parts are updated. Will add more later on customer requests.
    /// Actually this class is a workaround, good solution is to join ordinal color and theme color 
    /// into one attribute the same way as we did for fonts but it's a very complex task.
    /// </remarks>
    internal class ThemeColorUpdater
    {
        internal static void Update(Document doc)
        {
            Update(doc.Styles, doc.GetThemeInternal());
        }

        /// <summary>
        /// Updates style colors from given theme.
        /// </summary>
        internal static void Update(Style style, Theme theme)
        {
            TableStyle tableStyle = style as TableStyle;
            if (tableStyle != null)
                ApplyTheme(tableStyle.ConditionalStyles.DefinedStyles, theme);
            ApplyTheme(style.ParaPr, theme);
        }

        /// <summary>
        /// Updates background/foreground table style shading colors from theme colors if specified.
        /// </summary>
        private static void Update(StyleCollection styles, Theme theme)
        {
            if ((styles == null) || (theme == null))
                return;

            foreach (Style style in styles)
                Update(style, theme);
        }

        /// <summary>
        /// Updates background/foreground row shading and border colors from theme colors if specified.
        /// If ImportFormatMode.UseDestinationStyles is specified we have to permanently modify direct color formatting of row
        /// using destination document themes. 
        /// </summary>
        internal static void Update(Row row)
        {
            Theme docTheme = row.Document.GetThemeInternal();

            if (docTheme == null)
                return;

            TablePr tablePr = row.TablePr;

            if (tablePr.Contains(TableAttr.Shading))
                UpdateShadingColorFromTheme(tablePr.Shading, docTheme);

            foreach (int borderKey in TablePr.PossibleBorderKeys.Values)
                UpdateBorderColorFromTheme((Border)tablePr[borderKey], docTheme);
        }

        /// <summary>
        /// Updates background/foreground cell shading and border colors from theme colors if specified.
        /// If ImportFormatMode.UseDestinationStyles is specified we have to permanently modify direct color formatting of cell
        /// using destination document themes. 
        /// </summary>
        internal static void Update(Cell cell)
        {
            Theme docTheme = cell.Document.GetThemeInternal();

            if (docTheme == null)
                return;

            CellPr cellPr = cell.CellPr;

            if (cellPr.Contains(CellAttr.Shading))
                UpdateShadingColorFromTheme(cellPr.Shading, docTheme);

            foreach (int borderKey in CellPr.AllBorderKeys)
                UpdateBorderColorFromTheme((Border)cellPr[borderKey], docTheme);
        }

        /// <summary>
        /// Updates Color attribute from document theme if needed for run.
        /// </summary>
        internal static void Update(RunPr runPr, Theme theme)
        {
            // ThemeColor should be specified.
            object themeColor = runPr[FontAttr.ThemeColor];
            if (themeColor == null)
                return;

            DrColor newColor =
                OfficeColor.Resolve(themeColor, runPr[FontAttr.ThemeTint], runPr[FontAttr.ThemeShade], theme);

            // Don't want to update colors too often so let some threshold.
            // WORDSNET-16424 Do not update color when resolved color is empty.
            if ((newColor != null) && (!newColor.IsEmpty) && (MaxComponentDelta(runPr.Color, newColor) > 3))
                runPr.Color = newColor;
        }

        /// <summary>
        /// Updates background/foreground shading colors.
        /// </summary>
        private static void UpdateShadingColorFromTheme(Shading shading, Theme docTheme)
        {
            // Apply background theme color and shading.
            DrColor finalThemeColor = OfficeColor.Resolve(shading.ThemeFill, shading.ThemeFillTint, shading.ThemeFillShade, docTheme);
            if (finalThemeColor != null)
                shading.BackgroundPatternColorInternal = finalThemeColor;

            // Apply foreground theme color and shading.
            finalThemeColor = OfficeColor.Resolve(shading.ThemeColor, shading.ThemeTint, shading.ThemeShade, docTheme);
            if (finalThemeColor != null)
                shading.ForegroundPatternColorInternal = finalThemeColor;
        }

        /// <summary>
        /// Updates border color, applies theme shade and tint.
        /// </summary>
        private static void UpdateBorderColorFromTheme(Border border, Theme docTheme)
        {
            if ((docTheme == null) || (border == null))
                return;

            DrColor finalThemeColor = OfficeColor.Resolve(border.ThemeColorInternal, border.ThemeTint, border.ThemeShade, docTheme);

            if (finalThemeColor != null)
                border.ColorInternal = finalThemeColor;
        }

        /// <summary>
        /// Applies a theme to a table style.
        /// </summary>
        private static void ApplyTheme(IEnumerable<ConditionalStyle> conditionalStyles, Theme theme)
        {
            foreach (ConditionalStyle conditionalStyle in conditionalStyles)
            {
                if ((conditionalStyle != null) && 
                    conditionalStyle.HasCellFormatting && 
                    (conditionalStyle.CellPr.Shading != null))
                    UpdateShadingColorFromTheme(conditionalStyle.CellPr.Shading, theme);
            }
        }

        /// <summary>
        /// Applies a theme to any style.
        /// </summary>
        private static void ApplyTheme(ParaPr styleParaPr, Theme theme)
        {
            foreach (KeyValuePair<BorderType, int> borderKey in ParaPr.PossibleBorderKeys)
                UpdateBorderColorFromTheme((Border)styleParaPr[borderKey.Value], theme);
        }

        /// <summary>
        /// Calculates max difference in RGB components.
        /// </summary>
        private static int MaxComponentDelta(DrColor color1, DrColor color2)
        {
            int maxR = System.Math.Abs(color1.R - color2.R);
            int maxG = System.Math.Abs(color1.G - color2.G);
            int maxB = System.Math.Abs(color1.B - color2.B);

            return System.Math.Max(System.Math.Max(maxR, maxG), maxB);
        }
    }
}
