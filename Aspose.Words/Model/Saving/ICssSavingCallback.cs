// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/09/2012 by Alexey Butalov

using Aspose.JavaAttributes;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Implement this interface if you want to control how Aspose.Words saves CSS (Cascading Style Sheet) when 
    /// saving a document to HTML.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppVirtualInheritance("System.Object")]
    public interface ICssSavingCallback
    {
        /// <summary>
        /// Called when Aspose.Words saves an CSS (Cascading Style Sheet).
        /// </summary>
        [JavaThrows(true)]
        void CssSaving(CssSavingArgs args);
    }
}
