// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/02/2019 by Ilya Navrotskiy

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how list levels are indented when document is exporting to <see cref="SaveFormat.Text" /> format.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/save-a-document/">Save a Document</a> documentation article.</para>
    /// </summary>
    public class TxtListIndentation
    {
        /// <summary>
        /// Returns a indentation string for a specified level number. 
        /// </summary>
        internal string GetIndent(int levelNumber)
        {
            if (!HasIndentation || (levelNumber <= 0))
                return "";

            return new string(mCharacter, mCount * levelNumber);
        }

        /// <summary>
        /// Gets or sets how many <see cref="Character"/> to use as indentation per one list level.
        /// The default value is 0, that means no indentation.
        /// </summary>
        public int Count
        {
            get { return mCount; }
            set
            {
                ArgumentUtil.CheckNonNegative(value, "Count");
                mCount = value;
            }
        }

        /// <summary>
        /// Gets or sets which character to use for indenting list levels.
        /// The default value is '\0', that means there is no indentation.
        /// </summary>
        public char Character
        {
            get { return mCharacter; }
            set { mCharacter = value; }
        }

        /// <summary>
        /// Returns true, if this ListIndentation object has non-empty indentation.
        /// </summary>
        private bool HasIndentation
        {
            get { return ((mCount > 0) && (mCharacter != '\0')); }
        }

        private int mCount;
        private char mCharacter;
    }
}
