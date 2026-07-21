// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/07/2008 by Roman Korchagin

namespace Aspose.Words.RW.Odt
{
    /// <summary>
    /// Mime Content Types used in OpenOffice.
    /// </summary>
    /// <remarks>
    /// See https://www.openoffice.org/framework/documentation/mimetypes/mimetypes.html.
    /// </remarks>
    internal static class OdtContentTypes
    {
        /// <summary>
        /// This is the content type used by OOO which we support (text document).
        /// </summary>
        internal const string OpenDocumentText = "application/vnd.oasis.opendocument.text";
        
        /// <summary>
        /// This is the content type used by OOO which we support (text document used as template).
        /// </summary>
        internal const string OpenDocumentTextTemplate = "application/vnd.oasis.opendocument.text-template";

        /// <summary>
        /// OpenDocument Master Document. 
        /// </summary>
        internal const string OpenDocumentMasterDocument = "application/vnd.oasis.opendocument.text-master";
    }
}
