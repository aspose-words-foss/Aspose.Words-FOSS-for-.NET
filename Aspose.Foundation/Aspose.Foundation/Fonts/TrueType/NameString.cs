// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2005 by Roman Korchagin

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Specifies a string from the true type font Naming Table.
    /// </summary>
    internal enum NameString
    {
        CopyrightNotice = 0,
        /// <summary>
        /// Up to four fonts can share the Font Family name, forming a font style linking group 
        /// (regular, italic, bold, bold italic - as defined by OS/2.fsSelection bit settings).
        /// </summary>
        FamilyName = 1,
        /// <summary>
        /// The Font Subfamily name distiguishes the font in a group with the same 
        /// Font Family name (name ID 1). This is assumed to address style (italic, oblique) 
        /// and weight (light, bold, black, etc.). A font with no particular differences in 
        /// weight or style (e.g. medium weight, not italic and fsSelection bit 6 set) should 
        /// have the string "Regular" stored in this position.
        /// </summary>
        SubFamilyName = 2,
        /// <summary>
        /// Unique font identifier
        /// </summary>
        FontId = 3,
        /// <summary>
        /// Full font name; this should be a combination of strings 1 and 2. 
        /// Exception: if the font is "Regular" as indicated in string 2, 
        /// then use only the family name contained in string 1. 
        /// An exception to the above definition of Full font name is for Microsoft platform 
        /// strings for CFF OpenType fonts: in this case, the Full font name string must be i
        /// dentical to the PostScript FontName in the CFF Name INDEX.
        /// </summary>
        FullFontName = 4,
        /// <summary>
        /// Version string. Should begin with the syntax 'Version number.number' 
        /// (upper case, lower case, or mixed, with a space between "Version" and the number). 
        /// </summary>
        Version = 5,
        /// <summary>
        /// Postscript name for the font; Name ID 6 specifies a string which is used to invoke a 
        /// PostScript language font that corresponds to this OpenType font. 
        /// If no name ID 6 is present, then there is no defined method for invoking this font 
        /// on a PostScript interpreter.
        /// </summary>
        PostSciptName = 6,
        /// <summary>
        /// this is used to save any trademark notice/information for this font. 
        /// Such information should be based on legal advice. This is distinctly separate from the copyright. 
        /// </summary>
        Trademark = 7,
        Manufacturer = 8,
        Designed = 9,
        Description = 10
    }
}
