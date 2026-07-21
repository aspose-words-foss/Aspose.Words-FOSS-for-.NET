// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/08/2019 by Edward Voronov

using Aspose.Words.Fields;

namespace Aspose.Words
{
    /// <summary>
    /// When implemented, used by <see cref="FieldRemover"/> to determine
    /// wherever certain field should be replaced with result or not.
    /// </summary>
    internal interface IFieldRemoverFilter
    {
        bool NeedReplaceFieldWithResult(FieldChar fieldChar);
    }
}
