// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/11/2005 by Roman Korchagin, Michael Morozoff

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Provides various options that control how a document is shown in Microsoft Word.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/work-with-word-document-options-and-appearance/">Work with Options and Appearance of Word Documents</a> documentation article.</para>
    /// </summary>
    /// <seealso cref="Document"/>
    /// <seealso cref="Document.ViewOptions"/>
    public class ViewOptions
    {
        internal ViewOptions()
        {
            ViewType = ViewType.Normal;
            ZoomPercent = 100;
            ZoomType = ZoomType.None;
        }

        /// <summary>
        /// Controls the view mode in Microsoft Word.
        /// </summary>
        /// <remarks><p>Although Aspose.Words is able to read and write this option, its usage is application-specific.
        /// For example MS Word 2013 does not respect the value of this option.</p></remarks>
        public ViewType ViewType { get; set; }

        /// <summary>
        /// Gets or sets a zoom value based on the size of the window.
        /// </summary>
        public ZoomType ZoomType { get; set; }

        /// <summary>
        /// Gets or sets the percentage at which you want to view your document.
        /// </summary>
        /// <remarks>
        /// <p>Although Aspose.Words is able to read and write this option, its usage is application-specific.
        /// For example MS Word 2013 does not respect the value of this option. </p>
        /// </remarks>
        public int ZoomPercent { get; set; }

        /// <summary>
        /// Turns off display of the space between the top of the text and the top edge of the page.
        /// </summary>
        public bool DoNotDisplayPageBoundaries { get; set; }

        /// <summary>
        /// Controls display of the background shape in print layout view.
        /// </summary>
        public bool DisplayBackgroundShape { get; set; }

        /// <summary>
        /// Specifies whether the document is in forms design mode.
        /// </summary>
        /// <remarks>
        /// <p>Currently works only for documents in WordML format.</p>
        /// </remarks>
        public bool FormsDesign { get; set; }

        internal ViewOptions Clone()
        {
            return (ViewOptions)MemberwiseClone();
        }
    }
}
