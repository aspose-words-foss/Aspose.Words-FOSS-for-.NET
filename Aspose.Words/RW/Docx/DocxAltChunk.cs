// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/06/2012 by Andrey Noskov

namespace Aspose.Words.RW.Docx
{
    /// <summary>
    /// Container class for storing AltChunks upon reading.
    /// We store reference paragraph, reference node, AltChunk document and AltChunk importFormatMode 
    /// to insert them after reading destination document.
    /// </summary>
    internal class DocxAltChunk
    {
        internal DocxAltChunk(CompositeNode referenceParagraph, Node referenceNode, Document altChunkDocument, ImportFormatMode importFormatMode, bool isInline)
        {
            mReferenceContainer = referenceParagraph;
            mReferenceNode = referenceNode;
            mAltChunkDocument = altChunkDocument;
            mImportFormatMode = importFormatMode;
            mIsInline = isInline;
        }

        internal CompositeNode ReferenceContainer
        {
            get { return mReferenceContainer; }
        }

        internal Node ReferenceNode
        {
            get { return mReferenceNode; }
        }

        internal Document AltChunkDocument
        {
            get { return mAltChunkDocument; }
        }

        internal ImportFormatMode ImportFormatMode
        {
            get { return mImportFormatMode; }
        }

        internal bool IsInline
        {
            get { return mIsInline; }
        }

        private readonly CompositeNode mReferenceContainer;
        private readonly Node mReferenceNode;
        private readonly Document mAltChunkDocument;
        private readonly ImportFormatMode mImportFormatMode;
        private readonly bool mIsInline;
    }
}
