// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/05/2017 by Edward Voronov

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents the <see cref="FieldAutoText"/> and <see cref="FieldGlossary"/> field code.
    /// </summary>
    internal interface IFieldAutoTextCode
    {
        /// <summary>
        /// Gets or sets the name of the AutoText entry.
        /// </summary>
        [JavaThrows(true)]
        string EntryName { get; }
    }
}
