// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/03/2016 by Andrey Noskov

using Aspose.Words.Tables;


namespace Aspose.Words.Themes
{
    /// <summary>
    /// Contains methods to deal with table theme shade attributes upon importing and exporting 
    /// to formats that do not support themes.
    /// </summary>
    /// <remarks>
    /// Say if we import one DOCX to an other DOCX with different themes and ImportFormatMode.KeepSourceFormatting is
    /// specified, we have to preserve source formatting and avoid its updating from destination themes - to achieve this
    /// we have to reset all theme attributes. 
    /// </remarks>
    internal class ThemeColorRemover
    {
        /// <summary>
        /// Reset row border theme attributes and theme shading attributes of row.
        /// </summary>
        internal static void ResetRowThemeAttrs(Row row)
        {
            ResetShadingThemeAttrs(row.TablePr.Shading);

            foreach (int borderKey in TablePr.PossibleBorderKeys.Values)
                ResetBorderThemeAttrs((Border)row.TablePr[borderKey]);
        }

        /// <summary>
        /// Reset cell border theme attributes and theme shading attributes of cell.
        /// </summary>
        internal static void ResetCellThemeAttrs(Cell cell)
        {
            ResetShadingThemeAttrs(cell.CellPr.Shading);

            foreach (int borderKey in CellPr.AllBorderKeys)
                ResetBorderThemeAttrs((Border)cell.CellPr[borderKey]);
        }

        /// <summary>
        /// Reset theme shading attributes.
        /// </summary>
        private static void ResetShadingThemeAttrs(Shading shading)
        {
            if (shading != null)
            {
                shading.ThemeShade = string.Empty;
                shading.ThemeTint = string.Empty;
                shading.ThemeColor = string.Empty;

                shading.ThemeFillShade = string.Empty;
                shading.ThemeFillTint = string.Empty;
                shading.ThemeFill = string.Empty;
            }
        }

        /// <summary>
        /// Reset border ThemeShade, ThemeTint and ThemeColor attributes.
        /// </summary>
        private static void ResetBorderThemeAttrs(Border border)
        {
            if (border != null)
            {
                border.ThemeShade = string.Empty;
                border.ThemeTint = string.Empty;
                border.ThemeColorInternal = string.Empty;
            }
        }
    }
}
