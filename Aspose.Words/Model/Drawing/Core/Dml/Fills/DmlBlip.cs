// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

using System.Collections.Generic;
using Aspose.Common;
using Aspose.Images;
using Aspose.Words.Drawing.Core.Dml.Effects;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// 20.1.8.13 blip (Blip)
    /// This element specifies the existence of an image (binary large image or picture) 
    /// and contains a reference to the image data.
    /// </summary>
    internal class DmlBlip : DmlExtensionListSource
    {
        public DmlBlip Clone()
        {
            DmlBlip lhs = (DmlBlip)MemberwiseClone();
            if (mEffects != null)
            {
                lhs.mEffects = new List<DmlEffect>();
                foreach (DmlEffect effect in mEffects)
                    lhs.mEffects.Add(effect.Clone());
            }
            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlBlip value = (DmlBlip)obj;

            return (value.CompressionState == CompressionState) &&
                   object.Equals(value.ImageLink, ImageLink) &&
                   object.Equals(value.EmbedImage, EmbedImage) &&
                   HasSameEffects(value);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= CompressionState.GetHashCode();
            if (StringUtil.HasChars(ImageLink))
                hash ^= ImageLink.GetHashCode();
            if (HasEmbedImage)
                hash ^= EmbedImage.GetHashCode();

            foreach (DmlEffect effect in Effects)
                hash ^= effect.GetHashCode();

            return hash;
        }

        private bool HasSameEffects(DmlBlip value)
        {
            if (value.Effects.Count != Effects.Count)
                return false;

            bool hasSameEffects = true;
            for (int i = 0; i < Effects.Count; i++)
            {
                DmlEffect e1 = Effects[i];
                DmlEffect e2 = value.Effects[i];

                hasSameEffects &= object.Equals(e1, e2);
                if(!hasSameEffects)
                    break;
            }

            return hasSameEffects;
        }

        internal FileFormat GetImageType()
        {
            if (HasEmbedImage)
                return ImageUtil.GetImageType(mEmbedImage);

            return ImageUtil.GetImageType(GetImageBytes());
        }

        internal byte[] GetImageBytes()
        {            
            if (mEmbedImage != null)
                return CompressedData.GetData(mEmbedImage);

            if ((mCachedImage == null) && StringUtil.HasChars(mImageLink))
                mCachedImage = ImageDataUtil.LoadImageBytes(ImageLink, mDocument);

            return mCachedImage;
        }

        internal bool HasEmbedImage 
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return ArrayUtil.HasData(mEmbedImage); }
        }

        internal byte[] EmbedImage
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return CompressedData.GetData(mEmbedImage); }
            set { mEmbedImage = value; }
        }

        internal string ImageLink
        {
            get { return mImageLink; }
            set { mImageLink = value; }
        }

        /// <summary>
        /// Specifies the compression state with which the picture is stored. 
        /// This allows the application to specify the amount of compression 
        /// that has been applied to a picture.
        /// </summary>
        internal DmlCompressionState CompressionState
        {
            get { return mCompressionState; }
            set { mCompressionState = value; }
        }

        internal IList<DmlEffect> Effects
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mEffects == null)
                    mEffects = new List<DmlEffect>();
                return mEffects;
            }
            set { mEffects = value; }
        }

        internal DocumentBase Document
        {
            get { return mDocument; }
            set { mDocument = value; }
        }

        private DmlCompressionState mCompressionState;
        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private IList<DmlEffect> mEffects;
        private byte[] mCachedImage;
        private string mImageLink;
        private byte[] mEmbedImage;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DocumentBase mDocument;
    }
}
