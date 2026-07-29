// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2020 by Edward Voronov

namespace Aspose.Common
{
    /// <summary>Provides common functionality related to <see cref="IChar"/>, <see cref="IString"/> and <see cref="IStringBuilder"/>.</summary>
    public interface IStringBehaviour
    {
        /// <summary>Returns the empty <see cref="IString"/> string.</summary>
        IString EmptyString { get; }

        /// <summary>Indicates whether the specified string is null or an empty string.</summary>
        bool IsNullOrEmpty(IString value);

        /// <summary>Initializes a new instance of the <see cref="IString" /> class.</summary>
        IString CreateString(string value);

        /// <summary>Initializes a new instance of the <see cref="IStringBuilder" /> class.</summary>
        IStringBuilder CreateBuilder();

        /// <summary>Initializes a new instance of the <see cref="IStringBuilder" /> class.</summary>
        IStringBuilder CreateBuilder(IString value);

        /// <summary>Initializes a new instance of the <see cref="IStringBuilder" /> class.</summary>
        IStringBuilder CreateBuilder(int capacity);
    }
}
