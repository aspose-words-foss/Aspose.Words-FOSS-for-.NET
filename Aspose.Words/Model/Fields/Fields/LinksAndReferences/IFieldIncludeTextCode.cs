// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/04/2017 by Edward Voronov

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    internal interface IFieldIncludeTextCode
    {
        [JavaThrows(true)]
        string SourceFullName { get; }

        [JavaThrows(true)]
        string BookmarkName { get; }

        [JavaThrows(true)]
        bool LockFields { get; }

        [JavaThrows(true)]
        string NamespaceMappings { get; }

        [JavaThrows(true)]
        string XPath { get; }

        string XslTransformation { get; }

        string TextConverter { get; }

        int SourceFullNameArgumentIndex { get; }
    }
}
