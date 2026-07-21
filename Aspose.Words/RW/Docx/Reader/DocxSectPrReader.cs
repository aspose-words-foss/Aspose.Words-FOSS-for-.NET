// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/01/2017 by Alexey Butalov

using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    internal class DocxSectPrReader : NrxSectPrReaderBase
    {
        internal void SetAnnotationReader(DocxAnnotationReader annotationReader)
        {
            Debug.Assert(annotationReader != null);
            mAnnotationReader = annotationReader;
        }

        protected override void ReadFormatSpecific(string localName, 
            NrxDocumentReaderBase reader, SectPr sectPr, 
            bool hasBlockLevelSectPr, bool hasHeadersFooters)
        {
            switch (localName)
            {
                // Reads header/footer from DOCX.
                case "headerReference":
                case "footerReference":
                {
                    if (!hasBlockLevelSectPr && !hasHeadersFooters)
                        DocxHeaderFooterReader.Read(reader);
                    break;
                }
                case "footnotePr":
                case "endnotePr":
                {
                    DocxSettingsReader.ReadFootnotePr((DocxDocumentReaderBase)reader, localName, sectPr);
                    break;
                }
                case "sectPrChange":
                {
                    mAnnotationReader.ReadSectPrRevision(reader, sectPr);
                    break;
                }
                default:
                {
                    // Ignore.
                    break;
                }
            }
        }

        private DocxAnnotationReader mAnnotationReader;
    }
}
