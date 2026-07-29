// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/03/2011 by Denis Darkin

using System.Collections.Generic;

namespace Aspose.Words.Math
{
    /// <summary>
    /// In Office Math a matrix object has collection of column properties.
    /// This collection provide access to individual properties of each column <see cref="MathMatrixColumnPr"/>.
    /// </summary>
    internal class MathMatrixColumnPrCollection
    {
        internal MathMatrixColumnPrCollection Clone()
        {
            MathMatrixColumnPrCollection lhs = new MathMatrixColumnPrCollection();    
            
            for (int i = 0; i < Count; i++)
                lhs.Add(this[i].Clone());
            
            return lhs;
        }
        
        internal void Add(MathMatrixColumnPr matrixColumnPr)
        {
            Add(matrixColumnPr, 1);
        }
        
        internal void Add(MathMatrixColumnPr matrixColumnPr, int count)
        {
            for (int i = 0; i < count; i++)
                mChildren.Add(matrixColumnPr.Clone());
        }
        
        internal void RemoveAt(int index)
        {
            mChildren.RemoveAt(index);
        }
        
        internal void RemoveFromTheEnd()
        {
            RemoveAt(Count - 1);
        }
        
        /// <summary>
        /// Checks that number of columns (<see cref="Count"/>) in this collection 
        /// matches the number passed as nColumns parameter.
        /// - If the number is greater than parameter, then removes some of the columnPr items starting from the end. 
        /// - If the number is smaller than parameter, then populates collection with some default columnPr, starting from the end.
        /// </summary>
        internal void ValidateCollection(int nColumns)
        {
            int difference = Count - nColumns;
            if (difference > 0)
            {
                for (int i = 0; i < difference; i++)
                    RemoveFromTheEnd();
            }
            else if (difference < 0)
            {
                for (int i = difference; i < 0; i++)
                    Add(new MathMatrixColumnPr());
            }
        }
        
        internal MathMatrixColumnPr this[int i]
        {
            get { return mChildren[i]; }
        }
        
        internal int Count
        {
            get { return mChildren.Count; }
        }
        
        private readonly List<MathMatrixColumnPr> mChildren = new List<MathMatrixColumnPr>();
    }
}
