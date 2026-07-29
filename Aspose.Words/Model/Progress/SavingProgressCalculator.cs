// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/12/2021 by Dmitry Sokolov

using System;
using Aspose.Words.Progress;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Saving overall progress calculator.
    /// </summary>
    internal class SavingProgressCalculator : ProgressCalculator
    {
        /// <summary>
        /// Ctr.
        /// </summary>
        internal SavingProgressCalculator() : base()
        {
            #if CPLUSPLUS // C++ doesn't support casting from untyped Array to int[]
            SavingStageType[] stages = (SavingStageType[])Enum.GetValues(typeof(SavingStageType));
            AvailableStages = new int[stages.Length];
            for (int i = 0; i < stages.Length; i++)
            {
                AvailableStages[i] = (int)stages[i];
            }
            #else
            AvailableStages = (int[])Enum.GetValues(typeof(SavingStageType));
            #endif

            // Sum of values must be 100%.
            StagePercentageMap.Add((int)SavingStageType.Processing, 30.0d);
            StagePercentageMap.Add((int)SavingStageType.Saving, 70.0d);
        }
    }
}
