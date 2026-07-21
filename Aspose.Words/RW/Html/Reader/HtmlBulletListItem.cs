// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/06/2019 by Anton Savko

using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Represents list item with a bullet.
    /// </summary>
    internal class HtmlBulletListItem : HtmlListItemBase
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="listLevelId">List level identifier.</param>
        /// <param name="markerType">Marker type.</param>
        /// <param name="builder">DocumentBuilder.</param>
        /// <param name="formatter">DocumentFormatter.</param>
        /// <param name="numberFormat">Number format.</param>
        /// <param name="numberStyle">Number style.</param>
        /// <param name="fontName">Bullet's font name.</param>
        internal HtmlBulletListItem(
            HtmlListLevelId listLevelId,
            HtmlMarkerType markerType,
            DocumentBuilder builder,
            DocumentFormatter formatter,
            string numberFormat,
            NumberStyle numberStyle,
            string fontName)
            : base(listLevelId, markerType, numberFormat)
        {
            Debug.Assert(builder != null);
            Debug.Assert(formatter != null);

            mBuilder = builder;
            mFormatter = formatter;

            SetListLevelType(HtmlModelListLevelType.Bullet);
            SetNumberFormat(numberFormat);
            SetNumberStyle(ListLevelId.ListLevelNumber, numberStyle);

            mFontName = fontName;
        }

        internal override void PostModifyList(HtmlModelList modelList)
        {
            HtmlModelListLevel modelListLevel = modelList.GetListLevel(ListLevelId.ListLevelNumber);

            switch (MarkerType)
            {
                case HtmlMarkerType.Aw:
                case HtmlMarkerType.Html:
                {
                    modelListLevel.ListLevel.RunPr.SetSymbolFonts(mFontName);
                    break;
                }
                case HtmlMarkerType.PseudoElement:
                {
                    mFormatter.SwitchToPart(HtmlElementPart.Before, false);
                    mFormatter.ToFont(modelListLevel.ListLevel.Font, mBuilder.CurrentParagraph.ParagraphStyle);
                    mFormatter.SwitchToPart(HtmlElementPart.Element, false);

                    CssComputedDeclaration fontFamilyDeclaration =
                        (CssComputedDeclaration)mFormatter.BeforePseudoElementDeclarations["font-family"];
                    if (fontFamilyDeclaration == null)
                    {
                        // HTML uses the 'Times New Roman' font by default. 
                        modelListLevel.ListLevel.RunPr.SetSymbolFonts("Times New Roman");
                    }

                    break;
                }
                default:
                {
                    // At the moment of writing, there were no other HTML bullet types. We should never get here.
                    Debug.Fail("Unsupported HTML bullet type");
                    break;
                }
            }

            CssDeclaration fontWeightDeclaration = mFormatter.ElementDeclarations[HtmlConstants.ListLabelFontWeight];
            if (fontWeightDeclaration != null)
            {
                bool isBold = fontWeightDeclaration.Value.Equals(CssValue.Bold);
                modelListLevel.ListLevel.Font.Bold = isBold;
                modelListLevel.ListLevel.Font.BoldBi = isBold;
            }
        }

        private readonly DocumentBuilder mBuilder;
        private readonly DocumentFormatter mFormatter;
        private readonly string mFontName;
    }
}
