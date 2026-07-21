// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/06/2006 by Vladimir Averkin

using Aspose.Collections;
using Aspose.Words.Fonts;
using Aspose.Words.Notes;

namespace Aspose.Words.RW.Nrx
{
    /// <summary>
    /// Converts miscellaneous enumerated types between enum and WML string.
    /// </summary>
    internal class WmlEnum
    {
        internal static FontFamily WmlToFontFamily(string value)
        {
            return (FontFamily)gFontFamilyMap.GetValue(value, (int)FontFamily.Auto);
        }

        internal static string FontFamilyToWml(FontFamily value)
        {
            return gFontFamilyMap.GetValue((int)value, "");
        }

        internal static NumberStyle WmlToNumberStyle(string value)
        {
            return (NumberStyle)gNumberStyleMap.GetValue(value, (int)NumberStyle.Arabic);
        }

        internal static string NumberStyleToWml(NumberStyle value)
        {
            return gNumberStyleMap.GetValue((int)value, "");
        }

        internal static LineStyle WmlToLineStyle(string value)
        {
            return (LineStyle)gLineStyleMap.GetValue(value, (int)LineStyle.None);
        }

        internal static string LineStyleToWml(LineStyle value)
        {
            switch (value)
            {
                    // MS Word converts this line style to 'none' in my test.
                    // I still think we should convert it to 'single'.
                case LineStyle.Hairline: return "single";
                default:
                    return gLineStyleMap.GetValue((int)value, "none");
            }
        }

        internal static StyleType WmlToStyleType(string value)
        {
            return (StyleType)gStyleTypeMap.GetValue(value, (int)StyleType.Paragraph);
        }

        internal static string StyleTypeToWml(StyleType value)
        {
            return gStyleTypeMap.GetValue((int)value, "paragraph");
        }

        internal static FootnoteSeparatorType WmlToFootnoteSeparatorType(string value, bool isEndnote)
        {
            StringToIntBidirectionalMap map = isEndnote ? gEndnoteSeparatorTypeMap : gFootnoteSeparatorTypeMap;
            return (FootnoteSeparatorType)map.GetValue(value, -1);
        }

        internal static string FootnoteSeparatorTypeToWml(FootnoteSeparatorType value, bool isEndnote)
        {
            StringToIntBidirectionalMap map = isEndnote ? gEndnoteSeparatorTypeMap : gFootnoteSeparatorTypeMap;
            return map.GetValue((int)value, "");
        }

        private static readonly StringToIntBidirectionalMap gFontFamilyMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gNumberStyleMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gLineStyleMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gStyleTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gFootnoteSeparatorTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gEndnoteSeparatorTypeMap = new StringToIntBidirectionalMap();

