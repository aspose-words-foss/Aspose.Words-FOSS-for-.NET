// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2008 by Roman Korchagin

using Aspose.Words.Styles;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Extracted from DOCX styles reader.
    /// </summary>
    internal static class NrxStyleUtil
    {
        /// <summary>
        /// Tries to recognize a style name read from a DOCX, WordML or RTF file
        /// and sets the name, style identifier, istd and next istd correctly for the model.
        /// </summary>
        /// <param name="name">Locale independent "standard" style name as read from a file.</param>
        /// <param name="styles">Collection of document styles (used to get next free istd value).</param>
        /// <param name="isForceCustom">True to force the style to be a custom style.</param>
        /// <param name="tryLowerCase">True to try style name in lower case when detect built-in styles.</param>
        /// <param name="isReplaceExistingBuiltInStyle">True to replace existing BuiltIn style. Otherwise, makes it custom style.</param>
        /// <param name="style">The style to update.</param>
        internal static void SetStyleNameAndIdentifiers(
            string name,
            StyleCollection styles,
            bool isForceCustom,
            bool tryLowerCase,
            bool isReplaceExistingBuiltInStyle,
            Style style)
        {
            ArgumentUtil.CheckHasChars(name, "name");
            ArgumentUtil.CheckNotNull(styles, "styles");
            ArgumentUtil.CheckNotNull(style, "style");

            // Try to recognize a built-in style by the primary name.
            StyleIdentifier styleIdentifier = StyleIdentifier.User;

            if (!isForceCustom)
            {
                styleIdentifier = StyleConvertUtil.XmlToStyleIdentifier(name);

                if(tryLowerCase && (styleIdentifier == StyleIdentifier.User))
                    styleIdentifier = StyleConvertUtil.XmlToStyleIdentifier(name.ToLower());
            }

            // Some built-in style names on file are different from what we use in the model, translate such names.
            if (styleIdentifier != StyleIdentifier.User)
                name = StyleConvertUtil.XmlToStyleName(name);

            // In some ugly documents (such as TestDefect1478 there are several styles with the same name.
            // If first is recognized as a built-in style, we have to recognize others as custom styles.
            if (styles.Contains(styleIdentifier))
            {
                // WORDSNET-22587 Actually, Word takes only last occured built-in style. But for the moment this done
                // only for DOCX and WML and all other formats should be checked additionally because of various
                // specifics in different formats. For example, see RK comments in call of this method for ODT.
                if (isReplaceExistingBuiltInStyle)
                {
                    Style existingStyle = styles.GetBySti(styleIdentifier, false);
                    Debug.Assert(existingStyle != null);

                    // WORDSNET-26324 Handle duplicate name in styles with different types.
                    // There are could be many combinations, handle this particular case only.
                    if ((existingStyle.Name == name) &&
                        (existingStyle.Type == StyleType.Paragraph) &&
                        (style.Type == StyleType.Character))
                    {
                        name += " Char";
                        styleIdentifier = StyleIdentifier.User;
                    }
                    else
                    {
                        styles.RemoveCore(existingStyle);
                    }
                }
                else
                {
                    styleIdentifier = StyleIdentifier.User;
                }
            }

            int istd = StyleIndex.StyleIdentifierToIstd(styleIdentifier);
            if (istd == StyleIndex.Nil)
                istd = styles.GetNextFreeIstd();

            style.SetStyleIdentifier(styleIdentifier);
            style.SetNameCore(name);
            style.SetIstd(istd);
            // We also have to set next istd because by default it should point to the style itself.
            style.NextIstd = istd;
        }
    }
}
