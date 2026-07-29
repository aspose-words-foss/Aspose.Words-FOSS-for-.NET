// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.20 param (Parameter)
    /// </summary>
    internal class DmlParameter
    {
        internal string Type
        {
            get { return mType; }
            set { mType = value; }
        }

        internal string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        private string mType;
        private string mValue;
    }
}
