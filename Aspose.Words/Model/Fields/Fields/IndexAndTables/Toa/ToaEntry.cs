// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2017 by Edward Voronov

using System.Collections.Generic;
using System.Text;

namespace Aspose.Words.Fields
{
    internal class ToaEntry
    {
        internal ToaEntry(NodeRange range)
        {
            Range = range;
            mRangeDocument = range.Document;
        }

        internal NodeRange Range { get; }

        internal IList<Run> BuildPagesRuns(DocumentBase document, bool usePassim, string pageNumberListSeparator, string sequenceSeparator, string pageRangeSeparator)
        {
            if (usePassim && IsPassim())
                return BuildPassim(document);

            PagesRunsBuilder builder = new PagesRunsBuilder(document);
            BuildPagesRuns(builder, mPages, pageNumberListSeparator, pageRangeSeparator, sequenceSeparator);
            return builder.Runs;
        }

        private bool IsPassim()
        {
            const int limit = 5;
            if (mPages.Count < limit)
                return false;

            ToaEntryPage last = mPages[0].Page;
            int duplicates = 0;
            for (int i = 1; i < mPages.Count; i++)
            {
                ToaEntryPage current = mPages[i].Page;

                if (current.Equals(last) || AreDuplicates(current, last) || AreDuplicates(last, current))
                    duplicates++;

                last = current;
            }

            return mPages.Count - duplicates >= limit;
        }

        private static bool AreDuplicates(ToaEntryPage x, ToaEntryPage y)
        {
            ToaEntrySinglePage singlePage = x as ToaEntrySinglePage;
            ToaEntryPageRangeError pageRangeError = y as ToaEntryPageRangeError;

            if (singlePage == null || pageRangeError == null)
                return false;

            return singlePage.Number == pageRangeError.Number;
        }

        private static IList<Run> BuildPassim(DocumentBase document)
        {
            List<Run> result = new List<Run>();
            result.Add(new Run(document, "passim"));
            return result;
        }

        private static void BuildPagesRuns(
            PagesRunsBuilder builder,
            IList<IPageContainer> pages,
            string pageNumberListSeparator,
            string pageRangeSeparator,
            string sequenceSeparator)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                IPageContainer container = pages[i];
                SequencedPage sequencedPage = container as SequencedPage;
                ToaEntryPage page = container.Page;

                if (i != 0)
                    builder.Add(pageNumberListSeparator);

                ToaEntrySinglePage singlePage = page as ToaEntrySinglePage;
                if (singlePage != null)
                {
                    AddPageNumber(builder, sequencedPage, sequenceSeparator, singlePage.Number, singlePage.IsBold, singlePage.IsItalic);
                    continue;
                }

                ToaEntryPageRange pageRange = page as ToaEntryPageRange;
                if (pageRange != null)
                {
                    AddPageNumber(builder, sequencedPage, sequenceSeparator, pageRange.Start, pageRange.IsStartBold, pageRange.IsStartItalic);
                    builder.Add(pageRangeSeparator);
                    AddPageNumber(builder, sequencedPage, sequenceSeparator, pageRange.End, pageRange.IsEndBold, pageRange.IsEndItalic);
                    continue;
                }

                ToaEntryPageRangeError pageRangeError = page as ToaEntryPageRangeError;
                if (pageRangeError != null)
                {
                    builder.Add("Error! Not a valid bookmark in entry on page ", gBold);
                    AddPageNumber(builder, sequencedPage, sequenceSeparator, pageRangeError.Number, pageRangeError.IsBold, pageRangeError.IsItalic);
                    continue;
                }

