// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by romeok

namespace XmlUnit {
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;
#if !NETSTANDARD2_0
    using System.Security.Policy;
#endif

    public class Xslt {
        private readonly XmlInput _xsltInput;
    	private readonly XmlResolver _xsltResolver;
#if !NETSTANDARD2_0
        private readonly Evidence _evidence;
    	
        public Xslt(XmlInput xsltInput, XmlResolver xsltResolver, Evidence evidence) {
            _xsltInput = xsltInput;
        	_xsltResolver = xsltResolver;
        	_evidence = evidence;
        }
        
        public Xslt(XmlInput xsltInput) 
        	: this(xsltInput, null, null) {
        }
#else
        public Xslt(XmlInput xsltInput, XmlResolver xsltResolver)
        {
            _xsltInput = xsltInput;
            _xsltResolver = xsltResolver;
        }

        public Xslt(XmlInput xsltInput)
            : this(xsltInput, null)
        {
        }
#endif

        public Xslt(string xslt, string baseURI) 
            : this(new XmlInput(xslt, baseURI)) {
        }

        
        public Xslt(string xslt) 
            : this(new XmlInput(xslt)) {
        }
        
        public XmlOutput Transform(string someXml) {
        	return Transform(new XmlInput(someXml)); 
        }
        
        public XmlOutput Transform(XmlInput someXml) {
        	return Transform(someXml, null);
        }
        
        public XmlOutput Transform(XmlInput someXml, XsltArgumentList xsltArgs) {
        	return Transform(someXml.CreateXmlReader(), null, xsltArgs);
        }
        
        public XmlOutput Transform(XmlReader xmlTransformed, XmlResolver resolverForXmlTransformed, XsltArgumentList xsltArgs) {
            XslCompiledTransform transform = new XslCompiledTransform();
	        XmlReader xsltReader = _xsltInput.CreateXmlReader();

#if CPLUSPLUS
            transform.Load(xsltReader);
#elif !NETSTANDARD2_0
            XmlSecureResolver secureResolver = new XmlSecureResolver(_xsltResolver, _evidence);
            transform.Load(xsltReader, XsltSettings.Default, secureResolver);
#else
            transform.Load(xsltReader, XsltSettings.Default, _xsltResolver);
#endif

            XmlSpace space = XmlSpace.Default;
            XPathDocument document = new XPathDocument(xmlTransformed, space);
            XPathNavigator navigator = document.CreateNavigator();
            
            return new XmlOutput(transform, xsltArgs, navigator, resolverForXmlTransformed, 
                                 new XmlReader[] {xmlTransformed, xsltReader});
        }
    }
}
