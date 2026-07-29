// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2016 by Edward Voronov

using Aspose.JavaAttributes;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides result for the <see cref="FieldInfo"/> field.
    /// </summary>
    internal interface IFieldInfoResultProvider
    {
        [JavaThrows(true)]
        Constant GetResult(Document document, IFieldCode fieldCode);
    }
}