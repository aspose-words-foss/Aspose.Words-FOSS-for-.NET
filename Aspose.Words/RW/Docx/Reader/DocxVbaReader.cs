// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/03/2010 by Roman Korchagin

using Aspose.IO;
using Aspose.OpcPackaging;
using Aspose.Ss;
using Aspose.Words.Nrx;
using Aspose.Xml;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Reads a VBA project and its related parts from OOXML.
    /// </summary>
    internal class DocxVbaReader
    {
        internal static void Read(DocxDocumentReader reader)
        {
            OpcPackagePart vbaPart = ReadVbaProject(reader);
            if (vbaPart == null)
                return;

            if (reader.Document.VbaProject != null)
                reader.Document.VbaProject.Signature = ReadVbaSignature(reader, vbaPart);

            ReadVbaSupplementalData(reader, vbaPart);
        }

        /// <summary>
        /// Reads the 2.1.1.2 VBA Project part into the model. Also returns the part because we need it to find other related parts.
        /// </summary>
        private static OpcPackagePart ReadVbaProject(DocxDocumentReader reader)
        {
            OpcPackagePart vbaPart = reader.GetPartByRelationshipType(reader.DocumentPart, reader.RelTypes.Vba);
            if (vbaPart == null)
                return null;

            // Load the VBA project into the model.
            FileSystem vbaFs = new FileSystem(vbaPart.Stream);
            reader.Document.ReadVbaProject(vbaFs.Root, null);

            return vbaPart;
        }

        private static byte[] ReadVbaSignature(DocxDocumentReader reader, OpcPackagePart vbaPart)
        {
            Debug.Assert(vbaPart != null);

            OpcPackagePart vbaSignaturePart = reader.GetPartByRelationshipType(vbaPart, reader.RelTypes.VbaProjectSignature);
            if (vbaSignaturePart == null)
                return null;

            // Load the vba signature into the model and synthesize the 8 byte prefix.
            const int sigPrefixLength = Document.VbaSignaturePrefixLength;
            int sigLength = (int)vbaSignaturePart.Stream.Length;
            byte[] sigWithPrefix = new byte[sigPrefixLength + sigLength];
            ArrayUtil.WriteUInt32ToByteArray(sigLength, sigWithPrefix, 0);
            ArrayUtil.WriteUInt32ToByteArray(sigPrefixLength, sigWithPrefix, 4);
            StreamUtil.Read(vbaSignaturePart.Stream, sigWithPrefix, sigPrefixLength, sigLength);
            return sigWithPrefix;
        }

        /// <summary>
        /// Reads the 2.1.1.3 VBA Supplemental Data part into the model.
        /// </summary>
        private static void ReadVbaSupplementalData(DocxDocumentReader reader, OpcPackagePart vbaPart)
        {
            Debug.Assert(vbaPart != null);

            OpcPackagePart vbaDataPart = reader.GetPartByRelationshipType(vbaPart, reader.RelTypes.VbaData);
            if (vbaDataPart == null)
                return;

            NrxXmlReader xmlReader = new NrxXmlReader(vbaDataPart.Stream);
            while (xmlReader.ReadChild("vbaSuppData"))
            {
                switch (xmlReader.LocalName)
                {
                    case "docEvents":
                        ReadDocEvents(xmlReader, reader.Document);
                        break;
                    case "mcds":
                        ReadMcds(xmlReader, reader.Document);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads the 2.1.4.3 CT_DocEvents which contains names of macros for document events.
        /// </summary>
        private static void ReadDocEvents(AnyXmlReader xmlReader, Document doc)
        {
            VbaDocumentEvents events = 0;

            while (xmlReader.ReadChild("docEvents"))
            {
                switch (xmlReader.LocalName)
                {
                    case "eventDocNew":
                        events |= VbaDocumentEvents.New;
                        break;
                    case "eventDocOpen":
                        events |= VbaDocumentEvents.Open;
                        break;
                    case "eventDocClose":
                        events |= VbaDocumentEvents.Close;
                        break;
                    case "eventDocSync":
                        events |= VbaDocumentEvents.Sync;
                        break;
                    case "eventDocXmlAfterInsert":
                        events |= VbaDocumentEvents.XmlAfterInsert;
                        break;
                    case "eventDocXmlBeforeDelete":
                        events |= VbaDocumentEvents.XmlBeforeDelete;
                        break;
                    case "eventDocContentControlAfterInsert":
                        events |= VbaDocumentEvents.ContentControlAfterInsert;
                        break;
                    case "eventDocContentControlBeforeDelete":
                        events |= VbaDocumentEvents.ContentControlBeforeDelete;
                        break;
                    case "eventDocContentControlOnExit":
                        events |= VbaDocumentEvents.ContentControlOnExit;
                        break;
                    case "eventDocContentControlOnEnter":
                        events |= VbaDocumentEvents.ContentControlOnEnter;
                        break;
                    case "eventDocStoreUpdate":
                        events |= VbaDocumentEvents.StoreUpdate;
                        break;
                    case "eventDocContentControlContentUpdate":
                        events |= VbaDocumentEvents.ContentControlContentUpdate;
                        break;
                    case "eventDocBuildingBlockAfterInsert":
                        events |= VbaDocumentEvents.BuildingBlockAfterInsert;
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            doc.VbaDocumentEvents = events;
        }

        /// <summary>
        /// Reads the 2.1.4.2 CT_Mcds element which contains a list of macro names. These names appear in the Macros Window in MS Word.
        /// </summary>
        private static void ReadMcds(AnyXmlReader xmlReader, Document doc)
        {
            while (xmlReader.ReadChild("mcds"))
            {
                switch (xmlReader.LocalName)
                {
                    case "mcd":
                        ReadMcd(xmlReader, doc);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 2.1.4.1 CT_Mcd which specifies a macro name.
        /// </summary>
        private static void ReadMcd(AnyXmlReader xmlReader, Document doc)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "macroName":
                        // An ST_String ([ECMA-376] Part 4, Section 2.18.89) attribute that MUST equal 
                        // the name attribute with every character set to uppercase.
                        // RK Just ignore on read.
                        break;
                    case "name":
                        // An ST_String ([ECMA-376] Part 4, Section 2.18.89) attribute that specifies 
                        // the name of the macro. name MUST NOT exceed 255 characters.
                        if (doc.VbaProject != null)
                            doc.VbaProject.AddMacroName(xmlReader.Value);
                        break;
                    case "menuHelp":
                        // According to the SPEC this MUST be ignored.
                        break;
                    case "bEncrypt":
                        // According to the SPEC this MUST be 0. In fact it is "00" in the documents. Just ignore on read.
                        break;
                    case "cmg":
                        // According to the SPEC this must be "56". Just ignore on read.
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }
        }
    }
}
