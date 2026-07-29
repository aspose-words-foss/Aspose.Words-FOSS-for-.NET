// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/11/2011 by Alexey Kachalov

using System;
using System.Diagnostics.CodeAnalysis;

namespace Aspose.Drawing
{
    /// <summary>
    /// Represents a color transformation matrix. This is a 5x5 matrix that contains
    /// the coordinates for the RGBAW space. Color transformation matrices are used for
    /// applying effects to pictures. Objects of this class are used by DrImageAttributes
    /// class objects.
    /// </summary>
    public class DrColorMatrix
    {
        /// <summary>
        /// Initializes a new instance of the class with identity matrix by default.
        /// </summary>
        public DrColorMatrix()
            : this(true)
        {
        }

        /// <summary>
        /// Create a color matrix from the given array. Used in java Pal classes.
        /// </summary>
        public DrColorMatrix(float[][] colorMatrix)
        {
            for (int row = 0; row < ColorMatrixSize; row++)
                for (int col = 0; col < ColorMatrixSize; col++)
                    mColorMatrix[row, col] = colorMatrix[row][col];
        }

        private DrColorMatrix(bool initializeWithIdentityMatrix)
        {
            if (initializeWithIdentityMatrix)
            {
                // Set diagonal values with ones.
                for (int i = 0; i < ColorMatrixSize; ++i)
                {
                    mColorMatrix[i, i] = 1.0f;
                }
            }
        }

        /// <summary>
        /// Calculates a product of matrices multiplication for pointed color transformation matrices.
        /// </summary>
        public static DrColorMatrix Multiply(DrColorMatrix leftMatrix, DrColorMatrix rightMatrix)
        {
            if (leftMatrix == null)
                throw new ArgumentNullException("leftMatrix");
            if (rightMatrix == null)
                throw new ArgumentNullException("rightMatrix");

            DrColorMatrix productMatrix = new DrColorMatrix(false);

            for (int i = 0; i < ColorMatrixSize; ++i)
                for (int j = 0; j < ColorMatrixSize; ++j)
                    for (int k = 0; k < ColorMatrixSize; ++k)
                        productMatrix[i, j] += leftMatrix[i, k] * rightMatrix[k, j];

            return productMatrix;
        }

        /// <summary>
        /// Gets a value indicating whether this color transformation matrix is identity matrix.
        /// </summary>
        public bool IsIdentityMatrix
        {
            get
            {
                for (int i = 0; i < ColorMatrixSize; ++i)
                {
                    for (int j = 0; j < ColorMatrixSize; ++j)
                    {
                        float elementValue = mColorMatrix[i, j];
                        if (i == j && elementValue != 1.0f || i != j && elementValue != 0.0f)
                            return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value of this color matrix element by specified row and column indices.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional",
            Justification = "Multidimensional indexer is proper for matrix.")]
        public float this[int rowIndex, int columnIndex]
        {
            get
            {
                return mColorMatrix[rowIndex, columnIndex];
            }
            set
            {
                if (rowIndex > ColorMatrixSize - 1 || columnIndex > ColorMatrixSize - 1)
                    throw new ArgumentOutOfRangeException("DrColorMatrix has size 5x5.");

                mColorMatrix[rowIndex, columnIndex] = value;
            }
        }


        /// <summary>
        /// The size of color matrix.
        /// </summary>
        public const int ColorMatrixSize = 5;

        private readonly float[,] mColorMatrix = new float[ColorMatrixSize, ColorMatrixSize];
    }
}
