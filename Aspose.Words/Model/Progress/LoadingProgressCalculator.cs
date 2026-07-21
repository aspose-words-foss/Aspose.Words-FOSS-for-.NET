// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/09/2021 by Dmitry Sokolov

using System;
using Aspose.Words.Progress;

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Loading overall progress calculator.
    /// </summary>
    internal class LoadingProgressCalculator : ProgressCalculator
    {
        /// <summary>
        /// Ctr.
        /// </summary>
        internal LoadingProgressCalculator() : base()
        {
            #if CPLUSPLUS // C++ doesn't support casting from untyped Array to int[]
            LoadingStageType[] stages = (LoadingStageType[])Enum.GetValues(typeof(LoadingStageType));
            AvailableStages = new int[stages.Length];
            for (int i = 0; i < stages.Length; i++)
            {
                AvailableStages[i] = (int)stages[i];
            }
            #else
            AvailableStages = (int[])Enum.GetValues(typeof(LoadingStageType));
            #endif

            // Sum of values must be 100%.
            StagePercentageMap.Add((int)LoadingStageType.Reading, 70.0d);
            StagePercentageMap.Add((int)LoadingStageType.Processing, 30.0d);
        }
    }
}
