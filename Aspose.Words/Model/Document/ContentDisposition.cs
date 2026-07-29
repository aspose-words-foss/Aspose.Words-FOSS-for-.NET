// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/05/2006 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Enumerates different ways of presenting the document at the client browser. 
    /// </summary>
    /// <remarks>
    /// <para>Note that the actual behavior on the client browser might be affected by security configuration of the browser.</para>
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppSkipEntity("We do not provide saving document to a response stream in C++.")]
    public enum ContentDisposition
    {
        /// <summary>
        /// Send the document to the browser and present an option to save the document to disk or open in the application
        /// associated with the document's extension.
        /// </summary>
        Attachment = 0,

        /// <summary>
        /// Send the document to the browser and presents an option to save the document to disk or open inside the browser.
        /// </summary>
        Inline = 1,
    }
}
