// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/18/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.Guides
{
    internal abstract class DmlFormula
    {
        public abstract double Calculate(IDmlGuideValueProvider guideValueProvider);

        /// <summary>
        /// Clones this instance of formula.
        /// </summary>
        internal virtual DmlFormula Clone()
        {
            return (DmlFormula)MemberwiseClone();
        }

        public string Source
        {
            get { return mSource; }
            set { mSource = value; }
        }

        private string mSource;
    }
}
