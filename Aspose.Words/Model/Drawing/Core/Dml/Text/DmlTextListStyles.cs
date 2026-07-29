// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// 21.1.2.4.12 lstStyle (Text List Styles)
    /// This element specifies the list of styles associated with this body of text.
    /// </summary>
    internal class DmlTextListStyles
    {
        internal DmlTextListStyles()
        {
            for (int i = 0; i < mListLevelStyles.Length; i++)
            {
                mListLevelStyles[i] = new DmlParagraphProperties();
                // List styles usually does not have explicitly specified list level.
                // That is why specify list level using inherited attr.
                DmlParagraphProperties levelProperties = new DmlParagraphProperties();
                mListLevelStyles[i].SetParentProperties(levelProperties);
                levelProperties.Level = i;
                if (i > 0)
                {
                    levelProperties.SetParentProperties(mListLevelStyles[i - 1]);
                    mListLevelStyles[i].DefaultRunProperties.SetParentProperties(
                        mListLevelStyles[i - 1].DefaultRunProperties);
                }
            }

            // Create default properties and link properties to them
            mDefaultParagraphProperties = new DmlParagraphProperties();
            // For first level it is not required to set list level because it is zero by default.
            mListLevelStyles[0].SetParentProperties(mDefaultParagraphProperties);
            mListLevelStyles[0].DefaultRunProperties.SetParentProperties(
                mDefaultParagraphProperties.DefaultRunProperties);
        }

        internal DmlTextListStyles Clone()
        {
            DmlTextListStyles lhs = (DmlTextListStyles)MemberwiseClone();

            lhs.mDefaultParagraphProperties = mDefaultParagraphProperties.Clone();

            lhs.mListLevelStyles = new DmlParagraphProperties[mListLevelStyles.Length];
            for (int i = 0; i < mListLevelStyles.Length; i++)
                lhs.mListLevelStyles[i] = mListLevelStyles[i].Clone();

            return lhs;
        }

        /// <summary>
        /// Get text list style by level.
        /// </summary>
        /// <param name="level">Is 0-based.</param>
        internal DmlParagraphProperties GetTextListStyle(int level)
        {
            return mListLevelStyles[level];
        }

        internal DmlParagraphProperties DefaultParagraphProperties
        {
            get { return mDefaultParagraphProperties; }
        }

        internal DmlParagraphProperties ListLevel1Style
        {
            get { return mListLevelStyles[0]; }
        }

        internal DmlParagraphProperties ListLevel2Style
        {
            get { return mListLevelStyles[1]; }
        }

        internal DmlParagraphProperties ListLevel3Style
        {
            get { return mListLevelStyles[2]; }
        }

        internal DmlParagraphProperties ListLevel4Style
        {
            get { return mListLevelStyles[3]; }
        }

        internal DmlParagraphProperties ListLevel5Style
        {
            get { return mListLevelStyles[4]; }
        }

        internal DmlParagraphProperties ListLevel6Style
        {
            get { return mListLevelStyles[5]; }
        }

        internal DmlParagraphProperties ListLevel7Style
        {
            get { return mListLevelStyles[6]; }
        }

        internal DmlParagraphProperties ListLevel8Style
        {
            get { return mListLevelStyles[7]; }
        }

        internal DmlParagraphProperties ListLevel9Style
        {
            get { return mListLevelStyles[8]; }
        }

        /// <summary>
        /// Returns <c>true</c> if this property collection does not contain any non-empty styles.
        /// </summary>
        internal bool IsEmpty
        {
            get
            {
                foreach (DmlParagraphProperties properties in mListLevelStyles)
                {
                    if ((properties != null) && !properties.IsEmpty)
                        return false;
                }

                return true;
            }
        }

        private DmlParagraphProperties mDefaultParagraphProperties;
        private DmlParagraphProperties[] mListLevelStyles = new DmlParagraphProperties[9];
    }
}
