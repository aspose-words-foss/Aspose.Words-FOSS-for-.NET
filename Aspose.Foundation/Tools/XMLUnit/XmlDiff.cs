// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by romeok

namespace XmlUnit {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    
    public class XmlDiff {
        private const string XMLNS_PREFIX = "xmlns";

        public float MaxDelta;

        private readonly XmlInput controlInput;
        private readonly XmlInput testInput;
        private readonly DiffConfiguration _diffConfiguration;
        private DiffResult _diffResult;
                
        public XmlDiff(XmlInput control, XmlInput test, 
                       DiffConfiguration diffConfiguration) {
            _diffConfiguration =  diffConfiguration;
            controlInput = control;
            testInput = test;

            MaxDelta = 0;
        }
        
        public XmlDiff(XmlInput control, XmlInput test)
            : this(control, test, new DiffConfiguration()) {
        }

        public XmlDiff(TextReader control, TextReader test)
            : this(new XmlInput(control), new XmlInput(test)) {
        }
        
        public XmlDiff(string control, string test) 
            : this(new XmlInput(control), new XmlInput(test)) {
        }
        
        private XmlReader CreateXmlReader(XmlInput forInput) {
            XmlReader xmlReader = forInput.CreateXmlReader();
        	
            if (xmlReader is XmlTextReader) {
                ((XmlTextReader) xmlReader ).WhitespaceHandling = _diffConfiguration.WhitespaceHandling;
            }
            
            if (_diffConfiguration.UseValidatingParser) {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Auto;

                XmlReader validatingReader = XmlReader.Create(xmlReader, settings);
                return validatingReader;
            }
            
            return xmlReader;
        }
        
        public DiffResult Compare() {
            if (_diffResult == null) {
                _diffResult = new DiffResult();
                XmlReader controlReader, testReader;
                controlReader = null;
                testReader = null;
                try {
                    controlReader = CreateXmlReader(controlInput);
                    testReader = CreateXmlReader(testInput);
                    if (!controlInput.Equals(testInput)) {
                        Compare(_diffResult, controlReader, testReader);
                    }
                } finally {
                    try {
                        if (testReader != null) {
                            testReader.Close();
                        }
                    } finally {
                        if (controlReader != null) {
                            controlReader.Close();
                        }
                    }
                }
            }
            return _diffResult;
        }
        
        private void Compare(DiffResult result, XmlReader controlReader,
                             XmlReader testReader) {
            try {
                ReaderWithState control = new ReaderWithState(controlReader);
                ReaderWithState test = new ReaderWithState(testReader);
                do {
                    control.Read();
                    test.Read();
                    Compare(result, control, test);
                } while (control.HasRead && test.HasRead) ;
            } catch (FlowControlException e) 
            {    
                // RK I do not want this to litter my console.
                // Console.Out.WriteLine(e.Message);
            }
        }        
        
        private void Compare(DiffResult result, ReaderWithState control,
                             ReaderWithState test) {
            if (control.HasRead) {
                if (test.HasRead) {
                    CompareNodes(result, control, test);
                    CheckEmptyOrAtEndElement(result, control, test);
                } else {
                    DifferenceFound(DifferenceType.CHILD_NODELIST_LENGTH_ID, result);
                } 
            }
        }
                
        private void CompareNodes(DiffResult result, ReaderWithState control,
                                  ReaderWithState test) {
            XmlNodeType controlNodeType = control.Reader.NodeType;
            XmlNodeType testNodeType = test.Reader.NodeType;
            if (!controlNodeType.Equals(testNodeType)) {
                CheckNodeTypes(controlNodeType, testNodeType, result,
                               control, test);
            } else if (controlNodeType == XmlNodeType.Element) {
                CompareElements(result, control, test);
            } else if (controlNodeType == XmlNodeType.Text) {
                CompareText(result, control, test);
            }
        }
        
        private void CheckNodeTypes(XmlNodeType controlNodeType,
                                    XmlNodeType testNodeType,
                                    DiffResult result,
                                    ReaderWithState control,
                                    ReaderWithState test) {
            ReaderWithState readerToAdvance = null;
            if (controlNodeType.Equals(XmlNodeType.XmlDeclaration)) {
                readerToAdvance = control;
            } else if (testNodeType.Equals(XmlNodeType.XmlDeclaration)) {        			
                readerToAdvance = test;
            }
        	
            if (readerToAdvance != null) {
            	DifferenceFound(DifferenceType.HAS_XML_DECLARATION_PREFIX_ID, 
            	                controlNodeType, testNodeType, result);
                readerToAdvance.Read();
                CompareNodes(result, control, test);
            } else {
            	DifferenceFound(DifferenceType.NODE_TYPE_ID, controlNodeType, 
             	                testNodeType, result);
            }       
        }
        
