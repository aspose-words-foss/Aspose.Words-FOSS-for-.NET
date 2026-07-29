// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/10/2022 by Alexey Morozov

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Suspends mapped CustomXML runtime update. Resumes update when disposing.
    /// </summary>
    internal sealed class SuspendMappedCustomXmlUpdateDocument : IDisposable
    {
        internal SuspendMappedCustomXmlUpdateDocument(DocumentBase document)
        {
            mDocument = document;
            mDocument.SuspendMappedCustomXmlUpdate();
        }

        //JAVA-added for autoporting of using statements.
        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            mDocument.ResumeMappedCustomXmlUpdate();
        }

        private readonly DocumentBase mDocument;
    }
}
