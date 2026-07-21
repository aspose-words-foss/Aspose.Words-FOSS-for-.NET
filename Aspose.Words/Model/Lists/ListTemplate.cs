// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2006 by Roman Korchagin

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Specifies one of the predefined list formats available in Microsoft Word.
    /// </summary>
    /// <remarks>
    /// <p>A list template value is used as a parameter into the
    /// <see cref="ListCollection.Add(ListTemplate)"/> method.</p>
    ///
    /// <p>Aspose.Words list templates correspond to the 21 list templates available
    /// in the Bullets and Numbering dialog box in Microsoft Word 2003.</p>
    /// </remarks>
    public enum ListTemplate
    {
        /// <summary>
        /// <p>Default bulleted list with 9 levels. Bullet of the first level is a disc,
        /// bullet of the second level is a circle, bullet of the third level is a square.
        /// Then formatting repeats for the remaining levels.</p>
        ///
        /// <p>Each level is indented to the right by 0.25" relative to the previous level.</p>
        ///
        /// <p>Corresponds to the 1st bulleted list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        BulletDefault,
        /// <summary>
        /// <p>Same as <see cref="BulletDefault"/>.</p>
        /// <p>Corresponds to the 1st bulleted list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        BulletDisk = BulletDefault,
        /// <summary>
        /// <p>The bullet of the first level is a circle. The remaining levels are same as in <see cref="BulletDefault"/>.</p>
        /// <p>Corresponds to the 2nd bulleted list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        BulletCircle,
        /// <summary>
        /// <p>The bullet of the first level is a square. The remaining levels are same as in <see cref="BulletDefault"/>.</p>
        /// <p>Corresponds to the 3rd bulleted list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        BulletSquare,

        // bullet 4 here we don't have yet, it has a picture.

        /// <summary>
        /// <p>The bullet of the first level is a 4-diamond Wingding character. The remaining levels are same as in <see cref="BulletDefault"/>.</p>
        /// <p>Corresponds to the 5th bulleted list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        BulletDiamonds,
        /// <summary>
        /// <p>The bullet of the first level is an arrow head Wingding character. The remaining levels are same as in <see cref="BulletDefault"/>.</p>
        /// <p>Corresponds to the 6th bulleted list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        BulletArrowHead,
        /// <summary>
        /// <p>The bullet of the first level is a tick Wingding character. The remaining levels are same as in <see cref="BulletDefault"/>.</p>
        /// <p>Corresponds to the 7th bulleted list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        BulletTick,

        /// <summary>
        /// <p>Default numbered list with 9 levels. Arabic numbering (1., 2., 3., ...) for the first level,
        /// lowercase letter numbering (a., b., c., ...) for the second level,
        /// lowercase roman numbering (i., ii., iii., ...) for the third level.
        /// Then formatting repeats for the remaining levels.</p>
        ///
        /// <p>Each level is indented to the right by 0.25" relative to the previous level.</p>
        ///
        /// <p>Corresponds to the 1st numbered list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        NumberDefault,
        /// <summary>
        /// <p>Same as <see cref="NumberDefault"/>.</p>
        /// <p>Corresponds to the 1st numbered list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        NumberArabicDot = NumberDefault,
        /// <summary>
        /// <p>The number of the first level is "1)". The remaining levels are same as in <see cref="NumberDefault"/>.</p>
        /// <p>Corresponds to the 2nd numbered list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        NumberArabicParenthesis,
        /// <summary>
        /// <p>The number of the first level is "I.". The remaining levels are same as in <see cref="NumberDefault"/>.</p>
        /// <p>Corresponds to the 3rd numbered list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        NumberUppercaseRomanDot,
        /// <summary>
        /// <p>The number of the first level is "A.". The remaining levels are same as in <see cref="NumberDefault"/>.</p>
        /// <p>Corresponds to the 4th numbered list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        NumberUppercaseLetterDot,
        /// <summary>
        /// <p>The number of the first level is "a)". The remaining levels are same as in <see cref="NumberDefault"/>.</p>
        /// <p>Corresponds to the 5th numbered list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        NumberLowercaseLetterParenthesis,
        /// <summary>
        /// <p>The number of the first level is "a.". The remaining levels are same as in <see cref="NumberDefault"/>.</p>
        /// <p>Corresponds to the 6th numbered list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        NumberLowercaseLetterDot,
        /// <summary>
        /// <p>The number of the first level is "i.". The remaining levels are same as in <see cref="NumberDefault"/>.</p>
        /// <p>Corresponds to the 7th numbered list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        NumberLowercaseRomanDot,

        /// <summary>
        /// <p>An outline list with levels numbered "1), a), i), (1), (a), (i), 1., a., i.".</p>
        /// <p>Corresponds to the 1st outline list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        OutlineNumbers,
        /// <summary>
        /// <p>An outline list with levels are numbered "1., 1.1., 1.1.1, ...".</p>
        /// <p>Corresponds to the 2nd outline list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        OutlineLegal,
        /// <summary>
        /// <p>An outline lists with various bullets for different levels.</p>
        /// <p>Corresponds to the 3rd outline list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        OutlineBullets,
        /// <summary>
        /// <p>An outline list with levels linked to Heading styles.</p>
        /// <p>Corresponds to the 4th outline list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        OutlineHeadingsArticleSection,
        /// <summary>
        /// <p>An outline list with levels linked to Heading styles.</p>
        /// <p>Corresponds to the 5th outline list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        OutlineHeadingsLegal,
        /// <summary>
        /// <p>An outline list with levels linked to Heading styles.</p>
        /// <p>Corresponds to the 6th outline list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        OutlineHeadingsNumbers,
        /// <summary>
        /// <p>An outline list with levels linked to Heading styles.</p>
        /// <p>Corresponds to the 7th outline list template in the Bullets and Numbering dialog box in Microsoft Word.</p>
        /// </summary>
        OutlineHeadingsChapter
    }
}
