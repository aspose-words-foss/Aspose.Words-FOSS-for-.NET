// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/02/2013 by Alexey Butalov

using Aspose.Collections.Generic;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implementation of IHtmlElementProvider for mocking and test purpose.
    /// </summary>
    internal class MockHtmlElementProvider : IHtmlElementProvider
    {
        internal MockHtmlElementProvider(string name)
            : this(name, null)
        {
        }

        internal MockHtmlElementProvider(string name, MockHtmlElementProvider parent)
        {
            Debug.Assert(StringUtil.HasChars(name));
            mName = name;
            mAttributes = new SortedStringListGeneric<string>(false);
            mChildren = new HtmlElementProviderCollection();

            if (parent != null)
            {
                mElementIndex = parent.mChildren.Add(this);
                mParent = parent;
            }
            else
            {
                mElementIndex = -1;
            }
        }

        internal static MockHtmlElementProvider Create(string name)
        {
            return new MockHtmlElementProvider(name);
        }

        internal static MockHtmlElementProvider Create(string name, MockHtmlElementProvider parent)
        {
            return new MockHtmlElementProvider(name, parent);
        }

        public string GetInnerText()
        {
            return string.Empty;
        }

        public string GetAttributeValue(string attribName)
        {
            return mAttributes.GetValueOrNull(attribName);
        }

        public string GetAttributeValue(string name, string def)
        {
            return mAttributes.ContainsKey(name)
                       ? mAttributes[name]
                       : def;
        }

        public string[] GetClasses()
        {
            string classValue = GetAttributeValue("class", string.Empty);
            return CssUtil.SplitClassAttributeValue(classValue);
        }

        public string GetLanguage()
        {
            // This method does not implement the full procedure of getting the element's language, as described
            // in http://www.w3.org/TR/html5/dom.html#language, but other sources of the language information are either
            // non-conforming (the element <meta http-equiv="content-language">) or unavailable (HTTP headers).
            string langAttribute = mAttributes.GetValueOrNull("lang");
            if (langAttribute != null)
                return langAttribute;

            return (mParent != null)
                       ? mParent.GetLanguage()
                       : string.Empty;
        }

        public IElementProvider GetFirstChildElement()
        {
            return (mChildren.Count != 0) ? mChildren[0] : null;
        }

        public IElementProvider GetPreviousSiblingElement()
        {
            if (mParent == null)
                return null;

            return (mElementIndex > 0)
                       ? mParent.mChildren[mElementIndex - 1]
                       : null;
        }

        public IElementProvider GetNextSiblingElement()
        {
            if (mParent == null)
                return null;

            return (mElementIndex < mParent.mChildren.Count - 1)
                       ? mParent.mChildren[mElementIndex + 1]
                       : null;
        }

        public IElementProvider[] GetChildElements()
        {
            return mChildren.ToArray();
        }

        public bool HasOnlyOneChildElementWithOptionalWhitespace()
        {
            return mChildren.Count == 1;
        }

        public bool StartsWithImplicitBox
        {
            get
            {
                // Implicit elements do not participate in our CSS box model, so the return value does not matter.
                return false;
            }
        }

        public bool EndsWithImplicitBox
        {
            get
            {
                // Implicit elements do not participate in our CSS box model, so the return value does not matter.
                return false;
            }
        }

        public bool ContainsChildBoxes
        {
            get
            {
                // Implicit elements do not participate in our CSS box model, so the return value does not matter.
                return false;
            }
        }

        public bool IsElementContainsText
        {
            get
            {
                // Implicit elements do not participate in our CSS box model, so the return value does not matter.
                return false;
            }
        }

        public bool IsLastChildBox
        {
            get
            {
                // Implicit elements do not participate in our CSS box model, so the return value does not matter.
                return false;
            }
        }

        public IElementProvider GetParentElement()
        {
            return mParent;
        }

        internal MockHtmlElementProvider AddAttribute(string attributeName, string attributeValue)
        {
            mAttributes.Add(attributeName, attributeValue);
            return this;
        }

        public string ElementNamespace
        {
            get { return W3CNamespaces.Xhtml; }
        }

        public string ElementName
        {
            get { return mName; }
            set { mName = value; }
        }

        public bool IsImplicit
        {
            get { return false; }
        }

        private string mName;
        private readonly int mElementIndex;
        private readonly SortedStringListGeneric<string> mAttributes;

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly MockHtmlElementProvider mParent;
        private readonly HtmlElementProviderCollection mChildren;
    }
}
