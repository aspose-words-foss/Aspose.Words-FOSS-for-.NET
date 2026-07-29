// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/02/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Styles
{
    /// <summary>
    /// 20.1.4.2.8 effectRef (Effect Reference)
    /// This element defines a reference to an effect style within the style matrix. 
    /// </summary>
    internal class DmlEffectReference : DmlStyleReferenceBase
    {
        /// <summary>
        /// Specifies the style matrix index of the style referred to.
        /// The content is a restriction of the W3C XML Schema unsignedInt datatype.
        /// </summary>
        internal int StyleMatrixIndex
        {
            get { return mStyleMatrixIndex; }
            set { mStyleMatrixIndex = value; }
        }

        private int mStyleMatrixIndex;

        public EffectStyle GetEffectStyle(IThemeProvider theme)
        {
            // Zero index disables effects.
            if ((StyleMatrixIndex == 0) || (theme == null))
                return null;

            // In theme effect references have zero based indexes, but in document one based. 
            return theme.GetEffectStyle(StyleMatrixIndex - 1);
        }
    }
}
