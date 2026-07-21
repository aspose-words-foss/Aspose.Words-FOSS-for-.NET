// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2010 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Guides
{
    internal class DmlGuide
    {
        internal DmlGuide(string name, DmlFormula formula, bool isPreset)
        {
            mName = name;
            mFormula = formula;
            mIsPreset = isPreset;
        }

        /// <summary>
        /// Clones this instance of DML guid.
        /// </summary>
        internal DmlGuide Clone()
        {
            DmlGuide lhs = (DmlGuide)MemberwiseClone();

            if (mFormula != null)
                lhs.mFormula = mFormula.Clone();

            return lhs;
        }

        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        internal DmlFormula Formula
        {
            get { return mFormula; }
            set { mFormula = value; }
        }
        
        /// <summary>
        /// Mark this DmlGuide is read from the XML definitions of preset geometries.
        /// </summary>
        /// <remarks>
        /// Use this flag to avoid writing Guides from the XML definitions of preset geometries.
        /// </remarks>
        internal bool IsPreset
        {
            get { return mIsPreset; }
            set { mIsPreset = value; }
        }

        private DmlFormula mFormula;
        private string mName;
        private bool mIsPreset;
    }
}