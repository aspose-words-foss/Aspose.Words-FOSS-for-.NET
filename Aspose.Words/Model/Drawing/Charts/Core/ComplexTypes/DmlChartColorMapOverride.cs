// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/13/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// 5.7.2.30 clrMapOvr (Color Map Override)
    /// This element represents color mapping information. It is used to override the applications color mapping if the
    /// user has selected keep source formatting after a copy-paste.
    /// </summary>
    internal class DmlChartColorMapOverride
    {
        internal DmlChartColorMapOverride Clone()
        {
            return (DmlChartColorMapOverride)MemberwiseClone();
        }

        internal string Bg1;
        internal string Tx1;
        internal string Bg2;
        internal string Tx2;
        internal string Accent1;
        internal string Accent2;
        internal string Accent3;
        internal string Accent4;
        internal string Accent5;
        internal string Accent6;
        internal string Hlink;
        internal string FolHlink;
    }
}
