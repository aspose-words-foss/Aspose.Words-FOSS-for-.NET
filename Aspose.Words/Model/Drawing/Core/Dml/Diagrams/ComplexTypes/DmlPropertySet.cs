// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/01/2014 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Styles;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.3.4 prSet (Property Set)
    /// This element holds properties and customizations which are used throughout certain elements in DiagramML.
    /// </summary>
    internal class DmlPropertySet
    {
        internal DmlLayoutVariablePropertySet LayoutVariablePropertySet
        {
            get { return mLayoutVariablePropertySet; }
            set { mLayoutVariablePropertySet = value; }
        }

        internal DmlShapeStyle Style
        {
            get { return mStyle; }
            set { mStyle = value; }
        }

        internal DmlPropertySetPr PrSet
        {
            get { return mPrSet; }
        }

        private DmlLayoutVariablePropertySet mLayoutVariablePropertySet;
        private DmlShapeStyle mStyle;
        private readonly DmlPropertySetPr mPrSet = new DmlPropertySetPr();
    }
}
