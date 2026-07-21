// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2010 by Denis Darkin

namespace Aspose.Words.Markup
{

    /// <summary>
    /// Common ancestor for <see cref="SdtBuildingBlockGallery"/> and <see cref="SdtDocPartObj"/>.
    /// </summary>
    internal abstract class SdtDocPart : SdtControlProperties
    {

        internal override SdtControlProperties Clone()
        {
            return (SdtDocPart)MemberwiseClone();
        }

        /// <summary>
        /// Specifies the category of document parts which shall be used as the filter when determining the
        /// possible choices of document parts which are displayed for insertion into the sdt. 
        /// </summary>
        /// <remarks>
        /// A document part category is a sub-classification within a given document part gallery which can be used to further
        /// categorize the parts in a given gallery.
        /// </remarks>
        internal string BuildingBlockCategory
        {
            get { return mBuildingBlockCategory; }
            set { mBuildingBlockCategory = value; }
        }

        /// <summary>
        /// specifies the gallery of document parts which shall be used as the filter when determining the possible 
        /// choices of document parts which are displayed for insertion into the parent structured document tag.
        /// </summary>
        /// <remarks>
        /// A document part gallery is a classification of document parts, which might then be subdivided into categories.
        /// </remarks>
        internal string BuildingBlockType
        {
            get { return mBuildingBlockType; }
            set { mBuildingBlockType = value; }
        }

        /// <summary>
        /// Specifies that this structured document tag is being used to encapsulate a built-in document part.
        /// </summary>
        internal bool IsUnique
        {
            get { return mIsUnique; }
            set { mIsUnique = value; }
        }

        /// <summary>
        /// See Iso29500, chapter 1, 17.5.2.14 docPartUnique (Built-In Document Part).
        /// </summary>
        private bool mIsUnique;

        /// <summary>
        /// See Iso29500, chapter 1, 17.5.2.11 docPartGallery (Document Part Gallery Filter)
        /// </summary>
        private string mBuildingBlockType = "";

        /// <summary>
        /// See Iso29500, chapter 1, 17.5.2.10 docPartCategory (Document Part Category Filter)
        /// </summary>
        private string mBuildingBlockCategory = "";
    }
}
