// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2017 by Alexey Noskov

#if NETSTANDARD
namespace System.Drawing.Drawing2D
{
    public class ColorBlend
    {
        public ColorBlend(float[] positions, Color[] colors)
        {
            if (positions == null)
                throw new ArgumentNullException("Fractions array cannot be null");

            if (colors == null)
                throw new ArgumentNullException("Colors array cannot be null");

            if (positions.Length != colors.Length)
                throw new ArgumentException("Colors and positions must have equal size");

            if (colors.Length < 2)
                throw new ArgumentException("User must specify at least 2 colors");

            // check that values are in the proper range and progress
            // in increasing order from 0 to 1
            foreach (float currentFraction in positions)
            {
                if (currentFraction < 0f || currentFraction > 1f)
                    throw new ArgumentException("Fraction values must be in the range 0 to 1: " + currentFraction);
            }

            // We have to deal with the cases where the first gradient stop is not
            // equal to 0 and/or the last gradient stop is not equal to 1.
            // In both cases, create a new point and replicate the previous
            // extreme point's color.
            bool fixFirst = false;
            bool fixLast = false;
            int len = positions.Length;
            int off = 0;

            if (positions[0] != 0f)
            {
                // first stop is not equal to zero, fix this condition
                fixFirst = true;
                len++;
                off++;
            }
            if (positions[positions.Length - 1] != 1f)
            {
                // last stop is not equal to one, fix this condition
                fixLast = true;
                len++;
            }
            mPositions = new float[len];
            Array.Copy(positions, 0, mPositions, off, positions.Length);
            mColors = new Color[len];
            Array.Copy(colors, 0, mColors, off, colors.Length);

            if (fixFirst)
            {
                mPositions[0] = 0f;
                mColors[0] = colors[0];
            }
            if (fixLast)
            {
                mPositions[len - 1] = 1f;
                mColors[len - 1] = colors[colors.Length - 1];
            }
        }

        public float[] Positions
        {
            get { return mPositions; }
        }

        public Color[] Colors
        {
            get { return mColors; }
        }

        public int[] AndroidColors
        {
            get
            {
                int[] result = new int[Colors.Length];
                for (int i = 0; i < result.Length; i++)
                    result[i] = Colors[i].ToArgb();

                return result;
            }
        }

        private readonly float[] mPositions;
        private readonly Color[] mColors;
    }
}
#endif
