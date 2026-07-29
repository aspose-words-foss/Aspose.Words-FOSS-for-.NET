// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2010 by Alexey Titov

using System;
using Aspose.Words.Drawing.Core.Dml.Guides;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Guides
{
    [TestFixture]
    public class TestDmlThreeArgumentFormula
    {
        [Test]
        public void Calcualte_ValidFormulaWithValue_Calculated()
        {
            // Arrange
            DmlThreeArgumentFormula formula = new DmlThreeArgumentFormula(gFormulaParts, new ThreeArgumentFormulaCallbackStub());
            DmlGuideValueProviderStub valueProvider = new DmlGuideValueProviderStub();
            valueProvider.Add("fifty", 50).Add("one", 1);
            // Act
            double result = formula.Calculate(valueProvider);
            // Assert
            Assert.That(result, Is.EqualTo(151.0));
        }

        [Test]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void Constructor_InvalidFormula_ExceptionThrown()
        {
            new DmlThreeArgumentFormula(new string[1], new ThreeArgumentFormulaCallbackStub());
        }

        [Test]
        public void Constructor_ValidFormula_PropertyInitialized()
        {
            // Arrange
            // Act
            DmlThreeArgumentFormula formula = new DmlThreeArgumentFormula(gFormulaParts, new ThreeArgumentFormulaCallbackStub());
            // Assert
            Assert.That(formula.X, Is.EqualTo("100"));
            Assert.That(formula.Y, Is.EqualTo("fifty"));
            Assert.That(formula.Z, Is.EqualTo("one"));
        }

        private static readonly string[] gFormulaParts = new string[] {"val", "100", "fifty", "one"};
    }
}
