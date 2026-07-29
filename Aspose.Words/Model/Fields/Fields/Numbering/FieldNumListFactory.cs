// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/02/2013 by Ivan Lyagin

using System;
using Aspose.Words.Lists;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Builds lists used to maintain LISTNUM and AUTONUM fields' functionality.
    /// </summary>
    /// <remarks>
    /// Created lists are not persisted in a document. They are designed to be used within
    /// <see cref="FieldNumListLabelUpdater"/> only.
    /// </remarks>
    internal static class FieldNumListFactory
    {
        /// <summary>
        /// Creates a list used to maintain LISTNUM field functionality.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="listDef"></param>
        /// <param name="listType"></param>
        /// <returns></returns>
        internal static List CreateListNumList(DocumentBase document, ListDef listDef, FieldNumListType listType)
        {
            List list = CreateFieldNumList(document, listDef, ListNumListId);
            switch (listType)
            {
                case FieldNumListType.Number:
                    InitListNumNumberList(list);
                    break;
                case FieldNumListType.Outline:
                    // Use ListDef level definitions.
                    break;
                case FieldNumListType.Legal:
                    InitListNumLegalList(list);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("listType");
            }

            return list;
        }

        /// <summary>
        /// Creates a list used to maintain AUTONUM field functionality.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="listDef"></param>
        /// <param name="listType"></param>
        /// <param name="separator"></param>
        /// <param name="hasTrailingSeparator"></param>
        /// <param name="numberStyle"></param>
        /// <returns></returns>
        internal static List CreateAutoNumList(DocumentBase document, ListDef listDef, FieldNumListType listType, char separator, bool hasTrailingSeparator, NumberStyle numberStyle)
        {
            List list = CreateFieldNumList(document, listDef, AutoNumListId);
            switch (listType)
            {
                case FieldNumListType.Number:
                    InitAutoNumNumberList(list, separator, numberStyle);
                    break;
                case FieldNumListType.Outline:
                    // Use ListDef level definitions.
                    break;
                case FieldNumListType.Legal:
                    InitAutoNumLegalList(list, separator, hasTrailingSeparator, numberStyle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("listType");
            }

            return list;
        }

        /// <summary>
        /// Creates a list used to maintain LISTNUM or AUTONUM field functionality.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="listDef"></param>
        /// <param name="listId"></param>
        /// <returns></returns>
        private static List CreateFieldNumList(DocumentBase document, ListDef listDef, int listId)
        {
            if (listDef != null)
                return new List(listDef, listId);

            // Create a ListDef instance if a null reference is passed.
            listDef = new ListDef(document, FieldNumListDefId, ListType.MultiLevel, 0);
            List list = new List(listDef, listId);

            // Outline numbering format is the same for all of the LISTNUM and AUTONUM fields,
            // so let's use it to define ListDef levels.
            InitFieldNumListLevel(list, 0, NumberStyle.UppercaseRoman, "\x0000.", false);
            InitFieldNumListLevel(list, 1, NumberStyle.UppercaseLetter, "\x0001.", false);
            InitFieldNumListLevel(list, 2, NumberStyle.Arabic, "\x0002.", false);
            InitFieldNumListLevel(list, 3, NumberStyle.LowercaseLetter, "\x0003)", false);
            InitFieldNumListLevel(list, 4, NumberStyle.Arabic, "(\x0004)", false);
            InitFieldNumListLevel(list, 5, NumberStyle.LowercaseLetter, "(\x0005)", false);
            InitFieldNumListLevel(list, 6, NumberStyle.LowercaseRoman, "(\x0006)", false);
            InitFieldNumListLevel(list, 7, NumberStyle.LowercaseLetter, "(\x0007)", false);
            InitFieldNumListLevel(list, 8, NumberStyle.LowercaseRoman, "(\x0008)", false);

            return list;
        }

        /// <summary>
        /// Fills list levels for LISTNUM field's NumberDefault list.
        /// </summary>
        /// <param name="list"></param>
        private static void InitListNumNumberList(List list)
        {
            InitFieldNumListLevel(list, 0, NumberStyle.Arabic, "\x0000)");
            InitFieldNumListLevel(list, 1, NumberStyle.LowercaseLetter, "\x0001)");
            InitFieldNumListLevel(list, 2, NumberStyle.LowercaseRoman, "\x0002)");
            InitFieldNumListLevel(list, 3, NumberStyle.Arabic, "(\x0003)");
            InitFieldNumListLevel(list, 4, NumberStyle.LowercaseLetter, "(\x0004)");
            InitFieldNumListLevel(list, 5, NumberStyle.LowercaseRoman, "(\x0005)");
            InitFieldNumListLevel(list, 6, NumberStyle.Arabic, "\x0006.");
            InitFieldNumListLevel(list, 7, NumberStyle.LowercaseLetter, "\x0007.");
            InitFieldNumListLevel(list, 8, NumberStyle.LowercaseRoman, "\x0008.");
        }

        /// <summary>
        /// Fills list levels for LISTNUM field's LegalDefault list.
        /// </summary>
        /// <param name="list"></param>
        private static void InitListNumLegalList(List list)
        {
            InitFieldNumListLevel(list, 0, NumberStyle.Arabic, "\x0000.");
            InitFieldNumListLevel(list, 1, NumberStyle.Arabic, "\x0000.\x0001.");
            InitFieldNumListLevel(list, 2, NumberStyle.Arabic, "\x0000.\x0001.\x0002.");
            InitFieldNumListLevel(list, 3, NumberStyle.Arabic, "\x0000.\x0001.\x0002.\x0003.");
            InitFieldNumListLevel(list, 4, NumberStyle.Arabic, "\x0000.\x0001.\x0002.\x0003.\x0004.");
            InitFieldNumListLevel(list, 5, NumberStyle.Arabic, "\x0000.\x0001.\x0002.\x0003.\x0004.\x0005.");
            InitFieldNumListLevel(list, 6, NumberStyle.Arabic, "\x0000.\x0001.\x0002.\x0003.\x0004.\x0005.\x0006.");
            InitFieldNumListLevel(list, 7, NumberStyle.Arabic, "\x0000.\x0001.\x0002.\x0003.\x0004.\x0005.\x0006.\x0007.");
            InitFieldNumListLevel(list, 8, NumberStyle.Arabic, "\x0000.\x0001.\x0002.\x0003.\x0004.\x0005.\x0006.\x0007.\x0008.");
        }

        /// <summary>
        /// Fills list levels for AUTONUM field's list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="separator"></param>
        /// <param name="numberStyle"></param>
        private static void InitAutoNumNumberList(List list, char separator, NumberStyle numberStyle)
        {
            InitFieldNumListLevel(list, 0, numberStyle, "\x0000" + separator);
            InitFieldNumListLevel(list, 1, numberStyle, "\x0001" + separator);
            InitFieldNumListLevel(list, 2, numberStyle, "\x0002" + separator);
            InitFieldNumListLevel(list, 3, numberStyle, "\x0003" + separator);
            InitFieldNumListLevel(list, 4, numberStyle, "\x0004" + separator);
            InitFieldNumListLevel(list, 5, numberStyle, "\x0005" + separator);
            InitFieldNumListLevel(list, 6, numberStyle, "\x0006" + separator);
            InitFieldNumListLevel(list, 7, numberStyle, "\x0007" + separator);
            InitFieldNumListLevel(list, 8, numberStyle, "\x0008" + separator);
        }

        /// <summary>
        /// Fills list levels for AUTONUMLGL field's list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="separator"></param>
        /// <param name="hasTrailingSeparator"></param>
        /// <param name="numberStyle"></param>
        private static void InitAutoNumLegalList(List list, char separator, bool hasTrailingSeparator, NumberStyle numberStyle)
        {
            // Use the single boxed separator object.
            object boxedSeparator = separator;
            object boxedTrailingSeparator = hasTrailingSeparator ? boxedSeparator : null;

            InitFieldNumListLevel(list, 0, numberStyle,
                string.Format("\x0000{0}",
                    boxedTrailingSeparator));
            InitFieldNumListLevel(list, 1, numberStyle,
                string.Format("\x0000{0}\x0001{1}",
                    boxedSeparator, boxedTrailingSeparator));
            InitFieldNumListLevel(list, 2, numberStyle,
                string.Format("\x0000{0}\x0001{0}\x0002{1}",
                    boxedSeparator, boxedTrailingSeparator));
            InitFieldNumListLevel(list, 3, numberStyle,
                string.Format("\x0000{0}\x0001{0}\x0002{0}\x0003{1}",
                    boxedSeparator, boxedTrailingSeparator));
            InitFieldNumListLevel(list, 4, numberStyle,
                string.Format("\x0000{0}\x0001{0}\x0002{0}\x0003{0}\x0004{1}",
                    boxedSeparator, boxedTrailingSeparator));
            InitFieldNumListLevel(list, 5, numberStyle,
                string.Format("\x0000{0}\x0001{0}\x0002{0}\x0003{0}\x0004{0}\x0005{1}",
                    boxedSeparator, boxedTrailingSeparator));
            InitFieldNumListLevel(list, 6, numberStyle,
                string.Format("\x0000{0}\x0001{0}\x0002{0}\x0003{0}\x0004{0}\x0005{0}\x0006{1}",
                    boxedSeparator, boxedTrailingSeparator));
            InitFieldNumListLevel(list, 7, numberStyle,
                string.Format("\x0000{0}\x0001{0}\x0002{0}\x0003{0}\x0004{0}\x0005{0}\x0006{0}\x0007{1}",
                    boxedSeparator, boxedTrailingSeparator));
            InitFieldNumListLevel(list, 8, numberStyle,
                string.Format("\x0000{0}\x0001{0}\x0002{0}\x0003{0}\x0004{0}\x0005{0}\x0006{0}\x0007{0}\x0008{1}",
                    boxedSeparator, boxedTrailingSeparator));
        }

        /// <summary>
        /// Fills a list level of a list used to maintain LISTNUM or AUTONUM field functionality.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="levelNumber"></param>
        /// <param name="numberStyle"></param>
        /// <param name="numberFormat"></param>
        private static void InitFieldNumListLevel(List list, int levelNumber, NumberStyle numberStyle, string numberFormat)
        {
            InitFieldNumListLevel(list, levelNumber, numberStyle, numberFormat, true);
        }

        /// <summary>
        /// Fills a list level of a list used to maintain LISTNUM or AUTONUM field functionality.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="levelNumber"></param>
        /// <param name="numberStyle"></param>
        /// <param name="numberFormat"></param>
        /// <param name="useLevelOverride"></param>
        private static void InitFieldNumListLevel(
            List list,
            int levelNumber,
            NumberStyle numberStyle,
            string numberFormat,
            bool useLevelOverride)
        {
            ListLevel level;
            if (useLevelOverride)
            {
                ListLevelOverride levelOverride = new ListLevelOverride(list.Document, levelNumber);
                levelOverride.IsFormatting = true;
                list.Overrides.Add(levelOverride);

                level = levelOverride.ListLevel;
            }
            else
            {
                level = list.ListLevels[levelNumber];
            }

            ListLevelInitializer.InitNumberListLevel(level, levelNumber, numberStyle, numberFormat,
                ListLevelAlignment.Left, 0, 0, 0, StyleIndex.Nil, ListTrailingCharacter.Nothing);
        }

        /// <summary>
        /// Represents the value of an identifier for a <see cref="ListDef"/> instance used to maintain
        /// LISTNUM and AUTONUM fields functionality.
        /// </summary>
        /// <remarks>
        /// This identifier must be out of a normal <see cref="ListDef"/> identifier range.
        /// According to MS-OI29500 (see http://msdn.microsoft.com/en-us/library/ff530966(v=office.12).aspx),
        /// DOC format specification (see http://msdn.microsoft.com/en-us/library/dd907589(v=office.12).aspx) and
        /// RTF format specification (see http://msdn.microsoft.com/en-us/library/office/aa140301(v=office.12).aspx)
        /// the value of -1 can be surely used for this purpose.
        /// WORDSNET-1767 ensures this (see <see cref="ListCollection.GetNextAvailableListDefId()"/>).
        /// </remarks>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int FieldNumListDefId = -1;
        /// <summary>
        /// Represents the value of an identifier for a <see cref="List"/> instance used to maintain
        /// LISTNUM field functionality.
        /// </summary>
        /// <remarks>
        /// The value could be any as it does not represent a real list in a document.
        /// </remarks>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int ListNumListId = 0;
        /// <summary>
        /// Represents the value of an identifier for a <see cref="List"/> instance used to maintain
        /// AUTONUM field functionality.
        /// </summary>
        /// <remarks>
        /// The value could be any as it does not represent a real list in a document.
        /// </remarks>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int AutoNumListId = 1;
    }
}
