// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// Base class for text autofit modes.
    /// </summary>
    internal abstract class DmlAutoFitMode
    {
        internal DmlAutoFitMode Clone()
        {
            return (DmlAutoFitMode)MemberwiseClone();
        }
    }
}