// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.1 adj (Shape Adjust)
    /// </summary>
    internal class DmlShapeAdjust
    {
        internal int HandleIndex
        {
            get { return mHandleIndex; }
            set { mHandleIndex = value; }
        }

        internal double Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        private int mHandleIndex;
        private double mValue;
    }
}
