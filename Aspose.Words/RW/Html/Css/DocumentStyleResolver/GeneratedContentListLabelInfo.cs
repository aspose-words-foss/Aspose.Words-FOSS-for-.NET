// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/09/2022 by Anton Savko

using Aspose.Collections;

namespace Aspose.Words.RW.Html.Css
{
    internal class GeneratedContentListLabelInfo
    {
        internal GeneratedContentListLabelInfo(
            string numberFormat,
            IntToIntDictionary levelNumberToNumberValueMap,
            IntToIntDictionary levelNumberToNumberStyleMap)
        {
            Debug.Assert(StringUtil.HasChars(numberFormat));
            Debug.Assert(levelNumberToNumberValueMap != null);
            Debug.Assert(levelNumberToNumberStyleMap != null);

            NumberFormat = numberFormat;
            LevelNumberToNumberValueMap = levelNumberToNumberValueMap;
            LevelNumberToNumberStyleMap = levelNumberToNumberStyleMap;
        }

        internal string NumberFormat { get; }

        internal IntToIntDictionary LevelNumberToNumberValueMap { get; }

        internal IntToIntDictionary LevelNumberToNumberStyleMap { get; }
    }
}
