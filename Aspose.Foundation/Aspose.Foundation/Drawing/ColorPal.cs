// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/03/2010 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Drawing
{
    /// <summary>
    /// Port this class manually to Java.
    /// </summary>
    [JavaManual("Manual porting by design.")]
    internal static class ColorPal
    {
        internal static System.Drawing.Color ToNativeColor(DrColor drColor)
        {
            if (drColor.IsEmpty)
                return System.Drawing.Color.Empty;

            // check for system color "window"            
            if (drColor == DrColor.Window)
                return System.Drawing.SystemColors.Window;

            return System.Drawing.Color.FromArgb(drColor.ToArgb());
        }

        internal static DrColor FromNativeColor(System.Drawing.Color nativeColor)
        {
            return new DrColor(nativeColor.ToArgb());
        }

        internal static DrColor GetSystemColor(string colorName)
        {
            System.Drawing.Color color = System.Drawing.Color.Empty;

            switch (colorName.ToLowerInvariant())
            {
                case "window":
                    color = System.Drawing.SystemColors.Window;
                    break;
                case "windowtext":
                    color = System.Drawing.SystemColors.WindowText;
                    break;
                default:
                    // Ignore all other.
                    break;
            }

            return new DrColor(color.ToArgb());
        }
    }
}
