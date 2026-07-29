// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/02/2009 by Roman Korchagin

using System;
using Aspose.OpcPackaging;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Loading;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Docx.Reader
{
    internal class DocxGlossaryReader : DocxDocumentReaderBase
    {
        internal DocxGlossaryReader(
            OpcPackageBase package,
            OpcPackagePart documentPart,
            GlossaryDocument doc,
            LoadOptions loadOptions,
            OoxmlComplianceInfo complianceInfo,
            DocxStylesReader stylesReader,
            DocxStoryReader storyReader,
            DocxNumberingReader numberingReader,
            DocxSectPrReader sectPrReader) :
                base(package, documentPart, doc, loadOptions, complianceInfo, stylesReader, storyReader, numberingReader, sectPrReader)
        {
            mGlossaryDoc = doc;
        }

        internal override bool IsEquationXmlReader
        {
            get { return false; }
        }

        protected override void DoRead()
        {
            while (XmlReader.ReadChild("glossaryDocument"))
            {
                switch (XmlReader.LocalName)
                {
                    case "background":
                        DocxBackgroundReader.Read(this);
                        break;
                    case "docParts":
                        ReadDocParts();
                        break;
                    default:
                        XmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private void ReadDocParts()
        {
            while (XmlReader.ReadChild("docParts"))
            {
                switch (XmlReader.LocalName)
                {
                    case "docPart":
                        ReadDocPart();
                        break;
                    default:
                        XmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private void ReadDocPart()
        {
            mBlock = new BuildingBlock(mGlossaryDoc);
            AddAndPushContainer(mBlock);

            while (XmlReader.ReadChild("docPart"))
            {
                switch (XmlReader.LocalName)
                {
                    case "docPartPr":
                        ReadDocPartPr();
                        break;
                    case "docPartBody":
                        ReadBody(mGlossaryDoc);
                        EndSection();
                        break;
                    default:
                        XmlReader.IgnoreElement();
                        break;
                }
            }

            PopContainer(NodeType.BuildingBlock);
        }

        private void ReadDocPartPr()
        {
            while (XmlReader.ReadChild("docPartPr"))
            {
                switch (XmlReader.LocalName)
                {
                    case "behaviors":
                        ReadBehaviors();
                        break;
                    case "category":
                        ReadCategory();
                        break;
                    case "description":
                        mBlock.Description = XmlReader.ReadVal();
                        break;
                    case "guid":
                        mBlock.Guid = new Guid(XmlReader.ReadVal());
                        break;
                    case "name":
                        ReadName();
                        break;
                    case "style":
                        mBlock.Style = XmlReader.ReadVal();
                        break;
                    case "types":
                        ReadTypes();
                        break;
                    default:
                        XmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private void ReadBehaviors()
        {
            while (XmlReader.ReadChild("behaviors"))
            {
                switch (XmlReader.LocalName)
                {
                    case "behavior":
                        mBlock.Behavior = DocxEnum.DocxToDocPartBehavior(XmlReader.ReadVal());
                        break;
                    default:
                        XmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private void ReadCategory()
        {
            while (XmlReader.ReadChild("category"))
            {
                switch (XmlReader.LocalName)
                {
                    case "gallery":
                        mBlock.Gallery = DocxEnum.DocxToDocPartGallery(XmlReader.ReadVal());
                        break;
                    case "name":
                        mBlock.SetCategorySafe(XmlReader.ReadVal());
                        break;
                    default:
                        XmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private void ReadName()
        {
            while (XmlReader.MoveToNextAttribute())
            {
                switch (XmlReader.LocalName)
                {
                    case "decorated":
                        mBlock.Decorated = XmlReader.ValueAsBool;
                        break;
                    case "val":
                        mBlock.SetNameSafe(XmlReader.Value);
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }
        }

        private void ReadTypes()
        {
            bool isAll = false;
            while (XmlReader.MoveToNextAttribute())
            {
                switch (XmlReader.LocalName)
                {
                    case "all":
                        isAll = XmlReader.ValueAsBool;
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            while (XmlReader.ReadChild("types"))
            {
                switch (XmlReader.LocalName)
                {
                    case "type":
                        mBlock.Type = DocxEnum.DocxToDocPartType(XmlReader.ReadVal());
                        break;
                    default:
                        XmlReader.IgnoreElement();
                        break;
                }
            }

            if (isAll)
                mBlock.Type = BuildingBlockType.All;
        }

        private readonly GlossaryDocument mGlossaryDoc;
        private BuildingBlock mBlock;
    }
}
