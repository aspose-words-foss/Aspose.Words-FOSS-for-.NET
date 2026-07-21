// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/03/2019 by Dmitry Sokolov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Calculates effect extent values which are added with the outline of the shape.
    /// All code in this class is experimental.
    /// </summary>
    internal class DmlOutlineEffectExtentCalculator
    {
        /// <summary>
        /// Updates effect extents of the specified shape according to the change of outline weight. 
        /// Takes in attention current values of effect extents.
        /// </summary>
        internal static void UpdateWeightRelatedEffectExtentPart(ShapeBase shape, double curWeight, double newWeight)
        {
            if ((shape == null) || MathUtil.AreEqual(curWeight, newWeight))
                return;

            Debug.Assert(shape.MarkupLanguage == ShapeMarkupLanguage.Dml);
            // Currently implemented only calculation of effect extent for shapes with "image" shape type.
            if (shape.IsImage)
                DmlPictureOutlineEffectExtentCalculator.UpdateWeightRelatedEffectExtentPartImage(shape, curWeight, newWeight);
        }

        /// <summary>
        /// Calculates effect extent values which are added with the outline of the picture.
        /// </summary>
        private class DmlPictureOutlineEffectExtentCalculator
        {
            /// <summary>
            /// Updates effect extents of the passed shape according to change of the image outline weight.
            /// </summary>
            internal static void UpdateWeightRelatedEffectExtentPartImage(ShapeBase shape, double curWeight, double newWeight)
            {
                Debug.Assert((shape != null) && shape.IsImage);

                // This algorithm only for images.
                // Calculate effect extent values for current and new outline weight.
                int[] newWeightExtentValues = CalculateEffectExtentsRelatedToWeightPart(newWeight);
                int[] curWeigthExtentValues = CalculateEffectExtentsRelatedToWeightPart(curWeight);

                for (int key = ShapeAttr.DmlEffectExtentLeft; key <= ShapeAttr.DmlEffectExtentBottom; ++key)
                {
                    int extent = (int)shape.FetchShapeAttrInternal(key);
                    int extentIndex = key - ShapeAttr.DmlEffectExtentLeft;
                    int weightExtentDelta = newWeightExtentValues[extentIndex] - curWeigthExtentValues[extentIndex];

                    int newExtentVal = extent + weightExtentDelta;
                    newExtentVal = (newExtentVal < 0) && (newWeightExtentValues[extentIndex] > 0) ?
                        newWeightExtentValues[extentIndex] : newExtentVal;

                    // Avoid negative extent values at any case.
                    shape.SetShapeAttrInternal(key, (newExtentVal < 0) ? 0 : newExtentVal);
                }
            }

            /// <summary>
            /// Calculates amount of effect extent which is added with the outline weight of image.
            /// </summary>
            /// <param name="weight">Weight in points.</param>
            private static int[] CalculateEffectExtentsRelatedToWeightPart(double weight)
            {
                if (MathUtil.IsZero(weight))
                    return gZeroWeightEffectExtents;

                const double maxWeightWithBaseExtent = 2.0;

                if ((weight < maxWeightWithBaseExtent) || MathUtil.AreEqual(weight, maxWeightWithBaseExtent))
                    return gBaseWeightEffectExtents;

                const double weightIncrementDelta = 1.5;
                double weightWithAdditionalExtent = weight - maxWeightWithBaseExtent;

                int extentIncrementCount = (int)System.Math.Truncate(weightWithAdditionalExtent / weightIncrementDelta);
                extentIncrementCount += MathUtil.IsZero(weightWithAdditionalExtent % weightIncrementDelta) ? 0 : 1;

                const int extentIncrementDelta = 19050;
                int additionalExtentValue = extentIncrementCount * extentIncrementDelta;

                return new int[] {
                gBaseWeightEffectExtents[0] + additionalExtentValue,
                gBaseWeightEffectExtents[1] + additionalExtentValue,
                gBaseWeightEffectExtents[2] + additionalExtentValue,
                gBaseWeightEffectExtents[3] + additionalExtentValue };
            }

            private static readonly int[] gBaseWeightEffectExtents = new int[] { 19050, 19050, 28575, 19050 };
            private static readonly int[] gZeroWeightEffectExtents = new int[] { 0, 0, 0, 0 };
        }  
    }
}
