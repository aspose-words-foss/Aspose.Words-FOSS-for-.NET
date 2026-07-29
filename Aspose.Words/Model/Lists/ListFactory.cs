// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2006 by Roman Korchagin
using System;

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Builds lists according to list templates predefined in MS Word.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal static class ListFactory
    {
        /// <summary>
        /// Creates and adds a list and a list definition to the lists collection.
        /// </summary>
        /// <param name="lists">Where to add the new list.</param>
        /// <param name="template">Specifies what kind of list to create.</param>
        internal static List CreateList(ListCollection lists, ListTemplate template)
        {
            switch (template)
            {
                case ListTemplate.BulletDefault:
                    return CreateBulletDefault(lists);
                case ListTemplate.BulletCircle:
                    return CreateBulletDefault(lists, CourierCircle, CourierFont);
                case ListTemplate.BulletSquare:
                    return CreateBulletDefault(lists, WingdingSquare, WingdingsFont);
                case ListTemplate.BulletDiamonds:
                    return CreateBulletDefault(lists, WingdingDiamonds, WingdingsFont);
                case ListTemplate.BulletArrowHead:
                    return CreateBulletDefault(lists, WingdingArrowHead, WingdingsFont);
                case ListTemplate.BulletTick:
                    return CreateBulletDefault(lists, WingdingTick, WingdingsFont);

                case ListTemplate.NumberDefault:
                    return CreateNumberDefault(lists);
                case ListTemplate.NumberArabicParenthesis:
                    return CreateNumberDefault(lists, NumberStyle.Arabic, "\x0000)", ListLevelAlignment.Left);
                case ListTemplate.NumberUppercaseRomanDot:
                    return CreateNumberDefault(lists, NumberStyle.UppercaseRoman, "\x0000.", ListLevelAlignment.Right);
                case ListTemplate.NumberUppercaseLetterDot:
                    return CreateNumberDefault(lists, NumberStyle.UppercaseLetter, "\x0000.", ListLevelAlignment.Left);
                case ListTemplate.NumberLowercaseLetterParenthesis:
                    return CreateNumberDefault(lists, NumberStyle.LowercaseLetter, "\x0000)", ListLevelAlignment.Left);
                case ListTemplate.NumberLowercaseLetterDot:
                    return CreateNumberDefault(lists, NumberStyle.LowercaseLetter, "\x0000.", ListLevelAlignment.Left);
                case ListTemplate.NumberLowercaseRomanDot:
                    return CreateNumberDefault(lists, NumberStyle.LowercaseRoman, "\x0000.", ListLevelAlignment.Right);

                case ListTemplate.OutlineNumbers:
                    return CreateOutlineNumbers(lists);
                case ListTemplate.OutlineLegal:
                    return CreateOutlineLegal(lists);
                case ListTemplate.OutlineBullets:
                    return CreateOutlineBullets(lists);
                case ListTemplate.OutlineHeadingsArticleSection:
                    return CreateOutlineHeadingsArticleSection(lists);
                case ListTemplate.OutlineHeadingsLegal:
                    return CreateOutlineHeadingsLegal(lists);
                case ListTemplate.OutlineHeadingsNumbers:
                    return CreateOutlineHeadingsNumbers(lists);
                case ListTemplate.OutlineHeadingsChapter:
                    return CreateOutlineHeadingsChapter(lists);
                default:
                    throw new ArgumentException("template");
            }
        }

        /// <summary>
        /// Creates and adds a single level list and the list definition to the list collection.
        /// </summary>
        internal static List CreateSingleLevelList(ListCollection lists, ListTemplate template)
        {
            switch (template)
            {
                case ListTemplate.BulletDefault:
                    return CreateSingleLevelBulletList(lists, SymbolDisc, SymbolFont);
                case ListTemplate.BulletCircle:
                    return CreateSingleLevelBulletList(lists, CourierCircle, CourierFont);
                case ListTemplate.BulletSquare:
                    return CreateSingleLevelBulletList(lists, WingdingSquare, WingdingsFont);
                case ListTemplate.BulletDiamonds:
                    return CreateSingleLevelBulletList(lists, WingdingDiamonds, WingdingsFont);
                case ListTemplate.BulletArrowHead:
                    return CreateSingleLevelBulletList(lists, WingdingArrowHead, WingdingsFont);
                case ListTemplate.BulletTick:
                    return CreateSingleLevelBulletList(lists, WingdingTick, WingdingsFont);

                case ListTemplate.NumberDefault:
                    return CreateSingleLevelNumberList(lists, NumberStyle.Arabic, "\x0000.", ListLevelAlignment.Left);
                case ListTemplate.NumberArabicParenthesis:
                    return CreateSingleLevelNumberList(lists, NumberStyle.Arabic, "\x0000)", ListLevelAlignment.Left);
                case ListTemplate.NumberUppercaseRomanDot:
                    return CreateSingleLevelNumberList(lists, NumberStyle.UppercaseRoman, "\x0000.", ListLevelAlignment.Right);
                case ListTemplate.NumberUppercaseLetterDot:
                    return CreateSingleLevelNumberList(lists, NumberStyle.UppercaseLetter, "\x0000.", ListLevelAlignment.Left);
                case ListTemplate.NumberLowercaseLetterParenthesis:
                    return CreateSingleLevelNumberList(lists, NumberStyle.LowercaseLetter, "\x0000)", ListLevelAlignment.Left);
                case ListTemplate.NumberLowercaseLetterDot:
                    return CreateSingleLevelNumberList(lists, NumberStyle.LowercaseLetter, "\x0000.", ListLevelAlignment.Left);
                case ListTemplate.NumberLowercaseRomanDot:
                    return CreateSingleLevelNumberList(lists, NumberStyle.LowercaseRoman, "\x0000.", ListLevelAlignment.Right);
                default:
                    throw new ArgumentException("Unable to apply the outline template to a single level list.", "template");
            }
        }

        /// <summary>
        /// Checks whether list bullet is standard and gives appropriate font.
        /// Returns null for non-standard bullets.
        /// </summary>
        internal static string StandardBulletToFont(char bullet)
        {
            switch (bullet)
            {
                case SymbolDiscChar:
                case SymbolDiamondChar:
                    return SymbolFont;
                case CourierCircleChar:
                    return CourierFont;
                case WingdingSquareChar:
                case WingdingDiamondsChar:
                case WingdingArrowHeadChar:
                case WingdingTickChar:
                    return WingdingsFont;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates a bulleted single level list and the list definition.
        /// </summary>
        private static List CreateSingleLevelBulletList(ListCollection lists, string numberFormat, string fontName)
        {
            List list = lists.AddEmpty(ListType.SingleLevel);

            ListLevelInitializer.InitBulletListLevel(list, 0, fontName, numberFormat, 720, 720, -360);

            return list;
        }

        /// <summary>
        /// Creates a numbered single level list and the list definition.
        /// </summary>
        private static List CreateSingleLevelNumberList(ListCollection lists, NumberStyle numberStyle, string numberFormat, ListLevelAlignment alignment)
        {
            List list = lists.AddEmpty(ListType.SingleLevel);

            ListLevelInitializer.InitNumberListLevel(list, 0, numberStyle, numberFormat, alignment, 720, 720, -360);

            return list;
        }

        /// <summary>
        /// Creates a list and a list definition of a default bulleted list.
        /// </summary>
        private static List CreateBulletDefault(ListCollection lists)
        {
            List list = lists.AddEmpty(ListType.HybridMultiLevel);
            ListLevelInitializer.InitBulletListLevel(list, 0, SymbolFont, SymbolDisc, 720, 720, -360);
            ListLevelInitializer.InitBulletListLevel(list, 1, CourierFont, CourierCircle, 1440, 1440, -360);
            ListLevelInitializer.InitBulletListLevel(list, 2, WingdingsFont, WingdingSquare, 2160, 2160, -360);
            ListLevelInitializer.InitBulletListLevel(list, 3, SymbolFont, SymbolDisc, 2880, 2880, -360);
            ListLevelInitializer.InitBulletListLevel(list, 4, CourierFont, CourierCircle, 3600, 3600, -360);
            ListLevelInitializer.InitBulletListLevel(list, 5, WingdingsFont, WingdingSquare, 4320, 4320, -360);
            ListLevelInitializer.InitBulletListLevel(list, 6, SymbolFont, SymbolDisc, 5040, 5040, -360);
            ListLevelInitializer.InitBulletListLevel(list, 7, CourierFont, CourierCircle, 5760, 5760, -360);
            ListLevelInitializer.InitBulletListLevel(list, 8, WingdingsFont, WingdingSquare, 6480, 6480, -360);
            return list;
        }

        /// <summary>
        /// Creates a list and a list definition based on a default bulleted list,
        /// but with a customized first level. This is to match MS Word list templates.
        /// </summary>
        private static List CreateBulletDefault(ListCollection lists, string firstLevelNumberFormat, string firstLevelFontName)
        {
            List list = CreateBulletDefault(lists);
            list.ListLevels[0].NumberFormat = firstLevelNumberFormat;
            list.ListLevels[0].RunPr.SetSymbolFonts(firstLevelFontName);
            return list;
        }

        /// <summary>
        /// Creates a list and a list definition of a default numbered list.
        /// </summary>
        private static List CreateNumberDefault(ListCollection lists)
        {
            List list = lists.AddEmpty(ListType.MultiLevel);
            ListLevelInitializer.InitNumberListLevel(list, 0, NumberStyle.Arabic, "\x0000.",
                ListLevelAlignment.Left, 720, 720, -360);
            ListLevelInitializer.InitNumberListLevel(list, 1, NumberStyle.LowercaseLetter, "\x0001.",
                ListLevelAlignment.Left, 1440, 1440, -360);
            ListLevelInitializer.InitNumberListLevel(list, 2, NumberStyle.LowercaseRoman, "\x0002.",
                ListLevelAlignment.Right, 2160, 2160, -180);
            ListLevelInitializer.InitNumberListLevel(list, 3, NumberStyle.Arabic, "\x0003.",
                ListLevelAlignment.Left, 2880, 2880, -360);
            ListLevelInitializer.InitNumberListLevel(list, 4, NumberStyle.LowercaseLetter, "\x0004.",
                ListLevelAlignment.Left, 3600, 3600, -360);
            ListLevelInitializer.InitNumberListLevel(list, 5, NumberStyle.LowercaseRoman, "\x0005.",
                ListLevelAlignment.Right, 4320, 4320, -180);
            ListLevelInitializer.InitNumberListLevel(list, 6, NumberStyle.Arabic, "\x0006.",
                ListLevelAlignment.Left, 5040, 5040, -360);
            ListLevelInitializer.InitNumberListLevel(list, 7, NumberStyle.LowercaseLetter, "\x0007.",
                ListLevelAlignment.Left, 5760, 5760, -360);
            ListLevelInitializer.InitNumberListLevel(list, 8, NumberStyle.LowercaseRoman, "\x0008.",
                ListLevelAlignment.Right, 6480, 6480, -180);
            return list;
        }

        /// <summary>
        /// Creates a list and a list definition based on a default numbered list,
        /// but hwith a customized first level. This is to match MS Word list templates.
        /// </summary>
        private static List CreateNumberDefault(
            ListCollection lists,
            NumberStyle firstLevelNumberStyle,
            string firstLevelNumberFormat,
            ListLevelAlignment firstLevelAlignment)
        {
            List list = CreateNumberDefault(lists);
            list.ListLevels[0].NumberStyle = firstLevelNumberStyle;
            list.ListLevels[0].NumberFormat = firstLevelNumberFormat;
            list.ListLevels[0].Alignment = firstLevelAlignment;
            return list;
        }

        /// <summary>
        /// Creates the defualt outline numbered list.
        /// </summary>
        private static List CreateOutlineNumbers(ListCollection lists)
        {
            List list = lists.AddEmpty(ListType.MultiLevel);
            ListLevelInitializer.InitNumberListLevel(list, 0, NumberStyle.Arabic, "\x0000)",
                ListLevelAlignment.Left, 360, 360, -360);
            ListLevelInitializer.InitNumberListLevel(list, 1, NumberStyle.LowercaseLetter, "\x0001)",
                ListLevelAlignment.Left, 720, 720, -360);
            ListLevelInitializer.InitNumberListLevel(list, 2, NumberStyle.LowercaseRoman, "\x0002)",
                ListLevelAlignment.Left, 1080, 1080, -360);
            ListLevelInitializer.InitNumberListLevel(list, 3, NumberStyle.Arabic, "(\x0003)",
                ListLevelAlignment.Left, 1440, 1440, -360);
            ListLevelInitializer.InitNumberListLevel(list, 4, NumberStyle.LowercaseLetter, "(\x0004)",
                ListLevelAlignment.Left, 1800, 1800, -360);
            ListLevelInitializer.InitNumberListLevel(list, 5, NumberStyle.LowercaseRoman, "(\x0005)",
                ListLevelAlignment.Left, 2160, 2160, -360);
            ListLevelInitializer.InitNumberListLevel(list, 6, NumberStyle.Arabic, "\x0006.",
                ListLevelAlignment.Left, 2520, 2520, -360);
            ListLevelInitializer.InitNumberListLevel(list, 7, NumberStyle.LowercaseLetter, "\x0007.",
                ListLevelAlignment.Left, 2880, 2880, -360);
            ListLevelInitializer.InitNumberListLevel(list, 8, NumberStyle.LowercaseRoman, "\x0008.",
                ListLevelAlignment.Left, 3240, 3240, -360);
            return list;
        }

        /// <summary>
        /// Creates a legal numbered outline list.
        /// </summary>
        private static List CreateOutlineLegal(ListCollection lists)
        {
            List list = lists.AddEmpty(ListType.MultiLevel);
            ListLevelInitializer.InitNumberListLevel(list, 0, NumberStyle.Arabic, 
                "\x0000.",
                ListLevelAlignment.Left, 360, 360, -360);
            ListLevelInitializer.InitNumberListLevel(list, 1, NumberStyle.Arabic, 
                "\x0000.\x0001.",
                ListLevelAlignment.Left, 792, 792, -432);
            ListLevelInitializer.InitNumberListLevel(list, 2, NumberStyle.Arabic, 
                "\x0000.\x0001.\x0002.",
                ListLevelAlignment.Left, 1440, 1224, -504);
            ListLevelInitializer.InitNumberListLevel(list, 3, NumberStyle.Arabic, 
                "\x0000.\x0001.\x0002.\x0003.",
                ListLevelAlignment.Left, 1800, 1728, -648);
            ListLevelInitializer.InitNumberListLevel(list, 4, NumberStyle.Arabic, 
                "\x0000.\x0001.\x0002.\x0003.\x0004.",
                ListLevelAlignment.Left, 2520, 2232, -792);
            ListLevelInitializer.InitNumberListLevel(list, 5, NumberStyle.Arabic, 
                "\x0000.\x0001.\x0002.\x0003.\x0004.\x0005.",
                ListLevelAlignment.Left, 2880, 2736, -936);
            ListLevelInitializer.InitNumberListLevel(list, 6, NumberStyle.Arabic, 
                "\x0000.\x0001.\x0002.\x0003.\x0004.\x0005.\x0006.",
                ListLevelAlignment.Left, 3600, 3240, -1080);
            ListLevelInitializer.InitNumberListLevel(list, 7, NumberStyle.Arabic, 
                "\x0000.\x0001.\x0002.\x0003.\x0004.\x0005.\x0006.\x0007.",
                ListLevelAlignment.Left, 3960, 3744, -1224);
            ListLevelInitializer.InitNumberListLevel(list, 8, NumberStyle.Arabic, 
                "\x0000.\x0001.\x0002.\x0003.\x0004.\x0005.\x0006.\x0007.\x0008.",
                ListLevelAlignment.Left, 4680, 4320, -1440);
            return list;
        }

        /// <summary>
        /// Creates the outline numbered list with different bullets for the levels.
        /// </summary>
        private static List CreateOutlineBullets(ListCollection lists)
        {
            List list = lists.AddEmpty(ListType.MultiLevel);
            ListLevelInitializer.InitBulletListLevel(list, 0, WingdingsFont, WingdingDiamonds, 360, 360, -360);
            ListLevelInitializer.InitBulletListLevel(list, 1, WingdingsFont, WingdingArrowHead, 720, 720, -360);
            ListLevelInitializer.InitBulletListLevel(list, 2, WingdingsFont, WingdingSquare, 1080, 1080, -360);
            ListLevelInitializer.InitBulletListLevel(list, 3, SymbolFont, SymbolDisc, 1440, 1440, -360);
            ListLevelInitializer.InitBulletListLevel(list, 4, SymbolFont, SymbolDiamond, 1800, 1800, -360);
            ListLevelInitializer.InitBulletListLevel(list, 5, WingdingsFont, WingdingArrowHead, 2160, 2160, -360);
            ListLevelInitializer.InitBulletListLevel(list, 6, WingdingsFont, WingdingSquare, 2520, 2520, -360);
            ListLevelInitializer.InitBulletListLevel(list, 7, SymbolFont, SymbolDisc, 2880, 2880, -360);
            ListLevelInitializer.InitBulletListLevel(list, 8, SymbolFont, SymbolDiamond, 3240, 3240, -360);
            return list;
        }

        /// <summary>
        /// Creates the Article \ Section outline headings list.
        /// </summary>
        private static List CreateOutlineHeadingsArticleSection(ListCollection lists)
        {
            List list = lists.AddEmpty(ListType.MultiLevel);
            ListLevelInitializer.InitNumberListLevel(list, 0, NumberStyle.UppercaseRoman, "Article \x0000.",
                ListLevelAlignment.Left, 1800, 0, -0, StyleIndex.Heading1, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 1, NumberStyle.LeadingZero, "Section \x0000.\x0001",
                ListLevelAlignment.Left, 1800, 0, -0, StyleIndex.Heading2, ListTrailingCharacter.Tab);
            // This is the only level that is set to legal numbering.
            list.ListLevels[1].IsLegal = true;

            ListLevelInitializer.InitNumberListLevel(list, 2, NumberStyle.LowercaseLetter, "(\x0002)",
                ListLevelAlignment.Left, 720, 720, -432, StyleIndex.Heading3, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 3, NumberStyle.LowercaseRoman, "(\x0003)",
                ListLevelAlignment.Right, 864, 864, -144, StyleIndex.Heading4, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 4, NumberStyle.Arabic, "\x0004)",
                ListLevelAlignment.Left, 1008, 1008, -432, StyleIndex.Heading5, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 5, NumberStyle.LowercaseLetter, "\x0005)",
                ListLevelAlignment.Left, 1152, 1152, -432, StyleIndex.Heading6, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 6, NumberStyle.LowercaseRoman, "\x0006)",
                ListLevelAlignment.Right, 1296, 1296, -288, StyleIndex.Heading7, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 7, NumberStyle.LowercaseLetter, "\x0007.",
                ListLevelAlignment.Left, 1440, 1440, -432, StyleIndex.Heading8, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 8, NumberStyle.LowercaseRoman, "\x0008.",
                ListLevelAlignment.Right, 1584, 1584, -144, StyleIndex.Heading9, ListTrailingCharacter.Tab);
            return list;
        }

        private static List CreateOutlineHeadingsLegal(ListCollection lists)
        {
            List list = lists.AddEmpty(ListType.MultiLevel);
            ListLevelInitializer.InitNumberListLevel(list, 0, NumberStyle.Arabic, 
                "\x0000",
                ListLevelAlignment.Left, 432, 432, -432, StyleIndex.Heading1, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 1, NumberStyle.Arabic, 
                "\x0000.\x0001",
                ListLevelAlignment.Left, 576, 576, -576, StyleIndex.Heading2, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 2, NumberStyle.Arabic, 
                "\x0000.\x0001.\x0002",
                ListLevelAlignment.Left, 720, 720, -720, StyleIndex.Heading3, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 3, NumberStyle.Arabic, 
                "\x0000.\x0001.\x0002.\x0003",
                ListLevelAlignment.Left, 864, 864, -864, StyleIndex.Heading4, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 4, NumberStyle.Arabic, 
                "\x0000.\x0001.\x0002.\x0003.\x0004",
                ListLevelAlignment.Left, 1008, 1008, -1008, StyleIndex.Heading5, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 5, NumberStyle.Arabic, 
                "\x0000.\x0001.\x0002.\x0003.\x0004.\x0005",
                ListLevelAlignment.Left, 1152, 1152, -1152, StyleIndex.Heading6, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 6, NumberStyle.Arabic, 
                "\x0000.\x0001.\x0002.\x0003.\x0004.\x0005.\x0006",
                ListLevelAlignment.Left, 1296, 1296, -1296, StyleIndex.Heading7, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 7, NumberStyle.Arabic, 
                "\x0000.\x0001.\x0002.\x0003.\x0004.\x0005.\x0006.\x0007",
                ListLevelAlignment.Left, 1440, 1440, -1440, StyleIndex.Heading8, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 8, NumberStyle.Arabic, 
                "\x0000.\x0001.\x0002.\x0003.\x0004.\x0005.\x0006.\x0007.\x0008",
                ListLevelAlignment.Left, 1584, 1584, -1584, StyleIndex.Heading9, ListTrailingCharacter.Tab);
            return list;
        }

        private static List CreateOutlineHeadingsNumbers(ListCollection lists)
        {
            List list = lists.AddEmpty(ListType.MultiLevel);
            ListLevelInitializer.InitNumberListLevel(list, 0, NumberStyle.UppercaseRoman, "\x0000.",
                ListLevelAlignment.Left, 360, 0, -0, StyleIndex.Heading1, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 1, NumberStyle.UppercaseLetter, "\x0001.",
                ListLevelAlignment.Left, 1080, 720, -0, StyleIndex.Heading2, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 2, NumberStyle.Arabic, "\x0002.",
                ListLevelAlignment.Left, 1800, 1440, -0, StyleIndex.Heading3, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 3, NumberStyle.LowercaseLetter, "\x0003)",
                ListLevelAlignment.Left, 2520, 2160, -0, StyleIndex.Heading4, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 4, NumberStyle.Arabic, "(\x0004)",
                ListLevelAlignment.Left, 3240, 2880, -0, StyleIndex.Heading5, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 5, NumberStyle.LowercaseLetter, "(\x0005)",
                ListLevelAlignment.Left, 3960, 3600, -0, StyleIndex.Heading6, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 6, NumberStyle.LowercaseRoman, "(\x0006)",
                ListLevelAlignment.Left, 4680, 4320, -0, StyleIndex.Heading7, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 7, NumberStyle.LowercaseLetter, "(\x0007)",
                ListLevelAlignment.Left, 5400, 5040, -0, StyleIndex.Heading8, ListTrailingCharacter.Tab);
            ListLevelInitializer.InitNumberListLevel(list, 8, NumberStyle.LowercaseRoman, "(\x0008)",
                ListLevelAlignment.Left, 6120, 5760, -0, StyleIndex.Heading9, ListTrailingCharacter.Tab);
            return list;
        }

        private static List CreateOutlineHeadingsChapter(ListCollection lists)
        {
            List list = lists.AddEmpty(ListType.MultiLevel);
            ListLevelInitializer.InitNumberListLevel(list, 0, NumberStyle.Arabic, "Chapter \x0000",
                ListLevelAlignment.Left, 0, 0, -0, StyleIndex.Heading1, ListTrailingCharacter.Space);
            ListLevelInitializer.InitNumberListLevel(list, 1, NumberStyle.None, "",
                ListLevelAlignment.Left, 0, 0, -0, StyleIndex.Heading2, ListTrailingCharacter.Nothing);
            ListLevelInitializer.InitNumberListLevel(list, 2, NumberStyle.None, "",
                ListLevelAlignment.Left, 0, 0, -0, StyleIndex.Heading3, ListTrailingCharacter.Nothing);
            ListLevelInitializer.InitNumberListLevel(list, 3, NumberStyle.None, "",
                ListLevelAlignment.Left, 0, 0, -0, StyleIndex.Heading4, ListTrailingCharacter.Nothing);
            ListLevelInitializer.InitNumberListLevel(list, 4, NumberStyle.None, "",
                ListLevelAlignment.Left, 0, 0, -0, StyleIndex.Heading5, ListTrailingCharacter.Nothing);
            ListLevelInitializer.InitNumberListLevel(list, 5, NumberStyle.None, "",
                ListLevelAlignment.Left, 0, 0, -0, StyleIndex.Heading6, ListTrailingCharacter.Nothing);
            ListLevelInitializer.InitNumberListLevel(list, 6, NumberStyle.None, "",
                ListLevelAlignment.Left, 0, 0, -0, StyleIndex.Heading7, ListTrailingCharacter.Nothing);
            ListLevelInitializer.InitNumberListLevel(list, 7, NumberStyle.None, "",
                ListLevelAlignment.Left, 0, 0, -0, StyleIndex.Heading8, ListTrailingCharacter.Nothing);
            ListLevelInitializer.InitNumberListLevel(list, 8, NumberStyle.None, "",
                ListLevelAlignment.Left, 0, 0, -0, StyleIndex.Heading9, ListTrailingCharacter.Nothing);
            return list;
        }

        /// <summary>
        /// Constants for standard list bullets and their fonts (also standard).
        /// </summary>
        internal const string SymbolFont = "Symbol";

        internal const char SymbolDiscChar = '\xf0b7';
        internal const string SymbolDisc = "\xf0b7"; // == SymbolDiscChar.ToString()

        internal const char SymbolDiamondChar = '\xf0a8';
        internal const string SymbolDiamond = "\xf0a8"; // == SymbolDiamondChar.ToString()

        internal const string CourierFont = "Courier New";

        internal const char CourierCircleChar = 'o';
        internal const string CourierCircle = "o"; // == CourierCircleChar.ToString()

        internal const string WingdingsFont = "Wingdings";

        internal const char WingdingSquareChar = '\xf0a7';
        internal const string WingdingSquare = "\xf0a7"; // == WingdingSquareChar.ToString()

        internal const char WingdingDiamondsChar = '\xf076';
        internal const string WingdingDiamonds = "\xf076"; // == WingdingDiamondsChar.ToString()

        internal const char WingdingArrowHeadChar = '\xf0d8';
        internal const string WingdingArrowHead = "\xf0d8"; // == WingdingArrowHeadChar.ToString()

        internal const char WingdingTickChar = '\xf0fc';
        internal const string WingdingTick = "\xf0fc"; // == WingdingTickChar.ToString()
    }
}
