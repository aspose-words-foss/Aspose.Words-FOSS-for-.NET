// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/08/2008 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Specifies a property or formula to calculate a value of a formula parameter, when it is a calculated value.
    /// 
    /// 2.2.58 SG in the DOC SPEC.
    /// </summary>
    [CppConstexpr]
    internal static class CalculatedParam
    {
        /// <summary>
        /// X coordinate of the center of the geometry space of this shape.
        /// </summary>
        internal const int GeometrySpaceCenterX = 0x0140;
        /// <summary>
        /// Y coordinate of the center of the geometry space of this shape.
        /// </summary>
        internal const int GeometrySpaceCenterY = 0x0141;
        /// <summary>
        /// Width of the geometry space of this shape.
        /// </summary>
        internal const int GeometrySpaceWidth = 0x0142;
        /// <summary>
        /// Height of the geometry space of this shape.
        /// </summary>
        internal const int GeometrySpaceHeight = 0x0143;

        /// <summary>
        /// The value of this shape‟s adjustValue property.
        /// </summary>
        internal const int Adjust1 = 0x0147;
        /// <summary>
        /// The value of this shape‟s adjust2Value property.
        /// </summary>
        internal const int Adjust2 = 0x0148; 
        /// <summary>
        /// The value of this shape‟s adjust3Value property.
        /// </summary>
        internal const int Adjust3 = 0x0149; 
        /// <summary>
        /// The value of this shape‟s adjust4Value property.
        /// </summary>
        internal const int Adjust4 = 0x014a; 
        /// <summary>
        /// The value of this shape‟s adjust5Value property.
        /// </summary>
        internal const int Adjust5 = 0x014b; 
        /// <summary>
        /// The value of this shape‟s adjust6Value property.
        /// </summary>
        internal const int Adjust6 = 0x014c; 
        /// <summary>
        /// The value of this shape‟s adjust7Value property.
        /// </summary>
        internal const int Adjust7 = 0x014d; 
        /// <summary>
        /// The value of this shape‟s adjust8Value property.
        /// </summary>
        internal const int Adjust8 = 0x014e; 

        /// <summary>
        /// The value of this shape‟s xLimo property.
        /// </summary>
        internal const int LimoX = 0x0153;
        /// <summary>
        /// The value of this shape‟s yLimo property.
        /// </summary>
        internal const int LimoY = 0x0154;
        
        /// <summary>
        /// The value of the fLine bit from this shape‟s Line Style Boolean Properties.
        /// </summary>
        internal const int Stroked = 0x01fc;

        /// <summary>
        /// RK Basically it is an index into another Formula object. You just need to subtract 
        /// 0x0400 to get the index to the other formula.
        /// 
        /// The value calculated from another SG entry in this shape„s pGuides_complex array. 
        /// The index of the guide is specified by taking the value and subtracting 0x0400. 
        /// The index MUST be less than the size of the pGuides_complex array, 
        /// and it MUST be less than the index of this record in that same array.
        /// </summary>
        internal const int FormulaReferenceMin = 0x0400;
        internal const int FormulaReferenceMax = 0x047f;

        /// <summary>
        /// The width of a line in this shape in pixels.
        /// </summary>
        internal const int LineWidthPixels = 0x04f7;
        /// <summary>
        /// The width of this shape in pixels.
        /// </summary>
        internal const int ShapeWidthPixels = 0x04f8;
        /// <summary>
        /// The height of this shape in pixels.
        /// </summary>
        internal const int ShapeHeightPixels = 0x04f9;

        /// <summary>
        /// The width of this shape in English Metric Units.
        /// </summary>
        internal const int ShapeWidthEmus = 0x04fc;
        /// <summary>
        /// The height of this shape in English Metric Units.
        /// </summary>
        internal const int ShapeHeightEmus = 0x04fd;
        /// <summary>
        /// The width of this shape in English Metric Units divided by 2.
        /// </summary>
        internal const int HalfShapeWidthEmus = 0x04fe;
        /// <summary>
        /// The height of this shape in English Metric Units divided by 2.
        /// </summary>
        internal const int HalfShapeHeightEmus = 0x04ff;
    }
}
