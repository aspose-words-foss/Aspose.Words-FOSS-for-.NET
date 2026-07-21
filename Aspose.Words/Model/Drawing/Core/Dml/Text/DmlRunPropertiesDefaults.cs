// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using System;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Outlines;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    internal class DmlRunPropertiesDefaults : DmlHierarchicalPropertyBag
    {
        private DmlRunPropertiesDefaults()
        {
            SetProperty((int)DmlRunPropertiesIds.AlternativeLanguage, Language.LanguageNotSet);
            SetProperty((int)DmlRunPropertiesIds.Bold, false);
            SetProperty((int)DmlRunPropertiesIds.Baseline, 0.0);
            SetProperty((int)DmlRunPropertiesIds.BookmarkLinkTarget, String.Empty);
            SetProperty((int)DmlRunPropertiesIds.Capitalization, DmlCapitalization.None);
            SetProperty((int)DmlRunPropertiesIds.IsDirty, false);
            SetProperty((int)DmlRunPropertiesIds.HasSpellingError, false);
            SetProperty((int)DmlRunPropertiesIds.Italics, false);
            SetProperty((int)DmlRunPropertiesIds.Kerning, new DmlTextPoints());
            SetProperty((int)DmlRunPropertiesIds.Kumimoji, false);
            SetProperty((int)DmlRunPropertiesIds.Language, Language.LanguageNotSet);
            SetProperty((int)DmlRunPropertiesIds.NoProofing, false);
            SetProperty((int)DmlRunPropertiesIds.NormalizeHeights, false);

            // In Office, the smtClean attribute of the rPr, defRpr and endParaRPr elements has a default value of true.
            SetProperty((int)DmlRunPropertiesIds.SmartTagsClean, true);
            SetProperty((int)DmlRunPropertiesIds.SmartTagID, int.MinValue);
            SetProperty((int)DmlRunPropertiesIds.Spacing, new DmlTextPoints());
            SetProperty((int)DmlRunPropertiesIds.Strikethrough, DmlTextStrike.No);

            // Office uses a default value of 1800 for this attribute.
            SetProperty((int)DmlRunPropertiesIds.FontSize, new DmlTextPoints(1800));
            SetProperty((int)DmlRunPropertiesIds.Underline, Underline.None);
            SetProperty((int)DmlRunPropertiesIds.Fill, null);
            SetProperty((int)DmlRunPropertiesIds.Outline, new DmlOutline());
            SetProperty((int)DmlRunPropertiesIds.HighlightColor, null);
            SetProperty((int)DmlRunPropertiesIds.LatinFont, null);
            SetProperty((int)DmlRunPropertiesIds.EastAsianFont, null);
            SetProperty((int)DmlRunPropertiesIds.SymbolFont, null);
            SetProperty((int)DmlRunPropertiesIds.ComplexScriptFont, null);
            SetProperty((int)DmlRunPropertiesIds.HlinkClick, null);
            SetProperty((int)DmlRunPropertiesIds.HlinkHover, null);

            SetProperty((int)DmlRunPropertiesIds.UnderlineFill, null);
            SetProperty((int)DmlRunPropertiesIds.UnderlineFillTx, false);
            SetProperty((int)DmlRunPropertiesIds.UnderlineStroke, new DmlOutline());
            SetProperty((int)DmlRunPropertiesIds.UnderlineStrokeTx, false);
            SetProperty((int)DmlRunPropertiesIds.RightToLeftFlowDirection, false);

            SetProperty((int)DmlRunPropertiesIds.Extensions, null);
        }

        internal static DmlRunPropertiesDefaults Instance
        {
            get { return gInstance; }
        }

        private static readonly DmlRunPropertiesDefaults gInstance = new DmlRunPropertiesDefaults();
    }
}
