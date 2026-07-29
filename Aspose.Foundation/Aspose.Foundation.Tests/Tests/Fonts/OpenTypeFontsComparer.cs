// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov
using System.IO;
using Aspose.Collections;
using Aspose.Fonts.Sfnt;
using Aspose.Fonts.TrueType;
using Aspose.IO;
using NUnit.Framework;

namespace Aspose.Tests.Fonts
{
    /// <summary>
    /// Allows to compare OpenType fonts by content.
    /// </summary>
    internal static class OpenTypeFontsComparer
    {
        public static string GetGlyphMessage(int glyphId)
        {
            return string.Format("GlyphIndex={0}", glyphId);
        }

    }
}
