// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/09/2010 by Konstantin Sidorenko

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Aspose.Drawing;
using NUnit.Framework;

namespace Aspose.Tests.Drawing
{
    [TestFixture]
    public class TestDrMatrix
    {
        [Test]
        public void TestGetElements()
        {
            float[] matrix = new float[] {11, 12, 21, 22, 31, 32};
            DrMatrix drMatrix = new DrMatrix(matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
            float[] elements = drMatrix.GetElements();
            Assert.That(ArrayUtil.IsArrayEqual(matrix, elements), Is.True);
        }

        [Test]
        public void TestTransformPoints()
        {
            PointF[] points = new PointF[] { new PointF(1, 1) };

            DrMatrix identity = new DrMatrix();
            identity.TransformPoints(points);
            Assert.That(new PointF(1, 1), Is.EqualTo(points[0]));

            DrMatrix scaling = new DrMatrix(10, 0, 0, 20, 0, 0);
            scaling.TransformPoints(points);
            Assert.That(new PointF(10, 20), Is.EqualTo(points[0]));

            DrMatrix shear = new DrMatrix(0, 20, 10, 0, 0, 0);
            shear.TransformPoints(points);
            Assert.That(new PointF(200, 200), Is.EqualTo(points[0]));

            DrMatrix rotate90 = new DrMatrix(0, 1, -1, 0, 0, 0);
            rotate90.TransformPoints(points);
            Assert.That(new PointF(-200, 200), Is.EqualTo(points[0]));

            DrMatrix translate = new DrMatrix(1, 0, 0, 1, 10, 20);
            translate.TransformPoints(points);
            Assert.That(new PointF(-190, 220), Is.EqualTo(points[0]));

            points[0] =  new PointF(1, 1);
            DrMatrix allTogether = new DrMatrix(10, 20, -10, 20, 10, 20);
            allTogether.TransformPoints(points);
            Assert.That(new PointF(10, 60), Is.EqualTo(points[0]));
        }

        [Test]
        public void TestScale()
        {
            DrMatrix matrix = new DrMatrix();
            matrix.Scale(10, 20, MatrixOrder.Prepend);

            DrMatrix scaling = new DrMatrix(10, 0, 0, 20, 0, 0);
            Assert.That(scaling, Is.EqualTo(matrix));
        }

        [Test]
        public void TestScaleDifferentOrder()
        {
            DrMatrix matrix = new DrMatrix(1, 2, 3, 4, 5, 6);

            matrix.Scale(10, 20, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(10, 20, 60, 80, 5, 6), Is.EqualTo(matrix));

            matrix.Scale(10, 20, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(100, 200, 1200, 1600, 5, 6), Is.EqualTo(matrix));

            matrix.Scale(10, 20, MatrixOrder.Append);
            Assert.That(new DrMatrix(1000, 4000, 12000, 32000, 50, 120), Is.EqualTo(matrix));

            matrix.Scale(10, 20, MatrixOrder.Append);
            Assert.That(new DrMatrix(10000, 80000, 120000, 640000, 500, 2400), Is.EqualTo(matrix));
        }

        [Test]
        public void TestTranslate()
        {
            DrMatrix matrix = new DrMatrix();
            matrix.Translate(10, 20, MatrixOrder.Prepend);

            DrMatrix translate = new DrMatrix(1, 0, 0, 1, 10, 20);
            Assert.That(translate, Is.EqualTo(matrix));
        }

        [Test]
        public void TestTranslateDifferentOrder()
        {
            DrMatrix matrix = new DrMatrix(1, 2, 3, 4, 5, 6);

            matrix.Translate(10, 20, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(1, 2, 3, 4, 75, 106), Is.EqualTo(matrix));

            matrix.Translate(10, 20, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(1, 2, 3, 4, 145, 206), Is.EqualTo(matrix));

            matrix.Translate(10, 20, MatrixOrder.Append);
            Assert.That(new DrMatrix(1, 2, 3, 4, 155, 226), Is.EqualTo(matrix));

            matrix.Translate(10, 20, MatrixOrder.Append);
            Assert.That(new DrMatrix(1, 2, 3, 4, 165, 246), Is.EqualTo(matrix));
        }

        [Test]
        public void TestMultiply()
        {
            DrMatrix matrix = new DrMatrix(1, 2, 3, 4, 5, 6);
            DrMatrix another = new DrMatrix(10, 20, 30, 40, 50, 60);
            matrix.Multiply(another, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(70, 100, 150, 220, 235, 346), Is.EqualTo(matrix));
        }

        [Test]
        public void TestMultiplyDifferentOrder()
        {
            DrMatrix matrix = new DrMatrix(1, 2, 3, 4, 5, 6);
            DrMatrix another = new DrMatrix(10, 20, 30, 40, 50, 60);

            matrix.Multiply(another, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(70, 100, 150, 220, 235, 346), Is.EqualTo(matrix));

            matrix.Multiply(another, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(3700, 5400, 8100, 11800, 12735, 18546), Is.EqualTo(matrix));

            matrix.Multiply(another, MatrixOrder.Append);
            Assert.That(new DrMatrix(199000, 290000, 435000, 634000, 683780, 996600), Is.EqualTo(matrix));

            another = new DrMatrix(1, 2, 3, 4, 5, 6);
            matrix.Multiply(another, MatrixOrder.Append);
            Assert.That(new DrMatrix(1069000, 1558000, 2337000, 3406000, 3673585, 5353966), Is.EqualTo(matrix));
        }

        [Test]
        public void TestRotatePredefined()
        {
            DrMatrix matrix = new DrMatrix();

            //0 degrees - do nothing
            matrix.Rotate(0, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(), Is.EqualTo(matrix));

            matrix.Rotate(90, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(0, 1, -1, 0, 0, 0), Is.EqualTo(matrix));

            matrix.Reset();
            matrix.Rotate(-270, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(0, 1, -1, 0, 0, 0), Is.EqualTo(matrix));

            matrix.Reset();
            matrix.Rotate(180, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(-1, 0, 0, -1, 0, 0), Is.EqualTo(matrix));

            matrix.Reset();
            matrix.Rotate(-180, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(-1, 0, 0, -1, 0, 0), Is.EqualTo(matrix));

            matrix.Reset();
            matrix.Rotate(270, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(0, -1, 1, 0, 0, 0), Is.EqualTo(matrix));

            matrix.Reset();
            matrix.Rotate(-90, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(0, -1, 1, 0, 0, 0), Is.EqualTo(matrix));
        }

        [Test]
        public void TestAppendRotatePredefined()
        {
            DrMatrix matrix = new DrMatrix(1, 2, 3, 4, 5, 6);

            //0 degrees - do nothing
            matrix.Rotate(0, MatrixOrder.Append);
            Assert.That(new DrMatrix(1, 2, 3, 4, 5, 6), Is.EqualTo(matrix));

            matrix.Rotate(90, MatrixOrder.Append);
            Assert.That(new DrMatrix(-2, 1, -4, 3, -6, 5), Is.EqualTo(matrix));

            matrix.Rotate(-270, MatrixOrder.Append);
            Assert.That(new DrMatrix(-1, -2, -3, -4, -5, -6), Is.EqualTo(matrix));

            matrix.Rotate(180, MatrixOrder.Append);
            Assert.That(new DrMatrix(1, 2, 3, 4, 5, 6), Is.EqualTo(matrix));

            matrix.Rotate(-180, MatrixOrder.Append);
            Assert.That(new DrMatrix(-1, -2, -3, -4, -5, -6), Is.EqualTo(matrix));

            matrix.Rotate(270, MatrixOrder.Append);
            Assert.That(new DrMatrix(-2, 1, -4, 3, -6, 5), Is.EqualTo(matrix));

            matrix.Rotate(-90, MatrixOrder.Append);
            Assert.That(new DrMatrix(1, 2, 3, 4, 5, 6), Is.EqualTo(matrix));
        }

        [Test]
        public void TestRotate()
        {
            DrMatrix matrix = new DrMatrix();

            matrix.Rotate(30, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(0.8660254f, 0.5f, -0.5f, 0.8660254f, 0, 0), Is.EqualTo(matrix));

            matrix.Rotate(45, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(0.25881904f, 0.9659258f, -0.9659258f, 0.25881904f, 0, 0), Is.EqualTo(matrix));

            matrix.Rotate(60, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(-0.70710677f, 0.70710677f, -0.70710677f, -0.70710677f, 0, 0), Is.EqualTo(matrix));

            matrix.Rotate(42.42f, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(-0.9989863f, 0.04501431f, -0.04501431f, -0.9989863f, 0, 0), Is.EqualTo(matrix));
        }

        [Test]
        public void TestRotate2()
        {
            DrMatrix matrix = new DrMatrix(1, 2, 3, 4, 5, 6);

            matrix.Rotate(30, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(2.3660254f, 3.732051f, 2.098076f, 2.4641016f, 5, 6), Is.EqualTo(matrix));

            matrix.Rotate(45, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(3.1565964f, 4.3813415f, -0.1894688f, -0.89657557f, 5, 6), Is.EqualTo(matrix));

            matrix.Rotate(60, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(1.4142134f, 1.4142135f, -2.828427f, -4.242641f, 5, 6), Is.EqualTo(matrix));

            matrix.Rotate(42.42f, MatrixOrder.Prepend);
            Assert.That(new DrMatrix(-0.86394346f, -1.8179157f, -3.041973f, -4.085974f, 5, 6), Is.EqualTo(matrix));
        }

        [Test]
        public void TestAppendRotate()
        {
            DrMatrix matrix = new DrMatrix(1, 2, 3, 4, 5, 6);

            matrix.Rotate(30, MatrixOrder.Append);
            Assert.That(new DrMatrix(-0.1339746f, 2.232051f, 0.5980762f, 4.964102f, 1.330127f, 7.696152f), Is.EqualTo(matrix));

            matrix.Rotate(45, MatrixOrder.Append);
            Assert.That(new DrMatrix(-1.6730326f, 1.483564f, -3.0872462f, 3.9330537f, -4.5014596f, 6.382543f), Is.EqualTo(matrix));

            matrix.Rotate(60, MatrixOrder.Append);
            Assert.That(new DrMatrix(-2.1213205f, -0.70710677f, -4.9497476f, -0.70710677f, -7.7781744f, -0.7071068f), Is.EqualTo(matrix));

            matrix.Rotate(42.42f, MatrixOrder.Append);
            Assert.That(new DrMatrix(-1.0890151f, -1.9529585f, -3.1770163f, -3.8609025f, -5.2650175f, -5.7688465f), Is.EqualTo(matrix));
        }

        [Test]
        public void TestRotateAt()
        {
            DrMatrix matrix = new DrMatrix();

            matrix.RotateAt(90, new PointF(10, 20), MatrixOrder.Prepend);
            Assert.That(new DrMatrix(0, 1, -1, 0, 30, 10), Is.EqualTo(matrix));

            matrix = new DrMatrix(1, 2, 3, 4, 5, 6);

            matrix.RotateAt(42, new PointF(10, 20), MatrixOrder.Prepend);
            Assert.That(new DrMatrix(2.7505367f, 4.162812f, 1.5603039f, 1.6343181f, 16.288555f, 31.685516f), Is.EqualTo(matrix));

            matrix.RotateAt(47, new PointF(10, 20), MatrixOrder.Prepend);
            Assert.That(new DrMatrix(3.0169957f, 4.0342956f, -0.94749045f, -1.9298859f, 63.779854f, 104.25476f), Is.EqualTo(matrix));
        }

        [Test]
        public void TestAppendRotateAt()
        {
            DrMatrix matrix = new DrMatrix();

            matrix.RotateAt(90, new PointF(10, 20), MatrixOrder.Append);
            Assert.That(new DrMatrix(0, 1, -1, 0, 30, 10), Is.EqualTo(matrix));

            matrix = new DrMatrix(1, 2, 3, 4, 5, 6);

            matrix.RotateAt(42, new PointF(10, 20), MatrixOrder.Append);
            Assert.That(new DrMatrix(-0.5951164f, 2.1554203f, -0.44708794f, 4.979971f, 15.652104f, 6.2503195f), Is.EqualTo(matrix));

            matrix.RotateAt(47, new PointF(10, 20), MatrixOrder.Append);
            Assert.That(new DrMatrix(-1.9822431f, 1.0347525f, -3.9470334f, 3.0693526f, 23.910606f, 14.756428f), Is.EqualTo(matrix));
        }

        [Test]
        public void TestConcatenateDefaultOrder()
        {
            DrMatrix matrix = new DrMatrix();

            matrix.Scale(10, 20, MatrixOrder.Prepend);
            matrix.Translate(100, 200, MatrixOrder.Prepend);
            matrix.Rotate(42, MatrixOrder.Prepend);
            matrix.RotateAt(90, new PointF(10, 20), MatrixOrder.Prepend);
            matrix.Multiply(new DrMatrix(1, 2, 3, 4, 5, 6), MatrixOrder.Prepend);
            DrMatrix expected = new DrMatrix(-21.554203f, -11.9023275f, -49.7997131f, -8.941758f, 1077.98523f, 4544.12646f);
            ArrayUtil.CheckArraysEqual(matrix.GetElements(), expected.GetElements(), 0.0001f);
        }

        [Test]
        public void TestCreateStretchMatrix()
        {
            //Arrange
            RectangleF from = new RectangleF(1,2,4,16);
            RectangleF to = new RectangleF(1,2,2,4);
            //Act
            DrMatrix matrix = DrMatrix.CreateStretchMatrix(from, to);
            //Assert
            RectangleF result = matrix.Transform(from);
            Assert.That(result, Is.EqualTo(to));
        }

        [Test]
        public void TestTransformRectangle()
        {
            //Arrange
            RectangleF rect = new RectangleF(1, 2, 4, 16);
            DrMatrix matrix = new DrMatrix();
            matrix.Scale(2, 1, MatrixOrder.Prepend);
            matrix.Translate(-2,-2,MatrixOrder.Append);
            //Act
            RectangleF result = matrix.Transform(rect);
            //Assert
            Assert.That(result, Is.EqualTo(new RectangleF(0,0,8,16)));
        }

        /// <summary>
        /// There was error when multiplying identity matrix by shear matrix with MatrixOrder.Prepend.
        /// </summary>
        [Test]
        public void TestJira8250()
        {
            DrMatrix matrix = new DrMatrix();
            DrMatrix shearMatrix = new DrMatrix(0, -1, 1, 0, 0, 0);

            matrix.Multiply(shearMatrix, MatrixOrder.Prepend);

            Assert.That(shearMatrix, Is.EqualTo(matrix));
        }

        [Test]
        public void TestEqualsIgnoreTranslation()
        {
            DrMatrix matrix = new DrMatrix();
            Assert.That(DrMatrix.EqualsIgnoreTranslation(matrix, null), Is.False);
            Assert.That(DrMatrix.EqualsIgnoreTranslation(null, matrix), Is.False);

            Assert.That(DrMatrix.EqualsIgnoreTranslation(matrix, matrix), Is.True);
            Assert.That(DrMatrix.EqualsIgnoreTranslation(null, null), Is.True);

            DrMatrix matrix2 = matrix.Clone();
            Assert.That(DrMatrix.EqualsIgnoreTranslation(matrix, matrix2), Is.True);

            matrix2.Translate(10, 10, MatrixOrder.Prepend);
            Assert.That(DrMatrix.EqualsIgnoreTranslation(matrix, matrix2), Is.True);

            matrix2.Rotate(1, MatrixOrder.Prepend);
            Assert.That(DrMatrix.EqualsIgnoreTranslation(matrix, matrix2), Is.False);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestMatrixLock()
        {
            DrMatrix matrix = new DrMatrix();
            
            Assert.That(matrix.Locked, Is.False);

            matrix.Scale(10, 20, MatrixOrder.Prepend);

            matrix.Lock();

            Assert.That(matrix.Locked, Is.True);

            matrix.Scale(10, 20, MatrixOrder.Prepend);
        }

        [Test]
        public void TestCloneLockedMatrix()
        {
            DrMatrix matrix = new DrMatrix();

            matrix.Lock();

            DrMatrix clonedMatrix = matrix.Clone();

            Assert.That(matrix.Locked, Is.True);
            Assert.That(clonedMatrix.Locked, Is.False);
        }
    }
}
