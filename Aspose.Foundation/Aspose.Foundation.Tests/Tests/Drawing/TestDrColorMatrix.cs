// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/11/2011 by Alexey Kachalov

using Aspose.Drawing;
using NUnit.Framework;

namespace Aspose.Tests.Drawing
{
    [TestFixture]
    public class TestDrColorMatrix
    {
        [Test]
        public void Constructor_IdentityMatrix5x5IsCreated()
        {
            // Arrange, Act.
            DrColorMatrix identityColorMatrix = new DrColorMatrix();

            // Assert.
            for (int i = 0; i < 5; ++i)
            {
                for (int j = 0; j < 5; ++j)
                {
                    float expected = i == j ? 1.0f : 0.0f;
                    Assert.That(identityColorMatrix[i, j], Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void IsIdentityMatrix_DefaultConstructorIsCalled_TrueIsReturned()
        {
            // Arrange.
            DrColorMatrix identityColorMatrix = new DrColorMatrix();

            // Act.
            bool isIdentityMatrix = identityColorMatrix.IsIdentityMatrix;

            // Assert.
            Assert.That(isIdentityMatrix, Is.True);
        }

        [Test]
        public void IsIdentityMatrix_MatrixIsNotIdentity_FalseIsReturned()
        {
            // AAA1.
            DrColorMatrix colorMatrix = new DrColorMatrix();
            colorMatrix[0, 0] = 2.0f;
            Assert.That(colorMatrix.IsIdentityMatrix, Is.False);
            // AAA2.
            colorMatrix = new DrColorMatrix();
            colorMatrix[1, 2] = 1.0f;
            Assert.That(colorMatrix.IsIdentityMatrix, Is.False);
        }

        [Test]
        public void Multiply_TwoColorMatricesArePassed_ProductMatrix5x5IsCorrect()
        {
            // Arrange.
            DrColorMatrix a = new DrColorMatrix();
            FillColumn(a, 0, 1.0f);
            FillColumn(a, 1, 2.0f);
            FillColumn(a, 2, 3.0f);
            FillColumn(a, 3, 4.0f);
            FillColumn(a, 4, 5.0f);

            DrColorMatrix b = new DrColorMatrix();
            FillRow(b, 0, 6.0f);
            FillRow(b, 1, 7.0f);
            FillRow(b, 2, 8.0f);
            FillRow(b, 3, 9.0f);
            FillRow(b, 4, 10.0f);

            // Act.
            DrColorMatrix product = DrColorMatrix.Multiply(a, b);

            // Assert.
            // JAVA-changed to exclude autoporter warnings about float rounding.
            // see https://auckland.dynabic.com/wiki/display/org/Using+float+in+expression+with+more+than+one+operator
            const float expected = (float) (1.0*6.0 + 2.0*7.0 + 3.0*8.0 + 4.0*9.0 + 5.0*10.0);

            for (int i = 0; i < 5; ++i)
            {
                for (int j = 0; j < 5; ++j)
                {
                    Assert.That(product[i, j], Is.EqualTo(expected));
                }
            }
        }

        private void FillColumn(DrColorMatrix a, int column, float value)
        {
            for (int i = 0; i < DrColorMatrix.ColorMatrixSize; i++)
                a[i, column] = value;
        }

        private void FillRow(DrColorMatrix a, int row, float value)
        {
            for (int i = 0; i < DrColorMatrix.ColorMatrixSize; i++)
                a[row, i] = value;
        }
    }
}
