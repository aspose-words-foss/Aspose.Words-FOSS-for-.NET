// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/02/2008 by Roman Korchagin


using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.RW.Doc
{
    /// <summary>
    /// The value for the Fib.nFib field.
    /// </summary>
    [CppEnumEnableMetadata]
    internal enum NFibValue
    {
        Word60ForWin = 101,
        Word60ForMac = 103,

        /// <summary>
        /// Maybe Word95 is 105?
        /// </summary>
        Word95 = 104,

        /// <summary>
        /// Although Word97 is nFib = 193, the DOC spec sometimes refers to this value as Word 97.
        /// </summary>
        Value106 = 106,

        /// <summary>
        /// This value occurred sometimes. Meaning is unknown.
        /// </summary>
        Value191 = 191,

        /// <summary>
        /// A special empty document is installed with Word 97, Word 2000, Word 2002, and
        /// Office Word 2003 to allow "Create New Word Document" from the operating system.
        /// This document has an nFib of 0x00C0.
        /// </summary>
        Word97Empty = 192,

        /// <summary>
        /// 0x00C1.
        /// </summary>
        Word97 = 193,

        /// <summary>
        /// The BiDi build of Word 97 differentiates its documents by saving 0x00C2 as the nFib.
        /// </summary>
        Word97BiDi = 194,

        /// <summary>
        /// 0x00D9
        /// </summary>
        Word2000 = 217,

        /// <summary>
        /// 0x0101
        /// </summary>
        Word2002 = 257,

        /// <summary>
        /// 0x010C
        /// </summary>
        Word2003 = 268,

        /// <summary>
        /// 0x0112
        /// </summary>
        Word2007 = 274
    }
}
