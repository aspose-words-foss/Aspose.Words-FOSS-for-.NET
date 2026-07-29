// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.4.10 styleLbl (Style Label)
    /// </summary>
    internal class DmlColorTransformationStyleLabel : DmlExtensionListSource
    {
        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        internal DmlDiagramColors FillColorList
        {
            get { return mFillColorList; }
            set { mFillColorList = value; }
        }

        internal DmlDiagramColors LineColorList
        {
            get { return mLineColorList; }
            set { mLineColorList = value; }
        }

        internal DmlDiagramColors EffectColorList
        {
            get { return mEffectColorList; }
            set { mEffectColorList = value; }
        }

        internal DmlDiagramColors TextLineColorList
        {
            get { return mTextLineColorList; }
            set { mTextLineColorList = value; }
        }

        internal DmlDiagramColors TextFillColorList
        {
            get { return mTextFillColorList; }
            set { mTextFillColorList = value; }
        }

        internal DmlDiagramColors TextEffectColorList
        {
            get { return mTextEffectColorList; }
            set { mTextEffectColorList = value; }
        }

        private string mName;
        private DmlDiagramColors mFillColorList;
        private DmlDiagramColors mLineColorList;
        private DmlDiagramColors mEffectColorList;
        private DmlDiagramColors mTextLineColorList;
        private DmlDiagramColors mTextFillColorList;
        private DmlDiagramColors mTextEffectColorList;
    }
}
