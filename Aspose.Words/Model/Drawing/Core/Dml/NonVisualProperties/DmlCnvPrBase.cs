// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.NonVisualProperties
{
    /// <summary>
    /// Base class for:
    /// 20.1.2.2.4 cNvCxnSpPr (Non-Visual Connector Shape Drawing Properties)
    /// 20.1.2.2.5 cNvGraphicFramePr (Non-Visual Graphic Frame Drawing Properties)
    /// 20.1.2.2.6 cNvGrpSpPr (Non-Visual Group Shape Drawing Properties)
    /// 20.1.2.2.7 cNvPicPr (Non-Visual Picture Drawing Properties)
    /// 20.1.2.2.9 cNvSpPr (Non-Visual Shape Drawing Properties)
    /// </summary>
    internal abstract class DmlCnvPrBase : DmlExtensionListSource
    {
        internal DmlCnvPrBase Clone()
        {
            DmlCnvPrBase lhs = (DmlCnvPrBase)MemberwiseClone();
            lhs.mLocks = mLocks.Clone();
            lhs.Extensions = CloneExtensions();
            return lhs;
        }

        internal abstract DmlNvHolder Holder { get; }

        internal DmlLocks Locks
        {
            get { return mLocks; }
        }

        private DmlLocks mLocks = new DmlLocks();        
    }
}
