// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/11/2010 by Alexey Titov

using System;
using System.Collections.Generic;

namespace Aspose.Words.Drawing.Core.Dml.Guides
{
    internal class DmlGuideFactory : IDmlGuideFactory
    {
        public DmlGuide CreateGuide(string formula, string name, bool isPreset)
        {
            if (formula == null)
                throw new ArgumentNullException("formula");
            if (name == null)
                throw new ArgumentNullException("name");
            DmlFormula dmlFormula = FormulaFactory.Create(formula);
            return new DmlGuide(name, dmlFormula, isPreset);
        }

        internal DmlGuide CreateGuide(string formula, string name)
        {
            return CreateGuide(formula, name, false);
        }

        /// <summary>
        /// Initializes common guides.
        /// </summary>
        /// <remarks>
        /// Described in 5.1.12.56 ST_ShapeType (Preset Shape Types)
        /// </remarks>
        public IList<DmlGuide> CreateCommonGuides()
        {
            List<DmlGuide> commonGuides = new List<DmlGuide>(34);
            commonGuides.Add(CreateGuide("val 16200000", "3cd4"));
            commonGuides.Add(CreateGuide("val 8100000", "3cd8"));
            commonGuides.Add(CreateGuide("val 13500000", "5cd8"));
            commonGuides.Add(CreateGuide("val 18900000", "7cd8"));
            commonGuides.Add(CreateGuide("val 10800000", "cd2"));
            commonGuides.Add(CreateGuide("val 5400000", "cd4"));
            commonGuides.Add(CreateGuide("val 2700000", "cd8"));

            commonGuides.Add(CreateGuide("val h", "b"));
            commonGuides.Add(CreateGuide("val 0", "l"));
            commonGuides.Add(CreateGuide("val 0", "t"));
            commonGuides.Add(CreateGuide("val w", "r"));

            commonGuides.Add(CreateGuide("*/ w 1.0 2.0", "hc"));
            commonGuides.Add(CreateGuide("*/ h 1.0 2.0", "hd2"));
            commonGuides.Add(CreateGuide("*/ h 1.0 3.0", "hd3")); // Not described in specs
            commonGuides.Add(CreateGuide("*/ h 1.0 4.0", "hd4"));
            commonGuides.Add(CreateGuide("*/ h 1.0 5.0", "hd5"));
            commonGuides.Add(CreateGuide("*/ h 1.0 6.0", "hd6"));
            commonGuides.Add(CreateGuide("*/ h 1.0 8.0", "hd8"));

            commonGuides.Add(CreateGuide("*/ h 1.0 2.0", "vc"));
            commonGuides.Add(CreateGuide("*/ w 1.0 2.0", "wd2"));
            commonGuides.Add(CreateGuide("*/ w 1.0 3.0", "wd3")); // Not described in specs
            commonGuides.Add(CreateGuide("*/ w 1.0 4.0", "wd4"));
            commonGuides.Add(CreateGuide("*/ w 1.0 5.0", "wd5"));
            commonGuides.Add(CreateGuide("*/ w 1.0 6.0", "wd6"));
            commonGuides.Add(CreateGuide("*/ w 1.0 8.0", "wd8"));
            commonGuides.Add(CreateGuide("*/ w 1.0 10.0", "wd10"));

            // WORDSNET-26190
            // Text box rectangle for rtTriangle preset had an unrecognized value.
            commonGuides.Add(CreateGuide("*/ w 1.0 12.0", "wd12")); // Not described in specs

            commonGuides.Add(CreateGuide("*/ w 1.0 32.0", "wd32")); // Not described in specs

            commonGuides.Add(CreateGuide("max w h", "ls"));
            commonGuides.Add(CreateGuide("min w h", "ss"));
            commonGuides.Add(CreateGuide("*/ ss 1.0 2.0", "ssd2"));
            commonGuides.Add(CreateGuide("*/ ss 1.0 4.0", "ssd4"));
            commonGuides.Add(CreateGuide("*/ ss 1.0 6.0", "ssd6"));
            commonGuides.Add(CreateGuide("*/ ss 1.0 8.0", "ssd8"));
            commonGuides.Add(CreateGuide("*/ ss 1.0 16.0", "ssd16"));
            commonGuides.Add(CreateGuide("*/ ss 1.0 32.0", "ssd32")); // Not described in specs
            return commonGuides;
        }

        internal IDmlFormulaFactory FormulaFactory
        {
            get { return mFormulaFactory; }
            set { mFormulaFactory = value; }
        }

        private IDmlFormulaFactory mFormulaFactory = new DmlFormulaFactory();
    }
}
