// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/03/2019 by Konstantin Kornilov

using Aspose.Collections;

namespace Aspose.Fonts
{
    /// <summary>
    /// Contains utility methods to handle "Courier New" font hacks.
    /// </summary>
    /// <remarks>
    /// "Courier New" requires advanced typography support to render some diacritics. Use the zero advance for hardcoded
    /// list of diacritics improve the rendering quality in the meantime.
    /// </remarks>
    internal static class CourierNewHackUtil
    {
        static CourierNewHackUtil()
        {
            // I'm not 100% sure which chars should have zero advance. There was a theory that it should be chars
            // with GDEF class 3 (Mark glyph). But experiments shows that not all the class 3 chars have zero advance.
            // Here is the list of class 3 chars confirmed to have zero advance.
            gZeroAdvanceChars = new IntToIntDictionary();
            gZeroAdvanceChars.Add(0x0300, 0);
            gZeroAdvanceChars.Add(0x0301, 0);
            gZeroAdvanceChars.Add(0x0303, 0);
            gZeroAdvanceChars.Add(0x0307, 0);
            gZeroAdvanceChars.Add(0x0309, 0);
            gZeroAdvanceChars.Add(0x0316, 0);
            gZeroAdvanceChars.Add(0x0317, 0);
            gZeroAdvanceChars.Add(0x0318, 0);
            gZeroAdvanceChars.Add(0x0319, 0);
            gZeroAdvanceChars.Add(0x031C, 0);
            gZeroAdvanceChars.Add(0x031D, 0);
            gZeroAdvanceChars.Add(0x031E, 0);
            gZeroAdvanceChars.Add(0x031F, 0);
            gZeroAdvanceChars.Add(0x0320, 0);
            gZeroAdvanceChars.Add(0x0323, 0);
            gZeroAdvanceChars.Add(0x0324, 0);
            gZeroAdvanceChars.Add(0x0325, 0);
            gZeroAdvanceChars.Add(0x0326, 0);
            gZeroAdvanceChars.Add(0x0329, 0);
            gZeroAdvanceChars.Add(0x032A, 0);
            gZeroAdvanceChars.Add(0x032B, 0);
            gZeroAdvanceChars.Add(0x032C, 0);
            gZeroAdvanceChars.Add(0x032D, 0);
            gZeroAdvanceChars.Add(0x032E, 0);
            gZeroAdvanceChars.Add(0x032F, 0);
            gZeroAdvanceChars.Add(0x0330, 0);
            gZeroAdvanceChars.Add(0x0331, 0);
            gZeroAdvanceChars.Add(0x0332, 0);
            gZeroAdvanceChars.Add(0x0333, 0);
            gZeroAdvanceChars.Add(0x0339, 0);
            gZeroAdvanceChars.Add(0x033A, 0);
            gZeroAdvanceChars.Add(0x033B, 0);
            gZeroAdvanceChars.Add(0x033C, 0);
            gZeroAdvanceChars.Add(0x0345, 0);
            gZeroAdvanceChars.Add(0x0347, 0);
            gZeroAdvanceChars.Add(0x0348, 0);
            gZeroAdvanceChars.Add(0x0349, 0);
            gZeroAdvanceChars.Add(0x034D, 0);
            gZeroAdvanceChars.Add(0x034E, 0);
            gZeroAdvanceChars.Add(0x0353, 0);
            gZeroAdvanceChars.Add(0x0354, 0);
            gZeroAdvanceChars.Add(0x0355, 0);
            gZeroAdvanceChars.Add(0x0356, 0);
            gZeroAdvanceChars.Add(0x0359, 0);
            gZeroAdvanceChars.Add(0x035A, 0);
            gZeroAdvanceChars.Add(0x0591, 0);
            gZeroAdvanceChars.Add(0x0592, 0);
            gZeroAdvanceChars.Add(0x0593, 0);
            gZeroAdvanceChars.Add(0x0594, 0);
            gZeroAdvanceChars.Add(0x0595, 0);
            gZeroAdvanceChars.Add(0x0596, 0);
            gZeroAdvanceChars.Add(0x0597, 0);
            gZeroAdvanceChars.Add(0x0598, 0);
            gZeroAdvanceChars.Add(0x0599, 0);
            gZeroAdvanceChars.Add(0x059A, 0);
            gZeroAdvanceChars.Add(0x059B, 0);
            gZeroAdvanceChars.Add(0x059C, 0);
            gZeroAdvanceChars.Add(0x059D, 0);
            gZeroAdvanceChars.Add(0x059E, 0);
            gZeroAdvanceChars.Add(0x059F, 0);
            gZeroAdvanceChars.Add(0x05A0, 0);
            gZeroAdvanceChars.Add(0x05A1, 0);
            gZeroAdvanceChars.Add(0x05A2, 0);
            gZeroAdvanceChars.Add(0x05A3, 0);
            gZeroAdvanceChars.Add(0x05A4, 0);
            gZeroAdvanceChars.Add(0x05A5, 0);
            gZeroAdvanceChars.Add(0x05A6, 0);
            gZeroAdvanceChars.Add(0x05A7, 0);
            gZeroAdvanceChars.Add(0x05A8, 0);
            gZeroAdvanceChars.Add(0x05A9, 0);
            gZeroAdvanceChars.Add(0x05AA, 0);
            gZeroAdvanceChars.Add(0x05AB, 0);
            gZeroAdvanceChars.Add(0x05AC, 0);
            gZeroAdvanceChars.Add(0x05AD, 0);
            gZeroAdvanceChars.Add(0x05AE, 0);
            gZeroAdvanceChars.Add(0x05AF, 0);
            gZeroAdvanceChars.Add(0x05B0, 0);
            gZeroAdvanceChars.Add(0x05B1, 0);
            gZeroAdvanceChars.Add(0x05B2, 0);
            gZeroAdvanceChars.Add(0x05B3, 0);
            gZeroAdvanceChars.Add(0x05B4, 0);
            gZeroAdvanceChars.Add(0x05B5, 0);
            gZeroAdvanceChars.Add(0x05B6, 0);
            gZeroAdvanceChars.Add(0x05B7, 0);
            gZeroAdvanceChars.Add(0x05B8, 0);
            gZeroAdvanceChars.Add(0x05B9, 0);
            gZeroAdvanceChars.Add(0x05BB, 0);
            gZeroAdvanceChars.Add(0x05BC, 0);
            gZeroAdvanceChars.Add(0x05BD, 0);
            gZeroAdvanceChars.Add(0x05BF, 0);
            gZeroAdvanceChars.Add(0x05C4, 0);
            gZeroAdvanceChars.Add(0x05C5, 0);
            gZeroAdvanceChars.Add(0x05C7, 0);
            gZeroAdvanceChars.Add(0x0610, 0);
            gZeroAdvanceChars.Add(0x0611, 0);
            gZeroAdvanceChars.Add(0x0612, 0);
            gZeroAdvanceChars.Add(0x0613, 0);
            gZeroAdvanceChars.Add(0x0614, 0);
            gZeroAdvanceChars.Add(0x0615, 0);
            gZeroAdvanceChars.Add(0x0616, 0);
            gZeroAdvanceChars.Add(0x0617, 0);
            gZeroAdvanceChars.Add(0x0618, 0);
            gZeroAdvanceChars.Add(0x0619, 0);
            gZeroAdvanceChars.Add(0x061A, 0);
            gZeroAdvanceChars.Add(0x064B, 0);
            gZeroAdvanceChars.Add(0x064C, 0);
            gZeroAdvanceChars.Add(0x064D, 0);
            gZeroAdvanceChars.Add(0x064E, 0);
            gZeroAdvanceChars.Add(0x064F, 0);
            gZeroAdvanceChars.Add(0x0650, 0);
            gZeroAdvanceChars.Add(0x0651, 0);
            gZeroAdvanceChars.Add(0x0652, 0);
            gZeroAdvanceChars.Add(0x0653, 0);
            gZeroAdvanceChars.Add(0x0654, 0);
            gZeroAdvanceChars.Add(0x0655, 0);
            gZeroAdvanceChars.Add(0x0656, 0);
            gZeroAdvanceChars.Add(0x0657, 0);
            gZeroAdvanceChars.Add(0x0658, 0);
            gZeroAdvanceChars.Add(0x0659, 0);
            gZeroAdvanceChars.Add(0x065A, 0);
            gZeroAdvanceChars.Add(0x065B, 0);
            gZeroAdvanceChars.Add(0x065C, 0);
            gZeroAdvanceChars.Add(0x065D, 0);
            gZeroAdvanceChars.Add(0x065E, 0);
            gZeroAdvanceChars.Add(0x065F, 0);
            gZeroAdvanceChars.Add(0x0670, 0);
            gZeroAdvanceChars.Add(0x06D6, 0);
            gZeroAdvanceChars.Add(0x06D7, 0);
            gZeroAdvanceChars.Add(0x06D8, 0);
            gZeroAdvanceChars.Add(0x06D9, 0);
            gZeroAdvanceChars.Add(0x06DA, 0);
            gZeroAdvanceChars.Add(0x06DB, 0);
            gZeroAdvanceChars.Add(0x06DC, 0);
            gZeroAdvanceChars.Add(0x06DF, 0);
            gZeroAdvanceChars.Add(0x06E0, 0);
            gZeroAdvanceChars.Add(0x06E1, 0);
            gZeroAdvanceChars.Add(0x06E2, 0);
            gZeroAdvanceChars.Add(0x06E3, 0);
            gZeroAdvanceChars.Add(0x06E4, 0);
            gZeroAdvanceChars.Add(0x06E5, 0);
            gZeroAdvanceChars.Add(0x06E6, 0);
            gZeroAdvanceChars.Add(0x06E7, 0);
            gZeroAdvanceChars.Add(0x06E8, 0);
            gZeroAdvanceChars.Add(0x06EA, 0);
            gZeroAdvanceChars.Add(0x06EB, 0);
            gZeroAdvanceChars.Add(0x06EC, 0);
            gZeroAdvanceChars.Add(0x06ED, 0);
            gZeroAdvanceChars.Add(0x08D4, 0);
            gZeroAdvanceChars.Add(0x08D5, 0);
            gZeroAdvanceChars.Add(0x08D6, 0);
            gZeroAdvanceChars.Add(0x08D7, 0);
            gZeroAdvanceChars.Add(0x08D8, 0);
            gZeroAdvanceChars.Add(0x08D9, 0);
            gZeroAdvanceChars.Add(0x08DA, 0);
            gZeroAdvanceChars.Add(0x08DB, 0);
            gZeroAdvanceChars.Add(0x08DC, 0);
            gZeroAdvanceChars.Add(0x08DD, 0);
            gZeroAdvanceChars.Add(0x08DE, 0);
            gZeroAdvanceChars.Add(0x08DF, 0);
            gZeroAdvanceChars.Add(0x08E0, 0);
            gZeroAdvanceChars.Add(0x08E1, 0);
            gZeroAdvanceChars.Add(0x08E3, 0);
            gZeroAdvanceChars.Add(0x08FF, 0);
            gZeroAdvanceChars.Add(0x1DC2, 0);
            gZeroAdvanceChars.Add(0x1DCA, 0);
            gZeroAdvanceChars.Add(0x1DFF, 0);
            gZeroAdvanceChars.Add(0xFB1E, 0);
            gZeroAdvanceChars.Add(0xFC5E, 0);
            gZeroAdvanceChars.Add(0xFC5F, 0);
            gZeroAdvanceChars.Add(0xFC60, 0);
            gZeroAdvanceChars.Add(0xFC61, 0);
            gZeroAdvanceChars.Add(0xFC62, 0);
            gZeroAdvanceChars.Add(0xFC63, 0);
            
        }

        /// <summary>
        /// Returns true if zero advance hack should be used for the char.
        /// </summary>
        public static bool IsZeroAdvanceChar(int charCode)
        {
            return gZeroAdvanceChars.ContainsKey(charCode);
        }
        
        private static readonly IntToIntDictionary gZeroAdvanceChars;
    }
}
