// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2015 by Andrey Noskov

using Aspose.Words.Drawing.Core.Dml.Fills;

namespace Aspose.Words.Drawing.Core.Dml
{
    internal interface IDmlFillProvider
    {
        /// <summary>
        /// Gets the first fill in parent hierarchy that can be drawn.
        /// </summary>
        DmlFill FindDrawableFillInParents();
    }
}