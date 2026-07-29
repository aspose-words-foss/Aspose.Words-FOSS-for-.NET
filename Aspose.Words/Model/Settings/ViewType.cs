// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Possible values for the view mode in Microsoft Word.
    /// </summary>
    /// <seealso cref="ViewOptions"/>
    /// <seealso cref="ViewOptions.ViewType"/>
    public enum ViewType
    {
        /// <summary>
        /// The document shall be rendered in the default view of the application.
        /// </summary>
        None = 0,
        /// <summary>
        /// The document shall be rendered in the default view of the application.
        /// </summary>
        Reading = 0,
        /// <summary>
        /// The document shall be opened in a view that displays the document as it will print.
        /// </summary>
        PageLayout = 1,
        /// <summary>
        /// The document shall be rendered in a view optimized for outlining or creating long documents.
        /// </summary>
        Outline = 3,
        /// <summary>
        /// The document shall be rendered in a view optimized for outlining or creating long documents.
        /// </summary>
        Normal = 4,            
        /// <summary>
        /// The document shall be rendered in a view mimicking the way this document would be displayed 
        /// in a web page.
        /// </summary>
        Web = 5
    }
}
