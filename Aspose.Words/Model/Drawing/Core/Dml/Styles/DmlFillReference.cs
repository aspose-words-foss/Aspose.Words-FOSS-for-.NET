// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/02/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Styles
{
    /// <summary>
    /// 20.1.4.2.10 fillRef (Fill Reference)
    /// This element defines a reference to a fill style within the style matrix.
    /// </summary>
    internal class DmlFillReference : DmlStyleReferenceBase
    {
        /// <summary>
        /// Specifies the style matrix index of the style referred to.
        /// The content is a restriction of the W3C XML Schema unsignedInt datatype.
        /// A value of 0 or 1000 indicates no background,
        /// values 1-999 refer to the index of a fill style within the fillStyleLst element,
        /// and values 1001 and above refer to the index of a background fill
        /// style within the bgFillStyleLst element. The value 1001 corresponds to the
        /// first background fill style, 1002 to the second background fill style, and so on.
        /// </summary>
        internal int StyleMatrixIndex
        {
            get { return mStyleMatrixIndex; }
            set { mStyleMatrixIndex = value; }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int IndexOfFirstBackgroundFillStyle = 1001;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int IndexOfFirstFillStyle = 1;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int IndexOfLastFillStyle = 999;

        private int mStyleMatrixIndex;
    }
}
