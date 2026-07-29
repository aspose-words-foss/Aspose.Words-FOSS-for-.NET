// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/08/2024 by Dmitry Burov

#if NETSTANDARD

using Aspose.Drawing;

namespace Aspose.Images.Pal.Graphics.Brush
{
    public abstract class GradientBrushPal : NativeBrushPal
    {
        /// <summary>
        /// Simulates the blend with interpolation colors.
        /// </summary>
        /// <returns>The blended interpolation colors.</returns>
        /// <remarks>
        /// The algorithm was initially developed by Alexey Noskov.
        /// It is for a two-color gradient only and is mutually exclusive with interpolations colors.
        /// If a more complex gradient is required please consider implementing custom shader using SKRuntimeEffect.
        /// </remarks>
        protected static DrColor[] BlendStartEndColors(
            DrColor startColor,
            DrColor endColor,
            float[] blendPositions,
            float[] blendFactors)
        {
            if (blendPositions == null)
                blendPositions = gEmptyBlendPositions;

            if (blendFactors == null)
                blendFactors = gEmptyBlendFactors;

            int nSamples = blendPositions.Length;

            DrColor[] resultColors = new DrColor[nSamples];

            float sr = startColor.R;
            float sg = startColor.G;
            float sb = startColor.B;
            float sa = startColor.A;

            float er = endColor.R;
            float eg = endColor.G;
            float eb = endColor.B;
            float ea = endColor.A;

            for (int iSample = 0; iSample < nSamples; iSample++)
            {
                float f = blendFactors[iSample];

                // Blend alpha.
                float ba = (f * ea + ((1 - f) * sa));

                // Blend color.
                float br = (f * er + ((1 - f) * sr));
                float bg = (f * eg + ((1 - f) * sg));
                float bb = (f * eb + ((1 - f) * sb));

                resultColors[iSample] = DrColor.FromArgb((int)ba, (int)br, (int)bg, (int)bb);
            }

            return resultColors;
        }

        private static readonly float[] gEmptyBlendPositions = new float[] { 0, 1.0f };
        private static readonly float[] gEmptyBlendFactors = new float[] { 0, 1.0f };
    }
}
#endif
