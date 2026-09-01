// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/02/2026 by Anton Savko

using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS list at-rule's prelude.
    /// </summary>
    internal class CssListRulePrelude
    {
        internal CssListRulePrelude(
            string listDefName,
            string listLevelName,
            string lfoName)
        {
            Debug.Assert(StringUtil.HasChars(listDefName));

            ListDefName = listDefName;
            ListLevelName = listLevelName.ToLowerInvariant();
            LfoName = lfoName;

            bool hasListLevelName = StringUtil.HasChars(ListLevelName);
            ListRuleType = StringUtil.HasChars(LfoName)
                ? hasListLevelName
                    ? CssListRuleType.ListLevelLfo
                    : CssListRuleType.ListLfo
                : hasListLevelName
                    ? CssListRuleType.ListLevel
                    : CssListRuleType.ListDef;
        }

        internal bool Matches(
            CssListRuleType listRuleType,
            CssListRuleSelector selector)
        {
            if (listRuleType != ListRuleType)
            {
                return false;
            }

            if (!StringUtil.EqualsIgnoreCase(selector.ListDefName, ListDefName))
            {
                return false;
            }

            bool listLevelNameMatches = StringUtil.EqualsIgnoreCase(selector.ListLevelName, ListLevelName);
            bool lfoNameMatches = StringUtil.EqualsIgnoreCase(selector.LfoName, LfoName);
            switch (listRuleType)
            {
                case CssListRuleType.ListDef:
                    // We have already checked that the list def name matches.
                    return true;
                case CssListRuleType.ListLevel:
                    return listLevelNameMatches;
                case CssListRuleType.ListLfo:
                    return lfoNameMatches;
                case CssListRuleType.ListLevelLfo:
                    return listLevelNameMatches && lfoNameMatches;
                default:
                    Debug.Assert(false);
                    return false;
            }
        }

        internal string ToCss()
        {
            StringBuilder cssBuilder = new StringBuilder();

            cssBuilder.Append("@list ");
            cssBuilder.Append(ListDefName);

            if (StringUtil.HasChars(ListLevelName))
            {
                cssBuilder.Append(':');
                cssBuilder.Append(ListLevelName);
            }

            if (StringUtil.HasChars(LfoName))
            {
                cssBuilder.Append(' ');
                cssBuilder.Append(LfoName);
            }

            return cssBuilder.ToString();
        }

        internal string ListDefName { get; }

        internal string ListLevelName { get; }

        internal string LfoName { get; }

        internal CssListRuleType ListRuleType { get; }
    }
}
