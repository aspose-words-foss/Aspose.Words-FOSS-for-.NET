// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2010 by Alexey Titov

using System;
using Aspose.Words.Drawing.Core.Dml.Guides;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Guides
{
    [TestFixture]
    public class TestDmlOneArgumentFormula
    {
        [Test]
        public void Calcualte_ValidFormulaWithGuideName_Calculated()
        {
            // Arrange
            string[] formulaParts = new string[] { "val", "fifty" };
            DmlOneArgumentFormula formula = new DmlOneArgumentFormula(formulaParts, new OneArgumentFormulaCallbackStub());
            DmlGuideValueProviderStub valueProvider = new DmlGuideValueProviderStub();
            valueProvider.Add("fifty", 50);
            // Act
            double result = formula.Calculate(valueProvider);
            // Assert
            Assert.That(result, Is.EqualTo(50.0));
        }

        [Test]
        public void Calcualte_ValidFormulaWithValue_Calculated()
        {
            // Arrange
            DmlOneArgumentFormula formula = new DmlOneArgumentFormula(gFormulaParts, new OneArgumentFormulaCallbackStub());
            DmlGuideValueProviderStub valueProvider = new DmlGuideValueProviderStub();
            // Act
            double result = formula.Calculate(valueProvider);
            // Assert
            Assert.That(result, Is.EqualTo(100.0));
        }

        [Test]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void Constructor_InvalidFormula_ExceptionThrown()
        {
            new DmlOneArgumentFormula(new string[1], new OneArgumentFormulaCallbackStub());
        }

        [Test]
        public void Constructor_ValidFormula_PropertyInitialized()
        {
            // Act
            DmlOneArgumentFormula formula = new DmlOneArgumentFormula(gFormulaParts, new OneArgumentFormulaCallbackStub());
            // Assert
            Assert.That(formula.X, Is.EqualTo("100"));
        }

        private static readonly string[] gFormulaParts = new string[] {"val", "100"};
    }
}
