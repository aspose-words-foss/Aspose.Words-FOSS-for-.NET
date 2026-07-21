// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/11/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a source of generated content for pseudo-elements.
    /// </summary>
    internal abstract class PseudoElementContentPart
    {
        /// <summary>
        /// Accepts visitor.
        /// </summary>
        internal abstract void Accept(IPseudoElementContentPartVisitor visitor);
    }
}
