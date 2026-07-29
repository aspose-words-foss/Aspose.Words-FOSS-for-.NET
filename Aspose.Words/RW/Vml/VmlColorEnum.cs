// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/05/2007 by Vladimir Averkin

using Aspose.Collections;
using Aspose.Drawing;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Converts color enumerated types between enum and VML string.
    /// </summary>
    internal static class VmlColorEnum
    {
        /// <summary>
        /// Converts a named VML color into a <see cref="DrColor"/>
        /// If cannot recognize or the color is "none" returns <see cref="DrColor.Empty"/>
        /// </summary>
        internal static DrColor VmlToPalColor(string value)
        {
            return gColorMap.GetValue(value, DrColor.Empty);
        }

        internal static string PalColorToVml(DrColor value)
        {
            return gColorMap.GetKey(value, "");
        }

        //JAVA: declarations moved here to exclude java's illegal forward reference.
        private static readonly StringToObjBidirectionalMap<DrColor> gColorMap = new StringToObjBidirectionalMap<DrColor>();        

        static VmlColorEnum()
        {
            // Section 6.1.3.1 ST_ColorType (Color Type) of ECMA-376 documentation.
            gColorMap.AddEntry("none", DrColor.Transparent);
            gColorMap.AddEntry("maroon", DrColor.Maroon);
            gColorMap.AddEntry("red", DrColor.Red);
            gColorMap.AddEntry("yellow", DrColor.Yellow);
            gColorMap.AddEntry("olive", DrColor.Olive);
            gColorMap.AddEntry("purple", DrColor.Purple);
            gColorMap.AddEntry("fuchsia", DrColor.Fuchsia);
            gColorMap.AddEntry("white", DrColor.White);
            gColorMap.AddEntry("lime", DrColor.Lime);
            gColorMap.AddEntry("green", DrColor.Green);
            gColorMap.AddEntry("navy", DrColor.Navy);
            gColorMap.AddEntry("blue", DrColor.Blue);
            gColorMap.AddEntry("aqua", DrColor.Aqua);
            gColorMap.AddEntry("teal", DrColor.Teal);
            gColorMap.AddEntry("black", DrColor.Black);
            gColorMap.AddEntry("silver", DrColor.Silver);
            gColorMap.AddEntry("gray", DrColor.Gray);
        }
    }
}
