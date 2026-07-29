// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/07/2005 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// For more info see <see cref="IRunAttrSource"/>.
    /// </summary>
    internal interface IParaAttrSource
    {
        object GetDirectParaAttr(int key);

        object GetDirectParaAttr(int key, RevisionsView revisionsView);

        object FetchInheritedParaAttr(int key);

        object FetchParaAttr(int key);
        
        void SetParaAttr(int key, object value);

        void RemoveParaAttr(int key);

        void ClearParaAttrs();
    }
}
