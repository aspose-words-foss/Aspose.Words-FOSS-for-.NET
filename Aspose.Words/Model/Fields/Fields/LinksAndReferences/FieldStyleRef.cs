// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/07/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the STYLEREF field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// The STYLEREF is used to reference a fragment of text within the document that is formatted with
    /// the specified style.
    /// </remarks>
    public class FieldStyleRef : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateStage GetUpdateStage()
        {
            return FieldUpdateStage.DeferredUpdateRef;
        }

        internal override FieldUpdateAction UpdateCore()
        {
            string styleName = StyleName;
            if (!StringUtil.HasChars(styleName))
                return new FieldUpdateActionInsertErrorMessage(this, NoStyleNameGivenErrorMessage);

            // List labels should be up-to-date before the final update of the field.
            Updater.RequestExternalAction(new ExternalActionUpdateListLabels(FetchDocument()), FieldUpdateStage.DeferredUpdateRef);

            StyleSearchResult searchResult = StyleFinder.FindStyle(this, styleName);
            if (searchResult == null)
                return new FieldUpdateActionInsertErrorMessage(this, NoTextOfStyleErrorMessage);

            string result = GetParagraphNumberOrRelativePosition(searchResult);
            if (result != null)
                return new FieldUpdateActionApplyResult(this, result);

            return new FieldUpdateActionApplyResult(this, ExtractTextFromSearchResult(searchResult), ParagraphBreakCharReplacement.ParagraphBreakChar);
        }

        private string GetParagraphNumberOrRelativePosition(StyleSearchResult searchResult)
        {
            if (InsertParagraphNumber)
            {
                return FieldRefUtil.GetParagraphNumber(
                    this,
                    null,
                    searchResult.Paragraph,
                    SuppressNonDelimiters,
                    InsertRelativePosition);
            }

            if (InsertTrimmedParagraphNumberUndocumented)
            {
                // WORDSNET-11300 MS Word handles '\s' switch similar to '\n' switch, but not equally.
                return FieldRefUtil.GetTrimmedParagraphNumber(
                    this,
                    searchResult.Paragraph,
                    InsertRelativePosition);
            }

            if (InsertParagraphNumberInRelativeContext)
            {
                return FieldRefUtil.GetParagraphNumberInRelativeContext(
                    this,
                    null,
                    searchResult.Paragraph,
                    Start.ParentParagraph,
                    SuppressNonDelimiters,
                    null,
                    InsertRelativePosition);
            }

            if (InsertParagraphNumberInFullContext)
            {
                return FieldRefUtil.GetParagraphNumberInFullContext(
                    this,
                    null,
                    searchResult.Paragraph,
                    SuppressNonDelimiters,
                    null,
                    InsertRelativePosition);
            }

            if (InsertRelativePosition)
            {
                string result = FieldRefUtil.GetRelativePosition(this, !searchResult.IsForwardDirection);
                if (!string.IsNullOrEmpty(result))
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Extract up to 256 symbols.
        /// </summary>
        /// <param name="searchResult"></param>
        private string ExtractTextFromSearchResult(StyleSearchResult searchResult)
        {
            const int maxTextLength = 256;  // Word's limitation.

            string result;
            if (searchResult.HasIndexRange)
            {
                int startIndex = System.Math.Min(searchResult.StartIndex, searchResult.EndIndex);
                int endIndex = System.Math.Max(searchResult.StartIndex, searchResult.EndIndex);

                NodeCollection childNodes = StyleFinder.GetParagraphInlines(searchResult.Paragraph);
                Node start = childNodes[startIndex];
                Node end = childNodes[endIndex];

                // FOSS

                NodeTextCollectorOptions collectorOptions = new NodeTextCollectorOptions();
                collectorOptions.IsFieldResultMode = true;
                collectorOptions.AllowHiddenText = false;
                // FOSS

                result = NodeTextCollector.GetText(start, true, end, true, collectorOptions)
                    .Trim()
                    .Replace(ControlChar.NonBreakingSpaceChar, ControlChar.SpaceChar);

                if (result.Length > maxTextLength)
                    result = result.Substring(0, maxTextLength);
            }
            else
            {
                result = string.Empty;
            }

            if ((result.Length < maxTextLength) && searchResult.IsParagraphBreakIncluded)
            {
                // MS Word inserts a run with the carriage return character instead of a new paragraph here.
                result += ControlChar.ParagraphBreak;
            }

            return result;
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case SearchFromBottomSwitch:
                case InsertParagraphNumberSwitch:
                case InsertTrimmedParagraphNumberSwitchUndocumented:
                case InsertRelativePositionSwitch:
                case InsertParagraphNumberInRelativeContextSwitch:
                case SuppressNonDelimitersSwitch:
                case InsertParagraphNumberInFullContextSwitch:
                    return FieldSwitchType.Flag;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        /// <summary>
        /// Gets or sets the name of the style by which the text to search for is formatted.
        /// </summary>
        public string StyleName
        {
            get { return FieldCodeCache.GetArgumentAsString(StyleNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(StyleNameArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets whether to search from the bottom of the current page, rather from the top.
        /// </summary>
        public bool SearchFromBottom
        {
            get { return FieldCodeCache.HasSwitch(SearchFromBottomSwitch); }
            set { FieldCodeCache.SetSwitch(SearchFromBottomSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the paragraph number of the referenced paragraph exactly as it appears in the document.
        /// </summary>
        public bool InsertParagraphNumber
        {
            get { return FieldCodeCache.HasSwitch(InsertParagraphNumberSwitch); }
            set { FieldCodeCache.SetSwitch(InsertParagraphNumberSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the relative position of the referenced paragraph.
        /// </summary>
        public bool InsertRelativePosition
        {
            get { return FieldCodeCache.HasSwitch(InsertRelativePositionSwitch); }
            set { FieldCodeCache.SetSwitch(InsertRelativePositionSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the paragraph number of the referenced paragraph in relative context.
        /// </summary>
        public bool InsertParagraphNumberInRelativeContext
        {
            get { return FieldCodeCache.HasSwitch(InsertParagraphNumberInRelativeContextSwitch); }
            set { FieldCodeCache.SetSwitch(InsertParagraphNumberInRelativeContextSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to suppress non-delimiter characters.
        /// </summary>
        public bool SuppressNonDelimiters
        {
            get { return FieldCodeCache.HasSwitch(SuppressNonDelimitersSwitch); }
            set { FieldCodeCache.SetSwitch(SuppressNonDelimitersSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the paragraph number of the referenced paragraph in full context.
        /// </summary>
        public bool InsertParagraphNumberInFullContext
        {
            get { return FieldCodeCache.HasSwitch(InsertParagraphNumberInFullContextSwitch); }
            set { FieldCodeCache.SetSwitch(InsertParagraphNumberInFullContextSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the paragraph number of the referenced paragraph exactly as it appears in the document.
        /// This switch is undocumented in a Word's documentation but Word handles one as "\n" switch.
        /// </summary>
        private bool InsertTrimmedParagraphNumberUndocumented
        {
            get { return FieldCodeCache.HasSwitch(InsertTrimmedParagraphNumberSwitchUndocumented); }
         }

        private const int StyleNameArgumentIndex = 0;

        private const string SearchFromBottomSwitch = "\\l";
        private const string InsertParagraphNumberSwitch = "\\n";
        private const string InsertTrimmedParagraphNumberSwitchUndocumented = "\\s";
        private const string InsertRelativePositionSwitch = "\\p";
        private const string InsertParagraphNumberInRelativeContextSwitch = "\\r";
        private const string SuppressNonDelimitersSwitch = "\\t";
        private const string InsertParagraphNumberInFullContextSwitch = "\\w";
        internal const string NoTextOfStyleErrorMessage = "Error! No text of specified style in document.";
        private const string NoStyleNameGivenErrorMessage = "Error! No style name given.";
    }
}
