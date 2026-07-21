// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2019 by Anton Savko

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Reader
{
    internal class HtmlModelListLevelPropertyValueCollection : IHtmlModelListLevelPropertyValue
    {
        internal HtmlModelListLevelPropertyValueCollection()
        {
            mPropertyValues = new List<IHtmlModelListLevelPropertyValue>();
        }

        public bool CanModifyListLevel(HtmlModelListLevel modelListLevel)
        {
            foreach (IHtmlModelListLevelPropertyValue propertyValue in mPropertyValues)
            {
                if (!propertyValue.CanModifyListLevel(modelListLevel))
                {
                    return false;
                }
            }

            return true;
        }

        public void ModifyListLevel(HtmlModelListLevel modelListLevel)
        {
            foreach (IHtmlModelListLevelPropertyValue propertyValue in mPropertyValues)
            {
                propertyValue.ModifyListLevel(modelListLevel);
            }
        }

        internal void Add(IHtmlModelListLevelPropertyValue propertyValue)
        {
            mPropertyValues.Add(propertyValue);
        }

        private readonly List<IHtmlModelListLevelPropertyValue> mPropertyValues;
    }
}
