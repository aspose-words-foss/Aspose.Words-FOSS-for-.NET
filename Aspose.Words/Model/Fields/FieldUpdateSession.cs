// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/04/2015 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a common state shared between separated fields update invocations.
    /// </summary>
    /// <remarks>
    /// During fields updating in headers/footers session is used to hold data providers and
    /// to prevent from multiple performing of external actions.
    /// </remarks>
    internal class FieldUpdateSession
    {
        internal FieldUpdateSession(Document document)
        {
            ExternalActions = new ExternalActionCollection();
            DataProviderCollection = new FieldUpdateDataProviderCollection();
            HiddenAttributeCache = new HiddenAttributeCache();
            ImportedStylesIstdsCollector = new ImportedStylesIstdsCollector();
            AutoTextEntryExtractor = new AutoTextEntryExtractor(document);
            FieldParagraphFinderCache = new FieldParagraphFinderCache();
            HasNumPagesCoverPageOffset = false;
        }

        /// <summary>
        /// Returns a collection of external actions.
        /// </summary>
        internal ExternalActionCollection ExternalActions { get; }

        /// <summary>
        /// Returns a collection of <see cref="IFieldUpdateDataProvider"/>.
        /// </summary>
        internal FieldUpdateDataProviderCollection DataProviderCollection { get; }

        internal HiddenAttributeCache HiddenAttributeCache { get; }

        internal ImportedStylesIstdsCollector ImportedStylesIstdsCollector { get; }

        internal AutoTextEntryExtractor AutoTextEntryExtractor { get; }

        internal FieldParagraphFinderCache FieldParagraphFinderCache { get; }

        internal int NumPagesCoverPageOffset
        {
            get { return mNumPagesCoverPageOffset; }
            set
            {
                mNumPagesCoverPageOffset = value;
                HasNumPagesCoverPageOffset = true;
            }
        }

        private int mNumPagesCoverPageOffset;

        internal bool HasNumPagesCoverPageOffset { get; set; }
    }
}
