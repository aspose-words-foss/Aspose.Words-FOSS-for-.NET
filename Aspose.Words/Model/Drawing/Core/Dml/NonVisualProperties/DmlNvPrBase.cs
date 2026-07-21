// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.NonVisualProperties
{
    /// <summary>
    /// Base class for:
    /// 20.1.2.2.25 nvCxnSpPr (Non-Visual Properties for a Connection Shape)
    /// 20.1.2.2.26 nvGraphicFramePr (Non-Visual Properties for a Graphic Frame)
    /// 20.1.2.2.27 nvGrpSpPr (Non-Visual Properties for a Group Shape)
    /// 20.1.2.2.28 nvPicPr (Non-Visual Properties for a Picture)
    /// 20.1.2.2.29 nvSpPr (Non-Visual Properties for a Shape)
    /// </summary>
    internal abstract class DmlNvPrBase
    {
        internal abstract DmlNvHolder Holder { get; }

        internal DmlNvPrBase Clone()
        {
            DmlNvPrBase lhs = (DmlNvPrBase)MemberwiseClone();
            if (mNvDrawingProperties != null)
                lhs.mNvDrawingProperties = mNvDrawingProperties.Clone();
            if (mCNvProperties != null)
                lhs.mCNvProperties = mCNvProperties.Clone();

            return lhs;
        }

        internal DmlCnvPrBase CNvProperties
        {
            get { return mCNvProperties; }
            set { mCNvProperties = value; }
        }

        internal DmlNvDrawingProperties NvDrawingProperties
        {
            get { return mNvDrawingProperties; }
            set { mNvDrawingProperties = value; }
        }

        private DmlNvDrawingProperties mNvDrawingProperties;
        private DmlCnvPrBase mCNvProperties;
    }
}
