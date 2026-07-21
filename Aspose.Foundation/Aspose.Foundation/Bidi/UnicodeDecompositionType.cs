// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

namespace Aspose.Bidi
{
    /// <summary>
    /// The type of Unicode character decomposition.
    /// </summary>
    public enum UnicodeDecompositionType
    {
        ///<summary>A base form or no special variant.</summary>
        None,
        
        ///<summary>A font variant (e.g. a blackletter form).</summary>
        Font,
        
        ///<summary>A no-break version of a space or hyphen.</summary>
        NoBreak,
        
        ///<summary>An initial presentation form (Arabic).</summary>
        Initial,
        
        ///<summary>A medial presentation form (Arabic).</summary>
        Medial,
        
        ///<summary>A final presentation form (Arabic).</summary>
        Final,
        
        ///<summary>An isolated presentation form (Arabic).</summary>
        Isolated,
        
        ///<summary>An encircled form.</summary>
        Circle,
        
        ///<summary>A superscript form.</summary>
        Super,
        
        ///<summary>A subscript form.</summary>
        Sub,
        
        ///<summary>A vertical layout presentation form.</summary>
        Vertical,
        
        ///<summary>A wide (or zenkaku) compatibility character.</summary>
        Wide,
        
        ///<summary>A narrow (or hankaku) compatibility character.></summary>
        Narrow,
        
        ///<summary>A small variant form (CNS compatibility).</summary>
        Small,
        
        ///<summary>A CJK squared font variant.</summary>
        Square,
        
        ///<summary>A vulgar fraction form.</summary>
        Fraction,
        
        ///<summary>Otherwise unspecified compatibility character.</summary>
        Compat
    }
}
