// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2012 by Konstantin Kornilov

using Aspose.Collections;

namespace Aspose.Fonts
{
    /// <summary>
    /// Performs character replacement according to Windows 1252 codepage.
    /// </summary>
    /// <remarks>
    /// MW seems supports some obsolete non-Unicode fonts by mapping Unicode codepoint to Win1252+PUA.
    /// </remarks>
    internal class Win1252PuaCharacterReplacer : TableCharacterReplacer
    {
        public Win1252PuaCharacterReplacer()
            : base(gReplacements)
        {
        }

        static Win1252PuaCharacterReplacer()
        {
            gReplacements = new IntToIntDictionary();

            // See http://en.wikipedia.org/wiki/Windows-1252 for codepage to Unicode table.
            // Character code and Unicode value differs only for [0x80;0x9f] range.
            gReplacements.Add(0x20AC, 0xF080);
            gReplacements.Add(0x201A, 0xF082);
            gReplacements.Add(0x0192, 0xF083);
            gReplacements.Add(0x201E, 0xF084);
            gReplacements.Add(0x2026, 0xF085);
            gReplacements.Add(0x2020, 0xF086);
            gReplacements.Add(0x2021, 0xF087);
            gReplacements.Add(0x02C6, 0xF088);
            gReplacements.Add(0x2030, 0xF089);
            gReplacements.Add(0x0160, 0xF08A);
            gReplacements.Add(0x2039, 0xF08B);
            gReplacements.Add(0x0152, 0xF08C);
            gReplacements.Add(0x017D, 0xF08E);
            gReplacements.Add(0X2018, 0xF091);
            gReplacements.Add(0x2019, 0xF092);
            gReplacements.Add(0x201C, 0xF093);
            gReplacements.Add(0x201D, 0xF094);
            gReplacements.Add(0x2022, 0xF095);
            gReplacements.Add(0x2013, 0xF096);
            gReplacements.Add(0x2014, 0xF097);
            gReplacements.Add(0x02DC, 0xF098);
            gReplacements.Add(0x2122, 0xF099);
            gReplacements.Add(0x0161, 0xF09A);
            gReplacements.Add(0x203A, 0xF09B);
            gReplacements.Add(0x0153, 0xF09C);
            gReplacements.Add(0x017E, 0xF09E);
            gReplacements.Add(0x0178, 0xF09F);
        }

        private static readonly IntToIntDictionary gReplacements;
    }
}
