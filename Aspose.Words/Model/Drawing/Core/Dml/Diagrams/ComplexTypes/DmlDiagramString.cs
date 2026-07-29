// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/01/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// Represents CT_CTName Dml complex type.
    /// </summary>
    internal class DmlDiagramString
    {
        internal string Language
        {
            get { return mLang; }
            set { mLang = value; }
        }

        internal string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }
        private string mLang;
        private string mValue;
    }
}
