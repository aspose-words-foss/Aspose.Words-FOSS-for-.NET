// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/08/2007 by Vladimir Averkin

using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Writes 'v:imagedata' sub-element of 'v:shape' to WordML.
    /// </summary>
    internal class VmlImagedataWriter
    {
        internal VmlImagedataWriter(ShapeBase shape, NrxXmlBuilder builder, IVmlShapeWriterContext context)
        {
            Debug.Assert(null != context);
            mBuilder = builder;
            mShape = shape;
            mContext = context;
        }

        /// <summary>
        /// Add 'v:imagedata' related attribute.
        /// </summary>
        /// <param name="key">Attribute key.</param>
        /// <param name="value">Attribute value.</param>
        internal void AddAttribute(int key, object value)
        {
            switch (key)
            {
                case ShapeAttr.ImageActive:
                    {
                        break;
                    }
                case ShapeAttr.ImageBiLevel:
                    {
                        mImageBiLevel = VmlUtil.BoolToVml(value, false);
                        break;
                    }
                case ShapeAttr.ImageBrightness:
                    {
                        mImageBrightness = VmlUtil.FixedToVml(value);
                        break;
                    }
                case ShapeAttr.ImageBytes:
                    {
                        mImageBytes = (byte[])value;
                        break;
                    }
                case ShapeAttr.ImageContrast:
                    {
                        mImageContrast = VmlUtil.FixedToVml(value);
                        break;
                    }
                case ShapeAttr.ImageCropBottom:
                    {
                        mImageCropBottom = VmlUtil.FixedToVml(value);
                        break;
                    }
                case ShapeAttr.ImageCropLeft:
                    {
                        mImageCropLeft = VmlUtil.FixedToVml(value);
                        break;
                    }
                case ShapeAttr.ImageCropRight:
                    {
                        mImageCropRight = VmlUtil.FixedToVml(value);
                        break;
                    }
                case ShapeAttr.ImageCropTop:
                    {
                        mImageCropTop = VmlUtil.FixedToVml(value);
                        break;
                    }
                case ShapeAttr.ImageDblCrMod:
                    {
                        mImageDblCrMod = mContext.ColorToVml((DrColor)value);
                        break;
                    }
                case ShapeAttr.ImageFillCrMod:
                    {
                        break;
                    }
                case ShapeAttr.ImageGamma:
                    {
                        mImageGamma = VmlUtil.FixedToVml(value);
                        break;
                    }
                case ShapeAttr.ImageGrayScale:
                    {
                        mImageGrayScale = VmlUtil.BoolToVml(value, false);
                        break;
                    }
                case ShapeAttr.ImageLineCrMod:
                    {
                        break;
                    }
                case ShapeAttr.ImageNoHitTest:
                    {
                        break;
                    }
                case ShapeAttr.ImagePreserveGrays:
                    {
                        break;
                    }
                case ShapeAttr.ImageRecolor:
                    {
                        DrColor color = new DrColor((int)value);
                        mImageRecolor = VmlColor.ColorToVml(new DrColor(color.B, color.G, color.R));
                        break;
                    }
                case ShapeAttr.ImageRecolorExt:
                    {
                        break;
                    }
                case ShapeAttr.ImageRecolorExtCrMod:
                    {
                        break;
                    }
                case ShapeAttr.ImageSourceFullName:
                    {
                        mImageSourceFullName = (string)value;
                        break;
                    }
                case ShapeAttr.ImageTitle:
                    {
                        mImageTitle = (string)value;
                        break;
                    }
                case ShapeAttr.ImageTransparent:
                    {
                        mImageTransparent = mContext.ColorToVml((DrColor)value);
                        break;
                    }
                default:
                    return;
            }
        }

        /// <summary>
        /// Write 'v:imagedata' element.
        /// </summary>
        internal void Write()
        {
            // andrnosk: WORDSNET-7968 If ImageRecolor (recolortarget) is specified we should write imagedata.
            if ((mImageBytes == null) && (mImageSourceFullName == null) && (mImageDblCrMod == null) && (mImageRecolor == null))
                return;

            mBuilder.StartElement("v:imagedata");

            // If the image is linked, then, if image binary data is defined inside document,
            // the 'src' should be the internal reference to the binary data
            // and the file reference should be put inside 'o:href',
            // otherwise the file reference is put directly in 'src'.
            mBuilder.WriteAttribute(mContext.ImageSrcAttributeName, mImageName);

            if ((mImageBytes != null) && (mImageSourceFullName != mImageName))
                mBuilder.WriteAttribute(mContext.ImageHrefAttributeName, mImageSourceFullName);

            // RK This has to be written even if empty string. If not written, then in some cases
            // either MS Word fails to open a document or double click on an inline OLE object does not work
            // or a picture does not show when document is downloaded in Internet Explorer via URL.
            // WORDSNET-13261 If image is not embedded and stored with the path and name of the source file
            // then after modify and re-saving document from MS Word image became embedded. Documentation says: 
            // “If title attribute has a value, then the image is embedded”, so we need skip "title" for linked image
            // when "title" is not set (https://msdn.microsoft.com/en-us/library/bb229579(v=vs.85).aspx).
            if (CheckAddTitle(mImageTitle))
                mBuilder.WriteAttributeString("o:title", mImageTitle);

            mBuilder.WriteAttribute("croptop", mImageCropTop);
            mBuilder.WriteAttribute("cropbottom", mImageCropBottom);
            mBuilder.WriteAttribute("cropleft", mImageCropLeft);
            mBuilder.WriteAttribute("cropright", mImageCropRight);
            mBuilder.WriteAttribute("chromakey", mImageTransparent);
            mBuilder.WriteAttribute("gain", mImageContrast);
            mBuilder.WriteAttribute("blacklevel", mImageBrightness);
            mBuilder.WriteAttribute("gamma", mImageGamma);
            mBuilder.WriteAttribute("grayscale", mImageGrayScale);
            mBuilder.WriteAttribute("bilevel", mImageBiLevel);
            mBuilder.WriteAttribute("embosscolor", mImageDblCrMod);
            mBuilder.WriteAttribute("recolortarget", mImageRecolor);

            mBuilder.EndElement(); //v:imagedata
        }

        /// <summary>
        /// Checks whether it is required to add title attribute.
        /// </summary>
        /// <returns>Is True when "Title" attribute have to be added.</returns>
        private bool CheckAddTitle(string title)
        {
            // When current node is inside complex element, it's necessary to write title for properly show image.
            // When title sets implicitly we have to write it.
            if (!(mShape is Shape) || (null != title) || mContext.IsInsideField)
                return true;           

            Shape s = (Shape)mShape;                   
            ImageData imgData = s.ImageData;

            // Skip title only for linked image.
            return (imgData == null) || (s.ShapeType != ShapeType.Image) || !imgData.IsLinkOnly;
        }

        internal byte[] ImageBytes
        {
            get { return mImageBytes; }
        }

        internal string ImageName
        {
            get { return mImageName; }
            set { mImageName = value; }
        }

        internal string ImageSourceFullName
        {
            get { return mImageSourceFullName; }
            set { mImageSourceFullName = value; }
        }

        private readonly NrxXmlBuilder mBuilder;
        private readonly ShapeBase mShape;
        private readonly IVmlShapeWriterContext mContext;

        private byte[] mImageBytes;
        private string mImageName = null;
        private string mImageTitle = null;
        private string mImageSourceFullName = null;

        private string mImageCropTop = null;
        private string mImageCropBottom = null;
        private string mImageCropLeft = null;
        private string mImageCropRight = null;
        private string mImageTransparent = null;
        private string mImageContrast = null;
        private string mImageBrightness = null;
        private string mImageGamma = null;
        private string mImageDblCrMod = null;
        private string mImageGrayScale = null;
        private string mImageBiLevel = null;
        private string mImageRecolor = null;

        //id    
        //src    Yes
        //o:title    Yes
        //croptop    Yes
        //cropbottom    Yes
        //cropleft    Yes
        //cropright    Yes
        //chromakey    Yes
        //gain    Yes
        //blacklevel    Yes
        //gamma    Yes
        //grayscale    Yes
        //bilevel    Yes
        //embosscolor    Yes
        //o:href    Yes
        //o:althref    
        //o:oleid    
        //o:detectmouseclick    
        //o:movie    
    }
}
