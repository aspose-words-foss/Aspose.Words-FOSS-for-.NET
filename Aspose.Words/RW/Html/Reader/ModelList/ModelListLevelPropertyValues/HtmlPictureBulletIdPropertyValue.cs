// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2019 by Anton Savko

namespace Aspose.Words.RW.Html.Reader
{
    internal class HtmlPictureBulletIdPropertyValue : IHtmlModelListLevelPropertyValue
    {
        internal HtmlPictureBulletIdPropertyValue(int pictureBulletId)
        {
            Debug.Assert(pictureBulletId >= 0);

            mPictureBulletId = pictureBulletId;
        }

        bool IHtmlModelListLevelPropertyValue.CanModifyListLevel(HtmlModelListLevel modelListLevel)
        {
            return !modelListLevel.IsPictureBulletIdSet || (mPictureBulletId == modelListLevel.PictureBulletId);
        }

        void IHtmlModelListLevelPropertyValue.ModifyListLevel(HtmlModelListLevel modelListLevel)
        {
            if (!modelListLevel.IsPictureBulletIdSet)
            {
                modelListLevel.PictureBulletId = mPictureBulletId;
            }
        }

        private readonly int mPictureBulletId;
    }
}
