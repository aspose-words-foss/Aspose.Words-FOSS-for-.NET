// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/04/2013 by Victor Chebotok

using System.Text;
using Aspose.Common;
using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Represents an element node of an HTML document tree.
    /// </summary>
    internal class HtmlElementNode : HtmlNode, IHtmlElementProvider
    {
        /// <summary>
        /// Constructor. Initializes a new instance of the class. The element will be in the HTML namespace and will have no
        /// attributes.
        /// </summary>
        /// <param name="name">The element's name.</param>
        internal HtmlElementNode(string name)
            : this(name, W3CNamespaces.Xhtml, new HtmlAttributeCollection())
        {
            // This constructor is empty. The call is delegated to another constructor.
        }

        /// <summary>
        /// Constructor. Initializes a new instance of the class. The element will be in the HTML namespace and will have no
        /// attributes.
        /// </summary>
        /// <param name="name">The element's name.</param>
        /// <param name="isImplicit">Indicates whether the element is implicit.</param>
        internal HtmlElementNode(string name, bool isImplicit)
            : this(name, W3CNamespaces.Xhtml, new HtmlAttributeCollection())
        {
            mIsImplicit = isImplicit;
        }

        /// <summary>
        /// Constructor. Initializes a new instance of the class.
        /// </summary>
        /// <param name="name">The element's name.</param>
        /// <param name="ns">The element's namespace.</param>
        /// <param name="attributes">The element's attributes.</param>
        internal HtmlElementNode(string name, string ns, HtmlAttributeCollection attributes)
        {
            Debug.Assert(StringUtil.HasChars(name));
            Debug.Assert(ns != null);
            Debug.Assert(attributes != null);

            mName = name;
            mNamespace = ns;
            mChildren = new HtmlNodeCollection(this);
            mAttributes = attributes;
        }

        /// <summary>
        /// Searches for an element with the specified name in the depth-first manner.
        /// </summary>
        /// <param name="name">The name of the required element.</param>
        /// <returns>
        /// The first met element that has the specified name, or <c>null</c> if no such element is found.
        /// </returns>
        internal HtmlElementNode FindSingleElementByName(string name)
        {
            HtmlTreeEnumerator enumerator = new HtmlTreeEnumerator(this);
            while (enumerator.MoveNext())
            {
                if (enumerator.IsStart)
                {
                    HtmlElementNode currentElement = enumerator.Current as HtmlElementNode;
                    if ((currentElement != null) && (currentElement.Name == name))
                    {
                        return currentElement;
                    }
                }
            }

            return null;
        }

        IElementProvider IElementProvider.GetParentElement()
        {
            return Parent;
        }

        IElementProvider IElementProvider.GetFirstChildElement()
        {
            for (int i = 0; i < mChildren.Count; i++)
            {
                if (mChildren[i] is IHtmlElementProvider)
                    return (IHtmlElementProvider)mChildren[i];
            }
            return null;
        }

        IElementProvider IElementProvider.GetPreviousSiblingElement()
        {
            HtmlNode siblingElement = PreviousSibling;
            while (siblingElement != null)
            {
                if (siblingElement is IHtmlElementProvider)
                    return (IHtmlElementProvider)siblingElement;
                siblingElement = siblingElement.PreviousSibling;
            }
            return null;
        }

        IElementProvider IElementProvider.GetNextSiblingElement()
        {
            HtmlNode siblingElement = NextSibling;
            while (siblingElement != null)
            {
                if (siblingElement is IHtmlElementProvider)
                    return (IHtmlElementProvider)siblingElement;
                siblingElement = siblingElement.NextSibling;
            }
            return null;
        }

        /// <summary>
        /// Returns child elements of this node.
        /// </summary>
        IElementProvider[] IElementProvider.GetChildElements()
        {
            // Use cached array if it's still actual.
            if (mChildCollectionVersion == mChildren.Version)
            {
                Debug.Assert(mCachedChildElements != null);
                return mCachedChildElements;
            }

            int elementsCount = 0;
            foreach (HtmlNode childNode in mChildren)
            {
                if (childNode is IHtmlElementProvider)
                    elementsCount++;
            }

            mCachedChildElements = new IHtmlElementProvider[elementsCount];
            int index = 0;
            foreach (HtmlNode childNode in mChildren)
            {
                if (childNode is IHtmlElementProvider)
                    mCachedChildElements[index++] = (IHtmlElementProvider)childNode;
            }

            mChildCollectionVersion = mChildren.Version;
            return mCachedChildElements;
        }

        /// <summary>
        /// Returns a value indicating whether this element contains only one child element optionally surrounded by whitespace.
        /// </summary>
        public bool HasOnlyOneChildElementWithOptionalWhitespace()
        {
            bool oneChildElementFound = false;
            foreach (HtmlNode node in mChildren)
            {
                if (node is HtmlElementNode)
                {
                    if (oneChildElementFound)
                    {
                        // There are more than one child element.
                        return false;
                    }
                    oneChildElementFound = true;
                }
                else if (node.ContainsText())
                {
                    // There are non-whitespace child text nodes.
                    return false;
                }
            }
            return oneChildElementFound;
        }

        /// <summary>
        /// Gets an array of classes declared on this element (contents of the 'class' attribute).
        /// </summary>
        string[] IElementProvider.GetClasses()
        {
            CacheClassesIfNeeded();
            return mCachedClasses;
        }

        bool IHtmlElementProvider.StartsWithImplicitBox
        {
            get { return mStartsWithImplicitBox; }
        }

        bool IHtmlElementProvider.EndsWithImplicitBox
        {
            get { return mEndsWithImplicitBox; }
        }

        bool IHtmlElementProvider.ContainsChildBoxes
        {
            get { return mContainsChildBoxes; }
        }

        bool IHtmlElementProvider.IsElementContainsText
        {
            get { return mContainsText; }
        }

        bool IHtmlElementProvider.IsLastChildBox
        {
            get { return mIsLastChildBox; }
        }

        string IElementProvider.GetAttributeValue(string attribName)
        {
            return Attributes.GetAttributeValue(attribName, null);
        }

        string IElementProvider.GetAttributeValue(string attribName, string def)
        {
            return Attributes.GetAttributeValue(attribName, def);
        }

        string IElementProvider.GetLanguage()
        {
            return GetLanguageFromLangAttribute(this);
        }

        string IElementProvider.ElementNamespace
        {
            get { return mNamespace; }
        }

        string IElementProvider.ElementName
        {
            get { return mName; }
        }

        bool IElementProvider.IsImplicit
        {
            get { return mIsImplicit; }
        }

        /// <summary>
        /// Gets the text of the first child text node.
        /// </summary>
        /// <returns>
        /// The text of the first text child node, if the current node has one. If the current node does not have any text child nodes,
        /// an empty string is returned.
        /// </returns>
        public string GetInnerText()
        {
            for (int i = 0; i < mChildren.Count; i++)
            {
                if (mChildren[i] is HtmlTextNode)
                {
                    return ((HtmlTextNode)mChildren[i]).Text;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Serializes the node to XML text.
        /// </summary>
        /// <param name="result">
        /// The text buffer to which the XML text of this node will be appended.
        /// </param>
        /// <param name="serializationContext">Serialization context.</param>
        /// <remarks>
        /// This method follows the algorithm described here: http://www.w3.org/TR/DOM-Parsing/#serializing
        /// </remarks>
        internal override void SerializeToXml(StringBuilder result, XmlSerializationContext serializationContext)
        {
            serializationContext.StartNode(this);

            // Start tag.
            result.Append('<').Append(mName);
            string attributes = mAttributes.Serialize(serializationContext);
            if (attributes != string.Empty)
            {
                result.Append(' ').Append(attributes);
            }

            if (mChildren.Count == 0)
            {
                // Self closing tag.
                result.Append(" />");
            }
            else
            {
                // Finish the start tag.
                result.Append('>');

                // In elements with preformatted text, restore the first LF character, which was removed by HTML parser.
                if ((mNamespace == W3CNamespaces.Xhtml) &&
                    ((mName == "pre") || (mName == "textarea") || (mName == "listing")))
                {
                    if ((mChildren.Count > 0) && (mChildren[0] is HtmlTextNode))
                    {
                        string firstChildText = ((HtmlTextNode)mChildren[0]).Text;
                        if ((firstChildText.Length > 0) && (firstChildText[0] == '\n'))
                        {
                            result.Append('\n');
                        }
                    }
                }

                // Serialize children.
                foreach (HtmlNode child in mChildren)
                {
                    child.SerializeToXml(result, serializationContext);
                }

                // End tag.
                result.Append("</").Append(mName).Append('>');
            }

            serializationContext.EndNode();
        }

        internal override bool IsBoxOrContainsBoxes()
        {
            return mIsBoxOrContainsBoxes;
        }

        internal override bool ContainsText()
        {
            return mContainsText;
        }

        /// <summary>
        /// Gets the element's language from the nearest 'lang' attribute.
        /// </summary>
        /// <returns>
        /// The language defined in the element's 'lang' attribute or in a 'lang' attribute of the nearest parent.
        /// If neither the element itself nor its parents has the 'lang' attribute, empty string is returned;
        /// </returns>
        private static string GetLanguageFromLangAttribute(HtmlElementNode node)
        {
            HtmlAttribute langAttribute = node.Attributes["lang"];
            if (langAttribute != null)
                return langAttribute.Value;

            // Recursively examine element's parents until we met one that has the 'lang' attribute.
            return (node.Parent != null)
                       ? GetLanguageFromLangAttribute(node.Parent)
                       : string.Empty;
        }

        /// <summary>
        /// Gets the name of the element. Names of standard HTML elements created by the HTML parser are always in lowercase.
        /// </summary>
        internal string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets the namespace of the element.
        /// </summary>
        internal string Namespace
        {
            get { return mNamespace; }
        }

        /// <summary>
        /// Gets the children of the element.
        /// </summary>
        internal HtmlNodeCollection Children
        {
            get { return mChildren; }
        }

        /// <summary>
        /// Gets the attributes of the element;
        /// </summary>
        internal HtmlAttributeCollection Attributes
        {
            get { return mAttributes; }
        }

        protected override bool BuildNodeBoxLayout(bool isStart, bool preformattedWhitespace)
        {
            return isStart
                ? BeginBuildNodeBoxLayout()
                : EndBuildNodeBoxLayout();
        }

        protected override bool IsPreformattedWhitespaceNode()
        {
            // The following elements by default have preformatted contents with meaningful whitespace (text of the element
            // must be processed even if it contains whitespace characters only).
            if ((mName == "pre") || (mName == "xmp") || (mName == "listing") || (mName == "plaintext"))
            {
                return true;
            }

            // Whitespace of other elements is not preformatted by default.
            return false;
        }

        private bool BeginBuildNodeBoxLayout()
        {
            mStartsWithImplicitBox = false;
            mEndsWithImplicitBox = false;
            mContainsChildBoxes = false;
            mContainsText = false;
            mIsBoxOrContainsBoxes = false;
            mIsLastChildBox = false;

            switch (mName)
            {
                case "script":
                case "style":
                    // <script> and <style> elements have special meaning, and their text is ignored by the CSS box model.
                    return false;
                case "img":
                case "svg":
                case "br":
                case "hr":
                    // These elements have their own representations in Word documents, so they are considered non-empty
                    // even though they do not contain any text. They are like anonymous (implicit) text blocks.
                    mContainsText = true;
                    break;
                default:
                    // All other elements require no special processing.
                    break;
            }

            return true;
        }

        private bool EndBuildNodeBoxLayout()
        {
            // At this point, box layout of all child nodes has already been built.

            HtmlElementNode lastChildBox = null;
            bool isFirstElementWithText = true;
            foreach (HtmlNode child in mChildren)
            {
                if (child.ContainsText())
                {
                    if (isFirstElementWithText)
                    {
                        mStartsWithImplicitBox = !child.IsBoxOrContainsBoxes();
                        isFirstElementWithText = false;
                    }
                    mContainsText = true;
                    mEndsWithImplicitBox = !child.IsBoxOrContainsBoxes();
                }

                if (child.IsBoxOrContainsBoxes())
                {
                    mContainsChildBoxes = true;
                    HtmlElementNode childElement = child as HtmlElementNode;
                    if (childElement != null)
                    {
                        lastChildBox = childElement;
                    }
                }
            }

            mIsBoxOrContainsBoxes =
                (HtmlElementCategorizer.IsBlockElementByDefault(mName) && mContainsText) ||
                mContainsChildBoxes;

            if (lastChildBox != null)
            {
                lastChildBox.mIsLastChildBox = true;
            }
            if (Parent == null)
            {
                mIsLastChildBox = true;
            }

            return true;
        }

        private void CacheClassesIfNeeded()
        {
            if (mAttributesVersion != mAttributes.Version)
            {
                CacheClasses();
            }
        }

        private void CacheClasses()
        {
            string classValue = mAttributes.GetAttributeValue("class", string.Empty);
            mCachedClasses = (classValue != string.Empty)
                ? CssUtil.SplitClassAttributeValue(classValue)
                : gNoClasses;
            mAttributesVersion = mAttributes.Version;
        }

        /// <summary>
        /// A zero-length array of CSS class names.
        /// </summary>
        private static readonly string[] gNoClasses = new string[0];

        private readonly string mName;
        private readonly string mNamespace;
        private readonly HtmlNodeCollection mChildren;
        private readonly HtmlAttributeCollection mAttributes;

        private bool mStartsWithImplicitBox;
        private bool mEndsWithImplicitBox;
        private bool mContainsChildBoxes;
        private bool mIsLastChildBox;
        private bool mContainsText;
        private bool mIsBoxOrContainsBoxes;
        private readonly bool mIsImplicit;

        /// <summary>
        /// Cached child elements. Used for performance improvement.
        /// </summary>
        private IHtmlElementProvider[] mCachedChildElements;
        private long mChildCollectionVersion;

        /// <summary>
        /// An array of individual CSS classes declared on this element via the 'class' attribute.
        /// </summary>
        /// <remarks>
        /// SPEED. Individual classes are used when matching CSS selectors and HTML elements, which is a very frequent
        /// operation. We improve performance by splitting classes only once and caching the result.
        /// </remarks>
        private string[] mCachedClasses;
        private long mAttributesVersion;
    }
}
