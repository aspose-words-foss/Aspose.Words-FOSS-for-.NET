// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/11/2010 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Guides;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Guides
{
    [TestFixture]
    public class TestDmlFormulaFactory
    {
        [Test]
        public void AbsoluteValueFormula()
        {
            CheckFormulaResult("abs -1000", 1000);
        }

        [Test]
        public void AddDivideFormula()
        {
            CheckFormulaResult("+/ 2 3 4", 1.25);
        }

        [Test]
        public void AddDivideFormulaWithAdditionalSpaces()
        {
            CheckFormulaResult("+/  2  3    4   ", 1.25);
        }

        [Test]
        public void AddSubtractFormula()
        {
            CheckFormulaResult("+- 2 3 4", 1);
        }

        [Test]
        public void ArcTanFormula()
        {
            CheckFormulaResult("at2 2 3.5", 3615307.1221834663d);
        }

        [Test]
        public void CosineArcTanFormula()
        {
            CheckFormulaResult("cat2 2 3 4", 1.2);
        }

        [Test]
        public void CosineFormula()
        {
            CheckFormulaResult("cos 2 180000", 1.9972590695091477475689841168789);
        }

        [Test]
        public void IfElseFormula()
        {
            CheckFormulaResult("?: -1000 1 2", 2);
            CheckFormulaResult("?: 1 2 3", 2);
            CheckFormulaResult("?: 0 2 3", 3);
        }

        [Test]
        public void LiteralValueFormula()
        {
            CheckFormulaResult("val 1", 1.0);
        }

        [Test]
        public void MaximumValueFormula()
        {
            CheckFormulaResult("max 5.1 6.2", 6.2);
        }

        [Test]
        public void MinimumValueFormula()
        {
            CheckFormulaResult("min 5.1 6.2", 5.1);
        }

        [Test]
        public void ModuleFormula()
        {
            CheckFormulaResult("mod 1 2 3", 3.7416573867739413855837487323165);
        }

        [Test]
        public void MultiplyDivideFormula()
        {
            CheckFormulaResult("*/ 2 3 4", 1.5);
        }

        [Test]
        public void PinToFormula()
        {
            CheckFormulaResult("pin 3 4 5", 4);
            CheckFormulaResult("pin 5 4 3", 5);
            CheckFormulaResult("pin 3 5 4", 4);
        }

        [Test]
        public void SineArcTanFormula()
        {
            CheckFormulaResult("sat2 2 3 4", 1.6);
        }

        [Test]
        public void SineFormula()
        {
            CheckFormulaResult("sin 2 900000", 0.5176380902050415246977976752481);
        }

        [Test]
        public void SquareRootFormula()
        {
            CheckFormulaResult("sqrt -4", 2.0);
        }

        [Test]
        public void TangentFormula()
        {
            CheckFormulaResult("tan 2 180000", 0.10481555856608240807761164894797);
        }

        private static void CheckFormulaResult(string formulaString, double expectedResult)
        {
            DmlFormulaFactory factory = new DmlFormulaFactory();

            // Arrange
            DmlFormula formula = factory.Create(formulaString);
            // Act
            double result = formula.Calculate(new DmlGuideValueProviderStub());
            // Assert
            Assert.That(result, Is.EqualTo(expectedResult).Within(1e-9));
        }
    }
}
