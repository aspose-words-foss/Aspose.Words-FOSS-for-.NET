// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2011 by Konstantin Kornilov

using Aspose.Collections;

namespace Aspose.Fonts
{
    /// <summary>
    /// Performs character replacement according to provided table.
    /// </summary>
    internal abstract class TableCharacterReplacer : CharacterReplacerBase
    {
        protected TableCharacterReplacer(IntToIntDictionary table)
        {
            mTable = table;
        }

        internal override void Replace(int charCode, IntList replacements)
        {
            int c = mTable[charCode];
            if (!IntToIntDictionary.IsNullSubstitute(c))
                replacements.Add(c);
        }

        private readonly IntToIntDictionary mTable;
    }
}
