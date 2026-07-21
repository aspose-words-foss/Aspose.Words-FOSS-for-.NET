// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// 20.1.8.44 noFill (No Fill)
    /// This element specifies that no fill is applied to the parent element.
    /// </summary>
    internal class DmlNoFill : DmlFill
    {
        internal override DmlFill Clone()
        {
            return new DmlNoFill();
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether a parent object is filled.
        /// </summary>
        public override bool On
        {
            get { return false; }
            set
            {
                if (value)
                    Parent.SetFill(new DmlSolidFill());
            }
        }

        /// <summary>
        /// Defines the transparency of a fill. Valid range from 0 to 1, where 0 is fully transparent and 1 is fully opaque.
        /// </summary>
        public override double Opacity
        {
            get { return 0.0; }
            set { /*Do nothing*/ }
        }

        internal override DmlFillType DmlFillType
        {
            get { return DmlFillType.NoFill; }
        }

        internal override DmlColor DmlColorInternal
        {
            get { return null; }
            set
            {
                // IN. Word VBA allows to set up color for NoFill and changes the type of the fill to Solid implicitly.
                DmlSolidFill solidFill = new DmlSolidFill(value);
                Parent.SetFill(solidFill);
            }
        }
    }
}
