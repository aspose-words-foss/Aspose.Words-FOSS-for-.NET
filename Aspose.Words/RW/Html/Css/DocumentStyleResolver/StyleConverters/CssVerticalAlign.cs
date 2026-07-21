// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2015 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Stores information about sup or sub tags and other tags inside.
    /// </summary>
    internal class CssVerticalAlign
    {
        internal CssVerticalAlign(CssDeclarationCollection declarations)
        {
            CssDeclaration verticalAlignDeclaration = declarations["vertical-align"];
            if (verticalAlignDeclaration != null)
            {
                mVerticalAlignValue = verticalAlignDeclaration.Value;
            }
        }

        internal CssVerticalAlign(CssPropertyValue verticalAlignValue)
        {
            mVerticalAlignValue = verticalAlignValue;
        }

        internal CssPropertyValue VerticalAlignValue
        {
            get { return mVerticalAlignValue; }
        }

        private readonly CssPropertyValue mVerticalAlignValue;
    }
}