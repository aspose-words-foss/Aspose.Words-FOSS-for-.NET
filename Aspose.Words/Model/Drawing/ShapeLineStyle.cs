// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin


namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies the compound line style of a <see cref="Shape"/>.
    /// </summary>
    /// <seealso cref="Stroke.LineStyle"/>
    /// <dev>
    /// This enum name violates the rule "enum name equals property name", but I have to avoid
    /// collision between Aspose.Words.LineStyle and this line style because otherwise it will
    /// be too hard to port to Java as all types are in the same namespace in Java.
    /// </dev>
    public enum ShapeLineStyle
    {
        /// <summary>
        /// Single line.
        /// </summary>
        Single = 0,
        /// <summary>
        /// Double lines of equal width.
        /// </summary>
        Double = 1,
        /// <summary>
        /// Double lines, one thick, one thin.
        /// </summary>
        ThickThin = 2,
        /// <summary>
        /// Double lines, one thin, one thick.
        /// </summary>
        ThinThick = 3,
        /// <summary>
        /// Three lines, thin, thick, thin.
        /// </summary>
        Triple = 4,
        /// <summary>
        /// Default value is <see cref="Single"/>.
        /// </summary>
        Default = Single
    }
}
