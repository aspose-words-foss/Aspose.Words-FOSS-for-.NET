// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/03/2011 by Denis Darkin
using System;
using Aspose.Collections;
using Aspose.Words.Math;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Performs validation of Office Math element m:m (matrix element) by:
    /// - making number of columns in <see cref="MathMatrixColumnPrCollection"/> equal to max number of row elements.
    /// - adding empty Arguments to the rows that contain less than max number of elements, thus making
    ///   each row to contain equal number of arguments.
    /// </summary>
    internal class MathObjectMatrixValidator
    {
        internal MathObjectMatrixValidator(bool reconstructMissingArguments, DocumentBase doc)
        {
            mDoc = doc;
            mReconstructMissingArguments = reconstructMissingArguments;
            if (mReconstructMissingArguments)
                mMatrixRowsCache = new ObjToIntDictionary<OfficeMath>();
        }
        
        internal static bool NeedToValidateNode(OfficeMath officeMath)
        {
            MathObjectType type = officeMath.MathObject.MathObjectType;
            return ((type == MathObjectType.MatrixRow) || (type == MathObjectType.Argument));
        }

        internal VisitorAction VisitOfficeMathStart(OfficeMath officeMath)
        {
            switch (mStatus)
            {
                case ValidatorStatus.InsideMatrix:
                    {
                        if (officeMath.MathObject.MathObjectType == MathObjectType.MatrixRow)
                        {
                            mStatus = ValidatorStatus.InsideRow;
                            mColumnCount = 0;
                        }
                        break;
                    }
                case ValidatorStatus.InsideRow:
                    {
                        if (officeMath.MathObject.MathObjectType == MathObjectType.Argument)
                        {
                            mColumnCount++;
                            mArgumentCounter = 1; // first argument belongs to matrix cell itself.
                            mStatus = ValidatorStatus.InsideCell;
                        }
                        break;
                    }
                case ValidatorStatus.InsideCell:
                        {
                            if (officeMath.MathObject.MathObjectType == MathObjectType.Argument)
                            {
                                // while parsing throught the cell content, we've found one more nested argument
                                mArgumentCounter++;
                            }
                            break;
                        }
                default:
                    {
                        throw new InvalidOperationException("Please report exception");
                    }        
            }
            
            return VisitorAction.Continue;
        }

        internal VisitorAction VisitOfficeMathEnd(OfficeMath officeMath)
        {
            switch (mStatus)
            {
                case ValidatorStatus.InsideMatrix:
                    {                
                        // processed all rows, leaving cur nested matrix.
                        if (officeMath.MathObject.MathObjectType == MathObjectType.Matrix)
                        {
                            // now when we know number of columns per se, pass it to ColumnPrCollection for validation.
                            ((MathObjectMatrix)officeMath.MathObject).ColumnPrCollection.ValidateCollection(mMaxColumns);

                            if (mReconstructMissingArguments)
                                CreateMissingElements();
                        }
                        break;
                    }
                case ValidatorStatus.InsideRow:
                    {
                        if (officeMath.MathObject.MathObjectType == MathObjectType.MatrixRow)
                        {
                            mStatus = ValidatorStatus.InsideMatrix;
                            if (mMaxColumns < mColumnCount)
                                mMaxColumns = mColumnCount;

                            if (mReconstructMissingArguments)
                                mMatrixRowsCache.Add(officeMath, mColumnCount);
                        }
                        break;
                    }
                case ValidatorStatus.InsideCell:
                    {
                        if (officeMath.MathObject.MathObjectType == MathObjectType.Argument)
                        {
                            mArgumentCounter--; // we've returned from nested level of one more argument.
                            
                            if (mArgumentCounter == 0) // means we have come to the original argument, denoting the cur cell.
                                mStatus = ValidatorStatus.InsideRow;
                        }
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException("Please report exception");
                    }        
            }
             
            return VisitorAction.Continue;
        }
        
        /// <summary>
        /// Populate rows with empty arguments if rows have too few elements.
        /// </summary>
        private void CreateMissingElements()
        {
            ObjToIntDictionary<OfficeMath>.Enumerator enumerator = mMatrixRowsCache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                OfficeMath officeMath = enumerator.CurrentKey;
                int columnsToAdd = mMaxColumns - enumerator.CurrentValue;

                for (int iColumn = 0; iColumn < columnsToAdd; iColumn++)
                {
                    Debug.Assert(officeMath.MathObject.MathObjectType == MathObjectType.MatrixRow);
                    
                    // mimic MS Word behavior, which prepends empty arguments in case if matrix has rows with fewer els than needed.
                    officeMath.PrependChild(new OfficeMath(mDoc, new MathObjectArgumentBase(MathObjectType.Argument)));
                }
            }
        }
        
        private enum ValidatorStatus
        {
            InsideMatrix,
            
            InsideRow,
            
            InsideCell
         }
        
        private ValidatorStatus mStatus = ValidatorStatus.InsideMatrix;
        
        /// <summary>
        /// Count of columns for the current row
        /// </summary>
        private int mColumnCount;

        /// <summary>
        /// We can have multiple arguments inside a matrix cell, and only topmost one means that we on cell level. So whenever
        /// cell contains complex content with multiple n of arguments, we keep track of their nestedness by using this counter.
        /// </summary>
        private int mArgumentCounter;
        
        /// <summary>
        /// Number of columns in matrix. We are assuming that every row can contain variable n of elements, so we use max n.
        /// </summary>
        private int mMaxColumns;

        /// <summary>
        /// If set to true, then this validator will insert empty arguments to the rows that have number of argument smaller than
        /// maximum number of columns across the table. This means that all rows of the table will containt the same n of elements.
        /// </summary>
        private readonly bool mReconstructMissingArguments;

        /// <summary>
        /// Every node that we pass will be stored here along with number of elements it contains.
        /// </summary>
        private readonly ObjToIntDictionary<OfficeMath> mMatrixRowsCache;

        /// <summary>
        /// will need this when we insert empty arguments into the matrix.
        /// </summary>
        private readonly DocumentBase mDoc;
    }
}
