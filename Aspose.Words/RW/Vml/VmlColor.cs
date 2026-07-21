// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/10/2006 by Vladimir Averkin

using System;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Nrx;


namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Contains static methods for conversion of Color values to/from VML string representation.
    /// Note: I have decided to throw on unknown colors, at least for some time after WordML release, until we decide otherwise.
    /// </summary>
    internal static class VmlColor 
    {
        /// <summary>
        /// Converts VML color representation to <see cref="System.Drawing.Color"/>.
        /// Examples of VML color strings:
        ///  - 'fill darken(51)'
        ///  - 'yellow'
        ///  - '#38B'
        ///  - '#3388BB'
        ///  - 'FFFFFF'
        /// </summary>
        internal static DrColor VmlToColor(string color)
        {
            Match match = gRegexColor.Match(color);
            
            string action = match.Groups[ActionGroup].Value;
            string name = match.Groups[NameGroup].Value;
            string codeSix = match.Groups[CodeSixGroup].Value;
            string codeHashSix = match.Groups[CodeHashSixGroup].Value;
            string codeHashThree = match.Groups[CodeHashThreeGroup].Value;
            string codeHashOne = match.Groups[CodeHashOneGroup].Value;

            if (StringUtil.HasChars(action))
            {
                int a = 0xef;
                int r;
                int g;
                int b;

                switch (match.Groups[Action1Group].Value)
                {
                    case "fill":
                        r = 0xf0;
                        break;
                    case "lineOrFill":
                        r = 0xf1;
                        break;
                    case "line":
                        r = 0xf2;
                        break;
                    case "shadow":
                        r = 0xf3;
                        break;
                    default:
                        throw new InvalidOperationException(GetColorImportExceptionMessage(color));
                }

                switch (match.Groups[Action2Group].Value)
                {
                    case "darken":
                        g = 0x01;
                        break;
                    case "lighten":
                        g = 0x02;
                        break;
                    case "add":
                        g = 0x03;
                        break;
                    default:
                        throw new InvalidOperationException(GetColorImportExceptionMessage(color));
                }

                b = FormatterPal.ParseInt(match.Groups[ActionCodeGroup].Value);

                return new DrColor(a, r, g, b);
            }
            else if (StringUtil.HasChars(name))
            {
                // The color is expressed in named form.
                // It can be a known color name from CSS2, like 'fuchsia', 'navy' etc.
                // Or it can be a special name, like 'this', 'window', etc.

                if (name == "window")
                    return DrColor.Window;
                else if (name == "windowText")
                    return DrColor.Black;
                else if (name == "this")
                    return DrColor.Empty;
                else
                    return VmlColorEnum.VmlToPalColor(name);
            }
            else if (StringUtil.HasChars(codeSix))
            {
                // The color is expressed in color code, like '#3388BB'. We take only '3388BB' from this string.
                return NrxXmlUtil.XmlToColor(codeSix);
            }
            else if (StringUtil.HasChars(codeHashSix))
            {
                // The color is expressed in color code, like '3388BB'.
                return NrxXmlUtil.XmlToColor(codeHashSix);
            }
            else if (StringUtil.HasChars(codeHashThree))
            {
                // The color is expressed in short code, like '#38B'. The full color code for it will be '#3388BB'
                
                // Convert short color code to full color code.
                StringBuilder fullColorCodeBuilder = new StringBuilder(6);

                for (int i = 0; i < codeHashThree.Length; i++)
                {
                    char ch = codeHashThree[i];
                    fullColorCodeBuilder.Append(ch);
                    fullColorCodeBuilder.Append(ch);
                }

                return NrxXmlUtil.XmlToColor(fullColorCodeBuilder.ToString());
            }
            else if (StringUtil.HasChars(codeHashOne))
            {
                // The color is in form #X, for example #0 or #f. The full color code for #f will be #0000ff. WORDSNET-11964

                // Converting short color code to full color code.
                StringBuilder fullColorCodeBuilder = new StringBuilder(6);
                fullColorCodeBuilder.Append("0000");
                Debug.Assert(codeHashOne.Length == 2);
                fullColorCodeBuilder.Append(codeHashOne[1]);
                fullColorCodeBuilder.Append(codeHashOne[1]);

                return NrxXmlUtil.XmlToColor(fullColorCodeBuilder.ToString());
            }
            else
                throw new InvalidOperationException(GetColorImportExceptionMessage(color));
        }

        /// <summary>
        /// Converts <see cref="System.Drawing.Color"/> to VML color string.
        /// </summary>
        /// <param name="color">Color to convert.</param>
        /// <returns>VML color string, like 'fill darken(51)', 'yellow', '#38B' or '#3388BB'.</returns>
        internal static string ColorToVml(DrColor color)
        {
            return ColorToVml(color, true, false);
        }

        /// <summary>
        /// Converts <see cref="System.Drawing.Color"/> to VML color string.
        /// </summary>
        /// <param name="color">Color to convert.</param>
        /// <param name="isShortNotationAllowed">True - to allow short color notations like #7fc, instead of #77ffcc.</param>
        /// <param name="isDigitalNotationUpper">True - to output digital color notation in upper case, like #77FFCC.</param>
        /// <returns>VML color string, like 'fill darken(51)', 'yellow', '#38B' or '#3388BB'.</returns>
        internal static string ColorToVml(DrColor color, bool isShortNotationAllowed, bool isDigitalNotationUpper)
        {
            if (color.IsEmpty)
                return "this";

            // 0xef in transparency means that this color is actually a coloring action definition, e.g. 'fill darken(51)'.
            // This definitions are not described anywhere - I had to figure them out.
            if (color.A == 0xef)
            {
                StringBuilder sb = new StringBuilder();

                if (color.R >= 0xf0)
                {
                    switch (color.R)
                    {
                        case 0xf0:
                            sb.Append("fill ");
                            break;
                        case 0xf1:
                            sb.Append("lineOrFill ");
                            break;
                        case 0xf2:
                            // andrnosk: WORDSNET-5034 Added one more shadow color type.
                            sb.Append("line ");
                            break;
                        case 0xf3:
                            sb.Append("shadow ");
                            break;
                        case 0xf4: // Seen in DOC files sometimes (e.g. TestLeorMeller.doc) but not written to WordML.
                        case 0xf7: // Seen in TestDefect1402 in Doc2Docx conversion.
                        default:
                            return null;
                    }

                    switch (color.G)
                    {
                        case 0x01:
                            sb.Append("darken");
                            break;
                        case 0x02:
                            sb.Append("lighten");
                            break;
                        case 0x03:
                            sb.Append("add");
                            break;
                        default:
                            return null;
                    }

                    sb.AppendFormat("({0})", color.B);
                }
                else
                {
                    switch (color.R)
                    {
                        case 0x11:
                            sb.Append("window");
                            break;
                        case 0x01: // Seen in TestJira5597 in Doc2Docx conversion.
                                   // See TestExportDocx.TestDefect5597 for details.
                        default:
                            return null;
                    }
                }

                return sb.ToString();
            }

            // Check if the color is a known color and can be output in a human readable form,
            // e.g. 'yellow' instead of '#FFFF00'.

            string knownColorName = VmlColorEnum.PalColorToVml(color);

            if (knownColorName != "")
                return knownColorName;


            if (isShortNotationAllowed)
            {
                // Check if the color can be output in short form, e.g. '#38B' instead of '#3388BB'.

                if (color.A == 0xff && IsShortColorComponent(color.R) && IsShortColorComponent(color.G) && IsShortColorComponent(color.B))
                {
                    return string.Format("#{0}{1}{2}", ColorComponentToShortVml(color.R), ColorComponentToShortVml(color.G), ColorComponentToShortVml(color.B));
                }
            }

            string digitalNotation = string.Format("#{0}{1}{2}", ColorComponentToVml(color.R), ColorComponentToVml(color.G), ColorComponentToVml(color.B));

            if (isDigitalNotationUpper)
                return digitalNotation.ToUpper();

            return digitalNotation;
        }

        /// <summary>
        /// Returns the base color value from VML color extended markup.
        /// #3b1f2d [rgb(149,79,114) darken(102)] returns DrColor(149, 79, 114).
        /// </summary>
        internal static DrColor GetBaseColor(string vmlColor)
        {
            MatchCollection matchCol = gRegexBaseColor.Matches(vmlColor);

            if ((matchCol.Count != 3) || (!matchCol[0].Groups[1].Value.Contains("rgb")))
                return null;

            int r = int.Parse(matchCol[0].Groups[2].Value);
            int g = int.Parse(matchCol[1].Groups[2].Value);
            int b = int.Parse(matchCol[2].Groups[2].Value);

            return DrColor.FromArgb(r, g, b);
        }

        /// <summary>
        /// Returns the modifier value from VML color extended markup.
        /// #3b1f2d [rgb(149,79,114) darken(102)] returns 102.
        /// </summary>
        internal static int GetColorModifier(string vmlColor)
        {
            Match match = gRegexModifier.Match(vmlColor);

            if (match.Groups[1].Value.StartsWith("lighten", StringComparison.InvariantCulture))
                return int.Parse(match.Groups[2].Value);

            if (match.Groups[1].Value.StartsWith("darken", StringComparison.InvariantCulture))
                return -int.Parse(match.Groups[2].Value);

            return 0;
        }

        private static string GetColorImportExceptionMessage(string color)
        {
            return string.Format("Unknown color type encountered during WordML import - {0}.", color);
        }

        private static string ColorComponentToVml(int colorComponent)
        {
            return FormatterPal.IntToStrX2Lower(colorComponent);
        }

        private static string ColorComponentToShortVml(int colorComponent)
        {
            return FormatterPal.IntToStrXLower(colorComponent % 16);
        }

        private static bool IsShortColorComponent(int colorComponent)
        {
            return (colorComponent / 16) == (colorComponent % 16);
        }

        // In Java we must use group indexes, not names.
        private const int ActionGroup = 2;
        private const int Action1Group = 3;
        private const int Action2Group = 4;
        private const int ActionCodeGroup = 5;
        private const int CodeSixGroup = 6;
        private const int NameGroup = 8;
        private const int CodeHashSixGroup = 10;
        private const int CodeHashThreeGroup = 12;
        private const int CodeHashOneGroup = 13;

        /// <summary>
        /// RK this was created by VA originally. It included regex for rgb, cmyk and ink.
        /// But this was not used and I think it was not tested. I couldn't verify it by just reading the regex.
        /// It caused problems in DOCX for color like "#9bbb59 [3042]" because of the number in braces.
        /// I removed parsing of rgb, cmyk and ink until we need to support it and until we test it.
        /// </summary>
        private static readonly Regex gRegexColor = new Regex(
            @"^(" +
            // 'fill darken(51)'
            @"(([a-z]+) ([a-z]+)\((\d+)\))|" +              // Groups: Action1 Action2(ActionCode). The whole thing is the "action" group.
            // 'FFFFFF'
            @"([0-9a-f]{6})|" +                             // Group: CodeSix
            // 'yellow' and sometimes 'white [3102]' etc.
            @"(([a-z]+).*)|" +                              // Group: Name
            // '#3388BB' and sometimes '#9bbb59 [rgb(1,2,3)]' etc.
            @"(#([0-9a-f]{6}).*)|" +                        // Group: CodeHashSix
            // '#38B' and sometimes '#ddd [3204]'
            @"(#([0-9a-f]{3}).*)|" +                        // Group: CodeHashThree
            // '#0' WORDSNET-11964
            @"(#[0-9a-f])" +                                // Group: CodeHashOne
            @")$", 
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex gRegexBaseColor =
            new Regex(@"(rgb\(|,)(\b[\d]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex gRegexModifier =
            new Regex(@"(darken\(|lighten\()(\b[\d]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
