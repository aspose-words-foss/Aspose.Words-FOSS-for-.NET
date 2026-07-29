// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2010 by Alexey Titov

using System;

namespace Aspose.Words.Drawing.Core.Dml.Guides
{
    internal class DmlOneArgumentFormula : DmlFormula
    {
        internal DmlOneArgumentFormula(string[] formulaParts, IDmlOneArgumentFormulaCallback callback)
        {
            if (formulaParts.Length < 2)
                throw new ArgumentOutOfRangeException("formulaParts", "Wrong number of formula parts.");

            mX = formulaParts[1];
            mCallback = callback;
        }

        internal DmlOneArgumentFormula(string[] formulaParts, IDmlOneArgumentFormulaCallback callback, string source)
            : this(formulaParts, callback)
        {
            Source = source;
        }

        /// <summary>
        /// Calculates the formula.
        /// </summary>
        public override double Calculate(IDmlGuideValueProvider guideValueProvider)
        {
            double x = DmlFormulaHelper.GetValue(X, guideValueProvider);
            return mCallback.Calculate(x);
        }

        internal string X
        {
            get { return mX; }
            set { mX = value; }
        }

        private readonly IDmlOneArgumentFormulaCallback mCallback;
        private string mX;
    }
}