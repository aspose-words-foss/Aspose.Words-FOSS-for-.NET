// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/12/2010 by Roman Korchagin

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Nrx
{
    /// <summary>
    /// Converts enumerated types for Run properties between enum and DOCX/WML string.
    /// </summary>
    internal class NrxRunEnum
    {
        internal static EmphasisMark XmlToEmphasisMark(string value)
        {
            switch (value)
            {
                case "circle": return EmphasisMark.OverWhiteCircle;
                case "comma": return EmphasisMark.OverComma;
                case "dot": return EmphasisMark.OverSolidCircle;
                case "underDot": 
                case "under-dot":
                    return EmphasisMark.UnderSolidCircle;
                default: return EmphasisMark.None;
            }
        }

        internal static string EmphasisMarkToXml(EmphasisMark value, bool isDocx)
        {
            switch (value)
            {
                case EmphasisMark.OverWhiteCircle: return "circle";
                case EmphasisMark.OverComma: return "comma";
                case EmphasisMark.OverSolidCircle: return "dot";
                case EmphasisMark.UnderSolidCircle: return isDocx ? "underDot" : "under-dot";
                default: return "";
            }
        }

        internal static CombineBrackets XmlToCombineBrackets(string value)
        {
            switch (value)
            {
                case "round": return CombineBrackets.Round;
                case "angle": return CombineBrackets.Angle;
                case "square": return CombineBrackets.Square;
                case "curly": return CombineBrackets.Curly;
                default: return CombineBrackets.None;
            }
        }

        internal static string CombineBracketsToXml(CombineBrackets value)
        {
            switch (value)
            {
                case CombineBrackets.Round: return "round";
                case CombineBrackets.Angle: return "angle";
                case CombineBrackets.Square: return "square";
                case CombineBrackets.Curly: return "curly";
                default: return "";
            }
        }

        internal static ThemeFontCore XmlToThemeFont(string value)
        {
            switch (value)
            {
                case "majorAscii": return ThemeFontCore.MajorAscii;
                case "majorBidi": return ThemeFontCore.MajorBidi;
                case "majorEastAsia": return ThemeFontCore.MajorEastAsia;
                case "majorHAnsi": return ThemeFontCore.MajorHAnsi;
                case "minorAscii": return ThemeFontCore.MinorAscii;
                case "minorBidi": return ThemeFontCore.MinorBidi;
                case "minorEastAsia": return ThemeFontCore.MinorEastAsia;
                case "minorHAnsi": return ThemeFontCore.MinorHAnsi;
                default: return ThemeFontCore.None;
            }
        }

        internal static string ThemeFontToXml(ThemeFontCore value)
        {
            switch (value)
            {
                case ThemeFontCore.MajorAscii: return "majorAscii";
                case ThemeFontCore.MajorBidi: return "majorBidi";
                case ThemeFontCore.MajorEastAsia: return "majorEastAsia";
                case ThemeFontCore.MajorHAnsi: return "majorHAnsi";
                case ThemeFontCore.MinorAscii: return "minorAscii";
                case ThemeFontCore.MinorBidi: return "minorBidi";
                case ThemeFontCore.MinorEastAsia: return "minorEastAsia";
                case ThemeFontCore.MinorHAnsi: return "minorHAnsi";
                default: return "";
            }
        }

        internal static CharacterCategory XmlToHint(string value)
        {
            switch (value)
            {
                case "eastAsia":
                case "fareast":     // WML
                    return CharacterCategory.FarEast;
                case "cs":
                    return CharacterCategory.ComplexScript;
                default:
                    return CharacterCategory.Other;
            }
        }

        internal static string HintToXml(CharacterCategory value, bool isDocx)
        {
            switch (value)
            {
                case CharacterCategory.FarEast:
                    return isDocx ? "eastAsia" : "fareast";
                case CharacterCategory.ComplexScript:
                    return "cs";
                default:
                    return "default";
            }
        }

        internal static LineBreakClear XmlToLineBreakClear(string value)
        {
            switch (value)
            {
                case "none": return LineBreakClear.None;
                case "left": return LineBreakClear.Left;
                case "right": return LineBreakClear.Right;
                case "all": return LineBreakClear.All;
                default: return LineBreakClear.None;
            }
        }

        internal static string LineBreakClearToXml(LineBreakClear value)
        {
            switch (value)
            {
                case LineBreakClear.None: return "none";
                case LineBreakClear.Left: return "left";
                case LineBreakClear.Right: return "right";
                case LineBreakClear.All: return "all";
                default: return "";
            }
        }

        internal static char XmlToBreak(string value)
        {
            switch (value)
            {
                case "page": return '\f';
                case "column": return '\x0e';
                case "textWrapping":
                case "text-wrapping":
                    return '\v';
                default: return ' ';
            }
        }

        internal static string BreakToXml(char value, bool isDocx)
        {
            switch (value)
            {
                case '\f': return "page";
                case '\x0e': return "column";
                case '\v': return isDocx ? "textWrapping" : "text-wrapping";
                default: return "";
            }
        }

        internal static TextEffect XmlToTextEffect(string value)
        {
            switch (value)
            {
                case "blinkBackground":
                case "blink-background":
                    return TextEffect.BlinkingBackground;
                case "lights":
                    return TextEffect.LasVegasLights;
                case "antsBlack":
                case "ants-black":
                    return TextEffect.MarchingBlackAnts;
                case "antsRed":
                case "ants-red":
                    return TextEffect.MarchingRedAnts;
                case "shimmer":
                    return TextEffect.Shimmer;
                case "sparkle":
                    return TextEffect.SparkleText;
                case "none": 
                default:
                    return TextEffect.None;
            }
        }

        internal static string TextEffectToXml(TextEffect value, bool isDocx)
        {
            switch (value)
            {
                case TextEffect.BlinkingBackground: return isDocx ? "blinkBackground" : "blink-background";
                case TextEffect.LasVegasLights: return "lights";
                case TextEffect.MarchingBlackAnts: return isDocx ? "antsBlack" : "ants-black";
                case TextEffect.MarchingRedAnts: return isDocx ? "antsRed" : "ants-red";
                case TextEffect.Shimmer: return "shimmer";
                case TextEffect.SparkleText: return "sparkle";
                default: return "none";
            }
        }

        internal static Underline XmlToUnderline(string value)
        {
            switch (value)
            {
                case "single":
                    return Underline.Single;
                case "words":
                    return Underline.Words;
                case "double":
                    return Underline.Double;
                case "thick":
                    return Underline.Thick;
                case "dotted":
                    return Underline.Dotted;
                case "dotted-heavy":
                case "dottedHeavy":
                    return Underline.DottedHeavy;
                case "dash":
                    return Underline.Dash;
                case "dashed-heavy":
                case "dashedHeavy":
                    return Underline.DashHeavy;
                case "dash-long":
                case "dashLong":
                    return Underline.DashLong;
                case "dash-long-heavy":
                case "dashLongHeavy":
                    return Underline.DashLongHeavy;
                case "dot-dash":
                case "dotDash":
                    return Underline.DotDash;
                case "dash-dot-heavy":
                case "dashDotHeavy":
                    return Underline.DotDashHeavy;
                case "dot-dot-dash":
                case "dotDotDash":
                    return Underline.DotDotDash;
                case "dash-dot-dot-heavy":
                case "dashDotDotHeavy":
                    return Underline.DotDotDashHeavy;
                case "wave":
                    return Underline.Wavy;
                case "wavy-heavy":
                case "wavyHeavy":
                    return Underline.WavyHeavy;
                case "wavy-double":
                case "wavyDouble":
                    return Underline.WavyDouble;
                case "none":
                default:
                    return Underline.None;
            }
        }

        internal static string UnderlineToXml(Underline value, bool isDocx)
        {
            switch (value)
            {
                case Underline.Single: return "single";
                case Underline.Words: return "words";
                case Underline.Double: return "double";
                case Underline.Thick: return "thick";
                case Underline.Dotted: return "dotted";
                case Underline.DottedHeavy: return isDocx ? "dottedHeavy" : "dotted-heavy";
                case Underline.Dash: return "dash";
                case Underline.DashHeavy: return isDocx ? "dashedHeavy" : "dashed-heavy";
                case Underline.DashLong: return isDocx ? "dashLong" : "dash-long";
                case Underline.DashLongHeavy: return isDocx ? "dashLongHeavy" : "dash-long-heavy";
                case Underline.DotDash: return isDocx ? "dotDash" : "dot-dash";
                case Underline.DotDashHeavy: return isDocx ? "dashDotHeavy" : "dash-dot-heavy";
                case Underline.DotDotDash: return isDocx ? "dotDotDash" : "dot-dot-dash";
                case Underline.DotDotDashHeavy: return isDocx ? "dashDotDotHeavy" : "dash-dot-dot-heavy";
                case Underline.Wavy: return "wave";
                case Underline.WavyHeavy: return isDocx ? "wavyHeavy" : "wavy-heavy";
                case Underline.WavyDouble: return isDocx ? "wavyDouble" : "wavy-double";
                case Underline.None:
                default:
                    return "none";
            }
        }

        internal static DrColor XmlToHighlight(string value)
        {
            // Highlight colors are limited to 16 colors (we call this Word 97 colors).
            // First translate a string into a Word 97 color and then translate to .NET color.
            Word97Color color97;
            switch (value)
            {
                case "none": 
                    color97 = Word97Color.Auto;
                    break;
                case "white":
                    color97 = Word97Color.White;
                    break;
                case "yellow":
                    color97 = Word97Color.Yellow;
                    break;
                case "green":
                    color97 = Word97Color.Green;
                    break;
                case "cyan":
                    color97 = Word97Color.Cyan;
                    break;
                case "magenta":
                    color97 = Word97Color.Magenta;
                    break;
                case "blue":
                    color97 = Word97Color.Blue;
                    break;
                case "red":
                    color97 = Word97Color.Red;
                    break;
                case "darkBlue":
                case "dark-blue":
                    color97 = Word97Color.DarkBlue;
                    break;
                case "darkCyan":
                case "dark-cyan":
                    color97 = Word97Color.DarkCyan;
                    break;
                case "darkGreen":
                case "dark-green":
                    color97 = Word97Color.DarkGreen;
                    break;
                case "darkMagenta":
                case "dark-magenta":
                    color97 = Word97Color.DarkMagenta;
                    break;
                case "darkRed":
                case "dark-red":
                    color97 = Word97Color.DarkRed;
                    break;
                case "darkYellow":
                case "dark-yellow":
                    color97 = Word97Color.DarkYellow;
                    break;
                case "darkGray":
                case "dark-gray":
                    color97 = Word97Color.DarkGray;
                    break;
                case "lightGray":
                case "light-gray":
                    color97 = Word97Color.LightGray;
                    break;
                case "black":
                    color97 = Word97Color.Black;
                    break;
                default: color97 = Word97Color.Auto;
                    break;
            }

            return WordColorUtil.Word97ColorToColor(color97);
        }

        internal static string HighlightToXml(DrColor value, bool isDocx)
        {
            // First translate a .NET color into a Word 97 color and then translate into a string.
            Word97Color color97 = WordColorUtil.ColorToWord97Color(value);

            switch (color97)
            {
                case Word97Color.Auto: return "none";
                case Word97Color.White: return "white";
                case Word97Color.Yellow: return "yellow";
                case Word97Color.Green: return "green";
                case Word97Color.Cyan: return "cyan";
                case Word97Color.Magenta: return "magenta";
                case Word97Color.Blue: return "blue";
                case Word97Color.Red: return "red";
                case Word97Color.DarkBlue: return isDocx ? "darkBlue" : "dark-blue";
                case Word97Color.DarkCyan: return isDocx ? "darkCyan" : "dark-cyan";
                case Word97Color.DarkGreen: return isDocx ? "darkGreen" : "dark-green";
                case Word97Color.DarkMagenta: return isDocx ? "darkMagenta" : "dark-magenta";
                case Word97Color.DarkRed: return isDocx ? "darkRed" : "dark-red";
                case Word97Color.DarkYellow: return isDocx ? "darkYellow" : "dark-yellow";
                case Word97Color.DarkGray: return isDocx ? "darkGray" : "dark-gray";
                case Word97Color.LightGray: return isDocx ? "lightGray" : "light-gray";
                case Word97Color.Black: return "black";
                default: return "none";
            }
        }

        internal static RunVerticalAlignment XmlToRunVerticalAlignment(string value)
        {
            switch (value)
            {
                case "baseline": return RunVerticalAlignment.Baseline;
                case "superscript": return RunVerticalAlignment.Superscript;
                case "subscript": return RunVerticalAlignment.Subscript;
                default: return RunVerticalAlignment.Baseline;
            }
        }

        internal static string RunVerticalAlignmentToXml(RunVerticalAlignment value)
        {
            switch (value)
            {
                case RunVerticalAlignment.Baseline: return "baseline";
                case RunVerticalAlignment.Superscript: return "superscript";
                case RunVerticalAlignment.Subscript: return "subscript";
                default: return "baseline";
            }
        }

        internal static HyphenRule XmlToHyphenRule(string value)
        {
            switch (value)
            {
                case "none": return HyphenRule.None;
                case "normal": return HyphenRule.Normal;
                case "add-before": return HyphenRule.AddBefore;
                case "change-before": return HyphenRule.ChangeBefore;
                case "delete-before": return HyphenRule.DeleteBefore;
                case "change-after": return HyphenRule.ChangeAfter;
                case "delete-and-change": return HyphenRule.DeleteAndChange;
                default: return HyphenRule.None;
            }
        }

        internal static string HyphenRuleToXml(HyphenRule value)
        {
            switch (value)
            {
                case HyphenRule.None: return "none";
                case HyphenRule.Normal: return "normal";
                case HyphenRule.AddBefore: return "add-before";
                case HyphenRule.ChangeBefore: return "change-before";
                case HyphenRule.DeleteBefore: return "delete-before";
                case HyphenRule.ChangeAfter: return "change-after";
                case HyphenRule.DeleteAndChange: return "delete-and-change";
                default: return "none";
            }
        }

        internal static RubyAlignment XmlToRubyAlignment(string value)
        {
            switch (value)
            {
                case "center": return RubyAlignment.Center;
                case "distributeLetter": return RubyAlignment.DistributeLetter;
                case "distributeSpace": return RubyAlignment.DistributeSpace;
                case "left": return RubyAlignment.Left;
                case "right": return RubyAlignment.Right;
                case "rightVertical": return RubyAlignment.RightVertical;
                default: return RubyAlignment.Center;
            }
        }

        internal static string RubyAlignmentToXml(RubyAlignment value)
        {
            switch (value)
            {
                case RubyAlignment.Center: return "center";
                case RubyAlignment.DistributeLetter: return "distributeLetter";
                case RubyAlignment.DistributeSpace: return "distributeSpace";
                case RubyAlignment.Left: return "left";
                case RubyAlignment.Right: return "right";
                case RubyAlignment.RightVertical: return "rightVertical";
                default: return "center";
            }
        }
    }
}
