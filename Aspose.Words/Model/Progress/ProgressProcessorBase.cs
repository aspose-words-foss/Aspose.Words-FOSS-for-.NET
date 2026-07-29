// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/09/2021 by Dmitry Sokolov

using System.Collections.Generic;
using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Words.Progress
{
    /// <summary>
    /// Implements base ability to notify a client from the AW modules about a progress.
    /// </summary>
    /// <typeparam name="CallbackType">A type which implements custom method called during a document lifetime process.</typeparam>
    /// <typeparam name="ArgsType">Type of an arguments passing into callback method.</typeparam>
    internal abstract class ProgressProcessorBase<CallbackType, ArgsType>
        where CallbackType : class
        where ArgsType : class
    {
        /// <summary>
        /// Hide default ctr.
        /// </summary>
        private ProgressProcessorBase() { }

        /// <summary>
        /// Ctr.
        /// </summary>
        protected ProgressProcessorBase(DocumentBase docBase, CallbackType callback, int stageType)
        {
            Debug.Assert(docBase != null);

            Document doc = docBase as Document;
            if (doc == null)
                return;

            Doc = doc;
            mStageType = stageType;
            Callback = callback;
        }

        protected StageData GetActualStageData(
            int processedNodesCount,
            int totalNodesCount,
            ProgressCalculator progressCalculator)
        {
            int currentStageIndex = progressCalculator.GetStageIndex(mStageType);

            double prevProgress = GetPrevProgress(mStageType, currentStageIndex, progressCalculator);
            double currentStageProgress = GetStageProgress(processedNodesCount, totalNodesCount, prevProgress);

            StageData stageData = GetActualStageDataCore(currentStageProgress, progressCalculator);
            return stageData;
        }

        protected StageData GetActualStageData(
            Stream stream,
            ProgressCalculator progressCalculator,
            bool complete)
        {
            double currentStageProgress = complete ? CompletedProgressPercentages : GetStageProgress(stream);
            StageData stageData = GetActualStageDataCore(currentStageProgress, progressCalculator);

            return stageData;
        }

        [JavaThrows(true)]
        protected abstract void ExecuteCallback(ArgsType args);

        /// <summary>
        /// Determines that a client may be notified about the progress in current context.
        /// </summary>
        protected virtual bool IsProgressSupported()
        {
            // Filter glossary documents.
            if (Doc == null)
                return false;

            return true;
        }

        private StageData GetActualStageDataCore(double stageProgress,
            ProgressCalculator progressCalculator)
        {
            // 1. Create stage data.
            int stageIndex = progressCalculator.GetStageIndex(mStageType);
            Debug.Assert(stageIndex >= 0);

            StageData stageData = new StageData(mStageType, stageProgress);

            // 2. Ignore notification when rounded overall progress is not changed.
            // It is necessary to avoid insignificant progress notifications.
            double overallProgress = progressCalculator.GetEstimatedProgress(stageData);
            double prevOverallProgress = GetPrevProgress(OverallKey, stageIndex, progressCalculator);

            const int precision = ProgressCalculator.Precision;
            double roundedProgress = DoublePal.Trim(overallProgress, precision);
            double prevRoundedProgress = DoublePal.Trim(prevOverallProgress, precision);

            if (prevRoundedProgress >= roundedProgress)
                return null;

            // 3. Update progress map (for current stage and overall value).
            AddOrUpdateProgressToMap(mStageType, stageProgress);
            AddOrUpdateProgressToMap(OverallKey, overallProgress);

            return stageData;
        }

        private void AddOrUpdateProgressToMap(int stageType, double progress)
        {
            if (mProgressByStages.ContainsKey(stageType))
                mProgressByStages[stageType] = progress;
            else
                mProgressByStages.Add(stageType, progress);
        }

        /// <summary>
        /// Returns a stage progress based on the stream position.
        /// </summary>
        private static double GetStageProgress(Stream stream)
        {
            Debug.Assert(stream.CanSeek);

            if (stream.Length == 0)
                return CompletedProgressPercentages;

            double percentages = (double)stream.Position / stream.Length * 100.0d;
            return percentages;
        }

        /// <summary>
        /// Returns a stage progress based on the processed nodes count.
        /// </summary>
        private static double GetStageProgress(int processedNodesCount, int totalNodesCount, double prevProgress)
        {
            Debug.Assert(processedNodesCount >= 0);

            // Do not allow exceed 100% for a progress.
            if (processedNodesCount >= totalNodesCount)
                return CompletedProgressPercentages;

            double percentages = (double)processedNodesCount / totalNodesCount * 100.0d;

            // Do not rollback a progress.
            percentages = System.Math.Max(percentages, prevProgress);
            Debug.Assert(percentages <= CompletedProgressPercentages);

            return percentages;
        }

        private double GetPrevProgress(
            int stageType,
            int stageIndex,
            ProgressCalculator progressCalculator)
        {
            double prevProgress = 0.0d;
            if (mProgressByStages.ContainsKey(stageType))
            {
                prevProgress = mProgressByStages[stageType];
            }
            else if (stageType.Equals(OverallKey) && (stageIndex > 0))
            {
                // Looks like the processor did not update data about overall progress yet.
                // I.e. it is first notification on the current stage.
                int prevStageType = progressCalculator.GetPrevStageType(stageIndex);
                prevProgress = progressCalculator.GetEstimatedProgress(
                    new StageData(prevStageType, CompletedProgressPercentages));
            }

            return prevProgress;
        }

        protected bool HasCallback { get { return Callback != null; } }

        protected readonly Document Doc;
        protected readonly CallbackType Callback;
        private readonly int mStageType;

        /// <summary>
        /// Stores "Stage type -> Progress" map.
        /// </summary>
        private readonly Dictionary<int, double> mProgressByStages = new Dictionary<int, double>();

        /// <summary>
        /// Overall key for "stage type -> progress" mapping. Actually it is the stub and does not exist in the enumeration.
        /// </summary>
        private const int OverallKey = 0;

        private const double CompletedProgressPercentages = 100.0d;
    }
}
