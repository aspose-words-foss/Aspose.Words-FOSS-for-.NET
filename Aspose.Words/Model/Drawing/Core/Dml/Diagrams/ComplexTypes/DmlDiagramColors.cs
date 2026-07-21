// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// CT_Colors
    /// </summary>
    internal class DmlDiagramColors
    {
        internal DmlHueDirection HueDirection
        {
            get { return mHueDirection; }
            set { mHueDirection = value; }
        }

        internal DmlColorApplicationMethod ColorApplicationMethod
        {
            get { return mColorApplicationMethod; }
            set { mColorApplicationMethod = value; }
        }

        internal DmlColor[] Colors
        {
            get { return mColors; }
            set { mColors = value; }
        }

        internal bool IsEmpty
        {
            get { return Colors == null || Colors.Length == 0; }
        }

        private DmlHueDirection mHueDirection = DmlHueDirection.Clockwise;
        private DmlColorApplicationMethod mColorApplicationMethod = DmlColorApplicationMethod.Span;
        private DmlColor[] mColors;
    }
}
