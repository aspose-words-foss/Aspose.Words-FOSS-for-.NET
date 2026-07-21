// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/12/2016 by Alexey Morozov

namespace Aspose.Words.Formatting.Intern
{
    /// <summary>
    /// Implements pooled attribute collection.
    /// </summary>
    internal class InternPoolItem
    {
        internal InternPoolItem(InternManager internManager)
        {
            mInternManager = internManager;
        }

        internal InternManager InternManager 
        {
            get { return mInternManager; }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        internal AttrCollection Pr;
        internal int RefCount;
        internal int Id;

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly InternManager mInternManager;
    }
}
