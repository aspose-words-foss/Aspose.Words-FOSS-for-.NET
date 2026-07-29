// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/03/2010 by Roman Korchagin

using System.Collections.Generic;
using Aspose.OpcPackaging;
using Aspose.Ss;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Xml;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Writes the VBA project and its related parts (digital signature and vba data).
    /// </summary>
    internal class DocxVbaWriter
    {
        internal static void Write(DocxDocumentWriter writer)
        {
            if (!writer.Document.HasMacros)
                return;

            OpcPackagePart vbaPart = WriteVbaProject(writer);
            WriteVbaSignature(writer, vbaPart);
            WriteVbaData(writer, vbaPart);
        }

        private static OpcPackagePart WriteVbaProject(DocxDocumentWriter writer)
        {
            // Add the vba project part.
            OpcPackagePart vbaPart = writer.Package.CreateChildPart(
                writer.DocumentPart, "vbaProject.bin", DocxContentType.Vba, writer.RelTypes.Vba);

            // Save the vba project to the part.
            FileSystem vbaFs = new FileSystem(writer.Document.VbaProject.Storage);
            vbaFs.Save(vbaPart.Stream);

            return vbaPart;
        }

        /// <summary>
        /// Writes the VBA digital signature if present.
        /// </summary>
        private static void WriteVbaSignature(DocxDocumentWriter writer, OpcPackagePart vbaPart)
        {
            Debug.Assert(vbaPart != null);

            Document doc = writer.Document;

            if (doc.VbaProject == null)
                return;

            byte[] vbaSignature = doc.VbaProject.Signature;
            if (vbaSignature == null)
                return;


            // Add the vba signature part.
            OpcPackagePart vbaSignaturePart = writer.Package.CreateChildPart(
                vbaPart, "vbaProjectSignature.bin", DocxContentType.VbaProjectSignature, writer.RelTypes.VbaProjectSignature);

            // Save the vba signature to the part, stripping away the DOC prefix.
            vbaSignaturePart.Stream.Write(
                vbaSignature,
                Document.VbaSignaturePrefixLength,
                vbaSignature.Length - Document.VbaSignaturePrefixLength);
        }

        /// <summary>
        /// Although the SPEC says the VBA project must always have a relationship to the VBA data part,
        /// I see that many documents have VBA project, but no VBA part. So let's do the same and write only when needed.
        /// </summary>
        private static void WriteVbaData(DocxDocumentWriter writer, OpcPackagePart vbaPart)
        {
            Debug.Assert(vbaPart != null);

            Document doc = writer.Document;
            if (!HasDocEvents(doc) && !HasMacroNames(doc))
                return;

            string dummyRelId;
            DocxBuilder builder = writer.CreateChildPartAndBuilder(
                vbaPart, "vbaData.xml", DocxContentType.VbaData, writer.RelTypes.VbaData, out dummyRelId);

            builder.StartDocumentWithStandardNamespaces("wne:vbaSuppData");
            WriteDocEvents(writer.Document, builder);
            WriteMcds(writer.Document, builder);
            builder.EndDocument(); 
        }

        private static bool HasMacroNames(Document doc)
        {
            if (doc.VbaProject == null)
                return false;

            IList<string> macroNames = doc.VbaProject.MacroNames;
            return (macroNames != null) && (macroNames.Count > 0);
        }

        private static bool HasDocEvents(Document doc)
        {
            return (doc.VbaDocumentEvents != 0);
        }

        private static void WriteDocEvents(Document doc, AnyXmlBuilder builder)
        {
            if (!HasDocEvents(doc))
                return;

            builder.StartElement("wne:docEvents");
            VbaDocumentEvents events = doc.VbaDocumentEvents;
            WriteDocEvent(events, VbaDocumentEvents.New, "wne:eventDocNew", builder);
            WriteDocEvent(events, VbaDocumentEvents.Open, "wne:eventDocOpen", builder);
            WriteDocEvent(events, VbaDocumentEvents.Close, "wne:eventDocClose", builder);
            WriteDocEvent(events, VbaDocumentEvents.Sync, "wne:eventDocSync", builder);
            WriteDocEvent(events, VbaDocumentEvents.XmlAfterInsert, "wne:eventDocXmlAfterInsert", builder);
            WriteDocEvent(events, VbaDocumentEvents.XmlBeforeDelete, "wne:eventDocXmlBeforeDelete", builder);
            WriteDocEvent(events, VbaDocumentEvents.ContentControlAfterInsert, "wne:eventDocContentControlAfterInsert", builder);
            WriteDocEvent(events, VbaDocumentEvents.ContentControlBeforeDelete, "wne:eventDocContentControlBeforeDelete", builder);
            WriteDocEvent(events, VbaDocumentEvents.ContentControlOnExit, "wne:eventDocContentControlOnExit", builder);
            WriteDocEvent(events, VbaDocumentEvents.ContentControlOnEnter, "wne:eventDocContentControlOnEnter", builder);
            WriteDocEvent(events, VbaDocumentEvents.StoreUpdate, "wne:eventDocStoreUpdate", builder);
            WriteDocEvent(events, VbaDocumentEvents.ContentControlContentUpdate, "wne:eventDocContentControlContentUpdate", builder);
            WriteDocEvent(events, VbaDocumentEvents.BuildingBlockAfterInsert, "wne:eventDocBuildingBlockAfterInsert", builder);
            builder.EndElement();   // docEvents
        }

        private static void WriteDocEvent(VbaDocumentEvents events, VbaDocumentEvents mask, string elemName, AnyXmlBuilder builder)
        {
            if ((events & mask) != 0)
                builder.WriteEmptyElement(elemName);
        }

        private static void WriteMcds(Document doc, NrxXmlBuilder builder)
        {
            if (!HasMacroNames(doc))
                return;

            IList<string> macroNames = doc.VbaProject.MacroNames;
            builder.StartElement("wne:mcds");
            foreach (string macroName in macroNames)
            {
                builder.StartElement("wne:mcd");
                builder.WriteAttribute("wne:macroName", macroName.ToUpper());   // Must always be uppercase of name.
                builder.WriteAttribute("wne:name", macroName);
                builder.WriteAttribute("wne:bEncrypt", "00");   // Must always be this value.
                builder.WriteAttribute("wne:cmg", "56");        // Must always be this value.
                builder.EndElement();   // mcd
            }
            builder.EndElement();   // mcds
        }
    }
}
