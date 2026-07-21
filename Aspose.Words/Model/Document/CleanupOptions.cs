// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/05/2017 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Allows to specify options for document cleaning.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/clean-up-a-document/">Clean Up a Document</a> documentation article.</para>
    /// </summary>
    public class CleanupOptions
    {
        /// <summary>
        /// Specifies whether unused styles should be removed from document.
        /// Default value is <c>true</c>.
        /// </summary>
        public bool UnusedStyles
        {
            get { return mUnusedStyles; }
            set { mUnusedStyles = value; }
        }

        /// <summary>
        /// Specifies whether unused list and list definitions should be removed from document.
        /// Default value is <c>true</c>.
        /// </summary>
        public bool UnusedLists
        {
            get { return mUnusedLists; }
            set { mUnusedLists = value; }
        }

        /// <summary>
        /// Gets/sets a flag indicating whether duplicate styles should be removed from document.
        /// Default value is <c>false</c>.
        /// </summary>
        public bool DuplicateStyle
        {
            get { return mDuplicateStyle; }
            set { mDuplicateStyle = value; }
        }

        /// <summary>
        /// Specifies that unused <see cref="Style.BuiltIn"/> styles should be removed from document.
        /// </summary>
        public bool UnusedBuiltinStyles
        {
            get { return mUnusedBuiltinStyles; }
            set { mUnusedBuiltinStyles = value; }
        }

        private bool mUnusedLists = true;
        private bool mUnusedStyles = true;
        private bool mDuplicateStyle = false;
        private bool mUnusedBuiltinStyles = false;
    }
}
