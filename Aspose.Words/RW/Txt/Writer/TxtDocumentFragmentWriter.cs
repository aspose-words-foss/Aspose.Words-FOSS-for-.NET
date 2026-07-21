// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/09/2012 by Alexey Butalov

using Aspose.Words.Saving;

namespace Aspose.Words.RW.Txt.Writer
{
    internal class TxtDocumentFragmentWriter : IDocumentFragmentWriter
    {
        internal TxtDocumentFragmentWriter()
        {
            mSaveOptions = new TxtSaveOptions();
        }

        internal TxtDocumentFragmentWriter(TxtSaveOptions saveOptions)
        {
            Debug.Assert(saveOptions != null);
            mSaveOptions = saveOptions;
        }

        #region Implementation of IDocumentFragmentWriter

        public string SaveToString(Node node)
        {
            Debug.Assert(node != null);
            TxtWriter writer = new TxtWriter();
            Document doc = node as Document;
            // FOSS
            return writer.SaveToString(node, mSaveOptions);
        }

        #endregion

        private readonly TxtSaveOptions mSaveOptions;
    }
}
