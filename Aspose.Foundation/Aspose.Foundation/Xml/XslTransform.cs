// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2023 by Edward Voronov

using System.IO;
using System.Xml.Xsl;
using Aspose.JavaAttributes;

namespace Aspose.Xml
{
    /// <summary>Transforms XML data using an XSLT style sheet.</summary>
    /// <seealso cref="XslCompiledTransform"/>
    [JavaManual("Manual porting by design.")]
    public sealed class XslTransform
    {
        /// <summary>Compiles the style sheet contained in the <see cref="Stream" /> object.</summary>
        /// <seealso cref="XslCompiledTransform.Load(System.Xml.XmlReader)"/>
        public void Load(Stream stylesheet)
        {
            mNativeXslTransform.Load(XmlUtilPal.LoadXml(stylesheet, true));
        }

        /// <summary>Executes the transform using the input document specified by the <see cref="Stream" /> object and outputs the results to a stream.</summary>
        /// <seealso cref="XslCompiledTransform.Transform(System.Xml.XmlReader,XsltArgumentList,Stream)"/>
        public void TransformTo(Stream input, Stream result)
        {
            mNativeXslTransform.Transform(XmlUtilPal.LoadXml(input, true), null, result);
            result.Position = 0;
        }

        /// <summary>Executes the transform using the input document specified by the <see cref="Stream" /> object and outputs the results to a stream.</summary>
        /// <seealso cref="XslCompiledTransform.Transform(System.Xml.XmlReader,XsltArgumentList,Stream)"/>
        public Stream Transform(Stream input)
        {
            Stream result = new MemoryStream();
            TransformTo(input, result);
            return result;
        }

        private readonly XslCompiledTransform mNativeXslTransform = new XslCompiledTransform();
    }
}
