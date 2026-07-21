// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/01/2023 by Alexander Zhiltsov

namespace Aspose.Words
{
    /// <summary>
    /// Intended for complex attributes, which comparing behaviour should be different than behaviour of the
    /// <see cref="object.Equals(object)"/> method.
    /// </summary>
    /// <remarks>
    /// <see cref="FontAttr.FarEastLayout"/> and <see cref="FontAttr.FitText"/> attributes have an ID property that
    /// should not be included in value comparision when an attribute collection is collapsed.
    /// </remarks>
    internal interface ICustomEquality
    {
        /// <summary>
        /// Returns <b>true</b> if this attribute and the specified one have the same value.
        /// </summary>
        bool HasSameValue(ICustomEquality attr);
    }
}
