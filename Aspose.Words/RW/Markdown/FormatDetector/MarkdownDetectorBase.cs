// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2019 by Ilya Navrotskiy

using Aspose.JavaAttributes;

namespace Aspose.Words.RW.Markdown.FormatDetector
{
    /// <summary>
    /// The base class for all markdown feature detectors.
    /// </summary>
    internal abstract class MarkdownDetectorBase
    {
        /// <summary>
        /// Detects and updates a specified context with a detected markdown feature type.
        /// </summary>
        internal bool DetectAndUpdateContext(MarkdownDetectorContext context)
        {
            // Reset the number of detected markdown features to zero.
            mCount = 0;

            if (Detect(context))
            {
                context.CurLineDetectedFeatures.Add(Type);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Detects a markdown feature in a specified context.
        /// </summary>
        [JavaThrows(true)]
        protected abstract bool Detect(MarkdownDetectorContext context);

        /// <summary>
        /// Gets or sets a count of the detected markdown features.
        /// </summary>
        internal int Count
        {
            get { return mCount; }
            set { mCount = value; }
        }

        /// <summary>
        /// Gets a weight of the detected markdown features.
        /// </summary>
        internal int Weight
        {
            get { return (mCount * (int)Significance); }
        }

        /// <summary>
        /// Gets markdown feature significance of the detector.
        /// </summary>
        protected abstract MarkdownFeatureSignificance Significance { get; }
        
        /// <summary>
        /// Gets markdown feature type of the detector.
        /// </summary>
        protected abstract MarkdownFeatureType Type { get; }

        private int mCount;
    }
}
