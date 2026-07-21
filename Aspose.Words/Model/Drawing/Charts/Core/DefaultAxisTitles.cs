// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/06/2023 by Alexander Zhiltsov

using System.Collections.Generic;
using Aspose.Common;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Allows to get axis default title text.
    /// </summary>
    internal class DefaultAxisTitles
    {
        static DefaultAxisTitles()
        {
            PrepareTitles();
        }

        /// <summary>
        /// Returns default axis title text dependent on the current culture.
        /// </summary>
        internal static string GetTitle()
        {
            string result = gTitles.GetValueOrNull(SystemPal.GetCurrentCultureName());
            return result ?? DefaultTitleLabel;
        }

        private static void PrepareTitles()
        {
            gTitles["en"] = "Axis Title";                  //English
            gTitles["en-AU"] = "Axis Title";               //English (Australia)
            gTitles["en-BZ"] = "Axis Title";               //English (Belize)
            gTitles["en-CA"] = "Axis Title";               //English (Canada)
            gTitles["en-IN"] = "Axis Title";               //English (India)
            gTitles["en-IE"] = "Axis Title";               //English (Ireland)
            gTitles["en-JM"] = "Axis Title";               //English (Jamaica)
            gTitles["en-MY"] = "Axis Title";               //English (Malaysia)
            gTitles["en-NZ"] = "Axis Title";               //English (New Zealand)
            gTitles["en-PH"] = "Axis Title";               //English (Republic of the Philippines)
            gTitles["en-SG"] = "Axis Title";               //English (Singapore)
            gTitles["en-ZA"] = "Axis Title";               //English (South Africa)
            gTitles["en-TT"] = "Axis Title";               //English (Trinidad and Tobago)
            gTitles["en-GB"] = "Axis Title";               //English (United Kingdom)
            gTitles["en-US"] = "Axis Title";               //English (United States)
            gTitles["en-ZW"] = "Axis Title";               //English (Zimbabwe)

            gTitles["ja"] = "軸ラベル";                    //Japanese
            gTitles["ja-JP"] = "軸ラベル";                 //Japanese (Japan)
            gTitles["ru"] = "Название оси";                // Russian
            gTitles["ru-RU"] = "Название оси";             // Russian (Russia)
        }

        private static readonly Dictionary<string, string> gTitles = new Dictionary<string, string>();
        private const string DefaultTitleLabel = "Axis Title";
    }
}
