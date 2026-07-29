// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using System;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes
{
    /// <summary>
    /// 21.4.7.44 ST_NodeCount (Number of Nodes Definition)
    /// </summary>
    internal class DmlDiagramNodeCount
    {
        internal DmlDiagramNodeCount(int value)
        {
            if(value < -1)
                throw new ArgumentException("Unexpected value.");

            mValue = value;
        }

        internal int Value
        {
            get { return mValue; }
        }

        private readonly int mValue;
    }
}
