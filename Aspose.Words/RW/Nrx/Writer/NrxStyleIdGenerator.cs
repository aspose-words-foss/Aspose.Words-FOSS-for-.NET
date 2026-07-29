// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/01/2007 by Roman Korchagin

using System.Text.RegularExpressions;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Nrx.Writer
{
    /// <summary>
    /// Common code for HTML, WordML and DOCX that deals with writing style names to the output file.
    /// Original code by VA, extracted from WmlContext.
    /// </summary>
    internal class NrxStyleIdGenerator
    {
        internal NrxStyleIdGenerator(StyleCollection styles)
        {
            Debug.Assert(styles != null);

            int stylesCount = styles.Count;
            mStyleIdByIstdTable = new IntToObjDictionary<string>(stylesCount);
            mStyleAliasesIdsByIstdTable = new IntToObjDictionary<string[]>();
            mStyleIdTable = new CaseInsensitiveHashSet();

            for (int i = 0; i < stylesCount; ++i)
                AddStyleId(styles[i]);
        }

        /// <summary>
        /// Generates unique WordML-compliant style identifier for the specified style and adds it to styleId-by-istd table.
        /// </summary>
        private void AddStyleId(Style style)
        {
            string styleName;

            if (style.BuiltIn)
            {
                // This gives a locale-independent DOCX name of a built-in style, suitable for most cases.
                styleName = StyleConvertUtil.StyleIdentifierToXml(style.StyleIdentifier, style.Name);
                // However, some DOCX names, such as "annotation reference" are not what we need.
                // We do a trick of converting a DOCX name to a DOC/model name such as "Comment Reference".
                styleName = StyleConvertUtil.XmlToStyleName(styleName);
            }
            else
            {
                styleName = style.Name;
            }

            // If the resulting style name is an empty string, then "a" is taken as a name.
            int styleIstd = style.Istd;
            string styleId = GenerateStyleIdByName("a", styleName);
            mStyleIdByIstdTable.Add(styleIstd, styleId);

            // Style aliases if any present. They are comma separated.
            string aliases = style.Styles.GetAliases(style, false);
            if (StringUtil.HasChars(aliases))
            {
                string[] aliasesArray = aliases.Split(',');
                for (int i = aliasesArray.Length; --i >= 0; )
                    aliasesArray[i] = GenerateStyleIdByName(styleId, aliasesArray[i]);

                mStyleAliasesIdsByIstdTable.Add(styleIstd, aliasesArray);
            }
        }

        /// <summary>
        /// Removes prohibited characters from the name, ensures it is not empty and unique.
        /// Empty names are filled with defaultPrefix and then uniqueness is assured as usual.
        /// </summary>
        private string GenerateStyleIdByName(string defaultPrefix, string styleName)
        {
            // Remove all characters from the style name that are not -, A-Z, a-z, 0-9.
            string styleId = gRegexMakeStyleId.Replace(styleName, string.Empty);

            if (styleId == string.Empty)
                styleId = defaultPrefix;

            styleId = EnsureUniqueStyleId(styleId);
            mStyleIdTable.Add(styleId);
            return styleId;
        }

        /// <summary>
        /// If the styleid already exists in the document, try to add a number to it,
        /// starting from 0, until the resulting name is unique.
        /// </summary>
        private string EnsureUniqueStyleId(string styleId)
        {
            ArgumentUtil.CheckHasChars(styleId, "styleId");

            string newStyleId = styleId;

            int k = 0;
            while (mStyleIdTable.Contains(newStyleId))
            {
                newStyleId = styleId + k;
                k++;
            }

            return newStyleId;
        }

        /// <summary>
        /// Gets style id by the style's istd.
        /// </summary>
        internal string GetStyleId(int istd)
        {
            return mStyleIdByIstdTable[istd];
        }

        /// <summary>
        /// Gets style aliases ids by the style's istd.
        /// Maybe null if the style has no aliases.
        /// </summary>
        internal string[] GetStyleAliasesIds(int istd)
        {
            return mStyleAliasesIdsByIstdTable[istd];
        }

        /// <summary>
        /// The key is style istd, the value is a string WordML-compliant style identifier.
        /// </summary>
        private readonly IntToObjDictionary<string> mStyleIdByIstdTable;
        /// <summary>
        /// The key is style istd, the value is the array of aliases in WordML-compliant form.
        /// </summary>
        private readonly IntToObjDictionary<string[]> mStyleAliasesIdsByIstdTable;
        /// <summary>
        /// This case-insensitive hashset is used to ensure case-insensitivity of WordML style id generation.
        /// For example, 'name' and 'Name' are considered to be the same style id.
        /// Contains strings which are WordML-compliant style identifiers.
        /// </summary>
        private readonly ISetGeneric<string> mStyleIdTable;

        private static readonly Regex gRegexMakeStyleId = new Regex("[^-a-zA-Z0-9]*", RegexOptions.Singleline | RegexOptions.Compiled);
    }
}
