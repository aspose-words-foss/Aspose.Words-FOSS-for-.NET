// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/05/2016 by Alexander Zhiltsov

using Aspose.Collections;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Allows getting/setting an extension list.
    /// </summary>
    internal interface IDmlExtensionListSource
    {
        /// <summary>
        /// Represents collection of Dml extensions.
        /// </summary>
        /// <remarks>
        /// The key is <see cref="DmlExtensionUri"/> the value is <see cref="DmlExtension"/>.
        /// </remarks>
        StringToObjDictionary<DmlExtension> Extensions { get; set; }
    }
}
