// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/06/2016 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a key for <see cref="HtmlSelectorStyleCache"/>.
    /// </summary>
    internal class HtmlSelectorStyleCacheKey
    {
        internal HtmlSelectorStyleCacheKey(CssSelector selector, StyleType styleType)
        {
            Debug.Assert(selector != null);
            mSelector = selector;
            mStyleType = styleType;
        }

        internal CssSelector Selector
        {
            get { return mSelector; }
        }

        internal StyleType StyleType
        {
            get { return mStyleType; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((HtmlSelectorStyleCacheKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (mSelector.GetHashCode() * 397) ^ (int)mStyleType;
            }
        }

        protected bool Equals(HtmlSelectorStyleCacheKey other)
        {
            return mSelector.Equals(other.mSelector) && (mStyleType == other.mStyleType);
        }

        private readonly CssSelector mSelector;
        private readonly StyleType mStyleType;
    }
}
