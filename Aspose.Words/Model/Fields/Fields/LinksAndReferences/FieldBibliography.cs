// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Common;
using Aspose.Words.Bibliography;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the BIBLIOGRAPHY field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>Inserts the contents of the document's Bibliography part in a bibliographic style.</remarks>
    public class FieldBibliography : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            Document document = FetchDocument();
            Bibliography.Bibliography bibliography = document.Bibliography;

            if (!bibliography.EnsureTransform())
                return null;

            if (bibliography.Sources.Count == 0)
                return new FieldUpdateActionInsertErrorMessage(this, MissedSourcesErrorMessage);

            ICollection<Source> sources = CollectSources(bibliography.Sources);
            if (sources.Count == 0)
                return new FieldUpdateActionInsertErrorMessage(this, MissedSourcesErrorMessage);

            using (Stream xml = BuildBibliographyXml(sources))
            {
                if (xml == null)
                    return new FieldUpdateActionInsertErrorMessage(this, MissedSourcesErrorMessage);

                // FOSS This is the case where HTML has to be processed.
                throw new NotSupportedException("FOSS");
            }
        }

        private static string TransformBibliographyXml(Stream xml, Bibliography.Bibliography bibliography)
        {
            using (Stream result = bibliography.Transform(xml))
            {
                if (result == null)
                    return null;

                using (StreamReader reader = new StreamReader(result))
                    return reader.ReadToEnd();
            }
        }

        private Stream BuildBibliographyXml(IEnumerable<Source> sources)
        {
            return new BibliographyXmlBuilder("Bibliography", GetEffectiveDefaultLcid())
                .WithSources(sources)
                .BuildXml();
        }

        private ICollection<Source> CollectSources(ICollection<Source> sources)
        {
            IEnumerable<string> sourceTags = GetSourceTags();
            return sourceTags != null
                ? CollectSourcesByTags(sources, sourceTags)
                : CollectSourcesByFilter(sources);
        }

        private ICollection<Source> CollectSourcesByFilter(ICollection<Source> sources)
        {
            List<Source> result = new List<Source>();
            NullableInt32 filterLanguageId = GetEffectiveFilterLcid();

            foreach (Source source in sources)
            {
                if (FilterSource(source, filterLanguageId))
                    result.Add(source);
            }

            return result;
        }

        private static ICollection<Source> CollectSourcesByTags(ICollection<Source> sources, IEnumerable<string> sourceTags)
        {
            List<Source> result = new List<Source>();

            foreach (string sourceTag in sourceTags)
            {
                Source source = GetFirstSourceByTag(sources, sourceTag);
                if (source != null)
                    result.Add(source);
            }

            return result;
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

        private static bool FilterSource(Source source, NullableInt32 languageId)
        {
            if (!languageId.HasValue)
                return true;

            return source.NormalizedLcid == languageId.Value;
        }

        private int GetEffectiveDefaultLcid()
        {
            string lcid = GetDefaultLcid();
            return FieldBibliographyUtils.GetEffectiveLcid(lcid);
        }

        private string GetDefaultLcid()
        {
            string filterLanguageId = FilterLanguageId;
            if (!string.IsNullOrEmpty(filterLanguageId))
                return filterLanguageId;

            string formatLanguageId = FormatLanguageId;
            if (!string.IsNullOrEmpty(formatLanguageId))
                return formatLanguageId;

            return null;
        }

        private NullableInt32 GetEffectiveFilterLcid()
        {
            if (!FieldCodeCache.HasSwitch(FilterLanguageIdSwitch))
                return NullableInt32.Null;

            string filterLanguageId = FilterLanguageId;

            if (filterLanguageId == string.Empty)
                return gUnknownLcid;

            if (string.IsNullOrEmpty(filterLanguageId))
                return FieldBibliographyUtils.GetCurrentCultureLcid().AsNullable();

            return FieldBibliographyUtils.GetEffectiveLcid(filterLanguageId).AsNullable();
        }

        private IEnumerable<string> GetSourceTags()
        {
            if (!FieldCodeCache.HasSwitch(SourceTagSwitch))
                return null;

            IList<string> sourceTags = FieldCodeCache.GetSwitchArgumentsAsStrings(SourceTagSwitch);
            if (sourceTags.Count <= MaxSourceTagSwitchesCount)
                return sourceTags;

            string[] top10SourceTags = new string[MaxSourceTagSwitchesCount];
            for (int i = 0; i < top10SourceTags.Length; i++)
                top10SourceTags[i] = sourceTags[i];

            return top10SourceTags;
        }

        /// <summary>
        /// Gets or sets the language ID that is used to format the bibliographic sources in the document.
        /// </summary>
        public string FormatLanguageId
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FormatLanguageIdSwitch); }
            set { FieldCodeCache.SetSwitch(FormatLanguageIdSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the language ID that is used to filter the bibliographic data to only the sources in the document that
        /// use that language.
        /// </summary>
        public string FilterLanguageId
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FilterLanguageIdSwitch); }
            set { FieldCodeCache.SetSwitch(FilterLanguageIdSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a value so that only the sources with matching Tag element value are displayed in the bibliography.
        /// </summary>
        public string SourceTag
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(SourceTagSwitch); }
            set { FieldCodeCache.SetSwitch(SourceTagSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case FormatLanguageIdSwitch:
                case FilterLanguageIdSwitch:
                case SourceTagSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        private const string FormatLanguageIdSwitch = "\\l";
        private const string FilterLanguageIdSwitch = "\\f";
        private const string SourceTagSwitch = "\\m";
        private const string MissedSourcesErrorMessage = "There are no sources in the current document.";

        private const int MaxSourceTagSwitchesCount = 10;

        private static readonly NullableInt32 gUnknownLcid = new NullableInt32(-1);
    }
}
