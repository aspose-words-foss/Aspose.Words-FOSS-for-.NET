// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// AG_IteratorAttributes
    /// </summary>
    internal class DmlIteratorAttributes
    {
        internal DmlAxisType[] AxisType
        {
            get { return mAxisType; }
            set { mAxisType = value; }
        }

        internal DmlElementType[] PointType
        {
            get { return mPointType; }
            set { mPointType = value; }
        }

        internal bool[] HideLastTransition
        {
            get { return mHideLastTransition; }
            set { mHideLastTransition = value; }
        }

        internal int[] Start
        {
            get { return mStart; }
            set { mStart = value; }
        }

        internal int[] Count
        {
            get { return mCount; }
            set { mCount = value; }
        }

        internal int[] Step
        {
            get { return mStep; }
            set { mStep = value; }
        }

        private DmlAxisType[] mAxisType = new DmlAxisType[0];
        private DmlElementType[] mPointType = new DmlElementType[0];
        private bool[] mHideLastTransition = new bool[0];
        private int[] mStart = new int[0];
        private int[] mCount = new int[0];
        private int[] mStep = new int[0];
    }
}
