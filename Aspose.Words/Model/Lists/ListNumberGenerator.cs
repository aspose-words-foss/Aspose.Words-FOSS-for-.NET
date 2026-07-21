// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/11/2005 by Roman Korchagin

using Aspose.Collections;

namespace Aspose.Words.Lists
{
    /// <summary>
    /// <para>
    /// Represents a repository for <see cref="ListNumberState"/> instances used to generate sequential list numbers
    /// for different document lists.
    /// </para>
    /// <para>
    /// <see cref="ListNumberGenerator"/> must not be used on its own, so use <see cref="ListLabelUpdater"/>
    /// to update list labels for paragraphs in a document.
    /// </para>
    /// </summary>
    internal class ListNumberGenerator
    {
        /// <summary>
        /// <para>
        /// Gets the current list number state for the given list.
        /// </para>
        /// <para>
        /// Creates and initializes the numbers of a list if paragraphs with this list id were not visited before.
        /// (Use <see cref="ListLabelUpdater"/> to visit paragraphs in correct order).
        /// </para>
        /// </summary>
        internal ListNumberState GetCurrentListNumberState(List list)
        {
            ListNumberState state = mListNumberStates[list.ListDefId];
            if (state == null)
            {
                // A new list has started, create and initialize the numbers of the list.
                state = new ListNumberState(list);
                mListNumberStates[list.ListDefId] = state;
            }

            return state;
        }

        /// <summary>
        /// <para>
        /// Gets the next list number state for the given list and the given level.
        /// </para>
        /// <para>
        /// Creates and initializes the numbers of a list if paragraphs with this list id were not visited before.
        /// (Use <see cref="ListLabelUpdater"/> to visit paragraphs in correct order).
        /// </para>
        /// <para>
        /// Increments number on appropriate level if the numbering for this list is already under way.
        /// </para>
        /// </summary>
        internal ListNumberState GetNextListNumberState(List list, int listLevel)
        {
            ListNumberState state = GetCurrentListNumberState(list);

            // The numbering for this list is already under way, just select the next item.
            state.NextItem(list, listLevel);
            return state;
        }

        /// <summary>
        /// Restarts numbering from a specific list level.
        /// </summary>
        /// <remarks>
        /// AM. This is support for ListDef.RestartAtEachSection poorly documented flag.
        /// </remarks>
        internal void Restart(int listId, int level)
        {
            foreach (ListNumberState state in mListNumberStates.Values)
                if(state.ListId == listId)
                    if(state.ListDef.IsRestartAtEachSection)
                        state.Init(level);
        }

        /// <summary>
        /// Key is a list definition id. Value is <see cref="ListNumberState"/>.
        /// </summary>
        private readonly IntToObjDictionary<ListNumberState> mListNumberStates = new IntToObjDictionary<ListNumberState>();
    }
}
