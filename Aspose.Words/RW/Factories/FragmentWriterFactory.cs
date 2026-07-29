// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/08/2012 by Alexey Butalov

using System;
using Aspose.Words.RW.Txt.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Factories
{
    /// <summary>
    /// Document fragment writer factory.
    /// </summary>
    internal static class FragmentWriterFactory
    {
        /// <summary>
        /// Creates and returns a new instance of document fragment writer class.
        /// </summary>
        internal static IDocumentFragmentWriter CreateFragmentWriter(SaveFormat saveFormat)
        {
            switch (saveFormat)
            {
                case SaveFormat.Html:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Text:
                    return new TxtDocumentFragmentWriter();
                default:
                    throw new InvalidOperationException("Exporting fragments of a document in this format is not supported.");
            }
        }

        /// <summary>
        /// Creates and returns a document fragment writer object using the specified save options.
        /// </summary>
        internal static IDocumentFragmentWriter CreateFragmentWriter(SaveOptions saveOptions)
        {
            switch (saveOptions.SaveFormat)
            {
                case SaveFormat.Html:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Text:
                    return new TxtDocumentFragmentWriter((TxtSaveOptions) saveOptions);
                default:
                    throw new InvalidOperationException("Exporting fragments of a document in this format is not supported.");
            }
        }
    }
}
