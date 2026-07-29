// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/05/2012 by Alexey Butalov

using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.Drawing.Fonts;
using Aspose.Words.Lists;
using Aspose.Words.Loading;
using Aspose.Words.RW.Factories;

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Imports plain text file into a Document object.
    /// </summary>
    internal class TxtReader : IDocumentReader
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal TxtReader(Stream stream, LoadOptions loadOptions, FileFormatInfo fileFormatInfo, Document document)
        {
            Debug.Assert(stream != null);
            Debug.Assert(document != null);
            mStream = stream;
            mDocument = document;

            mLoadOptions = loadOptions as TxtLoadOptions;
            if (mLoadOptions == null)
                mLoadOptions = new TxtLoadOptions(loadOptions);

            FileFormatInfo info;
            if (fileFormatInfo != null)
            {
                info = fileFormatInfo;
            }
            else
            {
                FileFormatDetector detector = new FileFormatDetector();
                info = detector.Detect(mStream);
            }

            mEncoding = GetEncoding(mLoadOptions, info);
            mHasRtlScript = info.HasRtlScript;
            mSpaceWidth = -1;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal TxtReader(Stream stream, Document document, FileFormatInfo fileFormatInfo)
            : this(stream, null, fileFormatInfo, document)
        {
        }

        #region Methods

        /// <summary>
        /// Reads plain text file into the model.
        /// </summary>
        public void Read()
        {
            // For safety reasons. Obtaining a non-empty document is unlikely.
            bool wasEmptyDocument = mDocument.FirstSection == null;
            mDocumentBuilder = new DocumentBuilder(mDocument);

            // WORDSNET-19067 We have introduced a new DocumentDirection load option and
            // need to set paragraph BiDi attribute in accordance with it.
            if ((mLoadOptions.DocumentDirection == DocumentDirection.RightToLeft) ||
                ((mLoadOptions.DocumentDirection == DocumentDirection.Auto) && mHasRtlScript))
                mDocumentBuilder.ParagraphFormat.Bidi = true;

            SetFontProperties(mDocumentBuilder);
            TxtParagraphReader paragraphReader = new TxtParagraphReader(new StreamReader(mStream, mEncoding), mLoadOptions);
            mParagraphLists = new Dictionary<TxtParagraph, List>();
            mSpaceWidth = -1;

            // WORDSNET-22891 Implemented TXT loading progress notification.
            LoadingProgressProcessor progressProcessor = new LoadingProgressProcessor(mDocument, mLoadOptions);
            TxtParagraph[] paragraphs = paragraphReader.Read(progressProcessor);
            foreach (TxtParagraph paragraph in paragraphs)
                Write(paragraph);
            RemoveNumbersInNeed();

            if (wasEmptyDocument && !mDocument.FirstSection.Body.LastParagraph.HasChildNodes)
                mDocument.FirstSection.Body.LastParagraph.Remove();

            // After correct bidi runs are generated, we set fonts to them depending on text language like MS Word does.
            // If necessary, runs are split to several to assign font.
            mDocumentBuilder.Document.Accept(new TxtFontSetter());

            // WORDSNET-17877 Set locale-dependent default page size.
            mDocument.FirstSection.SectPr.SetDefaultPageSize();
        }

        public Stream Decrypt()
        {
            Debug.Assert(false, "Not supported");
            return null;
        }

        /// <summary>
        /// Converts indent in chars to indent in points.
        /// </summary>
        internal double GetIndentInPoints(int indentInChars)
        {
            if (mSpaceWidth < 0)
                mSpaceWidth = GetTextWidth("w");
            return mSpaceWidth * indentInChars;
        }

        /// <summary>
        /// Returns encoding obtained either from a specified LoadOptions or FileFormatInfo.
        /// By default returns UTF8.
        /// </summary>
        internal static Encoding GetEncoding(LoadOptions loadOptions, FileFormatInfo fileFormatInfo)
        {
            if ((loadOptions != null) && (loadOptions.Encoding != null))
                return loadOptions.Encoding;

            if ((fileFormatInfo != null) && (fileFormatInfo.Encoding != null))
                return fileFormatInfo.Encoding;

            return Encoding.UTF8;
        }

        /// <summary>
        /// Writes the text paragraph into the document.
        /// </summary>
        private void Write(TxtParagraph paragraph)
        {
            if (paragraph.Numbering != null)
            {
                WriteList(paragraph);
            }
            else
            {
                RemoveNumbersInNeed();
                WriteParagraph(paragraph);
            }

            mPrevParagraph = paragraph;
        }

        /// <summary>
        /// Writes the text paragraph to document's paragraph.
        /// </summary>
        private void WriteParagraph(TxtParagraph paragraph)
        {
            mDocumentBuilder.ParagraphFormat.LeftIndent = GetIndentInPoints(paragraph.LeftIndent);
            mDocumentBuilder.ParagraphFormat.FirstLineIndent = GetIndentInPoints(paragraph.FirstLineIndent);

            paragraph.Write(mDocumentBuilder, mLoadOptions);

            if (paragraph.IsNewSectionRequested)
                mDocumentBuilder.InsertSection(SectionStart.NewPage);
        }

        private void RemoveNumbersInNeed()
        {
            if ((mPrevParagraph != null) && (mPrevParagraph.Numbering != null))
                mDocumentBuilder.ListFormat.RemoveNumbers();
        }

        private double GetTextWidth(string text)
        {
            DocumentBuilder docBuilder;

            if(mDocumentBuilder == null)
            {
                docBuilder = new DocumentBuilder();
                SetFontProperties(docBuilder);
            }
            else
                docBuilder = mDocumentBuilder;

            Font font = docBuilder.Font;
            DrFont drawingFont = docBuilder.Document.FontProvider.FetchDrFont(
                font.Name, (float)font.Size, font.FontStyle);
            return drawingFont.GetTextWidthPoints(text);
        }

        /// <summary>
        /// Writes the text paragraph as document's list.
        /// </summary>
        private void WriteList(TxtParagraph paragraph)
        {
            SetListToDocument(paragraph);
            SetListFormating(paragraph.Numbering);

            double numberPos = GetIndentInPoints(paragraph.Numbering.NumberPosition);
            double numberWidth = GetTextWidth(paragraph.Numbering.Text + "  ");
            mDocumentBuilder.ParagraphFormat.LeftIndent = numberPos + numberWidth;
            mDocumentBuilder.ParagraphFormat.FirstLineIndent = -numberWidth;

            paragraph.Write(mDocumentBuilder, mLoadOptions);

            // WORDSNET-10612 List paragraph could be built from the string, which is ended with page break symbol (0x12).
            if (paragraph.IsNewSectionRequested)
                mDocumentBuilder.InsertSection(SectionStart.NewPage);
        }

        /// <summary>
        /// Sets document's list for the text paragraph.
        /// </summary>
        private void SetListToDocument(TxtParagraph paragraph)
        {
            Debug.Assert(paragraph.Numbering != null);

            List list;
            if (paragraph.Numbering.PrevNumberingParagraph != null)
                list = mParagraphLists.GetValueOrNull(paragraph.Numbering.PrevNumberingParagraph);
            else if (paragraph.Numbering.ParentNumberingParagraph != null)
                list = mParagraphLists.GetValueOrNull(paragraph.Numbering.ParentNumberingParagraph);
            else
                list = mDocumentBuilder.Document.Lists.AddEmpty(
                    paragraph.Numbering.NumberingStyle.IsLevelsSupported
                        ? ListType.MultiLevel
                        : ListType.SingleLevel);

            mParagraphLists.Add(paragraph, list);
            mDocumentBuilder.ListFormat.List = list;
        }

        /// <summary>
        /// Sets document's list formatting properties.
        /// </summary>
        /// <param name="numbering">Source numbering information</param>
        private void SetListFormating(TxtNumbering numbering)
        {
            mDocumentBuilder.ListFormat.ListLevelNumber = numbering.ListLevelNumber;
            mDocumentBuilder.ListFormat.ListLevel.NumberStyle = numbering.NumberingStyle.NumberStyle;

            // WORDSNET-9968 We should specify font properties for a ListLevel, in case the document starts from a list paragraph.
            SetFontProperties(mDocumentBuilder.ListFormat.ListLevel.Font);

            mDocumentBuilder.ListFormat.ListLevel.NumberFormat =
                numbering.NumberingStyle.GetNumberFormat(numbering.ListLevelNumber);
        }

        /// <summary>
        /// Sets document's builder font properties.
        /// </summary>
        /// <param name="documentBuilder">An instance of DocumentBuilder.</param>
        private static void SetFontProperties(DocumentBuilder documentBuilder)
        {
            if (documentBuilder != null)
                SetFontProperties(documentBuilder.Font);
        }

        /// <summary>
        /// Sets font properties which used by MS Word while importing a text files.
        /// </summary>
        private static void SetFontProperties(Font font)
        {
            font.Name = TxtFontSetter.DefaultFont;
            font.Size = 10.5;
            font.SizeBi = 10.5;
        }
        #endregion

        public bool IsEncrypted
        {
            get { return false; }
        }

        private DocumentBuilder mDocumentBuilder;
        private TxtParagraph mPrevParagraph;
        private Dictionary<TxtParagraph, List> mParagraphLists;
        private double mSpaceWidth;
        private readonly Stream mStream;
        private readonly Document mDocument;
        private readonly Encoding mEncoding;
        private readonly bool mHasRtlScript;
        private readonly TxtLoadOptions mLoadOptions;
    }
}
