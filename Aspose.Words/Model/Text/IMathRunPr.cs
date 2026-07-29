// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/16/2015 by Alexey Noskov

namespace Aspose.Words
{
    /// <summary>
    /// Interface used for unified reading of MathML run properties from main document content and form Dml.
    /// </summary>
    internal interface IMathRunPr
    {
        bool IsDml { get; }

        int Count { [CodePorting.Translator.Cs2Cpp.CppConstMethod()] get;  }
    }
}
