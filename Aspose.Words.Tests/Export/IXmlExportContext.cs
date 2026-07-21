// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/04/2023 by Edward Voronov

using System.Collections.Generic;
using System.Xml;

namespace Aspose.Words.Tests.Export
{
    /// <summary>
    /// Provides access to internal Docx/Wml structure.
    /// </summary>
    internal interface IXmlExportContext
    {
        /// <summary>
        /// Searches for a XML node with using specified parent node and XPath query. First found node is returned.
        /// </summary>
        XmlNode GetXmlNode(XmlElement parentNode, string xpath);

        /// <summary>
        /// Searches for XML nodes with using specified parent node and XPath query. Returns all found nodes.
        /// </summary>
        IList<XmlNode> GetXmlNodes(XmlElement parentNode, string xpath);

        XmlDocument XmlDocument { get; }
    }
}
