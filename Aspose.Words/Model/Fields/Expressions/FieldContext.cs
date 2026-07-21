// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2015 by Edward Voronov

using Aspose.Words.Tables;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Passes data between the parts of the evaluation module.
    /// </summary>
    internal class FieldContext
    {
        internal FieldContext(Field field)
        {
            Field = field;
        }

        internal bool DocumentContainsBookmark(string bookmarkName)
        {
            // SPEED Get a bookmark from a cache.
            return FieldUtil.GetCachedBookmark(Field, bookmarkName) != null;
        }

        /// <summary>
        /// Gets the field that owns the expression.
        /// </summary>
        internal Field Field { get; }

        /// <summary>
        /// Gets a parent table if the field is in a table, null otherwise.
        /// </summary>
        internal Table ParentTable
        {
            get { return (ParentCell != null) ? ParentCell.ParentTable : null; }
        }

        /// <summary>
        /// Gets a parent cell if the field is in a table, null otherwise.
        /// </summary>
        internal Cell ParentCell
        {
            get { return (Cell)Field.Start.GetAncestor(NodeType.Cell); }
        }
    }
}