// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/04/2006 by Roman Korchagin

using Aspose.Collections.Generic;

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Used by <see cref="ListNumberGenerator"/> to keep current numbers.
    /// </summary>
    internal class ListNumberState
    {
        /// <summary>
        /// Ctor to create a mutable instance.
        /// </summary>
        internal ListNumberState(List list)
        {
            ListDef = list.ListDef;
            mList = list;
            mUsedListIDs = new HashSetGeneric<int>();
            Init();
        }

        /// <summary>
        /// Ctor to create an immutable instance.
        /// </summary>
        internal ListNumberState(List list, int level, int[] numbers)
        {
            mList = list;
            CurrentLevel = level;
            mNumbers = numbers;
            mNumberDecrements = new int[mList.ListLevels.Count];
        }

        /// <summary>
        /// Snapshot ctor.
        /// </summary>
        private ListNumberState(ListNumberState source)
        {
            mList = source.mList;
            CurrentLevel = source.CurrentLevel;
            LocaleId = source.LocaleId;

            mNumbers = new int[source.mNumbers.Length];
            source.mNumbers.CopyTo(mNumbers, 0);

            mNumberDecrements = new int[source.mNumberDecrements.Length];
            source.mNumberDecrements.CopyTo(mNumberDecrements, 0);

            UsedLevels = source.UsedLevels;
            mNumberOverridesState = new ListNumberOverridesState(mNumbers, mList);
        }

        /// <summary>
        /// Snapshot is almost Clone but returns immutable object and skips cloning
        /// some members which don't make sense for immutable ListNumberState.
        /// </summary>
        internal ListNumberState Snapshot()
        {
            return new ListNumberState(this);
        }

        /// <summary>
        /// Inits numbering from a specific list level.
        /// </summary>
        internal void Init(int level)
        {
            int previousLevel = level - 1;
            CurrentLevel = previousLevel;
            UsedLevels = previousLevel;
            mNumberDecrements = new int[mList.ListLevels.Count];

            if (mNumbers == null)
                mNumbers = new int[mList.ListLevels.Count];
            else
                for (int i = level; i < mNumbers.Length; i++)
                    mNumbers[i] = 0;

            for (int i = level; i < mNumbers.Length; i++)
                mNumbers[i] = mList.GetStartAtOverrideAware(i) - 1;

            mNumberOverridesState = new ListNumberOverridesState(mNumbers, mList);
        }

        /// <summary>
        /// Selects the next item at the specified list level.
        /// Increments and resets numbers where appropriate.
        /// </summary>
        internal void NextItem(List list, int newLevel)
        {
            NextItem(list, newLevel, true);
        }

        /// <summary>
        /// Selects the next item at the specified list level.
        /// Increments and resets numbers where appropriate.
        /// </summary>
        internal void NextItem(List list, int newLevel, bool setListNumStartAtLevel)
        {
            Debug.Assert(list.ListDef == ListDef);

            // mUsedListIDs == null and mListDef == null for 'snapshot' ListNumberState, got by Snapshot method,
            // it's immutable object and NextItem operation is not available for it.
            Debug.Assert((mUsedListIDs != null) && (ListDef != null));

            mList = list;
            newLevel = mList.ListLevels.CorrectLevel(newLevel);

            // AS If currently processed list has different ListId from previous AND
            // it was not encountered before AND its Start At value is overridden,
            // it's considered as started. It's MS Word behavior.
            if (!mUsedListIDs.Contains(list.ListId))
            {
                for (int i = newLevel; i < mNumbers.Length; i++)
                {
                    if (list.IsStartAtOverridden(i))
                    {
                        mUsedListIDs.Add(list.ListId);
                        mNumbers[i] = list.GetStartAtOverrideAware(i) - 1;
                    }
                }
            }

            mNumbers[newLevel]++;

            // AS For the case when list level has increased by more than one,
            // we need to increment values of skipped levels.
            for (int i = CurrentLevel + 1; i < newLevel; i++)
            {
                int delta = GetNumberDeltaForMissingLabels(GetRestartAfterLevel(i), i);
                mNumbers[i] += delta;
            }

            // WORDSNET-22207 Switch current numbers to new starting value from overridden formatting.
            if ((UsedLevels != 0) && (newLevel != CurrentLevel) && (list.Overrides.Count > 0))
                mNumberOverridesState.UpdateNumbers(list, CurrentLevel, newLevel);

            if (newLevel < CurrentLevel)
            {
                for (int i = newLevel + 1; i < mNumbers.Length; i++)
                {
                    // WORDSNET-16207 It seems Word gets RestartAfterLevel value from ListLevel
                    // but not from override settings for a list level.
                    if (GetRestartAfterLevel(i) < newLevel)
                        continue;

                    ListLevelOverride formattingOverride = ListNumberOverridesState.GetLevelOverride(i, list);
                    mNumbers[i] = formattingOverride != null
                        ? formattingOverride.ListLevel.StartAt
                        : list.GetStartAtOverrideAware(i);

                    --mNumbers[i];
                }
            }

            CurrentLevel = newLevel;
            if (setListNumStartAtLevel)
                ListNumStartAtLevel = newLevel;

            UsedLevels = BitUtil.SetBit(UsedLevels, 1 << newLevel, true);
            UsedLevels = BitUtil.TruncateBits(UsedLevels, newLevel + 1);
        }

        private int GetNumberDeltaForMissingLabels(int restartAfterLevel, int currentLevel)
        {
            // WORDSNET-6402 MS Word applies sequential numeration when level has 'RestartAfter' property explicitly set 'off'
            // or no any levels of this list were used.
            if (UsedLevels == 0)
                return 1;

            if (restartAfterLevel < 0)
                return 0;

            for (int j = restartAfterLevel + 1; j < currentLevel; j++)
            {
                // WORDSNET-24444 Skip counter increment when there is a label with level between "lvlRestart" and current level.
                if (BitUtil.IsSetInt32(UsedLevels, 1 << j))
                    return 0;
            }
            return 1;
        }

        /// <summary>
        /// Gets the current number for the current level.
        /// </summary>
        internal int GetNumber()
        {
            return GetNumber(CurrentLevel);
        }

        /// <summary>
        /// Sets the current number for the current level.
        /// </summary>
        internal void SetNumber(int value)
        {
            SetNumber(CurrentLevel, value);
        }

        /// <summary>
        /// Increase decrement value for current level.
        /// Call this method when encounter paragraph insertions
        /// and final revisions is not shown.
        /// </summary>
        internal void IncreaseDecrement()
        {
            mNumberDecrements[mList.ListLevels.CorrectLevel(CurrentLevel)]++;
        }

        /// <summary>
        /// Gets the current number for the specified level.
        /// </summary>
        internal int GetNumber(int level)
        {
            int correctLevel = mList.ListLevels.CorrectLevel(level);
            return mNumbers[correctLevel] - mNumberDecrements[correctLevel];
        }

        /// <summary>
        /// Gets the level properties for the current level.
        /// </summary>
        internal ListLevel GetListLevel()
        {
            return GetListLevel(CurrentLevel);
        }

        /// <summary>
        /// Gets the level properties for the specified level.
        /// </summary>
        internal ListLevel GetListLevel(int level)
        {
            return mList.GetListLevelOverrideAware(level);
        }

        /// <summary>
        /// Inits numbering.
        /// </summary>
        private void Init()
        {
            CurrentLevel = -1;
            mNumbers = new int[mList.ListLevels.Count];
            mNumberDecrements = new int[mList.ListLevels.Count];
            UsedLevels = 0;
            mNumberOverridesState = new ListNumberOverridesState(mNumbers, mList);

            for (int i = 0; i < mNumbers.Length; i++)
                mNumbers[i] = mList.GetStartAtOverrideAware(i) - 1;
        }

        /// <summary>
        /// Get the RestartAfterLevel value for the specified level.
        /// </summary>
        /// <returns></returns>
        private int GetRestartAfterLevel(int level)
        {
            return mList.ListLevels.FetchListLevel(level).RestartAfterLevel;
        }

        /// <summary>
        /// Sets the current number for the specified level.
        /// </summary>
        private void SetNumber(int level, int value)
        {
            mNumbers[mList.ListLevels.CorrectLevel(level)] = value;
        }

        internal int ListId
        {
            get { return mList.ListId; }
        }

        /// <summary>
        /// Zero based current level in this list.
        /// </summary>
        internal int CurrentLevel { get; private set; }

        /// <summary>
        /// Zero based level in this list used to maintain LISTNUM field routines. Do not use it in other circumstances.
        /// </summary>
        internal int ListNumStartAtLevel { get; private set; }

        /// <summary>
        /// Gets or sets the locale identifier (language) of corresponded paragraph.
        /// </summary>
        /// <remarks>
        /// Locale identifier (language) is used to determine LocalizedOrdinalSuffix.
        /// </remarks>
        internal int LocaleId { get; set; }

        /// <summary>
        /// The list definition that is wrapped by this object.
        /// </summary>
        internal ListDef ListDef { get; }

        /// <summary>
        /// Usage bit map. If Nth bit set then Nth level was used in numbering.
        /// </summary>
        internal int UsedLevels { get; private set; }

        /// <summary>
        /// Last used list that referred to our list definition.
        /// </summary>
        private List mList;

        /// <summary>
        /// List IDs of all encountered lists of <see cref="ListDef"/> corresponding to this <see cref="ListNumberState"/>.
        /// </summary>
        private readonly HashSetGeneric<int> mUsedListIDs;

        /// <summary>
        /// int[9] or int[1] that keeps the current numbers for each level.
        /// </summary>
        private int[] mNumbers;
        private int[] mNumberDecrements;

        /// <summary>
        /// Holds calculating current numbers logic and data for the case when level formatting is overridden.
        /// </summary>
        private ListNumberOverridesState mNumberOverridesState;
    }
}
