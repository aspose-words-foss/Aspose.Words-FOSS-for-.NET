// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2023 by Edward Voronov

using System.Collections.Generic;
using Aspose.Common;
using Aspose.JavaAttributes;
using Aspose.Words.Bibliography;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests Bibliography facade.
    /// </summary>
    [TestFixture]
    public class TestBibliography
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestBibliographySources()
        {
            Document document = TestUtil.Open(@"Model\Bibliography\TestBibliographySources.docx");

            Assert.That(document.Bibliography.Sources.Count, Is.EqualTo(12));

            IList<Source> sources = document.Bibliography.Sources;

            AssertSource(
                sources[0],
                "BookNoLCID",
                "Book 0 (No LCID)",
                null,
                NullableInt32.Null,
                "Tejeda Roxanne Brielle",
                "Batchelor Kristopher Jaime",
                "Chaffin Tavian Kristopher",
                "Esquivel Dominic Braiden",
                "Brien Debra Janell",
                "David Priscilla Yazmin"
            );
            AssertSource(
                sources[1],
                "BookSectionRuRu",
                "Book Section 1 (ru-RU)",
                "ru-RU",
                1049.AsNullable(),
                "Rossi Jeniffer Daisy",
                "Camp Braxton Julio",
                "Wolford Asia Karleigh",
                "Gooch Carina Jaylen"
            );
            AssertSource(sources[2], "BookEnUs", "Book 2 (en-US)", "en-US", 1033.AsNullable(), "Aspose LTD");
            AssertSource(sources[3], "BookEnNz", "Book 3 (en-NZ)", "en-NZ", 5129.AsNullable());
            AssertSource(sources[4], "Book1049", "Book 4 (1049)", "1049", 1049.AsNullable());
            AssertSource(sources[5], "Book1033", "Book 5 (1033)", "1033", 1033.AsNullable());
            AssertSource(sources[6], "Book5129", "Book 6 (5129)", "5129", 5129.AsNullable());
            AssertSource(sources[7], "BookInvalidStringLCID", "Book 7 (Invalid string LCID)", "Blah", NullableInt32.Null);
            AssertSource(sources[8], "BookInvalidNumberLCID", "Book 8 (Invalid number LCID)", "54321",NullableInt32.Null);
            AssertSource(sources[9], "BookOverflowNumberLCID", "Book 9 (Overflow number LCID)", "123456", NullableInt32.Null);
            AssertSource(sources[10], "BookNegativeNumberLCID", "Book 10 (Negative number LCID)", "-1", NullableInt32.Null);
        }

        [Test]
        public void TestBibliographyMultipleCustomParts()
        {
            Document document = TestUtil.Open(@"Model\Bibliography\TestBibliographyMultipleCustomParts.docx");
            Bibliography.Bibliography bibliography = document.Bibliography;

            Assert.That(bibliography.BibliographyStyle, Is.EqualTo(@"\MLASeventhEditionOfficeOnline.xsl"));
            Assert.That(bibliography.Sources.Count, Is.EqualTo(1));
            AssertSource(bibliography.Sources[0], "Book2", "Book 2", null, NullableInt32.Null);
        }

        private static void AssertSource(
            Source source,
            string tag,
            string title,
            string lcid,
            NullableInt32 normalizedLcid,
            params string[] names)
        {
            Assert.That(source.Tag, Is.EqualTo(tag));
            Assert.That(source.Title, Is.EqualTo(title));
            Assert.That(source.Lcid, Is.EqualTo(lcid));
            Assert.That(source.HasNormalizedLcid, Is.EqualTo(normalizedLcid.HasValue));
            if (normalizedLcid.HasValue)
                Assert.That(source.NormalizedLcid, Is.EqualTo(normalizedLcid.Value));

            List<string> actualNames = new List<string>();
            foreach (Contributor contributor in source.Contributors)
            {
                Corporate corporate = contributor as Corporate;
                if (corporate != null)
                    actualNames.Add(corporate.Name);

                PersonCollection persons = contributor as PersonCollection;
                if (persons != null)
                {
                    foreach (Person person in persons)
                        actualNames.Add(string.Format("{0} {1} {2}", person.Last, person.First, person.Middle));
                }
            }

            Assert.That(actualNames, Is.EqualTo(names));
        }

        [Test]
        public void TestBibliographySource()
        {
            Document document = TestUtil.Open(@"Model\Bibliography\TestBibliographySources.docx");

            Source source = document.Bibliography.Sources[11];

            Assert.That(source.Tag, Is.EqualTo("Aspose"));
            Assert.That(source.AbbreviatedCaseNumber, Is.EqualTo("ABC"));
            Assert.That(source.AlbumTitle, Is.EqualTo("Heart Of Seasons"));
            Assert.That(source.BookTitle, Is.EqualTo("10,000 Hours In View"));
            Assert.That(source.Broadcaster, Is.EqualTo("RadiantEcho TV"));
            Assert.That(source.BroadcastTitle, Is.EqualTo("RETV"));
            Assert.That(source.CaseNumber, Is.EqualTo("ABC-2023-456789"));
            Assert.That(source.ChapterNumber, Is.EqualTo("17"));
            Assert.That(source.City, Is.EqualTo("Atlanta"));
            Assert.That(source.Comments, Is.EqualTo("no comments"));
            Assert.That(source.ConferenceName, Is.EqualTo("InnovateCon Global Summit"));
            Assert.That(source.CountryOrRegion, Is.EqualTo("USA"));
            Assert.That(source.Court, Is.EqualTo("Liberty Judicial Circuit Court"));
            Assert.That(source.Day, Is.EqualTo("3"));
            Assert.That(source.DayAccessed, Is.EqualTo("15"));
            Assert.That(source.Department, Is.EqualTo("Strategic Planning and Development Department"));
            Assert.That(source.Distributor, Is.EqualTo("Pinnacle Distributors, Inc"));
            Assert.That(source.Edition, Is.EqualTo("3rd"));
            Assert.That(source.Guid, Is.EqualTo("{A621825C-D9C4-42A8-A2A2-2BC2C765E417}"));
            Assert.That(source.Institution, Is.EqualTo("Harmony Institute of Advanced Studies"));
            Assert.That(source.InternetSiteTitle, Is.EqualTo("TechInsider Blog"));
            Assert.That(source.Issue, Is.EqualTo("Volume 25, Number 2"));
            Assert.That(source.JournalName, Is.EqualTo("Scientific Advances"));
            Assert.That(source.Lcid, Is.EqualTo("en-US"));
            Assert.That(source.Medium, Is.EqualTo("Document"));
            Assert.That(source.Month, Is.EqualTo("November"));
            Assert.That(source.MonthAccessed, Is.EqualTo("6"));
            Assert.That(source.NumberVolumes, Is.EqualTo("X"));
            Assert.That(source.Pages, Is.EqualTo("197"));
            Assert.That(source.PatentNumber, Is.EqualTo("US9876543"));
            Assert.That(source.PeriodicalTitle, Is.EqualTo("Nature Reviews Neuroscience"));
            Assert.That(source.ProductionCompany, Is.EqualTo("Stellar Studios"));
            Assert.That(source.PublicationTitle, Is.EqualTo("Data Science Today"));
            Assert.That(source.Publisher, Is.EqualTo("Pantheon Media"));
            Assert.That(source.RecordingNumber, Is.EqualTo("RD789012"));
            Assert.That(source.RefOrder, Is.EqualTo("1"));
            Assert.That(source.Reporter, Is.EqualTo("Alice Johnson"));
            Assert.That(source.SourceType, Is.EqualTo(SourceType.Book));
            Assert.That(source.ShortTitle, Is.EqualTo("Manual"));
            Assert.That(source.StandardNumber, Is.EqualTo("0-5245-5105-7"));
            Assert.That(source.StateOrProvince, Is.EqualTo("Georgia"));
            Assert.That(source.Station, Is.EqualTo("TechTalk Radio"));
            Assert.That(source.Tag, Is.EqualTo("Aspose"));
            Assert.That(source.Theater, Is.EqualTo("City Arts Theater"));
            Assert.That(source.ThesisType, Is.EqualTo("Ph.D. Thesis"));
            Assert.That(source.Title, Is.EqualTo("Aspose User Manual"));
            Assert.That(source.Type, Is.EqualTo("Video Documentary"));
            Assert.That(source.Url, Is.EqualTo("http://aspose.com"));
            Assert.That(source.Version, Is.EqualTo("2.1"));
            Assert.That(source.Volume, Is.EqualTo("VI"));
            Assert.That(source.Year, Is.EqualTo("2022"));
            Assert.That(source.YearAccessed, Is.EqualTo("2023"));

            ContributorCollection contributors = source.Contributors;

            AssertPersonsContributor(contributors.Artist, "Doe John A.");
            AssertCorporateContributor(contributors.Author, "Aspose Inc");
            AssertPersonsContributor(contributors.BookAuthor, "Smith Emma L.");
            AssertPersonsContributor(contributors.Compiler, "Johnson Michael R.");
            AssertPersonsContributor(contributors.Composer, "Williams Christopher S.");
            AssertPersonsContributor(contributors.Conductor, "Anderson David J.");
            AssertPersonsContributor(contributors.Counsel, "Miller Olivia K.");
            AssertPersonsContributor(contributors.Director, "Clark Robert P.");
            AssertPersonsContributor(contributors.Editor, "Ryder Demophon Tinashe", "Samara Sargon Shahid");
            AssertPersonsContributor(contributors.Interviewee, "White Jennifer M.");
            AssertPersonsContributor(contributors.Interviewer, "Johnson Andrew S.");
            AssertPersonsContributor(contributors.Inventor, "Adams Richard E.");
            AssertCorporateContributor(contributors.Performer, "Harmony Arts Collective");
            AssertPersonsContributor(contributors.Producer, "Turner Linda B.");
            AssertPersonsContributor(contributors.Translator, "Avellino Chagatai Zoe", "Aliev Deo Vojta", "Hume Otto Somerled");
            AssertPersonsContributor(contributors.Writer, "Anderson William F.");
        }

        private static void AssertCorporateContributor(Contributor contributor, string name)
        {
            Corporate corporate = (Corporate)contributor;
            Assert.That(corporate.Name, Is.EqualTo(name));
        }

        private static void AssertPersonsContributor(Contributor contributor, params string[] names)
        {
            List<string> actualNames = new List<string>();
            PersonCollection persons = (PersonCollection)contributor;
            foreach (Person person in persons)
                actualNames.Add(string.Format("{0} {1} {2}", person.Last, person.First, person.Middle));
            Assert.That(actualNames, Is.EqualTo(names));
        }

        [Test]
        [TestCase(UnifiedScenario.Docx2Docx)]
        [JavaDelete("until fix WORDSJAVA-2881")]
        public void TestBibliographyRoundtrip(UnifiedScenario us)
        {
            Document document = new Document();

            PopulateBibliography(document);

            AssertBibliography(document);

            document = document.Clone();

            AssertBibliography(document);

            document = TestUtil.SaveOpen(document, @"Model\Bibliography\TestBibliographyRoundtrip", us);

            AssertBibliography(document);
        }

        private static void PopulateBibliography(Document document)
        {
            document.Bibliography.BibliographyStyle = "\\APASixthEditionOfficeOnline.xsl";

            Source source;
            PersonCollection contributor;

            #region Art

            source = new Source("Art", SourceType.Art);
            source.Lcid = "1033";
            source.City = "Mumbai";
            source.Comments = "no comments";
            source.CountryOrRegion = "China";
            source.DayAccessed = "14";
            source.Edition = "1";
            source.Guid = "{2079AD07-875B-47C2-BD12-EF0C2A277A68}";
            source.Institution = "Louvre Museum";
            source.Medium = "Journal";
            source.MonthAccessed = "September";
            source.NumberVolumes = "IX";
            source.Pages = "288";
            source.PublicationTitle = "The Song of the Siren";
            source.Publisher = "Wiley";
            source.RefOrder = "9";
            source.ShortTitle = "The Song of the Siren";
            source.StandardNumber = "978-0-307-72037-2";
            source.StateOrProvince = "Michigan";
            source.Title = "The Song of the Siren";
            source.Url = "https://www.spotify.com";
            source.Volume = "IX";
            source.Year = "2001";
            source.YearAccessed = "1995";
            source.Doi = "10.9999/ghij.2222";
            contributor = new PersonCollection();
            contributor.Add(new Person("Madelaine", "Herbert", "Gwyneth"));
            contributor.Add(new Person("Linda", "Wardell", "Wren"));
            source.Contributors.Artist = contributor;
            document.Bibliography.Sources.Add(source);

            #endregion

            #region ArticleInAPeriodical

            source = new Source("ArticleInAPeriodical", SourceType.ArticleInAPeriodical);
            source.Lcid = "1033";
            source.City = "New York City";
            source.Comments = "no comments";
            source.CountryOrRegion = "United States";
            source.Day = "4";
            source.DayAccessed = "5";
            source.Edition = "1";
            source.Guid = "{624F5422-0B8F-4E5B-912A-0BF706D44B37}";
            source.Medium = "Hardcover";
            source.Month = "June";
            source.MonthAccessed = "January";
            source.NumberVolumes = "I";
            source.Pages = "304";
            source.PeriodicalTitle = "The Last Puzzle Piece";
            source.Publisher = "Penguin Random House";
            source.RefOrder = "1";
            source.ShortTitle = "The Last Puzzle Piece";
            source.StandardNumber = "978-0-306-40615-7";
            source.StateOrProvince = "California";
            source.Title = "The Last Puzzle Piece";
            source.Url = "https://www.example.com";
            source.Volume = "I";
            source.Year = "2005";
            source.YearAccessed = "2005";
            source.Doi = "10.1234/abcd.5678";
            contributor = new PersonCollection();
            contributor.Add(new Person("Emma", "Grace", "Smith"));
            contributor.Add(new Person("Liam", "Michael", "Johnson"));
            source.Contributors.Artist = contributor;
            contributor = new PersonCollection();
            contributor.Add(new Person("Lanford", "Corey", "Estella"));
            contributor.Add(new Person("Acacia", "Lacy", "Legacy"));
            source.Contributors.Author = contributor;
            contributor = new PersonCollection();
            contributor.Add(new Person("Anima", "Dax", "Journee"));
            contributor.Add(new Person("Denver", "Kaelyn", "Kristine"));
            source.Contributors.Editor = contributor;
            document.Bibliography.Sources.Add(source);

            #endregion

            #region Book

            source = new Source("Book", SourceType.Book);
            source.Lcid = "1033";
            source.City = "London";
            source.Comments = "no comments";
            source.CountryOrRegion = "Canada";
            source.DayAccessed = "17";
            source.Edition = "2";
            source.Guid = "{46464485-34F1-4892-8828-D055C5630471}";
            source.Medium = "Paperback";
            source.MonthAccessed = "February";
            source.NumberVolumes = "II";
            source.Pages = "432";
            source.Publisher = "HarperCollins";
            source.RefOrder = "2";
            source.ShortTitle = "Whispers of the Moon";
            source.StandardNumber = "978-0-553-38215-9";
            source.StateOrProvince = "New York";
            source.Title = "Whispers of the Moon";
            source.Url = "https://www.openai.org";
            source.Volume = "II";
            source.Year = "1998";
            source.YearAccessed = "2012";
            source.Doi = "10.9876/efgh.5432";
            contributor = new PersonCollection();
            contributor.Add(new Person("Connor", "Hazel", "Destiny"));
            contributor.Add(new Person("Keeley", "Mariah", "Freeman"));
            source.Contributors.Author = contributor;
            contributor = new PersonCollection();
            contributor.Add(new Person("Ora", "Breanna", "Amabel"));
            contributor.Add(new Person("Rosemary", "Aylmer", "Monday"));
            source.Contributors.Editor = contributor;
            contributor = new PersonCollection();
            contributor.Add(new Person("Suzanna", "Wilkie", "Lita"));
            contributor.Add(new Person("Cyrus", "Charmaine", "Levi"));
            source.Contributors.Translator = contributor;
            document.Bibliography.Sources.Add(source);

            #endregion
        }

        private static void AssertBibliography(Document document)
        {
            Assert.That(document.Bibliography.BibliographyStyle, Is.EqualTo("\\APASixthEditionOfficeOnline.xsl"));
            Assert.That(document.Bibliography.StyleName, Is.EqualTo("APA"));
            Assert.That(document.Bibliography.Version, Is.EqualTo("6"));

            Source source;
            Contributor contributor;
            PersonCollection personCollection;

            source = document.Bibliography.Sources[0];
            Assert.That(source.Tag, Is.EqualTo("Art"));
            Assert.That(source.Lcid, Is.EqualTo("1033"));
            Assert.That(source.AbbreviatedCaseNumber, Is.Null);
            Assert.That(source.AlbumTitle, Is.Null);
            Assert.That(source.BookTitle, Is.Null);
            Assert.That(source.Broadcaster, Is.Null);
            Assert.That(source.BroadcastTitle, Is.Null);
            Assert.That(source.CaseNumber, Is.Null);
            Assert.That(source.ChapterNumber, Is.Null);
            Assert.That(source.City, Is.EqualTo("Mumbai"));
            Assert.That(source.Comments, Is.EqualTo("no comments"));
            Assert.That(source.ConferenceName, Is.Null);
            Assert.That(source.CountryOrRegion, Is.EqualTo("China"));
            Assert.That(source.Court, Is.Null);
            Assert.That(source.Day, Is.Null);
            Assert.That(source.DayAccessed, Is.EqualTo("14"));
            Assert.That(source.Department, Is.Null);
            Assert.That(source.Distributor, Is.Null);
            Assert.That(source.Edition, Is.EqualTo("1"));
            Assert.That(source.Guid, Is.EqualTo("{2079AD07-875B-47C2-BD12-EF0C2A277A68}"));
            Assert.That(source.Institution, Is.EqualTo("Louvre Museum"));
            Assert.That(source.InternetSiteTitle, Is.Null);
            Assert.That(source.Issue, Is.Null);
            Assert.That(source.JournalName, Is.Null);
            Assert.That(source.Medium, Is.EqualTo("Journal"));
            Assert.That(source.Month, Is.Null);
            Assert.That(source.MonthAccessed, Is.EqualTo("September"));
            Assert.That(source.NumberVolumes, Is.EqualTo("IX"));
            Assert.That(source.Pages, Is.EqualTo("288"));
            Assert.That(source.PatentNumber, Is.Null);
            Assert.That(source.PeriodicalTitle, Is.Null);
            Assert.That(source.ProductionCompany, Is.Null);
            Assert.That(source.PublicationTitle, Is.EqualTo("The Song of the Siren"));
            Assert.That(source.Publisher, Is.EqualTo("Wiley"));
            Assert.That(source.RecordingNumber, Is.Null);
            Assert.That(source.RefOrder, Is.EqualTo("9"));
            Assert.That(source.Reporter, Is.Null);
            Assert.That(source.ShortTitle, Is.EqualTo("The Song of the Siren"));
            Assert.That(source.StandardNumber, Is.EqualTo("978-0-307-72037-2"));
            Assert.That(source.StateOrProvince, Is.EqualTo("Michigan"));
            Assert.That(source.Station, Is.Null);
            Assert.That(source.Theater, Is.Null);
            Assert.That(source.ThesisType, Is.Null);
            Assert.That(source.Title, Is.EqualTo("The Song of the Siren"));
            Assert.That(source.Type, Is.Null);
            Assert.That(source.Url, Is.EqualTo("https://www.spotify.com"));
            Assert.That(source.Version, Is.Null);
            Assert.That(source.Volume, Is.EqualTo("IX"));
            Assert.That(source.Year, Is.EqualTo("2001"));
            Assert.That(source.YearAccessed, Is.EqualTo("1995"));
            Assert.That(source.Doi, Is.EqualTo("10.9999/ghij.2222"));
            contributor = source.Contributors.Artist;
            Assert.That(contributor, Is.InstanceOf(typeof(PersonCollection)));
            personCollection = (PersonCollection)contributor;
            Assert.That(personCollection.Count, Is.EqualTo(2));
            Assert.That(personCollection[0].Last, Is.EqualTo("Madelaine"));
            Assert.That(personCollection[0].First, Is.EqualTo("Herbert"));
            Assert.That(personCollection[0].Middle, Is.EqualTo("Gwyneth"));
            Assert.That(personCollection[1].Last, Is.EqualTo("Linda"));
            Assert.That(personCollection[1].First, Is.EqualTo("Wardell"));
            Assert.That(personCollection[1].Middle, Is.EqualTo("Wren"));
            Assert.That(source.Contributors.Author, Is.Null);
            Assert.That(source.Contributors.BookAuthor, Is.Null);
            Assert.That(source.Contributors.Compiler, Is.Null);
            Assert.That(source.Contributors.Composer, Is.Null);
            Assert.That(source.Contributors.Conductor, Is.Null);
            Assert.That(source.Contributors.Counsel, Is.Null);
            Assert.That(source.Contributors.Director, Is.Null);
            Assert.That(source.Contributors.Editor, Is.Null);
            Assert.That(source.Contributors.Interviewee, Is.Null);
            Assert.That(source.Contributors.Interviewer, Is.Null);
            Assert.That(source.Contributors.Inventor, Is.Null);
            Assert.That(source.Contributors.Performer, Is.Null);
            Assert.That(source.Contributors.Producer, Is.Null);
            Assert.That(source.Contributors.Translator, Is.Null);
            Assert.That(source.Contributors.Writer, Is.Null);

            source = document.Bibliography.Sources[1];
            Assert.That(source.Tag, Is.EqualTo("ArticleInAPeriodical"));
            Assert.That(source.Lcid, Is.EqualTo("1033"));
            Assert.That(source.AbbreviatedCaseNumber, Is.Null);
            Assert.That(source.AlbumTitle, Is.Null);
            Assert.That(source.BookTitle, Is.Null);
            Assert.That(source.Broadcaster, Is.Null);
            Assert.That(source.BroadcastTitle, Is.Null);
            Assert.That(source.CaseNumber, Is.Null);
            Assert.That(source.ChapterNumber, Is.Null);
            Assert.That(source.City, Is.EqualTo("New York City"));
            Assert.That(source.Comments, Is.EqualTo("no comments"));
            Assert.That(source.ConferenceName, Is.Null);
            Assert.That(source.CountryOrRegion, Is.EqualTo("United States"));
            Assert.That(source.Court, Is.Null);
            Assert.That(source.Day, Is.EqualTo("4"));
            Assert.That(source.DayAccessed, Is.EqualTo("5"));
            Assert.That(source.Department, Is.Null);
            Assert.That(source.Distributor, Is.Null);
            Assert.That(source.Edition, Is.EqualTo("1"));
            Assert.That(source.Guid, Is.EqualTo("{624F5422-0B8F-4E5B-912A-0BF706D44B37}"));
            Assert.That(source.Institution, Is.Null);
            Assert.That(source.InternetSiteTitle, Is.Null);
            Assert.That(source.Issue, Is.Null);
            Assert.That(source.JournalName, Is.Null);
            Assert.That(source.Medium, Is.EqualTo("Hardcover"));
            Assert.That(source.Month, Is.EqualTo("June"));
            Assert.That(source.MonthAccessed, Is.EqualTo("January"));
            Assert.That(source.NumberVolumes, Is.EqualTo("I"));
            Assert.That(source.Pages, Is.EqualTo("304"));
            Assert.That(source.PatentNumber, Is.Null);
            Assert.That(source.PeriodicalTitle, Is.EqualTo("The Last Puzzle Piece"));
            Assert.That(source.ProductionCompany, Is.Null);
            Assert.That(source.PublicationTitle, Is.Null);
            Assert.That(source.Publisher, Is.EqualTo("Penguin Random House"));
            Assert.That(source.RecordingNumber, Is.Null);
            Assert.That(source.RefOrder, Is.EqualTo("1"));
            Assert.That(source.Reporter, Is.Null);
            Assert.That(source.ShortTitle, Is.EqualTo("The Last Puzzle Piece"));
            Assert.That(source.StandardNumber, Is.EqualTo("978-0-306-40615-7"));
            Assert.That(source.StateOrProvince, Is.EqualTo("California"));
            Assert.That(source.Station, Is.Null);
            Assert.That(source.Theater, Is.Null);
            Assert.That(source.ThesisType, Is.Null);
            Assert.That(source.Title, Is.EqualTo("The Last Puzzle Piece"));
            Assert.That(source.Type, Is.Null);
            Assert.That(source.Url, Is.EqualTo("https://www.example.com"));
            Assert.That(source.Version, Is.Null);
            Assert.That(source.Volume, Is.EqualTo("I"));
            Assert.That(source.Year, Is.EqualTo("2005"));
            Assert.That(source.YearAccessed, Is.EqualTo("2005"));
            Assert.That(source.Doi, Is.EqualTo("10.1234/abcd.5678"));
            contributor = source.Contributors.Artist;
            Assert.That(contributor, Is.InstanceOf(typeof(PersonCollection)));
            personCollection = (PersonCollection)contributor;
            Assert.That(personCollection.Count, Is.EqualTo(2));
            Assert.That(personCollection[0].Last, Is.EqualTo("Emma"));
            Assert.That(personCollection[0].First, Is.EqualTo("Grace"));
            Assert.That(personCollection[0].Middle, Is.EqualTo("Smith"));
            Assert.That(personCollection[1].Last, Is.EqualTo("Liam"));
            Assert.That(personCollection[1].First, Is.EqualTo("Michael"));
            Assert.That(personCollection[1].Middle, Is.EqualTo("Johnson"));
            contributor = source.Contributors.Author;
            Assert.That(contributor, Is.InstanceOf(typeof(PersonCollection)));
            personCollection = (PersonCollection)contributor;
            Assert.That(personCollection.Count, Is.EqualTo(2));
            Assert.That(personCollection[0].Last, Is.EqualTo("Lanford"));
            Assert.That(personCollection[0].First, Is.EqualTo("Corey"));
            Assert.That(personCollection[0].Middle, Is.EqualTo("Estella"));
            Assert.That(personCollection[1].Last, Is.EqualTo("Acacia"));
            Assert.That(personCollection[1].First, Is.EqualTo("Lacy"));
            Assert.That(personCollection[1].Middle, Is.EqualTo("Legacy"));
            Assert.That(source.Contributors.BookAuthor, Is.Null);
            Assert.That(source.Contributors.Compiler, Is.Null);
            Assert.That(source.Contributors.Composer, Is.Null);
            Assert.That(source.Contributors.Conductor, Is.Null);
            Assert.That(source.Contributors.Counsel, Is.Null);
            Assert.That(source.Contributors.Director, Is.Null);
            contributor = source.Contributors.Editor;
            Assert.That(contributor, Is.InstanceOf(typeof(PersonCollection)));
            personCollection = (PersonCollection)contributor;
            Assert.That(personCollection.Count, Is.EqualTo(2));
            Assert.That(personCollection[0].Last, Is.EqualTo("Anima"));
            Assert.That(personCollection[0].First, Is.EqualTo("Dax"));
            Assert.That(personCollection[0].Middle, Is.EqualTo("Journee"));
            Assert.That(personCollection[1].Last, Is.EqualTo("Denver"));
            Assert.That(personCollection[1].First, Is.EqualTo("Kaelyn"));
            Assert.That(personCollection[1].Middle, Is.EqualTo("Kristine"));
            Assert.That(source.Contributors.Interviewee, Is.Null);
            Assert.That(source.Contributors.Interviewer, Is.Null);
            Assert.That(source.Contributors.Inventor, Is.Null);
            Assert.That(source.Contributors.Performer, Is.Null);
            Assert.That(source.Contributors.Producer, Is.Null);
            Assert.That(source.Contributors.Translator, Is.Null);
            Assert.That(source.Contributors.Writer, Is.Null);

            source = document.Bibliography.Sources[2];
            Assert.That(source.Tag, Is.EqualTo("Book"));
            Assert.That(source.Lcid, Is.EqualTo("1033"));
            Assert.That(source.AbbreviatedCaseNumber, Is.Null);
            Assert.That(source.AlbumTitle, Is.Null);
            Assert.That(source.BookTitle, Is.Null);
            Assert.That(source.Broadcaster, Is.Null);
            Assert.That(source.BroadcastTitle, Is.Null);
            Assert.That(source.CaseNumber, Is.Null);
            Assert.That(source.ChapterNumber, Is.Null);
            Assert.That(source.City, Is.EqualTo("London"));
            Assert.That(source.Comments, Is.EqualTo("no comments"));
            Assert.That(source.ConferenceName, Is.Null);
            Assert.That(source.CountryOrRegion, Is.EqualTo("Canada"));
            Assert.That(source.Court, Is.Null);
            Assert.That(source.Day, Is.Null);
            Assert.That(source.DayAccessed, Is.EqualTo("17"));
            Assert.That(source.Department, Is.Null);
            Assert.That(source.Distributor, Is.Null);
            Assert.That(source.Edition, Is.EqualTo("2"));
            Assert.That(source.Guid, Is.EqualTo("{46464485-34F1-4892-8828-D055C5630471}"));
            Assert.That(source.Institution, Is.Null);
            Assert.That(source.InternetSiteTitle, Is.Null);
            Assert.That(source.Issue, Is.Null);
            Assert.That(source.JournalName, Is.Null);
            Assert.That(source.Medium, Is.EqualTo("Paperback"));
            Assert.That(source.Month, Is.Null);
            Assert.That(source.MonthAccessed, Is.EqualTo("February"));
            Assert.That(source.NumberVolumes, Is.EqualTo("II"));
            Assert.That(source.Pages, Is.EqualTo("432"));
            Assert.That(source.PatentNumber, Is.Null);
            Assert.That(source.PeriodicalTitle, Is.Null);
            Assert.That(source.ProductionCompany, Is.Null);
            Assert.That(source.PublicationTitle, Is.Null);
            Assert.That(source.Publisher, Is.EqualTo("HarperCollins"));
            Assert.That(source.RecordingNumber, Is.Null);
            Assert.That(source.RefOrder, Is.EqualTo("2"));
            Assert.That(source.Reporter, Is.Null);
            Assert.That(source.ShortTitle, Is.EqualTo("Whispers of the Moon"));
            Assert.That(source.StandardNumber, Is.EqualTo("978-0-553-38215-9"));
            Assert.That(source.StateOrProvince, Is.EqualTo("New York"));
            Assert.That(source.Station, Is.Null);
            Assert.That(source.Theater, Is.Null);
            Assert.That(source.ThesisType, Is.Null);
            Assert.That(source.Title, Is.EqualTo("Whispers of the Moon"));
            Assert.That(source.Type, Is.Null);
            Assert.That(source.Url, Is.EqualTo("https://www.openai.org"));
            Assert.That(source.Version, Is.Null);
            Assert.That(source.Volume, Is.EqualTo("II"));
            Assert.That(source.Year, Is.EqualTo("1998"));
            Assert.That(source.YearAccessed, Is.EqualTo("2012"));
            Assert.That(source.Doi, Is.EqualTo("10.9876/efgh.5432"));
            Assert.That(source.Contributors.Artist, Is.Null);
            contributor = source.Contributors.Author;
            Assert.That(contributor, Is.InstanceOf(typeof(PersonCollection)));
            personCollection = (PersonCollection)contributor;
            Assert.That(personCollection.Count, Is.EqualTo(2));
            Assert.That(personCollection[0].Last, Is.EqualTo("Connor"));
            Assert.That(personCollection[0].First, Is.EqualTo("Hazel"));
            Assert.That(personCollection[0].Middle, Is.EqualTo("Destiny"));
            Assert.That(personCollection[1].Last, Is.EqualTo("Keeley"));
            Assert.That(personCollection[1].First, Is.EqualTo("Mariah"));
            Assert.That(personCollection[1].Middle, Is.EqualTo("Freeman"));
            Assert.That(source.Contributors.BookAuthor, Is.Null);
            Assert.That(source.Contributors.Compiler, Is.Null);
            Assert.That(source.Contributors.Composer, Is.Null);
            Assert.That(source.Contributors.Conductor, Is.Null);
            Assert.That(source.Contributors.Counsel, Is.Null);
            Assert.That(source.Contributors.Director, Is.Null);
            contributor = source.Contributors.Editor;
            Assert.That(contributor, Is.InstanceOf(typeof(PersonCollection)));
            personCollection = (PersonCollection)contributor;
            Assert.That(personCollection.Count, Is.EqualTo(2));
            Assert.That(personCollection[0].Last, Is.EqualTo("Ora"));
            Assert.That(personCollection[0].First, Is.EqualTo("Breanna"));
            Assert.That(personCollection[0].Middle, Is.EqualTo("Amabel"));
            Assert.That(personCollection[1].Last, Is.EqualTo("Rosemary"));
            Assert.That(personCollection[1].First, Is.EqualTo("Aylmer"));
            Assert.That(personCollection[1].Middle, Is.EqualTo("Monday"));
            Assert.That(source.Contributors.Interviewee, Is.Null);
            Assert.That(source.Contributors.Interviewer, Is.Null);
            Assert.That(source.Contributors.Inventor, Is.Null);
            Assert.That(source.Contributors.Performer, Is.Null);
            Assert.That(source.Contributors.Producer, Is.Null);
            contributor = source.Contributors.Translator;
            Assert.That(contributor, Is.InstanceOf(typeof(PersonCollection)));
            personCollection = (PersonCollection)contributor;
            Assert.That(personCollection.Count, Is.EqualTo(2));
            Assert.That(personCollection[0].Last, Is.EqualTo("Suzanna"));
            Assert.That(personCollection[0].First, Is.EqualTo("Wilkie"));
            Assert.That(personCollection[0].Middle, Is.EqualTo("Lita"));
            Assert.That(personCollection[1].Last, Is.EqualTo("Cyrus"));
            Assert.That(personCollection[1].First, Is.EqualTo("Charmaine"));
            Assert.That(personCollection[1].Middle, Is.EqualTo("Levi"));
            Assert.That(source.Contributors.Writer, Is.Null);
        }
    }
}
