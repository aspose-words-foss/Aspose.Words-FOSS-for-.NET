// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System.IO;

namespace Aspose.Words.RW.Odt.Reader
{
    /// <summary>
    /// FOSS reduction: only the format-detection statics survive. The full ODT reader
    /// (IDocumentReader implementation, OdtPackage, OfficeCrypto, ~1400 lines) is gone.
    /// </summary>
    internal static class OdtReader
    {
        /// <summary>
        /// Returns true if the ODT document has a digital signature.
        /// You need to provide the META-INF/documentsignatures.xml stream positioned at the beginning.
        /// Does NOT preserve the stream position.
        /// </summary>
        internal static bool IsDigitalSignaturePresent(Stream documentSignaturesStream)
        {
            OdtPartReader partReader = new OdtPartReader(documentSignaturesStream);

            while (partReader.ReadChild("document-signatures"))
            {
                if (partReader.LocalName == "Signature")
                    return true;

                partReader.IgnoreElement();
            }

            return false;
        }

        /// <summary>
        /// Returns true if the ODT document is encrypted.
        /// You need to provide the META-INF/manifest.xml stream positioned at the beginning.
        /// Does NOT preserve the stream position.
        /// </summary>
        internal static bool IsEncrypted(Stream manifestStream)
        {
            OdtPartReader partReader = new OdtPartReader(manifestStream);

            while (partReader.ReadChild("manifest"))
            {
                if (partReader.LocalName == "file-entry")
                {
                    while (partReader.ReadChild("file-entry"))
                    {
                        if (partReader.LocalName == "encryption-data")
                            return true;

                        partReader.IgnoreElement();
                    }
                }
                else
                {
                    partReader.IgnoreElement();
                }
            }

            return false;
        }

        /// <summary>
        /// Detect load format by "manifest.xml". For ODT document without "mimetype" (WORDSNET-4985).
        /// </summary>
        internal static LoadFormat DetectLoadFormatByManifest(Stream manifestStream)
        {
            OdtPartReader partReader = new OdtPartReader(manifestStream);

            while (partReader.ReadChild("manifest"))
            {
                if (partReader.LocalName == "file-entry")
                {
                    string mediaType = "";
                    string fullPath = "";

                    while (partReader.MoveToNextAttribute())
                    {
                        if (partReader.LocalName == "full-path")
                            fullPath = partReader.Value;
                        if (partReader.LocalName == "media-type")
                            mediaType = partReader.Value;
                    }

                    if (fullPath == "/")
                        return DetectLoadFormat(mediaType);
                }
                else
                {
                    partReader.IgnoreElement();
                }
            }

            return LoadFormat.Unknown;
        }

        /// <summary>
        /// You need to pass the "mimetype" stream positioned at the beginning.
        /// Does NOT preserve the stream position.
        /// </summary>
        internal static LoadFormat DetectLoadFormat(Stream mimeTypeStream)
        {
            StreamReader reader = new StreamReader(mimeTypeStream);

            string mimeType = reader.ReadLine();

            return DetectLoadFormat(mimeType);
        }

        private static LoadFormat DetectLoadFormat(string mimeType)
        {
            switch (mimeType)
            {
                case OdtContentTypes.OpenDocumentText:
                // WORDSNET-23560 Added 'OpenDocument Master Document' mime type.
                case OdtContentTypes.OpenDocumentMasterDocument:
                    return LoadFormat.Odt;
                case OdtContentTypes.OpenDocumentTextTemplate:
                    return LoadFormat.Ott;
                default:
                    return LoadFormat.Unknown;
            }
        }
    }
}
