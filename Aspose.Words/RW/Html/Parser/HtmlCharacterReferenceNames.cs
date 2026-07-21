// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/04/2013 by Victor Chebotok

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Data structure that maps HTML named references to UTF-16 character sequences.
    /// </summary>
    /// <remarks>Ordinal case-sensitive comparison is used for names.</remarks>
    //
    // A hashtable cannot be used instead of this data stucture, because for each name we need to answer to the following
    // two questions:
    // 1. What UTF-16 characters this name corresponds to? (Hastable can effectively answer this question).
    // 2. Is there a longer name that is prefixed with the specified name? (Hashtable fails to answer this question.
    //
    // The data structure maintains an finite-state automaton that processes a name character by character.
    // The first-level hashtable maps a character being processed to a second-level hashtable.
    // The second-level hashtable maps the current automaton state to the next state.
    // The two levels of hashtables implement the state transition table of the automaton.
    // The automaton begins its work with the initial state.
    // Every state has the following two pieces of information linked with it:
    // 1. An UTF-16 character string, if the state corresponds to a valid HTML character reference name.
    //    If the state does not correspond to a valid name, this string is empty.
    // 2. A flag that indicates whether transitions from this state to other states exist.
    //    When this flag is cleared, it means that there are no names that are longer than the name that
    //    has been parsed up to the moment (final state). Initially every state is final.
    // Conceptually, every state corresponds to a string prefix composed of characters that are already processed by
    // the automaton, and the initial state corresponds to an empty prefix.
    // 
    // When the automaton adds a new name, it follows the state transition table until it reaches a final state. 
    // Note that the initial state is final until no names are added to the data structure. If there are more name characters
    // to be processed, the current state is declared non-final, and new non-final states are added for that characters.
    // The last added state is declared final, and the value provided with the name is linked to that state.
    //
    // When the automaton looks up a name, it follows the state transition table until it reaches a final state or reads
    // all the name characters. If the automaton reaches a final state before it reads all the name characters, then there
    // is no such a name in the data structure. If all the name characters have been read, and the last state is final, then
    // the name is found in the data structure.
    //
    // To clarify the things, consider the following example.
    // The following four names and values are added to the data structure:
    // 1. ant, "A"
    // 2. ante, "B"
    // 3. an, "C"
    // 4. can, "D"
    // In transition tables below the first column corresponds to the first-level hashtable entries. Every row of the transition
    // tables correspond to a second-level hashtable containing state transitions.
    // States are numbered, with zero used for the initial state. For every state the following values are provided: 
    // the state number, the state prefix (the prefix is not actually stored in the state, and is provided for clarity only), 
    // the "final" flag, and the value associated with a name.
    // Transitions are written in the form X->Y, that is a transition from the state X to the state Y.
    //
    // Initial state:
    // the transition table is empty
    // States:
    // 0: "", final, ""
    //
    // After the "ant" word is added. Note that the initial state is no longer final.
    // Transitions:
    // a | 0->1
    // n | 1->2
    // t | 2->3
    // States:
    // 0: "", non-final, ""
    // 1: "a", non-final, ""
    // 2: "an", non-final, ""
    // 3: "ant", final, "A"
    //
    // After the "ante" word is added. Note that the data structure reuses the existing states when name prefixes match.
    // Also note that the 3-rd state is no longer final.
    // Transitions:
    // a | 0->1
    // n | 1->2
    // t | 2->3
    // e | 3->4
    // States:
    // 0: "", non-final, ""
    // 1: "a", non-final, ""
    // 2: "an", non-final, ""
    // 3: "ant", non-final, "A"
    // 4: "ante", final, "B"
    //
    // After the "an" word is added. Note that the associated value of the state 2 is changed, but the state remains non-final,
    // because there are longer words that begin with the "an" prefix ("ant" and "ante").
    // a | 0->1, 5->6
    // n | 1->2, 6->7
    // t | 2->3
    // e | 3->4
    // c | 0->5
    // States:
    // 0: "", non-final, ""
    // 1: "a", non-final, ""
    // 2: "an", non-final, "C"
    // 3: "ant", non-final, "A"
    // 4: "ante", final, "B"
    //
    // After the "can" word is added.
    // a | 0->1, 5->6
    // n | 1->2, 6->7
    // t | 2->3
    // e | 3->4
    // c | 0->5
    // States:
    // 0: "", non-final, ""
    // 1: "a", non-final, ""
    // 2: "an", non-final, "C"
    // 3: "ant", non-final, "A"
    // 4: "ante", final, "B"
    // 5: "c", non-final, ""
    // 6: "ca", non-final, ""
    // 7: "can", final, "D"
    //
    internal class HtmlCharacterReferenceNames
    {
        /// <summary>
        /// Adds the name-characters pair to the data structure.
        /// </summary>
        /// <param name="name">The added name.</param>
        /// <param name="characters">The characters that correspond to the name.</param>
        /// <remarks>
        /// If this name has been already added, its characters will be replaced with the new ones.
        /// </remarks>
        internal void Add(string name, string characters)
        {
            Debug.Assert(StringUtil.HasChars(name));
            Debug.Assert(StringUtil.HasChars(characters));

            HtmlCharacterReferenceNamePrefix prefix = mInitialPrefix;

            for (int i = 0; i < name.Length; ++i)
            {
                // Find or create a prefix that starts with the current prefix and is one character longer.
                char c = name[i];
                Dictionary<HtmlCharacterReferenceNamePrefix, HtmlCharacterReferenceNamePrefix> prefixes =
                    mCharacters.GetValueOrNull(c);
                if (prefixes == null)
                {
                    prefixes = new Dictionary<HtmlCharacterReferenceNamePrefix, HtmlCharacterReferenceNamePrefix>();
                    mCharacters[c] = prefixes;
                }
                HtmlCharacterReferenceNamePrefix longerPrefix = prefixes.GetValueOrNull(prefix);
                if (longerPrefix == null)
                {
                    // There is no known longer prefix, let's create one.
                    // The newly created longer prefix is considered final until an even longer prefix is found.
                    longerPrefix = new HtmlCharacterReferenceNamePrefix();
                    prefixes[prefix] = longerPrefix;
                }

                // The current prefix is no longer final, because there is a longer prefix.
                prefix.IsFinal = false;

                prefix = longerPrefix;
            }
            Debug.Assert(prefix != mInitialPrefix);

            prefix.Characters = characters;
        }

        /// <summary>
        /// Returns the characters associated with the specified name.
        /// </summary>
        /// <param name="name">The name for which the associated characters will be returned.</param>
        /// <returns>
        /// The characters associated with the specified name, or an empty string, if the name is not found.
        /// </returns>
        internal string GetCharacters(string name)
        {
            return GetPrefix(name).Characters;
        }

        /// <summary>
        /// Determines whether the data structure contains a name which starts with the specified name and is longer.
        /// </summary>
        /// <param name="name">The prefix characters with which a longer name must start.</param>
        /// <returns>
        /// <c>true</c> if the data structure contains a longer name that starts with the specified name.
        /// <c>false</c> otherwise.
        /// </returns>
        internal bool LongerNameExists(string name)
        {
            return !GetPrefix(name).IsFinal;
        }

        /// <summary>
        /// Follows the specified name and returns the matched prefix (state).
        /// </summary>
        /// <returns>
        /// The prefix (state) that corresponds to the whole specified name.
        /// If the name is empty, the initial prefix is returned.
        /// If the name is not found, a special final empty prefix is returned.
        /// </returns>
        private HtmlCharacterReferenceNamePrefix GetPrefix(string name)
        {
            HtmlCharacterReferenceNamePrefix prefix = mInitialPrefix;

            if (name != null)
            {
                for (int i = 0; i < name.Length; ++i)
                {
                    char c = name[i];
                    Dictionary<HtmlCharacterReferenceNamePrefix, HtmlCharacterReferenceNamePrefix> prefixes =
                        mCharacters.GetValueOrNull(c);
                    if (prefixes == null)
                    {
                        return gUnknownPrefix;
                    }

                    HtmlCharacterReferenceNamePrefix longerPrefix = prefixes.GetValueOrNull(prefix);
                    if (longerPrefix == null)
                    {
                        return gUnknownPrefix;
                    }

                    prefix = longerPrefix;
                }
            }

            return prefix;
        }

        /// <summary>
        /// The first-level hashtable that maps the next observed character to state transition tables.
        /// </summary>
        private readonly Dictionary<char,
            Dictionary<HtmlCharacterReferenceNamePrefix, HtmlCharacterReferenceNamePrefix>> mCharacters =
                new Dictionary<char, Dictionary<HtmlCharacterReferenceNamePrefix, HtmlCharacterReferenceNamePrefix>>();

        /// <summary>
        /// The initial empty prefix (the initial state).
        /// </summary>
        /// <remarks>
        /// The initial prefix is considered final until any name is added to the data structure.
        /// </remarks>
        private readonly HtmlCharacterReferenceNamePrefix mInitialPrefix = new HtmlCharacterReferenceNamePrefix();

        /// <summary>
        /// The special prefix returned by the <see cref="GetPrefix"/> method when no prefix is found.
        /// </summary>
        /// <remarks>
        /// This special prefix is never modified and is always final.
        /// </remarks>
        private static readonly HtmlCharacterReferenceNamePrefix gUnknownPrefix = new HtmlCharacterReferenceNamePrefix();
    }
}
