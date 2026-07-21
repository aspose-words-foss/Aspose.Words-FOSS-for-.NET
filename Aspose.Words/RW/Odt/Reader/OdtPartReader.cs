// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System.IO;
using Aspose.Xml;


namespace Aspose.Words.RW.Odt.Reader
{
    /// <summary>
    /// Wrapper around XmlTextReader. Used for reading ODT parts.
    /// </summary>
    internal class OdtPartReader : AnyXmlReader
    {
        internal OdtPartReader(Stream partStream) : base(partStream)
        {
        }
    }
}