                Debug.Fail("Unexpected ToaEntryPageNumber implementation");
            }
        }

        private static void AddPageNumber(
            PagesRunsBuilder builder,
            SequencedPage sequencedPage,
            string sequenceSeparator,
            int page,
            bool isBold,
            bool isItalic)
        {
            string text = BuildSequencedPageNumberString(sequencedPage, sequenceSeparator, page);
            builder.Add(text, isBold, isItalic);
        }

        private static string BuildSequencedPageNumberString(SequencedPage sequencedPage, string sequenceSeparator, int page)
        {
            if (sequencedPage == null)
                return page.ToString();

            return sequencedPage.SequenceNumber + sequenceSeparator + page;
        }

        internal void AddSinglePage(bool isBold, bool isItalic, int pageNumber, int sequenceNumber)
        {
            AddPage(new ToaEntrySinglePage(isBold, isItalic, pageNumber), sequenceNumber);
        }

        internal void AddPageRange(bool isBold, bool isItalic, int start, int end, int sequenceNumber)
        {
            AddPage(new ToaEntryPageRange(isBold, isItalic, start, end), sequenceNumber);
        }

        internal void AddPageRangeError(bool isBold, bool isItalic, int pageNumber, int sequenceNumber)
        {
            AddPage(new ToaEntryPageRangeError(isBold, isItalic, pageNumber), sequenceNumber);
        }

        private void AddPage(ToaEntryPage page, int sequenceNumber)
        {
            if (sequenceNumber == NullSequenceNumber)
                Append(page);
            else
                Append(new SequencedPage(sequenceNumber, page));
        }

        internal void CopyFrom(ToaEntry entry)
        {
            foreach (IPageContainer page in entry.mPages)
                Append(page);
        }

        private void Append(IPageContainer item)
        {
            IPageContainer last = mPages.Count == 0
                ? null
                : mPages[mPages.Count - 1];

            if (last != null)
            {
                if (last.Equals(item))
                    return;

                if (ReplaceSinglePageWithPageRange(last, item))
                    return;
            }

            mPages.Add(item);
        }

        private bool ReplaceSinglePageWithPageRange(IPageContainer last, IPageContainer item)
        {
            ToaEntrySinglePage lastPage = last.Page as ToaEntrySinglePage;
            if (lastPage == null)
                return false;

            ToaEntryPageRange itemPage = item.Page as ToaEntryPageRange;
            if (itemPage == null)
                return false;

            if (lastPage.Number != itemPage.Start)
                return false;

            SequencedPage lastSeq = last as SequencedPage;
            SequencedPage itemSeq = item as SequencedPage;

            Debug.Assert((lastSeq == null) == (itemSeq == null));

            if ((lastSeq != null) && (itemSeq != null) && (lastSeq.SequenceNumber != itemSeq.SequenceNumber))
                return false;

            itemPage.IsStartBold = lastPage.IsBold;
            itemPage.IsStartItalic = lastPage.IsItalic;
            mPages[mPages.Count - 1] = item;

            return true;
        }

        protected static RunPr GetFormatting(bool isBold, bool isItalic)
        {
            if (isBold && isItalic)
                return gBoldItalic;
            if (isBold)
                return gBold;
            if (isItalic)
                return gItalic;
            return null;
        }

        private readonly List<IPageContainer> mPages = new List<IPageContainer>();
        // ReSharper disable once NotAccessedField.Local - Avoid C++ leaks
        private readonly DocumentBase mRangeDocument;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int NullSequenceNumber = int.MinValue;

        private static readonly RunPr gBold = new RunPr { Bold = AttrBoolEx.True };
        private static readonly RunPr gItalic = new RunPr { Italic = AttrBoolEx.True };
        private static readonly RunPr gBoldItalic = new RunPr { Bold = AttrBoolEx.True, Italic = AttrBoolEx.True };

        private interface IPageContainer
        {
            ToaEntryPage Page { get; }
        }

        [CodePorting.Translator.Cs2Cpp.CppDeclareFriendClass("Aspose.Words.Fields.ToaEntry")] // Mark as fried for ToaEntry to suppress C2248 C++ error (cannot access Page property)
        private abstract class ToaEntryPage : IPageContainer
        {
            ToaEntryPage IPageContainer.Page
            {
                get { return this; }
            }
        }

        private class ToaEntrySinglePage : ToaEntryPage
        {
            internal ToaEntrySinglePage(bool isBold, bool isItalic, int number)
            {
                IsBold = isBold;
                IsItalic = isItalic;
                Number = number;
            }

            internal int Number { get; }

            internal bool IsBold { get; }

            internal bool IsItalic { get; }

            public override int GetHashCode()
            {
                return Number;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals((ToaEntrySinglePage)obj);
            }

            private bool Equals(ToaEntrySinglePage other)
            {
                return Number == other.Number;
            }

#if DEBUG
            public override string ToString()
            {
                return Number.ToString();
            }
#endif
        }

        private class ToaEntryPageRange : ToaEntryPage
        {
            internal ToaEntryPageRange(bool isBold, bool isItalic, int start, int end)
            {
                IsStartBold = isBold;
                IsStartItalic = isItalic;
                IsEndBold = isBold;
                IsEndItalic = isItalic;
                Start = start;
                End = end;
            }

            internal int Start { get; }

            internal int End { get; }

            internal bool IsStartBold { get; set; }

            internal bool IsStartItalic { get; set; }

            internal bool IsEndBold { get; }

            internal bool IsEndItalic { get; }

#if DEBUG
            public override string ToString()
            {
                return string.Format("{0}-{1}", Start, End);
            }
#endif
        }

        private class ToaEntryPageRangeError : ToaEntryPage
        {
            internal ToaEntryPageRangeError(bool isBold, bool isItalic, int number)
            {
                IsBold = isBold;
                IsItalic = isItalic;
                Number = number;
            }

            internal int Number { get; }

            internal bool IsBold { get; }

            internal bool IsItalic { get; }

            public override int GetHashCode()
            {
                return Number;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals((ToaEntryPageRangeError)obj);
            }

            private bool Equals(ToaEntryPageRangeError other)
            {
                return Number == other.Number;
            }

#if DEBUG
            public override string ToString()
            {
                return string.Format("<Error>{0}", Number);
            }
#endif
        }

        [CodePorting.Translator.Cs2Cpp.CppDeclareFriendClass("Aspose.Words.Fields.ToaEntry")]  // Mark as fried for ToaEntry to suppress C2248 C++ error (cannot access Page property)
        private class SequencedPage : IPageContainer
        {
            internal SequencedPage(int sequenceNumber, ToaEntryPage number)
            {
                SequenceNumber = sequenceNumber;
                mNumber = number;
            }

            internal int SequenceNumber { get; }

            ToaEntryPage IPageContainer.Page
            {
                get { return mNumber; }
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (SequenceNumber * 397) ^ mNumber.GetHashCode();
                }
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != typeof(SequencedPage))
                    return false;
                return Equals((SequencedPage)obj);
            }

            private bool Equals(SequencedPage other)
            {
                return SequenceNumber == other.SequenceNumber && Equals(mNumber, other.mNumber);
            }

