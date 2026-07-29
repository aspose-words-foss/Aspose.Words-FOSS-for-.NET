// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2005 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Special node that is used to make the owner document accessible while the parent of a node is null.
    /// </summary>
    internal class NullNode : Node
    {
        internal NullNode(DocumentBase doc)
        {
            //The parent of the NullNode is the document directly.
            SetParent(doc);
        }

        public override NodeType NodeType
        {
            get { return NodeType.Null; }
        }

        public override string GetText()
        {
            return string.Empty;
        }

        public override bool Accept(DocumentVisitor visitor)
        {
            return true;
        }
    }
}
