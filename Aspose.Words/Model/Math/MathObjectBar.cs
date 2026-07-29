// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the bar function, consisting of a base argument and an overbar or underbar.
    /// </summary>
    internal class MathObjectBar : MathObject
    {
        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.Bar; }
        }

        /// <summary>
        /// Specifies the position of the bar in the parent object.
        /// Default: <see cref="MathPosition.Default"/>
        /// </summary>
        internal MathPosition Position
        {
            get { return (MathPosition)FetchAttr(MathAttr.Position); }
            set { SetAttr(MathAttr.Position, value, value != MathPosition.Default); }
        }
    }
}
