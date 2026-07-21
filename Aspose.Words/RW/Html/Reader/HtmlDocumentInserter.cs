// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/01/2017 by Alexey Butalov

using System.Collections.Generic;
using System.IO;
using Aspose.Collections.Generic;
using Aspose.Words.Loading;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Reader
{
    internal class HtmlDocumentInserter : IDocumentReader
    {
        internal HtmlDocumentInserter(string html, HtmlInsertOptions insertOptions,
            DocumentBuilder documentBuilder, HashSetGeneric<string> bookmarkNames)
        {
            Debug.Assert(html != null);
            Debug.Assert(documentBuilder != null);
            mInsertOptions = insertOptions;
            mHtml = html;
            mDocumentBuilder = documentBuilder;
            mBookmarkNames = bookmarkNames;
        }

        public void Read()
        {
            Document doc = mDocumentBuilder.Document;
            HtmlResourceLoader resourceLoader = new HtmlResourceLoader(doc.ResourceLoadingCallback, doc.WarningCallback);
            HtmlReaderSettings readerSettings = new HtmlReaderSettings();
            readerSettings.UseHtmlBlocks = (mInsertOptions & HtmlInsertOptions.PreserveBlocks) != 0;
            HtmlReader reader = new HtmlReader(readerSettings, LoadFormat.Html, resourceLoader, mBookmarkNames);

            // WORDSNET-25722 Section formatting of the last section imported from HTML must only be applied
            // if all of the following conditions are true:
            //  - HTML is inserted into the last section of the document;
            //  - the target section is empty;
            //  - the target section has no headers or footers.
            // Otherwise, the last section imported from HTML must have the same formatting as the target section.
            // This is how InsertDocument handles formatting of inserted sections and we mimic this behavior here
            // by saving and restoring the target section formatting appropriately.
            SectionFormatting sectionFormatting = SaveTargetSectionFormatting();

            reader.Read(mHtml, mDocumentBuilder, mInsertOptions, (mBookmarkNames == null));

            RestoreTargetSectionFormatting(sectionFormatting);
        }

        public bool IsEncrypted
        {
            get { return false; }
        }

        public Stream Decrypt()
        {
            Debug.Assert(false, "Not supported");
            return null;
        }

        private SectionFormatting SaveTargetSectionFormatting()
        {
            // If we're inserting into the main story of the last empty section, the last section imported from HTML can
            // be formatted using section formatting specified in HTML. There is no need to preserve target section's
            // formatting. This case also covers the scenario where HTML is inserted into a newly created empty document.
            // In this case, all sections imported from HTML will be formatted as specified in the HTML document.
            if (TargetSectionIsLastAndEmpty())
            {
                return null;
            }

            Section section = mDocumentBuilder.CurrentSection;

            SectionFormatting sectionFormatting = new SectionFormatting();
            sectionFormatting.SectPr = section.SectPr.Clone();

            List<HeaderFooter> headersFooters = new List<HeaderFooter>();
            foreach (HeaderFooter headerFooter in section.HeadersFooters)
            {
                if (!headerFooter.IsLinkedToPrevious)
                {
                    headersFooters.Add(headerFooter);
                }
            }
            sectionFormatting.HeadersFooters = headersFooters.ToArray();

            // WORDSNET-27848 We're removing the section's headers and footers temporarily and will put them back after
            // HTML is inserted. Since these are just thechnical details of how section's headers and footers are preserved,
            // we don't want this removal to be tracked.
            mDocumentBuilder.Document.SuspendTrackRevisions(SuspendedRevisionTypes.All);
            try
            {
                // Remove current section's headers and footers so that headers and footers from HTML can be loaded instead.
                // Original headers and footers will be restored by the calling code after HTML is inserted.
                section.HeadersFooters.Clear();
            }
            finally
            {
                mDocumentBuilder.Document.ResumeTrackRevisions(SuspendedRevisionTypes.All);
            }

            return sectionFormatting;
        }

        private bool TargetSectionIsLastAndEmpty()
        {
            // This also covers the scenario where HTML is being inserted into a header/footer story. In that scenario
            // we don't do anything to current section's properties. If there is no valid target section, we cannot preserve
            // its formatting.
            Body body = mDocumentBuilder.CurrentStory as Body;
            if (body == null)
            {
                return true;
            }

            Section section = body.ParentSection;
            if (section == null)
            {
                return true;
            }

            if (!section.IsLastSection)
            {
                return false;
            }

            foreach (HeaderFooter headerFooter in section.HeadersFooters)
            {
                if (!headerFooter.IsLinkedToPrevious)
                {
                    return false;
                }
            }

            return body.IsSectionBreak;
        }

        private void RestoreTargetSectionFormatting(SectionFormatting savedSectionFormatting)
        {
            if (savedSectionFormatting == null)
            {
                return;
            }

            Section section = mDocumentBuilder.CurrentSection;
            if (section == null)
            {
                return;
            }

            // WORDSNET-27848 The section's headers and footers are removed to be replaced with headers and footers that
            // we had stored before inserting HTML. Since these are just thechnical details of how section's headers and footers
            // are preserved, we don't want these operations to be tracked.
            mDocumentBuilder.Document.SuspendTrackRevisions(SuspendedRevisionTypes.All);
            try
            {
                section.SectPr = savedSectionFormatting.SectPr;

                section.HeadersFooters.Clear();
                foreach (HeaderFooter headerFooter in savedSectionFormatting.HeadersFooters)
                {
                    section.HeadersFooters.Add(headerFooter);
                }
            }
            finally
            {
                mDocumentBuilder.Document.ResumeTrackRevisions(SuspendedRevisionTypes.All);
            }
        }

        private class SectionFormatting
        {
            internal SectPr SectPr;
            internal HeaderFooter[] HeadersFooters;
        }

        private readonly HtmlInsertOptions mInsertOptions;
        private readonly string mHtml;
        private readonly DocumentBuilder mDocumentBuilder;
        private readonly HashSetGeneric<string> mBookmarkNames;
    }
}
