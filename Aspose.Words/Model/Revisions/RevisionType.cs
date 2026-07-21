// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/04/2012 by Denis Darkin
namespace Aspose.Words
{
    /// <summary>
    /// Specifies the type of change being tracked in <see cref="Revision"/>.
    /// </summary>
    // Additional comments are placed to indicate correspondence to MS Word DOM wdRevisionType enum.
    public enum RevisionType
    {
        // wdRevisionInsert - Insertion. 
        // wdRevisionCellInsertion - Table cell inserted. 
        /// <summary>
        /// New content was inserted in the document.
        /// </summary>
        Insertion,

        // wdRevisionDelete - Deletion. 
        // wdRevisionCellDeletion - Table cell deleted. 
        /// <summary>
        /// Content was removed from the document.
        /// </summary>
        Deletion,
        
        // wdRevisionSectionProperty - Section property changed. 
        // wdRevisionTableProperty - Table property changed. 
        // wdRevisionParagraphProperty - Paragraph property changed. 
        // wdRevisionStyle - Style changed. 
        // wdRevisionProperty - Property changed. 
        // wdRevisionDisplayField - Field display changed. 
        // wdRevisionParagraphNumber - Paragraph number changed. 
        /// <summary>
        /// Change of formatting was applied to the parent node.
        /// </summary>
        FormatChange,

        // wdRevisionStyleDefinition - Style definition changed. 
        /// <summary>
        /// Change of formatting was applied to the parent style.
        /// </summary>
        StyleDefinitionChange,

        // wdRevisionMovedFrom - Content moved from. 
        // wdRevisionMovedTo - Content moved to. 
        /// <summary>
        /// Content was moved in the document.
        /// </summary>
        Moving,

        // not supported in model
        // wdRevisionReconcile - Revision marked as reconciled conflict. 
        // wdRevisionConflict - Revision marked as a conflict. 
        // wdRevisionReplace - Replaced.
        // wdRevisionCellMerge - Table cells merged. 
    }
}
