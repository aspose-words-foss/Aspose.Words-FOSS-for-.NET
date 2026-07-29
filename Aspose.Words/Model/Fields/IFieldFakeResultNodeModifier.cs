// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2016 by Edward Voronov

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// When implemented, used by <see cref="FieldFakeResultAppender"/> to modify cloned field fake result nodes.
    /// </summary>
    internal interface IFieldFakeResultNodeModifier
    {
        /// <summary>
        /// Modifies the specified node.
        /// </summary>
        [JavaThrows(true)]
        void Modify(Node node);
    }
}