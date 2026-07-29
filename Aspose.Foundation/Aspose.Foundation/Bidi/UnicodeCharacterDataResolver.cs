// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

using System;
#if NET461_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using Aspose.Collections;
using System.Collections.Generic;

namespace Aspose.Bidi
{
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    public static class UnicodeCharacterDataResolver
    {
        /// <summary>
        /// Returns the BiDi type for a given character.
        /// </summary>
        /// <param name="c">A Unicode character for which to get the BiDi type.</param>
        /// <returns>The character BiDi type.</returns>
        public static BidiCharacterType GetBidiCharacterType(char c)
        {
            return (BidiCharacterType)GetCharDataItem(c, BidiCharacterTypeItemIndex);
        }

        /// <summary>
        /// Returns the Unicode category for a given character.
        /// </summary>
        /// <param name="c">A Unicode character for which to get the general Unicode category.</param>
        /// <returns>The character general Unicode category.</returns>
        public static UnicodeGeneralCategory GetUnicodeGeneralCategory(char c)
        {
            return (UnicodeGeneralCategory)GetCharDataItem(c, UnicodeGeneralCategoryItemIndex);
        }

        /// <summary>
        /// Returns the Unicode canonical class for a given character.
        /// </summary>
        /// <param name="c">A Unicode character for which to get the Unicode canonical class.</param>
        /// <returns>The character Unicode canonical class.</returns>
        public static UnicodeCanonicalClass GetUnicodeCanonicalClass(char c)
        {
            return (UnicodeCanonicalClass)GetCharDataItem(c, UnicodeCanonicalClassItemIndex);
        }

        public static UnicodeDecompositionType GetUnicodeDecompositionType(char c)
        {
            return (UnicodeDecompositionType)GetCharDataItem(c, UnicodeDecompositionTypeItemIndex);
        }

        /// <summary>
        /// Returns true, if a specified character has Strong type of Unicode BiDi algorithm. 
        /// </summary>
        public static bool IsStrongBidiCharacterType(char c)
        {
            BidiCharacterType bidiCharacterType = GetBidiCharacterType(c);

            return (bidiCharacterType == BidiCharacterType.L) ||
                   (bidiCharacterType == BidiCharacterType.R) ||
                   (bidiCharacterType == BidiCharacterType.AL);
        }

        /// <summary>
        /// Returns true, if a specified character has Strong Right-To-Left type of Unicode BiDi algorithm. 
        /// </summary>
        public static bool IsStrongRtlBidiCharacterType(char c)
        {
            BidiCharacterType bidiCharacterType = GetBidiCharacterType(c);

            return (bidiCharacterType == BidiCharacterType.R) ||
                   (bidiCharacterType == BidiCharacterType.AL);
        }

        /// <summary>
        /// Indicates whether the specified character has bidi character type L (strong left-to-right).
        /// </summary>
        public static bool IsStrongLtrBidiCharacterType(char c)
        {
            return GetBidiCharacterType(c) == BidiCharacterType.L;
        }

        public static bool IsNumberSeparatorType(BidiCharacterType type)
        {
            return (type == BidiCharacterType.ES) ||
                   (type == BidiCharacterType.CS);
        }

        /// <summary>
        /// Returns one of the enum values packed in <see cref="gCharData"/> item corresponding to its byte index.
        /// </summary>
#if NET461_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static int GetCharDataItem(char c, int itemIndex)
        {
            int storedValue = gCharData[c];
            if (itemIndex == 0)
                return (storedValue & MaxByte);

            int offset = itemIndex * 8;
            return ((storedValue & MaxByte << offset) >> offset);
        }

        /// <summary>
        /// Sets one of the enum values packed in <see cref="gCharData"/> item corresponding to its byte index.
        /// </summary>
        private static void SetCharDataItem(char c, int itemIndex, int value)
        {
            int storedValue = gCharData[c];
            if (itemIndex == 0)
            {
                storedValue = storedValue & ~MaxByte | value & MaxByte;
            }
            else
            {
                int offset = itemIndex * 8;
                int byteMask = MaxByte << offset;
                storedValue = storedValue & ~byteMask | value << offset & byteMask;
            }

            gCharData[c] = storedValue;
        }

        public static string GetUnicodeDecompositionMapping(char c)
        {
            return gDecomposeMap.GetValueOrNull(c);
        }

        public static char Compose(char firstChar, char secondChar)
        {
            // See GetInt32Key(string) for explanation of the following check.
            Debug.Assert(firstChar != BidiChars.NotAChar);

            char value = gComposeMap1[GetInt32Key(firstChar, secondChar)];
            return IntToCharDictionary.IsNullSubstitute(value) ? BidiChars.NotAChar : value;
        }

        /// <dev>
        /// Currently this method is replaced with its overload, but it is left as it may be needed in the future.
        /// It was replaced because it was called for strings with length of two characters only. This strings were
        /// built using the following pattern: firstChar.ToString() + secondChar.ToString(). It is too expensive 
        /// to create three strings per this method call. In contrast, this method's overload combines two characters
        /// to an integer value and uses it for lookup, i.e. no string is created per the overload call.
        /// </dev>
        internal static char Compose(string sequence)
        {
            char value;
            if (NeedUseFirstComposeMap(sequence))
            {
                value = gComposeMap1[GetInt32Key(sequence)];
                return IntToCharDictionary.IsNullSubstitute(value) ? BidiChars.NotAChar : value;
            }

            value = gComposeMap2[sequence];
            return StringToCharDictionary.IsNullSubstitute(value) ? BidiChars.NotAChar : value;
        }

        /// <summary>
        /// Transforms the specified string key to an integer one. Throws if the given key's length is more
        /// than two characters.
        /// </summary>
        private static int GetInt32Key(string key)
        {
            switch (key.Length)
            {
                case 1:
                {
                    return GetInt32Key(BidiChars.NotAChar, key[0]);
                }
                case 2:
                {
                    // Ensure that the first character is not "not-a-char" to avoid possible intersection
                    // with a single-char key (see case 1).
                    char firstChar = key[0];
                    Debug.Assert(firstChar != BidiChars.NotAChar);

                    return GetInt32Key(firstChar, key[1]);
                }
                default:
                {
                    throw new ArgumentOutOfRangeException("key");
                }
            }
        }

#if NET461_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static int GetInt32Key(char firstChar, char secondChar)
        {
            return (firstChar << 16 | secondChar);
        }

#if NET461_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool NeedUseFirstComposeMap(string key)
        {
            return (key.Length <= 2);
        }

        static UnicodeCharacterDataResolver()
        {
            SetDefaultCharData();
            InitBidiCharacterTypes();
            InitUnicodeGeneralCategories();
            InitUnicodeDecompositionTypes();
            InitUnicodeCanonicalClasses();

            gDecomposeMap = UnicodeCharacterDecomposeMap.Get();
            UnicodeCharacterComposeMap composeMapInitializer = new UnicodeCharacterComposeMap();
            gComposeMap1 = composeMapInitializer.GetMap1();
            gComposeMap2 = composeMapInitializer.GetMap2();
        }

        private static void SetDefaultCharData()
        {
#if !JAVA && !CPLUSPLUS
            // Ensure that all of the enum values stored in gCharData items are less or equal to the maximum
            // byte value, so they could be successfully packed.
            // Note, that we can not use a sign bit of a gCharData item, because it has a special meaning
            // in the context of bit shifting operations, so we need to ensure that the maximum value of
            // the last packed enum (i.e. UnicodeDecompositionType) is twice less.
            Debug.Assert(EnumUtilPal.GetEffectiveArrayLength(BidiCharacterType.AL.GetType()) <= MaxByte);
            Debug.Assert(EnumUtilPal.GetEffectiveArrayLength(UnicodeGeneralCategory.Cc.GetType()) <= MaxByte);
            Debug.Assert(EnumUtilPal.GetEffectiveArrayLength(UnicodeDecompositionType.Circle.GetType()) <= MaxByte / 2);
            Debug.Assert(EnumUtilPal.GetEffectiveArrayLength(UnicodeCanonicalClass.A.GetType()) <= MaxByte);
#endif

            // Combine default values for all of the enums in the first item.
            SetCharDataItem(char.MinValue, BidiCharacterTypeItemIndex, (int)BidiCharacterType.L);
            SetCharDataItem(char.MinValue, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Cn);
            SetCharDataItem(char.MinValue, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.None);
            SetCharDataItem(char.MinValue, UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.NR);

            // Make all of the items equal to the first one.
            int defaultCharData = gCharData[char.MinValue];

            for (int i = char.MinValue + 1; i <= char.MaxValue; i++)
                gCharData[i] = defaultCharData;
        }

        private static void InitBidiCharacterTypes()
        {
            // The following arrays moved to the method scope, since they are not used anywhere else,
            // so there is no reason to store them as permanent static fields.
            int[] BctList_LRE = new int[] { 0x202A, 1 };
            int[] BctList_LRO = new int[] { 0x202D, 1 };
            int[] BctList_R = new int[] { 0x05BE, 1, 0x05C0, 1, 0x05C3, 1, 0x05C6, 1, 0x05D0, 27, 0x05F0, 
                5, 0x07C0, 43, 0x07F4, 2, 0x07FA, 1, 0x200F, 1, 0xFB1D, 1, 0xFB1F, 10, 0xFB2A, 13, 0xFB38, 5, 0xFB3E, 1, 0xFB40, 
                2, 0xFB43, 2, 0xFB46, 10};
            int[] BctList_AL = new int[] { 0x0608, 1, 0x060B, 1, 0x060D, 1, 0x061B, 1, 0x061E, 2, 0x0620, 
                43, 0x066D, 3, 0x0671, 101, 0x06E5, 2, 0x06EE, 2, 0x06FA, 20, 0x0710, 1, 0x0712, 30, 0x074D, 89, 0x07B1, 1, 0xFB50, 
                98, 0xFBD3, 363, 0xFD50, 64, 0xFD92, 54, 0xFDF0, 13, 0xFE70, 5, 0xFE76, 135};
            int[] BctList_RLE = new int[] { 0x202B, 1 };
            int[] BctList_RLO = new int[] { 0x202E, 1 };
            int[] BctList_PDF = new int[] { 0x202C, 1 };
            int[] BctList_EN = new int[] { 0x0030, 10, 0x00B2, 2, 0x00B9, 1, 0x06F0, 10, 0x2070, 1, 0x2074, 
                6, 0x2080, 10, 0x2488, 20, 0xFF10, 10};
            int[] BctList_ES = new int[] { 0x002B, 1, 0x002D, 1, 0x207A, 2, 0x208A, 2, 0x2212, 1, 0xFB29, 
                1, 0xFE62, 2, 0xFF0B, 1, 0xFF0D, 1};
            int[] BctList_ET = new int[] { 0x0023, 3, 0x00A2, 4, 0x00B0, 2, 0x0609, 2, 0x066A, 1, 0x09F2, 
                2, 0x0AF1, 1, 0x0BF9, 1, 0x0E3F, 1, 0x17DB, 1, 0x2030, 5, 0x20A0, 22, 0x212E, 1, 0x2213, 1, 0xFE5F, 1, 0xFE69, 
                2, 0xFF03, 3, 0xFFE0, 2, 0xFFE5, 2};
            int[] BctList_AN = new int[] { 0x0600, 4, 0x0660, 10, 0x066B, 2, 0x06DD, 1 };

            int[] BctList_CS = new int[] { 0x002C, 1, 0x002E, 2, 0x003A, 1, 0x00A0, 1, 0x060C, 1, 0x202F, 
                1, 0x2044, 1, 0xFE50, 1, 0xFE52, 1, 0xFE55, 1, 0xFF0C, 1, 0xFF0E, 2, 0xFF1A, 1};

            int[] BctList_NSM = new int[] { 0x0300, 112, 0x0483, 7, 0x0591, 45, 0x05BF, 1, 0x05C1, 2, 0x05C4, 
                2, 0x05C7, 1, 0x0610, 11, 0x064B, 20, 0x0670, 1, 0x06D6, 7, 0x06DE, 7, 0x06E7, 2, 0x06EA, 4, 0x0711, 1, 0x0730, 
                27, 0x07A6, 11, 0x07EB, 9, 0x0901, 2, 0x093C, 1, 0x0941, 8, 0x094D, 1, 0x0951, 4, 0x0962, 2, 0x0981, 1, 0x09BC, 
                1, 0x09C1, 4, 0x09CD, 1, 0x09E2, 2, 0x0A01, 2, 0x0A3C, 1, 0x0A41, 2, 0x0A47, 2, 0x0A4B, 3, 0x0A51, 1, 0x0A70, 
                2, 0x0A75, 1, 0x0A81, 2, 0x0ABC, 1, 0x0AC1, 5, 0x0AC7, 2, 0x0ACD, 1, 0x0AE2, 2, 0x0B01, 1, 0x0B3C, 1, 0x0B3F, 
                1, 0x0B41, 4, 0x0B4D, 1, 0x0B56, 1, 0x0B62, 2, 0x0B82, 1, 0x0BC0, 1, 0x0BCD, 1, 0x0C3E, 3, 0x0C46, 3, 0x0C4A, 
                4, 0x0C55, 2, 0x0C62, 2, 0x0CBC, 1, 0x0CCC, 2, 0x0CE2, 2, 0x0D41, 4, 0x0D4D, 1, 0x0D62, 2, 0x0DCA, 1, 0x0DD2, 
                3, 0x0DD6, 1, 0x0E31, 1, 0x0E34, 7, 0x0E47, 8, 0x0EB1, 1, 0x0EB4, 6, 0x0EBB, 2, 0x0EC8, 6, 0x0F18, 2, 0x0F35, 
                1, 0x0F37, 1, 0x0F39, 1, 0x0F71, 14, 0x0F80, 5, 0x0F86, 2, 0x0F90, 8, 0x0F99, 36, 0x0FC6, 1, 0x102D, 4, 0x1032, 
                6, 0x1039, 2, 0x103D, 2, 0x1058, 2, 0x105E, 3, 0x1071, 4, 0x1082, 1, 0x1085, 2, 0x108D, 1, 0x135F, 1, 0x1712, 
                3, 0x1732, 3, 0x1752, 2, 0x1772, 2, 0x17B7, 7, 0x17C6, 1, 0x17C9, 11, 0x17DD, 1, 0x180B, 3, 0x18A9, 1, 0x1920, 
                3, 0x1927, 2, 0x1932, 1, 0x1939, 3, 0x1A17, 2, 0x1B00, 4, 0x1B34, 1, 0x1B36, 5, 0x1B3C, 1, 0x1B42, 1, 0x1B6B, 
                9, 0x1B80, 2, 0x1BA2, 4, 0x1BA8, 2, 0x1C2C, 8, 0x1C36, 2, 0x1DC0, 39, 0x1DFE, 2, 0x20D0, 33, 0x2DE0, 32, 0x302A, 
                6, 0x3099, 2, 0xA66F, 4, 0xA67C, 2, 0xA802, 1, 0xA806, 1, 0xA80B, 1, 0xA825, 2, 0xA8C4, 1, 0xA926, 8, 0xA947, 
                11, 0xAA29, 6, 0xAA31, 2, 0xAA35, 2, 0xAA43, 1, 0xAA4C, 1, 0xFB1E, 1, 0xFE00, 16, 0xFE20, 7};
            int[] BctList_BN = new int[] { 0x0000, 9, 0x000E, 14, 0x007F, 6, 0x0086, 26, 0x00AD, 1, 0x070F, 
                1, 0x200B, 3, 0x2060, 5, 0x206A, 6, 0xFEFF, 1, 0xFFFE, 2};
            int[] BctList_B = new int[] { 0x000A, 1, 0x000D, 1, 0x001C, 3, 0x0085, 1, 0x2029, 1 };
            int[] BctList_S = new int[] { 0x0009, 1, 0x000B, 1, 0x001F, 1 };
            int[] BctList_WS = new int[] { 0x000C, 1, 0x0020, 1, 0x1680, 1, 0x180E, 1, 0x2000, 11, 0x2028, 
                1, 0x205F, 1, 0x3000, 1};

            int[] BctList_ON = new int[] { 0x0021, 2, 0x0026, 5, 0x003B, 6, 0x005B, 6, 0x007B, 4, 0x00A1, 
                1, 0x00A6, 4, 0x00AB, 2, 0x00AE, 2, 0x00B4, 1, 0x00B6, 3, 0x00BB, 5, 0x00D7, 1, 0x00F7, 1, 0x02B9, 2, 0x02C2, 
                14, 0x02D2, 14, 0x02E5, 9, 0x02EF, 17, 0x0374, 2, 0x037E, 1, 0x0384, 2, 0x0387, 1, 0x03F6, 1, 0x058A, 1, 0x0606, 
                2, 0x060E, 2, 0x06E9, 1, 0x07F6, 4, 0x0BF3, 6, 0x0BFA, 1, 0x0C78, 7, 0x0CF1, 2, 0x0F3A, 4, 0x1390, 10, 0x169B, 
                2, 0x17F0, 10, 0x1800, 11, 0x1940, 1, 0x1944, 2, 0x19DE, 34, 0x1FBD, 1, 0x1FBF, 3, 0x1FCD, 3, 0x1FDD, 3, 0x1FED, 
                3, 0x1FFD, 2, 0x2010, 24, 0x2035, 15, 0x2045, 26, 0x207C, 3, 0x208C, 3, 0x2100, 2, 0x2103, 4, 0x2108, 2, 0x2114, 
                1, 0x2116, 3, 0x211E, 6, 0x2125, 1, 0x2127, 1, 0x2129, 1, 0x213A, 2, 0x2140, 5, 0x214A, 4, 0x2153, 13, 0x2190, 
                130, 0x2214, 290, 0x237B, 26, 0x2396, 82, 0x2400, 39, 0x2440, 11, 0x2460, 40, 0x24EA, 436, 0x26A0, 12, 0x26AD, 16, 0x26C0, 
                4, 0x2701, 4, 0x2706, 4, 0x270C, 28, 0x2729, 35, 0x274D, 1, 0x274F, 4, 0x2756, 1, 0x2758, 7, 0x2761, 52, 0x2798, 
                24, 0x27B1, 14, 0x27C0, 11, 0x27CC, 1, 0x27D0, 48, 0x2900, 589, 0x2B50, 5, 0x2CE5, 6, 0x2CF9, 7, 0x2E00, 49, 0x2E80, 
                26, 0x2E9B, 89, 0x2F00, 214, 0x2FF0, 12, 0x3001, 4, 0x3008, 25, 0x3030, 1, 0x3036, 2, 0x303D, 3, 0x309B, 2, 0x30A0, 
                1, 0x30FB, 1, 0x31C0, 36, 0x321D, 2, 0x3250, 16, 0x327C, 3, 0x32B1, 15, 0x32CC, 4, 0x3377, 4, 0x33DE, 2, 0x33FF, 
                1, 0x4DC0, 64, 0xA490, 55, 0xA60D, 3, 0xA673, 1, 0xA67E, 2, 0xA700, 34, 0xA788, 1, 0xA828, 4, 0xA874, 4, 0xFD3E, 
                2, 0xFDFD, 1, 0xFE10, 10, 0xFE30, 32, 0xFE51, 1, 0xFE54, 1, 0xFE56, 9, 0xFE60, 2, 0xFE64, 3, 0xFE68, 1, 0xFE6B, 
                1, 0xFF01, 2, 0xFF06, 5, 0xFF1B, 6, 0xFF3B, 6, 0xFF5B, 11, 0xFFE2, 3, 0xFFE8, 7, 0xFFF9, 5};

            for (int i = 0; i < BctList_LRE.Length; i += 2)
                for (int j = BctList_LRE[i]; j < BctList_LRE[i] + BctList_LRE[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.LRE);

            for (int i = 0; i < BctList_LRO.Length; i += 2)
                for (int j = BctList_LRO[i]; j < BctList_LRO[i] + BctList_LRO[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.LRO);

            for (int i = 0; i < BctList_R.Length; i += 2)
                for (int j = BctList_R[i]; j < BctList_R[i] + BctList_R[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.R);

            for (int i = 0; i < BctList_AL.Length; i += 2)
                for (int j = BctList_AL[i]; j < BctList_AL[i] + BctList_AL[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.AL);

            for (int i = 0; i < BctList_RLE.Length; i += 2)
                for (int j = BctList_RLE[i]; j < BctList_RLE[i] + BctList_RLE[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.RLE);

            for (int i = 0; i < BctList_RLO.Length; i += 2)
                for (int j = BctList_RLO[i]; j < BctList_RLO[i] + BctList_RLO[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.RLO);

            for (int i = 0; i < BctList_PDF.Length; i += 2)
                for (int j = BctList_PDF[i]; j < BctList_PDF[i] + BctList_PDF[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.PDF);

            for (int i = 0; i < BctList_EN.Length; i += 2)
                for (int j = BctList_EN[i]; j < BctList_EN[i] + BctList_EN[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.EN);

            for (int i = 0; i < BctList_ES.Length; i += 2)
                for (int j = BctList_ES[i]; j < BctList_ES[i] + BctList_ES[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.ES);

            for (int i = 0; i < BctList_ET.Length; i += 2)
                for (int j = BctList_ET[i]; j < BctList_ET[i] + BctList_ET[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.ET);

            for (int i = 0; i < BctList_AN.Length; i += 2)
                for (int j = BctList_AN[i]; j < BctList_AN[i] + BctList_AN[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.AN);

            for (int i = 0; i < BctList_CS.Length; i += 2)
                for (int j = BctList_CS[i]; j < BctList_CS[i] + BctList_CS[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.CS);

            for (int i = 0; i < BctList_NSM.Length; i += 2)
                for (int j = BctList_NSM[i]; j < BctList_NSM[i] + BctList_NSM[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.NSM);

            for (int i = 0; i < BctList_BN.Length; i += 2)
                for (int j = BctList_BN[i]; j < BctList_BN[i] + BctList_BN[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.BN);

            for (int i = 0; i < BctList_B.Length; i += 2)
                for (int j = BctList_B[i]; j < BctList_B[i] + BctList_B[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.B);

            for (int i = 0; i < BctList_S.Length; i += 2)
                for (int j = BctList_S[i]; j < BctList_S[i] + BctList_S[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.S);

            for (int i = 0; i < BctList_WS.Length; i += 2)
                for (int j = BctList_WS[i]; j < BctList_WS[i] + BctList_WS[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.WS);

            for (int i = 0; i < BctList_ON.Length; i += 2)
                for (int j = BctList_ON[i]; j < BctList_ON[i] + BctList_ON[i + 1]; ++j)
                    SetCharDataItem((char)j, BidiCharacterTypeItemIndex, (int)BidiCharacterType.ON);
        }

        private static void InitUnicodeGeneralCategories()
        {
            // The following arrays moved to the method scope, since they are not used anywhere else,
            // so there is no reason to store them as permanent static fields.
            int[] UgcList_Lu = new int[] { 0x0041, 26, 0x00C0, 23, 0x00D8, 7, 0x0100, 1, 0x0102, 1, 0x0104, 
                1, 0x0106, 1, 0x0108, 1, 0x010A, 1, 0x010C, 1, 0x010E, 1, 0x0110, 1, 0x0112, 1, 0x0114, 1, 0x0116, 1, 0x0118, 
                1, 0x011A, 1, 0x011C, 1, 0x011E, 1, 0x0120, 1, 0x0122, 1, 0x0124, 1, 0x0126, 1, 0x0128, 1, 0x012A, 1, 0x012C, 
                1, 0x012E, 1, 0x0130, 1, 0x0132, 1, 0x0134, 1, 0x0136, 1, 0x0139, 1, 0x013B, 1, 0x013D, 1, 0x013F, 1, 0x0141, 
                1, 0x0143, 1, 0x0145, 1, 0x0147, 1, 0x014A, 1, 0x014C, 1, 0x014E, 1, 0x0150, 1, 0x0152, 1, 0x0154, 1, 0x0156, 
                1, 0x0158, 1, 0x015A, 1, 0x015C, 1, 0x015E, 1, 0x0160, 1, 0x0162, 1, 0x0164, 1, 0x0166, 1, 0x0168, 1, 0x016A, 
                1, 0x016C, 1, 0x016E, 1, 0x0170, 1, 0x0172, 1, 0x0174, 1, 0x0176, 1, 0x0178, 2, 0x017B, 1, 0x017D, 1, 0x0181, 
                2, 0x0184, 1, 0x0186, 2, 0x0189, 3, 0x018E, 4, 0x0193, 2, 0x0196, 3, 0x019C, 2, 0x019F, 2, 0x01A2, 1, 0x01A4, 
                1, 0x01A6, 2, 0x01A9, 1, 0x01AC, 1, 0x01AE, 2, 0x01B1, 3, 0x01B5, 1, 0x01B7, 2, 0x01BC, 1, 0x01C4, 1, 0x01C7, 
                1, 0x01CA, 1, 0x01CD, 1, 0x01CF, 1, 0x01D1, 1, 0x01D3, 1, 0x01D5, 1, 0x01D7, 1, 0x01D9, 1, 0x01DB, 1, 0x01DE, 
                1, 0x01E0, 1, 0x01E2, 1, 0x01E4, 1, 0x01E6, 1, 0x01E8, 1, 0x01EA, 1, 0x01EC, 1, 0x01EE, 1, 0x01F1, 1, 0x01F4, 
                1, 0x01F6, 3, 0x01FA, 1, 0x01FC, 1, 0x01FE, 1, 0x0200, 1, 0x0202, 1, 0x0204, 1, 0x0206, 1, 0x0208, 1, 0x020A, 
                1, 0x020C, 1, 0x020E, 1, 0x0210, 1, 0x0212, 1, 0x0214, 1, 0x0216, 1, 0x0218, 1, 0x021A, 1, 0x021C, 1, 0x021E, 
                1, 0x0220, 1, 0x0222, 1, 0x0224, 1, 0x0226, 1, 0x0228, 1, 0x022A, 1, 0x022C, 1, 0x022E, 1, 0x0230, 1, 0x0232, 
                1, 0x023A, 2, 0x023D, 2, 0x0241, 1, 0x0243, 4, 0x0248, 1, 0x024A, 1, 0x024C, 1, 0x024E, 1, 0x0370, 1, 0x0372, 
                1, 0x0376, 1, 0x0386, 1, 0x0388, 3, 0x038C, 1, 0x038E, 2, 0x0391, 17, 0x03A3, 9, 0x03CF, 1, 0x03D2, 3, 0x03D8, 
                1, 0x03DA, 1, 0x03DC, 1, 0x03DE, 1, 0x03E0, 1, 0x03E2, 1, 0x03E4, 1, 0x03E6, 1, 0x03E8, 1, 0x03EA, 1, 0x03EC, 
                1, 0x03EE, 1, 0x03F4, 1, 0x03F7, 1, 0x03F9, 2, 0x03FD, 51, 0x0460, 1, 0x0462, 1, 0x0464, 1, 0x0466, 1, 0x0468, 
                1, 0x046A, 1, 0x046C, 1, 0x046E, 1, 0x0470, 1, 0x0472, 1, 0x0474, 1, 0x0476, 1, 0x0478, 1, 0x047A, 1, 0x047C, 
                1, 0x047E, 1, 0x0480, 1, 0x048A, 1, 0x048C, 1, 0x048E, 1, 0x0490, 1, 0x0492, 1, 0x0494, 1, 0x0496, 1, 0x0498, 
                1, 0x049A, 1, 0x049C, 1, 0x049E, 1, 0x04A0, 1, 0x04A2, 1, 0x04A4, 1, 0x04A6, 1, 0x04A8, 1, 0x04AA, 1, 0x04AC, 
                1, 0x04AE, 1, 0x04B0, 1, 0x04B2, 1, 0x04B4, 1, 0x04B6, 1, 0x04B8, 1, 0x04BA, 1, 0x04BC, 1, 0x04BE, 1, 0x04C0, 
                2, 0x04C3, 1, 0x04C5, 1, 0x04C7, 1, 0x04C9, 1, 0x04CB, 1, 0x04CD, 1, 0x04D0, 1, 0x04D2, 1, 0x04D4, 1, 0x04D6, 
                1, 0x04D8, 1, 0x04DA, 1, 0x04DC, 1, 0x04DE, 1, 0x04E0, 1, 0x04E2, 1, 0x04E4, 1, 0x04E6, 1, 0x04E8, 1, 0x04EA, 
                1, 0x04EC, 1, 0x04EE, 1, 0x04F0, 1, 0x04F2, 1, 0x04F4, 1, 0x04F6, 1, 0x04F8, 1, 0x04FA, 1, 0x04FC, 1, 0x04FE, 
                1, 0x0500, 1, 0x0502, 1, 0x0504, 1, 0x0506, 1, 0x0508, 1, 0x050A, 1, 0x050C, 1, 0x050E, 1, 0x0510, 1, 0x0512, 
                1, 0x0514, 1, 0x0516, 1, 0x0518, 1, 0x051A, 1, 0x051C, 1, 0x051E, 1, 0x0520, 1, 0x0522, 1, 0x0531, 38, 0x10A0, 
                38, 0x1E00, 1, 0x1E02, 1, 0x1E04, 1, 0x1E06, 1, 0x1E08, 1, 0x1E0A, 1, 0x1E0C, 1, 0x1E0E, 1, 0x1E10, 1, 0x1E12, 
                1, 0x1E14, 1, 0x1E16, 1, 0x1E18, 1, 0x1E1A, 1, 0x1E1C, 1, 0x1E1E, 1, 0x1E20, 1, 0x1E22, 1, 0x1E24, 1, 0x1E26, 
                1, 0x1E28, 1, 0x1E2A, 1, 0x1E2C, 1, 0x1E2E, 1, 0x1E30, 1, 0x1E32, 1, 0x1E34, 1, 0x1E36, 1, 0x1E38, 1, 0x1E3A, 
                1, 0x1E3C, 1, 0x1E3E, 1, 0x1E40, 1, 0x1E42, 1, 0x1E44, 1, 0x1E46, 1, 0x1E48, 1, 0x1E4A, 1, 0x1E4C, 1, 0x1E4E, 
                1, 0x1E50, 1, 0x1E52, 1, 0x1E54, 1, 0x1E56, 1, 0x1E58, 1, 0x1E5A, 1, 0x1E5C, 1, 0x1E5E, 1, 0x1E60, 1, 0x1E62, 
                1, 0x1E64, 1, 0x1E66, 1, 0x1E68, 1, 0x1E6A, 1, 0x1E6C, 1, 0x1E6E, 1, 0x1E70, 1, 0x1E72, 1, 0x1E74, 1, 0x1E76, 
                1, 0x1E78, 1, 0x1E7A, 1, 0x1E7C, 1, 0x1E7E, 1, 0x1E80, 1, 0x1E82, 1, 0x1E84, 1, 0x1E86, 1, 0x1E88, 1, 0x1E8A, 
                1, 0x1E8C, 1, 0x1E8E, 1, 0x1E90, 1, 0x1E92, 1, 0x1E94, 1, 0x1E9E, 1, 0x1EA0, 1, 0x1EA2, 1, 0x1EA4, 1, 0x1EA6, 
                1, 0x1EA8, 1, 0x1EAA, 1, 0x1EAC, 1, 0x1EAE, 1, 0x1EB0, 1, 0x1EB2, 1, 0x1EB4, 1, 0x1EB6, 1, 0x1EB8, 1, 0x1EBA, 
                1, 0x1EBC, 1, 0x1EBE, 1, 0x1EC0, 1, 0x1EC2, 1, 0x1EC4, 1, 0x1EC6, 1, 0x1EC8, 1, 0x1ECA, 1, 0x1ECC, 1, 0x1ECE, 
                1, 0x1ED0, 1, 0x1ED2, 1, 0x1ED4, 1, 0x1ED6, 1, 0x1ED8, 1, 0x1EDA, 1, 0x1EDC, 1, 0x1EDE, 1, 0x1EE0, 1, 0x1EE2, 
                1, 0x1EE4, 1, 0x1EE6, 1, 0x1EE8, 1, 0x1EEA, 1, 0x1EEC, 1, 0x1EEE, 1, 0x1EF0, 1, 0x1EF2, 1, 0x1EF4, 1, 0x1EF6, 
                1, 0x1EF8, 1, 0x1EFA, 1, 0x1EFC, 1, 0x1EFE, 1, 0x1F08, 8, 0x1F18, 6, 0x1F28, 8, 0x1F38, 8, 0x1F48, 6, 0x1F59, 
                1, 0x1F5B, 1, 0x1F5D, 1, 0x1F5F, 1, 0x1F68, 8, 0x1FB8, 4, 0x1FC8, 4, 0x1FD8, 4, 0x1FE8, 5, 0x1FF8, 4, 0x2102, 
                1, 0x2107, 1, 0x210B, 3, 0x2110, 3, 0x2115, 1, 0x2119, 5, 0x2124, 1, 0x2126, 1, 0x2128, 1, 0x212A, 4, 0x2130, 
                4, 0x213E, 2, 0x2145, 1, 0x2183, 1, 0x2C00, 47, 0x2C60, 1, 0x2C62, 3, 0x2C67, 1, 0x2C69, 1, 0x2C6B, 1, 0x2C6D, 
                3, 0x2C72, 1, 0x2C75, 1, 0x2C80, 1, 0x2C82, 1, 0x2C84, 1, 0x2C86, 1, 0x2C88, 1, 0x2C8A, 1, 0x2C8C, 1, 0x2C8E, 
                1, 0x2C90, 1, 0x2C92, 1, 0x2C94, 1, 0x2C96, 1, 0x2C98, 1, 0x2C9A, 1, 0x2C9C, 1, 0x2C9E, 1, 0x2CA0, 1, 0x2CA2, 
                1, 0x2CA4, 1, 0x2CA6, 1, 0x2CA8, 1, 0x2CAA, 1, 0x2CAC, 1, 0x2CAE, 1, 0x2CB0, 1, 0x2CB2, 1, 0x2CB4, 1, 0x2CB6, 
                1, 0x2CB8, 1, 0x2CBA, 1, 0x2CBC, 1, 0x2CBE, 1, 0x2CC0, 1, 0x2CC2, 1, 0x2CC4, 1, 0x2CC6, 1, 0x2CC8, 1, 0x2CCA, 
                1, 0x2CCC, 1, 0x2CCE, 1, 0x2CD0, 1, 0x2CD2, 1, 0x2CD4, 1, 0x2CD6, 1, 0x2CD8, 1, 0x2CDA, 1, 0x2CDC, 1, 0x2CDE, 
                1, 0x2CE0, 1, 0x2CE2, 1, 0xA640, 1, 0xA642, 1, 0xA644, 1, 0xA646, 1, 0xA648, 1, 0xA64A, 1, 0xA64C, 1, 0xA64E, 
                1, 0xA650, 1, 0xA652, 1, 0xA654, 1, 0xA656, 1, 0xA658, 1, 0xA65A, 1, 0xA65C, 1, 0xA65E, 1, 0xA662, 1, 0xA664, 
                1, 0xA666, 1, 0xA668, 1, 0xA66A, 1, 0xA66C, 1, 0xA680, 1, 0xA682, 1, 0xA684, 1, 0xA686, 1, 0xA688, 1, 0xA68A, 
                1, 0xA68C, 1, 0xA68E, 1, 0xA690, 1, 0xA692, 1, 0xA694, 1, 0xA696, 1, 0xA722, 1, 0xA724, 1, 0xA726, 1, 0xA728, 
                1, 0xA72A, 1, 0xA72C, 1, 0xA72E, 1, 0xA732, 1, 0xA734, 1, 0xA736, 1, 0xA738, 1, 0xA73A, 1, 0xA73C, 1, 0xA73E, 
                1, 0xA740, 1, 0xA742, 1, 0xA744, 1, 0xA746, 1, 0xA748, 1, 0xA74A, 1, 0xA74C, 1, 0xA74E, 1, 0xA750, 1, 0xA752, 
                1, 0xA754, 1, 0xA756, 1, 0xA758, 1, 0xA75A, 1, 0xA75C, 1, 0xA75E, 1, 0xA760, 1, 0xA762, 1, 0xA764, 1, 0xA766, 
                1, 0xA768, 1, 0xA76A, 1, 0xA76C, 1, 0xA76E, 1, 0xA779, 1, 0xA77B, 1, 0xA77D, 2, 0xA780, 1, 0xA782, 1, 0xA784, 
                1, 0xA786, 1, 0xA78B, 1, 0xFF21, 26};
            int[] UgcList_Ll = new int[] { 0x0061, 26, 0x00AA, 1, 0x00B5, 1, 0x00BA, 1, 0x00DF, 24, 0x00F8, 
                8, 0x0101, 1, 0x0103, 1, 0x0105, 1, 0x0107, 1, 0x0109, 1, 0x010B, 1, 0x010D, 1, 0x010F, 1, 0x0111, 1, 0x0113, 
                1, 0x0115, 1, 0x0117, 1, 0x0119, 1, 0x011B, 1, 0x011D, 1, 0x011F, 1, 0x0121, 1, 0x0123, 1, 0x0125, 1, 0x0127, 
                1, 0x0129, 1, 0x012B, 1, 0x012D, 1, 0x012F, 1, 0x0131, 1, 0x0133, 1, 0x0135, 1, 0x0137, 2, 0x013A, 1, 0x013C, 
                1, 0x013E, 1, 0x0140, 1, 0x0142, 1, 0x0144, 1, 0x0146, 1, 0x0148, 2, 0x014B, 1, 0x014D, 1, 0x014F, 1, 0x0151, 
                1, 0x0153, 1, 0x0155, 1, 0x0157, 1, 0x0159, 1, 0x015B, 1, 0x015D, 1, 0x015F, 1, 0x0161, 1, 0x0163, 1, 0x0165, 
                1, 0x0167, 1, 0x0169, 1, 0x016B, 1, 0x016D, 1, 0x016F, 1, 0x0171, 1, 0x0173, 1, 0x0175, 1, 0x0177, 1, 0x017A, 
                1, 0x017C, 1, 0x017E, 3, 0x0183, 1, 0x0185, 1, 0x0188, 1, 0x018C, 2, 0x0192, 1, 0x0195, 1, 0x0199, 3, 0x019E, 
                1, 0x01A1, 1, 0x01A3, 1, 0x01A5, 1, 0x01A8, 1, 0x01AA, 2, 0x01AD, 1, 0x01B0, 1, 0x01B4, 1, 0x01B6, 1, 0x01B9, 
                2, 0x01BD, 3, 0x01C6, 1, 0x01C9, 1, 0x01CC, 1, 0x01CE, 1, 0x01D0, 1, 0x01D2, 1, 0x01D4, 1, 0x01D6, 1, 0x01D8, 
                1, 0x01DA, 1, 0x01DC, 2, 0x01DF, 1, 0x01E1, 1, 0x01E3, 1, 0x01E5, 1, 0x01E7, 1, 0x01E9, 1, 0x01EB, 1, 0x01ED, 
                1, 0x01EF, 2, 0x01F3, 1, 0x01F5, 1, 0x01F9, 1, 0x01FB, 1, 0x01FD, 1, 0x01FF, 1, 0x0201, 1, 0x0203, 1, 0x0205, 
                1, 0x0207, 1, 0x0209, 1, 0x020B, 1, 0x020D, 1, 0x020F, 1, 0x0211, 1, 0x0213, 1, 0x0215, 1, 0x0217, 1, 0x0219, 
                1, 0x021B, 1, 0x021D, 1, 0x021F, 1, 0x0221, 1, 0x0223, 1, 0x0225, 1, 0x0227, 1, 0x0229, 1, 0x022B, 1, 0x022D, 
                1, 0x022F, 1, 0x0231, 1, 0x0233, 7, 0x023C, 1, 0x023F, 2, 0x0242, 1, 0x0247, 1, 0x0249, 1, 0x024B, 1, 0x024D, 
                1, 0x024F, 69, 0x0295, 27, 0x0371, 1, 0x0373, 1, 0x0377, 1, 0x037B, 3, 0x0390, 1, 0x03AC, 35, 0x03D0, 2, 0x03D5, 
                3, 0x03D9, 1, 0x03DB, 1, 0x03DD, 1, 0x03DF, 1, 0x03E1, 1, 0x03E3, 1, 0x03E5, 1, 0x03E7, 1, 0x03E9, 1, 0x03EB, 
                1, 0x03ED, 1, 0x03EF, 5, 0x03F5, 1, 0x03F8, 1, 0x03FB, 2, 0x0430, 48, 0x0461, 1, 0x0463, 1, 0x0465, 1, 0x0467, 
                1, 0x0469, 1, 0x046B, 1, 0x046D, 1, 0x046F, 1, 0x0471, 1, 0x0473, 1, 0x0475, 1, 0x0477, 1, 0x0479, 1, 0x047B, 
                1, 0x047D, 1, 0x047F, 1, 0x0481, 1, 0x048B, 1, 0x048D, 1, 0x048F, 1, 0x0491, 1, 0x0493, 1, 0x0495, 1, 0x0497, 
                1, 0x0499, 1, 0x049B, 1, 0x049D, 1, 0x049F, 1, 0x04A1, 1, 0x04A3, 1, 0x04A5, 1, 0x04A7, 1, 0x04A9, 1, 0x04AB, 
                1, 0x04AD, 1, 0x04AF, 1, 0x04B1, 1, 0x04B3, 1, 0x04B5, 1, 0x04B7, 1, 0x04B9, 1, 0x04BB, 1, 0x04BD, 1, 0x04BF, 
                1, 0x04C2, 1, 0x04C4, 1, 0x04C6, 1, 0x04C8, 1, 0x04CA, 1, 0x04CC, 1, 0x04CE, 2, 0x04D1, 1, 0x04D3, 1, 0x04D5, 
                1, 0x04D7, 1, 0x04D9, 1, 0x04DB, 1, 0x04DD, 1, 0x04DF, 1, 0x04E1, 1, 0x04E3, 1, 0x04E5, 1, 0x04E7, 1, 0x04E9, 
                1, 0x04EB, 1, 0x04ED, 1, 0x04EF, 1, 0x04F1, 1, 0x04F3, 1, 0x04F5, 1, 0x04F7, 1, 0x04F9, 1, 0x04FB, 1, 0x04FD, 
                1, 0x04FF, 1, 0x0501, 1, 0x0503, 1, 0x0505, 1, 0x0507, 1, 0x0509, 1, 0x050B, 1, 0x050D, 1, 0x050F, 1, 0x0511, 
                1, 0x0513, 1, 0x0515, 1, 0x0517, 1, 0x0519, 1, 0x051B, 1, 0x051D, 1, 0x051F, 1, 0x0521, 1, 0x0523, 1, 0x0561, 
                39, 0x1D00, 44, 0x1D62, 22, 0x1D79, 34, 0x1E01, 1, 0x1E03, 1, 0x1E05, 1, 0x1E07, 1, 0x1E09, 1, 0x1E0B, 1, 0x1E0D, 
                1, 0x1E0F, 1, 0x1E11, 1, 0x1E13, 1, 0x1E15, 1, 0x1E17, 1, 0x1E19, 1, 0x1E1B, 1, 0x1E1D, 1, 0x1E1F, 1, 0x1E21, 
                1, 0x1E23, 1, 0x1E25, 1, 0x1E27, 1, 0x1E29, 1, 0x1E2B, 1, 0x1E2D, 1, 0x1E2F, 1, 0x1E31, 1, 0x1E33, 1, 0x1E35, 
                1, 0x1E37, 1, 0x1E39, 1, 0x1E3B, 1, 0x1E3D, 1, 0x1E3F, 1, 0x1E41, 1, 0x1E43, 1, 0x1E45, 1, 0x1E47, 1, 0x1E49, 
                1, 0x1E4B, 1, 0x1E4D, 1, 0x1E4F, 1, 0x1E51, 1, 0x1E53, 1, 0x1E55, 1, 0x1E57, 1, 0x1E59, 1, 0x1E5B, 1, 0x1E5D, 
                1, 0x1E5F, 1, 0x1E61, 1, 0x1E63, 1, 0x1E65, 1, 0x1E67, 1, 0x1E69, 1, 0x1E6B, 1, 0x1E6D, 1, 0x1E6F, 1, 0x1E71, 
                1, 0x1E73, 1, 0x1E75, 1, 0x1E77, 1, 0x1E79, 1, 0x1E7B, 1, 0x1E7D, 1, 0x1E7F, 1, 0x1E81, 1, 0x1E83, 1, 0x1E85, 
                1, 0x1E87, 1, 0x1E89, 1, 0x1E8B, 1, 0x1E8D, 1, 0x1E8F, 1, 0x1E91, 1, 0x1E93, 1, 0x1E95, 9, 0x1E9F, 1, 0x1EA1, 
                1, 0x1EA3, 1, 0x1EA5, 1, 0x1EA7, 1, 0x1EA9, 1, 0x1EAB, 1, 0x1EAD, 1, 0x1EAF, 1, 0x1EB1, 1, 0x1EB3, 1, 0x1EB5, 
                1, 0x1EB7, 1, 0x1EB9, 1, 0x1EBB, 1, 0x1EBD, 1, 0x1EBF, 1, 0x1EC1, 1, 0x1EC3, 1, 0x1EC5, 1, 0x1EC7, 1, 0x1EC9, 
                1, 0x1ECB, 1, 0x1ECD, 1, 0x1ECF, 1, 0x1ED1, 1, 0x1ED3, 1, 0x1ED5, 1, 0x1ED7, 1, 0x1ED9, 1, 0x1EDB, 1, 0x1EDD, 
                1, 0x1EDF, 1, 0x1EE1, 1, 0x1EE3, 1, 0x1EE5, 1, 0x1EE7, 1, 0x1EE9, 1, 0x1EEB, 1, 0x1EED, 1, 0x1EEF, 1, 0x1EF1, 
                1, 0x1EF3, 1, 0x1EF5, 1, 0x1EF7, 1, 0x1EF9, 1, 0x1EFB, 1, 0x1EFD, 1, 0x1EFF, 9, 0x1F10, 6, 0x1F20, 8, 0x1F30, 
                8, 0x1F40, 6, 0x1F50, 8, 0x1F60, 8, 0x1F70, 14, 0x1F80, 8, 0x1F90, 8, 0x1FA0, 8, 0x1FB0, 5, 0x1FB6, 2, 0x1FBE, 
                1, 0x1FC2, 3, 0x1FC6, 2, 0x1FD0, 4, 0x1FD6, 2, 0x1FE0, 8, 0x1FF2, 3, 0x1FF6, 2, 0x2071, 1, 0x207F, 1, 0x210A, 
                1, 0x210E, 2, 0x2113, 1, 0x212F, 1, 0x2134, 1, 0x2139, 1, 0x213C, 2, 0x2146, 4, 0x214E, 1, 0x2184, 1, 0x2C30, 
                47, 0x2C61, 1, 0x2C65, 2, 0x2C68, 1, 0x2C6A, 1, 0x2C6C, 1, 0x2C71, 1, 0x2C73, 2, 0x2C76, 7, 0x2C81, 1, 0x2C83, 
                1, 0x2C85, 1, 0x2C87, 1, 0x2C89, 1, 0x2C8B, 1, 0x2C8D, 1, 0x2C8F, 1, 0x2C91, 1, 0x2C93, 1, 0x2C95, 1, 0x2C97, 
                1, 0x2C99, 1, 0x2C9B, 1, 0x2C9D, 1, 0x2C9F, 1, 0x2CA1, 1, 0x2CA3, 1, 0x2CA5, 1, 0x2CA7, 1, 0x2CA9, 1, 0x2CAB, 
                1, 0x2CAD, 1, 0x2CAF, 1, 0x2CB1, 1, 0x2CB3, 1, 0x2CB5, 1, 0x2CB7, 1, 0x2CB9, 1, 0x2CBB, 1, 0x2CBD, 1, 0x2CBF, 
                1, 0x2CC1, 1, 0x2CC3, 1, 0x2CC5, 1, 0x2CC7, 1, 0x2CC9, 1, 0x2CCB, 1, 0x2CCD, 1, 0x2CCF, 1, 0x2CD1, 1, 0x2CD3, 
                1, 0x2CD5, 1, 0x2CD7, 1, 0x2CD9, 1, 0x2CDB, 1, 0x2CDD, 1, 0x2CDF, 1, 0x2CE1, 1, 0x2CE3, 2, 0x2D00, 38, 0xA641, 
                1, 0xA643, 1, 0xA645, 1, 0xA647, 1, 0xA649, 1, 0xA64B, 1, 0xA64D, 1, 0xA64F, 1, 0xA651, 1, 0xA653, 1, 0xA655, 
                1, 0xA657, 1, 0xA659, 1, 0xA65B, 1, 0xA65D, 1, 0xA65F, 1, 0xA663, 1, 0xA665, 1, 0xA667, 1, 0xA669, 1, 0xA66B, 
                1, 0xA66D, 1, 0xA681, 1, 0xA683, 1, 0xA685, 1, 0xA687, 1, 0xA689, 1, 0xA68B, 1, 0xA68D, 1, 0xA68F, 1, 0xA691, 
                1, 0xA693, 1, 0xA695, 1, 0xA697, 1, 0xA723, 1, 0xA725, 1, 0xA727, 1, 0xA729, 1, 0xA72B, 1, 0xA72D, 1, 0xA72F, 
                3, 0xA733, 1, 0xA735, 1, 0xA737, 1, 0xA739, 1, 0xA73B, 1, 0xA73D, 1, 0xA73F, 1, 0xA741, 1, 0xA743, 1, 0xA745, 
                1, 0xA747, 1, 0xA749, 1, 0xA74B, 1, 0xA74D, 1, 0xA74F, 1, 0xA751, 1, 0xA753, 1, 0xA755, 1, 0xA757, 1, 0xA759, 
                1, 0xA75B, 1, 0xA75D, 1, 0xA75F, 1, 0xA761, 1, 0xA763, 1, 0xA765, 1, 0xA767, 1, 0xA769, 1, 0xA76B, 1, 0xA76D, 
                1, 0xA76F, 1, 0xA771, 8, 0xA77A, 1, 0xA77C, 1, 0xA77F, 1, 0xA781, 1, 0xA783, 1, 0xA785, 1, 0xA787, 1, 0xA78C, 
                1, 0xFB00, 7, 0xFB13, 5, 0xFF41, 26};
            int[] UgcList_Lt = new int[] { 0x01C5, 1, 0x01C8, 1, 0x01CB, 1, 0x01F2, 1, 0x1F88, 8, 0x1F98, 
                8, 0x1FA8, 8, 0x1FBC, 1, 0x1FCC, 1, 0x1FFC, 1};
            int[] UgcList_Lm = new int[] { 0x02B0, 18, 0x02C6, 12, 0x02E0, 5, 0x02EC, 1, 0x02EE, 1, 0x0374, 
                1, 0x037A, 1, 0x0559, 1, 0x0640, 1, 0x06E5, 2, 0x07F4, 2, 0x07FA, 1, 0x0971, 1, 0x0E46, 1, 0x0EC6, 1, 0x10FC, 
                1, 0x17D7, 1, 0x1843, 1, 0x1C78, 6, 0x1D2C, 54, 0x1D78, 1, 0x1D9B, 37, 0x2090, 5, 0x2C7D, 1, 0x2D6F, 1, 0x2E2F, 
                1, 0x3005, 1, 0x3031, 5, 0x303B, 1, 0x309D, 2, 0x30FC, 3, 0xA015, 1, 0xA60C, 1, 0xA67F, 1, 0xA717, 9, 0xA770, 
                1, 0xA788, 1, 0xFF70, 1, 0xFF9E, 2};
            int[] UgcList_Lo = new int[] { 0x01BB, 1, 0x01C0, 4, 0x0294, 1, 0x05D0, 27, 0x05F0, 3, 0x0621, 
                31, 0x0641, 10, 0x066E, 2, 0x0671, 99, 0x06D5, 1, 0x06EE, 2, 0x06FA, 3, 0x06FF, 1, 0x0710, 1, 0x0712, 30, 0x074D, 
                89, 0x07B1, 1, 0x07CA, 33, 0x0904, 54, 0x093D, 1, 0x0950, 1, 0x0958, 10, 0x0972, 1, 0x097B, 5, 0x0985, 8, 0x098F, 
                2, 0x0993, 22, 0x09AA, 7, 0x09B2, 1, 0x09B6, 4, 0x09BD, 1, 0x09CE, 1, 0x09DC, 2, 0x09DF, 3, 0x09F0, 2, 0x0A05, 
                6, 0x0A0F, 2, 0x0A13, 22, 0x0A2A, 7, 0x0A32, 2, 0x0A35, 2, 0x0A38, 2, 0x0A59, 4, 0x0A5E, 1, 0x0A72, 3, 0x0A85, 
                9, 0x0A8F, 3, 0x0A93, 22, 0x0AAA, 7, 0x0AB2, 2, 0x0AB5, 5, 0x0ABD, 1, 0x0AD0, 1, 0x0AE0, 2, 0x0B05, 8, 0x0B0F, 
                2, 0x0B13, 22, 0x0B2A, 7, 0x0B32, 2, 0x0B35, 5, 0x0B3D, 1, 0x0B5C, 2, 0x0B5F, 3, 0x0B71, 1, 0x0B83, 1, 0x0B85, 
                6, 0x0B8E, 3, 0x0B92, 4, 0x0B99, 2, 0x0B9C, 1, 0x0B9E, 2, 0x0BA3, 2, 0x0BA8, 3, 0x0BAE, 12, 0x0BD0, 1, 0x0C05, 
                8, 0x0C0E, 3, 0x0C12, 23, 0x0C2A, 10, 0x0C35, 5, 0x0C3D, 1, 0x0C58, 2, 0x0C60, 2, 0x0C85, 8, 0x0C8E, 3, 0x0C92, 
                23, 0x0CAA, 10, 0x0CB5, 5, 0x0CBD, 1, 0x0CDE, 1, 0x0CE0, 2, 0x0D05, 8, 0x0D0E, 3, 0x0D12, 23, 0x0D2A, 16, 0x0D3D, 
                1, 0x0D60, 2, 0x0D7A, 6, 0x0D85, 18, 0x0D9A, 24, 0x0DB3, 9, 0x0DBD, 1, 0x0DC0, 7, 0x0E01, 48, 0x0E32, 2, 0x0E40, 
                6, 0x0E81, 2, 0x0E84, 1, 0x0E87, 2, 0x0E8A, 1, 0x0E8D, 1, 0x0E94, 4, 0x0E99, 7, 0x0EA1, 3, 0x0EA5, 1, 0x0EA7, 
                1, 0x0EAA, 2, 0x0EAD, 4, 0x0EB2, 2, 0x0EBD, 1, 0x0EC0, 5, 0x0EDC, 2, 0x0F00, 1, 0x0F40, 8, 0x0F49, 36, 0x0F88, 
                4, 0x1000, 43, 0x103F, 1, 0x1050, 6, 0x105A, 4, 0x1061, 1, 0x1065, 2, 0x106E, 3, 0x1075, 13, 0x108E, 1, 0x10D0, 
                43, 0x1100, 90, 0x115F, 68, 0x11A8, 82, 0x1200, 73, 0x124A, 4, 0x1250, 7, 0x1258, 1, 0x125A, 4, 0x1260, 41, 0x128A, 
                4, 0x1290, 33, 0x12B2, 4, 0x12B8, 7, 0x12C0, 1, 0x12C2, 4, 0x12C8, 15, 0x12D8, 57, 0x1312, 4, 0x1318, 67, 0x1380, 
                16, 0x13A0, 85, 0x1401, 620, 0x166F, 8, 0x1681, 26, 0x16A0, 75, 0x1700, 13, 0x170E, 4, 0x1720, 18, 0x1740, 18, 0x1760, 
                13, 0x176E, 3, 0x1780, 52, 0x17DC, 1, 0x1820, 35, 0x1844, 52, 0x1880, 41, 0x18AA, 1, 0x1900, 29, 0x1950, 30, 0x1970, 
                5, 0x1980, 42, 0x19C1, 7, 0x1A00, 23, 0x1B05, 47, 0x1B45, 7, 0x1B83, 30, 0x1BAE, 2, 0x1C00, 36, 0x1C4D, 3, 0x1C5A, 
                30, 0x2135, 4, 0x2D30, 54, 0x2D80, 23, 0x2DA0, 7, 0x2DA8, 7, 0x2DB0, 7, 0x2DB8, 7, 0x2DC0, 7, 0x2DC8, 7, 0x2DD0, 
                7, 0x2DD8, 7, 0x3006, 1, 0x303C, 1, 0x3041, 86, 0x309F, 1, 0x30A1, 90, 0x30FF, 1, 0x3105, 41, 0x3131, 94, 0x31A0, 
                24, 0x31F0, 16, 0x3400, 1, 0x4DB5, 1, 0x4E00, 1, 0x9FC3, 1, 0xA000, 21, 0xA016, 1143, 0xA500, 268, 0xA610, 16, 0xA62A, 
                2, 0xA66E, 1, 0xA7FB, 7, 0xA803, 3, 0xA807, 4, 0xA80C, 23, 0xA840, 52, 0xA882, 50, 0xA90A, 28, 0xA930, 23, 0xAA00, 
                41, 0xAA40, 3, 0xAA44, 8, 0xAC00, 1, 0xD7A3, 1, 0xF900, 302, 0xFA30, 59, 0xFA70, 106, 0xFB1D, 1, 0xFB1F, 10, 0xFB2A, 
                13, 0xFB38, 5, 0xFB3E, 1, 0xFB40, 2, 0xFB43, 2, 0xFB46, 108, 0xFBD3, 363, 0xFD50, 64, 0xFD92, 54, 0xFDF0, 12, 0xFE70, 
                5, 0xFE76, 135, 0xFF66, 10, 0xFF71, 45, 0xFFA0, 31, 0xFFC2, 6, 0xFFCA, 6, 0xFFD2, 6, 0xFFDA, 3};
            int[] UgcList_Mn = new int[] { 0x0300, 112, 0x0483, 5, 0x0591, 45, 0x05BF, 1, 0x05C1, 2, 0x05C4, 
                2, 0x05C7, 1, 0x0610, 11, 0x064B, 20, 0x0670, 1, 0x06D6, 7, 0x06DF, 6, 0x06E7, 2, 0x06EA, 4, 0x0711, 1, 0x0730, 
                27, 0x07A6, 11, 0x07EB, 9, 0x0901, 2, 0x093C, 1, 0x0941, 8, 0x094D, 1, 0x0951, 4, 0x0962, 2, 0x0981, 1, 0x09BC, 
                1, 0x09C1, 4, 0x09CD, 1, 0x09E2, 2, 0x0A01, 2, 0x0A3C, 1, 0x0A41, 2, 0x0A47, 2, 0x0A4B, 3, 0x0A51, 1, 0x0A70, 
                2, 0x0A75, 1, 0x0A81, 2, 0x0ABC, 1, 0x0AC1, 5, 0x0AC7, 2, 0x0ACD, 1, 0x0AE2, 2, 0x0B01, 1, 0x0B3C, 1, 0x0B3F, 
                1, 0x0B41, 4, 0x0B4D, 1, 0x0B56, 1, 0x0B62, 2, 0x0B82, 1, 0x0BC0, 1, 0x0BCD, 1, 0x0C3E, 3, 0x0C46, 3, 0x0C4A, 
                4, 0x0C55, 2, 0x0C62, 2, 0x0CBC, 1, 0x0CBF, 1, 0x0CC6, 1, 0x0CCC, 2, 0x0CE2, 2, 0x0D41, 4, 0x0D4D, 1, 0x0D62, 
                2, 0x0DCA, 1, 0x0DD2, 3, 0x0DD6, 1, 0x0E31, 1, 0x0E34, 7, 0x0E47, 8, 0x0EB1, 1, 0x0EB4, 6, 0x0EBB, 2, 0x0EC8, 
                6, 0x0F18, 2, 0x0F35, 1, 0x0F37, 1, 0x0F39, 1, 0x0F71, 14, 0x0F80, 5, 0x0F86, 2, 0x0F90, 8, 0x0F99, 36, 0x0FC6, 
                1, 0x102D, 4, 0x1032, 6, 0x1039, 2, 0x103D, 2, 0x1058, 2, 0x105E, 3, 0x1071, 4, 0x1082, 1, 0x1085, 2, 0x108D, 
                1, 0x135F, 1, 0x1712, 3, 0x1732, 3, 0x1752, 2, 0x1772, 2, 0x17B7, 7, 0x17C6, 1, 0x17C9, 11, 0x17DD, 1, 0x180B, 
                3, 0x18A9, 1, 0x1920, 3, 0x1927, 2, 0x1932, 1, 0x1939, 3, 0x1A17, 2, 0x1B00, 4, 0x1B34, 1, 0x1B36, 5, 0x1B3C, 
                1, 0x1B42, 1, 0x1B6B, 9, 0x1B80, 2, 0x1BA2, 4, 0x1BA8, 2, 0x1C2C, 8, 0x1C36, 2, 0x1DC0, 39, 0x1DFE, 2, 0x20D0, 
                13, 0x20E1, 1, 0x20E5, 12, 0x2DE0, 32, 0x302A, 6, 0x3099, 2, 0xA66F, 1, 0xA67C, 2, 0xA802, 1, 0xA806, 1, 0xA80B, 
                1, 0xA825, 2, 0xA8C4, 1, 0xA926, 8, 0xA947, 11, 0xAA29, 6, 0xAA31, 2, 0xAA35, 2, 0xAA43, 1, 0xAA4C, 1, 0xFB1E, 
                1, 0xFE00, 16, 0xFE20, 7};
            int[] UgcList_Mc = new int[] { 0x0903, 1, 0x093E, 3, 0x0949, 4, 0x0982, 2, 0x09BE, 3, 0x09C7, 
                2, 0x09CB, 2, 0x09D7, 1, 0x0A03, 1, 0x0A3E, 3, 0x0A83, 1, 0x0ABE, 3, 0x0AC9, 1, 0x0ACB, 2, 0x0B02, 2, 0x0B3E, 
                1, 0x0B40, 1, 0x0B47, 2, 0x0B4B, 2, 0x0B57, 1, 0x0BBE, 2, 0x0BC1, 2, 0x0BC6, 3, 0x0BCA, 3, 0x0BD7, 1, 0x0C01, 
                3, 0x0C41, 4, 0x0C82, 2, 0x0CBE, 1, 0x0CC0, 5, 0x0CC7, 2, 0x0CCA, 2, 0x0CD5, 2, 0x0D02, 2, 0x0D3E, 3, 0x0D46, 
                3, 0x0D4A, 3, 0x0D57, 1, 0x0D82, 2, 0x0DCF, 3, 0x0DD8, 8, 0x0DF2, 2, 0x0F3E, 2, 0x0F7F, 1, 0x102B, 2, 0x1031, 
                1, 0x1038, 1, 0x103B, 2, 0x1056, 2, 0x1062, 3, 0x1067, 7, 0x1083, 2, 0x1087, 6, 0x108F, 1, 0x17B6, 1, 0x17BE, 
                8, 0x17C7, 2, 0x1923, 4, 0x1929, 3, 0x1930, 2, 0x1933, 6, 0x19B0, 17, 0x19C8, 2, 0x1A19, 3, 0x1B04, 1, 0x1B35, 
                1, 0x1B3B, 1, 0x1B3D, 5, 0x1B43, 2, 0x1B82, 1, 0x1BA1, 1, 0x1BA6, 2, 0x1BAA, 1, 0x1C24, 8, 0x1C34, 2, 0xA823, 
                2, 0xA827, 1, 0xA880, 2, 0xA8B4, 16, 0xA952, 2, 0xAA2F, 2, 0xAA33, 2, 0xAA4D, 1};
            int[] UgcList_Me = new int[] { 0x0488, 2, 0x06DE, 1, 0x20DD, 4, 0x20E2, 3, 0xA670, 3 };
            int[] UgcList_Nd = new int[] { 0x0030, 10, 0x0660, 10, 0x06F0, 10, 0x07C0, 10, 0x0966, 10, 0x09E6, 
                10, 0x0A66, 10, 0x0AE6, 10, 0x0B66, 10, 0x0BE6, 10, 0x0C66, 10, 0x0CE6, 10, 0x0D66, 10, 0x0E50, 10, 0x0ED0, 10, 0x0F20, 
                10, 0x1040, 10, 0x1090, 10, 0x17E0, 10, 0x1810, 10, 0x1946, 10, 0x19D0, 10, 0x1B50, 10, 0x1BB0, 10, 0x1C40, 10, 0x1C50, 
                10, 0xA620, 10, 0xA8D0, 10, 0xA900, 10, 0xAA50, 10, 0xFF10, 10};
            int[] UgcList_Nl = new int[] { 0x16EE, 3, 0x2160, 35, 0x2185, 4, 0x3007, 1, 0x3021, 9, 0x3038, 
                3};
            int[] UgcList_No = new int[] { 0x00B2, 2, 0x00B9, 1, 0x00BC, 3, 0x09F4, 6, 0x0BF0, 3, 0x0C78, 
                7, 0x0D70, 6, 0x0F2A, 10, 0x1369, 20, 0x17F0, 10, 0x2070, 1, 0x2074, 6, 0x2080, 10, 0x2153, 13, 0x2460, 60, 0x24EA, 
                22, 0x2776, 30, 0x2CFD, 1, 0x3192, 4, 0x3220, 10, 0x3251, 15, 0x3280, 10, 0x32B1, 15};
            int[] UgcList_Pc = new int[] { 0x005F, 1, 0x203F, 2, 0x2054, 1, 0xFE33, 2, 0xFE4D, 3, 0xFF3F, 
                1};
            int[] UgcList_Pd = new int[] { 0x002D, 1, 0x058A, 1, 0x05BE, 1, 0x1806, 1, 0x2010, 6, 0x2E17, 
                1, 0x2E1A, 1, 0x301C, 1, 0x3030, 1, 0x30A0, 1, 0xFE31, 2, 0xFE58, 1, 0xFE63, 1, 0xFF0D, 1};
            int[] UgcList_Ps = new int[] { 0x0028, 1, 0x005B, 1, 0x007B, 1, 0x0F3A, 1, 0x0F3C, 1, 0x169B, 
                1, 0x201A, 1, 0x201E, 1, 0x2045, 1, 0x207D, 1, 0x208D, 1, 0x2329, 1, 0x2768, 1, 0x276A, 1, 0x276C, 1, 0x276E, 
                1, 0x2770, 1, 0x2772, 1, 0x2774, 1, 0x27C5, 1, 0x27E6, 1, 0x27E8, 1, 0x27EA, 1, 0x27EC, 1, 0x27EE, 1, 0x2983, 
                1, 0x2985, 1, 0x2987, 1, 0x2989, 1, 0x298B, 1, 0x298D, 1, 0x298F, 1, 0x2991, 1, 0x2993, 1, 0x2995, 1, 0x2997, 
                1, 0x29D8, 1, 0x29DA, 1, 0x29FC, 1, 0x2E22, 1, 0x2E24, 1, 0x2E26, 1, 0x2E28, 1, 0x3008, 1, 0x300A, 1, 0x300C, 
                1, 0x300E, 1, 0x3010, 1, 0x3014, 1, 0x3016, 1, 0x3018, 1, 0x301A, 1, 0x301D, 1, 0xFD3E, 1, 0xFE17, 1, 0xFE35, 
                1, 0xFE37, 1, 0xFE39, 1, 0xFE3B, 1, 0xFE3D, 1, 0xFE3F, 1, 0xFE41, 1, 0xFE43, 1, 0xFE47, 1, 0xFE59, 1, 0xFE5B, 
                1, 0xFE5D, 1, 0xFF08, 1, 0xFF3B, 1, 0xFF5B, 1, 0xFF5F, 1, 0xFF62, 1};
            int[] UgcList_Pe = new int[] { 0x0029, 1, 0x005D, 1, 0x007D, 1, 0x0F3B, 1, 0x0F3D, 1, 0x169C, 
                1, 0x2046, 1, 0x207E, 1, 0x208E, 1, 0x232A, 1, 0x2769, 1, 0x276B, 1, 0x276D, 1, 0x276F, 1, 0x2771, 1, 0x2773, 
                1, 0x2775, 1, 0x27C6, 1, 0x27E7, 1, 0x27E9, 1, 0x27EB, 1, 0x27ED, 1, 0x27EF, 1, 0x2984, 1, 0x2986, 1, 0x2988, 
                1, 0x298A, 1, 0x298C, 1, 0x298E, 1, 0x2990, 1, 0x2992, 1, 0x2994, 1, 0x2996, 1, 0x2998, 1, 0x29D9, 1, 0x29DB, 
                1, 0x29FD, 1, 0x2E23, 1, 0x2E25, 1, 0x2E27, 1, 0x2E29, 1, 0x3009, 1, 0x300B, 1, 0x300D, 1, 0x300F, 1, 0x3011, 
                1, 0x3015, 1, 0x3017, 1, 0x3019, 1, 0x301B, 1, 0x301E, 2, 0xFD3F, 1, 0xFE18, 1, 0xFE36, 1, 0xFE38, 1, 0xFE3A, 
                1, 0xFE3C, 1, 0xFE3E, 1, 0xFE40, 1, 0xFE42, 1, 0xFE44, 1, 0xFE48, 1, 0xFE5A, 1, 0xFE5C, 1, 0xFE5E, 1, 0xFF09, 
                1, 0xFF3D, 1, 0xFF5D, 1, 0xFF60, 1, 0xFF63, 1};
            int[] UgcList_Pi = new int[] { 0x00AB, 1, 0x2018, 1, 0x201B, 2, 0x201F, 1, 0x2039, 1, 0x2E02, 
                1, 0x2E04, 1, 0x2E09, 1, 0x2E0C, 1, 0x2E1C, 1, 0x2E20, 1};
            int[] UgcList_Pf = new int[] { 0x00BB, 1, 0x2019, 1, 0x201D, 1, 0x203A, 1, 0x2E03, 1, 0x2E05, 
                1, 0x2E0A, 1, 0x2E0D, 1, 0x2E1D, 1, 0x2E21, 1};
            int[] UgcList_Po = new int[] { 0x0021, 3, 0x0025, 3, 0x002A, 1, 0x002C, 1, 0x002E, 2, 0x003A, 
                2, 0x003F, 2, 0x005C, 1, 0x00A1, 1, 0x00B7, 1, 0x00BF, 1, 0x037E, 1, 0x0387, 1, 0x055A, 6, 0x0589, 1, 0x05C0, 
                1, 0x05C3, 1, 0x05C6, 1, 0x05F3, 2, 0x0609, 2, 0x060C, 2, 0x061B, 1, 0x061E, 2, 0x066A, 4, 0x06D4, 1, 0x0700, 
                14, 0x07F7, 3, 0x0964, 2, 0x0970, 1, 0x0DF4, 1, 0x0E4F, 1, 0x0E5A, 2, 0x0F04, 15, 0x0F85, 1, 0x0FD0, 5, 0x104A, 
                6, 0x10FB, 1, 0x1361, 8, 0x166D, 2, 0x16EB, 3, 0x1735, 2, 0x17D4, 3, 0x17D8, 3, 0x1800, 6, 0x1807, 4, 0x1944, 
                2, 0x19DE, 2, 0x1A1E, 2, 0x1B5A, 7, 0x1C3B, 5, 0x1C7E, 2, 0x2016, 2, 0x2020, 8, 0x2030, 9, 0x203B, 4, 0x2041, 
                3, 0x2047, 11, 0x2053, 1, 0x2055, 10, 0x2CF9, 4, 0x2CFE, 2, 0x2E00, 2, 0x2E06, 3, 0x2E0B, 1, 0x2E0E, 9, 0x2E18, 
                2, 0x2E1B, 1, 0x2E1E, 2, 0x2E2A, 5, 0x2E30, 1, 0x3001, 3, 0x303D, 1, 0x30FB, 1, 0xA60D, 3, 0xA673, 1, 0xA67E, 
                1, 0xA874, 4, 0xA8CE, 2, 0xA92E, 2, 0xA95F, 1, 0xAA5C, 4, 0xFE10, 7, 0xFE19, 1, 0xFE30, 1, 0xFE45, 2, 0xFE49, 
                4, 0xFE50, 3, 0xFE54, 4, 0xFE5F, 3, 0xFE68, 1, 0xFE6A, 2, 0xFF01, 3, 0xFF05, 3, 0xFF0A, 1, 0xFF0C, 1, 0xFF0E, 
                2, 0xFF1A, 2, 0xFF1F, 2, 0xFF3C, 1, 0xFF61, 1, 0xFF64, 2};
            int[] UgcList_Sm = new int[] { 0x002B, 1, 0x003C, 3, 0x007C, 1, 0x007E, 1, 0x00AC, 1, 0x00B1, 
                1, 0x00D7, 1, 0x00F7, 1, 0x03F6, 1, 0x0606, 3, 0x2044, 1, 0x2052, 1, 0x207A, 3, 0x208A, 3, 0x2140, 5, 0x214B, 
                1, 0x2190, 5, 0x219A, 2, 0x21A0, 1, 0x21A3, 1, 0x21A6, 1, 0x21AE, 1, 0x21CE, 2, 0x21D2, 1, 0x21D4, 1, 0x21F4, 
                268, 0x2308, 4, 0x2320, 2, 0x237C, 1, 0x239B, 25, 0x23DC, 6, 0x25B7, 1, 0x25C1, 1, 0x25F8, 8, 0x266F, 1, 0x27C0, 
                5, 0x27C7, 4, 0x27CC, 1, 0x27D0, 22, 0x27F0, 16, 0x2900, 131, 0x2999, 63, 0x29DC, 32, 0x29FE, 258, 0x2B30, 21, 0x2B47, 
                6, 0xFB29, 1, 0xFE62, 1, 0xFE64, 3, 0xFF0B, 1, 0xFF1C, 3, 0xFF5C, 1, 0xFF5E, 1, 0xFFE2, 1, 0xFFE9, 4};
            int[] UgcList_Sc = new int[] { 0x0024, 1, 0x00A2, 4, 0x060B, 1, 0x09F2, 2, 0x0AF1, 1, 0x0BF9, 
                1, 0x0E3F, 1, 0x17DB, 1, 0x20A0, 22, 0xFDFC, 1, 0xFE69, 1, 0xFF04, 1, 0xFFE0, 2, 0xFFE5, 2};
            int[] UgcList_Sk = new int[] { 0x005E, 1, 0x0060, 1, 0x00A8, 1, 0x00AF, 1, 0x00B4, 1, 0x00B8, 
                1, 0x02C2, 4, 0x02D2, 14, 0x02E5, 7, 0x02ED, 1, 0x02EF, 17, 0x0375, 1, 0x0384, 2, 0x1FBD, 1, 0x1FBF, 3, 0x1FCD, 
                3, 0x1FDD, 3, 0x1FED, 3, 0x1FFD, 2, 0x309B, 2, 0xA700, 23, 0xA720, 2, 0xA789, 2, 0xFF3E, 1, 0xFF40, 1, 0xFFE3, 
                1};
            int[] UgcList_So = new int[] { 0x00A6, 2, 0x00A9, 1, 0x00AE, 1, 0x00B0, 1, 0x00B6, 1, 0x0482, 
                1, 0x060E, 2, 0x06E9, 1, 0x06FD, 2, 0x07F6, 1, 0x09FA, 1, 0x0B70, 1, 0x0BF3, 6, 0x0BFA, 1, 0x0C7F, 1, 0x0CF1, 
                2, 0x0D79, 1, 0x0F01, 3, 0x0F13, 5, 0x0F1A, 6, 0x0F34, 1, 0x0F36, 1, 0x0F38, 1, 0x0FBE, 8, 0x0FC7, 6, 0x0FCE, 
                2, 0x109E, 2, 0x1360, 1, 0x1390, 10, 0x1940, 1, 0x19E0, 32, 0x1B61, 10, 0x1B74, 9, 0x2100, 2, 0x2103, 4, 0x2108, 
                2, 0x2114, 1, 0x2116, 3, 0x211E, 6, 0x2125, 1, 0x2127, 1, 0x2129, 1, 0x212E, 1, 0x213A, 2, 0x214A, 1, 0x214C, 
                2, 0x214F, 1, 0x2195, 5, 0x219C, 4, 0x21A1, 2, 0x21A4, 2, 0x21A7, 7, 0x21AF, 31, 0x21D0, 2, 0x21D3, 1, 0x21D5, 
                31, 0x2300, 8, 0x230C, 20, 0x2322, 7, 0x232B, 81, 0x237D, 30, 0x23B4, 40, 0x23E2, 6, 0x2400, 39, 0x2440, 11, 0x249C, 
                78, 0x2500, 183, 0x25B8, 9, 0x25C2, 54, 0x2600, 111, 0x2670, 46, 0x26A0, 29, 0x26C0, 4, 0x2701, 4, 0x2706, 4, 0x270C, 
                28, 0x2729, 35, 0x274D, 1, 0x274F, 4, 0x2756, 1, 0x2758, 7, 0x2761, 7, 0x2794, 1, 0x2798, 24, 0x27B1, 14, 0x2800, 
                256, 0x2B00, 48, 0x2B45, 2, 0x2B50, 5, 0x2CE5, 6, 0x2E80, 26, 0x2E9B, 89, 0x2F00, 214, 0x2FF0, 12, 0x3004, 1, 0x3012, 
                2, 0x3020, 1, 0x3036, 2, 0x303E, 2, 0x3190, 2, 0x3196, 10, 0x31C0, 36, 0x3200, 31, 0x322A, 26, 0x3250, 1, 0x3260, 
                32, 0x328A, 39, 0x32C0, 63, 0x3300, 256, 0x4DC0, 64, 0xA490, 55, 0xA828, 4, 0xFDFD, 1, 0xFFE4, 1, 0xFFE8, 1, 0xFFED, 
                2, 0xFFFC, 2};
            int[] UgcList_Zs = new int[] { 0x0020, 1, 0x00A0, 1, 0x1680, 1, 0x180E, 1, 0x2000, 11, 0x202F, 
                1, 0x205F, 1, 0x3000, 1};
            int[] UgcList_Zl = new int[] { 0x2028, 1 };
            int[] UgcList_Zp = new int[] { 0x2029, 1 };
            int[] UgcList_Cc = new int[] { 0x0000, 32, 0x007F, 33 };
            int[] UgcList_Cf = new int[] { 0x00AD, 1, 0x0600, 4, 0x06DD, 1, 0x070F, 1, 0x17B4, 2, 0x200B, 
                5, 0x202A, 5, 0x2060, 5, 0x206A, 6, 0xFEFF, 1, 0xFFF9, 3};
            int[] UgcList_Cs = new int[] { 0xD800, 1, 0xDB7F, 2, 0xDBFF, 2, 0xDFFF, 1 };
            int[] UgcList_Co = new int[] { 0xE000, 1, 0xF8FF, 1 };

            for (int i = 0; i < UgcList_Lu.Length; i += 2)
                for (int j = UgcList_Lu[i]; j < UgcList_Lu[i] + UgcList_Lu[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Lu);

            for (int i = 0; i < UgcList_Ll.Length; i += 2)
                for (int j = UgcList_Ll[i]; j < UgcList_Ll[i] + UgcList_Ll[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Ll);

            for (int i = 0; i < UgcList_Lt.Length; i += 2)
                for (int j = UgcList_Lt[i]; j < UgcList_Lt[i] + UgcList_Lt[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Lt);

            for (int i = 0; i < UgcList_Lm.Length; i += 2)
                for (int j = UgcList_Lm[i]; j < UgcList_Lm[i] + UgcList_Lm[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Lm);

            for (int i = 0; i < UgcList_Lo.Length; i += 2)
                for (int j = UgcList_Lo[i]; j < UgcList_Lo[i] + UgcList_Lo[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Lo);

            for (int i = 0; i < UgcList_Mn.Length; i += 2)
                for (int j = UgcList_Mn[i]; j < UgcList_Mn[i] + UgcList_Mn[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Mn);

            for (int i = 0; i < UgcList_Mc.Length; i += 2)
                for (int j = UgcList_Mc[i]; j < UgcList_Mc[i] + UgcList_Mc[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Mc);

            for (int i = 0; i < UgcList_Me.Length; i += 2)
                for (int j = UgcList_Me[i]; j < UgcList_Me[i] + UgcList_Me[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Me);

            for (int i = 0; i < UgcList_Nd.Length; i += 2)
                for (int j = UgcList_Nd[i]; j < UgcList_Nd[i] + UgcList_Nd[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Nd);

            for (int i = 0; i < UgcList_Nl.Length; i += 2)
                for (int j = UgcList_Nl[i]; j < UgcList_Nl[i] + UgcList_Nl[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Nl);

            for (int i = 0; i < UgcList_No.Length; i += 2)
                for (int j = UgcList_No[i]; j < UgcList_No[i] + UgcList_No[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.No);

            for (int i = 0; i < UgcList_Pc.Length; i += 2)
                for (int j = UgcList_Pc[i]; j < UgcList_Pc[i] + UgcList_Pc[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Pc);

            for (int i = 0; i < UgcList_Pd.Length; i += 2)
                for (int j = UgcList_Pd[i]; j < UgcList_Pd[i] + UgcList_Pd[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Pd);

            for (int i = 0; i < UgcList_Ps.Length; i += 2)
                for (int j = UgcList_Ps[i]; j < UgcList_Ps[i] + UgcList_Ps[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Ps);

            for (int i = 0; i < UgcList_Pe.Length; i += 2)
                for (int j = UgcList_Pe[i]; j < UgcList_Pe[i] + UgcList_Pe[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Pe);

            for (int i = 0; i < UgcList_Pi.Length; i += 2)
                for (int j = UgcList_Pi[i]; j < UgcList_Pi[i] + UgcList_Pi[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Pi);

            for (int i = 0; i < UgcList_Pf.Length; i += 2)
                for (int j = UgcList_Pf[i]; j < UgcList_Pf[i] + UgcList_Pf[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Pf);

            for (int i = 0; i < UgcList_Po.Length; i += 2)
                for (int j = UgcList_Po[i]; j < UgcList_Po[i] + UgcList_Po[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Po);

            for (int i = 0; i < UgcList_Sm.Length; i += 2)
                for (int j = UgcList_Sm[i]; j < UgcList_Sm[i] + UgcList_Sm[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Sm);

            for (int i = 0; i < UgcList_Sc.Length; i += 2)
                for (int j = UgcList_Sc[i]; j < UgcList_Sc[i] + UgcList_Sc[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Sc);

            for (int i = 0; i < UgcList_Sk.Length; i += 2)
                for (int j = UgcList_Sk[i]; j < UgcList_Sk[i] + UgcList_Sk[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Sk);

            for (int i = 0; i < UgcList_So.Length; i += 2)
                for (int j = UgcList_So[i]; j < UgcList_So[i] + UgcList_So[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.So);

            for (int i = 0; i < UgcList_Zs.Length; i += 2)
                for (int j = UgcList_Zs[i]; j < UgcList_Zs[i] + UgcList_Zs[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Zs);

            for (int i = 0; i < UgcList_Zl.Length; i += 2)
                for (int j = UgcList_Zl[i]; j < UgcList_Zl[i] + UgcList_Zl[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Zl);

            for (int i = 0; i < UgcList_Zp.Length; i += 2)
                for (int j = UgcList_Zp[i]; j < UgcList_Zp[i] + UgcList_Zp[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Zp);

            for (int i = 0; i < UgcList_Cc.Length; i += 2)
                for (int j = UgcList_Cc[i]; j < UgcList_Cc[i] + UgcList_Cc[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Cc);

            for (int i = 0; i < UgcList_Cf.Length; i += 2)
                for (int j = UgcList_Cf[i]; j < UgcList_Cf[i] + UgcList_Cf[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Cf);

            for (int i = 0; i < UgcList_Cs.Length; i += 2)
                for (int j = UgcList_Cs[i]; j < UgcList_Cs[i] + UgcList_Cs[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Cs);

            for (int i = 0; i < UgcList_Co.Length; i += 2)
                for (int j = UgcList_Co[i]; j < UgcList_Co[i] + UgcList_Co[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeGeneralCategoryItemIndex, (int)UnicodeGeneralCategory.Co);
        }

        private static void InitUnicodeDecompositionTypes()
        {
            // The following arrays moved to the method scope, since they are not used anywhere else,
            // so there is no reason to store them as permanent static fields.
            int[] UdtList_Font = new int[]
            {
                0x2102, 1, 0x210A, 10, 0x2115, 1, 0x2119, 5, 0x2124, 1, 0x2128,
                1, 0x212C, 2, 0x212F, 3, 0x2133, 2, 0x2139, 1, 0x213C, 5, 0x2145, 5, 0xFB20, 10
            };
            int[] UdtList_NoBreak = new int[] { 0x00A0, 1, 0x0F0C, 1, 0x2007, 1, 0x2011, 1, 0x202F, 1 };
            int[] UdtList_Initial = new int[]
            {
                0xFB54, 1, 0xFB58, 1, 0xFB5C, 1, 0xFB60, 1, 0xFB64, 1, 0xFB68,
                1, 0xFB6C, 1, 0xFB70, 1, 0xFB74, 1, 0xFB78, 1, 0xFB7C, 1, 0xFB80, 1, 0xFB90, 1, 0xFB94, 1, 0xFB98, 1, 0xFB9C,
                1, 0xFBA2, 1, 0xFBA8, 1, 0xFBAC, 1, 0xFBD5, 1, 0xFBE6, 1, 0xFBE8, 1, 0xFBF8, 1, 0xFBFB, 1, 0xFBFE, 1, 0xFC97,
                72, 0xFD2D, 7, 0xFD50, 1, 0xFD52, 6, 0xFD59, 1, 0xFD5C, 2, 0xFD60, 2, 0xFD63, 1, 0xFD65, 1, 0xFD68, 1, 0xFD6B,
                1, 0xFD6D, 1, 0xFD70, 1, 0xFD72, 2, 0xFD77, 1, 0xFD7D, 1, 0xFD83, 1, 0xFD86, 1, 0xFD88, 3, 0xFD8C, 4, 0xFD92,
                4, 0xFD98, 1, 0xFD9D, 1, 0xFDB4, 2, 0xFDB8, 1, 0xFDBA, 1, 0xFDC3, 3, 0xFE8B, 1, 0xFE91, 1, 0xFE97, 1, 0xFE9B,
                1, 0xFE9F, 1, 0xFEA3, 1, 0xFEA7, 1, 0xFEB3, 1, 0xFEB7, 1, 0xFEBB, 1, 0xFEBF, 1, 0xFEC3, 1, 0xFEC7, 1, 0xFECB,
                1, 0xFECF, 1, 0xFED3, 1, 0xFED7, 1, 0xFEDB, 1, 0xFEDF, 1, 0xFEE3, 1, 0xFEE7, 1, 0xFEEB, 1, 0xFEF3, 1
            };
            int[] UdtList_Medial = new int[]
            {
                0xFB55, 1, 0xFB59, 1, 0xFB5D, 1, 0xFB61, 1, 0xFB65, 1, 0xFB69,
                1, 0xFB6D, 1, 0xFB71, 1, 0xFB75, 1, 0xFB79, 1, 0xFB7D, 1, 0xFB81, 1, 0xFB91, 1, 0xFB95, 1, 0xFB99, 1, 0xFB9D,
                1, 0xFBA3, 1, 0xFBA9, 1, 0xFBAD, 1, 0xFBD6, 1, 0xFBE7, 1, 0xFBE9, 1, 0xFBFF, 1, 0xFCDF, 22, 0xFD34, 8, 0xFE71,
                1, 0xFE77, 1, 0xFE79, 1, 0xFE7B, 1, 0xFE7D, 1, 0xFE7F, 1, 0xFE8C, 1, 0xFE92, 1, 0xFE98, 1, 0xFE9C, 1, 0xFEA0,
                1, 0xFEA4, 1, 0xFEA8, 1, 0xFEB4, 1, 0xFEB8, 1, 0xFEBC, 1, 0xFEC0, 1, 0xFEC4, 1, 0xFEC8, 1, 0xFECC, 1, 0xFED0,
                1, 0xFED4, 1, 0xFED8, 1, 0xFEDC, 1, 0xFEE0, 1, 0xFEE4, 1, 0xFEE8, 1, 0xFEEC, 1, 0xFEF4, 1
            };
            int[] UdtList_Final = new int[]
            {
                0xFB51, 1, 0xFB53, 1, 0xFB57, 1, 0xFB5B, 1, 0xFB5F, 1, 0xFB63,
                1, 0xFB67, 1, 0xFB6B, 1, 0xFB6F, 1, 0xFB73, 1, 0xFB77, 1, 0xFB7B, 1, 0xFB7F, 1, 0xFB83, 1, 0xFB85, 1, 0xFB87,
                1, 0xFB89, 1, 0xFB8B, 1, 0xFB8D, 1, 0xFB8F, 1, 0xFB93, 1, 0xFB97, 1, 0xFB9B, 1, 0xFB9F, 1, 0xFBA1, 1, 0xFBA5,
                1, 0xFBA7, 1, 0xFBAB, 1, 0xFBAF, 1, 0xFBB1, 1, 0xFBD4, 1, 0xFBD8, 1, 0xFBDA, 1, 0xFBDC, 1, 0xFBDF, 1, 0xFBE1,
                1, 0xFBE3, 1, 0xFBE5, 1, 0xFBEB, 1, 0xFBED, 1, 0xFBEF, 1, 0xFBF1, 1, 0xFBF3, 1, 0xFBF5, 1, 0xFBF7, 1, 0xFBFA,
                1, 0xFBFD, 1, 0xFC64, 51, 0xFD11, 28, 0xFD3C, 1, 0xFD51, 1, 0xFD58, 1, 0xFD5A, 2, 0xFD5E, 2, 0xFD62, 1, 0xFD64,
                1, 0xFD66, 2, 0xFD69, 2, 0xFD6C, 1, 0xFD6E, 2, 0xFD71, 1, 0xFD74, 3, 0xFD78, 5, 0xFD7E, 5, 0xFD84, 2, 0xFD87,
                1, 0xFD8B, 1, 0xFD96, 2, 0xFD99, 4, 0xFD9E, 22, 0xFDB6, 2, 0xFDB9, 1, 0xFDBB, 8, 0xFDC6, 2, 0xFE82, 1, 0xFE84,
                1, 0xFE86, 1, 0xFE88, 1, 0xFE8A, 1, 0xFE8E, 1, 0xFE90, 1, 0xFE94, 1, 0xFE96, 1, 0xFE9A, 1, 0xFE9E, 1, 0xFEA2,
                1, 0xFEA6, 1, 0xFEAA, 1, 0xFEAC, 1, 0xFEAE, 1, 0xFEB0, 1, 0xFEB2, 1, 0xFEB6, 1, 0xFEBA, 1, 0xFEBE, 1, 0xFEC2,
                1, 0xFEC6, 1, 0xFECA, 1, 0xFECE, 1, 0xFED2, 1, 0xFED6, 1, 0xFEDA, 1, 0xFEDE, 1, 0xFEE2, 1, 0xFEE6, 1, 0xFEEA,
                1, 0xFEEE, 1, 0xFEF0, 1, 0xFEF2, 1, 0xFEF6, 1, 0xFEF8, 1, 0xFEFA, 1, 0xFEFC, 1
            };
            int[] UdtList_Isolated = new int[]
            {
                0xFB50, 1, 0xFB52, 1, 0xFB56, 1, 0xFB5A, 1, 0xFB5E, 1, 0xFB62,
                1, 0xFB66, 1, 0xFB6A, 1, 0xFB6E, 1, 0xFB72, 1, 0xFB76, 1, 0xFB7A, 1, 0xFB7E, 1, 0xFB82, 1, 0xFB84, 1, 0xFB86,
                1, 0xFB88, 1, 0xFB8A, 1, 0xFB8C, 1, 0xFB8E, 1, 0xFB92, 1, 0xFB96, 1, 0xFB9A, 1, 0xFB9E, 1, 0xFBA0, 1, 0xFBA4,
                1, 0xFBA6, 1, 0xFBAA, 1, 0xFBAE, 1, 0xFBB0, 1, 0xFBD3, 1, 0xFBD7, 1, 0xFBD9, 1, 0xFBDB, 1, 0xFBDD, 2, 0xFBE0,
                1, 0xFBE2, 1, 0xFBE4, 1, 0xFBEA, 1, 0xFBEC, 1, 0xFBEE, 1, 0xFBF0, 1, 0xFBF2, 1, 0xFBF4, 1, 0xFBF6, 1, 0xFBF9,
                1, 0xFBFC, 1, 0xFC00, 100, 0xFCF5, 28, 0xFD3D, 1, 0xFDF0, 13, 0xFE70, 1, 0xFE72, 1, 0xFE74, 1, 0xFE76, 1, 0xFE78,
                1, 0xFE7A, 1, 0xFE7C, 1, 0xFE7E, 1, 0xFE80, 2, 0xFE83, 1, 0xFE85, 1, 0xFE87, 1, 0xFE89, 1, 0xFE8D, 1, 0xFE8F,
                1, 0xFE93, 1, 0xFE95, 1, 0xFE99, 1, 0xFE9D, 1, 0xFEA1, 1, 0xFEA5, 1, 0xFEA9, 1, 0xFEAB, 1, 0xFEAD, 1, 0xFEAF,
                1, 0xFEB1, 1, 0xFEB5, 1, 0xFEB9, 1, 0xFEBD, 1, 0xFEC1, 1, 0xFEC5, 1, 0xFEC9, 1, 0xFECD, 1, 0xFED1, 1, 0xFED5,
                1, 0xFED9, 1, 0xFEDD, 1, 0xFEE1, 1, 0xFEE5, 1, 0xFEE9, 1, 0xFEED, 1, 0xFEEF, 1, 0xFEF1, 1, 0xFEF5, 1, 0xFEF7,
                1, 0xFEF9, 1, 0xFEFB, 1
            };
            int[] UdtList_Circle = new int[] { 0x2460, 20, 0x24B6, 53, 0x3251, 46, 0x3280, 64, 0x32D0, 47 };
            int[] UdtList_Super = new int[]
            {
                0x00AA, 1, 0x00B2, 2, 0x00B9, 2, 0x02B0, 9, 0x02E0, 5, 0x10FC,
                1, 0x1D2C, 3, 0x1D30, 11, 0x1D3C, 18, 0x1D4F, 19, 0x1D78, 1, 0x1D9B, 37, 0x2070, 2, 0x2074, 12, 0x2120, 1, 0x2122,
                1, 0x2C7D, 1, 0x2D6F, 1, 0x3192, 14, 0xA770, 1
            };
            int[] UdtList_Sub = new int[] { 0x1D62, 9, 0x2080, 15, 0x2090, 5, 0x2C7C, 1 };
            int[] UdtList_Vertical = new int[] { 0x309F, 1, 0x30FF, 1, 0xFE10, 10, 0xFE30, 21, 0xFE47, 2 };
            int[] UdtList_Wide = new int[] { 0x3000, 1, 0xFF01, 96, 0xFFE0, 7 };
            int[] UdtList_Narrow = new int[]
            {
                0xFF61, 94, 0xFFC2, 6, 0xFFCA, 6, 0xFFD2, 6, 0xFFDA, 3, 0xFFE8,
                7
            };
            int[] UdtList_Small = new int[] { 0xFE50, 3, 0xFE54, 19, 0xFE68, 4 };
            int[] UdtList_Square = new int[] { 0x3250, 1, 0x32CC, 4, 0x3300, 88, 0x3371, 111, 0x33FF, 1 };
            int[] UdtList_Fraction = new int[] { 0x00BC, 3, 0x2153, 13 };
            int[] UdtList_Compat = new int[]
            {
                0x00A8, 1, 0x00AF, 1, 0x00B4, 2, 0x00B8, 1, 0x0132, 2, 0x013F,
                2, 0x0149, 1, 0x017F, 1, 0x01C4, 9, 0x01F1, 3, 0x02D8, 6, 0x037A, 1, 0x0384, 1, 0x03D0, 3, 0x03D5, 2, 0x03F0,
                3, 0x03F4, 2, 0x03F9, 1, 0x0587, 1, 0x0675, 4, 0x0E33, 1, 0x0EB3, 1, 0x0EDC, 2, 0x0F77, 1, 0x0F79, 1, 0x1E9A,
                1, 0x1FBD, 1, 0x1FBF, 2, 0x1FFE, 1, 0x2002, 5, 0x2008, 3, 0x2017, 1, 0x2024, 3, 0x2033, 2, 0x2036, 2, 0x203C,
                1, 0x203E, 1, 0x2047, 3, 0x2057, 1, 0x205F, 1, 0x20A8, 1, 0x2100, 2, 0x2103, 1, 0x2105, 3, 0x2109, 1, 0x2116,
                1, 0x2121, 1, 0x2135, 4, 0x213B, 1, 0x2160, 32, 0x222C, 2, 0x222F, 2, 0x2474, 66, 0x2A0C, 1, 0x2A74, 3, 0x2E9F,
                1, 0x2EF3, 1, 0x2F00, 214, 0x3036, 1, 0x3038, 3, 0x309B, 2, 0x3131, 94, 0x3200, 31, 0x3220, 36, 0x32C0, 12, 0x3358,
                25, 0x33E0, 31, 0xFB00, 7, 0xFB13, 5, 0xFB4F, 1, 0xFE49, 7
            };

            for (int i = 0; i < UdtList_Font.Length; i += 2)
                for (int j = UdtList_Font[i]; j < UdtList_Font[i] + UdtList_Font[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Font);

            for (int i = 0; i < UdtList_NoBreak.Length; i += 2)
                for (int j = UdtList_NoBreak[i]; j < UdtList_NoBreak[i] + UdtList_NoBreak[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.NoBreak);

            for (int i = 0; i < UdtList_Initial.Length; i += 2)
                for (int j = UdtList_Initial[i]; j < UdtList_Initial[i] + UdtList_Initial[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Initial);

            for (int i = 0; i < UdtList_Medial.Length; i += 2)
                for (int j = UdtList_Medial[i]; j < UdtList_Medial[i] + UdtList_Medial[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Medial);

            for (int i = 0; i < UdtList_Final.Length; i += 2)
                for (int j = UdtList_Final[i]; j < UdtList_Final[i] + UdtList_Final[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Final);

            for (int i = 0; i < UdtList_Isolated.Length; i += 2)
                for (int j = UdtList_Isolated[i]; j < UdtList_Isolated[i] + UdtList_Isolated[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Isolated);

            for (int i = 0; i < UdtList_Circle.Length; i += 2)
                for (int j = UdtList_Circle[i]; j < UdtList_Circle[i] + UdtList_Circle[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Circle);

            for (int i = 0; i < UdtList_Super.Length; i += 2)
                for (int j = UdtList_Super[i]; j < UdtList_Super[i] + UdtList_Super[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Super);

            for (int i = 0; i < UdtList_Sub.Length; i += 2)
                for (int j = UdtList_Sub[i]; j < UdtList_Sub[i] + UdtList_Sub[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Sub);

            for (int i = 0; i < UdtList_Vertical.Length; i += 2)
                for (int j = UdtList_Vertical[i]; j < UdtList_Vertical[i] + UdtList_Vertical[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Vertical);

            for (int i = 0; i < UdtList_Wide.Length; i += 2)
                for (int j = UdtList_Wide[i]; j < UdtList_Wide[i] + UdtList_Wide[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Wide);

            for (int i = 0; i < UdtList_Narrow.Length; i += 2)
                for (int j = UdtList_Narrow[i]; j < UdtList_Narrow[i] + UdtList_Narrow[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Narrow);

            for (int i = 0; i < UdtList_Small.Length; i += 2)
                for (int j = UdtList_Small[i]; j < UdtList_Small[i] + UdtList_Small[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Small);

            for (int i = 0; i < UdtList_Square.Length; i += 2)
                for (int j = UdtList_Square[i]; j < UdtList_Square[i] + UdtList_Square[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Square);

            for (int i = 0; i < UdtList_Fraction.Length; i += 2)
                for (int j = UdtList_Fraction[i]; j < UdtList_Fraction[i] + UdtList_Fraction[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Fraction);

            for (int i = 0; i < UdtList_Compat.Length; i += 2)
                for (int j = UdtList_Compat[i]; j < UdtList_Compat[i] + UdtList_Compat[i + 1]; ++j)
                    SetCharDataItem((char)j, UnicodeDecompositionTypeItemIndex, (int)UnicodeDecompositionType.Compat);
        }

        private static void InitUnicodeCanonicalClasses()
        {
            SetCharDataItem('\u0300', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0301', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0302', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0303', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0304', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0305', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0306', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0307', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0308', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0309', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u030A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u030B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u030C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u030D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u030E', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u030F', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0310', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0311', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0312', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0313', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0314', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0315', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.AR);
            SetCharDataItem('\u0316', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0317', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0318', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0319', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u031A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.AR);
            SetCharDataItem('\u031B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.ATAR);
            SetCharDataItem('\u031C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u031D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u031E', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u031F', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0320', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0321', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.ATB);
            SetCharDataItem('\u0322', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.ATB);
            SetCharDataItem('\u0323', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0324', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0325', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0326', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0327', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.ATB);
            SetCharDataItem('\u0328', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.ATB);
            SetCharDataItem('\u0329', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u032A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u032B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u032C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u032D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u032E', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u032F', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0330', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0331', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0332', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0333', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0334', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.OV);
            SetCharDataItem('\u0335', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.OV);
            SetCharDataItem('\u0336', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.OV);
            SetCharDataItem('\u0337', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.OV);
            SetCharDataItem('\u0338', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.OV);
            SetCharDataItem('\u0339', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u033A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u033B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u033C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u033D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u033E', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u033F', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0340', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0341', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0342', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0343', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0344', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0345', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.IS);
            SetCharDataItem('\u0346', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0347', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0348', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0349', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u034A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u034B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u034C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u034D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u034E', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0350', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0351', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0352', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0353', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0354', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0355', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0356', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0357', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0358', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.AR);
            SetCharDataItem('\u0359', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u035A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u035B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u035C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.DB);
            SetCharDataItem('\u035D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.DA);
            SetCharDataItem('\u035E', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.DA);
            SetCharDataItem('\u035F', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.DB);
            SetCharDataItem('\u0360', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.DA);
            SetCharDataItem('\u0361', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.DA);
            SetCharDataItem('\u0362', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.DB);
            SetCharDataItem('\u0363', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0364', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0365', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0366', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0367', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0368', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0369', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u036A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u036B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u036C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u036D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u036E', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u036F', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0483', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0484', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0485', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0486', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0487', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0591', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0592', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0593', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0594', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0595', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0596', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0597', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0598', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0599', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u059A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.BR);
            SetCharDataItem('\u059B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u059C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u059D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u059E', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u059F', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u05A0', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u05A1', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u05A2', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u05A3', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u05A4', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u05A5', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u05A6', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u05A7', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u05A8', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u05A9', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u05AA', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u05AB', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u05AC', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u05AD', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.BR);
            SetCharDataItem('\u05AE', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.AL);
            SetCharDataItem('\u05AF', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u05B0', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_10);
            SetCharDataItem('\u05B1', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_11);
            SetCharDataItem('\u05B2', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_12);
            SetCharDataItem('\u05B3', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_13);
            SetCharDataItem('\u05B4', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_14);
            SetCharDataItem('\u05B5', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_15);
            SetCharDataItem('\u05B6', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_16);
            SetCharDataItem('\u05B7', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_17);
            SetCharDataItem('\u05B8', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_18);
            SetCharDataItem('\u05B9', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_19);
            SetCharDataItem('\u05BA', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_19);
            SetCharDataItem('\u05BB', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_20);
            SetCharDataItem('\u05BC', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_21);
            SetCharDataItem('\u05BD', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_22);
            SetCharDataItem('\u05BF', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_23);
            SetCharDataItem('\u05C1', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_24);
            SetCharDataItem('\u05C2', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_25);
            SetCharDataItem('\u05C4', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u05C5', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u05C7', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_18);
            SetCharDataItem('\u0610', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0611', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0612', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0613', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0614', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0615', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0616', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0617', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0618', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_30);
            SetCharDataItem('\u0619', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_31);
            SetCharDataItem('\u061A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_32);
            SetCharDataItem('\u064B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_27);
            SetCharDataItem('\u064C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_28);
            SetCharDataItem('\u064D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_29);
            SetCharDataItem('\u064E', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_30);
            SetCharDataItem('\u064F', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_31);
            SetCharDataItem('\u0650', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_32);
            SetCharDataItem('\u0651', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_33);
            SetCharDataItem('\u0652', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_34);
            SetCharDataItem('\u0653', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0654', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0655', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0656', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0657', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0658', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0659', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u065A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u065B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u065C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u065D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u065E', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0670', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_35);
            SetCharDataItem('\u06D6', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06D7', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06D8', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06D9', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06DA', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06DB', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06DC', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06DF', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06E0', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06E1', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06E2', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06E3', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u06E4', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06E7', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06E8', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06EA', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u06EB', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06EC', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u06ED', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0711', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_36);
            SetCharDataItem('\u0730', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0731', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0732', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0733', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0734', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0735', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0736', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0737', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0738', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0739', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u073A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u073B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u073C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u073D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u073E', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u073F', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0740', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0741', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0742', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0743', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0744', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0745', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0746', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0747', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0748', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0749', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u074A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u07EB', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u07EC', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u07ED', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u07EE', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u07EF', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u07F0', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u07F1', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u07F2', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u07F3', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u093C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.NK);
            SetCharDataItem('\u094D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u0951', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0952', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0953', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0954', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u09BC', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.NK);
            SetCharDataItem('\u09CD', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u0A3C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.NK);
            SetCharDataItem('\u0A4D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u0ABC', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.NK);
            SetCharDataItem('\u0ACD', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u0B3C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.NK);
            SetCharDataItem('\u0B4D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u0BCD', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u0C4D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u0C55', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_84);
            SetCharDataItem('\u0C56', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_91);
            SetCharDataItem('\u0CBC', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.NK);
            SetCharDataItem('\u0CCD', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u0D4D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u0DCA', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u0E38', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_103);
            SetCharDataItem('\u0E39', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_103);
            SetCharDataItem('\u0E3A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u0E48', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_107);
            SetCharDataItem('\u0E49', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_107);
            SetCharDataItem('\u0E4A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_107);
            SetCharDataItem('\u0E4B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_107);
            SetCharDataItem('\u0EB8', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_118);
            SetCharDataItem('\u0EB9', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_118);
            SetCharDataItem('\u0EC8', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_122);
            SetCharDataItem('\u0EC9', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_122);
            SetCharDataItem('\u0ECA', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_122);
            SetCharDataItem('\u0ECB', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_122);
            SetCharDataItem('\u0F18', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0F19', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0F35', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0F37', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u0F39', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.ATAR);
            SetCharDataItem('\u0F71', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_129);
            SetCharDataItem('\u0F72', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_130);
            SetCharDataItem('\u0F74', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_132);
            SetCharDataItem('\u0F7A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_130);
            SetCharDataItem('\u0F7B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_130);
            SetCharDataItem('\u0F7C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_130);
            SetCharDataItem('\u0F7D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_130);
            SetCharDataItem('\u0F80', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_130);
            SetCharDataItem('\u0F82', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0F83', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0F84', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u0F86', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0F87', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u0FC6', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u1037', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.NK);
            SetCharDataItem('\u1039', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u103A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u108D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u135F', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1714', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u1734', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u17D2', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u17DD', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u18A9', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.AL);
            SetCharDataItem('\u1939', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.BR);
            SetCharDataItem('\u193A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u193B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u1A17', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1A18', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u1B34', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.NK);
            SetCharDataItem('\u1B44', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u1B6B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1B6C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u1B6D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1B6E', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1B6F', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1B70', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1B71', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1B72', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1B73', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1BAA', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\u1C37', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.NK);
            SetCharDataItem('\u1DC0', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DC1', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DC2', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u1DC3', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DC4', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DC5', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DC6', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DC7', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DC8', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DC9', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DCA', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u1DCB', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DCC', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DCD', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.DA);
            SetCharDataItem('\u1DCE', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.ATA);
            SetCharDataItem('\u1DCF', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u1DD0', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.ATB);
            SetCharDataItem('\u1DD1', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DD2', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DD3', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DD4', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DD5', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DD6', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DD7', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DD8', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DD9', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DDA', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DDB', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DDC', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DDD', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DDE', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DDF', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DE0', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DE1', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DE2', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DE3', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DE4', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DE5', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DE6', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DFE', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u1DFF', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u20D0', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u20D1', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u20D2', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.OV);
            SetCharDataItem('\u20D3', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.OV);
            SetCharDataItem('\u20D4', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u20D5', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u20D6', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u20D7', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u20D8', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.OV);
            SetCharDataItem('\u20D9', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.OV);
            SetCharDataItem('\u20DA', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.OV);
            SetCharDataItem('\u20DB', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u20DC', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u20E1', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u20E5', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.OV);
            SetCharDataItem('\u20E6', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.OV);
            SetCharDataItem('\u20E7', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u20E8', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u20E9', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u20EA', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.OV);
            SetCharDataItem('\u20EB', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.OV);
            SetCharDataItem('\u20EC', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u20ED', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u20EE', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u20EF', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\u20F0', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DE0', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DE1', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DE2', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DE3', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DE4', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DE5', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DE6', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DE7', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DE8', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DE9', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DEA', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DEB', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DEC', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DED', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DEE', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DEF', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DF0', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DF1', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DF2', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DF3', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DF4', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DF5', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DF6', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DF7', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DF8', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DF9', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DFA', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DFB', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DFC', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DFD', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DFE', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u2DFF', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\u302A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.BL);
            SetCharDataItem('\u302B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.AL);
            SetCharDataItem('\u302C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.AR);
            SetCharDataItem('\u302D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.BR);
            SetCharDataItem('\u302E', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.L);
            SetCharDataItem('\u302F', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.L);
            SetCharDataItem('\u3099', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.KV);
            SetCharDataItem('\u309A', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.KV);
            SetCharDataItem('\uA66F', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\uA67C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\uA67D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\uA806', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\uA8C4', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\uA92B', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\uA92C', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\uA92D', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.B);
            SetCharDataItem('\uA953', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.VR);
            SetCharDataItem('\uFB1E', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.CLASS_26);
            SetCharDataItem('\uFE20', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\uFE21', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\uFE22', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\uFE23', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\uFE24', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\uFE25', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
            SetCharDataItem('\uFE26', UnicodeCanonicalClassItemIndex, (int)UnicodeCanonicalClass.A);
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int MaxByte = 0xFF;

        // Indexes of bytes used to represent corresponding enum values in gCharData array items.
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int BidiCharacterTypeItemIndex = 0;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int UnicodeGeneralCategoryItemIndex = 1;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int UnicodeCanonicalClassItemIndex = 2;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int UnicodeDecompositionTypeItemIndex = 3;

        // WORDSNET-13730 Split array to sub arrays to be able to initialize character types data
        // and avoid "TypeInitializationException" when memory is fragmented.
        /// <summary>
        /// Bidirectional character types. Index is the character's code point; value is the bidirectional type of the character.
        /// Covers only the Basic Multilingual Plane (code points U+0000..U+FFFF).
        /// </summary>
        /// <dev>
        /// Every item of this array corresponds to a character with a code point equal to the item index and
        /// represents an integer value combining the following enum values: BidiCharacterType (byte 0),
        /// UnicodeGeneralCategory (byte 1), UnicodeCanonicalClass (byte 2) and UnicodeDecompositionType
        /// (byte 3, bit 31 i.e. sign bit is not used).
        /// </dev>
        private static readonly IntBigArray gCharData = new IntBigArray(char.MaxValue + 1);

        private static readonly Dictionary<char, string> gDecomposeMap;

        /// <summary>
        /// Composition map used for character sequences with length of one or two characters.
        /// </summary>
        private static readonly IntToCharDictionary gComposeMap1;

        /// <summary>
        /// Composition map used for character sequences with length greater than two characters.
        /// </summary>
        private static readonly StringToCharDictionary gComposeMap2;
    }
}
