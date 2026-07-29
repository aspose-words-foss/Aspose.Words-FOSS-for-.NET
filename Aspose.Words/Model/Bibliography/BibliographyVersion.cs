// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2024 by Edward Voronov

namespace Aspose.Words.Bibliography
{
    /// <summary>
    /// Calculates <see cref="Bibliography"/> or <see cref="Source"/> version (hashcode).
    /// </summary>
    internal static class BibliographyVersion
    {
        internal static int Calculate(Bibliography bibliography)
        {
            int version = 0;

            version = Calculate(version, bibliography.BibliographyStyle);

            foreach (Source source in bibliography.Sources)
                version = Calculate(version, Calculate(source));

            return version;
        }

        internal static int Calculate(Source source)
        {
            int version = 0;
            version = Calculate(version, source.Lcid);
            version = Calculate(version, (int)source.SourceType);
            version = Calculate(version, source.AbbreviatedCaseNumber);
            version = Calculate(version, source.AlbumTitle);
            version = Calculate(version, source.BookTitle);
            version = Calculate(version, source.Broadcaster);
            version = Calculate(version, source.BroadcastTitle);
            version = Calculate(version, source.CaseNumber);
            version = Calculate(version, source.ChapterNumber);
            version = Calculate(version, source.City);
            version = Calculate(version, source.Comments);
            version = Calculate(version, source.ConferenceName);
            version = Calculate(version, source.CountryOrRegion);
            version = Calculate(version, source.Court);
            version = Calculate(version, source.Day);
            version = Calculate(version, source.DayAccessed);
            version = Calculate(version, source.Department);
            version = Calculate(version, source.Distributor);
            version = Calculate(version, source.Edition);
            version = Calculate(version, source.Guid);
            version = Calculate(version, source.Institution);
            version = Calculate(version, source.InternetSiteTitle);
            version = Calculate(version, source.Issue);
            version = Calculate(version, source.JournalName);
            version = Calculate(version, source.Medium);
            version = Calculate(version, source.Month);
            version = Calculate(version, source.MonthAccessed);
            version = Calculate(version, source.NumberVolumes);
            version = Calculate(version, source.Pages);
            version = Calculate(version, source.PatentNumber);
            version = Calculate(version, source.PeriodicalTitle);
            version = Calculate(version, source.ProductionCompany);
            version = Calculate(version, source.PublicationTitle);
            version = Calculate(version, source.Publisher);
            version = Calculate(version, source.RecordingNumber);
            version = Calculate(version, source.RefOrder);
            version = Calculate(version, source.Reporter);
            version = Calculate(version, source.ShortTitle);
            version = Calculate(version, source.StandardNumber);
            version = Calculate(version, source.StateOrProvince);
            version = Calculate(version, source.Station);
            version = Calculate(version, source.Tag);
            version = Calculate(version, source.Theater);
            version = Calculate(version, source.ThesisType);
            version = Calculate(version, source.Title);
            version = Calculate(version, source.Type);
            version = Calculate(version, source.Url);
            version = Calculate(version, source.Version);
            version = Calculate(version, source.Volume);
            version = Calculate(version, source.Year);
            version = Calculate(version, source.YearAccessed);
            version = Calculate(version, source.Doi);

            foreach (Contributor contributor in source.Contributors)
                version = Calculate(version, Calculate(contributor));

            return version;
        }

        private static int Calculate(Contributor contributor)
        {
            Corporate corporate = contributor as Corporate;
            if (corporate != null)
                return Calculate(corporate);

            PersonCollection personCollection = contributor as PersonCollection;
            if (personCollection != null)
                return Calculate(personCollection);

            Debug.Fail("Unknown contributor type");
            return 0;
        }

        private static int Calculate(Corporate corporate)
        {
            return Calculate(0, corporate.Name);
        }

        private static int Calculate(PersonCollection contributor)
        {
            int version = 0;

            foreach (Person person in contributor)
                version = Calculate(version, Calculate(person));

            return version;
        }

        private static int Calculate(Person person)
        {
            int version = 0;

            version = Calculate(version, person.Last);
            version = Calculate(version, person.First);
            version = Calculate(version, person.Middle);

            return version;
        }

        private static int Calculate(int version, string value)
        {
            return Calculate(version, value != null ? value.GetHashCode() : 0);
        }

        private static int Calculate(int version, int value)
        {
            unchecked
            {
                return (version * 397) ^ value;
            }
        }
    }
}
