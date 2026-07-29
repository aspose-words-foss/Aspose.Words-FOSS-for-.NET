// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/03/2009 by Roman Korchagin

using System.ComponentModel;
using Aspose.OpcPackaging;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Loading;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Reads the main document and associated parts from OOXML.
    /// </summary>
    internal class DocxDocumentReader : DocxDocumentReaderBase
    {
        internal DocxDocumentReader(
            OpcPackageBase package,
            OpcPackagePart documentPart,
            Document doc,
            LoadOptions loadOptions,
            OoxmlComplianceInfo complianceInfo,
            DocxStylesReader stylesReader,
            DocxStoryReader storyReader,
            DocxNumberingReader numberingReader,
            DocxSectPrReader sectPrReader)
            : base(package, documentPart, doc, loadOptions, complianceInfo, stylesReader, storyReader, numberingReader, sectPrReader)
        {
            mMainDoc = doc;
            mStoryReader = storyReader;
        }

        internal override bool IsEquationXmlReader
        {
            get { return false; }
        }

        protected override void DoRead()
        {
            DocxCustomXmlPartReader.Read(this, LoadOptions.WarningCallback);

            // WORDSNET-14302 Theme should be loaded before main document since it is needed for font resolution that occurs
            // on accessing the Font property of a run. The Font property may be called on reading/parsing fields.
            DocxThemeReader.Read(this);

            ReadDocument();
            ResolveMarkup();

            DocxVbaReader.Read(this);
            DocxCustomizationsReader.Read(this);

            ReadGlossary();
            ValidatePageSize();
        }

        /// <summary>
        /// Imports the "Main Document" package part.
        /// </summary>
        private void ReadDocument()
        {
            // WORDSNET-6928 It will be needed to determine whether we should trim leading and trailing white-spaces from runs.
            PreserveSpace = (XmlReader.ReadAttribute("space", "") == "preserve");

            while (XmlReader.ReadChild("document"))
            {
                switch (XmlReader.LocalName)
                {
                    case "background":
                        DocxBackgroundReader.Read(this);
                        break;
                    case "body":
                        ReadBody(mMainDoc);
                        break;
                    default:
                        ResilientBodyRead();
                        break;
                }
            }
            EndSection();
        }

        /// <summary>
        /// Read content outside body. 
        /// <remarks>andrnosk: WORDSNET-5252</remarks>
        /// </summary>
        private void ResilientBodyRead()
        {
            // Document can contain some content outside body, if such content encountered before body,
            // we should start a new section. 
            StartSection(mMainDoc);

            // Read content outside body into the current section.
            mStoryReader.ReadChild(this);
        }

        private void ReadGlossary()
        {
            OpcPackagePart glossaryPart = GetPartByRelationshipType(DocumentPart, RelTypes.GlossaryDocument);
            if (glossaryPart == null)
                return;

            mMainDoc.GlossaryDocument = new GlossaryDocument();
            DocxGlossaryReader glossaryReader = DocxReaderFactory.CreateGlossaryReader(
                Package, glossaryPart, mMainDoc.GlossaryDocument, LoadOptions, mMainDoc.ComplianceInfo);
            glossaryReader.Read();
            glossaryReader.ResolveMarkup();
        }

        /// <summary>
        /// Validates page size. It should not exceeds the limit.
        /// </summary>
        private void ValidatePageSize()
        {
            // WORDSNET-20656 We need to truncate page size if it's too big.
            // The spec says that the maximum page size is 31680 twips, but Word actually
            // truncates it to 61056 twips (3052.8 points), so do we.

            const double maximumSize = 3052.8;

            foreach (Section section in Document.Sections)
            {
                if (section.PageSetup.PageWidth > maximumSize)
                {
                    section.PageSetup.PageWidth = maximumSize;
                    Warn(WarningType.MinorFormattingLoss, WarningSource.Docx, "PageWidth has been truncated.");
                }

                if (section.PageSetup.PageHeight > maximumSize)
                {
                    section.PageSetup.PageHeight = maximumSize;
                    Warn(WarningType.MinorFormattingLoss, WarningSource.Docx, "PageHeight has been truncated.");
                }
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppRenameEntity("Doc")] // Rename for C++, 'new' keyword is not properly handled.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Document Document    // Re-expose this as Document instead of DocumentBase.
        {
            get { return mMainDoc; }
        }

        private readonly Document mMainDoc;
        private readonly DocxStoryReader mStoryReader;
    }
}
