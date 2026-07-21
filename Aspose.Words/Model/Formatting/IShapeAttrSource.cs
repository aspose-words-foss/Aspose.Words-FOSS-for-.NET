// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/07/2005 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// For more info see <see cref="IRunAttrSource"/>.
    /// </summary>
    internal interface IShapeAttrSource
    {
        object GetDirectShapeAttr(int key);

        object FetchInheritedShapeAttr(int key);

        object FetchShapeAttr(int key);

        void SetShapeAttr(int key, object value);

        void RemoveShapeAttr(int key);
    }
}
