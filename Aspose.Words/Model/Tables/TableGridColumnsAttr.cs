// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/01/2018 by Dmitry Matveenko

using System;
using Aspose.Collections;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// This is a wrapper for storing a list of table grid column widths in a table attribute that implements <see cref="IComplexAttr"/>
    /// </summary>
    internal class TableGridColumnsAttr: IComplexAttr
    {
        internal TableGridColumnsAttr(IntList columnWidths)
        {
            if (columnWidths == null)
                throw new ArgumentNullException("columnWidths");

            GridColumns = columnWidths;
        }

        bool IComplexAttr.IsInheritedComplexAttr
        {
            get { return false; }
        }

        IComplexAttr IComplexAttr.DeepCloneComplexAttr()
        {
            IntList clonedList = new IntList(GridColumns.Count);
            clonedList.AddRange(GridColumns);
            return new TableGridColumnsAttr(clonedList);
        }

        /// <summary>
        /// Determines whether the specified object is equal in value to the current object.
        /// </summary>
        /// <remarks>
        /// It is used for testing.
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(TableGridColumnsAttr))
                return false;

            return GridColumns.Equals(((TableGridColumnsAttr)obj).GridColumns);
        }

        public override int GetHashCode()
        {
            return GridColumns.GetHashCode();
        }

        internal readonly IntList GridColumns;
    }
}