        private void CompareElements(DiffResult result, ReaderWithState control,
                                     ReaderWithState test) {
            string controlTagName = control.Reader.Name;
            string testTagName = test.Reader.Name;
            if (!String.Equals(controlTagName, testTagName)) {
                DifferenceFound(DifferenceType.ELEMENT_TAG_NAME_ID, result);
            } else {
                XmlAttribute[] controlAttributes =
                    GetNonSpecialAttributes(control);
                XmlAttribute[] testAttributes = GetNonSpecialAttributes(test);
                if (controlAttributes.Length != testAttributes.Length) {
                    DifferenceFound(DifferenceType.ELEMENT_NUM_ATTRIBUTES_ID, result);
                }
                CompareAttributes(result, controlAttributes, testAttributes);
            }
        }
        
        private void CompareAttributes(DiffResult result,
                                       XmlAttribute[] controlAttributes,
                                       XmlAttribute[] testAttributes) {
            List<XmlAttribute> unmatchedTestAttributes = new List<XmlAttribute>();
            unmatchedTestAttributes.AddRange(testAttributes);
            for (int i=0; i < controlAttributes.Length; ++i) {
                
                bool controlIsInNs = IsNamespaced(controlAttributes[i]);
                string controlAttrName =
                    GetUnNamespacedNodeName(controlAttributes[i]);
                XmlAttribute testAttr = null;
                if (!controlIsInNs) {
                    testAttr = FindAttributeByName(testAttributes,
                                                   controlAttrName);
                } else {
                    testAttr = FindAttributeByNameAndNs(testAttributes,
                                                        controlAttrName,
                                                        controlAttributes[i]
                                                        .NamespaceURI);
                }

                if (testAttr != null) {
                    unmatchedTestAttributes.Remove(testAttr);
                    if (!_diffConfiguration.IgnoreAttributeOrder
                        && testAttr != testAttributes[i]) {
                        DifferenceFound(DifferenceType.ATTR_SEQUENCE_ID,
                                        result);
                    }

                    if (controlAttributes[i].Value != testAttr.Value) 
					{
						DifferenceFound(DifferenceType.ATTR_VALUE_ID, result);

						// RK This code was added by amorozov to allow XPS gold verification to pass on Mono by
						// allowing some delta in the compared values.
						//
						// Major problem:
						// 1. This code always reports "comparision success" back to the program and provides the MaxDelta found.
						// This is completely wrong. This code should accept MaxAllowedDelta in the configuration and then
						// report a difference if delta exceeds the value.
						// 
						// Minor problem:
						// 2. I do not have capability on Java. I can code that (XmlDiff on Java seems to be extensible),
						// but maybe we can get the values to actually match...
						//
/*
                        string str1 = controlAttributes[i].Value, str2 = testAttr.Value;

                        float float1 = 0, float2 = 0;

                        if (controlAttributes[i].LocalName == "OriginX" || controlAttributes[i].LocalName == "OriginY")
                        {
                            float1 = Convert.ToSingle(str1);
                            float2 = Convert.ToSingle(str2);
                            MaxDelta = Math.Max(MaxDelta, Math.Abs(float1 - float2));
                        }
                        else if (controlAttributes[i].LocalName == "RenderTransform")
                        {
                            float1 = Convert.ToSingle(str1.Split(',')[4]);
                            float2 = Convert.ToSingle(str2.Split(',')[4]);
                            MaxDelta = Math.Max(MaxDelta, Math.Abs(float1 - float2));
                        }
                        else if (controlAttributes[i].LocalName == "Data")
                        {
                            String[] data1 = str1.Split(' ');
                            String[] data2 = str2.Split(' ');

                            for (int i1 = 0; i < data1.Length - 1; i++)
                            {
                                float1 = Convert.ToSingle(data1[i1].Split(',')[0].TrimStart('M', 'L'));
                                float2 = Convert.ToSingle(data2[i1].Split(',')[0].TrimStart('M', 'L'));
                                MaxDelta = Math.Max(MaxDelta, Math.Abs(float1 - float2));

                                float1 = Convert.ToSingle(data1[i1].Split(',')[1]);
                                float2 = Convert.ToSingle(data2[i1].Split(',')[1]);
                                MaxDelta = Math.Max(MaxDelta, Math.Abs(float1 - float2));
                            }
                            
                        }
                        else
                        {
                            DifferenceFound(DifferenceType.ATTR_VALUE_ID, result);
                        }
*/						
                    }

                } else {
                    DifferenceFound(DifferenceType.ATTR_NAME_NOT_FOUND_ID,
                                    result);
                }
            }
            foreach (XmlAttribute a in unmatchedTestAttributes) {
                DifferenceFound(DifferenceType.ATTR_NAME_NOT_FOUND_ID, result);
            }
        }
        
      private void CompareText(DiffResult result, ReaderWithState control,
                               ReaderWithState test) {
            string controlText = control.Reader.Value;
            string testText = test.Reader.Value;
            if (!String.Equals(controlText, testText)) {
                DifferenceFound(DifferenceType.TEXT_VALUE_ID, result);
            }
        }
        
