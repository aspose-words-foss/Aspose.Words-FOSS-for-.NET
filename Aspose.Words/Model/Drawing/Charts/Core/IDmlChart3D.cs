// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/07/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core
{
    internal interface IDmlChart3D : IDmlChart2D
    {
        int AxIdZ { get; }
        ChartAxis AxZ { get; }
    }
}
