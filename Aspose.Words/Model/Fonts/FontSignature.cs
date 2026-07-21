// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/01/2018 by Alexey Morozov
using System;
using Aspose.Charset;
using Aspose.Fonts;
using Aspose.Fonts.TrueType;

namespace Aspose.Words.Fonts
{
    /// <summary>
    /// Represents FontSignature which stores information about a font's <see cref="FontUnicodeRanges"/> and <see cref="FontCodepageRanges"/>
    /// </summary>
    internal class FontSignature
    {
        internal FontSignature(byte[] data)
        {
            mData = (data == null) ? new byte[SigLength] : data;
        }

        internal void AddCharset(int charset)
        {            
            if (charset == FontUtil.UndefinedCharset)
                return;

            // Seems there is no codepage for this charset.
            if ((charset == FontUtil.DefaultCharset))
                return;

            int codePageBit = GetCodePageBit(charset);

            if (codePageBit >= 0)
                SetBit(16, codePageBit);
        }


        private void SetBit(int offset, int index)
        {
            int byteNo = (index / 8) + offset;
            int bitNo = index % 8;

            int curByte = mData[byteNo];
            mData[byteNo] = (byte)BitUtil.SetBit(curByte, (byte)(1 << bitNo), true);
        }

        internal byte[] Data
        {
            get { return mData; }
        }

        /// <summary>
        /// Determines CodePage bit by charset.
        /// </summary>
        private static int GetCodePageBit(int charset)
        {
            // There is no such code page in .NET, so let's just return the corresponding bit. 
            if (charset == CodePage.SymbolCharSet)
                return SymbolCodePageBit;

            int codepage = CodePage.CharSetToCodePage(charset, CodePage.WindowsLatin1CodePage);

            return Array.IndexOf(CodePageBits, codepage);
        }

        internal static readonly int[] CodePageBits = new int[]
        {
            // 0
            1252, 1250, 1251, 1253, 1254, 1255, 1256, 1257,
            // 8
            1258, 0, 0, 0, 0, 0, 0, 0,
            // 16
            874, 932, 936, 949, 950, 1361, 0, 0,
            // 24
            0, 0, 0, 0, 0, 0, 0, 2,
            // 32
            0, 0, 0, 0, 0, 0, 0, 0,
            // 40
            0, 0, 0, 0, 0, 0, 0, 0,
            // 48
            869, 866, 865, 864, 863, 862, 861, 860,
            // 56
            857, 855, 852, 775, 737, 708, 850, 437
        };

        private readonly byte[] mData;

        private const int SigLength = 24;

        private const int SymbolCodePageBit = 31;
    }
}
