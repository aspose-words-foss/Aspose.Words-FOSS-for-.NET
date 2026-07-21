// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/06/2011 by Alexey Titov

using Aspose.Common;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Represents a data source for image of a VML-based shape.
    /// </summary>
    internal class ImageDataSource : IImageDataSource
    {
        internal ImageDataSource(IShapeAttrSource parent)
        {
            mParent = parent;
        }

        string IImageDataSource.SourceFullName
        {
            get { return (string)mParent.FetchShapeAttr(ShapeAttr.ImageSourceFullName); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mParent.SetShapeAttr(ShapeAttr.ImageSourceFullName, value);
            }
        }

        byte[] IImageDataSource.ImageBytes
        {
            get 
            {
                byte[] imageBytes = (byte[])mParent.FetchShapeAttr(ShapeAttr.ImageBytes);
                return CompressedData.GetData(imageBytes);
            }
            set
            {
                if (value == null || value.Length == 0)
                    mParent.RemoveShapeAttr(ShapeAttr.ImageBytes);
                else
                    mParent.SetShapeAttr(ShapeAttr.ImageBytes, value);
            }
        }

        bool IImageDataSource.HasImageBytes
        {
            get { return ArrayUtil.HasData((byte[])mParent.FetchShapeAttr(ShapeAttr.ImageBytes)); }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IShapeAttrSource mParent;
    }
}
