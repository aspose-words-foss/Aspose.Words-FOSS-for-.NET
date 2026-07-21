// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2016 by Edward Voronov

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides access to field code arguments and switches.
    /// </summary>
    internal interface IFieldCode
    {
        /// <summary>
        /// Gets an argument as a plain text at the specified index.
        /// </summary>
        [JavaThrows(true)]
        string GetArgumentAsString(int authorNameArgumentIndex);

        /// <summary>
        /// Returns <c>true</c> if the switch was found in the field code.
        /// </summary>
        bool HasSwitch(string switchName);
    }
}