#if DEBUG
            public override string ToString()
            {
                return string.Format("<{0}>{1}", SequenceNumber, mNumber);
            }
#endif

            private readonly ToaEntryPage mNumber;
        }

        private class PagesRunsBuilder
        {
            internal PagesRunsBuilder(DocumentBase document)
            {
                mDocument = document;
            }

            internal void Add(string text)
            {
                Add(text, null);
            }

            internal void Add(string text, bool isBold, bool isItalic)
            {
                Add(text, GetFormatting(isBold, isItalic));
            }

            internal void Add(string text, RunPr runPr)
            {
                if (runPr == null)
                    runPr = new RunPr();

                if (mRuns.Count > 0)
                {
                    Run lastRun = mRuns[mRuns.Count - 1];
                    // ReSharper disable once RedundantCast - Casting for C++ to avoid C2668 error.
                    if (lastRun.RunPr.Equals((AttrCollection)runPr))
                    {
                        lastRun.Text += text;
                        return;
                    }
                }

                mRuns.Add(new Run(mDocument, text, runPr));
            }

            internal IList<Run> Runs
            {
                get { return mRuns; }
            }

            private readonly DocumentBase mDocument;
            private readonly List<Run> mRuns = new List<Run>();
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(NodeTextCollector.GetText(Range));
            builder.Append(" [");
#if !CPLUSPLUS
            builder.Append(string.Join(", ", mPages));
#else // C++ doesn't support such string.Join()
            string[] pagesStr = new string[mPages.Count];
            for (int idx = 0; idx < mPages.Count; idx++)
            {
                pagesStr[idx] = mPages[idx].ToString();
            }
            builder.Append(string.Join(", ", pagesStr));
#endif
            builder.Append(']');
            return builder.ToString();
        }
#endif
    }
}
