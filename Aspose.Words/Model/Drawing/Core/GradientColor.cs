// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/07/2006 by Roman Korchagin

using Aspose.Drawing;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Defines a band of color in a gradient with multiple colors.
    ///
    /// Made into a class to simplify autporting to Java. Do not make this a struct.
    /// </summary>
    internal class GradientColor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GradientColor" /> class.
        /// </summary>
        internal GradientColor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientColor" /> class.
        /// </summary>
        internal GradientColor(GradientColor other)
        {
            Color = new DrColor(other.Color.ToArgb());
            Start = other.Start;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if ((obj == null) || (obj.GetType() != typeof(GradientColor)))
                return false;

            GradientColor other = (GradientColor)obj;
            return ((Start == other.Start) && Color.Equals(other.Color));
        }

        /// <summary>
        /// The index of this instance in collection to which it belongs.
        /// </summary>
        /// <remarks>
        /// This used in sorting to preserve indexes of equal gradient colors within collection being sorted.
        /// </remarks>
        internal int Index { get; set; }

        /// <summary>
        /// The color of the band.
        /// </summary>
        internal DrColor Color;
        /// <summary>
        /// The starting position of the band in the whole gradient. This is Fixed data type.
        /// </summary>
        internal int Start;
    }
}
