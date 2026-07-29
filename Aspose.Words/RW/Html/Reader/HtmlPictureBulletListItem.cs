// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/06/2019 by Anton Savko

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Represents list item with a bullet. Bullet is specified by 'list-style-image' CSS property.
    /// </summary>
    internal class HtmlPictureBulletListItem : HtmlListItemBase
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="listLevelId">List level identifier.</param>
        /// <param name="pictureBulletId">Picture bullet's id.</param>
        /// <param name="pictureBulletSize">Picture bullet's size.</param>
        internal HtmlPictureBulletListItem(
            HtmlListLevelId listLevelId,
            int pictureBulletId,
            int pictureBulletSize)
            : base(listLevelId, HtmlMarkerType.Html, string.Empty)
        {
            Debug.Assert(pictureBulletId >= 0);
            Debug.Assert(pictureBulletSize >= 0);

            SetListLevelType(HtmlModelListLevelType.PictureBullet);
            SetPictureBulletId(pictureBulletId);

            mPictureBulletSize = pictureBulletSize;
        }

        internal override void PostModifyList(HtmlModelList modelList)
        {
            HtmlModelListLevel modelListLevel = modelList.GetListLevel(ListLevelId.ListLevelNumber);
            modelListLevel.ListLevel.RunPr.Size = mPictureBulletSize;
        }

        private readonly int mPictureBulletSize;
    }
}
