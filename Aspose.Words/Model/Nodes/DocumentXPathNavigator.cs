// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2005 by Roman Korchagin
using System.Xml;
using System.Xml.XPath;
using Aspose.JavaAttributes;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// This class is to be MANUALLY ported to Java.
    /// 
    /// Allows XPath navigation over a Word document.
    /// </summary>
    [JavaManual("In Java this works with Jaxen Xpath engine.")]
    [CodePorting.Translator.Cs2Cpp.CppSkipEntity("XPath navigation is supported on C++ yet.")]
    internal class DocumentXPathNavigator : XPathNavigator
    {
        /// <summary>
        /// This method is needed both on .NET and Java.
        /// </summary>
        internal static NodeList SelectNodes(Node node, string xpath)
        {
            XPathNavigator navigator = new DocumentXPathNavigator(node);
            return new NodeList(navigator.Select(xpath));
        }

        /// <summary>
        /// This method is needed both on .NET and Java.
        /// </summary>
        internal static Node SelectSingleNode(Node node, string xpath)
        {
            XPathNavigator navigator = new DocumentXPathNavigator(node);
            XPathNodeIterator it = navigator.Select(xpath);
            if (it.MoveNext())
            {
                DocumentXPathNavigator curNav = (DocumentXPathNavigator)it.Current;
                return curNav.CurrentNode;
            }
            else
                return null;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal DocumentXPathNavigator(Node node)
        {
            mDoc = node.Document;
            mCurrentNode = node;
            mNameTable = new NameTable();
            mNameTable.Add(string.Empty);
        }

        /// <summary>
        /// Copy ctor for Clone to work.
        /// </summary>
        private DocumentXPathNavigator(DocumentXPathNavigator src)
        {
            mDoc = src.mDoc;
            mCurrentNode = src.mCurrentNode;
            //Clones can share the same name table okay.
            mNameTable = src.mNameTable;
        }

        public override XPathNodeType NodeType
        {
            get
            {
                switch (mCurrentNode.NodeType)
                {
                    case Aspose.Words.NodeType.Document:
                    case Aspose.Words.NodeType.GlossaryDocument:
                        return XPathNodeType.Root;
                    default:
                        return XPathNodeType.Element;
                }
            }
        }

        public override string LocalName
        {
            get
            {
                mNameTable.Add(Name);
                return mNameTable.Get(Name);
            }
        }

        public override string Name
        {
            get
            {
                return mCurrentNode.GetType().Name;
            }
        }

        public override string Prefix
        {
            get { return mNameTable.Get(string.Empty); }
        }

        public override string NamespaceURI
        {
            get { return mNameTable.Get(string.Empty); }
        }

        public override string Value
        {
            get { return mCurrentNode.GetText(); }
        }

        public override string BaseURI
        {
            get { return string.Empty; }
        }

        public override string XmlLang
        {
            get { return string.Empty; }
        }

        public override bool IsEmptyElement
        {
            get { return false; }
        }

        public override XmlNameTable NameTable
        {
            get { return mNameTable; }
        }

        public override bool HasAttributes
        {
            get { return false; }
        }

        public override bool HasChildren
        {
            get
            {
                return (mCurrentNode.IsComposite && ((CompositeNode)mCurrentNode).FirstChild != null);
            }
        }

        public Node CurrentNode
        {
            get { return mCurrentNode; }
        }

        public override XPathNavigator Clone()
        {
            return new DocumentXPathNavigator(this);
        }

        public override string GetAttribute(string localName, string namespaceURI)
        {
            return string.Empty;
        }

        public override bool MoveToAttribute(string localName, string namespaceURI)
        {
            return false;
        }

        public override bool MoveToFirstAttribute()
        {
            return false;
        }

        public override bool MoveToNextAttribute()
        {
            return false;
        }

        public override string GetNamespace(string name)
        {
            return string.Empty;
        }

        public override bool MoveToNamespace(string name)
        {
            return false;
        }

        [CppSkipEntity("C++ has incomplited implemnetation of XPathNavigator interface")]
        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            return false;
        }

        [CppSkipEntity("C++ has incomplited implemnetation of XPathNavigator interface")]
        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            return false;
        }

        public override bool MoveToNext()
        {
            return MoveTo(mCurrentNode.NextSibling);
        }

        public override bool MoveToPrevious()
        {
            return MoveTo(mCurrentNode.PreviousSibling);
        }

        public override bool MoveToFirst()
        {
            if (mCurrentNode.ParentNode == null)
                return false;

            return MoveTo(mCurrentNode.ParentNode.FirstChild);
        }

        public override bool MoveToFirstChild()
        {
            return (mCurrentNode.IsComposite) && MoveTo(((CompositeNode)mCurrentNode).FirstChild);
        }

        public override bool MoveToParent()
        {
            return MoveTo(mCurrentNode.ParentNode);
        }

        public override void MoveToRoot()
        {
            MoveTo(mDoc);
        }

        public override bool MoveTo(XPathNavigator other)
        {
            return MoveTo(((DocumentXPathNavigator)other).mCurrentNode);
        }

        public override bool MoveToId(string id)
        {
            return false;
        }

        public override bool IsSamePosition(XPathNavigator other)
        {
            return (mCurrentNode == ((DocumentXPathNavigator)other).mCurrentNode);
        }

        private bool MoveTo(Node node)
        {
            if (node != null)
            {
                mCurrentNode = node;
                return true;
            }
            else
                return false;
        }

        private readonly DocumentBase mDoc;
        private Node mCurrentNode;
        private readonly NameTable mNameTable;
    }
}
