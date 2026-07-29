// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2023 by Edward Voronov

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Aspose.Common;
using Aspose.Xml;

namespace Aspose.Words.Bibliography
{
    /// <summary>
    /// Represents an individual source, such as a book, journal article, or interview.
    /// </summary>
    public sealed class Source
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="Source"/> class.
        /// </summary>
        /// <param name="tag">The identifying tag name.</param>
        /// <param name="sourceType">The source type.</param>
        public Source(string tag, SourceType sourceType)
            : this()
        {
            Tag = tag;
            SourceType = sourceType;
            mVersions = BibliographyVersion.Calculate(this);
        }

        internal Source(string xml)
            : this()
        {
            mXml = xml;
            SourceReader.Read(this, xml);
            mVersions = BibliographyVersion.Calculate(this);
        }

        private Source()
        {
            HasNormalizedLcid = false;
            Contributors = new ContributorCollection();
        }

        /// <summary>
        /// Returns XML representation of a source with normalized lcid.
        /// </summary>
        internal string GetNormalizedXml()
        {
            if (!HasNormalizedLcid)
                return Xml;

            if (NormalizedLcid.ToString() == Lcid)
                return Xml;

            return Xml.Replace(
                string.Format("<b:LCID>{0}</b:LCID>", Lcid),
                string.Format("<b:LCID>{0}</b:LCID>", NormalizedLcid));
        }

        /// <summary>
        /// Returns all person contributors of a source.
        /// </summary>
        internal IList<Person> ExpandPersons()
        {
            List<Person> result = new List<Person>();
            foreach (Contributor author in Contributors)
            {
                PersonCollection persons = author as PersonCollection;
                if (persons != null)
                    result.AddRange(persons);
            }

            return result;
        }

        private string Xml
        {
            get
            {
                int version = BibliographyVersion.Calculate(this);

                if ((mXml != null) && (mVersions == version))
                    return mXml;

                using (MemoryStream stream = new MemoryStream())
                {
                    AnyXmlBuilder builder = new AnyXmlBuilder(stream, true);
                    SourceWriter.Write(this, builder);
                    builder.Flush();

                    stream.Position = 0;

                    using (StreamReader reader = new StreamReader(stream))
                        mXml = reader.ReadToEnd();
                }

                mVersions = version;

                return mXml;
            }
        }

        /// <summary>
        /// Gets or sets the locale ID of a source.
        /// </summary>
        public string Lcid
        {
            get { return mLcid; }
            set
            {
                if (mLcid == value)
                    return;

                mLcid = value;
                NormalizeLocale();
            }
        }

        private void NormalizeLocale()
        {
            NullableInt32 lcid = FormatterPal.ParseNullableInt(Lcid);
            CultureInfo culture = lcid.HasValue
                ? SystemPal.TryGetCulture(lcid.Value)
                : SystemPal.TryGetCulture(Lcid);

            if (culture != null)
            {
                NormalizedLcid = culture.LCID;
                HasNormalizedLcid = true;
            }
            else
            {
                HasNormalizedLcid = false;
            }
        }

        /// <summary>
        ///  Gets the parsed locale ID of a source.
        /// </summary>
        internal int NormalizedLcid { get; private set; }

        internal bool HasNormalizedLcid { get; private set; }

        /// <summary>
        /// Gets contributors list (author, editor, writer etc) of a source.
        /// </summary>
        public ContributorCollection Contributors { get; private set; }

        /// <summary>
        /// Gets or sets the source type of a source.
        /// </summary>
        public SourceType SourceType { get; set; }

        /// <summary>
        /// Gets or sets the abbreviated case number of a source.
        /// </summary>
        public string AbbreviatedCaseNumber { get; set; }

        /// <summary>
        /// Gets or sets the album title of a source.
        /// </summary>
        public string AlbumTitle { get; set; }

        /// <summary>
        /// Gets or sets the book title of a source.
        /// </summary>
        public string BookTitle { get; set; }

        /// <summary>
        /// Gets or sets the broadcaster of a source.
        /// </summary>
        public string Broadcaster { get; set; }

        /// <summary>
        /// Gets or sets the broadcast title of a source.
        /// </summary>
        public string BroadcastTitle { get; set; }

        /// <summary>
        /// Gets or sets the case number of a source.
        /// </summary>
        public string CaseNumber { get; set; }

        /// <summary>
        /// Gets or sets the chapter number of a source.
        /// </summary>
        public string ChapterNumber { get; set; }

