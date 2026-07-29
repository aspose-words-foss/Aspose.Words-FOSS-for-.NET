// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/03/2014 by Alexey Noskov

using System.Diagnostics;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>Represents a single character with RunPr formatting.</summary>
    [DebuggerDisplay("{Character}")]
    internal class RichChar : IChar
    {
        internal RichChar(char c, RunPr runPr)
        {
            Debug.Assert(runPr != null);
            Character = c;
            RunPr = runPr;
        }

        public char ToSystemChar()
        {
            return Character;
        }

        IChar IChar.ToUpper()
        {
            return ToUpperInternal();
        }

        internal RichChar ToUpperInternal()
        {
            return new RichChar(char.ToUpper(Character), RunPr);
        }

        IChar IChar.ToLower()
        {
            return ToLowerInternal();
        }

        internal RichChar ToLowerInternal()
        {
            return new RichChar(char.ToLower(Character), RunPr);
        }

        internal char Character
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get;
        }

        internal RunPr RunPr { get; }

        internal RichString ToRichString()
        {
            return new RichString(this);
        }

        internal static readonly RichChar NullChar = new RichChar(char.MinValue, new RunPr());
    }
}
