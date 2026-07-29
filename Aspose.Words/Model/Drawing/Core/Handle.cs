// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/08/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// A shape handle. Normally occurs only for new shapes imported from clipart.
    /// 
    /// 2.3.6.25 pAdjustHandles_complex in the DOC SPEC
    /// 
    /// RK This was a struct, but I changed it into a class. This should be a class because it is too big for a struct.
    /// Also, I would have to make it immutable if it was a struct, but that would be quite ugly because the object is too big.
    /// </summary>
    internal class Handle
    {
        //Flags bitmasks:
        //    reserved0x8000 = 0x8000
        //    reserved0x4000 = 0x4000
        //    hasRadiusRange = 0x2000
        //    isPolarOrMapYReferenced = 0x1000
        //    isPolarOrMapXReferenced = 0x0800
        //    isYRangeMaxReferenced = 0x0400
        //    isYRangeMinReferenced = 0x0200
        //    isXRangeOrRadiusRangeMaxReferenced = 0x0100
        //    isXRangeOrRadiusRangeMinReferenced = 0x0080
        //    reserved0x0040 = 0x0040
        //    hasXYRange = 0x0020
        //    hasMap = 0x0010
        //    hasPolar = 0x08
        //    hasSwitch = 0x04
        //    hasInvY = 0x02
        //    hasInvX = 0x01
        /// <summary>
        /// Helper method. Returns handle flags as int. Useful for DOC and RTF exporters.
        /// </summary>
        internal int GetFlags()
        {
            int flags = 0;

            flags |= HasRadiusRange ? 0x2000 : 0;
            flags |= PolarY.IsFormula ? 0x1000 : 0;
            flags |= PolarX.IsFormula ? 0x0800 : 0;
            flags |= YMax.IsFormula ? 0x0400 : 0;
            flags |= YMin.IsFormula ? 0x0200 : 0;
            flags |= XMax.IsFormula ? 0x0100 : 0;
            flags |= XMin.IsFormula ? 0x0080 : 0;
            flags |= HasXYRange ? 0x0020 : 0;
            flags |= HasMap ? 0x0010 : 0;
            flags |= HasPolar ? 0x08 : 0;
            flags |= HasSwitch ? 0x04 : 0;
            flags |= HasInvY ? 0x02 : 0;
            flags |= HasInvX ? 0x01 : 0;

            return flags;
        }


        // Order of ints in handle bytes:
        //    flags
        //    posx
        //    posy
        //    polarx | mapx
        //    polary | mapy
        //    xrangemin | radiusrangemin
        //    xrangemax | radiusrangemax
        //    yrangemin
        //    yrangemax

        // Reference values are deciphered as follows:
        //    0 - topLeft
        //    1 - bottomRight
        //    2 - center
        //    3 - @0
        //    4 - @1
        //    ...
        //    0x0100 - #0
        //    0x0101 - #1
        //    ...
        
        //    posx, posy values are always references. Other value are references if correspondent bits in flags are 1.

        /// <summary>
        /// True if radius range is defined for this handle.
        /// </summary>
        internal bool HasRadiusRange;

        /// <summary>
        /// True if x, y ranges are defined for this handle.
        /// Range value 0x80000000 - 0x7FFFFFFF means that the range is not defined and should be ignored.
        /// </summary>
        internal bool HasXYRange;

        /// <summary>
        /// True if map is defined for this handle.
        /// In this case the x, y positions of the adjust handle are mapped from the coordsize range into the given range.
        /// </summary>
        internal bool HasMap;

        /// <summary>
        /// True if this handle has polar adjustment.
        /// </summary>
        internal bool HasPolar;

        /// <summary>
        /// If true, the adjust handle is switched between the x and y direction depending on the aspect ratio of the shape.
        /// The x and y positions of the adjust handle are swapped when the shape is taller than it is wide.
        /// This is useful for shapes with limo behavior.
        /// </summary>
        internal bool HasSwitch;

        /// <summary>
        /// If true, the x position of the adjust handle is inverted by setting it to coordoriginx + coordsizex - x.
        /// </summary>
        internal bool HasInvX;

        /// <summary>
        /// If true, the y position of the adjust handle is inverted by setting it to coordoriginy + coordsizey - y.
        /// </summary>
        internal bool HasInvY;

        /// <summary>
        /// The x position of the adjust handle.
        /// Each value can be either a constant, a formula value (e.g., @2), center, topLeft, bottomRight, or an adjust value (e.g. #3).
        /// If a constant, formula value, center, topleft, or bottomright is specified, the handle position is fixed in that dimension.
        /// If an adjust value (e.g. #3) is specified, the handle is free to move that dimension and the adjust value is determined by the position of the handle.
        /// </summary>
        internal HandlePositionValue PositionX;

        /// <summary>
        /// The y position of the adjust handle.
        /// Each value can be either a constant, a formula value (e.g., @2), center, topLeft, bottomRight, or an adjust value (e.g. #3).
        /// If a constant, formula value, center, topleft, or bottomright is specified, the handle position is fixed in that dimension.
        /// If an adjust value (e.g. #3) is specified, the handle is free to move that dimension and the adjust value is determined by the position of the handle.
        /// </summary>
        internal HandlePositionValue PositionY;

        /// <summary>
        /// Stores polar center X coordinate if adjust handle has polar adjustment.
        /// Stores map range minimum value if adjust handle has map defined.
        /// </summary>
        internal PathValue PolarX = new PathValue();

        /// <summary>
        /// Stores polar center Y coordinate if adjust handle has polar adjustment.
        /// Stores map range maximum value if adjust handle has map defined.
        /// </summary>
        internal PathValue PolarY = new PathValue();

        /// <summary>
        /// Stores minimum value for x range if case of orthogonal adjustment, or for radius range in case of polar adjustment.
        /// </summary>
        internal PathValue XMin = new PathValue();
        
        /// <summary>
        /// Stores maximum value for X range in case of orthogonal adjustment, or for radius range in case of polar adjustment.
        /// </summary>
        internal PathValue XMax = new PathValue();

        /// <summary>
        /// Stores minimum value for Y range in case of orthogonal adjustment.
        /// </summary>
        internal PathValue YMin = new PathValue();

        /// <summary>
        /// Stores maximum value for Y range in case of orthogonal adjustment.
        /// </summary>
        internal PathValue YMax = new PathValue();
    }
}
