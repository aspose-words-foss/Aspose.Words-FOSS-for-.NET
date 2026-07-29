// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/10/2012 by Alexey Butalov

using System.Collections.Generic;
using Aspose.Words.Lists;

namespace Aspose.Words.RW.Html.HtmlList
{
    /// <summary>
    /// Represents a list-item marker in a list. 
    /// </summary>
    internal class HtmlListItemMarker
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="listStyleType">The type of the list-item marker in a HTML list.</param>
        /// <param name="isBullet">Specifies whether the list-item marker is a bullet.</param>
        /// <param name="listTypeAttribute">HTML UL or OL type attribute.</param>
        /// <param name="listTemplate">Corresponding predefined list format available in Microsoft Word.</param>
        /// <param name="numberStyle">Corresponding number style for a list.</param>
        /// <param name="bulletFormat">Bullet format for the list level.</param>
        /// <param name="bulletFontName">Bullet font name for the list level.</param>
        private HtmlListItemMarker(
            string listStyleType,
            bool isBullet,
            string listTypeAttribute,
            ListTemplate listTemplate,
            NumberStyle numberStyle,
            string bulletFormat,
            string bulletFontName)
        {
            ListStyleType = listStyleType;
            IsBullet = isBullet;
            ListTypeAttribute = listTypeAttribute;
            ListTemplate = listTemplate;
            NumberStyle = numberStyle;
            BulletFormat = bulletFormat;
            BulletFontName = bulletFontName;
        }

        /// <summary>
        /// Returns a list-item marker by a value of CSS list-style-type property.
        /// </summary>
        /// <param name="listStyleType">Value of CSS list-style-type property.</param>
        /// <returns>List-item marker, or null if a list style type is unknown.</returns>
        internal static HtmlListItemMarker GetFromListStyleType(string listStyleType)
        {
            foreach (HtmlListItemMarker listTypeDef in gRegisteredListTypeDefs)
            {
                if (StringUtil.EqualsIgnoreCase(listTypeDef.ListStyleType, listStyleType))
                    return listTypeDef;
            }
            return null;
        }

