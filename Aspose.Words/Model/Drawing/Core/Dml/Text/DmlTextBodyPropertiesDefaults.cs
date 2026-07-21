// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2013 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    internal class DmlTextBodyPropertiesDefaults : DmlHierarchicalPropertyBag
    {
        private DmlTextBodyPropertiesDefaults()
        {
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.Anchor, DmlTextAnchoringType.Top);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.AnchorCenter, false);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.AreFirstAndLastParagraphsUseSpacing, false);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.ColumnNumber, 1);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.ColumnOrder, DmlTextColumnOrder.LeftToRight);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.ForceAntiAlias, false);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.FromWordArt, false);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.IsTextUpright, false);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.Rotation, new DmlAngle());
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.HasDefaultRotation, false);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.SpaceBetweenColumns, 0);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.TextHorizontalOverflow, DmlTextHorizontalOverflowType.Overflow);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.TextVerticalOverflow, DmlTextVerticalOverflowType.Overflow);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.TextOrientation, ShapeTextOrientation.Horizontal);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.TextWrappingType, TextBoxWrapMode.Square);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.UseCompatibleLineSpacing, false);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.LeftInset, LeftInset);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.TopInset, TopInset);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.RightInset, RightInset);
            SetProperty((int)DmlTextBodyPropertiesDefaultsIds.BottomInset, BottomInset);
        }

        internal static DmlTextBodyPropertiesDefaults Instance
        {
            get { return gInstance; }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int LeftInset = 91440;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int TopInset = 45720;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int RightInset = 91440;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int BottomInset = 45720;
      
        private static readonly DmlTextBodyPropertiesDefaults gInstance = new DmlTextBodyPropertiesDefaults();
    }
}
