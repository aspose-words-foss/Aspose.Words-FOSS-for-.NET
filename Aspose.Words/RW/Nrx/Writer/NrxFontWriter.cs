// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2019 by Andrey Noskov

using System;
using Aspose.Words.Fonts;

namespace Aspose.Words.RW.Nrx.Writer
{
    /// <summary>
    /// Implements writing common elements related to font for both DOCX and WordML formats.
    /// </summary>
    internal static class NrxFontWriter
    {
        internal static void WriteAltName(FontInfo fontInfo, NrxXmlBuilder builder, SaveFormat sf)
        {
            Debug.Assert((sf == SaveFormat.Docx) || (sf == SaveFormat.WordML));

            if (StringUtil.HasChars(fontInfo.AltName))
            {
                // WORDSNET-19341 On Linux String.IndexOf() works slightly different and returns zero 
                // instead of negative value (like on Windows), use String.Contains() instead of IndexOf() 
                // to make it works on both Linux and Windows the same way.
                string altName = fontInfo.AltName;
                if (altName.Contains("\0"))
                    altName = altName.Substring(0, altName.IndexOf("\0", StringComparison.Ordinal));

                builder.WriteVal("w:altName", altName);
            }
        }
    }
}
