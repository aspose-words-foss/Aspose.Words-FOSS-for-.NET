// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/02/2017 by Alexey Butalov

namespace Aspose.Words.Nrx
{
    /// <summary>
    /// Useful constants for dealing with 32 bit colors as they stored in DOC files.
    /// They are different from .NET Color.ToArgb() in order and in transparency/opacity.
    /// The bytes are as follows (from high to low) T, B, G, R
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal static class WordXPColor
    {
        /// <summary>
        /// This is auto color as encountered in DOC files.
        /// It can be translated as "full transparency black".
        /// </summary>
        internal const int Auto        = unchecked((int)0xff000000);
        internal const int Black       = 0x00000000;
        internal const int Blue        = 0x00ff0000;
        internal const int Cyan        = 0x00ffff00;
        internal const int Green       = 0x0000ff00;
        internal const int Magenta     = 0x00ff00ff;
        internal const int Red         = 0x000000ff;
        internal const int Yellow      = 0x0000ffff;
        internal const int White       = 0x00ffffff;
        internal const int DarkBlue    = 0x00800000;
        internal const int DarkCyan    = 0x00808000;
        internal const int DarkGreen   = 0x00008000;
        internal const int DarkMagenta = 0x00800080;
        internal const int DarkRed     = 0x00000080;
        internal const int DarkYellow  = 0x00008080;
        internal const int DarkGray    = 0x00808080;
        internal const int LightGray   = 0x00C0C0C0;
    }
}
