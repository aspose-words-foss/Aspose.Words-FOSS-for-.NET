// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/03/2009 by Roman Korchagin

using System;
using Aspose.OpcPackaging;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Writes the glossary document to an OOXML part.
    /// </summary>
    internal class DocxGlossaryWriter : DocxDocumentWriterBase
    {
        internal DocxGlossaryWriter(DocxWriter writer) :
            base(writer, writer.Document.GlossaryDocument, writer.WarningCallback)
        {
        }

        protected override OpcPackagePart DoCreateDocumentPart()
        {
            OpcPackagePart mainDocPart = Package.FetchPartByRelationshipType(null, RelTypes.OfficeDocument);
            return Package.CreateChildPart(mainDocPart, "glossary/document.xml", DocxContentType.GlossaryDocument, RelTypes.GlossaryDocument);
        }

        protected override void DoWrite()
        {
            DocxBuilder builder = CurrentBuilder;

            builder.StartDocumentWithStandardNamespaces("w:glossaryDocument");
            builder.StartElement("w:docParts");

            Document.Accept(this);

            builder.EndElement();   // docParts
            builder.EndDocument();
        }

        public override VisitorAction VisitBuildingBlockStart(BuildingBlock block)
        {
            CurrentBuilder.StartElement("w:docPart");
            WriteDocPartPr(block);

            CurrentBuilder.StartElement("w:docPartBody");
            WriteSections(block);
            CurrentBuilder.EndElement();   // docPartBody
            
            CurrentBuilder.EndElement();    // docPart
            return VisitorAction.SkipThisNode;
        }

        private void WriteDocPartPr(BuildingBlock block)
        {
            DocxBuilder builder = CurrentBuilder;
            builder.StartElement("w:docPartPr");

            builder.StartElement("w:name");
            builder.WriteAttributeIfTrue("w:decorated", block.Decorated);
            builder.WriteAttribute("w:val", block.Name);
            builder.EndElement();

            builder.WriteVal("w:style", block.Style);

            builder.StartElement("w:category");
            builder.WriteVal("w:name", block.Category);
            builder.WriteVal("w:gallery", DocxEnum.DocPartGalleryToDocx(block.Gallery));
            builder.EndElement();

            if (block.Type != BuildingBlockType.None)
            {
                builder.StartElement("w:types");
                if (block.Type == BuildingBlockType.All)
                    builder.WriteAttribute("all", true);
                else
                    builder.WriteVal("w:type", DocxEnum.DocPartTypeToDocx(block.Type));
                builder.EndElement();
            }

            builder.StartElement("w:behaviors");
            builder.WriteVal("w:behavior", DocxEnum.DocPartBehaviorToDocx(block.Behavior));
            builder.EndElement();

            builder.WriteVal("w:description", block.Description);

            if (!block.Guid.Equals(Guid.Empty))
                builder.WriteVal("w:guid", block.Guid.ToString("B").ToUpper());

            builder.EndElement();   // docPartPr
        }
    }
}
