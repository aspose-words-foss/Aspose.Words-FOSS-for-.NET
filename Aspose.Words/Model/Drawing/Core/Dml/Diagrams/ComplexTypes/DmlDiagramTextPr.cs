// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/01/2014 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.Text;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.5.12 txPr (Text Properties)
    /// This element defines special text formatting that can be applied to text through a style label.
    /// </summary>
    internal class DmlDiagramTextPr
    {
        internal DmlFlatText FlatText
        {
            get { return mFlatText; }
            set { mFlatText = value; }
        }

        internal DmlShape3DProperties Shape3DProperties
        {
            get { return mShape3DProperties; }
            set { mShape3DProperties = value; }
        }

        private DmlFlatText mFlatText;
        private DmlShape3DProperties mShape3DProperties;
    }
}
