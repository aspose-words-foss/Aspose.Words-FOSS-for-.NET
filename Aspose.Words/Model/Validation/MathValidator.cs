// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/03/2011 by Denis Darkin

using System.Collections.Generic;
using Aspose.Words.Math;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Encapsulate specifics of Office Math validation and prepare reuse for consumer classes: 
    /// <see cref="DocumentValidator"/> and <see cref="DocumentPostLoader"/>
    /// </summary>
    internal class MathValidator
    {
        internal MathValidator(DocumentBase doc)
        {
            mDoc = doc;
        }
        
        internal VisitorAction VisitOfficeMathStart(OfficeMath officeMath)
        {
            if (officeMath.MathObject.MathObjectType == MathObjectType.Matrix)
            {
                // found one more matrix, create a validator for it.
                mMatrixValidatorStack.Push(new MathObjectMatrixValidator(true, mDoc));
            }
            else if ((mMatrixValidatorStack.Count != 0) && MathObjectMatrixValidator.NeedToValidateNode(officeMath))
            {
                // Start processing rows and arguments(children of row)
                mMatrixValidatorStack.Peek().VisitOfficeMathStart(officeMath);
            }

            return VisitorAction.Continue;
        }
        
        internal VisitorAction VisitOfficeMathEnd(OfficeMath officeMath)
        {
            if (officeMath.MathObject.MathObjectType == MathObjectType.Matrix)
            {
                // we are leaving current nested matrix, remove validator from stack.
                mMatrixValidatorStack.Pop().VisitOfficeMathEnd(officeMath);
            }
            else if ((mMatrixValidatorStack.Count != 0) && MathObjectMatrixValidator.NeedToValidateNode(officeMath))
            {
                //Finish processing rows and arguments(children of row)
                mMatrixValidatorStack.Peek().VisitOfficeMathEnd(officeMath);
            }
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Stores reference to validated document.
        /// </summary>
        private readonly DocumentBase mDoc;

        /// <summary>
        /// Process math objects m:m (matrices) to perform validation.
        /// Make sure number of columns in matrixPr is equal to number of elements in each matrix row.
        /// Since matrices can be nested, we need a stack of validators, each instance processes one level of matrix nesting.
        /// </summary>
        private readonly Stack<MathObjectMatrixValidator> mMatrixValidatorStack = new Stack<MathObjectMatrixValidator>();
    }
}
