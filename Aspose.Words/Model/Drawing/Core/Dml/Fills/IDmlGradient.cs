// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/03/2011 by Alexey Titov

using System.Collections.Generic;
using System.Drawing;
using Aspose.Drawing;
using Aspose.JavaAttributes;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// Represents a interface for gradient types.
    /// </summary>
    internal interface IDmlGradient
    {
        IDmlGradient Clone();

        void Write(IDmlShapeWriterContext writer, bool isTextEffect);

        /// <summary>
        /// Returns the gradient variant for the specified fill as an integer value from 1 to 4, or 0 if not defined.
        /// </summary>
        GradientVariant GetGradientVariant(IList<DmlGradientStop> gradientStops);

        bool AreColorsInReverseOrder { get; }

        /// <summary>
        /// Gets <see cref="GradientStyle"/> of the gradient.
        /// </summary>
        GradientStyle GradientStyle { get; }
    }
}
