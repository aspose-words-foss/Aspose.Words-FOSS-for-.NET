// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/10/2013 by Andrey Noskov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Helper class for Dml/Vml attributes comparing.
    /// </summary>
    internal class DmlVmlTestUtil
    {
        internal DmlVmlTestUtil(Shape shape, Shape drawingMl)
        {
            mShape = shape;
            mDrawingMl = drawingMl;
        }

        internal void Check(int key)
        {
            object vmlVal = mShape.FetchShapeAttrInternal(key);
            object dmlVal = mDrawingMl.FetchShapeAttrInternal(key);

            if (key == ShapeAttr.TransformRotation)
            {
                // Before compare Dml and Vml angle we have to normalize them.
                Assert.That(NormalizeAngle((int)dmlVal), Is.EqualTo(NormalizeAngle((int)vmlVal)).Within(1), MessageFiledKey(key));
                return;
            }

            if ((key == ShapeAttr.FillShadeColors) && (mShape.ShapePr.ContainsKey(ShapeAttr.FillShadeColors)))
            {
                // Currently we check only Length of the array, because the color and position is little bit different in Dml.
                Assert.That(((GradientColor[])dmlVal).Length, Is.EqualTo(((GradientColor[])vmlVal).Length), MessageFiledKey(key));
                return;
            }

            Assert.That(dmlVal, Is.EqualTo(vmlVal), MessageFiledKey(key));
        }

        internal ShapePr VmlShapePr
        {
            get { return mShape.ShapePr; }
        }

        internal ShapePr DmlShapePr
        {
            get { return mDrawingMl.ShapePr; }
        }

        private static string MessageFiledKey(int key)
        {
            return string.Format("Filed ShapeAttr key = {0}", key);
        }

        /// <summary>
        /// Normalizes specified angle so it becomes greater or equal to zero and less then 360 degrees.
        /// In Vml one degree equals 65536 (0x10000).
        /// </summary>
        private double NormalizeAngle(int value)
        {
            const int angle360 = 360 * 0x10000;

            while (value < 0)
                value += angle360;

            if (value > angle360)
                value %= angle360;

            return value;
        }

        private readonly Shape mShape;
        private readonly Shape mDrawingMl;
    }
}
