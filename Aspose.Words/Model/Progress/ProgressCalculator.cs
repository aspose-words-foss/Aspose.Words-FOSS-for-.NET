// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/09/2021 by Dmitry Sokolov

using System;
using System.Collections.Generic;

namespace Aspose.Words.Progress
{
    /// <summary>
    /// Base class for an overall progress calculator.
    /// </summary>
    internal class ProgressCalculator
    {
        /// <summary>
        /// Ctr.
        /// </summary>
        protected ProgressCalculator()
        {
            StagePercentageMap = new Dictionary<int, double>();
        }

        /// <summary>
        /// Returns approximate overall progress.
        /// </summary>
        /// <param name="currentStage">Current stage data.</param>
        /// <returns>Approximate overall progress.</returns>
        internal double GetEstimatedProgress(StageData currentStage)
        {
            double totalProgress = 0;

            for(int i = 0; i < StagesCount; i++)
            {
                int nextStageType = AvailableStages[i];
                double nextStageWeight = StagePercentageMap[nextStageType];

                if (nextStageType.Equals(currentStage.StageType))
                {
                    // Map progress to current stage weight.
                    totalProgress += currentStage.EstimatedProgress * nextStageWeight / 100.0d;
                    break;
                }

                // Next stage already completed.
                totalProgress += nextStageWeight;
            }

            return totalProgress;
        }

        /// <summary>
        /// Returns index in sequence of the specified stages.
        /// </summary>
        /// <param name="currentStageType"></param>
        /// <returns>Stage index. "-1" when specified stage was not found.</returns>
        internal int GetStageIndex(int currentStageType)
        {
            for (int i = 0; i < AvailableStages.Length; ++i)
            {
                if (currentStageType.Equals(AvailableStages[i]))
                    return i;
            }

            // Not found.
            return -1;
        }

        /// <summary>
        /// Returns previous stage type.
        /// </summary>
        /// <param name="currentStageIndex">Index of stage in the sequence.</param>
        /// <returns>Stage type. Throws exception when specified stage is first in the sequence.</returns>
        internal int GetPrevStageType(int currentStageIndex)
        {
            if (currentStageIndex <= 0)
                throw new ArgumentException("Progress stage has not previous stages.");

            if (currentStageIndex > AvailableStages.Length - 1)
                throw new ArgumentException("There is not a progress stage with specified index.");

            return AvailableStages[currentStageIndex - 1];
        }

        /// <summary>
        /// Overall progress precision.
        /// </summary>
        internal const int Precision = 2;

        /// <summary>
        /// Available stages in progress.
        /// </summary>
        protected int[] AvailableStages;

        /// <summary>
        /// Map of stage type to weight in overall progress.
        /// </summary>
        protected readonly IDictionary<int, double> StagePercentageMap;

        /// <summary>
        /// Specifies overall stages count.
        /// </summary>
        private int StagesCount { get { return AvailableStages.Length; } }
    }
}
