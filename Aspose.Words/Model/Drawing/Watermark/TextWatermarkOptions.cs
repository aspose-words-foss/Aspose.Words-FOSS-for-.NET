// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/22/2020 by Artem Ptitsin.

using System;
using System.Drawing;

namespace Aspose.Words
{
    /// <summary>
    /// Contains options that can be specified when adding a watermark with text.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-watermark/">Working with Watermark</a> documentation article.</para>
    /// </summary>
    public class TextWatermarkOptions
    {
        /// <summary>
        /// Gets or sets font family name. The default value is "Calibri".
        /// </summary>
        public string FontFamily
        {
            get { return mFontFamily; }
            set { mFontFamily = value; }
        }

        /// <summary>
        /// Gets or sets font color. The default value is <see cref="Color.Silver"/>.
        /// </summary>
        public Color Color
        {
            get { return mColor; }
            set { mColor = value; }
        }

        /// <summary>
        /// Gets or sets a font size. The default value is 0 - auto.
        /// </summary>
        /// <remarks>
        /// <p>Valid values range from 0 to 65.5 inclusive.</p>
        /// <p> Auto font size means that the watermark will be scaled to its max width and max height relative to
        /// the page margins.</p>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when argument was out of the range of valid values.
        /// </exception>
        public float FontSize
        {
            get { return mFontSize; }
            set { SetFontSizeInternal(value); }
        }

        /// <summary>
        /// Gets or sets a boolean value which is responsible for opacity of the watermark.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool IsSemitrasparent
        {
            get { return mIsSemitransparent; }
            set { mIsSemitransparent = value; }
        }

        /// <summary>
        /// Gets or sets layout of the watermark. The default value is <see cref="WatermarkLayout.Diagonal"/>.
        /// </summary>
        public WatermarkLayout Layout
        {
            get { return mLayout; }
            set { mLayout = value; }
        }

        internal bool IsAutoSize
        {
            get { return mFontSize == AutoSize; }
        }

        private void SetFontSizeInternal(double value)
        {
            mFontSize = (float)ArgumentUtil.ValidateRange(value, AutoSize, AutoSize, WordUtil.MaxFontSize,
                WordUtil.MaxFontSize, true, "Size");
        }

        private string mFontFamily = "Calibri";
        private Color mColor = Color.Silver;
        private bool mIsSemitransparent = true;
        private float mFontSize = AutoSize;
        private WatermarkLayout mLayout = WatermarkLayout.Diagonal;
        private const float AutoSize = 0;
    }
}
