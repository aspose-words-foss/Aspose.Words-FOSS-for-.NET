// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2012 by Konstantin Kornilov

using Aspose.Collections;

namespace Aspose.Fonts
{
    /// <summary>
    /// Performs character replacement according to Symbol codepage.
    /// </summary>
    /// <remarks>
    /// Experiments shows that MS Word replaces characters from [0x20;0xFF] range with characters from 
    /// Unicode Private Use Area ([0xF020;0xF0FF] range) and vice versa for symbolic fonts.
    /// </remarks>
    internal class SymbolCharacterReplacer : TableCharacterReplacer
    {
        public SymbolCharacterReplacer()
            : base(gReplacements)
        {
        }

        static SymbolCharacterReplacer()
        {
            gReplacements = new IntToIntDictionary();

            for (int i = 0x20; i <= 0xFF; i++)
            {
                gReplacements.Add(i, i + 0xF000);
                gReplacements.Add(i + 0xF000, i);
            }
        }

        private static readonly IntToIntDictionary gReplacements;
    }
}
