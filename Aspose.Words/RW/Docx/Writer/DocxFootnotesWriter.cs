// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/08/2007 by Vladimir Averkin

using System;
using Aspose.Words.Notes;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Writes footnotes and endnotes.
    /// </summary>
    internal class DocxFootnotesWriter : NrxFootnotesWriter
    {
        internal DocxFootnotesWriter(FootnoteType footnoteType, DocxDocumentWriterBase writer)
            : base(footnoteType,
                (footnoteType == FootnoteType.Endnote)
                    ? writer.SaveInfo.HasEndnotes || !writer.Document.FootnoteSeparators.IsDefault
                    : writer.SaveInfo.HasFootnotes || !writer.Document.FootnoteSeparators.IsDefault,
                writer)
        {
            mWriter = writer;

            if (HasFootnotes)
            {
                string partName;
                string contentType;
                string relType;
                string rootName;
                switch (footnoteType)
                {
                    case FootnoteType.Footnote:
                        partName = "footnotes.xml";
                        contentType = DocxContentType.Footnotes;
                        relType = writer.RelTypes.Footnotes;
                        rootName = "w:footnotes";
                        break;
                    case FootnoteType.Endnote:
                        partName = "endnotes.xml";
                        contentType = DocxContentType.Endnotes;
                        relType = writer.RelTypes.Endnotes;
                        rootName = "w:endnotes";
                        break;
                    default:
                        throw new InvalidOperationException("Unknown footnote type.");
                }

                mBuilder = mWriter.CreateChildPartAndBuilder(partName, contentType, relType);
                mBuilder.StartDocumentWithStandardNamespaces(rootName);
            }
        }

        internal void Close()
        {
            if (mBuilder != null)
                mBuilder.EndDocument();
        }

        /// <summary>
        /// Writes a footnote start into "Footnotes"/"Endnotes" package part.
        /// </summary>
        internal void WriteFootnoteStart()
        {
            mBuilder.StartElement(IsEndnote ? "w:endnote" : "w:footnote");
            mBuilder.WriteAttribute("w:id", mId);

            // We need to write all the following story nodes to "Footnotes"/"Endnotes" part, so save the current builder in the main writer.
            mWriter.PushBuilder(mBuilder);
        }

        /// <summary>
        /// Writes footnote end into "Footnotes"/"Endnotes" package part.
        /// </summary>
        /// <returns>Returns footnote reference id.</returns>
        internal int WriteFootnoteEnd()
        {
            mBuilder.EndElement();

            // Restore the writer's builder to the previous setting.
            mWriter.PopBuilder();

            return mId++;
        }

        /// <summary>
        /// Writes footnote separator definitions to "Footnote"/"Endnote" builder.
        /// and corresponding footnote references to the current builder.
        /// </summary>
        protected override void WriteSeparator(FootnoteSeparator separator)
        {
            if (separator == null)
                return;

            WriteFootnoteStart(separator.SeparatorType);
            separator.Accept(mWriter);
            int id = WriteFootnoteEnd();

            mWriter.WriteElementWithId(IsEndnote ? "w:endnote" : "w:footnote", id);
        }

        /// <summary>
        /// Converts the specified number style to a string value to write it to document XML.
        /// </summary>
        protected override string NumberStyleToXml(NumberStyle value)
        {
            return DocxEnum.NumberStyleToDocx(value);
        }

        /// <summary>
        /// Writes a footnote start into "Footnotes"/"Endnotes" package part for footnote/endnote separators.
        /// </summary>
        private void WriteFootnoteStart(FootnoteSeparatorType separatorType)
        {
            mBuilder.StartElement(IsEndnote ? "w:endnote" : "w:footnote");
            mBuilder.WriteAttributeIfNotDefault("w:type", DocxEnum.FootnoteSeparatorTypeToDocx(separatorType, IsEndnote), "normal");
            mBuilder.WriteAttribute("w:id", mId);

            // We need to write all the following story nodes to "Footnotes"/"Endnotes" part, so save the current builder in the main writer.
            mWriter.PushBuilder(mBuilder);
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DocxDocumentWriterBase mWriter;
        /// <summary>
        /// This is the builder for the footnotes or endnotes package part.
        /// </summary>
        private readonly DocxBuilder mBuilder;
        private int mId;
    }
}
