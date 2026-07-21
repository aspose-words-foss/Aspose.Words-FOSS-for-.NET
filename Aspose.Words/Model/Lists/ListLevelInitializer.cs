// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/02/2013 by Ivan Lyagin

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Provides utility methods to initialize <see cref="ListLevel"/> objects used by list factories.
    /// </summary>
    /// <dev>
    /// This class is a product of <see cref="ListFactory"/> class refactoring. All of the source code 
    /// was left untouched while moving.
    /// </dev>
    internal static class ListLevelInitializer
    {
        /// <summary>
        /// Fills a list level of a bulleted list.
        /// </summary>
        internal static void InitBulletListLevel(
            List list,
            int levelNumber,
            string fontName,
            string bullet,
            int tabPosition,
            int leftIndent,
            int firstLineIndent)
        {
            InitBulletListLevel(list.ListLevels[levelNumber], levelNumber, fontName, bullet,
                tabPosition, leftIndent, firstLineIndent);
        }

        /// <summary>
        /// Fills a list level of a bulleted list.
        /// </summary>
        internal static void InitBulletListLevel(
            ListLevel level,
            int levelNumber,
            string fontName,
            string bullet,
            int tabPosition,
            int leftIndent,
            int firstLineIndent)
        {
            //Set list level properties
            level.StartAt = 1;
            level.NumberStyle = NumberStyle.Bullet;
            level.NumberFormat = bullet;
            level.Alignment = ListLevelAlignment.Left;
            level.TrailingCharacter = ListTrailingCharacter.Tab;
            level.LegacySpace = -firstLineIndent;
            level.LegacyPrev = true;
            level.ParaStyleIstd = StyleIndex.Nil;
            level.RestartAfterLevel = levelNumber - 1;

            //Set list level paragraph properties
            level.ParaPr.LeftIndent = leftIndent;
            level.ParaPr.FirstLineIndent = firstLineIndent;

            TabStopCollection tabStops = new TabStopCollection();
            tabStops.HasTolerances = true;    //This is what MS Word does.
            level.ParaPr.TabStops = tabStops;

            TabStop tabStop = new TabStop(tabPosition, TabAlignment.List, TabLeader.None);
            tabStops.Add(tabStop);

            //Set list level font properties
            level.RunPr.SetSymbolFonts(fontName);
        }

        /// <summary>
        /// Fills a list level of a numbered list.
        /// </summary>
        internal static void InitNumberListLevel(
            List list,
            int levelNumber,
            NumberStyle numberStyle,
            string numberFormat,
            ListLevelAlignment alignment,
            int tabPosition,
            int leftIndent,
            int firstLineIndent)
        {
            InitNumberListLevel(list, levelNumber, numberStyle, numberFormat,
                alignment, tabPosition, leftIndent, firstLineIndent, StyleIndex.Nil, ListTrailingCharacter.Tab);
        }

        /// <summary>
        /// Fills a list level of a numbered list.
        /// </summary>
        internal static void InitNumberListLevel(
            List list,
            int levelNumber,
            NumberStyle numberStyle,
            string numberFormat,
            ListLevelAlignment alignment,
            int tabPosition,
            int leftIndent,
            int firstLineIndent,
            int linkedParagraphStyleIstd,
            ListTrailingCharacter trailingCharacter)
        {
            InitNumberListLevel(list.ListLevels[levelNumber], levelNumber, numberStyle, numberFormat,
                alignment, tabPosition, leftIndent, firstLineIndent, linkedParagraphStyleIstd, trailingCharacter);
        }

        /// <summary>
        /// Fills a list level of a numbered list.
        /// </summary>
        internal static void InitNumberListLevel(
            ListLevel level,
            int levelNumber,
            NumberStyle numberStyle,
            string numberFormat,
            ListLevelAlignment alignment,
            int tabPosition,
            int leftIndent,
            int firstLineIndent,
            int linkedParagraphStyleIstd,
            ListTrailingCharacter trailingCharacter)
        {
            InitNumberListLevel(level, levelNumber, 1, numberStyle, numberFormat,
                alignment, tabPosition, leftIndent, firstLineIndent, linkedParagraphStyleIstd, trailingCharacter);
        }

        /// <summary>
        /// Fills a list level of a numbered list.
        /// </summary>
        internal static void InitNumberListLevel(
            ListLevel level,
            int levelNumber,
            int startAt,
            NumberStyle numberStyle,
            string numberFormat,
            ListLevelAlignment alignment,
            int tabPosition,
            int leftIndent,
            int firstLineIndent,
            int linkedParagraphStyleIstd,
            ListTrailingCharacter trailingCharacter)
        {
            level.StartAt = startAt;
            level.NumberStyle = numberStyle;
            level.NumberFormat = numberFormat;
            level.Alignment = alignment;
            level.TrailingCharacter = trailingCharacter;
            level.LegacySpace = -firstLineIndent;
            level.LegacyPrev = true;
            level.ParaStyleIstd = linkedParagraphStyleIstd;
            level.RestartAfterLevel = levelNumber - 1;

            //Set list level paragraph properties
            level.ParaPr.LeftIndent = leftIndent;
            level.ParaPr.FirstLineIndent = firstLineIndent;

            TabStopCollection tabStops = new TabStopCollection();
            tabStops.HasTolerances = true;    //This is what MS Word does.
            level.ParaPr.TabStops = tabStops;

            if (trailingCharacter == ListTrailingCharacter.Tab)
            {
                TabStop tabStop = new TabStop(tabPosition, TabAlignment.List, TabLeader.None);
                tabStops.Add(tabStop);
            }
        }

        /// <summary>
        /// Fills a list level with None number style.
        /// </summary>
        internal static void InitNoneListLevel(ListLevel level, int levelNumber)
        {
            level.StartAt = 0;
            level.NumberStyle = NumberStyle.None;
            level.NumberFormat = "";
            level.Alignment = ListLevelAlignment.Left;
            level.TrailingCharacter = ListTrailingCharacter.Tab;

            level.IsLegal = false;
            level.LegacySpace = 0;
            level.LegacyIndent = 0;
            level.LegacyPrev = false;
            level.IsTentative = false;
            level.LegacyPrevSpace = false;
            level.RestartAfterLevel = levelNumber - 1;

            level.RunPr.Clear();

            level.ParaPr.Clear();

            TabStopCollection tabStops = new TabStopCollection();
            level.ParaPr.TabStops = tabStops;

            TabStop tabStop = new TabStop(360, TabAlignment.List, TabLeader.None);
            tabStops.Add(tabStop);
        }
    }
}
