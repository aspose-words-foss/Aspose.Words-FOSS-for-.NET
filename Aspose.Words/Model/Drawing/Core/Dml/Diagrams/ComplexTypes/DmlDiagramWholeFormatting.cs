// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2013 by Konstantin Kornilov

using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.3.9 whole (Whole E2O Formatting)
    /// </summary>
    internal class DmlDiagramWholeFormatting
    {
        internal DmlOutline Outline
        {

            get
            { 
                if (mOutline == null)
                    mOutline = new DmlOutline();
                return mOutline;
            }
            set { mOutline = value; }
        }

        internal DmlShapeEffectsCollection Effects
        {
            get { return mEffects; }
            set { mEffects = value; }
        }

        private DmlOutline mOutline;
        private DmlShapeEffectsCollection mEffects;
    }
}
