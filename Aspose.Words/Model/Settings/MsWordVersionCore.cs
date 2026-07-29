// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2013 by Denis Darkin

namespace Aspose.Words
{
    /// <summary>Specifies desired behavior of AW DOM as well as desired compatibility of documents when exported to MS Formats.</summary>
    /// <remarks>
    /// During OOXML document reading sometimes we're able to identify MS Word version, which allows us to better roundtrip and save
    /// the document. Each MS Word version has more features than previous one, so we can define order between versions.
    /// However during loading we can only guess a minimum version,
    /// meaning that if we identified document as containing Word2007 features, it might be originating from 
    /// any MS Word version not earlier than Word2007 but from later versions as well e.g. Word2010 and Word2013.
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    internal enum MsWordVersionCore
    {
        /// <summary>
        /// Unspecified document version means all AW teams are free to proceed with their custom behavior logic since user
        /// does not specify preferences for MS Version and the document does not contain it as well.
        /// </summary>
        Unspecified = 0,

        // DD: These commented ones are for historical and info purpose only, we're not going to detect them or expose to public
        // Word1990 = 1, DD actually 1.1, first version ever.
        // Word1992 = 2,
        // Word1993 = 6, Wikipedia comment: Jumped from 2.0 to 6.0 so that it would have the same version number as the MS-DOS and Macintosh versions
        // Word1995 = 7,

        Word1997 = 8,

        Word2000 = 9,

        Word2002 = 10,

        Word2003 = 11,

        Word2007 = 12,
            
        Word2010 = 14,
            
        Word2013 = 15,

        Word2016 = 16,

        Word2019 = 17
    }
}
