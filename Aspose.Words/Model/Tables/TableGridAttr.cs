// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/01/2018 by Dmitry Matveenko

using Aspose.Collections;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Represents the result of grid calculation and implements <see cref="IComplexAttr"/>.
    /// </summary>
    internal class TableGridAttr: IComplexAttr
    {
        internal TableGridAttr()
        {
            // Empty column list.
            Columns = new IntList();
        }

        internal TableGridAttr(IntList columnWidths)
        {
            Columns = columnWidths;
        }

        /// <summary>
        /// Represents different states during grid calculation.
        /// </summary>
        /// <remarks>
        /// The grid may be calculated but not applied.
        /// It is used for testing.
        /// </remarks>
        internal GridState State
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return mState; }
            set { mState = value; }
        }

        /// <summary>
        /// Gets a boolean value indicating if the given grid is not null and has Applied state.
        /// </summary>
        internal static bool IsApplied(TableGridAttr grid)
        {
            return (grid != null) && (grid.State == GridState.Applied);
        }

        // The method is used by the layout engine only.
        internal int[] ToLisArray()
        {
            int[] columns = new int[Columns.Count];

            for (int i = 0; i < Columns.Count; ++i)
            {
                int twips = Columns[i];
                columns[i] = ConvertUtilCore.TwipToLi(twips);
            }

            return columns;
        }

        bool IComplexAttr.IsInheritedComplexAttr
        {
            get { return false; }
        }

        IComplexAttr IComplexAttr.DeepCloneComplexAttr()
        {
            IntList clonedColumns = new IntList(Columns.Count);
            clonedColumns.AddRange(Columns);

            TableGridAttr clonedGrid = new TableGridAttr(clonedColumns);

            clonedGrid.State = State;

            return clonedGrid;
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
            if (obj.GetType() != typeof(TableGridAttr))
                return false;

            TableGridAttr otherGrid = (TableGridAttr)obj;
            return Columns.Equals(otherGrid.Columns) && (State == otherGrid.State);
        }

        public override int GetHashCode()
        {
            // It is not actually used. It is overridden because Equals() is overridden.
            // alexnosk: modified for C++ porting.
            int hash = ((int)State).GetHashCode();
            for (int i = 0; i < mColumns.Count; i++)
                hash ^= mColumns[i].GetHashCode();

            return hash;
        }

        /// <summary>
        /// A collection of grid column widths.
        /// </summary>
        internal IntList Columns
        {
            get { return mColumns; }
            set { mColumns = value; }
        }

        private IntList mColumns;
        private GridState mState;
    }
}
