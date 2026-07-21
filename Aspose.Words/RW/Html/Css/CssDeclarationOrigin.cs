// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/07/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Each style rule has an origin, which determines where it enters the cascade. 
    /// </summary>
    internal enum CssDeclarationOrigin
    {
        /// <summary>
        /// The author specifies style sheets for a source document according to the conventions of the document language. 
        /// For instance, in HTML, style sheets may be included in the document or linked externally.
        /// </summary>
        Author,
        /// <summary>
        /// The user may be able to specify style information for a particular document. 
        /// For example, the user may specify a file that contains a style sheet or the user agent may provide an interface 
        /// that generates a user style sheet (or behaves as if it did).
        /// </summary>
        User,
        /// <summary>
        /// Conforming user agents must apply a default style sheet (or behave as if they did). 
        /// A user agent's default style sheet should present the elements of the document language in ways that satisfy general 
        /// presentation expectations for the document language (e.g., for visual browsers, the EM element in HTML is presented using an italic font).
        /// </summary>
        UserAgent
    }
}
