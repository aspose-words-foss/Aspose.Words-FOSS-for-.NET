// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/08/2007 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Operation code for a formula of a shape.
    /// 2.2.58 SG in the DOC SPEC.
    /// </summary>
    internal enum Operation
    {
        /// <summary>
        /// Addition and subtraction. param1 + param2 – param3
        /// </summary>
        Sum = 0,
        /// <summary>
        /// Multiplication and division. (param1*param2)/param3
        /// </summary>
        Prod = 1,
        /// <summary>
        /// Simple average. (param1+param2)/2
        /// </summary>
        Mid = 2,
        /// <summary>
        /// Absolute value. abs(param1)
        /// </summary>
        Abs = 3,
        /// <summary>
        /// The lesser of two values. min(param1, param2)
        /// </summary>
        Min = 4,
        /// <summary>
        /// The greater of two values. max(param1, param2)
        /// </summary>
        Max = 5,
        /// <summary>
        /// Conditional selection. param1 > 0 ? param2 : param3
        /// </summary>
        If = 6,
        /// <summary>
        /// Modulus. sqrt(param1^2 + param2^2 + param3^2)
        /// </summary>
        Mod = 7,
        /// <summary>
        /// Trigonometric arc tangent of a quotient. Result is a FixedPoint of the angles in degrees.
        /// atan2(param2,param1)
        /// </summary>
        Atan2 = 8,
        /// <summary>
        /// Sine. param2 is a FixedPoint of angles in degrees.
        /// param1*sin(param2)
        /// </summary>
        Sin = 9,
        /// <summary>
        /// Cosine. param2 is a FixedPoint of angles in degrees.
        /// param1*cos(param2)
        /// </summary>
        Cos = 10,
        /// <summary>
        /// Cosine and atan2 in one formula. param1*cos(atan2(param3,param2))
        /// </summary>
        CosAtan2 = 11,
        /// <summary>
        /// Sine and atan2 in one formula. param1*sin(atan2(param3,param2))
        /// </summary>
        SinAtan2 = 12,
        /// <summary>
        /// Square root. sqrt(param1)
        /// </summary>
        Sqrt = 13,
        /// <summary>
        /// Add an angle in degrees as a FixedPoint to two other angles specified in degrees. 
        /// param2 and param3 are scaled by 2^16. param1 + param2*2^16 - param3*2^16
        /// </summary>
        SumAngle = 14,
        /// <summary>
        /// The eccentricity formula for an ellipse, where param1 is the length of the semiminor 
        /// axis and param2 is the length of the semimajor axis. param3*sqrt(1-(param1/param2)^2)
        /// </summary>
        Ellipse = 15,
        /// <summary>
        /// Tangent. param2 is a FixedPoint of angles in degrees. param1*tan(param2)
        /// </summary>
        Tan = 16
    }
}
