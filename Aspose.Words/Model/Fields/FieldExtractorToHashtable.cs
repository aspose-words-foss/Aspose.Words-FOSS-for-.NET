// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2008 by Roman Korchagin

using System;
using System.Collections.Generic;


namespace Aspose.Words.Fields
{
    /// <summary>
    /// Extracts fields from a given node into a dictionary.
    /// </summary>
    internal class FieldExtractorToHashtable : FieldExtractor
    {
        internal FieldExtractorToHashtable() : this(NodeType.FieldStart)
        {
        }

        /// <summary>
        /// Creates dictionary of fields where key is either FieldStart or FieldSeparator or FieldEnd.
        /// </summary>
        /// <remarks>
        /// AM. I need such type of collection during RTF/DOC export to write HFD structures
        /// This structure is written with field separator.
        /// </remarks>
        internal FieldExtractorToHashtable(NodeType type)
        {
            Debug.Assert((type == NodeType.FieldStart) || (type == NodeType.FieldSeparator) || (type == NodeType.FieldEnd));

            mType = type;
        }

        /// <summary>
        /// Access to the result of the extraction.
        /// Key is <see cref="FieldStart"/> and value is <see cref="Field"/>.
        /// </summary>
        internal IDictionary<Node, Field> Fields
        {
            get { return mFields; }
        }

        protected override void OnFieldExtracted()
        {
            switch (mType)
            {
                case NodeType.FieldStart:
                    mFields[CurrentField.Start] = CurrentField;
                    break;
                case NodeType.FieldSeparator:
                    // Some fields might not have separator. Do not add such field to table.
                    if (CurrentField.Separator != null)
                        mFields[CurrentField.Separator] = CurrentField;
                    break;
                case NodeType.FieldEnd:
                    mFields[CurrentField.End] = CurrentField;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private readonly NodeType mType;
        private readonly Dictionary<Node, Field> mFields = new Dictionary<Node, Field>();
    }
}
