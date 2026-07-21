// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/07/2005 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// For more info see <see cref="IRunAttrSource"/>.
    /// </summary>
    internal interface IRowAttrSource
    {
        object GetDirectRowAttr(int key);

        object FetchRowAttr(int key);

        object FetchInheritedRowAttr(int key);

        void SetRowAttr(int key, object value);

        void ClearRowAttrs();

        /// <summary>
        /// Resets to default attributes values.
        /// </summary>
        /// <remarks>
        /// This is different from clearing the attributes as default attributes may have values.
        /// </remarks>
        [JavaThrows(true)]
        void ResetToDefaultAttrs();
    }
}
