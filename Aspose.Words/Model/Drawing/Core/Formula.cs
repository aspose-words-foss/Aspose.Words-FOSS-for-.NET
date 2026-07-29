// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/08/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// This record specifies a formula used to calculate a value for use in a shape's pGuides_complex property. 
    /// Formulas are used to calculate values involved in the geometry of this shape so that a user may adjust 
    /// some of those values and the entire shape's geometry can adjust appropriately as a result.
    /// 
    /// 2.2.58 SG in the DOC SPEC.
    /// </summary>
    internal class Formula
    {
        /// <summary>
        /// Operation of the formula.
        /// </summary>
        internal Operation Operation;

        /// <summary>
        /// Low-level property. Use <see cref="IsParam1Calculated"/> etc instead.
        /// 
        /// 0x20 - The V argument is either a reference to another formula or an adjustment value.
        /// 0x40 - The P1 argument is either a reference to another formula or an adjustment value.
        /// 0x80 - The P2 argument is either a reference to another formula or an adjustment value.
        /// 
        /// When the argument is a reference to an adjustment value, the flag will be set
        /// and the value of the argument will be equal to the id of the adjustment attribute
        /// such as ShapeAttr.GeometryAdjustment0 etc.
        /// 
        /// When the argument is a reference to a formula, the flag will be set and the
        /// argument will be a zero-based formula index with 0x0400 bit set.
        /// For example, 0x0406 for @6. 
        /// </summary>
        internal int Flags;
       
        internal bool IsParam1Calculated
        {
            get { return (Flags & 0x20) != 0; }
        }

        internal bool IsParam2Calculated
        {
            get { return (Flags & 0x40) != 0; }
        }

        internal bool IsParam3Calculated
        {
            get { return (Flags & 0x80) != 0; }
        }

        /// <summary>
        /// Also known as V in VML.
        /// A constant value or a reference. If <see cref="IsParam1Calculated"/> is true, 
        /// then this value is one of <see cref="CalculatedParam"/> values.
        /// </summary>
        internal int Param1;

        /// <summary>
        /// Also known as P1 in VML.
        /// A constant value or a reference. If <see cref="IsParam2Calculated"/> is true, 
        /// then this value is one of <see cref="CalculatedParam"/> values.
        /// </summary>
        internal int Param2;

        /// <summary>
        /// Also known as P2 in VML.
        /// A constant value or a reference. If <see cref="IsParam3Calculated"/> is true, 
        /// then this value is one of <see cref="CalculatedParam"/> values.
        /// </summary>
        internal int Param3;
    }
}
