// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2016 by Alexander Zhiltsov

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Implements writing the People document part.
    /// </summary>
    internal static class DocxPeopleWriter
    {
        /// <summary>
        /// Writes the People document part with using the specified document writer.
        /// </summary>
        internal static void Write(DocxDocumentWriterBase writer)
        {
            DocumentBase doc = writer.Document;
            if (doc.People.Count == 0 || writer.Compliance == OoxmlComplianceCore.Ecma376)
                return;

            DocxBuilder builder = writer.CreateChildPartAndBuilder("people.xml", DocxContentType.People,
                writer.RelTypes.People);
            writer.PushBuilder(builder);

            builder.StartDocumentWithStandardNamespaces("w15:people");

            foreach (PersonInternal person in doc.People)
                WritePerson(person, writer);

            builder.EndDocument(); //w15:people
            writer.PopBuilder();
        }

        /// <summary>
        /// Writes the person element of 2.5.3.5 CT_Person [MS-DOCX] complex type.
        /// </summary>
        private static void WritePerson(PersonInternal person, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("w15:person");

            builder.WriteAttribute("w15:author", person.Author);

            builder.StartElement("w15:presenceInfo");

            builder.WriteAttribute("w15:providerId", DocxEnum.ContactIdentityProviderToDocx(person.IdentityProvider));

            // WORDSNET-16147 Always write userId, even it is empty. It is required in scheme
            // https://msdn.microsoft.com/en-us/library/documentformat.openxml.office2013.word.presenceinfo.aspx
            builder.WriteAttributeString("w15:userId", person.UserId);

            builder.EndElement(); //w15:presenceInfo

            builder.EndElement(); //w15:person
        }
    }
}
