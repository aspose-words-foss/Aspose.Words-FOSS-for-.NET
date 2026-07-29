// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/05/2009 by Roman Korchagin

using System.Collections.Generic;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Associates protected content with a specific data space definition.
    /// </summary>
    internal class DataSpaceMapEntry
    {
        /// <summary>
        /// Specifies the name of the data space definition associated with the protected content 
        /// specified in the ReferenceComponents field.
        /// </summary>
        internal string DataSpaceName;

        /// <summary>
        /// Item is a string name of a reference component.
        /// </summary>
        internal IList<string> ReferenceComponents = new List<string>();
    }
}
