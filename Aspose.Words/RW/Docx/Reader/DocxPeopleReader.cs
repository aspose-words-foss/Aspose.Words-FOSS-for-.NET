// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2016 by Alexander Zhiltsov

using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Implements reading data of the People document part.
    /// </summary>
    internal static class DocxPeopleReader
    {
        /// <summary>
        /// Reads the People document part.
        /// </summary>
        internal static void Read(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.SwitchToPartReaderByRelType(reader.RelTypes.People);
            if (xmlReader == null)
                return;

            reader.ComplianceInfo.IsDocxExtensions = true;
            
            while (xmlReader.ReadChild("people"))
            {
                switch (xmlReader.LocalName)
                {
                    case "person":
                        ReadPerson(reader);
                        break;
                    default:
                    {
                        WarnUnexpected(xmlReader);
                        xmlReader.IgnoreElement();
                        break;
                    }
                }
            }

            reader.RestorePartReader();
        }

        /// <summary>
        /// Reads the person element of the 2.5.3.5 CT_Person type [MS-DOCX].
        /// </summary>
        private static void ReadPerson(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            PersonInternal person = new PersonInternal();

            // Read attributes.
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "author":
                        person.Author = xmlReader.Value;
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }

            // Read elements.
            xmlReader.MoveToElement();
            while (xmlReader.ReadChild("person"))
            {
                switch (xmlReader.LocalName)
                {
                    case "presenceInfo":
                        ReadPresenceInfo(person, xmlReader);
                        break;
                    default:
                    {
                        WarnUnexpected(xmlReader);
                        xmlReader.IgnoreElement();
                        break;
                    }
                }
            }

            reader.Document.People.Add(person);
        }

        /// <summary>
        /// Reads the presenceInfo element of the 2.5.3.6 CT_PresenceInfo type [MS-DOCX].
        /// </summary>
        private static void ReadPresenceInfo(PersonInternal person, NrxXmlReader xmlReader)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "providerId":
                        person.IdentityProvider = DocxEnum.DocxToContactIdentityProvider(xmlReader.Value);
                        break;
                    case "userId":
                        person.UserId = xmlReader.Value;
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Generates a warning that current element or attribute of XML reader is unexpected.
        /// </summary>
        private static void WarnUnexpected(NrxXmlReader xmlReader)
        {
            xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Docx,
                string.Format(WarningStrings.UnexpectedTagOrAttribute, xmlReader.LocalName));
        }
    }
}
