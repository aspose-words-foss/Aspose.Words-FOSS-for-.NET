// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/12/2016 by Alexey Butalov

using Aspose.OpcPackaging;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.RW.Vml;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Helps to create DOCX reader classes.
    /// </summary>
    internal class DocxReaderFactory
    {
        internal static DocxDocumentReader CreateDocumentReader(OpcPackageBase package,
            OpcPackagePart documentPart,
            Document doc,
            LoadOptions loadOptions,
            OoxmlComplianceInfo complianceInfo)
        {
            return new DocxDocumentReader(package,
                documentPart,
                doc,
                loadOptions,
                complianceInfo,
                gStylesReader,
                gStoryReader,
                gNumberingReader,
                gSectPrReader);
        }

        internal static DocxDocumentReader CreateDocumentReader(OpcPackagePart documentPart)
        {
            return new DocxDocumentReader(new OpcPackage(),
                documentPart,
                new Document(DocumentCtorMode.BlankDocumentNode),
                new LoadOptions(),
                new OoxmlComplianceInfo(),
                gStylesReader,
                gStoryReader,
                gNumberingReader,
                gSectPrReader);
        }

        internal static DocxSdtReader CreateInlineLevelSdtReader()
        {
            return new DocxSdtReader(gInlineReader, MarkupLevel.Inline);
        }

        internal static DocxSdtReader CreateBlockLevelSdtReader()
        {
            return new DocxSdtReader(gInlineReader, MarkupLevel.Block);
        }

        internal static DocxSdtReader CreateRowLevelSdtReader(TablePr tablePr)
        {
            return new DocxSdtReader(gInlineReader, MarkupLevel.Row, tablePr);
        }

        internal static DocxSdtReader CreateCellLevelSdtReader(TablePr rowPr)
        {
            return new DocxSdtReader(gInlineReader, MarkupLevel.Cell, rowPr);
        }

        internal static DocxCustomXmlMarkupReader CreateInlineLevelCustomXmlReader()
        {
            return new DocxCustomXmlMarkupReader(gInlineReader, MarkupLevel.Inline);
        }

        internal static DocxCustomXmlMarkupReader CreateBlockLevelCustomXmlReader()
        {
            return new DocxCustomXmlMarkupReader(gInlineReader, MarkupLevel.Block);
        }

        internal static DocxCustomXmlMarkupReader CreateRowLevelCustomXmlReader(TablePr tablePr)
        {
            DocxCustomXmlMarkupReader reader = new DocxCustomXmlMarkupReader(gInlineReader, MarkupLevel.Row, tablePr);
            return reader;
        }

        internal static DocxCustomXmlMarkupReader CreateCellLevelCustomXmlReader(TablePr rowPr)
        {
            DocxCustomXmlMarkupReader reader = new DocxCustomXmlMarkupReader(gInlineReader, MarkupLevel.Cell, rowPr);
            return reader;
        }

        internal static DocxMathReader CreateMathReader(NrxDocumentReaderBase reader)
        {
            return new DocxMathReader(reader,
                gRunPrReader,
                gInlineReader,
                gAnnotationReader);
        }

        internal static DocxGlossaryReader CreateGlossaryReader(OpcPackageBase package,
            OpcPackagePart documentPart,
            GlossaryDocument doc,
            LoadOptions loadOptions,
            OoxmlComplianceInfo complianceInfo)
        {
            return new DocxGlossaryReader(package,
                documentPart,
                doc,
                loadOptions,
                complianceInfo,
                gStylesReader,
                gStoryReader,
                gNumberingReader,
                gSectPrReader);
        }

        internal static DocxRunPrReader RunPrReader
        {
            get { return gRunPrReader; }
        }

        internal static DocxAnnotationReader AnnotationReader
        {
            get { return gAnnotationReader; }
        }

        internal static DocxParaPrReader ParaPrReader
        {
            get { return gParaPrReader; }
        }

        internal static NrxAnnotationReader NrxAnnotationReader
        {
            get { return gNrxAnnotationReader; }
        }

        internal static DocxStoryReader StoryReader
        {
            get { return gStoryReader; }
        }

        internal static DocxRunReader RunReader
        {
            get { return gRunReader; }
        }

        internal static DocxStylesReader StylesReader
        {
            get { return gStylesReader; }
        }

        internal static DocxNumberingReader NumberingReader
        {
            get { return gNumberingReader; }
        }

        internal static DocxCellReader CellReader
        {
            get { return gCellReader; }
        }

        internal static DocxCellPrReader CellPrReader
        {
            get { return gCellPrReader; }
        }

        internal static DocxMailMergePrReader MailMergePrReader
        {
            get { return gMailMergePrReader; }
        }

        internal static DocxSectPrReader SectPrReader
        {
            get { return gSectPrReader; }
        }

        static DocxReaderFactory()
        {
            // Note that any class created here as a static field should be immutable (state cannot be modified after it is created)!
            gSectPrReader = new DocxSectPrReader();

            gMailMergePrReader = new DocxMailMergePrReader();

            DocxFldCharReader fldCharReader = new DocxFldCharReader();

            DocxHyperlinkReader hyperlinkReader = new DocxHyperlinkReader();

            DocxSmartTagReader smartTagReader = new DocxSmartTagReader();

            gRunPrReader = new DocxRunPrReader(fldCharReader);

            gInlineReader = new DocxInlineReader(hyperlinkReader, smartTagReader);
            hyperlinkReader.SetInlineReader(gInlineReader);
            smartTagReader.SetInlineReader(gInlineReader);

            NrxFldSimpleReader fldSimpleReader = new NrxFldSimpleReader(gInlineReader);

            gParaPrReader = new DocxParaPrReader(gRunPrReader, gSectPrReader);

            gNrxAnnotationReader = new NrxAnnotationReader(gRunPrReader, gParaPrReader);

            gAnnotationReader = new DocxAnnotationReader(gNrxAnnotationReader, gInlineReader, gSectPrReader);
            gInlineReader.SetAnnotationReader(gAnnotationReader);
            gParaPrReader.SetAnnotationReader(gAnnotationReader);
            gRunPrReader.SetAnnotationReader(gAnnotationReader);
            gSectPrReader.SetAnnotationReader(gAnnotationReader);

            DocxParaReader paraReader = new DocxParaReader(gParaPrReader, gInlineReader);

            gCellPrReader = new DocxCellPrReader(paraReader);

            gStoryReader = new DocxStoryReader(paraReader);

            VmlShapeReader vmlReader = new VmlShapeReader();

            gRunReader = new DocxRunReader(gStoryReader, gRunPrReader, paraReader,
                fldCharReader, fldSimpleReader, hyperlinkReader, gAnnotationReader, vmlReader);

            gStylesReader = new DocxStylesReader(gParaPrReader, gRunPrReader);

            gNumberingReader = new DocxNumberingReader(gParaPrReader, gRunPrReader, vmlReader);

            gCellReader = new DocxCellReader(gStoryReader, gParaPrReader, gCellPrReader);
        }

        private static readonly DocxRunPrReader gRunPrReader;
        private static readonly DocxAnnotationReader gAnnotationReader;
        private static readonly DocxParaPrReader gParaPrReader;
        private static readonly NrxAnnotationReader gNrxAnnotationReader;
        private static readonly DocxStoryReader gStoryReader;
        private static readonly DocxRunReader gRunReader;
        private static readonly DocxStylesReader gStylesReader;
        private static readonly DocxNumberingReader gNumberingReader;
        private static readonly DocxCellReader gCellReader;
        private static readonly DocxCellPrReader gCellPrReader;
        private static readonly DocxMailMergePrReader gMailMergePrReader;
        private static readonly DocxInlineReader gInlineReader;
        private static readonly DocxSectPrReader gSectPrReader;
    }
}
