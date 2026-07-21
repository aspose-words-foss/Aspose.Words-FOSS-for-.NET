// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/22/2020 by Artem Ptitsin.

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Contains options that can be specified when adding a watermark with image.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-watermark/">Working with Watermark</a> documentation article.</para>
    /// </summary>
    public class ImageWatermarkOptions
    {
        /// <summary>
        /// Gets or sets the scale factor expressed as a fraction of the image. The default value is 0 - auto.
        /// </summary>
        /// <remarks>
        /// <p>Valid values range from 0 to 65.5 inclusive.</p>
        /// <p>Auto scale means that the watermark will be scaled to its max width and max height relative to
        /// the page margins.</p>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when argument was out of the range of valid values.
        /// </exception>
        public double Scale
        {
            get { return mScale; }
            set { SetScaleInternal(value); }
        }

        /// <summary>
        /// Gets or sets a boolean value which is responsible for washout effect of the watermark.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool IsWashout
        {
            get { return mIsWashout; }
            set { mIsWashout = value; }
        }

        internal bool IsScaleAuto
        {
            get { return mScale == AutoScale; }
        }

        private void SetScaleInternal(double value)
        {
            mScale = ArgumentUtil.ValidateRange(value, 0, 0, MaxValue, MaxValue, true, "Scale");
        }

        private const double AutoScale = 0;
        private double mScale = AutoScale;
        private bool mIsWashout = true;
        private const double MaxValue = 65.5;
    }
}
