// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/05/2025 by Anton Savko

using System.IO;
using Aspose.Collections;
using Aspose.Words.Drawing;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Reader
{
    internal class HtmlPictureBulletLoader
    {
        internal HtmlPictureBulletLoader(
            Document document,
            HtmlResourceLoader resourceLoader,
            string baseUri)
        {
            Debug.Assert(document != null);
            Debug.Assert(resourceLoader != null);

            mDocument = document;
            mResourceLoader = resourceLoader;
            mBaseUri = baseUri;
            mPictureBulletIds = new ObjToIntDictionary<string>();
        }

        internal int GetPictureBulletId(string pictureBulletUri)
        {
            if (mPictureBulletIds.ContainsKey(pictureBulletUri))
            {
                return mPictureBulletIds[pictureBulletUri];
            }
            else
            {
                byte[] imageBytes = mResourceLoader.LoadImage(mBaseUri, pictureBulletUri, false);
                if (imageBytes != null)
                {
                    Shape pictureBullet = new Shape(mDocument, ShapeType.Image);
                    pictureBullet.ImageData.ImageBytes = imageBytes;
                    pictureBullet.WrapType = WrapType.Inline;
                    pictureBullet.Title = Path.GetFileNameWithoutExtension(pictureBulletUri);

                    int newPictureBulletId = mDocument.Lists.AddPictureBullet(pictureBullet);
                    mPictureBulletIds.Add(pictureBulletUri, newPictureBulletId);

                    ImageSize imageSize = pictureBullet.ImageData.ImageSize;
                    pictureBullet.SetSizeSafe(imageSize.WidthPoints, imageSize.HeightPoints);

                    return newPictureBulletId;
                }
            }

            return -1;
        }

        private readonly Document mDocument;
        private readonly HtmlResourceLoader mResourceLoader;
        private readonly string mBaseUri;
        private readonly ObjToIntDictionary<string> mPictureBulletIds;
    }
}