        private void DifferenceFound(DifferenceType differenceType, DiffResult result) {
            DifferenceFound(new Difference(differenceType), result);
        }
        
        private void DifferenceFound(Difference difference, DiffResult result) {
            result.DifferenceFound(this, difference);
            if (!ContinueComparison(difference)) {
                throw new FlowControlException(difference);
            }
        }
        
        private void DifferenceFound(DifferenceType differenceType, 
                                     XmlNodeType controlNodeType,
                                     XmlNodeType testNodeType, 
                                     DiffResult result) {
            DifferenceFound(new Difference(differenceType, controlNodeType, testNodeType),
                            result);
        }
        
        private bool ContinueComparison(Difference afterDifference) {
            return !afterDifference.MajorDifference;
        }
        
        private void CheckEmptyOrAtEndElement(DiffResult result, 
                                              ReaderWithState control,
                                              ReaderWithState test) {
            if (control.LastElementWasEmpty) {
                if (!test.LastElementWasEmpty) {
                    CheckEndElement(test, result);
                }
            } else {
                if (test.LastElementWasEmpty) {
                    CheckEndElement(control, result);
                }
            }
        }
        
        private XmlAttribute[] GetNonSpecialAttributes(ReaderWithState r) {
            List<XmlAttribute> l = new List<XmlAttribute>();
            int length = r.Reader.AttributeCount;
            if (length > 0) {
                XmlDocument doc = new XmlDocument();
                r.Reader.MoveToFirstAttribute();
                for (int i = 0; i < length; i++) {
                    XmlAttribute a = doc.CreateAttribute(r.Reader.Name,
                                                         r.Reader.NamespaceURI);
                    if (!IsXMLNSAttribute(a)) {
                        a.Value = r.Reader.Value;
                        l.Add(a);
                    }
                    r.Reader.MoveToNextAttribute();
                }
            }
            return l.ToArray();
        }

        private bool IsXMLNSAttribute(XmlAttribute attribute) {
            return XMLNS_PREFIX == attribute.Prefix ||
                XMLNS_PREFIX == attribute.Name;
        }

        private XmlAttribute FindAttributeByName(XmlAttribute[] attrs,
                                                 string name) {
            foreach (XmlAttribute a in attrs) {
                if (GetUnNamespacedNodeName(a) == name) {
                    return a;
                }
            }
            return null;
        }

        private XmlAttribute FindAttributeByNameAndNs(XmlAttribute[] attrs,
                                                      string name,
                                                      string nsUri) {
            foreach (XmlAttribute a in attrs) {
                if (GetUnNamespacedNodeName(a) == name
                    && a.NamespaceURI == nsUri) {
                    return a;
                }
            }
            return null;
        }

        private string GetUnNamespacedNodeName(XmlNode aNode) {
            return GetUnNamespacedNodeName(aNode, IsNamespaced(aNode));
        }
    
        private string GetUnNamespacedNodeName(XmlNode aNode,
                                               bool isNamespacedNode) {
            if (isNamespacedNode) {
                return aNode.LocalName;
            }
            return aNode.Name;
        }

        private bool IsNamespaced(XmlNode aNode) {
            string ns = aNode.NamespaceURI;
            return ns != null && ns.Length > 0;
        }

        private void CheckEndElement(ReaderWithState reader, DiffResult result) {            
            bool readResult = reader.Read();
            if (!readResult
                || reader.Reader.NodeType != XmlNodeType.EndElement) {
                DifferenceFound(DifferenceType.CHILD_NODELIST_LENGTH_ID, result);
            }
        }
        
        public string OptionalDescription {
            get {
                return _diffConfiguration.Description;
            }
        }

        private class FlowControlException : ApplicationException {
            public FlowControlException(Difference cause) : base(cause.ToString()) {
            }
        }
        
        private class ReaderWithState {
            internal ReaderWithState(XmlReader reader) {
                Reader = reader;
                HasRead = false;
                LastElementWasEmpty = false;
            }

            internal readonly XmlReader Reader;
            internal bool HasRead;
            internal bool LastElementWasEmpty;

            internal bool Read() {
                HasRead = Reader.Read();
                if (HasRead) {
                    switch (Reader.NodeType) {
                    case XmlNodeType.Element:
                        LastElementWasEmpty = Reader.IsEmptyElement;
                        break;
                    case XmlNodeType.EndElement:
                        LastElementWasEmpty = false;
                        break;
                    default:
                        // don't care
                        break;
                    }
                }
                return HasRead;
            }

            internal string State {
                get {
                    return string.Format("Name {0}, NodeType {1}, IsEmpty {2},"
                                         + " HasRead {3}, LastWasEmpty {4}",
                                         Reader.Name,
                                         Reader.NodeType,
                                         Reader.IsEmptyElement,
                                         HasRead, LastElementWasEmpty);
                }
            }
        }
    }
}
