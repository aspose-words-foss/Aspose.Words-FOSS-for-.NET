// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/08/2013 by Roman Korchagin

using System;
using System.Drawing.Drawing2D;
using Aspose.Drawing;
using Aspose.Images.Pal;
using NUnit.Framework;

namespace Aspose.Tests.Drawing
{
    /// <summary>
    /// Without optimization DrMatrix has to calculate 6 matrix elements
    /// for each transformation.
    /// DrMatrix can optimize number of calculations to 2 or 4 elements instead 
    /// of 6 accoding to type of transformation.
    /// Let's check is the optimization properly ported from java's AffineTransform.
    /// </summary>
    /// <remarks>
    /// We have here 10 types of transformations of 10 types of matrix: 4 pure types, 
    /// 4 combinations and 2 for rotation. Each combination of matrix transformation
    /// tested in one default order and two explicitly set orders.
    /// </remarks>
    [TestFixture]
    public class TestDrMatrixOptimizations
    {
#if !ANDROID && !NETSTANDARD
        #region default (Prepend) order

        [Test]
        public void TestScale()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Scale(10, 20, MatrixOrder.Prepend);
                matrix.Scale(10, 20);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestShear()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                //DrMatrix hasn't shear method yet
                drMatrix.Multiply(GetMatrix("Shear"), MatrixOrder.Prepend);
                matrix.Shear(4, 5);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestTranslate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Translate(10, 20, MatrixOrder.Prepend);
                matrix.Translate(10, 20);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestIdentity()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("Identity"), MatrixOrder.Prepend);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("Identity")));

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestScaleShear()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("ScaleShear"), MatrixOrder.Prepend);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("ScaleShear")));

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestScaleTranslate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("ScaleTranslate"), MatrixOrder.Prepend);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("ScaleTranslate")));

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestScaleShearTranslate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("ScaleShearTranslate"), MatrixOrder.Prepend);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("ScaleShearTranslate")));

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }
        
        [Test]
        public void TestShearTranslate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("ShearTranslate"), MatrixOrder.Prepend);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("ShearTranslate")));

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }
        
        [Test]
        public void TestRotate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("Rotate"), MatrixOrder.Prepend);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("Rotate")));

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }
        
        [Test]
        public void TestRotateAt()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("RotateAt"), MatrixOrder.Prepend);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("RotateAt")));

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        #endregion default (Prepend) order

        #region Append order

        [Test]
        public void TestAppendScale()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Scale(10, 20, MatrixOrder.Append);
                matrix.Scale(10, 20, MatrixOrder.Append);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestAppendShear()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                //DrMatrix hasn't shear method yet
                drMatrix.Multiply(GetMatrix("Shear"), MatrixOrder.Append);
                matrix.Shear(4, 5, MatrixOrder.Append);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestAppendTranslate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Translate(10, 20, MatrixOrder.Append);
                matrix.Translate(10, 20, MatrixOrder.Append);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestAppendIdentity()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("Identity"), MatrixOrder.Append);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("Identity")), MatrixOrder.Append);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestAppendScaleShear()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("ScaleShear"), MatrixOrder.Append);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("ScaleShear")), MatrixOrder.Append);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestAppendScaleTranslate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("ScaleTranslate"), MatrixOrder.Append);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("ScaleTranslate")), MatrixOrder.Append);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestAppendScaleShearTranslate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("ScaleShearTranslate"), MatrixOrder.Append);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("ScaleShearTranslate")), MatrixOrder.Append);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }
        
        [Test]
        public void TestAppendShearTranslate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("ShearTranslate"), MatrixOrder.Append);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("ShearTranslate")), MatrixOrder.Append);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }
        
        [Test]
        public void TestAppendRotate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("Rotate"), MatrixOrder.Append);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("Rotate")), MatrixOrder.Append);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }
        
        [Test]
        public void TestAppendRotateAt()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("RotateAt"), MatrixOrder.Append);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("RotateAt")), MatrixOrder.Append);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        #endregion Append order

        #region explicitly Prepend order

        [Test]
        public void TestPrependScale()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Scale(10, 20, MatrixOrder.Prepend);
                matrix.Scale(10, 20, MatrixOrder.Prepend);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestPrependShear()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                //DrMatrix hasn't shear method yet
                drMatrix.Multiply(GetMatrix("Shear"), MatrixOrder.Prepend);
                matrix.Shear(4, 5, MatrixOrder.Prepend);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestPrependTranslate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Translate(10, 20, MatrixOrder.Prepend);
                matrix.Translate(10, 20, MatrixOrder.Prepend);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestPrependIdentity()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("Identity"), MatrixOrder.Prepend);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("Identity")), MatrixOrder.Prepend);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestPrependScaleShear()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("ScaleShear"), MatrixOrder.Prepend);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("ScaleShear")), MatrixOrder.Prepend);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestPrependScaleTranslate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("ScaleTranslate"), MatrixOrder.Prepend);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("ScaleTranslate")), MatrixOrder.Prepend);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        [Test]
        public void TestPrependScaleShearTranslate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("ScaleShearTranslate"), MatrixOrder.Prepend);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("ScaleShearTranslate")), MatrixOrder.Prepend);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }
        
        [Test]
        public void TestPrependShearTranslate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("ShearTranslate"), MatrixOrder.Prepend);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("ShearTranslate")), MatrixOrder.Prepend);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }
        
        [Test]
        public void TestPrependRotate()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("Rotate"), MatrixOrder.Prepend);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("Rotate")), MatrixOrder.Prepend);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }
        
        [Test]
        public void TestPrependRotateAt()
        {
            foreach (string type in AllTypes)
            {
                DrMatrix drMatrix = GetMatrix(type);
                Matrix matrix = MatrixPal.ToNativeMatrix(drMatrix);

                drMatrix.Multiply(GetMatrix("RotateAt"), MatrixOrder.Prepend);
                matrix.Multiply(MatrixPal.ToNativeMatrix(GetMatrix("RotateAt")), MatrixOrder.Prepend);

                try
                {
                    ArrayUtil.CheckArraysEqual(drMatrix.GetElements(), matrix.Elements, 0.0001f);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(type, e);
                }
            }
        }

        #endregion explicitly Prepend order

        // We have 10 types of matrix to check: 4 pure types, 4 for combinations plus 2 types for rotate.

        private DrMatrix GetMatrix(string type)
        {
            switch (type)
            {
                case "Identity":
                    return new DrMatrix();//1, 0, 0, 1, 0, 0
                case "Scale":
                    return new DrMatrix(2, 0, 0, 3, 0, 0);
                case "Shear":
                    return new DrMatrix(1, 5, 4, 1, 0, 0);
                case "Translate":
                    return new DrMatrix(1, 0, 0, 1, 6, 7);
                case "ScaleShear":
                    return new DrMatrix(2, 5, 4, 3, 0, 0);
                case "ScaleTranslate":
                    return new DrMatrix(2, 0, 0, 3, 6, 7);
                case "ScaleShearTranslate":
                    return new DrMatrix(2, 5, 4, 3, 6, 7);
                case "ShearTranslate":
                    return new DrMatrix(1, 5, 4, 1, 6, 7);
                case "Rotate":
                    //particular case of 'scale & shear' combination
                    //let's take 30 graduses.
                    return new DrMatrix(0.866026f, -0.5f, 0.5f, 0.866026f, 0, 0);
                case "RotateAt":
                    //particular case of 'scale & shear & translate' combination
                    //let's take 30 graduses.
                    return new DrMatrix(0.866026f, -0.5f, 0.5f, 0.866026f, 6, 7);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static readonly string[] AllTypes = new string[] 
        {
            "Identity",
            "Scale",
            "Shear",
            "Translate",
            "ScaleShear",
            "ScaleTranslate",
            "ScaleShearTranslate",
            "ShearTranslate",
            "Rotate",
            "RotateAt"
        };
#endif
    }
}
