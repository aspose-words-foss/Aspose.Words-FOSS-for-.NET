// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/05/2006 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Locale independent style identifier.
    /// </summary>
    /// <remarks>
    /// <p>The names of built-in styles in MS Word are localized for different languages.
    /// Using a style identifier you can find the correct style regardless of the document language.</p>
    /// <p>All user defined styles are assigned the <see cref="StyleIdentifier.User"/> value.</p>
    /// </remarks>
    /// <dev>DO NOT RENUMBER! These values are important for DOC import/export.</dev>
    /// <dev>Please note that the CURRENT USED MAXIMUM COUNT is SmartLink (371).</dev>
    [CppEnumWithOperators]
    [CppEnumEnableMetadata]
    public enum StyleIdentifier
    {
        // Character styles.

        /// <summary>
        /// </summary>
        BookTitle = 264,
        /// <summary>
        /// The Annotation (Comment) Reference style.
        /// </summary>
        CommentReference = 39,    // 0x0027  char style
        /// <summary>
        /// The Default Paragraph Font style.
        /// </summary>
        DefaultParagraphFont = 65,    // 0x0041  char style
        /// <summary>
        /// </summary>
        Emphasis = 88,    // 0x0058  char style
        /// <summary>
        /// The Endnote Reference style.
        /// </summary>
        EndnoteReference = 42,    // 0x002A  char style
        /// <summary>
        /// </summary>
        FollowedHyperlink = 86, // 0x0056   char style
        /// <summary>
        /// The Footnote Reference style.
        /// </summary>
        FootnoteReference = 38,    // 0x0026  char style
        /// <summary>
        /// </summary>
        HtmlAcronym = 95,
        /// <summary>
        /// </summary>
        HtmlCite = 97,
        /// <summary>
        /// </summary>
        HtmlCode = 98,
        /// <summary>
        /// </summary>
        HtmlDefinition = 99,
        /// <summary>
        /// </summary>
        HtmlKeyboard = 100,
        /// <summary>
        /// </summary>
        HtmlSample = 102,
        /// <summary>
        /// </summary>
        HtmlTypewriter = 103,
        /// <summary>
        /// </summary>
        HtmlVariable = 104,
        /// <summary>
        /// The Hyperlink style.
        /// </summary>
        Hyperlink = 85,    // 0x0055  char style
        /// <summary>
        /// </summary>
        IntenseEmphasis = 261,
        /// <summary>
        /// </summary>
        IntenseReference = 263,
        /// <summary>
        /// The Line Number style.
        /// </summary>
        LineNumber = 40,    // 0x0028  char style
        /// <summary>
        /// The Page Number style.
        /// </summary>
        PageNumber = 41,    // 0x0029  char style
        /// <summary>
        /// </summary>
        PlaceholderText = 156,
        /// <summary>
        /// The Smart Link style.
        /// </summary>
        SmartLink = 371,
        /// <summary>
        /// </summary>
        Strong = 87,    // 0x0057  char style
        /// <summary>
        /// </summary>
        SubtleEmphasis = 260,
        /// <summary>
        /// </summary>
        SubtleReference = 262,


        // Paragraph styles that have linked character styles.
        /// <summary>
        /// </summary>
        BalloonText = 153,    // 0x0099
        /// <summary>
        /// The Body Text style.
        /// </summary>
        BodyText = 66,    // 0x0042
        /// <summary>
        /// </summary>
        BodyText2 = 80,    // 0x0050
        /// <summary>
        /// </summary>
        BodyText3 = 81,    // 0x0051
        /// <summary>
        /// </summary>
        BodyText1I = 77,    // 0x004D
        /// <summary>
        /// </summary>
        BodyText1I2 = 78,    // 0x004E
        /// <summary>
        /// </summary>
        BodyTextInd = 67,    // 0x0043
        /// <summary>
        /// </summary>
        BodyTextInd2 = 82,   // 0x0052
        /// <summary>
        /// </summary>
        BodyTextInd3 = 83,   // 0x0053
        /// <summary>
        /// </summary>
        Closing = 63,    // 0x003F
        /// <summary>
        /// </summary>
        CommentSubject = 106,
        /// <summary>
        /// The Annotation (Comment) Text style.
        /// </summary>
        CommentText = 30,
        /// <summary>
        /// </summary>
        Date = 76,    // 0X004C
        /// <summary>
        /// </summary>
        DocumentMap = 89,    // 0x0059  char style
        /// <summary>
        /// </summary>
        EmailSignature = 91, // 0x005b
        /// <summary>
        /// The Endnote Text style.
        /// </summary>
        EndnoteText = 43,    // 0x002B
        /// <summary>
        /// The Footer style.
        /// </summary>
        Footer = 32,
        /// <summary>
        /// The Footnote Text style.
        /// </summary>
        FootnoteText = 29,
        /// <summary>
        /// The Header style.
        /// </summary>
        Header = 31,
        /// <summary>
        /// The Heading 1 style.
        /// </summary>
        Heading1 = 1,
        /// <summary>
        /// The Heading 2 style.
        /// </summary>
        Heading2 = 2,
        /// <summary>
        /// The Heading 3 style.
        /// </summary>
        Heading3 = 3,
        /// <summary>
        /// The Heading 4 style.
        /// </summary>
        Heading4 = 4,
        /// <summary>
        /// The Heading 5 style.
        /// </summary>
        Heading5 = 5,
        /// <summary>
        /// The Heading 6 style.
        /// </summary>
        Heading6 = 6,
        /// <summary>
        /// The Heading 7 style.
        /// </summary>
        Heading7 = 7,
        /// <summary>
        /// The Heading 8 style.
        /// </summary>
        Heading8 = 8,
        /// <summary>
        /// The Heading 9 style.
        /// </summary>
        Heading9 = 9,
        /// <summary>
        /// </summary>
        HtmlAddress = 96,
        /// <summary>
        /// </summary>
        HtmlTopOfForm = 92, // 0x005c       // This is an obscure style. Can be seen in the RTF spec and on the internet.
        /// <summary>
        /// </summary>
        HtmlBottomOfForm = 93, // 0x005d    // This is an obscure style. Can be seen in the RTF spec and on the internet.
        /// <summary>
        /// </summary>
        HtmlPreformatted = 101,
        /// <summary>
        /// </summary>
        IntenseQuote = 181,
        /// <summary>
        /// </summary>
        Macro = 45,    // 0x002D
        /// <summary>
        /// </summary>
        MessageHeader = 73,    // 0x0049
        /// <summary>
        /// </summary>
        NoteHeading = 79,    // 0x004F
        /// <summary>
        /// </summary>
        PlainText = 90,    // 0x005A
        /// <summary>
        /// </summary>
        Quote = 180,
        /// <summary>
        /// </summary>
        Salutation = 75,    // 0x004B
        /// <summary>
        /// </summary>
        Signature = 64,    // 0x0040
        /// <summary>
        /// </summary>
        Subtitle = 74,    // 0x004A
        /// <summary>
        /// The Title style.
        /// </summary>
        Title = 62,    // 0x003E


        // Paragraph styles.
        /// <summary>
        /// </summary>
        Bibliography = 265,
        /// <summary>
        /// </summary>
        BlockText = 84,    // 0x0054
        /// <summary>
        /// </summary>
        Caption = 34,
        /// <summary>
        /// The Envelope Address style.
        /// </summary>
        EnvelopeAddress = 36,
        /// <summary>
        /// The Envelope Return style.
        /// </summary>
        EnvelopeReturn = 37,
        /// <summary>
        /// </summary>
        Index1 = 10,
        /// <summary>
        /// </summary>
        Index2 = 11,
        /// <summary>
        /// </summary>
        Index3 = 12,
        /// <summary>
        /// </summary>
        Index4 = 13,
        /// <summary>
        /// </summary>
        Index5 = 14,
        /// <summary>
        /// </summary>
        Index6 = 15,
        /// <summary>
        /// </summary>
        Index7 = 16,
        /// <summary>
        /// </summary>
        Index8 = 17,
        /// <summary>
        /// </summary>
        Index9 = 18,
        /// <summary>
        /// The Index Heading style.
        /// </summary>
        IndexHeading = 33,
        /// <summary>
        /// The List style.
        /// </summary>
        List = 47,    // 0x002F
        /// <summary>
        /// </summary>
        List2 = 50,    // 0x0032
        /// <summary>
        /// </summary>
        List3 = 51,    // 0x0033
        /// <summary>
        /// </summary>
        List4 = 52,    // 0x0034
        /// <summary>
        /// </summary>
        List5 = 53,    // 0x0035
        /// <summary>
        /// The List Bullet style.
        /// </summary>
        ListBullet = 48,    // 0x0030
        /// <summary>
        /// </summary>
        ListBullet2 = 54,    // 0x0036
        /// <summary>
        /// </summary>
        ListBullet3 = 55,    // 0x0037
        /// <summary>
        /// </summary>
        ListBullet4 = 56,    // 0x0038
        /// <summary>
        /// </summary>
        ListBullet5 = 57,    // 0x0039
        /// <summary>
        /// </summary>
        ListContinue = 68,    // 0x0044
        /// <summary>
        /// </summary>
        ListContinue2 = 69,    // 0x0045
        /// <summary>
        /// </summary>
        ListContinue3 = 70,    // 0x0046
        /// <summary>
        /// </summary>
        ListContinue4 = 71,    // 0x0047
        /// <summary>
        /// </summary>
        ListContinue5 = 72,    // 0x0048
        /// <summary>
        /// The List Number style.
        /// </summary>
        ListNumber = 49,    // 0x0031
        /// <summary>
        /// </summary>
        ListNumber2 = 58,    // 0x003A
        /// <summary>
        /// </summary>
        ListNumber3 = 59,    // 0x003B
        /// <summary>
        /// </summary>
        ListNumber4 = 60,    // 0x003C
        /// <summary>
        /// </summary>
        ListNumber5 = 61,    // 0x003D
        /// <summary>
        /// </summary>
        ListParagraph = 179,
        /// <summary>
        /// </summary>
        NoSpacing = 157,
        /// <summary>
        /// The Normal style.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// </summary>
        NormalWeb = 94,        // 0x005e
        /// <summary>
        /// The Normal Indent style.
        /// </summary>
        NormalIndent = 28,
        /// <summary>
        /// </summary>
        TableOfAuthorities = 44,    // 0x002C
        /// <summary>
        /// The Table of Figures style.
        /// </summary>
        TableOfFigures = 35,
        /// <summary>
        /// </summary>
        ToaHeading = 46,    // 0x002E
        /// <summary>
        /// </summary>
        Toc1 = 19,
        /// <summary>
        /// </summary>
        Toc2 = 20,
        /// <summary>
        /// </summary>
        Toc3 = 21,
        /// <summary>
        /// </summary>
        Toc4 = 22,
        /// <summary>
        /// </summary>
        Toc5 = 23,
        /// <summary>
        /// </summary>
        Toc6 = 24,
        /// <summary>
        /// </summary>
        Toc7 = 25,
        /// <summary>
        /// </summary>
        Toc8 = 26,
        /// <summary>
        /// </summary>
        Toc9 = 27,
        /// <summary>
        /// </summary>
        TocHeading = 266,
        /// <summary>
        /// </summary>
        Revision = 178,


        // List styles.
        /// <summary>
        /// The 1 / a / i style.
        /// </summary>
        OutlineList1 = 108,
        /// <summary>
        /// The 1 / 1.1 / 1.1.1 style.
        /// </summary>
        OutlineList2 = 109,
        /// <summary>
        /// The Article / Section style.
        /// </summary>
        OutlineList3 = 110,
        /// <summary>
        /// </summary>
        NoList = 107,        // 0x006b


        // Table styles.
        /// <summary>
        /// </summary>
        ColorfulGrid = 171,
        /// <summary>
        /// </summary>
        ColorfulGridAccent1 = 189,
        /// <summary>
        /// </summary>
        ColorfulGridAccent2 = 203,
        /// <summary>
        /// </summary>
        ColorfulGridAccent3 = 217,
        /// <summary>
        /// </summary>
        ColorfulGridAccent4 = 231,
        /// <summary>
        /// </summary>
        ColorfulGridAccent5 = 245,
        /// <summary>
        /// </summary>
        ColorfulGridAccent6 = 259,
        /// <summary>
        /// </summary>
        ColorfulList = 170,
        /// <summary>
        /// </summary>
        ColorfulListAccent1 = 188,
        /// <summary>
        /// </summary>
        ColorfulListAccent2 = 202,
        /// <summary>
        /// </summary>
        ColorfulListAccent3 = 216,
        /// <summary>
        /// </summary>
        ColorfulListAccent4 = 230,
        /// <summary>
        /// </summary>
        ColorfulListAccent5 = 244,
        /// <summary>
        /// </summary>
        ColorfulListAccent6 = 258,
        /// <summary>
        /// </summary>
        ColorfulShading = 169,
        /// <summary>
        /// </summary>
        ColorfulShadingAccent1 = 187,
        /// <summary>
        /// </summary>
        ColorfulShadingAccent2 = 201,
        /// <summary>
        /// </summary>
        ColorfulShadingAccent3 = 215,
        /// <summary>
        /// </summary>
        ColorfulShadingAccent4 = 229,
        /// <summary>
        /// </summary>
        ColorfulShadingAccent5 = 243,
        /// <summary>
        /// </summary>
        ColorfulShadingAccent6 = 257,
        /// <summary>
        /// </summary>
        DarkList = 168,
        /// <summary>
        /// </summary>
        DarkListAccent1 = 186,
        /// <summary>
        /// </summary>
        DarkListAccent2 = 200,
        /// <summary>
        /// </summary>
        DarkListAccent3 = 214,
        /// <summary>
        /// </summary>
        DarkListAccent4 = 228,
        /// <summary>
        /// </summary>
        DarkListAccent5 = 242,
        /// <summary>
        /// </summary>
        DarkListAccent6 = 256,
        /// <summary>
        /// </summary>
        LightGrid = 160,
        /// <summary>
        /// </summary>
        LightGridAccent1 = 174,
        /// <summary>
        /// </summary>
        LightGridAccent2 = 192,
        /// <summary>
        /// </summary>
        LightGridAccent3 = 206,
        /// <summary>
        /// </summary>
        LightGridAccent4 = 220,
        /// <summary>
        /// </summary>
        LightGridAccent5 = 234,
        /// <summary>
        /// </summary>
        LightGridAccent6 = 248,
        /// <summary>
        /// </summary>
        LightList = 159,
        /// <summary>
        /// </summary>
        LightListAccent1 = 173,
        /// <summary>
        /// </summary>
        LightListAccent2 = 191,
        /// <summary>
        /// </summary>
        LightListAccent3 = 205,
        /// <summary>
        /// </summary>
        LightListAccent4 = 219,
        /// <summary>
        /// </summary>
        LightListAccent5 = 233,
        /// <summary>
        /// </summary>
        LightListAccent6 = 247,
        /// <summary>
        /// </summary>
        LightShading = 158,
        /// <summary>
        /// </summary>
        LightShadingAccent1 = 172,
        /// <summary>
        /// </summary>
        LightShadingAccent2 = 190,
        /// <summary>
        /// </summary>
        LightShadingAccent3 = 204,
        /// <summary>
        /// </summary>
        LightShadingAccent4 = 218,
        /// <summary>
        /// </summary>
        LightShadingAccent5 = 232,
        /// <summary>
        /// </summary>
        LightShadingAccent6 = 246,
        /// <summary>
        /// </summary>
        MediumGrid1 = 165,
        /// <summary>
        /// </summary>
        MediumGrid1Accent1 = 183,
        /// <summary>
        /// </summary>
        MediumGrid1Accent2 = 197,
        /// <summary>
        /// </summary>
        MediumGrid1Accent3 = 211,
        /// <summary>
        /// </summary>
        MediumGrid1Accent4 = 225,
        /// <summary>
        /// </summary>
        MediumGrid1Accent5 = 239,
        /// <summary>
        /// </summary>
        MediumGrid1Accent6 = 253,
        /// <summary>
        /// </summary>
        MediumGrid2 = 166,
        /// <summary>
        /// </summary>
        MediumGrid2Accent1 = 184,
        /// <summary>
        /// </summary>
        MediumGrid2Accent2 = 198,
        /// <summary>
        /// </summary>
        MediumGrid2Accent3 = 212,
        /// <summary>
        /// </summary>
        MediumGrid2Accent4 = 226,
        /// <summary>
        /// </summary>
        MediumGrid2Accent5 = 240,
        /// <summary>
        /// </summary>
        MediumGrid2Accent6 = 254,
        /// <summary>
        /// </summary>
        MediumGrid3 = 167,
        /// <summary>
        /// </summary>
        MediumGrid3Accent1 = 185,
        /// <summary>
        /// </summary>
        MediumGrid3Accent2 = 199,
        /// <summary>
        /// </summary>
        MediumGrid3Accent3 = 213,
        /// <summary>
        /// </summary>
        MediumGrid3Accent4 = 227,
        /// <summary>
        /// </summary>
        MediumGrid3Accent5 = 241,
        /// <summary>
        /// </summary>
        MediumGrid3Accent6 = 255,
        /// <summary>
        /// </summary>
        MediumList1 = 163,
        /// <summary>
        /// </summary>
        MediumList1Accent1 = 177,
        /// <summary>
        /// </summary>
        MediumList1Accent2 = 195,
        /// <summary>
        /// </summary>
        MediumList1Accent3 = 209,
        /// <summary>
        /// </summary>
        MediumList1Accent4 = 223,
        /// <summary>
        /// </summary>
        MediumList1Accent5 = 237,
        /// <summary>
        /// </summary>
        MediumList1Accent6 = 251,
        /// <summary>
        /// </summary>
        MediumList2 = 164,
        /// <summary>
        /// </summary>
        MediumList2Accent1 = 182,
        /// <summary>
        /// </summary>
        MediumList2Accent2 = 196,
        /// <summary>
        /// </summary>
        MediumList2Accent3 = 210,
        /// <summary>
        /// </summary>
        MediumList2Accent4 = 224,
        /// <summary>
        /// </summary>
        MediumList2Accent5 = 238,
        /// <summary>
        /// </summary>
        MediumList2Accent6 = 252,
        /// <summary>
        /// </summary>
        MediumShading1 = 161,
        /// <summary>
        /// </summary>
        MediumShading1Accent1 = 175,
        /// <summary>
        /// </summary>
        MediumShading1Accent2 = 193,
        /// <summary>
        /// </summary>
        MediumShading1Accent3 = 207,
        /// <summary>
        /// </summary>
        MediumShading1Accent4 = 221,
        /// <summary>
        /// </summary>
        MediumShading1Accent5 = 235,
        /// <summary>
        /// </summary>
        MediumShading1Accent6 = 249,
        /// <summary>
        /// </summary>
        MediumShading2 = 162,
        /// <summary>
        /// </summary>
        MediumShading2Accent1 = 176,
        /// <summary>
        /// </summary>
        MediumShading2Accent2 = 194,
        /// <summary>
        /// </summary>
        MediumShading2Accent3 = 208,
        /// <summary>
        /// </summary>
        MediumShading2Accent4 = 222,
        /// <summary>
        /// </summary>
        MediumShading2Accent5 = 236,
        /// <summary>
        /// </summary>
        MediumShading2Accent6 = 250,
        /// <summary>
        /// </summary>
        Table3DEffects1 = 142,
        /// <summary>
        /// </summary>
        Table3DEffects2 = 143,
        /// <summary>
        /// </summary>
        Table3DEffects3 = 144,
        /// <summary>
        /// </summary>
        TableClassic1 = 114,
        /// <summary>
        /// </summary>
        TableClassic2 = 115,
        /// <summary>
        /// </summary>
        TableClassic3 = 116,
        /// <summary>
        /// </summary>
        TableClassic4 = 117,
        /// <summary>
        /// </summary>
        TableColorful1 = 118,
        /// <summary>
        /// </summary>
        TableColorful2 = 119,
        /// <summary>
        /// </summary>
        TableColorful3 = 120,
        /// <summary>
        /// </summary>
        TableColumns1 = 121,
        /// <summary>
        /// </summary>
        TableColumns2 = 122,
        /// <summary>
        /// </summary>
        TableColumns3 = 123,
        /// <summary>
        /// </summary>
        TableColumns4 = 124,
        /// <summary>
        /// </summary>
        TableColumns5 = 125,
        /// <summary>
        /// </summary>
        TableContemporary = 145,
        /// <summary>
        /// </summary>
        TableElegant = 146,
        /// <summary>
        /// </summary>
        TableGrid = 154,
        /// <summary>
        /// </summary>
        TableGrid1 = 126,
        /// <summary>
        /// </summary>
        TableGrid2 = 127,
        /// <summary>
        /// </summary>
        TableGrid3 = 128,
        /// <summary>
        /// </summary>
        TableGrid4 = 129,
        /// <summary>
        /// </summary>
        TableGrid5 = 130,
        /// <summary>
        /// </summary>
        TableGrid6 = 131,
        /// <summary>
        /// </summary>
        TableGrid7 = 132,
        /// <summary>
        /// </summary>
        TableGrid8 = 133,
        /// <summary>
        /// </summary>
        TableList1 = 134,
        /// <summary>
        /// </summary>
        TableList2 = 135,
        /// <summary>
        /// </summary>
        TableList3 = 136,
        /// <summary>
        /// </summary>
        TableList4 = 137,
        /// <summary>
        /// </summary>
        TableList5 = 138,
        /// <summary>
        /// </summary>
        TableList6 = 139,
        /// <summary>
        /// </summary>
        TableList7 = 140,
        /// <summary>
        /// </summary>
        TableList8 = 141,
        /// <summary>
        /// </summary>
        TableNormal = 105,    // 0x0069
        /// <summary>
        /// </summary>
        TableProfessional = 147,
        /// <summary>
        /// </summary>
        TableSimple1 = 111,
        /// <summary>
        /// </summary>
        TableSimple2 = 112,
        /// <summary>
        /// </summary>
        TableSimple3 = 113,
        /// <summary>
        /// </summary>
        TableSubtle1 = 148,
        /// <summary>
        /// </summary>
        TableSubtle2 = 149,
        /// <summary>
        /// </summary>
        TableTheme = 155,
        /// <summary>
        /// </summary>
        TableWeb1 = 150,
        /// <summary>
        /// </summary>
        TableWeb2 = 151,
        /// <summary>
        /// </summary>
        TableWeb3 = 152,

        #region New table styles
        
        /// <summary>
        /// Plain Table 1
        /// </summary>
        PlainTable1 = 267,
        /// <summary>
        /// Plain Table 2
        /// </summary>
        PlainTable2 = 268,
        /// <summary>
        /// Plain Table 3
        /// </summary>
        PlainTable3 = 269,
        /// <summary>
        /// Plain Table 4
        /// </summary>
        PlainTable4 = 270,
        /// <summary>
        /// Plain Table 5
        /// </summary>
        PlainTable5 = 271,
        /// <summary>
        /// Table Grid Light
        /// </summary>
        TableGridLight = 272,
        /// <summary>
        /// Grid Table 1 Light
        /// </summary>
        GridTable1Light = 273,
        /// <summary>
        /// Grid Table 2
        /// </summary>
        GridTable2 = 274,
        /// <summary>
        /// Grid Table 3
        /// </summary>
        GridTable3 = 275,
        /// <summary>
        /// Grid Table 4
        /// </summary>
        GridTable4 = 276,
        /// <summary>
        /// Grid Table 5 Dark
        /// </summary>
        GridTable5Dark = 277,
        /// <summary>
        /// Grid Table 6 Colorful
        /// </summary>
        GridTable6Colorful = 278,
        /// <summary>
        /// Grid Table 7 Colorful
        /// </summary>
        GridTable7Colorful = 279,
        /// <summary>
        /// Grid Table 1 Light - Accent 1
        /// </summary>
        GridTable1LightAccent1 = 280,
        /// <summary>
        /// Grid Table 2 - Accent 1
        /// </summary>
        GridTable2Accent1 = 281,
        /// <summary>
        /// Grid Table 3 - Accent 1
        /// </summary>
        GridTable3Accent1 = 282,
        /// <summary>
        /// Grid Table 4 - Accent 1
        /// </summary>
        GridTable4Accent1 = 283,
        /// <summary>
        /// Grid Table 5 Dark - Accent 1
        /// </summary>
        GridTable5DarkAccent1 = 284,
        /// <summary>
        /// Grid Table 6 Colorful - Accent 1
        /// </summary>
        GridTable6ColorfulAccent1 = 285,
        /// <summary>
        /// Grid Table 7 Colorful - Accent 1
        /// </summary>
        GridTable7ColorfulAccent1 = 286,
        /// <summary>
        /// Grid Table 1 Light - Accent 2
        /// </summary>
        GridTable1LightAccent2 = 287,
        /// <summary>
        /// Grid Table 2 - Accent 2
        /// </summary>
        GridTable2Accent2 = 288,
        /// <summary>
        /// Grid Table 3 - Accent 2
        /// </summary>
        GridTable3Accent2 = 289,
        /// <summary>
        /// Grid Table 4 - Accent 2
        /// </summary>
        GridTable4Accent2 = 290,
        /// <summary>
        /// Grid Table 5 Dark - Accent 2
        /// </summary>
        GridTable5DarkAccent2 = 291,
        /// <summary>
        /// Grid Table 6 Colorful - Accent 2
        /// </summary>
        GridTable6ColorfulAccent2 = 292,
        /// <summary>
        /// Grid Table 7 Colorful - Accent 2
        /// </summary>
        GridTable7ColorfulAccent2 = 293,
        /// <summary>
        /// Grid Table 1 Light - Accent 3
        /// </summary>
        GridTable1LightAccent3 = 294,
        /// <summary>
        /// Grid Table 2 - Accent 3
        /// </summary>
        GridTable2Accent3 = 295,
        /// <summary>
        /// Grid Table 3 - Accent 3
        /// </summary>
        GridTable3Accent3 = 296,
        /// <summary>
        /// Grid Table 4 - Accent 3
        /// </summary>
        GridTable4Accent3 = 297,
        /// <summary>
        /// Grid Table 5 Dark - Accent 3
        /// </summary>
        GridTable5DarkAccent3 = 298,
        /// <summary>
        /// Grid Table 6 Colorful - Accent 3
        /// </summary>
        GridTable6ColorfulAccent3 = 299,
        /// <summary>
        /// Grid Table 7 Colorful - Accent 3
        /// </summary>
        GridTable7ColorfulAccent3 = 300,
        /// <summary>
        /// Grid Table 1 Light - Accent 4
        /// </summary>
        GridTable1LightAccent4 = 301,
        /// <summary>
        /// Grid Table 2 - Accent 4
        /// </summary>
        GridTable2Accent4 = 302,
        /// <summary>
        /// Grid Table 3 - Accent 4
        /// </summary>
        GridTable3Accent4 = 303,
        /// <summary>
        /// Grid Table 4 - Accent 4
        /// </summary>
        GridTable4Accent4 = 304,
        /// <summary>
        /// Grid Table 5 Dark - Accent 4
        /// </summary>
        GridTable5DarkAccent4 = 305,
        /// <summary>
        /// Grid Table 6 Colorful - Accent 4
        /// </summary>
        GridTable6ColorfulAccent4 = 306,
        /// <summary>
        /// Grid Table 7 Colorful - Accent 4
        /// </summary>
        GridTable7ColorfulAccent4 = 307,
        /// <summary>
        /// Grid Table 1 Light - Accent 5
        /// </summary>
        GridTable1LightAccent5 = 308,
        /// <summary>
        /// Grid Table 2 - Accent 5
        /// </summary>
        GridTable2Accent5 = 309,
        /// <summary>
        /// Grid Table 3 - Accent 5
        /// </summary>
        GridTable3Accent5 = 310,
        /// <summary>
        /// Grid Table 4 - Accent 5
        /// </summary>
        GridTable4Accent5 = 311,
        /// <summary>
        /// Grid Table 5 Dark - Accent 5
        /// </summary>
        GridTable5DarkAccent5 = 312,
        /// <summary>
        /// Grid Table 6 Colorful - Accent 5
        /// </summary>
        GridTable6ColorfulAccent5 = 313,
        /// <summary>
        /// Grid Table 7 Colorful - Accent 5
        /// </summary>
        GridTable7ColorfulAccent5 = 314,
        /// <summary>
        /// Grid Table 1 Light - Accent 6
        /// </summary>
        GridTable1LightAccent6 = 315,
        /// <summary>
        /// Grid Table 2 - Accent 6
        /// </summary>
        GridTable2Accent6 = 316,
        /// <summary>
        /// Grid Table 3 - Accent 6
        /// </summary>
        GridTable3Accent6 = 317,
        /// <summary>
        /// Grid Table 4 - Accent 6
        /// </summary>
        GridTable4Accent6 = 318,
        /// <summary>
        /// Grid Table 5 Dark - Accent 6
        /// </summary>
        GridTable5DarkAccent6 = 319,
        /// <summary>
        /// Grid Table 6 Colorful - Accent 6
        /// </summary>
        GridTable6ColorfulAccent6 = 320,
        /// <summary>
        /// Grid Table 7 Colorful - Accent 6
        /// </summary>
        GridTable7ColorfulAccent6 = 321,
        /// <summary>
        /// List Table 1 Light
        /// </summary>
        ListTable1Light = 322,
        /// <summary>
        /// List Table 2
        /// </summary>
        ListTable2 = 323,
        /// <summary>
        /// List Table 3
        /// </summary>
        ListTable3 = 324,
        /// <summary>
        /// List Table 4
        /// </summary>
        ListTable4 = 325,
        /// <summary>
        /// List Table 5 Dark
        /// </summary>
        ListTable5Dark = 326,
        /// <summary>
        /// List Table 6 Colorful
        /// </summary>
        ListTable6Colorful = 327,
        /// <summary>
        /// List Table 7 Colorful
        /// </summary>
        ListTable7Colorful = 328,
        /// <summary>
        /// List Table 1 Light - Accent 1
        /// </summary>
        ListTable1LightAccent1 = 329,
        /// <summary>
        /// List Table 2 - Accent 1
        /// </summary>
        ListTable2Accent1 = 330,
        /// <summary>
        /// List Table 3 - Accent 1
        /// </summary>
        ListTable3Accent1 = 331,
        /// <summary>
        /// List Table 4 - Accent 1
        /// </summary>
        ListTable4Accent1 = 332,
        /// <summary>
        /// List Table 5 Dark - Accent 1
        /// </summary>
        ListTable5DarkAccent1 = 333,
        /// <summary>
        /// List Table 6 Colorful - Accent 1
        /// </summary>
        ListTable6ColorfulAccent1 = 334,
        /// <summary>
        /// List Table 7 Colorful - Accent 1
        /// </summary>
        ListTable7ColorfulAccent1 = 335,
        /// <summary>
        /// List Table 1 Light - Accent 2
        /// </summary>
        ListTable1LightAccent2 = 336,
        /// <summary>
        /// List Table 2 - Accent 2
        /// </summary>
        ListTable2Accent2 = 337,
        /// <summary>
        /// List Table 3 - Accent 2
        /// </summary>
        ListTable3Accent2 = 338,
        /// <summary>
        /// List Table 4 - Accent 2
        /// </summary>
        ListTable4Accent2 = 339,
        /// <summary>
        /// List Table 5 Dark - Accent 2
        /// </summary>
        ListTable5DarkAccent2 = 340,
        /// <summary>
        /// List Table 6 Colorful - Accent 2
        /// </summary>
        ListTable6ColorfulAccent2 = 341,
        /// <summary>
        /// List Table 7 Colorful - Accent 2
        /// </summary>
        ListTable7ColorfulAccent2 = 342,
        /// <summary>
        /// List Table 1 Light - Accent 3
        /// </summary>
        ListTable1LightAccent3 = 343,
        /// <summary>
        /// List Table 2 - Accent 3
        /// </summary>
        ListTable2Accent3 = 344,
        /// <summary>
        /// List Table 3 - Accent 3
        /// </summary>
        ListTable3Accent3 = 345,
        /// <summary>
        /// List Table 4 - Accent 3
        /// </summary>
        ListTable4Accent3 = 346,
        /// <summary>
        /// List Table 5 Dark - Accent 3
        /// </summary>
        ListTable5DarkAccent3 = 347,
        /// <summary>
        /// List Table 6 Colorful - Accent 3
        /// </summary>
        ListTable6ColorfulAccent3 = 348,
        /// <summary>
        /// List Table 7 Colorful - Accent 3
        /// </summary>
        ListTable7ColorfulAccent3 = 349,
        /// <summary>
        /// List Table 1 Light - Accent 4
        /// </summary>
        ListTable1LightAccent4 = 350,
        /// <summary>
        /// List Table 2 - Accent 4
        /// </summary>
        ListTable2Accent4 = 351,
        /// <summary>
        /// List Table 3 - Accent 4
        /// </summary>
        ListTable3Accent4 = 352,
        /// <summary>
        /// List Table 4 - Accent 4
        /// </summary>
        ListTable4Accent4 = 353,
        /// <summary>
        /// List Table 5 Dark - Accent 4
        /// </summary>
        ListTable5DarkAccent4 = 354,
        /// <summary>
        /// List Table 6 Colorful - Accent 4
        /// </summary>
        ListTable6ColorfulAccent4 = 355,
        /// <summary>
        /// List Table 7 Colorful - Accent 4
        /// </summary>
        ListTable7ColorfulAccent4 = 356,
        /// <summary>
        /// List Table 1 Light - Accent 5
        /// </summary>
        ListTable1LightAccent5 = 357,
        /// <summary>
        /// List Table 2 - Accent 5
        /// </summary>
        ListTable2Accent5 = 358,
        /// <summary>
        /// List Table 3 - Accent 5
        /// </summary>
        ListTable3Accent5 = 359,
        /// <summary>
        /// List Table 4 - Accent 5
        /// </summary>
        ListTable4Accent5 = 360,
        /// <summary>
        /// List Table 5 Dark - Accent 5
        /// </summary>
        ListTable5DarkAccent5 = 361,
        /// <summary>
        /// List Table 6 Colorful - Accent 5
        /// </summary>
        ListTable6ColorfulAccent5 = 362,
        /// <summary>
        /// List Table 7 Colorful - Accent 5
        /// </summary>
        ListTable7ColorfulAccent5 = 363,
        /// <summary>
        /// List Table 1 Light - Accent 6
        /// </summary>
        ListTable1LightAccent6 = 364,
        /// <summary>
        /// List Table 2 - Accent 6
        /// </summary>
        ListTable2Accent6 = 365,
        /// <summary>
        /// List Table 3 - Accent 6
        /// </summary>
        ListTable3Accent6 = 366,
        /// <summary>
        /// List Table 4 - Accent 6
        /// </summary>
        ListTable4Accent6 = 367,
        /// <summary>
        /// List Table 5 Dark - Accent 6
        /// </summary>
        ListTable5DarkAccent6 = 368,
        /// <summary>
        /// List Table 6 Colorful - Accent 6
        /// </summary>
        ListTable6ColorfulAccent6 = 369,
        /// <summary>
        /// List Table 7 Colorful - Accent 6
        /// </summary>
        ListTable7ColorfulAccent6 = 370,
        #endregion New table styles

        /// <summary>
        /// The Mention style.
        /// </summary>
        Mention = 372,

        /// <summary>
        /// The SmartHyperlink style.
        /// </summary>
        SmartHyperlink = 373,

        /// <summary>
        /// The Hashtag style.
        /// </summary>
        Hashtag = 374,

        /// <summary>
        /// The UnresolvedMention style.
        /// </summary>
        UnresolvedMention = 375,

        /// <summary>
        /// A user defined style.
        /// </summary>
        User = 0x0FFE,
        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        Nil = 0x2FFF
        // WORDSCPP workaround - get rid of cyclic dependencies between StyleIdentifier and StyleIndex
        // Nil = StyleIndex.Nil
    }
}
