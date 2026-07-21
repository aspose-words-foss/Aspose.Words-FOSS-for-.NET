// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/02/2020 by Ilya Egorov

using System.Text.RegularExpressions;
using Aspose.Collections;
using Aspose.Drawing;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Provides methods for obtaining <see cref="DrColor"/> from a format string.
    /// </summary>

    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal class DmlChartFormatCodeColors
    {
        /// <summary>
        /// Gets <see cref="DrColor"/> from the specified string.
        /// </summary>
        /// <remarks>
        /// The text color can be represented by the name or color index (3.8.31 OOXML) in square brackets in the section.
        /// </remarks>
        /// <param name="formatCode">The specified string</param>
        /// <returns><see cref="DrColor"/></returns>
        internal static DrColor GetColor(string formatCode)
        {
            Match match = gSquareBracketsCodeRegex.Match(formatCode);
            string stringWithColor = (match.Length == 0) ? string.Empty : match.Value.TrimStart('[').TrimEnd(']');
            stringWithColor = DmlChartRenderingUtil.SpaceRegex.Replace(stringWithColor, string.Empty);
            
            DrColor color = gFormatCodeColors[stringWithColor];
            return (color != null) ? color : DrColor.Empty;
        }

        /// <summary>
        /// Gets the value color according to the specified value and the specified format code.
        /// </summary>
        /// <param name="value">The specified value</param>
        /// <param name="formatCode">The specified format code</param>
        /// <returns><see cref="DrColor"/></returns>
        internal static DrColor GetValueColor(double value, string formatCode)
        {
            DrColor color;
            string[] formats = formatCode.Split(DmlChartRenderingUtil.SectionSeparator);

            if (MathUtil.IsZero(value))
                color = (formats.Length > 2) ? GetColor(formats[2]) : GetColor(formats[0]);
            else if (value > 0 )
                color = GetColor(formats[0]);
            else 
                color = (formats.Length > 1) ? GetColor(formats[1]) : GetColor(formatCode);
          
            return color;
        }

        /// <summary>
        /// Regex matches if string contains text like '[....]' representing color settings.
        /// </summary>
        private static readonly Regex gSquareBracketsCodeRegex = new Regex(@"\[\w*\s*\d*\]");

        private static readonly StringToObjDictionary<DrColor> gFormatCodeColors;

        static DmlChartFormatCodeColors()
        {
            gFormatCodeColors = new StringToObjDictionary<DrColor>(false);

            // Colors by Name.
            gFormatCodeColors.Add("Black", DrColor.Black);
            gFormatCodeColors.Add("Green", DrColor.Green);
            gFormatCodeColors.Add("White", DrColor.White);
            gFormatCodeColors.Add("Blue", DrColor.Blue);
            gFormatCodeColors.Add("Magenta", DrColor.Magenta);
            gFormatCodeColors.Add("Yellow", DrColor.Yellow);
            gFormatCodeColors.Add("Cyan", DrColor.Cyan);
            gFormatCodeColors.Add("Red", DrColor.Red);
            
            // Colors by index.
            gFormatCodeColors.Add("Color1", DrColor.Black);
            gFormatCodeColors.Add("Color2", DrColor.White);
            gFormatCodeColors.Add("Color3", DrColor.Red);
            gFormatCodeColors.Add("Color4", DrColor.Lime);
            gFormatCodeColors.Add("Color5", DrColor.Blue);
            gFormatCodeColors.Add("Color6", DrColor.Yellow);
            gFormatCodeColors.Add("Color7", DrColor.Magenta);
            gFormatCodeColors.Add("Color8", DrColor.Cyan);
            gFormatCodeColors.Add("Color9", DrColor.Maroon);
            gFormatCodeColors.Add("Color10", DrColor.Green);
            gFormatCodeColors.Add("Color11", DrColor.Navy);
            gFormatCodeColors.Add("Color12", DrColor.Olive);
            gFormatCodeColors.Add("Color13", DrColor.Purple);
            gFormatCodeColors.Add("Color14", DrColor.Teal);
            gFormatCodeColors.Add("Color15", DrColor.Silver);
            gFormatCodeColors.Add("Color16", DrColor.Gray);
            gFormatCodeColors.Add("Color17", new DrColor(ArgbColor17));
            gFormatCodeColors.Add("Color18", new DrColor(ArgbColor18));
            gFormatCodeColors.Add("Color19", new DrColor(ArgbColor19));
            gFormatCodeColors.Add("Color20", new DrColor(ArgbColor20));
            gFormatCodeColors.Add("Color21", new DrColor(ArgbColor21));
            gFormatCodeColors.Add("Color22", new DrColor(ArgbColor22));
            gFormatCodeColors.Add("Color23", new DrColor(ArgbColor23));
            gFormatCodeColors.Add("Color24", new DrColor(ArgbColor24));
            gFormatCodeColors.Add("Color25", new DrColor(ArgbColor25));
            gFormatCodeColors.Add("Color26", DrColor.Magenta);
            gFormatCodeColors.Add("Color27", DrColor.Yellow);
            gFormatCodeColors.Add("Color28", DrColor.Cyan);
            gFormatCodeColors.Add("Color29", DrColor.Purple);
            gFormatCodeColors.Add("Color30", DrColor.Maroon);
            gFormatCodeColors.Add("Color31", DrColor.Teal);
            gFormatCodeColors.Add("Color32", DrColor.Blue);
            gFormatCodeColors.Add("Color33", new DrColor(ArgbColor33));
            gFormatCodeColors.Add("Color34", new DrColor(ArgbColor34));
            gFormatCodeColors.Add("Color35", new DrColor(ArgbColor35));
            gFormatCodeColors.Add("Color36", new DrColor(ArgbColor36));
            gFormatCodeColors.Add("Color37", new DrColor(ArgbColor37));
            gFormatCodeColors.Add("Color38", new DrColor(ArgbColor38));
            gFormatCodeColors.Add("Color39", new DrColor(ArgbColor39));
            gFormatCodeColors.Add("Color40", new DrColor(ArgbColor40));
            gFormatCodeColors.Add("Color41", new DrColor(ArgbColor41));
            gFormatCodeColors.Add("Color42", new DrColor(ArgbColor42));
            gFormatCodeColors.Add("Color43", new DrColor(ArgbColor43));
            gFormatCodeColors.Add("Color44", new DrColor(ArgbColor44));
            gFormatCodeColors.Add("Color45", new DrColor(ArgbColor45));
            gFormatCodeColors.Add("Color46", new DrColor(ArgbColor46));
            gFormatCodeColors.Add("Color47", new DrColor(ArgbColor47));
            gFormatCodeColors.Add("Color48", new DrColor(ArgbColor48));
            gFormatCodeColors.Add("Color49", new DrColor(ArgbColor49));
            gFormatCodeColors.Add("Color50", new DrColor(ArgbColor50));
            gFormatCodeColors.Add("Color51", new DrColor(ArgbColor51));
            gFormatCodeColors.Add("Color52", new DrColor(ArgbColor52));
            gFormatCodeColors.Add("Color53", new DrColor(ArgbColor53));
            gFormatCodeColors.Add("Color54", new DrColor(ArgbColor54));
            gFormatCodeColors.Add("Color55", new DrColor(ArgbColor55));
            gFormatCodeColors.Add("Color56", new DrColor(ArgbColor56));
        }

        // Uint values of colors.
        private const int ArgbColor17= unchecked((int)0xff9999ff);
        private const int ArgbColor18 = unchecked((int)0xff993366);
        private const int ArgbColor19 = unchecked((int)0xffffffcc);
        private const int ArgbColor20 = unchecked((int)0xffccffff);
        private const int ArgbColor21 = unchecked((int)0xff660066);
        private const int ArgbColor22 = unchecked((int)0xffff8080);
        private const int ArgbColor23 = unchecked((int)0xff0066CC);
        private const int ArgbColor24 = unchecked((int)0xffccccff);
        private const int ArgbColor25 = unchecked((int)0xff000080);
        private const int ArgbColor33 = unchecked((int)0xff00ccff);
        private const int ArgbColor34 = unchecked((int)0xffccffff);
        private const int ArgbColor35 = unchecked((int)0xffccffcc);
        private const int ArgbColor36 = unchecked((int)0xffffff99);
        private const int ArgbColor37 = unchecked((int)0xff99ccff);
        private const int ArgbColor38 = unchecked((int)0xffff99cc);
        private const int ArgbColor39 = unchecked((int)0xffcc99ff);
        private const int ArgbColor40 = unchecked((int)0xffffcc99);
        private const int ArgbColor41 = unchecked((int)0xff3366ff);
        private const int ArgbColor42 = unchecked((int)0xff33cccc);
        private const int ArgbColor43 = unchecked((int)0xff99cc00);
        private const int ArgbColor44 = unchecked((int)0xffffcc00);
        private const int ArgbColor45 = unchecked((int)0xffff9900);
        private const int ArgbColor46 = unchecked((int)0xffff6600);
        private const int ArgbColor47 = unchecked((int)0xff666699);
        private const int ArgbColor48 = unchecked((int)0xff969696);
        private const int ArgbColor49 = unchecked((int)0xff003366);
        private const int ArgbColor50 = unchecked((int)0xff339966);
        private const int ArgbColor51 = unchecked((int)0xff003300);
        private const int ArgbColor52 = unchecked((int)0xff333300);
        private const int ArgbColor53 = unchecked((int)0xff993300);
        private const int ArgbColor54 = unchecked((int)0xff993366);
        private const int ArgbColor55 = unchecked((int)0xff333399);
        private const int ArgbColor56 = unchecked((int)0xff333333);
    }
}

