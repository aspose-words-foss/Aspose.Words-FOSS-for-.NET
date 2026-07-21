// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/11/2015 by Alexander Zhiltsov

using System.Drawing;

namespace Aspose.Words
{
    /// <summary>
    /// Contains methods that returns information dependent on system locale during MS Word installation
    /// (i.e. dependent on system locale when the Normal template was generated).
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal static class LocaleDefaults
    {
        /// <summary>
        /// Returns MS Word default page margins that depends on system locale during generation of the Normal template.
        /// </summary>
        /// <param name="lcid">Culture identifier.</param>
        /// <returns>Default page margins in twips.</returns>
        internal static Margins GetPageMargins(int lcid)
        {
            // Most of the values below are taken from the screenshots in the Office documentation for corresponding
            // language. The page for USA is: 
            // https://support.office.com/en-us/article/Change-page-margins-acf0317f-86ab-4bd7-a6a3-75df6234fac3#
            // Other pages can be got by changing language code in the URL. For some languages, margins are mistakenly 
            // displayed as 1''.
            // Values for several popular languages are checked in MS Word.

            switch (lcid)
            {
                case 0x1009: // English - Canada
                case 0x0409: // English - United States
                case 0x0809: // English - United Kingdom
                case 0x0C09: // English - Australia
                case 0x1409: // English - New Zealand
                    return new Margins(Margin1Inch, Margin1Inch, Margin1Inch, Margin1Inch);

                case 0x0804: // Chinese - China
                case 0x0408: // Greek - Greece
                case 0x040D: // Hebrew - Israel
                    return new Margins(Margin1_25Inch, Margin1_25Inch, Margin1Inch, Margin1Inch);

                case 0x0411: // Japanese - Japan
                    return new Margins(Margin30mm, Margin30mm, Margin35mm, Margin30mm);

                case 0x0402: // Bulgarian - Bulgaria
                case 0x041A: // Croatian - Croatia
                case 0x0405: // Czech - Czech Republic
                case 0x0813: // Dutch - Belgium
                case 0x0413: // Dutch - The Netherlands
                case 0x080C: // French - Belgium
                case 0x040C: // French - France
                case 0x100C: // French - Switzerland
                case 0x0414: // Norwegian (Bokmål) - Norway
                case 0x0415: // Polish - Poland
                case 0x041D: // Swedish - Sweden
                case 0x041F: // Turkish - Turkey
                    return new Margins(Margin25mm, Margin25mm, Margin25mm, Margin25mm);

                case 0x0C07: // German - Austria        
                case 0x0407: // German - Germany
                case 0x1407: // German - Liechtenstein  
                case 0x1007: // German - Luxembourg     
                case 0x0807: // German - Switzerland    
                    return new Margins(Margin25mm, Margin25mm, Margin25mm, Margin20mm);

                case 0x2C0A: // Spanish - Argentina
                case 0x340A: // Spanish - Chile
                case 0x080A: // Spanish - Mexico
                case 0x0C0A: // Spanish - Spain
                case 0x0416: // Portuguese - Brazil
                case 0x0816: // Portuguese - Portugal
                    return new Margins(Margin30mm, Margin30mm, Margin25mm, Margin25mm);

                case 0x0406: // Danish - Denmark
                    return new Margins(Margin20mm, Margin20mm, Margin30mm, Margin30mm);

                case 0x0419: // Russian - Russia
                case 0x0444: // Tatar - Russia
                    return new Margins(Margin30mm, Margin15mm, Margin20mm, Margin20mm);

                case 0x0410: // Italian - Italy
                case 0x0810: // Italian - Switzerland   
                    return new Margins(Margin20mm, Margin20mm, Margin25mm, Margin20mm);

                case 0x0422: // Ukrainian - Ukraine
                    return new Margins(Margin25mm, Margin15mm, Margin15mm, Margin15mm);

                case 0x0427: // Lithuanian - Lithuania
                    return new Margins(Margin30mm, Margin10mm, Margin30mm, Margin20mm);

                default:
                    if ((lcid & 0xFF) == 0x09) // English - other, made same as United States
                        return new Margins(Margin1Inch, Margin1Inch, Margin1Inch, Margin1Inch);

                    if ((lcid & 0xFF) == 0x07) // German - other, made same as Germany
                        return new Margins(Margin25mm, Margin25mm, Margin25mm, Margin20mm);

                    if ((lcid & 0xFF) == 0x0A) // Spanish - other, made same as Spain
                        return new Margins(Margin30mm, Margin30mm, Margin25mm, Margin25mm);

                    return new Margins(Margin25mm, Margin25mm, Margin25mm, Margin25mm);
            }
        }

        /// <summary>
        /// Returns MS Word defaults for header/footer distance from the top/bottom edge of the page. 
        /// The values depend on system locale during generation of the Normal template.
        /// </summary>
        /// <param name="lcid">Culture identifier.</param>
        /// <returns>Default distance from the edge of the page in twips.</returns>
        internal static int GetHeaderFooterDistance(int lcid)
        {
            // Checked for United States, United Kingdom, Australia, Germany, France, Russia, Lithuania, China, Japan,
            // Hebrew (Israel), Greece and other.
            switch (lcid)
            {
                case 0x0409: // English - United States
                    return Margin0_5Inch;
                default:
                    return Margin12_5mm;
            }
        }

        /// <summary>
        /// Returns MS Word defaults for space between columns. 
        /// The value depends on system locale during generation of the Normal template.
        /// </summary>
        /// <param name="lcid">Culture identifier.</param>
        /// <returns>Default space between columns in twips.</returns>
        internal static int GetSpaceBetweenColumns(int lcid)
        {
            return GetHeaderFooterDistance(lcid); // Has same value as header/footer distance for checked languages
        }

        /// <summary>
        /// Returns MS Word defaults for page width and height. 
        /// The values depend on system locale during generation of the Normal template.
        /// </summary>
        /// <param name="lcid">Culture identifier.</param>
        /// <returns>Default page size in twips.</returns>
        internal static Size GetPageSize(int lcid)
        {
            PaperSize paperSize = GetPaperSize(lcid);
            return WordUtil.PaperSizeToSize(paperSize);
        }

        private static PaperSize GetPaperSize(int lcid)
        {
            // Note that this wasn't thoroughly checked and there may be other locale/size combinations used by MS Word.
            switch (lcid)
            {
                case 0x0419: // Russian - Russia
                case 0x1409: // English - New Zealand
                case 0x0411: // Japanese - Japan
                case 0x0407: // German - Germany
                    return PaperSize.A4;
                default:
                    return PaperSize.Letter;
            }
        }

        private const int Margin0_5Inch = 720;
        private const int Margin1Inch = 1440;
        private const int Margin1_25Inch = 1800;
        private const int Margin10mm = 567;
        private const int Margin12_5mm = 708; // In defaults of headers and columns MS Word uses value rounded to lesser
        private const int Margin15mm = 850;
        private const int Margin20mm = 1134;
        private const int Margin25mm = 1417;
        private const int Margin30mm = 1701;
        private const int Margin35mm = 1985; // In defaults of margins MS Word uses value rounded to greater

        internal class Margins
        {
            internal Margins(int left, int right, int top, int bottom)
            {
                Left = left;
                Right = right;
                Top = top;
                Bottom = bottom;
            }

            internal int Left { get; }
            internal int Right { get; }
            internal int Top { get; }
            internal int Bottom { get; }
        }
    }
}
