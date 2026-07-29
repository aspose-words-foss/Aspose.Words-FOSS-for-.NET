// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// 20.1.8.35 grpFill (Group Fill)
    /// This element specifies a group fill. When specified, this setting indicates that the parent element is part
    /// of a group and should inherit the fill properties of the group.
    /// </summary>
    internal class DmlGroupFill : DmlFill
    {
        /// <summary>
        /// Gets or sets foreground color of the fill.
        /// </summary>
        internal override DmlColor DmlColorInternal
        {
            get { return null; }
            set
            {
                // Word VBA allows to set up color for GroupFill and changes the type of the fill to Solid implicitly.
                DmlSolidFill solidFill = new DmlSolidFill(value);
                Parent.SetFill(solidFill);
            }
        }

        internal override DmlFill Clone()
        {
            return new DmlGroupFill();
        }

        internal override DmlFillType DmlFillType
        {
            get { return DmlFillType.GroupFill; }
        }

        /// <summary>
        /// Gets or sets fill color opacity.
        /// </summary>
        public override double Opacity
        {
            get { return 0.0; }
            set
            {
                /*Do nothing*/
            }
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
    }
}
