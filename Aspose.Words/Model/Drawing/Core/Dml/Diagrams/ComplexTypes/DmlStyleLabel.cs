// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.Styles;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.5.10 styleLbl (Style Label)
    /// </summary>
    internal class DmlStyleLabel : DmlExtensionListSource
    {
        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        internal DmlScene3DProperties Scene3D
        {
            get { return mScene3D; }
            set { mScene3D = value; }
        }

        internal DmlShape3DProperties Shape3D
        {
            get { return mShape3D; }
            set { mShape3D = value; }
        }

        internal DmlDiagramTextPr TextProperties
        {
            get { return mTextProperties; }
            set { mTextProperties = value; }
        }

        internal DmlShapeStyle Style
        {
            get { return mStyle; }
            set { mStyle = value; }
        }

        private string mName;
        private DmlScene3DProperties mScene3D;
        private DmlShape3DProperties mShape3D;
        private DmlDiagramTextPr mTextProperties;
        private DmlShapeStyle mStyle;
    }
}
