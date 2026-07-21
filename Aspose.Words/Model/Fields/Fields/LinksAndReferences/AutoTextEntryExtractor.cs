// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/05/2017 by Edward Voronov

using System.Collections.Generic;
using System.IO;
using Aspose.Words.BuildingBlocks;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Extracts auto text building block from templates document.
    /// </summary>
    internal class AutoTextEntryExtractor
    {
        internal AutoTextEntryExtractor(Document document)
        {
            mDocument = document;
        }

        internal BuildingBlock Extract(string entryName)
        {
            EnsureGlossaryDocuments();

            foreach (GlossaryDocument document in mGlossaryDocuments)
            {
                BuildingBlock buildingBlock = Extract(document, entryName);
                if (buildingBlock != null)
                    return buildingBlock;
            }

            return null;
        }

        private static BuildingBlock Extract(GlossaryDocument document, string entryName)
        {
            foreach (BuildingBlock buildingBlock in document.BuildingBlocks)
            {
                // NOTE: ECMA $17.16.5.5 restricts gallery attribute to autotext, but MS Word does not after the Insert (Tab) -> Text (Group) -> Quick Parts menu is opened.
                if (buildingBlock.Gallery == BuildingBlockGallery.NoGallery)
                    continue;

                if (!StringUtil.EqualsIgnoreCase(buildingBlock.Name, entryName))
                    continue;

                return buildingBlock;
            }

            return null;
        }

        private void EnsureGlossaryDocuments()
        {
            if (mGlossaryDocuments != null)
                return;

            mGlossaryDocuments = new List<GlossaryDocument>(mDocument.FieldOptions.BuiltInTemplatesPaths.Length + 2);

            AddGlossaryDocument(mDocument);
            TryLoadTemplateDocument(mDocument.AttachedTemplate);
            foreach (string path in mDocument.FieldOptions.BuiltInTemplatesPaths)
                TryLoadTemplateDocument(path);
        }

        private void TryLoadTemplateDocument(string path)
        {
            if (!StringUtil.HasChars(path))
                return;

            try
            {
                using (Stream stream = Document.OpenDocumentStream(path, mDocument.ResourceLoadingCallback))
                {
                    Document document = new Document(stream, null, false);
                    AddGlossaryDocument(document);
                }
            }
            catch
            {
                // Suppress any exeptions
            }
        }

        private void AddGlossaryDocument(Document document)
        {
            if (document.GlossaryDocument != null)
                mGlossaryDocuments.Add(document.GlossaryDocument);
        }

        private readonly Document mDocument;

        private List<GlossaryDocument> mGlossaryDocuments;
    }
}
