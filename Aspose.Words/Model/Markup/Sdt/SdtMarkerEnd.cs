// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/08/2021 by Alexey Morozov

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Internal node used during comparison only.
    /// </summary>
    internal class SdtMarkerEnd : Node
    {
        internal SdtMarkerEnd(DocumentBase doc, int id, MarkupLevel level)
            : base(doc)
        {
            Id = id;
            Level = level;
        }

        internal readonly MarkupLevel Level;
        internal int Id;

        public override NodeType NodeType
        {
            get { return NodeType.System; }
        }

        public override bool Accept(DocumentVisitor visitor)
        {
            return true;
        }
    }
}
