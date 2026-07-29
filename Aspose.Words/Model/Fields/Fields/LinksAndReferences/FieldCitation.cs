// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

using System.Collections.Generic;
using System.IO;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Bibliography;
using Aspose.Words.Loading;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the CITATION field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts the contents of the <b>Source</b> element with a specified <b>Tag</b> element using a bibliographic style.
    /// </remarks>
    public class FieldCitation : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            Bibliography.Bibliography bibliography = FetchDocument().Bibliography;

            if (!bibliography.EnsureTransform())
                return null;

            string sourceTag = SourceTag;
            if (string.IsNullOrEmpty(sourceTag))
                return new FieldUpdateActionInsertErrorMessage(this, MissedSourcesErrorMessage);

            IEnumerable<Stream> xmls = BuildCitationsXmls(bibliography.Sources, sourceTag);
            if (xmls == null)
                return new FieldUpdateActionInsertErrorMessage(this, InvalidSourcesErrorMessage);

            ICollection<Stream> htmls;
            using (new CompositeDisposable<Stream>(xmls))
                htmls = TransformCitationsXmls(xmls, bibliography);

            if (htmls == null)
                return new FieldUpdateActionInsertErrorMessage(this, InvalidSourcesErrorMessage);

            using (new CompositeDisposable<Stream>(htmls))
            using (UpdateContext.RemoveOldResultSafe())
            {
                AppendLeadingSpaceIfNeeded();

                foreach (Stream html in htmls)
                    AppendHtml(html);
            }

            return new FieldUpdateActionDoNothing(this);
        }

        private IList<Citation> CollectCitations(
            ICollection<Source> sources,
            string sourceTag,
            ISetGeneric<Person> repeatedAuthors,
            ISetGeneric<string> nonUniqueLastNames)
        {
            Citation lastCitation = FindCitation(sources, sourceTag, repeatedAuthors, nonUniqueLastNames);
            if (lastCitation == null)
                return null;

            List<Citation> result = new List<Citation>();
            result.Add(lastCitation);

            int switchesCount = 0;
            foreach (FieldSwitch fieldSwitch in FieldCodeCache.Switches)
            {
                string switchName = fieldSwitch.InvariantName;
                switch (switchName)
                {
                    case AnotherSourceTagSwitch:
                        string anotherSourceTag = fieldSwitch.GetArgumentNormalizedText();
                        lastCitation = FindCitation(sources, anotherSourceTag, repeatedAuthors, nonUniqueLastNames);
                        if (lastCitation == null)
                            return null;

                        result.Add(lastCitation);
                        break;
                    case SuppressAuthorSwitch:
                        lastCitation.SuppressAuthor = true;
                        break;
                    case SuppressTitleSwitch:
                        lastCitation.SuppressTitle = true;
                        break;
                    case SuppressYearSwitch:
                        lastCitation.SuppressYear = true;
                        break;
                    case PrefixSwitch:
                        lastCitation.Prefix = fieldSwitch.GetArgumentNormalizedText();
                        break;
                    case SuffixSwitch:
                        lastCitation.Suffix = fieldSwitch.GetArgumentNormalizedText();
                        break;
                    case VolumeNumberSwitch:
                        lastCitation.VolumeNumber = fieldSwitch.GetArgumentNormalizedText();
                        break;
                    case PageNumberSwitch:
                        lastCitation.PageNumber = fieldSwitch.GetArgumentNormalizedText();
                        break;
                    default:
                        break;
                }

                // MS Word takes into account only first 10 field specific switches.
                if (GetSwitchTypeInternal(switchName) != FieldSwitchType.Unknown && (++switchesCount == MaxEffectiveSwitchesCount))
                    break;
            }

            return result;
        }

        private static Citation FindCitation(
            ICollection<Source> sources,
            string sourceTag,
            ISetGeneric<Person> repeatedAuthors,
            ISetGeneric<string> nonUniqueLastNames)
        {
            Source source = GetFirstSourceByTag(sources, sourceTag);
            if (source == null)
                return null;

            bool repeatedAuthor = IsRepeatedAuthor(source, repeatedAuthors);
            bool nonUniqueLastName = IsNonUniqueLastName(source, nonUniqueLastNames);

            return new Citation(source, repeatedAuthor, nonUniqueLastName);
        }

        private static Source GetFirstSourceByTag(IEnumerable<Source> sources, string sourceTag)
        {
            foreach (Source source in sources)
            {
                if (source.Tag == sourceTag)
                    return source;
            }

            return null;
        }

        private void AppendLeadingSpaceIfNeeded()
        {
            if (!NeedAppendLeadingSpace())
                return;

            End.ParentNode.InsertBefore(new Run(Document, " "), End);
        }

        private bool NeedAppendLeadingSpace()
        {
            NullableBool leadingSpace = NeedAppendLeadingSpace(Start.PreviousNonAnnotationSibling, true);
            if (leadingSpace != NullableBool.NotDefined)
                return leadingSpace == NullableBool.True;

            Paragraph previousPara = Start.ParentParagraph.PreviousNonAnnotationSibling as Paragraph;
            if (previousPara == null)
                return false;

            leadingSpace = NeedAppendLeadingSpace(previousPara.LastNonAnnotationChild, false);
            return NullableBoolUtil.GetValueOrDefault(leadingSpace,false);
        }

        private static NullableBool NeedAppendLeadingSpace(Node node, bool appendIfWhitespace)
        {
            while (node != null)
            {
                if (node is FieldChar)
                    return NullableBool.True;

                if (node.GetTextLength() != 0)
                {
                    string text = node.GetText();
                    bool isWhitespace = StringUtil.IsWhiteSpace(text[text.Length - 1]);
                    return NullableBoolUtil.AsNullable(isWhitespace == appendIfWhitespace);
                }

                node = node.PreviousNonAnnotationSibling;
            }

            return NullableBool.NotDefined;
        }

        private void AppendHtml(Stream html)
        {
            Document document = new Document(html, new HtmlLoadOptions());

            Section section = document.FirstSection;
            if (section == null)
                return;

            Body body = section.Body;
            if (body == null)
                return;

            Paragraph paragraph = body.FirstParagraph;
            if (paragraph == null)
                return;

            NodeImporter importer = new NodeImporter(document, Document, ImportFormatMode.UseDestinationStyles);
            paragraph = (Paragraph)importer.ImportNode(paragraph, true);

            End.InsertPrevious(paragraph.FirstChild, null);
        }

        private static ICollection<Stream> TransformCitationsXmls(
            IEnumerable<Stream> xmls,
            Bibliography.Bibliography bibliography)
        {
            List<Stream> result = new List<Stream>();

            foreach (Stream xml in xmls)
            {
                Stream html = bibliography.Transform(xml);
                if (html == null)
                    return null;

                result.Add(html);
            }

            return result;
        }

        private ICollection<Stream> BuildCitationsXmls(ICollection<Source> sources, string sourceTag)
        {
            List<Stream> result = new List<Stream>();

            ISetGeneric<Person> repeatedAuthors = FindRepeatedAuthors(sources);
            ISetGeneric<string> nonUniqueLastNames = FindNonUniqueLastNames(sources);

            IList<Citation> citations = CollectCitations(sources, sourceTag, repeatedAuthors, nonUniqueLastNames);
            if (citations == null)
                return null;

            int defaultLcid = FieldBibliographyUtils.GetEffectiveLcid(FormatLanguageId);

            for (int i = 0; i < citations.Count; i++)
            {
                bool isFirst = i == 0;
                bool isLast = i == citations.Count - 1;

                Stream xml = BuildCitationXml(citations[i], defaultLcid, isFirst, isLast);

                result.Add(xml);
            }

            return result;
        }

        private static ISetGeneric<Person> FindRepeatedAuthors(IEnumerable<Source> sources)
        {
            HashSetGeneric<Person> allPersons = new HashSetGeneric<Person>();
            HashSetGeneric<Person> repeatedPersons = new HashSetGeneric<Person>();

            foreach (Source source in sources)
            {
                IList<Person> persons = source.ExpandPersons();

                foreach (Person person in persons)
                {
                    if (allPersons.Contains(person))
                        repeatedPersons.Add(person);
                }

                foreach (Person person in persons)
                    allPersons.Add(person);
            }

            return repeatedPersons;
        }

        private static ISetGeneric<string> FindNonUniqueLastNames(IEnumerable<Source> sources)
        {
            Dictionary<string, HashSetGeneric<Person>> allLastNames =
                new Dictionary<string, HashSetGeneric<Person>>();

            foreach (Source source in sources)
            {
                foreach (Person person in source.ExpandPersons())
                {
                    HashSetGeneric<Person> lastName;
                    if (!allLastNames.TryGetValue(person.Last, out lastName))
                    {
                        lastName = new HashSetGeneric<Person>();
                        allLastNames.Add(person.Last, lastName);
                    }

                    lastName.Add(person);
                }
            }

            HashSetGeneric<string> nonUniqueLastNames = new HashSetGeneric<string>();
            foreach (KeyValuePair<string, HashSetGeneric<Person>> pair in allLastNames)
            {
                if (pair.Value.Count > 1)
                    nonUniqueLastNames.Add(pair.Key);
            }

            return nonUniqueLastNames;
        }

        private static bool IsRepeatedAuthor(Source source, ISetGeneric<Person> repeatedAuthors)
        {
            foreach (Person person in source.ExpandPersons())
            {
                if (repeatedAuthors.Contains(person))
                    return true;
            }

            return false;
        }

        private static bool IsNonUniqueLastName(Source source, ISetGeneric<string> nonUniqueLastNames)
        {
            foreach (Person person in source.ExpandPersons())
            {
                if (nonUniqueLastNames.Contains(person.Last))
                    return true;
            }

            return false;
        }

        private static Stream BuildCitationXml(Citation citation, int defaultLcid, bool isFirst, bool isLast)
        {
            int effectiveDefaultLcid = citation.Source.HasNormalizedLcid ? citation.Source.NormalizedLcid : defaultLcid;

            return new BibliographyXmlBuilder("Citation", effectiveDefaultLcid)
                .WithOptionIf("FirstAuthor", isFirst)
                .WithOptionIf("LastAuthor", isLast)
                .WithOptionIf("RepeatedAuthor", citation.RepeatedAuthor)
                .WithOptionIf("NonUniqueLastName", citation.NonUniqueLastName)
                .WithOptionIf("NoAuthor", citation.SuppressAuthor)
                .WithOptionIf("NoTitle", citation.SuppressTitle)
                .WithOptionIf("NoYear", citation.SuppressYear)
                .WithOptionIf("PagePrefix", citation.Prefix)
                .WithOptionIf("PageSuffix", citation.Suffix)
                .WithOptionIf("Volume", citation.VolumeNumber)
                .WithOptionIf("Pages", citation.PageNumber)
                .WithSource(citation.Source)
                .BuildXml();
        }

        /// <summary>
        /// Gets or sets a value that matches the <b>Tag</b> element's value of the source to insert.
        /// </summary>
        public string SourceTag
        {
            get { return FieldCodeCache.GetArgumentAsString(SourceTagArgumentIndex); }
            set { FieldCodeCache.SetArgument(SourceTagArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the language ID that is used in conjunction with the specified bibliographic style to format the citation
        /// in the document.
        /// </summary>
        public string FormatLanguageId
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FormatLanguageIdSwitch); }
            set { FieldCodeCache.SetSwitch(FormatLanguageIdSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a prefix that is prepended to the citation.
        /// </summary>
        public string Prefix
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(PrefixSwitch); }
            set { FieldCodeCache.SetSwitch(PrefixSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a suffix that is appended to the citation.
        /// </summary>
        public string Suffix
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(SuffixSwitch); }
            set { FieldCodeCache.SetSwitch(SuffixSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether the author information is suppressed from the citation.
        /// </summary>
        public bool SuppressAuthor
        {
            get { return FieldCodeCache.HasSwitch(SuppressAuthorSwitch); }
            set { FieldCodeCache.SetSwitch(SuppressAuthorSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether the title information is suppressed from the citation.
        /// </summary>
        public bool SuppressTitle
        {
            get { return FieldCodeCache.HasSwitch(SuppressTitleSwitch); }
            set { FieldCodeCache.SetSwitch(SuppressTitleSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether the year information is suppressed from the citation.
        /// </summary>
        public bool SuppressYear
        {
            get { return FieldCodeCache.HasSwitch(SuppressYearSwitch); }
            set { FieldCodeCache.SetSwitch(SuppressYearSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a page number associated with the citation.
        /// </summary>
        public string PageNumber
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(PageNumberSwitch); }
            set { FieldCodeCache.SetSwitch(PageNumberSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a volume number associated with the citation.
        /// </summary>
        public string VolumeNumber
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(VolumeNumberSwitch); }
            set { FieldCodeCache.SetSwitch(VolumeNumberSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a value that matches the <b>Tag</b> element's value of another source to be included in the citation.
        /// </summary>
        public string AnotherSourceTag
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(AnotherSourceTagSwitch); }
            set { FieldCodeCache.SetSwitch(AnotherSourceTagSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            return GetSwitchTypeInternal(switchName);
        }

        private static FieldSwitchType GetSwitchTypeInternal(string switchName)
        {
            switch (switchName)
            {
                case SuppressAuthorSwitch:
                case SuppressTitleSwitch:
                case SuppressYearSwitch:
                    return FieldSwitchType.Flag;
                case FormatLanguageIdSwitch:
                case PrefixSwitch:
                case SuffixSwitch:
                case PageNumberSwitch:
                case VolumeNumberSwitch:
                case AnotherSourceTagSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        private const int SourceTagArgumentIndex = 0;

        private const string FormatLanguageIdSwitch = "\\l";
        private const string PrefixSwitch = "\\f";
        private const string SuffixSwitch = "\\s";
        private const string PageNumberSwitch = "\\p";
        private const string VolumeNumberSwitch = "\\v";
        private const string SuppressAuthorSwitch = "\\n";
        private const string SuppressTitleSwitch = "\\t";
        private const string SuppressYearSwitch = "\\y";
        private const string AnotherSourceTagSwitch = "\\m";
        private const string InvalidSourcesErrorMessage = "Invalid source specified.";
        private const string MissedSourcesErrorMessage = "No source specified.";

        private const int MaxEffectiveSwitchesCount = 10;

        private class Citation
        {
            internal Citation(Source source, bool repeatedAuthor, bool nonUniqueLastName)
            {
                Source = source;
                RepeatedAuthor = repeatedAuthor;
                NonUniqueLastName = nonUniqueLastName;
            }

            internal Source Source { get; }
            internal bool RepeatedAuthor { get; }
            internal bool NonUniqueLastName { get; }
            internal bool SuppressAuthor { get; set; }
            internal bool SuppressTitle { get; set; }
            internal bool SuppressYear { get; set; }
            internal string Prefix { get; set; }
            internal string Suffix { get; set; }
            internal string VolumeNumber { get; set; }
            internal string PageNumber { get; set; }
        }
    }
}
