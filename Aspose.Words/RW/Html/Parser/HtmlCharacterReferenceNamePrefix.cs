// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/04/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// An auxiliary class used by the <see cref="HtmlCharacterReferenceNames"/> data structure.
    /// </summary>
    /// <remarks>
    /// Every instance of this class represents a state of the finite-state automaton used by the data structure.
    /// </remarks>
    internal class HtmlCharacterReferenceNamePrefix
    {
        /// <summary>
        /// The characters associated with this prefix (state).
        /// </summary>
        internal string Characters
        {
            get { return mCharacters; }
            set
            {
                Debug.Assert(value != null);
                mCharacters = value;
            }
        }

        /// <summary>
        /// The flag indicating whether this prefix (state) is final.
        /// </summary>
        internal bool IsFinal
        {
            get { return mIsFinal; }
            set { mIsFinal = value; }
        }

        private string mCharacters = string.Empty;

        /// <summary>
        /// The flag indicating whether this prefix (state) is final.
        /// </summary>
        /// <remarks>
        /// All new prefixes (states) are initially considered final; it simplifies adding new prefixes (states)
        /// to the <see cref="HtmlCharacterReferenceNames"/> data structure.
        /// </remarks>
        private bool mIsFinal = true;
    }
}
