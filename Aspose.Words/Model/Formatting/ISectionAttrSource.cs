// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/07/2005 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// For more info see <see cref="IRunAttrSource"/>.
    /// </summary>
    internal interface ISectionAttrSource
    {
        object GetDirectSectionAttr(int key);

        object GetDirectSectionAttr(int key, RevisionsView revisionsView);

        object FetchInheritedSectionAttr(int key);

        object FetchSectionAttr(int key);

        object FetchSectionAttr(int key, RevisionsView revisionsView);

        void SetSectionAttr(int key, object value);

        void SetSectionAttr(int key, object value, RevisionsView revisionsView);

        void ClearSectionAttrs();
    }
}
