// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/05/2006 by Roman Korchagin

using Aspose.Collections;
using Aspose.Words.Styles;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Umbrella class for storing methods and constants for managing StyleIndex in styles.
    ///  StyleIndexor sti, seems to be the most important thing that unites and defines istd and names for Built-In styles.
    /// DD: I will move methods here from elsewhere for better cohesion. Someday we will encapsulate all style-id related stuff here.
    /// </summary>
    /// <remarks>
    /// Contains: 
    /// 1. static methods that allow conversion of regular to built in styles
    /// 
    /// 2. Constants that map between sti's of built-in style to ISTD (index into the style sheet) for some built in styles.
    /// Other built in styles have no standard istd values for them.
    /// In Word 97 to 2007 there are 15 fixed istd styles. Last two entries are unused according to the SPEC.
    /// </remarks>
    [CppConstexpr]
    internal static class StyleIndex
    {
        /// <summary>
        /// Returns <see cref="Style.StyleIdentifier"/> from style name for MS Word 2007 English localization.
        /// If style is not recognized as Built-in by its name, returns User style.   
        /// </summary>
        /// <remarks>This function may fail for any-non English localization.</remarks>
        internal static StyleIdentifier BuiltInStyleNameEnglish(string name)
        {
            return StyleConvertUtil.XmlToStyleIdentifier(StyleConvertUtil.StyleNameToXml(name));
        }

        /// <summary>
        /// Removes built-in values and make the style regular.
        /// </summary>
        internal static void ConvertToRegularStyle(Style style, Style oldStyle)
        {
            Debug.Assert(style.Styles != null);

            style.SetStyleIdentifier(StyleIdentifier.User, true);
            
            // remove style special values: reserved istd, sti, update caches.
            if (oldStyle != null)
                style.SetIstd(oldStyle.Istd, true);
            else if (IsIstdReserved(style.Istd))
                style.SetIstd(style.Styles.GetNextFreeIstd(), true);

        }

        /// <summary>
        /// Converts the style into Built-In.
        /// Returns true if istd change happened.
        /// </summary>
        internal static bool ConvertToBuiltInStyle(Style style, StyleIdentifier sti, Style oldStyle, bool updateCollection)
        {
            bool result = false;

            style.SetStyleIdentifier(sti, updateCollection);
            
            int defIstd = StyleIdentifierToIstd(sti);
            if (defIstd != Nil) // now check if built-in style has reserved istd.
            {
                style.SetIstd(defIstd, updateCollection);
                result = true;
            }
            else if (oldStyle != null) // else check if we need to override old style with new style.
            {
                style.SetIstd(oldStyle.Istd, updateCollection);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Converts a style identifier into style index (istd). There is only a handful of styles
        /// with fixed istd values, for all others returns StyleIndex.Nil.
        /// </summary>
        internal static int StyleIdentifierToIstd(StyleIdentifier sti)
        {
            int result = gAllReservedIstd[(int)sti];
            if (IntToIntDictionary.IsNullSubstitute(result))
                result = Nil;

            return result;
        }

        /// <summary>
        /// Returns true if given istd is reserved for a built-in style.
        /// </summary>
        internal static bool IsIstdReserved(int istd)
        {
            return gAllReservedIstd.ContainsValue(istd);
        }

        /// <summary>
        /// Returns a value indicating whether the specified style identifier corresponds to a heading style.
        /// </summary>
        internal static bool IsHeadingIstd(int istd)
        {
            return ((istd >= Heading1) && (istd <= Heading9));
        }

        /// <summary>
        /// Special handling sometimes is required for default paragraph font style and table normal because they cannot be modified in Microsoft Word. 
        /// </summary>
        internal static bool IsNonModifiable(Style srcStyle)
        {
            return (srcStyle.StyleIdentifier == StyleIdentifier.DefaultParagraphFont) ||
                   (srcStyle.StyleIdentifier == StyleIdentifier.TableNormal);
        }

        internal const int Normal = 0;
        internal const int Heading1 = 1;
        internal const int Heading2 = 2;
        internal const int Heading3 = 3;
        internal const int Heading4 = 4;
        internal const int Heading5 = 5;
        internal const int Heading6 = 6;
        internal const int Heading7 = 7;
        internal const int Heading8 = 8;
        internal const int Heading9 = 9;
        internal const int DefaultParagraphFont = 10;
        internal const int TableNormal = 11;
        internal const int NoList = 12;

        /// <summary>
        /// Nil style index.
        /// </summary>
        /// <remarks>
        /// Note, this value is different than Nil value used for styles in Word document.
        /// Actually this could be something like Int32.MaxValue but lets have some limitation for a while.
        /// </remarks>
        internal const int Nil = 0x2FFF;

        /// <summary>
        /// Maximal style index.
        /// </summary>
        /// <remarks>
        /// Note, this value is different than Limit value used for styles in Word document.
        /// </remarks>
        internal const int Limit = 0x2FFE;

        private static readonly IntToIntDictionary gAllReservedIstd = new IntToIntDictionary();
        
        static StyleIndex()
        {
            gAllReservedIstd.Add((int)StyleIdentifier.Normal, Normal);
            gAllReservedIstd.Add((int)StyleIdentifier.Heading1, Heading1);
            gAllReservedIstd.Add((int)StyleIdentifier.Heading2, Heading2);
            gAllReservedIstd.Add((int)StyleIdentifier.Heading3, Heading3);
            gAllReservedIstd.Add((int)StyleIdentifier.Heading4, Heading4);
            gAllReservedIstd.Add((int)StyleIdentifier.Heading5, Heading5);
            gAllReservedIstd.Add((int)StyleIdentifier.Heading6, Heading6);
            gAllReservedIstd.Add((int)StyleIdentifier.Heading7, Heading7);
            gAllReservedIstd.Add((int)StyleIdentifier.Heading8, Heading8);
            gAllReservedIstd.Add((int)StyleIdentifier.Heading9, Heading9);
            gAllReservedIstd.Add((int)StyleIdentifier.DefaultParagraphFont, DefaultParagraphFont);
            gAllReservedIstd.Add((int)StyleIdentifier.TableNormal, TableNormal);
            gAllReservedIstd.Add((int)StyleIdentifier.NoList, NoList);
        }
    }
}
