// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/02/2025 by Alexander Zhiltsov

using Aspose.JavaAttributes;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Provides methods called when writing DML shapes. The interface is implemented to be able to use common shape
    /// writing code for DOCX and XLSX exports.
    /// </summary>
    internal interface IDmlShapeWriterContext
    {
        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        [JavaThrows(true)]
        void Warn(WarningType warningType, WarningSource warningSource, string description);

        /// <summary>
        /// Writes image binary data to the output document.
        /// </summary>
        [JavaThrows(true)]
        string WriteImageBinData(byte[] imageBytes);

        /// <summary>
        /// Writes a link to an external image to the output document.
        /// </summary>
        string WriteImageLink(string link);

        /// <summary>
        /// Adds a hyperlink relationship to the current part and returns the id.
        /// </summary>
        string AddHyperlinkRelationship(string dest);

        /// <summary>
        /// Get an XML builder used to write document XML contents.
        /// </summary>
        NrxXmlBuilder Builder { get; }

        /// <summary>
        /// Based on the value of this property we will write different attributes, elements into the Docx document.
        /// </summary>
        OoxmlComplianceCore Compliance { get; }
    }
}
