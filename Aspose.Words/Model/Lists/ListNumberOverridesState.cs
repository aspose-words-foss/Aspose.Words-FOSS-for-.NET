// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/05/2021 by Dmitry Sokolov

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Implements calculation logic for current numbers which related with processing overridden formatting on list level.
    /// </summary>
    /// <remarks>
    /// Word calculates current numbers according to "StartAt" value in the overridden formatting. Although, previous
    /// numbers were calculated according to values from a list definition. This class attempts to mimic this logic.
    /// </remarks>
    internal class ListNumberOverridesState
    {
        internal ListNumberOverridesState(int[] numbers, List list)
        {
            mNumbers = numbers;
            mMinLevelWithRecalcStartAt = list.ListLevels.Count;
        }

        /// <summary>
        /// Updates numbers according to overridden formatting.
        /// </summary>
        internal void UpdateNumbers(List list, int prevLevel, int newLevel)
        {
            Debug.Assert(list.Overrides.Count > 0);

            if (mInitialLevel < 0)
                mInitialLevel = prevLevel;

            int startLevel = newLevel < prevLevel ? newLevel : prevLevel + 1;
            if ((mMinLevelWithRecalcStartAt > startLevel) && (startLevel != mInitialLevel))
            {
                for (int i = startLevel; i < mMinLevelWithRecalcStartAt; ++i)
                {
                    ListLevelOverride formattingOverride = GetLevelOverride(i, list);
                    if (formattingOverride == null)
                        continue;

                    int usedLines = mNumbers[i] - list.GetStartAtOverrideAware(i);
                    mNumbers[i] = formattingOverride.ListLevel.StartAt + usedLines;
                }

                mMinLevelWithRecalcStartAt = startLevel;
            }
        }

        /// <summary>
        /// Gets level override when it applicable.
        /// </summary>
        internal static ListLevelOverride GetLevelOverride(int level, List list)
        {
            // DS: Skip for a while cases when both "StartOverride" and "StartAt" is specified  in overrides.
            return !list.IsStartAtOverridden(level)
                ? list.GetFormattingOverride(level)
                : null;
        }

        /// <summary>
        /// Current numbers on list levels.
        /// </summary>
        private readonly int[] mNumbers;

        /// <summary>
        /// Holds list level number which was used before the first calculation of numbers based on new "StartAt" value.
        /// The Word has special logic when a new list level equal to initial list level.
        /// </summary>
        private int mInitialLevel = -1;

        /// <summary>
        /// Holds number of the level which already was switched to new "StartAt" value.
        /// This property is required to avoid extra calculations of current numbers based on new "StartAt".
        /// Also it allows to skip logic which calculates previous "StartAt" value (it is necessary to subtract this value).
        /// </summary>
        private int mMinLevelWithRecalcStartAt;
    }
}
