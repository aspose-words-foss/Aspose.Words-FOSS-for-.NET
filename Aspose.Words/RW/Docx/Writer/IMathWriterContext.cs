// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/02/2025 by Alexander Zhiltsov

using Aspose.JavaAttributes;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Docx.Writer
{
    internal interface IMathWriterContext
    {
        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        [JavaThrows(true)]
        void Warn(WarningType warningType, string description);

        /// <summary>
        /// Gets a next unique value for writing as w:id of annotation elements in DOCX/WordML
        /// (comments, bookmarks and revision marks, etc).
        /// </summary>
        int GetNextAnnotationId();

        [JavaThrows(true)]
        void WriteRunPr(RunPr runPr);

        NrxXmlBuilder Builder { get; }

        bool IsDocx { get; }

        /// <summary>
        /// Based on the value of this property we will write different attributes, elements into the Docx document.
        /// </summary>
        OoxmlComplianceCore Compliance { get; }
    }
}
