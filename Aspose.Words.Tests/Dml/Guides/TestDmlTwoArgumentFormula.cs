// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2010 by Alexey Titov

using System;
using Aspose.Words.Drawing.Core.Dml.Guides;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Guides
{
    [TestFixture]
    public class TestDmlTwoArgumentFormula
    {
        [Test]
        public void Calcualte_ValidFormulaWithValue_Calculated()
        {
            // Arrange
            DmlTwoArgumentFormula formula = new DmlTwoArgumentFormula(gFormulaParts, new TwoArgumentFormulaCallbackStub());
            DmlGuideValueProviderStub valueProvider = new DmlGuideValueProviderStub();
            valueProvider.Add("fifty", 50);
            // Act
            double result = formula.Calculate(valueProvider);
            // Assert
            Assert.That(result, Is.EqualTo(150.0));
        }

        [Test]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void Constructor_InvalidFormula_ExceptionThrown()
        {
            new DmlTwoArgumentFormula(new string[1], new TwoArgumentFormulaCallbackStub());
        }

        [Test]
        public void Constructor_ValidFormula_PropertyInitialized()
        {
            // Arrange
            // Act
            DmlTwoArgumentFormula formula = new DmlTwoArgumentFormula(gFormulaParts, new TwoArgumentFormulaCallbackStub());
            // Assert
            Assert.That(formula.X, Is.EqualTo("100"));
            Assert.That(formula.Y, Is.EqualTo("fifty"));
        }

        private static readonly string[] gFormulaParts = new string[] { "val", "100", "fifty" };
    }
}
