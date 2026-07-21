// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/12/2003 by Roman Korchagin

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using Aspose.Words.Replacing;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a contiguous area in a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-ranges/">Working with Ranges</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>The document is represented by a tree of nodes and the nodes provide operations
    /// to work with the tree, but some operations are easier to perform if the document
    /// is treated as a contiguous sequence of text.</p>
    ///
    /// <p><see cref="Range"/> is a "facade" interface that provide methods that treat the document
    /// or portions of the document as "flat" text regardless of the fact that the document
    /// nodes are stored in a tree-like object model.</p>
    ///
    /// <p><see cref="Range"/> does not contain any text or nodes, it is merely a view or "window"
    /// over a fragment of a document.</p>
    ///
    /// </remarks>
    public class Range : IEnumerable<Node>
    {
        /// <summary>
        /// Currently range attaches to a single node, but in the future it would be nice
        /// to allow ranges for any arbitrary fragment of a document.
        /// </summary>
        internal Range(Node node)
        {
            mNode = node;
        }

        /// <summary>
        /// Gets the text of the range.
        /// </summary>
        /// <remarks>
        /// <p>The returned string includes all control and special characters as described in <see cref="ControlChar"/>.</p>
        /// </remarks>
        public string Text
        {
            get { return mNode.GetText(); }
        }

        /// <summary>
        /// Returns a <see cref="FormFields"/> collection that represents all form fields in the range.
        /// </summary>
        public FormFieldCollection FormFields
        {
            get
            {
                if (mFormFields == null)
                    mFormFields = new FormFieldCollection(mNode);

                return mFormFields;
            }
        }

        /// <summary>
        /// Returns a <see cref="Bookmarks"/> collection that represents all bookmarks in the range.
        /// </summary>
        public BookmarkCollection Bookmarks
        {
            get
            {
                if (mBookmarks == null)
                    mBookmarks = new BookmarkCollection(mNode);

                return mBookmarks;
            }
        }

        /// <summary>
        /// Returns a <see cref="Fields"/> collection that represents all fields in the range.
        /// </summary>
        public FieldCollection Fields
        {
            get
            {
                if (mFields == null)
                    mFields = new FieldCollection(mNode);

                return mFields;
            }
        }

        /// <summary>
        /// Returns a <see cref="StructuredDocumentTags"/> collection that represents all structured document tags in the range.
        /// </summary>
        public StructuredDocumentTagCollection StructuredDocumentTags
        {
            get
            {
                if (mStructuredDocumentTags == null)
                    mStructuredDocumentTags = new StructuredDocumentTagCollection(mNode);

                return mStructuredDocumentTags;
            }
        }

        /// <summary>
        /// Deletes all characters of the range.
        /// </summary>
        public void Delete()
        {
            // We do want to delete all content from the range.
            if (mNode.IsComposite)
                ((CompositeNode)mNode).RemoveAllChildren();

            // If this is not a detached node or document, we do want to remove the node itself from the tree.
            if (mNode.ParentNode != null)
                mNode.ParentNode.RemoveChild(mNode);
        }

        /// <summary>
        /// Replaces all occurrences of a specified character string pattern with a replacement string.
        /// </summary>
        /// <remarks>
        /// <p>The pattern will not be used as regular expression.
        /// Please use <see cref="Replace(Regex, string)"/> if you need regular expressions.</p>
        /// <p>Used case-insensitive comparison.</p>
        /// <p>Method is able to process breaks in both pattern and replacement strings.</p>
        /// You should use special meta-characters if you need to work with breaks:
        /// <list type="bullet">
        /// <item><b>&amp;p</b> - paragraph break</item>
        /// <item><b>&amp;b</b> - section break</item>
        /// <item><b>&amp;m</b> - page break</item>
        /// <item><b>&amp;l</b> - manual line break</item>
        /// </list>
        /// Use method <see cref="Range.Replace(string, string, FindReplaceOptions)"/> to have more flexible customization.
        /// </remarks>
        /// <param name="pattern">A string to be replaced.</param>
        /// <param name="replacement">A string to replace all occurrences of pattern.</param>
        /// <returns>The number of replacements made.</returns>
        /// <example>
        /// <code>
        /// Document doc = new Document();
        /// DocumentBuilder builder = new DocumentBuilder(doc);
        /// builder.Writeln("Numbers 1, 2, 3");
        ///
        /// // Inserts paragraph break after Numbers.
        /// doc.Range.Replace("Numbers", "Numbers&amp;p", new FindReplaceOptions());
        /// </code>
        /// </example>
        public int Replace(string pattern, string replacement)
        {
            return Replace(pattern, replacement, new FindReplaceOptions());
        }

        /// <summary>
        /// Replaces all occurrences of a character pattern specified by a regular expression with another string.
        /// </summary>
        /// <remarks>
        /// <p>Replaces the whole match captured by the regular expression.</p>
        /// <p>Method is able to process breaks in both pattern and replacement strings.</p>
        /// You should use special meta-characters if you need to work with breaks:
        /// <list type="bullet">
        /// <item><b>&amp;p</b> - paragraph break</item>
        /// <item><b>&amp;b</b> - section break</item>
        /// <item><b>&amp;m</b> - page break</item>
        /// <item><b>&amp;l</b> - manual line break</item>
        /// </list>
        /// Use method <see cref="Range.Replace(Regex, string, FindReplaceOptions)"/> to have more flexible customization.
        /// </remarks>
        /// <param name="pattern">A regular expression pattern used to find matches.</param>
        /// <param name="replacement">A string to replace all occurrences of pattern.</param>
        /// <returns>The number of replacements made.</returns>
        /// <example>
        /// <code>
        /// Document doc = new Document();
        /// DocumentBuilder builder = new DocumentBuilder(doc);
        /// builder.Writeln("a1, b2, c3");
        ///
        /// // Replaces each number with paragraph break.
        /// doc.Range.Replace(new Regex(@"\d+"), "&amp;p");
        /// </code>
        /// </example>
        public int Replace(Regex pattern, string replacement)
        {
            return Replace(pattern, replacement, new FindReplaceOptions());
        }

        /// <summary>
        /// Replaces all occurrences of a specified character string pattern with a replacement string.
        /// </summary>
        /// <remarks>
        /// <p>The pattern will not be used as regular expression.
        /// Please use <see cref="Replace(Regex, string, FindReplaceOptions)"/> if you need regular expressions.</p>
        /// <p>Method is able to process breaks in both pattern and replacement strings.</p>
        /// You should use special meta-characters if you need to work with breaks:
        /// <list type="bullet">
        /// <item><b>&amp;p</b> - paragraph break</item>
        /// <item><b>&amp;b</b> - section break</item>
        /// <item><b>&amp;m</b> - page break</item>
        /// <item><b>&amp;l</b> - manual line break</item>
        /// <item><b>&amp;&amp;</b> - &amp; character</item>
        /// </list>
        /// </remarks>
        /// <param name="pattern">A string to be replaced.</param>
        /// <param name="replacement">A string to replace all occurrences of pattern.</param>
        /// <param name="options"><see cref="FindReplaceOptions"/> object to specify additional options.</param>
        /// <returns>The number of replacements made.</returns>
        /// <example>
        /// <code>
        /// Document doc = new Document();
        /// DocumentBuilder builder = new DocumentBuilder(doc);
        /// builder.Writeln("Numbers 1, 2, 3");
        ///
        /// // Inserts paragraph break after Numbers.
        /// doc.Range.Replace("Numbers", "Numbers&amp;p", new FindReplaceOptions());
        /// </code>
        /// </example>
        public int Replace(string pattern, string replacement, FindReplaceOptions options)
        {
            if (options.LegacyMode)
            {
                FindReplaceLegacy fr = new FindReplaceLegacy(mNode, pattern, replacement, options.MatchCase, options.FindWholeWordsOnly);
                return fr.Replace();
            }
            else
            {
                FindReplace fr = new FindReplace(mNode, pattern, replacement, options);
                return fr.Replace();
            }
        }

        /// <summary>
        /// Replaces all occurrences of a character pattern specified by a regular expression with another string.
        /// </summary>
        /// <remarks>
        /// <p>Replaces the whole match captured by the regular expression.</p>
        /// <p>Method is able to process breaks in both pattern and replacement strings.</p>
        /// You should use special meta-characters if you need to work with breaks:
        /// <list type="bullet">
        /// <item><b>&amp;p</b> - paragraph break</item>
        /// <item><b>&amp;b</b> - section break</item>
        /// <item><b>&amp;m</b> - page break</item>
        /// <item><b>&amp;l</b> - manual line break</item>
        /// <item><b>&amp;&amp;</b> - &amp; character</item>
        /// </list>
        /// </remarks>
        /// <param name="pattern">A regular expression pattern used to find matches.</param>
        /// <param name="replacement">A string to replace all occurrences of pattern.</param>
        /// <param name="options"><see cref="FindReplaceOptions"/> object to specify additional options.</param>
        /// <returns>The number of replacements made.</returns>
        /// <example>
        /// <code>
        /// Document doc = new Document();
        /// DocumentBuilder builder = new DocumentBuilder(doc);
        /// builder.Writeln("a1, b2, c3");
        ///
        /// // Replaces each number with paragraph break.
        /// doc.Range.Replace(new Regex(@"\d+"), "&amp;p", new FindReplaceOptions());
        /// </code>
        /// </example>
        public int Replace(Regex pattern, string replacement, FindReplaceOptions options)
        {
            if (options.LegacyMode)
            {
                FindReplaceLegacy fr = new FindReplaceLegacy(mNode, pattern, replacement, options.ReplacingCallback,
                    (options.Direction == FindReplaceDirection.Forward));
                return fr.Replace();
            }
            else
            {
                FindReplace fr = new FindReplace(mNode, pattern, replacement, options);
                return fr.Replace();
            }
        }

        /// <summary>
        /// Updates the values of document fields in this range.
        /// </summary>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="UpdateFields"]/*'/>
        ///
        /// <para>To update fields in the whole document use <see cref="Document.UpdateFields"/>.</para>
        /// </remarks>
        public void UpdateFields()
        {
            FieldUpdater.UpdateFields(mNode);
        }

        /// <summary>
        /// Unlinks fields in this range.
        /// </summary>
        /// <remarks>
        /// <para>Replaces all the fields in this range with their most recent results.</para>
        /// <para>To unlink fields in the whole document use <see cref="Range.UnlinkFields"/>.</para>
        /// </remarks>
        public void UnlinkFields()
        {
            FieldUnlinker.UnlinkFields(mNode);
        }

        /// <summary>
        /// Changes field type values <see cref="FieldChar.FieldType"/> of <see cref="FieldStart"/>, <see cref="FieldSeparator"/>, <see cref="FieldEnd"/>
        /// in this range so that they correspond to the field types contained in the field codes.
        /// </summary>
        /// <remarks>
        /// <para>Use this method after document changes that affect field types.</para>
        /// <para>To change field type values in the whole document use <see cref="Document.NormalizeFieldTypes"/>.</para>
        /// </remarks>
        public void NormalizeFieldTypes()
        {
            IList<Field> fieldCollection = FieldExtractor.ExtractToCollection(mNode);

            foreach (Field field in fieldCollection)
              field.DetermineFieldType();
        }

        /// <summary>
        /// Constructs a new fully formed document that contains the range.
        /// </summary>
        public Document ToDocument()
        {
            return RangeDocumentBuilder.Build(this);
        }

        public IEnumerator<Node> GetEnumerator()
        {
            if (mNode.NodeType == NodeType.StructuredDocumentTagRangeStart)
            {
                NodeCollection childNodes = ((StructuredDocumentTagRangeStart)mNode).ChildNodes;

                return new RangeEnumerator(new NodeRange(childNodes[0], childNodes[childNodes.Count - 1]));
            }

            if (!mNode.IsComposite)
                return new RangeEnumerator(new NodeRange(mNode, mNode));

            if (mNode.NodeType == NodeType.Section)
                return new RangeEnumerator(new NodeRange(((Section)mNode).Body, ((Section)mNode).Body.LastChild));

            return new RangeEnumerator(new NodeRange(((CompositeNode)mNode).FirstChild, ((CompositeNode)mNode).LastChild));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Node that is attached to the range.
        /// Currently range attaches to a single node, but in the future it would be nice
        /// to allow ranges for any arbitrary fragment of a document.
        /// </summary>
        internal Node Node
        {
            get { return mNode; }
        }

        /// <summary>
        /// Gets a collection of revisions (tracked changes) that exist in this range.
        /// </summary>
        /// <remarks>
        /// <para>The returned collection is a "live" collection, which means if you remove parts of a document that contain
        /// revisions, the deleted revisions will automatically disappear from this collection.</para>
        /// </remarks>
        public RevisionCollection Revisions
        {
            get
            {
                if (mRevisionsCache == null)
                    mRevisionsCache = new RevisionCollection(mNode);

                return mRevisionsCache;
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Node mNode;
        private FormFieldCollection mFormFields;
        private BookmarkCollection mBookmarks;
        private FieldCollection mFields;
        private StructuredDocumentTagCollection mStructuredDocumentTags;
        private RevisionCollection mRevisionsCache;
    }
}
