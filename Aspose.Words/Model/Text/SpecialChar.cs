// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/05/2005 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Base class for special characters in the document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>A Microsoft Word document can include a number of special characters
    /// that represent fields, form fields, shapes, OLE objects, footnotes etc. For the list
    /// of special characters see <see cref="ControlChar"/>.</p>
    ///
    /// <p><see cref="SpecialChar"/> is an inline-node and can only be a child of <see cref="Paragraph"/>.</p>
    ///
    /// <p><see cref="SpecialChar"/> char is used as a base class for more specific classes
    /// that represent special characters that Aspose.Words provides programmatic access for.
    /// The <see cref="SpecialChar"/> class is also used itself to represent special character for which
    /// Aspose.Words does not provide detailed programmatic access.</p>
    /// </remarks>
    public class SpecialChar : Inline
    {
        internal SpecialChar(DocumentBase doc, char ch, RunPr runPr) : base(doc, runPr)
        {
            Char = ch;
            mText = ch.ToString();
        }

        /// <summary>
        /// Returns <see cref="NodeType.SpecialChar"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.SpecialChar; }
        }

        /// <summary>
        /// Gets the special character that this node represents.
        /// </summary>
        internal char Char { get; }

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <remarks>
        /// <p>Calls <see cref="DocumentVisitor.VisitSpecialChar"/>.</p>
        /// <p>For more info see the Visitor design pattern.</p>
        /// </remarks>
        /// <param name="visitor">The visitor that will visit the node.</param>
        /// <returns><c>false</c> if the visitor requested the enumeration to stop.</returns>
        public override bool Accept(DocumentVisitor visitor)
        {
            return VisitorActionToBool(visitor.VisitSpecialChar(this));
        }

        /// <summary>
        /// Gets the special character that this node represents.
        /// </summary>
        /// <returns>The string that contains the character that this node represents.</returns>
        public override string GetText()
        {
            return mText;
        }

        private readonly string mText;
    }
}
