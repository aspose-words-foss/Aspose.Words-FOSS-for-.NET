// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2024 by Edward Voronov

using Aspose.Xml;

namespace Aspose.Words.Bibliography
{
    /// <summary>
    /// <see cref="Source"/> xml serializer.
    /// </summary>
    internal static class SourceWriter
    {
        internal static void Write(Source source, AnyXmlBuilder builder)
        {
            builder.StartElement("b:Source");

            WriteAttributes(source, builder);

            builder.EndElement();
        }

        private static void WriteAttributes(Source source, AnyXmlBuilder builder)
        {
            builder.WriteOptionalElement("b:Tag", source.Tag);
            builder.WriteElement("b:SourceType", SourceTypeMapping.EnumToString(source.SourceType));
            builder.WriteOptionalElement("b:LCID", source.Lcid);
            builder.WriteOptionalElement("b:Title", source.Title);
            builder.WriteOptionalElement("b:Year", source.Year);
            builder.WriteOptionalElement("b:City", source.City);
            builder.WriteOptionalElement("b:Publisher", source.Publisher);

            WriteContributors(source, builder);

            builder.WriteOptionalElement("b:StateProvince", source.StateOrProvince);
            builder.WriteOptionalElement("b:CountryRegion", source.CountryOrRegion);
            builder.WriteOptionalElement("b:Volume", source.Volume);
            builder.WriteOptionalElement("b:NumberVolumes", source.NumberVolumes);
            builder.WriteOptionalElement("b:ShortTitle", source.ShortTitle);
            builder.WriteOptionalElement("b:StandardNumber", source.StandardNumber);
            builder.WriteOptionalElement("b:Pages", source.Pages);
            builder.WriteOptionalElement("b:Edition", source.Edition);
            builder.WriteOptionalElement("b:Comments", source.Comments);
            builder.WriteOptionalElement("b:Medium", source.Medium);
            builder.WriteOptionalElement("b:YearAccessed", source.YearAccessed);
            builder.WriteOptionalElement("b:MonthAccessed", source.MonthAccessed);
            builder.WriteOptionalElement("b:DayAccessed", source.DayAccessed);
            builder.WriteOptionalElement("b:URL", source.Url);
            builder.WriteOptionalElement("b:DOI", source.Doi);
            builder.WriteOptionalElement("b:Guid", source.Guid);
            builder.WriteOptionalElement("b:Department", source.Department);
            builder.WriteOptionalElement("b:Institution", source.Institution);
            builder.WriteOptionalElement("b:ThesisType", source.ThesisType);
            builder.WriteOptionalElement("b:CaseNumber", source.CaseNumber);
            builder.WriteOptionalElement("b:Court", source.Court);
            builder.WriteOptionalElement("b:PeriodicalTitle", source.PeriodicalTitle);
            builder.WriteOptionalElement("b:PublicationTitle", source.PublicationTitle);
            builder.WriteOptionalElement("b:InternetSiteTitle", source.InternetSiteTitle);
            builder.WriteOptionalElement("b:Theater", source.Theater);
            builder.WriteOptionalElement("b:Month", source.Month);
            builder.WriteOptionalElement("b:Day", source.Day);
            builder.WriteOptionalElement("b:AbbreviatedCaseNumber", source.AbbreviatedCaseNumber);
            builder.WriteOptionalElement("b:AlbumTitle", source.AlbumTitle);
            builder.WriteOptionalElement("b:BookTitle", source.BookTitle);
            builder.WriteOptionalElement("b:Broadcaster", source.Broadcaster);
            builder.WriteOptionalElement("b:BroadcastTitle", source.BroadcastTitle);
            builder.WriteOptionalElement("b:ChapterNumber", source.ChapterNumber);
            builder.WriteOptionalElement("b:ConferenceName", source.ConferenceName);
            builder.WriteOptionalElement("b:Distributor", source.Distributor);
            builder.WriteOptionalElement("b:Issue", source.Issue);
            builder.WriteOptionalElement("b:JournalName", source.JournalName);
            builder.WriteOptionalElement("b:PatentNumber", source.PatentNumber);
            builder.WriteOptionalElement("b:ProductionCompany", source.ProductionCompany);
            builder.WriteOptionalElement("b:RecordingNumber", source.RecordingNumber);
            builder.WriteOptionalElement("b:Reporter", source.Reporter);
            builder.WriteOptionalElement("b:Station", source.Station);
            builder.WriteOptionalElement("b:Type", source.Type);
            builder.WriteOptionalElement("b:Version", source.Version);
            builder.WriteOptionalElement("b:RefOrder", source.RefOrder);
        }

        private static void WriteContributors(Source source, AnyXmlBuilder builder)
        {
            if (source.Contributors.IsEmpty)
                return;

            builder.StartElement("b:Author");

            foreach (ContributorType contributorType in source.Contributors.ContributorTypes)
            {
                WriteContributor(
                    source.Contributors.GetContributor(contributorType),
                    contributorType,
                    builder);
            }

            builder.EndElement();
        }

        private static void WriteContributor(
            Contributor contributor,
            ContributorType contributorType,
            AnyXmlBuilder builder)
        {
            builder.StartElement("b:" + ContributorTypeMapping.EnumToString(contributorType));

            WriteContributor(contributor, builder);

            builder.EndElement();
        }

        private static void WriteContributor(Contributor contributor, AnyXmlBuilder builder)
        {
            Corporate corporate = contributor as Corporate;
            if (corporate != null)
            {
                WriteCorporateContributor(corporate, builder);
                return;
            }

            PersonCollection personCollection = contributor as PersonCollection;
            if (personCollection != null)
            {
                WritePersonCollectionContributor(personCollection, builder);
                return;
            }

            Debug.Fail("Unknown contributor type");
        }

        private static void WritePersonCollectionContributor(PersonCollection personCollection, AnyXmlBuilder builder)
        {
            builder.StartElement("b:NameList");

            foreach (Person person in personCollection)
                WritePersonContributor(person, builder);

            builder.EndElement();
        }

        private static void WritePersonContributor(Person person, AnyXmlBuilder builder)
        {
            builder.StartElement("b:Person");

            builder.WriteOptionalElement("b:Last", person.Last);
            builder.WriteOptionalElement("b:First", person.First);
            builder.WriteOptionalElement("b:Middle", person.Middle);

            builder.EndElement();
        }

        private static void WriteCorporateContributor(Corporate corporate, AnyXmlBuilder builder)
        {
            builder.WriteElement("b:Corporate", corporate.Name);
        }
    }
}
