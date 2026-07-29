// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2024 by Vadim Saltykov

namespace Aspose.Words
{
    /// <summary>
    /// Allows to specify additional options for the document building process.
    /// </summary>
    public class DocumentBuilderOptions
    {
        private bool mContextTableFormatting = true;

        /// <summary>
        /// True if the formatting applied to table content does not affect the formatting of the content that follows it.
        /// Default value is <c>true</c>.
        /// </summary>
        public bool ContextTableFormatting
        {
            get { return mContextTableFormatting; }
            set { mContextTableFormatting = value; }
        }

        /// <summary>
        /// Corresponds to Design Mode in Microsoft Word.
        /// </summary>
        public bool DesignMode { get; set; }
    }
}
