// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2010 by Denis Darkin
namespace Aspose.Words.Markup
{
    /// <summary>
    /// Specifies that the parent sdt shall be of type equation.
    /// </summary>
    internal class SdtEquation : SdtControlProperties
    {
        internal override SdtType Type
        {
            get { return SdtType.Equation; }
        }
    }
}
