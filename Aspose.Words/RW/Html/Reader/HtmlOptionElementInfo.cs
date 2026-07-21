// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/05/2017 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Stores information about 'option' element.
    /// </summary>
    internal class HtmlOptionElementInfo
    {
        internal HtmlOptionElementInfo(string text, string value, bool isSelected)
        {
            mText = text;
            mValue = value;
            mIsSelected = isSelected;
        }

        internal string Text
        {
            get { return mText; }
            set { mText = value; }
        }

        internal bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; }
        }

        internal string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        private string mText;
        private bool mIsSelected;
        private string mValue;
    }
}