        /// <summary>
        /// Returns a list-item marker for a list tag with no type specified.
        /// </summary>
        /// <param name="listTag">List tag (UL or OL).</param>
        /// <param name="listLevelNumber">List level number.</param>
        /// <returns>List-item marker, or null if a list tag is unknown.</returns>
        internal static HtmlListItemMarker GetFromListTag(
            string listTag,
            int listLevelNumber)
        {
            switch (listTag)
            {
                case "ol":
                    return gDecimal;
                case "ul":
                    return GetBulletMarkerFromListLevel(listLevelNumber);
                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        /// <summary>
        /// Returns a list-item marker for a specified list level.
        /// </summary>
        internal static HtmlListItemMarker GetFromListLevel(ListLevel level)
        {
            HtmlListItemMarker listItemMarker = GetBulletMarkerFromListLevel(level);
            if (listItemMarker != null)
                return listItemMarker;

            switch (level.NumberStyle)
            {
                case NumberStyle.Arabic:
                case NumberStyle.Number:
                case NumberStyle.Ordinal:
                case NumberStyle.OrdinalText:
                    return gDecimal;
                case NumberStyle.LeadingZero:
                    return gDecimalLeadingZero;
                case NumberStyle.LowercaseLetter:
                    return gLowerLatin;
                case NumberStyle.LowercaseRoman:
                    return gLowerRoman;
                case NumberStyle.UppercaseLetter:
                    return gUpperLatin;
                case NumberStyle.UppercaseRoman:
                    return gUpperRoman;
                default:
                    return gDecimal;
            }

            // These ones have no counterparts in the model:
            // lower-greek | armenian | georgian

            // These are the same as lower-latin and upper-latin
            // lower-alpha | upper-alpha
        }

        internal bool Equals(HtmlListItemMarker other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return (IsBullet == other.IsBullet) &&
                   (ListTypeAttribute == other.ListTypeAttribute) &&
                   (ListTemplate == other.ListTemplate) &&
                   (NumberStyle == other.NumberStyle) &&
                   (ListStyleType == other.ListStyleType) &&
                   StringUtil.EqualsIgnoreCase(BulletFormat, other.BulletFormat) &&
                   StringUtil.EqualsIgnoreCase(BulletFontName, other.BulletFontName);
        }

        /// <summary>
        /// Returns list-item bullet marker for specified list level number.
        /// </summary>
        private static HtmlListItemMarker GetBulletMarkerFromListLevel(int listLevelNumber)
        {
            // Every three levels the bullets repeat so I normalize to [0..2] range.
            // In the future we'll hopefully provide support for all bullets
            // (either drawing them as images or anyhow else).
            switch (listLevelNumber % 3)
            {
                case 0:
                    return gDisk;
                case 1:
                    return gCircle;
                case 2:
                    return gSquare;
                default:
                    Debug.Fail("Invalid value.");
                    return null;
            }
        }

        /// <summary>
        /// Returns list-item bullet marker if a list level should be output with bullets, otherwise null.
        /// </summary>
        private static HtmlListItemMarker GetBulletMarkerFromListLevel(ListLevel level)
        {
            // Check against standard HTML bullets first (#3325): disc, circle, square.
            // For some bullet formats we know how to output HTML.
            // We should have been already checked the possibility to represent it as HTML native list (font etc.).
            switch (level.NumberFormat)
            {
                case ListFactory.SymbolDisc:
                    return gDisk;
                case ListFactory.CourierCircle:
                    return gCircle;
                case ListFactory.WingdingSquare:
                    return gSquare;
                default:
                    // There are no standard HTML counterparts for other number formats.
                    break;
            }

            if ((level.NumberStyle == NumberStyle.Bullet) || (level.NumberStyle == NumberStyle.None))
            {
                // Standard HTML bullets are already processed.
                // This is applied to all other bullets.
                return GetBulletMarkerFromListLevel(level.LevelNumber);
            }

            return null;
        }

        /// <summary>
        /// Creates a list-item bullet marker.
        /// </summary>
        /// <param name="listStyleType">The type of the list-item marker in a HTML list.</param>
        /// <param name="listTypeAttribute">HTML UL or OL type attribute.</param>
        /// <param name="listTemplate">Corresponding predefined list format available in Microsoft Word.</param>
        /// <param name="bulletFormat">Bullet format for the list level.</param>
        /// <param name="bulletFontName">Bullet font name for the list level.</param>
        private static HtmlListItemMarker CreateBulletMarker(
            string listStyleType,
            string listTypeAttribute,
            ListTemplate listTemplate,
            string bulletFormat,
            string bulletFontName)
        {
            HtmlListItemMarker listDef = new HtmlListItemMarker(
                listStyleType,
                true,
                listTypeAttribute,
                listTemplate,
                NumberStyle.Bullet,
                bulletFormat,
                bulletFontName);
            gRegisteredListTypeDefs.Add(listDef);
            return listDef;
        }

        /// <summary>
        /// Creates a list-item numbered marker.
        /// </summary>
        /// <param name="listStyleType">The type of the list-item marker in a HTML list.</param>
        /// <param name="listTypeAttribute">HTML UL or OL type attribute.</param>
        /// <param name="listTemplate">Corresponding predefined list format available in Microsoft Word.</param>
        /// <param name="numberStyle">Corresponding number style for a list.</param>
        private static HtmlListItemMarker CreateNumberedMarker(
            string listStyleType,
            string listTypeAttribute,
            ListTemplate listTemplate,
            NumberStyle numberStyle)
        {
            HtmlListItemMarker listDef = new HtmlListItemMarker(
                listStyleType,
                false,
                listTypeAttribute,
                listTemplate,
                numberStyle,
                string.Empty,
                string.Empty);
            gRegisteredListTypeDefs.Add(listDef);
            return listDef;
        }

        /// <summary>
        /// Specifies whether the list-item marker is a bullet.
        /// </summary>
        internal bool IsBullet { get; }

        /// <summary>
        /// Specifies the type of list-item marker in a HTML list.
        /// Corresponds to CSS 'list-style-type' property.
        /// </summary>
        internal string ListStyleType { get; }

        /// <summary>
        /// Specifies the kind of marker to use in the list.
        /// Corresponds to HTML UL or OL 'type' attribute.
        /// </summary>
        internal string ListTypeAttribute { get; }

        /// <summary>
        /// Predefined list format available in Microsoft Word.
        /// </summary>
        internal ListTemplate ListTemplate { get; }

        /// <summary>
        /// Number style for the list.
        /// </summary>
        internal NumberStyle NumberStyle { get; }

        /// <summary>
        /// Bullet format for the list level.
        /// </summary>
        internal string BulletFormat { get; }

        /// <summary>
        /// Bullet font name for the list level.
        /// </summary>
        internal string BulletFontName { get; }

        /// <summary>
        /// Represents HTML list with circle marker.
        /// </summary>
        private static readonly HtmlListItemMarker gCircle;
        /// <summary>
        /// Represents HTML list with filled circle marker.
        /// </summary>
        private static readonly HtmlListItemMarker gDisk;
        /// <summary>
        /// Represents HTML list with square marker.
        /// </summary>
        private static readonly HtmlListItemMarker gSquare;
        private static readonly HtmlListItemMarker gDecimal;
        private static readonly HtmlListItemMarker gDecimalLeadingZero;
        private static readonly HtmlListItemMarker gLowerLatin;
        private static readonly HtmlListItemMarker gLowerRoman;
        private static readonly HtmlListItemMarker gUpperLatin;
        private static readonly HtmlListItemMarker gUpperRoman;
        /// <summary>
        /// Array of list style types supported in HTML.
        /// </summary>
        private static readonly List<HtmlListItemMarker> gRegisteredListTypeDefs;

        static HtmlListItemMarker()
        {
            // Defines list style types supported in HTML.
            gRegisteredListTypeDefs = new List<HtmlListItemMarker>();

            gCircle = CreateBulletMarker(
                "circle",
                "circle",
                ListTemplate.BulletCircle,
                ListFactory.CourierCircle,
                ListFactory.CourierFont);
            gDisk = CreateBulletMarker(
                "disc",
                "disc",
                ListTemplate.BulletDisk,
                ListFactory.SymbolDisc,
                ListFactory.SymbolFont);
            gSquare = CreateBulletMarker(
                "square",
                "square",
                ListTemplate.BulletSquare,
                ListFactory.WingdingSquare,
                ListFactory.WingdingsFont);

            gDecimal = CreateNumberedMarker(
                "decimal",
                "1",
                ListTemplate.NumberArabicDot,
                NumberStyle.Arabic);
            gDecimalLeadingZero = CreateNumberedMarker(
                "decimal-leading-zero",
                "1",
                ListTemplate.NumberArabicDot,
                NumberStyle.LeadingZero);

            gLowerLatin = CreateNumberedMarker(
                "lower-latin",
                "a",
                ListTemplate.NumberLowercaseLetterDot,
                NumberStyle.LowercaseLetter);
            gUpperLatin = CreateNumberedMarker(
                "upper-latin",
                "A",
                ListTemplate.NumberUppercaseLetterDot,
                NumberStyle.UppercaseLetter);

            gLowerRoman = CreateNumberedMarker(
                "lower-roman",
                "i",
                ListTemplate.NumberLowercaseRomanDot,
                NumberStyle.LowercaseRoman);
            gUpperRoman = CreateNumberedMarker(
                "upper-roman",
                "I",
                ListTemplate.NumberUppercaseRomanDot,
                NumberStyle.UppercaseRoman);

            CreateNumberedMarker(
                "lower-alpha",
                "a",
                ListTemplate.NumberLowercaseLetterDot,
                NumberStyle.LowercaseLetter);
            CreateNumberedMarker(
                "upper-alpha",
                "A",
                ListTemplate.NumberUppercaseLetterDot,
                NumberStyle.UppercaseLetter);

            // {lower|upper}-greek isn't supported in AW.
            CreateNumberedMarker(
                "lower-greek",
                string.Empty,
                ListTemplate.NumberLowercaseLetterDot,
                NumberStyle.LowercaseLetter);
            CreateNumberedMarker(
                "upper-greek",
                string.Empty,
                ListTemplate.NumberUppercaseLetterDot,
                NumberStyle.UppercaseLetter);

            // WORDSNET-26977 Add support for "arabic-indic" list style.
            CreateNumberedMarker(
                "arabic-indic",
                string.Empty,
                ListTemplate.NumberArabicDot,
                NumberStyle.Arabic);
        }
    }
}
