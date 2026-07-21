// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2016 by Konstantin Kornilov

using Aspose.Collections;

namespace Aspose.Fonts
{
    /// <summary>
    /// Performs character replacement for control characters.
    /// </summary>
    /// <remarks>
    /// For now only different space chars are handled here. They should have special treatment in layout and probably 
    /// should be replaced there. But sometimes they are passed to rendering as is. This replacer make sure that
    /// spaces are rendered properly.
    /// </remarks>
    internal class ControlCharacterReplacer : TableCharacterReplacer
    {
        public ControlCharacterReplacer()
            : base(gReplacements)
        {
        }

        static ControlCharacterReplacer()
        {
            gReplacements = new IntToIntDictionary();

            // Spaces
            gReplacements.Add(0x2000, 0x0020); // EN QUAD
            gReplacements.Add(0x2001, 0x0020); // EM QUAD
            gReplacements.Add(0x2002, 0x0020); // EN SPACE
            gReplacements.Add(0x2003, 0x0020); // EM SPACE
            gReplacements.Add(0x2004, 0x0020); // THREE-PER-EM SPACE
            gReplacements.Add(0x2005, 0x0020); // FOUR-PER-EM SPACE
            gReplacements.Add(0x2006, 0x0020); // SIX-PER-EM SPACE
            gReplacements.Add(0x2007, 0x0020); // FIGURE SPACE
            gReplacements.Add(0x2008, 0x0020); // PUNCTUATION SPACE
            gReplacements.Add(0x2009, 0x0020); // THIN SPACE
            gReplacements.Add(0x200A, 0x0020); // HAIR SPACE
        }

        private static readonly IntToIntDictionary gReplacements;
    }
}