        /// <summary>
        /// Gets or sets the city of a source.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the comments of a source.
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the conference or proceedings name of a source.
        /// </summary>
        public string ConferenceName { get; set; }

        /// <summary>
        /// Gets or sets the country or region of a source.
        /// </summary>
        public string CountryOrRegion { get; set; }

        /// <summary>
        /// Gets or sets the court of a source.
        /// </summary>
        public string Court { get; set; }

        /// <summary>
        /// Gets or sets the day of a source.
        /// </summary>
        public string Day { get; set; }

        /// <summary>
        /// Gets or sets the day accessed of a source.
        /// </summary>
        public string DayAccessed { get; set; }

        /// <summary>
        /// Gets or sets the department of a source.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the distributor of a source.
        /// </summary>
        public string Distributor { get; set; }

        /// <summary>
        /// Gets or sets the editor of a source.
        /// </summary>
        public string Edition { get; set; }

        /// <summary>
        /// Gets or sets the guid of a source.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets the institution of a source.
        /// </summary>
        public string Institution { get; set; }

        /// <summary>
        /// Gets or sets the internet site title of a source.
        /// </summary>
        public string InternetSiteTitle { get; set; }

        /// <summary>
        /// Gets or sets the issue of a source.
        /// </summary>
        public string Issue { get; set; }

        /// <summary>
        /// Gets or sets the journal name of a source.
        /// </summary>
        public string JournalName { get; set; }

        /// <summary>
        /// Gets or sets the medium of a source.
        /// </summary>
        public string Medium { get; set; }

        /// <summary>
        /// Gets or sets the month of a source.
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// Gets or sets the month accessed of a source.
        /// </summary>
        public string MonthAccessed { get; set; }

        /// <summary>
        /// Gets or sets the number of volumes of a source.
        /// </summary>
        public string NumberVolumes { get; set; }

        /// <summary>
        /// Gets or sets the pages of a source.
        /// </summary>
        public string Pages { get; set; }

        /// <summary>
        /// Gets or sets the patent number of a source.
        /// </summary>
        public string PatentNumber { get; set; }

        /// <summary>
        /// Gets or sets the periodical title of a source.
        /// </summary>
        public string PeriodicalTitle { get; set; }

        /// <summary>
        /// Gets or sets the production company of a source.
        /// </summary>
        public string ProductionCompany { get; set; }

        /// <summary>
        /// Gets or sets the publication title of a source.
        /// </summary>
        public string PublicationTitle { get; set; }

        /// <summary>
        /// Gets or sets the publisher of a source.
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// Gets or sets the recording number of a source.
        /// </summary>
        public string RecordingNumber { get; set; }

        /// <summary>
        /// Gets or sets the reference order of a source.
        /// </summary>
        public string RefOrder { get; set; }

        /// <summary>
        /// Gets or sets the reporter of a source.
        /// </summary>
        public string Reporter { get; set; }

        /// <summary>
        /// Gets or sets the short title of a source.
        /// </summary>
        public string ShortTitle { get; set; }

        /// <summary>
        /// Gets or sets the standard number of a source.
        /// </summary>
        public string StandardNumber { get; set; }

        /// <summary>
        /// Gets or sets the state or province of a source.
        /// </summary>
        public string StateOrProvince { get; set; }

        /// <summary>
        /// Gets or sets the station of a source.
        /// </summary>
        public string Station { get; set; }

        /// <summary>
        /// Gets or sets the identifying tag name of a source.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the theater of a source.
        /// </summary>
        public string Theater { get; set; }

        /// <summary>
        /// Gets or sets the thesis type of a source.
        /// </summary>
        public string ThesisType { get; set; }

        /// <summary>
        /// Gets or sets the title of a source.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the type of a source.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the url of a source.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the version of a source.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the volume of a source.
        /// </summary>
        public string Volume { get; set; }

        /// <summary>
        /// Gets or sets the year of a source.
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// Gets or sets the year accessed of a source.
        /// </summary>
        public string YearAccessed { get; set; }

        /// <summary>
        /// Gets or sets the digital object identifier.
        /// </summary>
        public string Doi { get; set; }

        internal Source Clone()
        {
            Source lhs = (Source)MemberwiseClone();

            lhs.Contributors = Contributors.Clone();

            return lhs;
        }

        private string mLcid;
        private string mXml;
        private int mVersions;
    }
}
