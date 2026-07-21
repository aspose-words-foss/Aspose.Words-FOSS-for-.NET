// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/03/2019 by Konstantin Kornilov

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Aspose.Fonts.TrueType;

namespace Aspose.Fonts
{
    /// <summary>
    /// Font config substitution rule.
    /// </summary>
    public class FontConfigSubstitutionRuleCore
    {
        /// <summary>
        /// Check if fontconfig utility is available or not.
        /// </summary>
        public bool IsFontConfigAvailable()
        {
            string[] defaultFontResult = GetFontConfigResult("Arial", FontStyle.Regular, null);
            return defaultFontResult != null && defaultFontResult.Length > 0;
        }

        /// <summary>
        /// Resets the cache of fontconfig calling results.
        /// </summary>
        public void ResetCache()
        {
            mFontConfigResultCache.Clear();
        }

        public TTFont PerformSubstitution(string familyName, FontStyle style, FontSubstitutionInfo info,
            ExternalFontCache fontCache)
        {
            string[] fcResult = GetFontConfigResult(familyName, style, info);
            if (fcResult == null)
                return null;

            foreach (string substituteName in fcResult)
            {
                TTFont font = fontCache.GetFont(substituteName, style);
                if (font != null)
                    return font;
            }

            return null;
        }

        [JavaAttributes.JavaThrows(false)]
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)] // C++ directly uses FontConfig instead of calling fc-match
        private string[] GetFontConfigResult(string family, FontStyle style, FontSubstitutionInfo info)
        {
            if (!StringUtil.HasChars(family))
                return null;

            if (mFontConfigResultCache.ContainsKey(family))
                return mFontConfigResultCache[family];

            string arguments = BuildArgumentString(family, style, info);
            ProcessExecutionResult result = PlatformUtilPal.RunProcess(FcMatchExe, "-s", arguments);
            
            // the default OS independent formatting is 
            // LiberationSans-Regular.ttf: "Liberation Sans" "Regular"
            if (!result.Successful || result.Output == null || !result.Output.Contains(":"))
                return null;

            // By default FontConfig returns the best match which is available in system.
            string[] substitutions = ParseFontConfigResponse(result.Output);
            if (substitutions.Length == 0)
                return null;

            mFontConfigResultCache.Add(family, substitutions);
            return substitutions;
        }

        [CodePorting.Translator.Cs2Cpp.CppSkipEntity]
        private static string[] ParseFontConfigResponse(string response)
        {
            StringBuilder s = new StringBuilder();

            List<string> substitutions = new List<string>();
            string[] variants = response.Split('\n');
            foreach (string variant in variants)
            {
                string[] responseLine = variant.Split(':');
                if (!responseLine[0].EndsWith(TtfExtension, StringComparison.Ordinal))
                    continue;

                s.Length = 0;
                string nameStyle = responseLine[1].Trim();
                string fontName = ExtractFontName(s, nameStyle);

                if (!string.IsNullOrEmpty(fontName))
                    substitutions.Add(fontName);
            }

            return substitutions.ToArray();
        }

        [CodePorting.Translator.Cs2Cpp.CppSkipEntity]
        private static string ExtractFontName(StringBuilder s, string nameStyle)
        {
            for (int i = 1; i < nameStyle.Length; i++)
            {
                char c = nameStyle[i];
                if (c == '"')
                    break;

                s.Append(c);
            }
            return s.ToString();
        }

        [CodePorting.Translator.Cs2Cpp.CppSkipEntity]
        private static string BuildArgumentString(string family, FontStyle style, FontSubstitutionInfo info)
        {
            StringBuilder args = new StringBuilder();
            args.Append(family);
            args.Append(":style=").Append(style);

            if (info != null)
            {
                if (info.Weight != 0)
                    args.Append(":weight=").Append(info.Weight);
                if (info.Charset != FontUtil.UndefinedCharset && info.Charset != 0)
                    args.Append(":charset=").Append(info.Charset);
            }

            return args.ToString();
        }

        private readonly Dictionary<string, string[]> mFontConfigResultCache = new Dictionary<string, string[]>();
        private const string TtfExtension = ".ttf";
        private const string FcMatchExe = "fc-match";
    }
}
