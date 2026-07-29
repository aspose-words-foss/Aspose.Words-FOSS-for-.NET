// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2010 by Alexey Titov

using System;
using Aspose.Words.Drawing.Core.Dml.Guides.FormulaCallbacks;

namespace Aspose.Words.Drawing.Core.Dml.Guides
{
    internal class DmlFormulaFactory : IDmlFormulaFactory
    {
        public DmlFormula Create(string formula)
        {
            string[] formulaParts = ParseFormula(formula);

            string formulaType = formulaParts[0];

            DmlFormula dmlFormula;
            switch (formulaType)
            {
                case "*/":
                    dmlFormula = new DmlThreeArgumentFormula(formulaParts, new MultiplyDivideCallback());
                    break;
                case "+-":
                    dmlFormula = new DmlThreeArgumentFormula(formulaParts, new AddSubtractCallback());
                    break;
                case "+/":
                    dmlFormula = new DmlThreeArgumentFormula(formulaParts, new AddDivideCallback());
                    break;
                case "?:":
                    dmlFormula = new DmlThreeArgumentFormula(formulaParts, new IfElseCallback());
                    break;
                case "abs":
                    dmlFormula = new DmlOneArgumentFormula(formulaParts, new AbsoluteValueCallback());
                    break;
                case "at2":
                    dmlFormula = new DmlTwoArgumentFormula(formulaParts, new ArcTanCallback());
                    break;
                case "cat2":
                    dmlFormula = new DmlThreeArgumentFormula(formulaParts, new CosineArcTanCallback());
                    break;
                case "cos":
                    dmlFormula = new DmlTwoArgumentFormula(formulaParts, new CosineCallback());
                    break;
                case "max":
                    dmlFormula = new DmlTwoArgumentFormula(formulaParts, new MaximumValueCallback());
                    break;
                case "min":
                    dmlFormula = new DmlTwoArgumentFormula(formulaParts, new MinimumValueCallback());
                    break;
                case "mod":
                    dmlFormula = new DmlThreeArgumentFormula(formulaParts, new ModuloCallback());
                    break;
                case "pin":
                    dmlFormula = new DmlThreeArgumentFormula(formulaParts, new PinToCallback());
                    break;
                case "sat2":
                    dmlFormula = new DmlThreeArgumentFormula(formulaParts, new SineArcTanCallback());
                    break;
                case "sin":
                    dmlFormula = new DmlTwoArgumentFormula(formulaParts, new SineCallback());
                    break;
                case "sqrt":
                    dmlFormula = new DmlOneArgumentFormula(formulaParts, new SquareRootCallback());
                    break;
                case "tan":
                    dmlFormula = new DmlTwoArgumentFormula(formulaParts, new TangentCallback());
                    break;
                case "val":
                    dmlFormula = new DmlOneArgumentFormula(formulaParts, new LiteralValueCallback());
                    break;
                default:
                    throw new ArgumentOutOfRangeException("formula",
                                                          String.Format("Unknown formula type '{0}'.", formulaType));
            }

            dmlFormula.Source = formula;

            return dmlFormula;
        }

        private static string[] ParseFormula(string formula)
        {
            string[] formulaParts = formula.Split(' ');
            formulaParts = RemoveEmptyEntities(formulaParts);
            if (formulaParts.Length == 0)
                throw new ArgumentOutOfRangeException("formula", String.Format("Bad formula '{0}'.", formula));
            return formulaParts;
        }

        private static string[] RemoveEmptyEntities(string[] parts)
        {
            int nonEmptyCount = 0;
            foreach (string s in parts)
            {
                if (StringUtil.HasChars(s))
                    nonEmptyCount++;
            }

            string[] result = new string[nonEmptyCount];
            int i = 0;
            foreach (string s in parts)
            {
                if (StringUtil.HasChars(s))
                    result[i++] = s;
            }

            return result;
        }
    }
}