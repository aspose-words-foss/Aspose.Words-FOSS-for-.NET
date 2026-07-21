// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.NonVisualProperties
{
    internal class DmlConnection
    {
        internal uint Id
        {
            get { return mId; }
            set { mId = value; }
        }

        internal uint Index
        {
            get { return mIdx; }
            set { mIdx = value; }
        }

        private uint mId;
        private uint mIdx;
    }
}
