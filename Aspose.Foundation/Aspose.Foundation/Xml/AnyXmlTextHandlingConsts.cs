// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/08/2010 by Roman Korchagin

namespace Aspose.Xml
{
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    public static class AnyXmlTextHandlingConsts
    {
        /// <summary>
        /// Text and significant whitespace is returned. 
        /// This is the mode to read most of the "normal" XML documents.
        /// </summary>
        public const AnyXmlTextHandling TextAndSignificant = 
            AnyXmlTextHandling.Text | AnyXmlTextHandling.SignificantWhitespace;
        /// <summary>
        /// Text, significant and ignorable whitespace is returned to the caller.
        /// Returning ignorable whitespace to the caller is used when reading ODT because ODT rules for whitespace are different from .NET.
        /// </summary>
        public const AnyXmlTextHandling TextAndSignificantAndIgnorable =
            AnyXmlTextHandling.Text | AnyXmlTextHandling.SignificantWhitespace | AnyXmlTextHandling.Whitespace;
        /// <summary>
        /// All of defined here XML node types are returned to the caller.
        /// </summary>
        public const AnyXmlTextHandling All = AnyXmlTextHandling.Text |
            AnyXmlTextHandling.SignificantWhitespace | AnyXmlTextHandling.Whitespace |
            AnyXmlTextHandling.ProcessingInstruction | AnyXmlTextHandling.EntityReference;
    }
}
