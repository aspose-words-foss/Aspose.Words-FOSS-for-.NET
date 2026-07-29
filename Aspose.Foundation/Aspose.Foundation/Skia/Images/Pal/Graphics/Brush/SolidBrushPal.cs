// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/08/2017 by Alexey Noskov

#if NETSTANDARD
using Aspose.Drawing;
using SkiaSharp;

namespace Aspose.Images.Pal.Graphics.Brush
{
    public class SolidBrushPal : NativeBrushPal
    {
        public SolidBrushPal(DrColor color)
        {
            SKColor skColor = DrColorToSKColor(color);
            Color = skColor;
        }
    }
}
#endif
