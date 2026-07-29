// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/09/2017 by Konstantin Kornilov

using System.Text;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Helper methods to work with TrueType encodings.
    /// </summary>
    internal abstract class TTEncoding
    {
        public static TTEncoding GetEncoding(int platformId, int encodingId)
        {
            // The task here is to use a proper encoding to read the string.
            // At the moment we support only a few most common encoding.
            if (platformId == Cmap.PlatformIdUnicode)
                return GetBigEndianUnicode();

            if (platformId == Cmap.PlatformIdMacintosh &&
                ((encodingId == Cmap.MacintoshEncodingIdRoman) ||
                 (encodingId == Cmap.MacintoshEncodingIdJapanese) ||
                 (encodingId == Cmap.MacintoshEncodingIdKorean) ||
                 (encodingId == Cmap.MacintoshEncodingIdSimplifiedChinese)))
                return GetCp1252();

            if (platformId == Cmap.PlatformIdMacintosh &&
                encodingId == Cmap.MacintoshEncodingIdTraditionalChinese)
                return GetMacTradChinese();

            if (platformId == Cmap.PlatformIdMicrosoft &&
                ((encodingId == Cmap.MicrosoftEncodingIdUnicodeUcs2) ||
                 (encodingId == Cmap.MicrosoftEncodingIdUnicodeUcs4) ||
                 (encodingId == Cmap.MicrosoftEncodingIdSymbol)))
                return GetBigEndianUnicode();

            if (platformId == Cmap.PlatformIdMicrosoft &&
                encodingId == Cmap.MicrosoftEncodingIdPrc)
                return GetPrc();

            if (platformId == Cmap.PlatformIdMicrosoft &&
                encodingId == Cmap.MicrosoftEncodingIdBig5)
                return GetBig5();

            return null;
        }

        public abstract string GetString(byte[] bytes);

        private static TTEncoding GetBigEndianUnicode()
        {
            if(gBigEndianUnicode == null)
                lock (gSyncRoot)
                {
                    if(gBigEndianUnicode == null)
                        gBigEndianUnicode = new TTEncodingRegular(Encoding.BigEndianUnicode);
                }

            return gBigEndianUnicode;
        }

        private static TTEncoding GetCp1252()
        {
            if(gCp1252 == null)
                lock (gSyncRoot)
                {
                    if(gCp1252 == null)
                        gCp1252 = new TTEncodingRegular(Encoding.GetEncoding(1252));
                }

            return gCp1252;
        }

        private static TTEncoding GetPrc()
        {
            if(gPrc == null)
                lock (gSyncRoot)
                {
                    if(gPrc == null)
                        gPrc = new TTEncodingLegacyChinese(Encoding.GetEncoding("gb2312"));
                }

            return gPrc;
        }

        private static TTEncoding GetBig5()
        {
            if(gBig5 == null)
                lock (gSyncRoot)
                {
                    if(gBig5 == null)
                        gBig5 = new TTEncodingLegacyChinese(Encoding.GetEncoding(950));
                }

            return gBig5;
        }

        private static TTEncoding GetMacTradChinese()
        {
            if(gMacTradChinese == null)
                lock (gSyncRoot)
                {
                    if(gMacTradChinese == null)
                        gMacTradChinese = new TTEncodingLegacyChinese(Encoding.GetEncoding(10002));
                }

            return gMacTradChinese;
        }

        private static TTEncoding gBigEndianUnicode;
        private static TTEncoding gCp1252;
        private static TTEncoding gPrc;
        private static TTEncoding gBig5;
        private static TTEncoding gMacTradChinese;
        private static readonly object gSyncRoot = new object();
    }
}
