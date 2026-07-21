// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2014 by Edward Voronov

using System;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Suspends revision tracking. Resumes revision tracking when disposing.
    /// </summary>
    [CppOverrideAccessModifier(AccessModifiers.Public)]
    internal sealed class SuspendTrackRevisionsDocument : IDisposable
    {
#if CPLUSPLUS
        public
#else
        internal
#endif
        SuspendTrackRevisionsDocument(DocumentBase document)
            : this (document, SuspendedRevisionTypes.All)
        {
        }

#if CPLUSPLUS
        public
#else
        internal
#endif
        SuspendTrackRevisionsDocument(DocumentBase document, SuspendedRevisionTypes revisionTypes)
        {
            mDocument = document;
            mDocument.SuspendTrackRevisions(revisionTypes);
            mRevisionTypes = revisionTypes;
        }

        //JAVA-added for autoporting of using statements.
        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            mDocument.ResumeTrackRevisions(mRevisionTypes);
        }

        private readonly DocumentBase mDocument;
        private readonly SuspendedRevisionTypes mRevisionTypes;
    }
}
