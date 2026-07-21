// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/05/2025 by Anton Savko

namespace Aspose.Words.RW.Html.Css
{
    internal class CssListRuleSelector
    {
        internal CssListRuleSelector(
            string listDefName,
            string listLevelName,
            string lfoName)
        {
            ListDefName = listDefName;
            ListLevelName = listLevelName.ToLowerInvariant();
            LfoName = lfoName;
        }

        internal CssListRuleSelector GetListDefSelector()
        {
            return GetListDefSelector(string.Empty);
        }

        internal CssListRuleSelector GetListDefSelector(string listLevelName)
        {
            return new CssListRuleSelector(ListDefName, listLevelName, string.Empty);
        }

        internal CssListRuleSelector GetLfoSelector()
        {
            return GetLfoSelector(string.Empty);
        }

        internal CssListRuleSelector GetLfoSelector(string listLevelName)
        {
            return new CssListRuleSelector(ListDefName, listLevelName, LfoName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (GetType() != obj.GetType())
                return false;

            CssListRuleSelector other = (CssListRuleSelector)obj;
            return (ListDefName == other.ListDefName) &&
                (ListLevelName == other.ListLevelName) &&
                (LfoName == other.LfoName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 818794172;
                hashCode = hashCode * -1521134295 + ((ListDefName == null) ? 0 : ListDefName.GetHashCode());
                hashCode = hashCode * -1521134295 + ((ListLevelName == null) ? 0 : ListLevelName.GetHashCode());
                hashCode = hashCode * -1521134295 + ((LfoName == null) ? 0 : LfoName.GetHashCode());
                return hashCode;
            }
        }

        internal bool IsListDefSelector
        {
            get { return StringUtil.HasChars(ListDefName) && !StringUtil.HasChars(LfoName); }
        }

        internal bool IsLfoSelector
        {
            get { return StringUtil.HasChars(ListDefName) && StringUtil.HasChars(LfoName); }
        }

        internal string ListDefName { get; }

        internal string ListLevelName { get; }

        internal string LfoName { get; }
    }
}