        static WmlEnum()
        {
            // It seems that all border styles that are available via Borders and Shading menu in MS Word 2003 are here.
            gLineStyleMap.AddEntry("none", (int)LineStyle.None);
            gLineStyleMap.AddEntry("single", (int)LineStyle.Single);
            gLineStyleMap.AddEntry("thick", (int)LineStyle.Thick);
            gLineStyleMap.AddEntry("double", (int)LineStyle.Double);
            gLineStyleMap.AddEntry("dotted", (int)LineStyle.Dot);
            gLineStyleMap.AddEntry("dashed", (int)LineStyle.DashLargeGap);
            gLineStyleMap.AddEntry("dot-dash", (int)LineStyle.DotDash);
            gLineStyleMap.AddEntry("dot-dot-dash", (int)LineStyle.DotDotDash);
            gLineStyleMap.AddEntry("triple", (int)LineStyle.Triple);
            gLineStyleMap.AddEntry("thin-thick-small-gap", (int)LineStyle.ThinThickSmallGap);
            gLineStyleMap.AddEntry("thick-thin-small-gap", (int)LineStyle.ThickThinSmallGap);
            gLineStyleMap.AddEntry("thin-thick-thin-small-gap", (int)LineStyle.ThinThickThinSmallGap);
            gLineStyleMap.AddEntry("thin-thick-medium-gap", (int)LineStyle.ThinThickMediumGap);
            gLineStyleMap.AddEntry("thick-thin-medium-gap", (int)LineStyle.ThickThinMediumGap);
            gLineStyleMap.AddEntry("thin-thick-thin-medium-gap", (int)LineStyle.ThinThickThinMediumGap);
            gLineStyleMap.AddEntry("thin-thick-large-gap", (int)LineStyle.ThinThickLargeGap);
            gLineStyleMap.AddEntry("thick-thin-large-gap", (int)LineStyle.ThickThinLargeGap);
            gLineStyleMap.AddEntry("thin-thick-thin-large-gap", (int)LineStyle.ThinThickThinLargeGap);
            gLineStyleMap.AddEntry("wave", (int)LineStyle.Wave);
            gLineStyleMap.AddEntry("double-wave", (int)LineStyle.DoubleWave);
            gLineStyleMap.AddEntry("dash-small-gap", (int)LineStyle.DashSmallGap);
            gLineStyleMap.AddEntry("dash-dot-stroked", (int)LineStyle.DashDotStroker);
            gLineStyleMap.AddEntry("three-d-emboss", (int)LineStyle.Emboss3D);
            gLineStyleMap.AddEntry("three-d-engrave", (int)LineStyle.Engrave3D);
            gLineStyleMap.AddEntry("outset", (int)LineStyle.Outset);
            gLineStyleMap.AddEntry("inset", (int)LineStyle.Inset);
            // page border art
            gLineStyleMap.AddEntry("apples", (int)PageBorderArt.Apples);
            gLineStyleMap.AddEntry("arched-scallops", (int)PageBorderArt.ArchedScallops);
            gLineStyleMap.AddEntry("baby-pacifier", (int)PageBorderArt.BabyPacifier);
            gLineStyleMap.AddEntry("baby-rattle", (int)PageBorderArt.BabyRattle);
            gLineStyleMap.AddEntry("balloons-3-colors", (int)PageBorderArt.Balloons3Colors);
            gLineStyleMap.AddEntry("balloons-hot-air", (int)PageBorderArt.BalloonsHotAir);
            gLineStyleMap.AddEntry("basic-black-dashes", (int)PageBorderArt.BasicBlackDashes);
            gLineStyleMap.AddEntry("basic-black-dots", (int)PageBorderArt.BasicBlackDots);
            gLineStyleMap.AddEntry("basic-black-squares", (int)PageBorderArt.BasicBlackSquares);
            gLineStyleMap.AddEntry("basic-thin-lines", (int)PageBorderArt.BasicThinLines);
            gLineStyleMap.AddEntry("basic-white-dashes", (int)PageBorderArt.BasicWhiteDashes);
            gLineStyleMap.AddEntry("basic-white-dots", (int)PageBorderArt.BasicWhiteDots);
            gLineStyleMap.AddEntry("basic-white-squares", (int)PageBorderArt.BasicWhiteSquares);
            gLineStyleMap.AddEntry("basic-wide-inline", (int)PageBorderArt.BasicWideInline);
            gLineStyleMap.AddEntry("basic-wide-midline", (int)PageBorderArt.BasicWideMidline);
            gLineStyleMap.AddEntry("basic-wide-outline", (int)PageBorderArt.BasicWideOutline);
            gLineStyleMap.AddEntry("bats", (int)PageBorderArt.Bats);
            gLineStyleMap.AddEntry("birds", (int)PageBorderArt.Birds);
            gLineStyleMap.AddEntry("birds-flight", (int)PageBorderArt.BirdsFlight);
            gLineStyleMap.AddEntry("cabins", (int)PageBorderArt.Cabins);
            gLineStyleMap.AddEntry("cake-slice", (int)PageBorderArt.CakeSlice);
            gLineStyleMap.AddEntry("candy-corn", (int)PageBorderArt.CandyCorn);
            gLineStyleMap.AddEntry("celtic-knotwork", (int)PageBorderArt.CelticKnotwork);
            gLineStyleMap.AddEntry("certificate-banner", (int)PageBorderArt.CertificateBanner);
            gLineStyleMap.AddEntry("chain-link", (int)PageBorderArt.ChainLink);
            gLineStyleMap.AddEntry("champagne-bottle", (int)PageBorderArt.ChampagneBottle);
            gLineStyleMap.AddEntry("checked-bar-black", (int)PageBorderArt.CheckedBarBlack);
            gLineStyleMap.AddEntry("checked-bar-color", (int)PageBorderArt.CheckedBarColor);
            gLineStyleMap.AddEntry("checkered", (int)PageBorderArt.Checkered);
            gLineStyleMap.AddEntry("christmas-tree", (int)PageBorderArt.ChristmasTree);
            gLineStyleMap.AddEntry("circles-lines", (int)PageBorderArt.CirclesLines);
            gLineStyleMap.AddEntry("circles-rectangles", (int)PageBorderArt.CirclesRectangles);
            gLineStyleMap.AddEntry("classical-wave", (int)PageBorderArt.ClassicalWave);
            gLineStyleMap.AddEntry("clocks", (int)PageBorderArt.Clocks);
            gLineStyleMap.AddEntry("compass", (int)PageBorderArt.Compass);
            gLineStyleMap.AddEntry("confetti", (int)PageBorderArt.Confetti);
            gLineStyleMap.AddEntry("confetti-grays", (int)PageBorderArt.ConfettiGrays);
            gLineStyleMap.AddEntry("confetti-outline", (int)PageBorderArt.ConfettiOutline);
            gLineStyleMap.AddEntry("confetti-streamers", (int)PageBorderArt.ConfettiStreamers);
            gLineStyleMap.AddEntry("confetti-white", (int)PageBorderArt.ConfettiWhite);
            gLineStyleMap.AddEntry("corner-triangles", (int)PageBorderArt.CornerTriangles);
            gLineStyleMap.AddEntry("coupon-cutout-dashes", (int)PageBorderArt.CouponCutoutDashes);
            gLineStyleMap.AddEntry("coupon-cutout-dots", (int)PageBorderArt.CouponCutoutDots);
            gLineStyleMap.AddEntry("crazy-maze", (int)PageBorderArt.CrazyMaze);
            gLineStyleMap.AddEntry("creatures-butterfly", (int)PageBorderArt.CreaturesButterfly);
            gLineStyleMap.AddEntry("creatures-fish", (int)PageBorderArt.CreaturesFish);
            gLineStyleMap.AddEntry("creatures-insects", (int)PageBorderArt.CreaturesInsects);
            gLineStyleMap.AddEntry("creatures-lady-bug", (int)PageBorderArt.CreaturesLadyBug);
            gLineStyleMap.AddEntry("cross-stitch", (int)PageBorderArt.CrossStitch);
            gLineStyleMap.AddEntry("cup", (int)PageBorderArt.Cup);
            gLineStyleMap.AddEntry("deco-arch", (int)PageBorderArt.DecoArch);
            gLineStyleMap.AddEntry("deco-arch-color", (int)PageBorderArt.DecoArchColor);
            gLineStyleMap.AddEntry("deco-blocks", (int)PageBorderArt.DecoBlocks);
            gLineStyleMap.AddEntry("diamonds-gray", (int)PageBorderArt.DiamondsGray);
            gLineStyleMap.AddEntry("double-d", (int)PageBorderArt.DoubleD);
            gLineStyleMap.AddEntry("double-diamonds", (int)PageBorderArt.DoubleDiamonds);
            gLineStyleMap.AddEntry("earth-1", (int)PageBorderArt.Earth1);
            gLineStyleMap.AddEntry("earth-2", (int)PageBorderArt.Earth2);
            gLineStyleMap.AddEntry("eclipsing-squares-1", (int)PageBorderArt.EclipsingSquares1);
            gLineStyleMap.AddEntry("eclipsing-squares-2", (int)PageBorderArt.EclipsingSquares2);
            gLineStyleMap.AddEntry("eggs-black", (int)PageBorderArt.EggsBlack);
            gLineStyleMap.AddEntry("fans", (int)PageBorderArt.Fans);
            gLineStyleMap.AddEntry("film", (int)PageBorderArt.Film);
            gLineStyleMap.AddEntry("firecrackers", (int)PageBorderArt.Firecrackers);
            gLineStyleMap.AddEntry("flowers-block-print", (int)PageBorderArt.FlowersBlockPrint);
            gLineStyleMap.AddEntry("flowers-daisies", (int)PageBorderArt.FlowersDaisies);
            gLineStyleMap.AddEntry("flowers-modern-1", (int)PageBorderArt.FlowersModern1);
            gLineStyleMap.AddEntry("flowers-modern-2", (int)PageBorderArt.FlowersModern2);
            gLineStyleMap.AddEntry("flowers-pansy", (int)PageBorderArt.FlowersPansy);
            gLineStyleMap.AddEntry("flowers-red-rose", (int)PageBorderArt.FlowersRedRose);
            gLineStyleMap.AddEntry("flowers-roses", (int)PageBorderArt.FlowersRoses);
            gLineStyleMap.AddEntry("flowers-teacup", (int)PageBorderArt.FlowersTeacup);
            gLineStyleMap.AddEntry("flowers-tiny", (int)PageBorderArt.FlowersTiny);
            gLineStyleMap.AddEntry("gems", (int)PageBorderArt.Gems);
            gLineStyleMap.AddEntry("gingerbread-man", (int)PageBorderArt.GingerbreadMan);
            gLineStyleMap.AddEntry("gradient", (int)PageBorderArt.Gradient);
            gLineStyleMap.AddEntry("handmade-1", (int)PageBorderArt.Handmade1);
            gLineStyleMap.AddEntry("handmade-2", (int)PageBorderArt.Handmade2);
            gLineStyleMap.AddEntry("heart-balloon", (int)PageBorderArt.HeartBalloon);
            gLineStyleMap.AddEntry("heart-gray", (int)PageBorderArt.HeartGray);
            gLineStyleMap.AddEntry("hearts", (int)PageBorderArt.Hearts);
            gLineStyleMap.AddEntry("heebie-jeebies", (int)PageBorderArt.HeebieJeebies);
            gLineStyleMap.AddEntry("holly", (int)PageBorderArt.Holly);
            gLineStyleMap.AddEntry("house-funky", (int)PageBorderArt.HouseFunky);
            gLineStyleMap.AddEntry("hypnotic", (int)PageBorderArt.Hypnotic);
            gLineStyleMap.AddEntry("ice-cream-cones", (int)PageBorderArt.IceCreamCones);
            gLineStyleMap.AddEntry("light-bulb", (int)PageBorderArt.LightBulb);
            gLineStyleMap.AddEntry("lightning-1", (int)PageBorderArt.Lightning1);
            gLineStyleMap.AddEntry("lightning-2", (int)PageBorderArt.Lightning2);
            gLineStyleMap.AddEntry("map-pins", (int)PageBorderArt.MapPins);
            gLineStyleMap.AddEntry("maple-leaf", (int)PageBorderArt.MapleLeaf);
            gLineStyleMap.AddEntry("maple-muffins", (int)PageBorderArt.MapleMuffins);
            gLineStyleMap.AddEntry("marquee", (int)PageBorderArt.Marquee);
            gLineStyleMap.AddEntry("marquee-toothed", (int)PageBorderArt.MarqueeToothed);
            gLineStyleMap.AddEntry("moons", (int)PageBorderArt.Moons);
            gLineStyleMap.AddEntry("mosaic", (int)PageBorderArt.Mosaic);
            gLineStyleMap.AddEntry("music-notes", (int)PageBorderArt.MusicNotes);
            gLineStyleMap.AddEntry("northwest", (int)PageBorderArt.Northwest);
            gLineStyleMap.AddEntry("ovals", (int)PageBorderArt.Ovals);
            gLineStyleMap.AddEntry("packages", (int)PageBorderArt.Packages);
            gLineStyleMap.AddEntry("palms-black", (int)PageBorderArt.PalmsBlack);
            gLineStyleMap.AddEntry("palms-color", (int)PageBorderArt.PalmsColor);
            gLineStyleMap.AddEntry("paper-clips", (int)PageBorderArt.PaperClips);
            gLineStyleMap.AddEntry("papyrus", (int)PageBorderArt.Papyrus);
            gLineStyleMap.AddEntry("party-favor", (int)PageBorderArt.PartyFavor);
            gLineStyleMap.AddEntry("party-glass", (int)PageBorderArt.PartyGlass);
            gLineStyleMap.AddEntry("pencils", (int)PageBorderArt.Pencils);
            gLineStyleMap.AddEntry("people", (int)PageBorderArt.People);
            gLineStyleMap.AddEntry("people-waving", (int)PageBorderArt.PeopleWaving);
            gLineStyleMap.AddEntry("people-hats", (int)PageBorderArt.PeopleHats);
            gLineStyleMap.AddEntry("poinsettias", (int)PageBorderArt.Poinsettias);
            gLineStyleMap.AddEntry("postage-stamp", (int)PageBorderArt.PostageStamp);
            gLineStyleMap.AddEntry("pumpkin-1", (int)PageBorderArt.Pumpkin1);
            gLineStyleMap.AddEntry("push-pin-note-1", (int)PageBorderArt.PushPinNote1);
            gLineStyleMap.AddEntry("push-pin-note-2", (int)PageBorderArt.PushPinNote2);
            gLineStyleMap.AddEntry("pyramids", (int)PageBorderArt.Pyramids);
            gLineStyleMap.AddEntry("pyramids-above", (int)PageBorderArt.PyramidsAbove);
            gLineStyleMap.AddEntry("quadrants", (int)PageBorderArt.Quadrants);
            gLineStyleMap.AddEntry("rings", (int)PageBorderArt.Rings);
            gLineStyleMap.AddEntry("safari", (int)PageBorderArt.Safari);
            gLineStyleMap.AddEntry("sawtooth", (int)PageBorderArt.Sawtooth);
            gLineStyleMap.AddEntry("sawtooth-gray", (int)PageBorderArt.SawtoothGray);
            gLineStyleMap.AddEntry("scared-cat", (int)PageBorderArt.ScaredCat);
            gLineStyleMap.AddEntry("seattle", (int)PageBorderArt.Seattle);
            gLineStyleMap.AddEntry("shadowed-squares", (int)PageBorderArt.ShadowedSquares);
            gLineStyleMap.AddEntry("sharks-teeth", (int)PageBorderArt.SharksTeeth);
            gLineStyleMap.AddEntry("shorebird-tracks", (int)PageBorderArt.ShorebirdTracks);
            gLineStyleMap.AddEntry("skyrocket", (int)PageBorderArt.Skyrocket);
            gLineStyleMap.AddEntry("snowflake-fancy", (int)PageBorderArt.SnowflakeFancy);
            gLineStyleMap.AddEntry("snowflakes", (int)PageBorderArt.Snowflakes);
            gLineStyleMap.AddEntry("sombrero", (int)PageBorderArt.Sombrero);
            gLineStyleMap.AddEntry("southwest", (int)PageBorderArt.Southwest);
            gLineStyleMap.AddEntry("stars", (int)PageBorderArt.Stars);
            gLineStyleMap.AddEntry("stars-top", (int)PageBorderArt.StarsTop);
            gLineStyleMap.AddEntry("stars-3d", (int)PageBorderArt.Stars3D);
            gLineStyleMap.AddEntry("stars-black", (int)PageBorderArt.StarsBlack);
            gLineStyleMap.AddEntry("stars-shadowed", (int)PageBorderArt.StarsShadowed);
            gLineStyleMap.AddEntry("sun", (int)PageBorderArt.Sun);
            gLineStyleMap.AddEntry("swirligig", (int)PageBorderArt.Swirligig);
            gLineStyleMap.AddEntry("torn-paper", (int)PageBorderArt.TornPaper);
            gLineStyleMap.AddEntry("torn-paper-black", (int)PageBorderArt.TornPaperBlack);
            gLineStyleMap.AddEntry("trees", (int)PageBorderArt.Trees);
            gLineStyleMap.AddEntry("triangle-party", (int)PageBorderArt.TriangleParty);
            gLineStyleMap.AddEntry("triangles", (int)PageBorderArt.Triangles);
            gLineStyleMap.AddEntry("tribal-1", (int)PageBorderArt.Tribal1);
            gLineStyleMap.AddEntry("tribal-2", (int)PageBorderArt.Tribal2);
            gLineStyleMap.AddEntry("tribal-3", (int)PageBorderArt.Tribal3);
            gLineStyleMap.AddEntry("tribal-4", (int)PageBorderArt.Tribal4);
            gLineStyleMap.AddEntry("tribal-5", (int)PageBorderArt.Tribal5);
            gLineStyleMap.AddEntry("tribal-6", (int)PageBorderArt.Tribal6);
            gLineStyleMap.AddEntry("twisted-lines-1", (int)PageBorderArt.TwistedLines1);
            gLineStyleMap.AddEntry("twisted-lines-2", (int)PageBorderArt.TwistedLines2);
            gLineStyleMap.AddEntry("vine", (int)PageBorderArt.Vine);
            gLineStyleMap.AddEntry("waveline", (int)PageBorderArt.Waveline);
            gLineStyleMap.AddEntry("weaving-angles", (int)PageBorderArt.WeavingAngles);
            gLineStyleMap.AddEntry("weaving-braid", (int)PageBorderArt.WeavingBraid);
            gLineStyleMap.AddEntry("weaving-ribbon", (int)PageBorderArt.WeavingRibbon);
            gLineStyleMap.AddEntry("weaving-strips", (int)PageBorderArt.WeavingStrips);
            gLineStyleMap.AddEntry("white-flowers", (int)PageBorderArt.WhiteFlowers);
            gLineStyleMap.AddEntry("woodwork", (int)PageBorderArt.Woodwork);
            gLineStyleMap.AddEntry("x-illusions", (int)PageBorderArt.XIllusions);
            gLineStyleMap.AddEntry("zany-triangles", (int)PageBorderArt.ZanyTriangles);
            gLineStyleMap.AddEntry("zig-zag", (int)PageBorderArt.ZigZag);
            gLineStyleMap.AddEntry("zig-zag-stitch", (int)PageBorderArt.ZigZagStitch);

            gNumberStyleMap.AddEntry("decimal", (int)NumberStyle.Arabic);
            gNumberStyleMap.AddEntry("upper-roman", (int)NumberStyle.UppercaseRoman);
            gNumberStyleMap.AddEntry("lower-roman", (int)NumberStyle.LowercaseRoman);
            gNumberStyleMap.AddEntry("upper-letter", (int)NumberStyle.UppercaseLetter);
            gNumberStyleMap.AddEntry("lower-letter", (int)NumberStyle.LowercaseLetter);
            gNumberStyleMap.AddEntry("ordinal", (int)NumberStyle.Ordinal);
            gNumberStyleMap.AddEntry("cardinal-text", (int)NumberStyle.Number);
            gNumberStyleMap.AddEntry("ordinal-text", (int)NumberStyle.OrdinalText);
            gNumberStyleMap.AddEntry("hex", (int)NumberStyle.Hex);
            gNumberStyleMap.AddEntry("chicago", (int)NumberStyle.ChicagoManual);
            gNumberStyleMap.AddEntry("ideograph-digital", (int)NumberStyle.Kanji);
            gNumberStyleMap.AddEntry("japanese-counting", (int)NumberStyle.KanjiDigit);
            gNumberStyleMap.AddEntry("aiueo", (int)NumberStyle.AiueoHalfWidth);
            gNumberStyleMap.AddEntry("iroha", (int)NumberStyle.IrohaHalfWidth);
            gNumberStyleMap.AddEntry("decimal-full-width", (int)NumberStyle.ArabicFullWidth);
            gNumberStyleMap.AddEntry("decimal-half-width", (int)NumberStyle.ArabicHalfWidth);
            gNumberStyleMap.AddEntry("japanese-legal", (int)NumberStyle.KanjiTraditional);
            gNumberStyleMap.AddEntry("japanese-digital-ten-thousand", (int)NumberStyle.KanjiTraditional2);
            gNumberStyleMap.AddEntry("decimal-enclosed-circle", (int)NumberStyle.NumberInCircle);
            gNumberStyleMap.AddEntry("decimal-full-width2", (int)NumberStyle.DecimalFullWidth);
            gNumberStyleMap.AddEntry("aiueo-full-width", (int)NumberStyle.Aiueo);
            gNumberStyleMap.AddEntry("iroha-full-width", (int)NumberStyle.Iroha);
            gNumberStyleMap.AddEntry("decimal-zero", (int)NumberStyle.LeadingZero);
            gNumberStyleMap.AddEntry("bullet", (int)NumberStyle.Bullet);
            gNumberStyleMap.AddEntry("ganada", (int)NumberStyle.Ganada);
            gNumberStyleMap.AddEntry("chosung", (int)NumberStyle.Chosung);
            gNumberStyleMap.AddEntry("decimal-enclosed-fullstop", (int)NumberStyle.GB1);
            gNumberStyleMap.AddEntry("decimal-enclosed-paren", (int)NumberStyle.GB2);
            gNumberStyleMap.AddEntry("decimal-enclosed-circle-chinese", (int)NumberStyle.GB3);
            gNumberStyleMap.AddEntry("ideograph-enclosed-circle", (int)NumberStyle.GB4);
            gNumberStyleMap.AddEntry("ideograph-traditional", (int)NumberStyle.Zodiac1);
            gNumberStyleMap.AddEntry("ideograph-zodiac", (int)NumberStyle.Zodiac2);
            gNumberStyleMap.AddEntry("ideograph-zodiac-traditional", (int)NumberStyle.Zodiac3);
            gNumberStyleMap.AddEntry("taiwanese-counting", (int)NumberStyle.TradChinNum1);
            gNumberStyleMap.AddEntry("ideograph-legal-traditional", (int)NumberStyle.TradChinNum2);
            gNumberStyleMap.AddEntry("taiwanese-counting-thousand", (int)NumberStyle.TradChinNum3);
            gNumberStyleMap.AddEntry("taiwanese-digital", (int)NumberStyle.TradChinNum4);
            gNumberStyleMap.AddEntry("chinese-counting", (int)NumberStyle.SimpChinNum1);
            gNumberStyleMap.AddEntry("chinese-legal-simplified", (int)NumberStyle.SimpChinNum2);
            gNumberStyleMap.AddEntry("chinese-counting-thousand", (int)NumberStyle.SimpChinNum3);
            gNumberStyleMap.AddEntry("chinese-not-impl", (int)NumberStyle.SimpChinNum4);
            gNumberStyleMap.AddEntry("korean-digital", (int)NumberStyle.HanjaRead);
            gNumberStyleMap.AddEntry("korean-counting", (int)NumberStyle.HanjaReadDigit);
            gNumberStyleMap.AddEntry("korean-legal", (int)NumberStyle.Hangul);
            gNumberStyleMap.AddEntry("korean-digital2", (int)NumberStyle.Hanja);
            gNumberStyleMap.AddEntry("vietnamese-counting", (int)NumberStyle.VietCardinalText);
            gNumberStyleMap.AddEntry("russian-lower", (int)NumberStyle.LowercaseRussian);
            gNumberStyleMap.AddEntry("russian-upper", (int)NumberStyle.UppercaseRussian);
            gNumberStyleMap.AddEntry("none", (int)NumberStyle.None);
            gNumberStyleMap.AddEntry("number-in-dash", (int)NumberStyle.NumberInDash);
            gNumberStyleMap.AddEntry("hebrew-1", (int)NumberStyle.Hebrew1);
            gNumberStyleMap.AddEntry("hebrew-2", (int)NumberStyle.Hebrew2);
            gNumberStyleMap.AddEntry("arabic-alpha", (int)NumberStyle.Arabic1);
            gNumberStyleMap.AddEntry("arabic-abjad", (int)NumberStyle.Arabic2);
            gNumberStyleMap.AddEntry("hindi-vowels", (int)NumberStyle.HindiLetter1);
            gNumberStyleMap.AddEntry("hindi-consonants", (int)NumberStyle.HindiLetter2);
            gNumberStyleMap.AddEntry("hindi-numbers", (int)NumberStyle.HindiArabic);
            gNumberStyleMap.AddEntry("hindi-counting", (int)NumberStyle.HindiCardinalText);
            gNumberStyleMap.AddEntry("thai-letters", (int)NumberStyle.ThaiLetter);
            gNumberStyleMap.AddEntry("thai-numbers", (int)NumberStyle.ThaiArabic);
            gNumberStyleMap.AddEntry("thai-counting", (int)NumberStyle.ThaiCardinalText);

            gStyleTypeMap.AddEntry("paragraph", (int)StyleType.Paragraph);
            gStyleTypeMap.AddEntry("character", (int)StyleType.Character);
            gStyleTypeMap.AddEntry("table", (int)StyleType.Table);
            gStyleTypeMap.AddEntry("list", (int)StyleType.List);

            gFontFamilyMap.AddEntry("Auto", (int)FontFamily.Auto);
            gFontFamilyMap.AddEntry("Decorative", (int)FontFamily.Decorative);
            gFontFamilyMap.AddEntry("Modern", (int)FontFamily.Modern);
            gFontFamilyMap.AddEntry("Roman", (int)FontFamily.Roman);
            gFontFamilyMap.AddEntry("Script", (int)FontFamily.Script);
            gFontFamilyMap.AddEntry("Swiss", (int)FontFamily.Swiss);

            gFootnoteSeparatorTypeMap.AddEntry("normal", -1);
            gFootnoteSeparatorTypeMap.AddEntry("separator", (int)FootnoteSeparatorType.FootnoteSeparator);
            gFootnoteSeparatorTypeMap.AddEntry("continuation-separator", (int)FootnoteSeparatorType.FootnoteContinuationSeparator);
            gFootnoteSeparatorTypeMap.AddEntry("continuation-notice", (int)FootnoteSeparatorType.FootnoteContinuationNotice);

            gEndnoteSeparatorTypeMap.AddEntry("normal", -1);
            gEndnoteSeparatorTypeMap.AddEntry("separator", (int)FootnoteSeparatorType.EndnoteSeparator);
            gEndnoteSeparatorTypeMap.AddEntry("continuation-separator", (int)FootnoteSeparatorType.EndnoteContinuationSeparator);
            gEndnoteSeparatorTypeMap.AddEntry("continuation-notice", (int)FootnoteSeparatorType.EndnoteContinuationNotice);

        }
    }
}
