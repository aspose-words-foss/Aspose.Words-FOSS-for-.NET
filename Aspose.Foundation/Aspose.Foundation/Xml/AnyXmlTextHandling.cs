// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/08/2010 by Roman Korchagin

using System;

namespace Aspose.Xml
{
    /// <summary>
    /// Specifies (in an autoportable way) how text, ignorable and significant whitespace 
    /// will be returned by <see cref="AnyXmlReader"/>.
    /// </summary>
    [Flags]
    public enum AnyXmlTextHandling
    {
        /// <summary>
        /// No text and whitespace is returned to the caller. Only elements are returned.
        /// </summary>
        None = 0x0000,
        Text = 0x0001,
        SignificantWhitespace = 0x0002,
        Whitespace = 0x0004,
        ProcessingInstruction = 0x0008,
        EntityReference = 0x0010
    }
}
