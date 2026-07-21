// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.NonVisualProperties
{
    /// <summary>
    /// 20.1.2.2.4 cNvCxnSpPr (Non-Visual Connector Shape Drawing Properties)
    /// This element specifies the non-visual drawing properties for a connector shape.
    /// </summary>
    internal class DmlCnvPrConnectorShape : DmlCnvPrBase
    {
        internal override DmlNvHolder Holder
        {
            get { return DmlNvHolder.ConnectionShape; }
        }

        internal DmlConnection ConnectionEnd
        {
            get { return mConnectionEnd; }
            set { mConnectionEnd = value; }
        }

        internal DmlConnection ConnectionStart
        {
            get { return mConnectionStart; }
            set { mConnectionStart = value; }
        }

        private DmlConnection mConnectionEnd;
        private DmlConnection mConnectionStart;
    }
}
