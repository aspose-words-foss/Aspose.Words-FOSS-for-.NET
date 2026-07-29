// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/12/2010 by Andrey Soldatov

using System;
using System.IO;
using Aspose.Common;
using Aspose.Words.Fonts;

namespace Aspose.Words.RW.Docx
{
    /// <summary>
    /// <para>Embedded fonts are saved in DOCX package in obfuscated form. This class helps to obfuscate them on saving and
    /// de-obfuscate on loading.</para>
    /// <para>For algorithm see ISO/IEC_29500-1_2008, 17.8.1 Font Embedding.</para>
    /// <para>To write obfuscated font to file, call <see cref="WriteObfuscatedToStream"/>.</para>
    /// <para>To de-obfuscate font read from file, call <see cref="DeObfuscate"/>.</para>
    /// </summary>
    internal static class DocxFontObfuscator
    {
        /// <summary>
        /// <para>Writes obfuscated font to stream.</para>
        /// </summary>
        /// <param name="embeddedFont">Written font.</param>
        /// <param name="outputStream">Stream to write obfuscated font.</param>
        internal static string WriteObfuscatedToStream(EmbeddedFont embeddedFont, Stream outputStream)
        {
            byte[] fontData = embeddedFont.Data;

            if (fontData == null)
                throw new ArgumentNullException("fontData");

            byte[] obfuscatedHead = new byte[System.Math.Min(fontData.Length, ObfuscatedDataLength)];
            Array.Copy(fontData, obfuscatedHead, obfuscatedHead.Length);

            Guid guid = RandomUtil.NewGuid(fontData);

            ObfuscateOrDeobfuscateFontData(obfuscatedHead, guid);

            outputStream.Write(obfuscatedHead, 0, obfuscatedHead.Length);
            outputStream.Write(fontData, obfuscatedHead.Length, embeddedFont.Data.Length - obfuscatedHead.Length);
            
            return guid.ToString("B").ToUpper(); // "B" means "in braces"
        }

        /// <summary>
        /// De-obfuscates font read from DOCX package.
        /// </summary>
        /// <param name="fontData">Font data, obfuscated on input and de-obfuscated on output.</param>
        /// <param name="fontKey">Key in GUID format, read from fontKey attribute int fontTable.xml.</param>
        internal static void DeObfuscate(byte[] fontData, string fontKey)
        {
            if (fontData == null)
                throw new ArgumentNullException("fontData");

            Guid guid = new Guid(fontKey);

            ObfuscateOrDeobfuscateFontData(fontData, guid);
        }

        /// <summary>
        /// <para>Obfuscates or deobfuscate font data using obfuscation guid.</para>
        /// <para>The algorithm is described in ISO/IEC_29500-1_2008, 17.8.1 Font Embedding.</para>
        /// </summary>
        private static void ObfuscateOrDeobfuscateFontData(byte[] fontData, Guid guid)
        {
            int dataPos = 0;
            byte[] guidBytes = ArrayUtil.GuidToByteArray(guid);

            for (int pass = 0; pass < 2; pass++)
                for (int i = guidBytes.Length - 1; (i >= 0) && (dataPos < fontData.Length); i--)
                    fontData[dataPos++] ^= guidBytes[i];
        }

        private const int ObfuscatedDataLength = 32;
    }
}
