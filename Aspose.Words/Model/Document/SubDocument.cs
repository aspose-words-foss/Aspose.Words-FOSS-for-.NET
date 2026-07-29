// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/02/2012 by Alexey Morozov

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a <b>SubDocument</b> - which is a reference to an externally stored document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>In this version of Aspose.Words, <see cref="SubDocument"/> nodes do not provide public methods
    /// and properties to create or modify a subdocument. In this version you are not able to instantiate
    /// <see cref="SubDocument"/> nodes or modify existing except deleting them.</para>
    /// 
    /// <p><see cref="SubDocument"/> can only be a child of <see cref="Paragraph"/>.</p>
    /// </remarks>
    public class SubDocument : Node
    {
        /// <summary>
        /// no public ctor.
        /// </summary>
        /// AM. I briefly tested insertion of SubDocuments into different containers such as paragraph, table or custom xml markup and 
        /// found that SubDocument is not "real" inline. For example, it can not be child of table cell paragraph. Word accepts it but move out 
        /// subDocument out of table and so on. Since SubDocument manipulation is not available for customers it's OK now.
        /// I raised separate task WORDSNET-6087 that must be completed before this ctor is made public.
        internal SubDocument(DocumentBase doc, string fileName)
            : base(doc)
        {
            FileName = fileName;
        }

        /// <summary>
        /// Gets/sets filename of this SubDocument node.
        /// <para>
        /// <b>Note</b>It is user responsibility to check that filename is valid or 
        /// referred document can be accessed, this filename is written as is.
        /// </para>
        /// <para>Can not be null or empty string.</para>
        /// </summary>
        internal string FileName
        {
            get { return mFileName; }
            set
            {
                ArgumentUtil.CheckHasChars(value, "fileName");
                mFileName = value;
            }
        }

        /// <summary>
        /// Returns <see cref="NodeType.SubDocument"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.SubDocument; }
        }


        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        public override bool Accept(DocumentVisitor visitor)
        {
            return VisitorActionToBool(visitor.VisitSubDocument(this));
        }

        private string mFileName;

#if DEBUG
        public override string ToString()
        {
            return String.Format("{0} '{1}'", base.ToString(), mFileName);
        }
#endif
    }
}
