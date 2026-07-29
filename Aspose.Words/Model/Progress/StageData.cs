// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/09/2021 by Dmitry Sokolov

namespace Aspose.Words.Progress
{
    /// <summary>
    /// Represents state of a progress stage.
    /// </summary>
    /// <remarks>
    /// The process is split into stages. Approximate progress may be calculated for every stage.
    /// And these parts compound to overall progress estimate.
    /// </remarks>
    internal class StageData
    {
        /// <summary>
        /// Ctr.
        /// </summary>
        internal StageData(int stageType, double progress)
        {
            StageType = stageType;
            EstimatedProgress = progress;
        }

        /// <summary>
        /// Stage progress status.
        /// </summary>
        /// <remarks>
        /// Can take a value from 0 to 100.
        /// </remarks>>
        internal double EstimatedProgress { get; }

        /// <summary>
        /// Current stage type.
        /// </summary>
        internal int StageType { get; }

    }
}
