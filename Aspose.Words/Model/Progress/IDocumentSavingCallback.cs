// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/12/2021 by Dmitry Sokolov

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Implement this interface if you want to have your own custom method called during saving a document.
    /// </summary>
    public interface IDocumentSavingCallback
    {
        /// <summary>
        /// This is called to notify of document saving progress.
        /// </summary>
        /// <param name="args">An argument of the event.</param>
        /// <remarks>
        /// The primary uses for this interface is to allow application code to obtain progress status and abort saving process.<para/>
        /// An exception should be threw from the progress callback for abortion and it should be caught in the consumer code.<para/>
        /// </remarks>
        void Notify(DocumentSavingArgs args);
    }
}
