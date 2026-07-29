// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/06/2010 by Dmitry Vorobyev

using Aspose.Collections;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Used when copying nodes from an external document. Uses <see cref="NodeImporter"/> to import the nodes.
    /// </summary>
    internal class ExternalDocumentModifier : INodeModifier
    {
        // WORDSNET-14587 It looks like Word uses UseDestinationStyles mode when updates fields. See for example
        // TestNodeImporter.TestJira14587UpdateField and also some tests in TestLinksAndReferences: TestInclude(), TestIncludeText().
        // So switched to UseDestinationStyles mode by default instead of KeepSourceFormatting.
        internal ExternalDocumentModifier(DocumentBase sourceDocument, DocumentBase destinationDocument, IntToIntBidirectionalMap importedIstds)
            : this(sourceDocument, destinationDocument, ImportFormatMode.UseDestinationStyles, importedIstds)
        {
        }

        internal ExternalDocumentModifier(DocumentBase sourceDocument, DocumentBase destinationDocument, ImportFormatMode importFormatMode)
            : this(sourceDocument, destinationDocument, importFormatMode, null)
        {
        }

        private ExternalDocumentModifier(DocumentBase sourceDocument, DocumentBase destinationDocument, ImportFormatMode importFormatMode, IntToIntBidirectionalMap importedIstds)
        {
            ImportContext context = new ImportContext(sourceDocument, destinationDocument, importFormatMode, null, importedIstds);
            mNodeImporter = new NodeImporter(sourceDocument, destinationDocument, context);
        }

        Node INodeModifier.Modify(Node referenceNode, Node nodeToModify, bool modifyChildren, INodeCloningListener cloningListener)
        {
            return mNodeImporter.ImportNode(nodeToModify, modifyChildren, cloningListener);
        }

        private readonly NodeImporter mNodeImporter;
    }
}
