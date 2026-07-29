// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/10/2015 by Anton Savko

using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html.Reader.CommonBorder
{
    /// <summary>
    /// Stores border information.
    /// </summary>
    internal class BorderInfo
    {
        internal BorderInfo(
            CssDeclarationCollection containerDeclarations,
            BorderType borderType,
            string paddingPropertyName)
        {
            Debug.Assert(containerDeclarations != null);
            Debug.Assert(StringUtil.HasChars(paddingPropertyName));

            mBorder = new Border();
            CssBorderStyleConverter.ToModelBorder(containerDeclarations, borderType, mBorder);

            mPadding = containerDeclarations.GetLength(paddingPropertyName);
        }

        internal void ApplyBorderPropertiesTo(Border border)
        {
            Debug.Assert(border != null);
            if (mBorder.IsVisible)
            {
                border.LineStyle = mBorder.LineStyle;
                border.LineWidth = mBorder.LineWidth;
                border.ColorInternal = mBorder.ColorInternal;
            }
        }

        internal void ApplyPaddingTo(Border border)
        {
            Debug.Assert(border != null);
            if (HasPadding)
            {
                border.SetDistanceFromTextSafe(mPadding);
            }
        }

        internal bool IsVisible
        {
            get { return mBorder.IsVisible; }
        }

        internal LineStyle LineStyle
        {
            get { return mBorder.LineStyle; }
        }

        internal bool HasPadding
        {
            get { return !MathUtil.IsMinValue(mPadding); }
        }

        internal double Padding
        {
            get
            {
                return (HasPadding)
                    ? mPadding
                    : 0;
            }
        }

        private readonly Border mBorder;

        private readonly double mPadding;
    }
}
