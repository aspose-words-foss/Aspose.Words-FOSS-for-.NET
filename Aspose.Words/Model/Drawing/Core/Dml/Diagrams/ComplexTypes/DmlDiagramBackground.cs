// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2013 by Konstantin Kornilov

using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.3.1 bg (Background Formatting)
    /// </summary>
    internal class DmlDiagramBackground
    {
        internal DmlFill Fill
        {
            get
            {
                // If fill isn't specified then use new DmlStyleFill.
                if (mFill == null)
                    mFill = new DmlStyleFill();

                return mFill;
            }
            set { mFill = value; }
        }

        internal DmlShapeEffectsCollection Effects
        {
            get { return mEffects; }
            set { mEffects = value; }
        }

        private DmlFill mFill;
        private DmlShapeEffectsCollection mEffects;
    }
}
