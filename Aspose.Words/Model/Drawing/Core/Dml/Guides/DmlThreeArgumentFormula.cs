// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2010 by Alexey Titov

using System;

namespace Aspose.Words.Drawing.Core.Dml.Guides
{
    internal class DmlThreeArgumentFormula : DmlFormula
    {
        internal DmlThreeArgumentFormula(string[] formulaParts, IDmlThreeArgumentFormulaCallback callback)
        {
            if (formulaParts.Length < 4)
                throw new ArgumentOutOfRangeException("formulaParts", "Wrong number of formula parts.");

            mX = formulaParts[1];
            mY = formulaParts[2];
            mZ = formulaParts[3];
            mCallback = callback;
        }
                
        internal DmlThreeArgumentFormula(string[] formulaParts, IDmlThreeArgumentFormulaCallback callback,
            string source) : this(formulaParts, callback)
        {
            Source = source;
        }

        /// <summary>
        /// Calculates the formula.
        /// </summary>
        public override double Calculate(IDmlGuideValueProvider guideValueProvider)
        {
            double x = DmlFormulaHelper.GetValue(X, guideValueProvider);
            double y = DmlFormulaHelper.GetValue(Y, guideValueProvider);
            double z = DmlFormulaHelper.GetValue(Z, guideValueProvider);
            return mCallback.Calculate(x, y, z);
        }

        internal string X
        {
            get { return mX; }
            set { mX = value; }
        }

        internal string Y
        {
            get { return mY; }
            set { mY = value; }
        }

        internal string Z
        {
            get { return mZ; }
            set { mZ = value; }
        }

        private readonly IDmlThreeArgumentFormulaCallback mCallback;
        private string mX;
        private string mY;
        private string mZ;
    }
}