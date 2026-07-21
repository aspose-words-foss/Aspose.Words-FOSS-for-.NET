// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2024 by Edward Voronov

using Aspose.Xml;

namespace Aspose.Words.Bibliography
{
    /// <summary>
    /// <see cref="Source"/> xml deserializer.
    /// </summary>
    internal static class SourceReader
    {
        internal static void Read(Source source, string xml)
        {
            AnyXmlReader reader = new AnyXmlReader(xml, null);

            while (reader.ReadChild("Source"))
            {
                switch (reader.LocalName)
                {
                    case "AbbreviatedCaseNumber":
                        source.AbbreviatedCaseNumber = reader.ReadString();
                        break;
                    case "AlbumTitle":
                        source.AlbumTitle = reader.ReadString();
                        break;
                    case "BookTitle":
                        source.BookTitle = reader.ReadString();
                        break;
                    case "Broadcaster":
                        source.Broadcaster = reader.ReadString();
                        break;
                    case "BroadcastTitle":
                        source.BroadcastTitle = reader.ReadString();
                        break;
                    case "CaseNumber":
                        source.CaseNumber = reader.ReadString();
                        break;
                    case "ChapterNumber":
                        source.ChapterNumber = reader.ReadString();
                        break;
                    case "City":
                        source.City = reader.ReadString();
                        break;
                    case "Comments":
                        source.Comments = reader.ReadString();
                        break;
                    case "ConferenceName":
                        source.ConferenceName = reader.ReadString();
                        break;
                    case "CountryRegion":
                        source.CountryOrRegion = reader.ReadString();
                        break;
                    case "Court":
                        source.Court = reader.ReadString();
                        break;
                    case "Day":
                        source.Day = reader.ReadString();
                        break;
                    case "DayAccessed":
                        source.DayAccessed = reader.ReadString();
                        break;
                    case "Department":
                        source.Department = reader.ReadString();
                        break;
                    case "Distributor":
                        source.Distributor = reader.ReadString();
                        break;
                    case "Edition":
                        source.Edition = reader.ReadString();
                        break;
                    case "Guid":
                        source.Guid = reader.ReadString();
                        break;
                    case "Institution":
                        source.Institution = reader.ReadString();
                        break;
                    case "InternetSiteTitle":
                        source.InternetSiteTitle = reader.ReadString();
                        break;
                    case "Issue":
                        source.Issue = reader.ReadString();
                        break;
                    case "JournalName":
                        source.JournalName = reader.ReadString();
                        break;
                    case "LCID":
                        source.Lcid = reader.ReadString();
                        break;
                    case "Medium":
                        source.Medium = reader.ReadString();
                        break;
                    case "Month":
                        source.Month = reader.ReadString();
                        break;
                    case "MonthAccessed":
                        source.MonthAccessed = reader.ReadString();
                        break;
                    case "NumberVolumes":
                        source.NumberVolumes = reader.ReadString();
                        break;
                    case "Pages":
                        source.Pages = reader.ReadString();
                        break;
                    case "PatentNumber":
                        source.PatentNumber = reader.ReadString();
                        break;
                    case "PeriodicalTitle":
                        source.PeriodicalTitle = reader.ReadString();
                        break;
                    case "ProductionCompany":
                        source.ProductionCompany = reader.ReadString();
                        break;
                    case "PublicationTitle":
                        source.PublicationTitle = reader.ReadString();
                        break;
                    case "Publisher":
                        source.Publisher = reader.ReadString();
                        break;
                    case "RecordingNumber":
                        source.RecordingNumber = reader.ReadString();
                        break;
                    case "RefOrder":
                        source.RefOrder = reader.ReadString();
                        break;
                    case "Reporter":
                        source.Reporter = reader.ReadString();
                        break;
                    case "ShortTitle":
                        source.ShortTitle = reader.ReadString();
                        break;
                    case "StandardNumber":
                        source.StandardNumber = reader.ReadString();
                        break;
                    case "StateProvince":
                        source.StateOrProvince = reader.ReadString();
                        break;
                    case "Station":
                        source.Station = reader.ReadString();
                        break;
                    case "Tag":
                        source.Tag = reader.ReadString();
                        break;
                    case "Theater":
                        source.Theater = reader.ReadString();
                        break;
                    case "ThesisType":
                        source.ThesisType = reader.ReadString();
                        break;
                    case "Title":
                        source.Title = reader.ReadString();
                        break;
                    case "Type":
                        source.Type = reader.ReadString();
                        break;
                    case "URL":
                        source.Url = reader.ReadString();
                        break;
                    case "Version":
                        source.Version = reader.ReadString();
                        break;
                    case "Volume":
                        source.Volume = reader.ReadString();
                        break;
                    case "Year":
                        source.Year = reader.ReadString();
                        break;
                    case "YearAccessed":
                        source.YearAccessed = reader.ReadString();
                        break;
                    case "DOI":
                        source.Doi = reader.ReadString();
                        break;

                    case "SourceType":
                        source.SourceType = SourceTypeMapping.StringToEnum(reader.ReadString());
                        break;
                    case "Author":
                        ReadContributors(reader, source.Contributors);
                        break;

                    default:
                        break;
                }
            }
        }

        private static void ReadContributors(AnyXmlReader reader, ContributorCollection contributors)
        {
            while (reader.ReadChild("Author"))
            {
                ContributorType type;
                if (ContributorTypeMapping.TryStringToEnum(reader.LocalName, out type))
                {
                    Contributor contributor = ReadContributor(reader);
                    if (contributor != null)
                        contributors.SetContributor(type, contributor);
                }
            }
        }

        private static Contributor ReadContributor(AnyXmlReader reader)
        {
            Contributor contributor = null;

            string localName = reader.LocalName;
            while (reader.ReadChild(localName))
            {
                switch (reader.LocalName)
                {
                    case "NameList":
                        contributor = ReadPersons(reader);
                        break;
                    case "Corporate":
                        contributor = ReadCorporate(reader);
                        break;
                    default:
                        break;
                }
            }

            return contributor;
        }

        private static PersonCollection ReadPersons(AnyXmlReader reader)
        {
            PersonCollection persons = new PersonCollection();

            while (reader.ReadChild("NameList"))
            {
                switch (reader.LocalName)
                {
                    case "Person":
                        persons.Add(ReadPerson(reader));
                        break;
                    default:
                        break;
                }
            }

            return persons;
        }

        private static Person ReadPerson(AnyXmlReader reader)
        {
            string first = null;
            string last = null;
            string middle = null;
            while (reader.ReadChild("Person"))
            {
                switch (reader.LocalName)
                {
                    case "Last":
                        last = reader.ReadString();
                        break;
                    case "First":
                        first = reader.ReadString();
                        break;
                    case "Middle":
                        middle = reader.ReadString();
                        break;
                    default:
                        break;
                }
            }

            return new Person(last, first, middle);
        }

        private static Corporate ReadCorporate(AnyXmlReader reader)
        {
            return new Corporate(reader.ReadString());
        }
    }
}
