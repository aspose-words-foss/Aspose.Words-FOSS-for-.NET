// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/01/2012 by Konstantin Kornilov

using System.Collections.Generic;
using Aspose.Warnings;

namespace Aspose.Fonts
{
    /// <summary>
    /// Base class for font source.
    /// </summary>
    /// <remarks>
    /// This hierarchy is currently required only for unit tests. In real scenarios the FontSourceBase hierarchy from
    /// Aspose.Words assembly is used.
    /// </remarks>
    public abstract class FontSourceBaseCore : IFontSource
    {
        protected FontSourceBaseCore()
            : this(DefaultPriority)
        {
        }

        protected FontSourceBaseCore(int priority)
        {
            mPriority = priority;
        }

        /// <summary>
        /// Scans this font source and returns all available font data.
        /// </summary>
        public abstract IEnumerable<IFontData> GetFontDataInternal();

        /// <summary>
        /// Font source priority.
        /// </summary>
        public int PriorityInternal
        {
            get { return mPriority; }
        }

        /// <summary>
        /// Called during processing of font source when an issue is detected that might result in formatting fidelity loss.
        /// </summary>
        public IWarningCallbackCore WarningCallbackCore
        {
            get { return mWarningCallbackCore; }
            set { mWarningCallbackCore = value; }
        }

        internal static void Warn(IWarningCallbackCore warningCallback, string description)
        {
            if (warningCallback != null)
                warningCallback.Warn(WarningTypeCore.MajorFormattingLoss, WarningSourceCore.Font, description);
        }

        private IWarningCallbackCore mWarningCallbackCore;
        private readonly int mPriority;

        public const int DefaultPriority = 0;
    }
}
