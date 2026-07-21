// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/04/2013 by Victor Chebotok

using System.Text;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Represents a node of an HTML document tree.
    /// </summary>
    internal abstract class HtmlNode
    {
        /// <summary>
        /// Serializes the node to XML text.
        /// </summary>
        /// <returns>
        /// The string containing XML text of this node.
        /// </returns>
        /// <remarks>
        /// At the moment this method is used only to serialize SVG fragments back to text.
        /// </remarks>
        internal string SerializeToXml()
        {
            XmlSerializationContext context = new XmlSerializationContext(this);
            StringBuilder result = new StringBuilder();
            SerializeToXml(result, context);
            return result.ToString();
        }

        /// <summary>
        /// Recalculates properties of all HTML elements in this HTML tree for the CSS box model.
        /// </summary>
        internal void BuildBoxLayout()
        {
            HtmlElementNode root = this as HtmlElementNode;
            if (root == null)
                return;

            // Subtrees with preformatted whitespace require special processing. We rember the root of such a subtree while
            // traversing its children.
            HtmlNode preformattedWhitespaceSubtreeRoot = null;

            HtmlTreeEnumerator enumerator = new HtmlTreeEnumerator(root);
            while (enumerator.MoveNext())
            {
                if (enumerator.IsStart &&
                    (preformattedWhitespaceSubtreeRoot == null) &&
                    enumerator.Current.IsPreformattedWhitespaceNode())
                {
                    // We're entering a preformatted whitespace subtree.
                    preformattedWhitespaceSubtreeRoot = enumerator.Current;
                }

                bool preformattedWhitespace = preformattedWhitespaceSubtreeRoot != null;
                bool processSubtree = enumerator.Current.BuildNodeBoxLayout(enumerator.IsStart, preformattedWhitespace);
                if (!processSubtree)
                {
                    // Note that after this call the current element won't be visited again while normally each element is
                    // visited twice.
                    enumerator.DontEnumerateCurrentSubTree();
                }

                if ((!enumerator.IsStart || !processSubtree) &&
                    ReferenceEquals(enumerator.Current, preformattedWhitespaceSubtreeRoot))
                {
                    // We're leaving a preformatted whitespace subtree.
                    preformattedWhitespaceSubtreeRoot = null;
                }
            }
        }

        /// <summary>
        /// Serializes the node to XML text.
        /// </summary>
        /// <param name="result">
        /// The text buffer to which the XML text of this node will be appended.
        /// </param>
        /// <param name="serializationContext">Serialization context.</param>
        internal abstract void SerializeToXml(StringBuilder result, XmlSerializationContext serializationContext);

        /// <summary>
        /// Gets the parent node of this node. May be <c>null</c>. Do not set this property directly.
        /// </summary>
        /// <remarks>
        /// This property should not be changed directly. Use parent's property <see cref="HtmlElementNode.Children"/>
        /// to change position of the node in an HTML tree.
        /// </remarks>
        internal HtmlElementNode Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        /// <summary>
        /// Gets the sibling node that follows this node. May be <c>null</c>. Do not set this property directly.
        /// </summary>
        /// <remarks>
        /// This property should not be changed directly. Use parent's property <see cref="HtmlElementNode.Children"/>
        /// to change position of the node in an HTML tree.
        /// </remarks>
        internal HtmlNode NextSibling
        {
            get { return mNextSibling; }
            set { mNextSibling = value; }
        }

        /// <summary>
        /// Gets the sibling node that precedes this node. May be <c>null</c>. Do not set this property directly.
        /// </summary>
        /// <remarks>
        /// This property should not be changed directly. Use parent's property <see cref="HtmlElementNode.Children"/>
        /// to change position of the node in an HTML tree.
        /// </remarks>
        internal HtmlNode PreviousSibling
        {
            get { return mPreviousSibling; }
            set { mPreviousSibling = value; }
        }

        /// <summary>
        /// Indicates whether this node or any of its child nodes is a box.
        /// </summary>
        internal abstract bool IsBoxOrContainsBoxes();

        /// <summary>
        /// Indicates whether this node or any of its child nodes contains meaningful text (text that contains non-ignored
        /// characters). In most cases, any character except whitespace is meaningful, but in preformatted text whitespace
        /// characters are meaningful too.
        /// </summary>
        internal abstract bool ContainsText();

        /// <summary>
        /// Builds box layout of the node.
        /// </summary>
        /// <returns>
        /// A value indicating whether the subtree of the node must also be processed.
        /// </returns>
        protected abstract bool BuildNodeBoxLayout(bool isStart, bool preformattedWhitespace);

        /// <summary>
        /// Gets a value indicating whether whitespace of this node and of all its children is preformatted.
        /// </summary>
        protected virtual bool IsPreformattedWhitespaceNode()
        {
            // Whitespace of most nodes is not preformatted by default.
            return false;
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private HtmlElementNode mParent;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private HtmlNode mPreviousSibling;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private HtmlNode mNextSibling;
    }
}
