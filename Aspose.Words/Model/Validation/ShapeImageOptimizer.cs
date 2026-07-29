// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/08/2008 by Roman Korchagin
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Crypto;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Removes any image bytes duplication among shapes of the document.
    /// </summary>
    internal class ShapeImageOptimizer
    {
        /// <summary>
        /// Remove image duplication if necessary from all shape attributes that can store images.
        /// </summary>
        internal void RemoveImageDuplication(ShapeBase shape)
        {
            RemoveImageDuplicationCore(shape, ShapeAttr.ImageBytes);
            RemoveImageDuplicationCore(shape, ShapeAttr.FillImageBytes);
            RemoveImageDuplicationCore(shape, ShapeAttr.LineImageBytes);
        }

        /// <summary>
        /// Remove image duplication if necessary.
        /// </summary>
        /// <param name="shape">Shape object.</param>
        /// <param name="imageBytesAttr">Shape attribute for image data.
        /// Can be <see cref="ShapeAttr.ImageBytes"/>, <see cref="ShapeAttr.FillImageBytes"/> or <see cref="ShapeAttr.LineImageBytes"/>.</param>
        private void RemoveImageDuplicationCore(ShapeBase shape, int imageBytesAttr)
        {
            byte[] imageBytes = (byte[])shape.ShapePr.GetDirectAttr(imageBytesAttr);
            if (imageBytes == null)
                return;

            // If new image bytes encountered.
            if (!mImageBytesSet.Contains(imageBytes))
            {
                BytesHash imageGuid = HashUtil.GetSHA512Hash(imageBytes);
                byte[] cachedBytes = mImageGuidTable[imageGuid];

                if (cachedBytes != null)
                {
                    // It is a duplicate image, change it to the first encounered image bytes with this GUID.
                    shape.ShapePr.SetAttr(imageBytesAttr, cachedBytes);
                }
                else
                {
                    mImageBytesSet.Add(imageBytes);
                    mImageGuidTable[imageGuid] = imageBytes;
                }
            }
        }

        /// <summary>
        /// Contains byte[] objects. Allows to quickly discard the same image bytes without calculating hash.
        /// </summary>
        private readonly HashSetGeneric<byte[]> mImageBytesSet = new HashSetGeneric<byte[]>();
        /// <summary>
        /// Table to store relationship between shape GUIDs and image data .
        /// </summary>
        private readonly BytesHashToObjDictionary<byte[]> mImageGuidTable = new BytesHashToObjDictionary<byte[]>();
    }
}
