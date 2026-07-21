// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/07/2009 by Roman Korchagin

using Aspose.JavaAttributes;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Nrx.Writer
{
    internal interface INrxWriterContext
    {
        DocumentBase Document { get; }

        string GetStyleId(int istd);

        NrxXmlBuilder Builder { get; }

        bool IsDocx { get; }

        /// <summary>
        /// Based on the value of this property we will write different attributes, elements into the Docx document.
        /// </summary>
        OoxmlComplianceCore Compliance { get; }

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

        /// <summary>
        /// Writing of header/footer is too format specific. 
        /// The common section properties writer calls this back so the main writer can write the header/footer itself.
        /// </summary>
        [JavaThrows(true)]
        void WriteHeaderFooter(HeaderFooter headerFooter);

        /// <summary>
        /// Writing of footnote properties is too format specific.
        /// The writers that need to invoke writing of footnotes invoke this method, which the main writer should implement.
        /// </summary>
        /// <param name="attrs"></param>
        /// <param name="writeSeparators">True to write footnote separator characters. Use when writing document-wide footnote properties.</param>
        [JavaThrows(true)]
        void WriteFootnotePr(AttrCollection attrs, bool writeSeparators);

        /// <summary>
        /// Writes run attributes specified in 'w14' namespace, if it's needed.
        /// </summary>
        [JavaThrows(true)]
        void WriteW14Attributes(INrxWriterContext writer, RunPr runPr);

        /// <summary>
        /// <para>Returns list label string for current paragraph to write as list property.</para>
        /// </summary>
        string CurrentParagraphListLabelString { get; }

        /// <summary>
        /// <para>Returns list label font name for current paragraph to write as list property.</para>
        /// </summary>
        string CurrentParagraphListLabelFontName { get; }
    }
}
