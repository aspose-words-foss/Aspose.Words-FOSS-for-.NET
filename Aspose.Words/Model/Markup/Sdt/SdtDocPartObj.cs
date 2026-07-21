// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2010 by Denis Darkin

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Specifies that the parent structured document tag shall be of a document part type.
    /// </summary>
    /// <remarks>
    /// This element differs from the <see cref="SdtBuildingBlockGallery"/> element in that it can be used to semantically tag a set of 
    /// block-level objects in a WordprocessingML document without requiring the ability to specify a category and 
    /// gallery of objects which can be swapped with it via the user interface.
    /// </remarks>
    internal class SdtDocPartObj : SdtDocPart
    {
        internal override SdtType Type
        {
            get { return SdtType.DocPartObj; }
        }
    }
}
