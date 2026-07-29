// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2011 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Implement this interface if you want to have your own custom method called to 
    /// capture loss of fidelity warnings that can occur during document loading or saving.
    /// </summary>
    [CppVirtualInheritance("System.Object")]
    public interface IWarningCallback
    {
        /// <summary>
        /// Aspose.Words invokes this method when it encounters some issue during document loading 
        /// or saving that might result in loss of formatting or data fidelity.
        /// </summary>
        void Warning(WarningInfo info);
    }
}
