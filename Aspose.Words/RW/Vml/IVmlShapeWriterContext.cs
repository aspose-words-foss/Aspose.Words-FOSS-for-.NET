// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/08/2007 by Vladimir Averkin

using System.Collections.Generic;
using Aspose.Drawing;
using Aspose.JavaAttributes;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// RK Makes it possible to use common shape writing code for WML and DOCX exports.
    /// </summary>
    internal interface IVmlShapeWriterContext
    {
        /// <summary>
        /// Returns a value that will be written to idsrc, iddest and idcntry fields
        /// in the diagram relationtable.
        /// </summary>
        /// <param name="key">A model (and DOC, RTF) value (hash of the shape name).</param>
        /// <returns>The value that can be written to WML or DOCX (shape id).</returns>
        string GetDiagramMemberName(int key);

        [JavaThrows(true)]
        string WriteImageBinData(byte[] imageBytes);

        string WriteImageLink(string link);

        /// <summary>
        /// Converts the specified <see cref="DrColor"/> to VML color string.
        /// </summary>
        /// <remarks>
        /// Logs a warning to the user-provided warning callback if this is a special color definition (system or scheme
        /// color or palette index) that is currently not supported.
        /// </remarks>
        string ColorToVml(DrColor color);

        IList<string> ShapeTypesWritten { get; }

        SaveInfo SaveInfo { get; }

        /// <summary>
        /// The name of XML attribute to use when writing image reference for image binary data in VML.
        /// </summary>
        string ImageSrcAttributeName { get; }

        /// <summary>
        /// The name of XML attribute to use when writing external image reference for image binary data in VML.
        /// </summary>
        string ImageHrefAttributeName { get; }

        bool IsDocx { get; }

        /// <summary>
        /// Indicates the current position is inside the complex field area.
        /// </summary>
        bool IsInsideField { get; }
    }
}
