// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2013 by Vyacheslav Durin

using Aspose.Collections;

namespace Aspose.Fonts
{
    /// <summary>
    /// Performs character replacement when the current platform is Linux.
    /// </summary>
    internal class LinuxCharacterReplacer : TableCharacterReplacer
    {
        public LinuxCharacterReplacer()
            : base(gReplacements)
        {
        }

        static LinuxCharacterReplacer()
        {
            gReplacements = new IntToIntDictionary();

            gReplacements.Add(61623, 8226); // I didn't manage to find which font supports U+F0B7. So replace it with general bullet.
            gReplacements.Add(0, 32); // on Linux ? is shown instead of empty char
            gReplacements.Add(8899, 8746);  // union char (related to the math font replacement)
            gReplacements.Add(8901, 8729);  // Dot Operator to bullet operator (related to the math font replacement)
        }

        private static readonly IntToIntDictionary gReplacements;
    }
}