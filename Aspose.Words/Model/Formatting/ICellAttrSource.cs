// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/07/2005 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// For more info see <see cref="IRunAttrSource"/>.
    /// </summary>
    internal interface ICellAttrSource
    {
        object GetDirectCellAttr(int key);

        object FetchCellAttr(int key);

        object FetchInheritedCellAttr(int key);

        void SetCellAttr(int key, object value);

        void ClearCellAttrs();
    }
}
