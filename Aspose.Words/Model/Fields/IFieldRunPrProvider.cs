// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2013 by Ivan Lyagin

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides an access to run properties' collection which is not owned by any run.
    /// </summary>
    internal interface IFieldRunPrProvider
    {
        /// <summary>
        /// Returns a run properties' collection which is not owned by any run.
        /// </summary>
        [JavaThrows(true)]
        RunPr GetRunPr();
    }
}
