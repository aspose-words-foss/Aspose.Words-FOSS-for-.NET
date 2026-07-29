// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2010 by Dmitry Vorobyev

using System.Text.RegularExpressions;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Designates a range of levels, for example 1 to 9.
    /// </summary>
    internal class LevelRange
    {
        private LevelRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Attempts to parse range values from the specified string of type "startLevel-endLevel".
        /// </summary>
        internal static LevelRange TryParse(string text)
        {
            if (!StringUtil.HasChars(text))
                return null;

            Match match = gRangeRegex.Match(text);

            if (!match.Success)
                return null;

            int min = FormatterPal.ParseInt(match.Groups[MinLevelGroup].Value);
            int max = FormatterPal.ParseInt(match.Groups[MaxLevelGroup].Value);

            // Invalid range?
            if (min > max)
                return null;

            // Invalid level?
            if (!IsLevelValid(min) || !IsLevelValid(max))
                return null;

            return new LevelRange(min, max);
        }

        internal bool IsInRange(int level)
        {
            return (level >= Min) && (level <= Max);
        }

        internal static bool IsLevelValid(int level)
        {
            return (level >= MinLevel) && (level <= MaxLevel);
        }

        internal int Min { get; }

        internal int Max { get; }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MinLevel = 1;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxLevel = 9;

        internal static readonly LevelRange MaxRange = new LevelRange(MinLevel, MaxLevel);
        internal static readonly LevelRange EmptyRange = new LevelRange(MaxLevel, MinLevel);

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int InvalidLevel = -1;

        //JAVA lacks named groups, let's use numbered ones.
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int MinLevelGroup = 1;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int MaxLevelGroup = 2;
        private static readonly Regex gRangeRegex = new Regex(@"\A\D*(\d)\D+(\d)\D*\z", RegexOptions.Compiled);
        internal const string ParsingErrorMessage = "Error! Not a valid heading level range.";
    }
}
