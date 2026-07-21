// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/08/2018 by Victor Moskvitin

using System.Collections.Generic;
using Aspose.Common;

namespace Aspose.Words.Drawing.Charts.Core
{
    internal static class DefaultChartTitles
    {
        static DefaultChartTitles()
        {
            gTitles.Add("en", EnglishTitleLabel);    // English
            gTitles.Add("ru", "Название диаграммы"); // Russian
            gTitles.Add("ja", "グラフ タイトル");     // Japanese
            gTitles.Add("zh", "图表标题");           // ChineseChina
            gTitles.Add("zh-cn", "图表标题");        // ChineseChina
            gTitles.Add("zh-tw", "圖表標題");        // ChineseTaiwan
            gTitles.Add("fr", "Titre du graphique");// French
            gTitles.Add("es", "Título del gráfico");// Spanish
            gTitles.Add("pt", "Título do gráfico"); // Portuguese
        }

        /// <summary>
        /// Returns the default chart title text based on the current culture.
        /// </summary>
        internal static string GetTitle()
        {
            string cultureName = SystemPal.GetCurrentCultureName().ToLower();

            if (cultureName == null)
                return EnglishTitleLabel;

            if (gTitles.ContainsKey(cultureName))
                return gTitles[cultureName];

            string langOnly = cultureName.Split(gSeparator)[0].ToLower();

            if (gTitles.ContainsKey(langOnly))
                return gTitles[langOnly];

            return EnglishTitleLabel;
        }
       
        private static readonly Dictionary<string, string> gTitles =
            new Dictionary<string, string>();
        private static readonly char[] gSeparator = new char[] { '-', '_' };
        private const string EnglishTitleLabel = "Chart Title";
    }
}
