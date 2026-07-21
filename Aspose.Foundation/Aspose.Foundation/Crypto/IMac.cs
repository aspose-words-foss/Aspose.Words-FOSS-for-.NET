// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/04/2016 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Crypto
{
    /// <summary>
    /// The base interface for implementations of message authentication codes (MACs).
    /// </summary>
    public interface IMac
    {
        /// <summary>
        /// Initializes underlying MAC with the specified key.
        /// </summary>
        void Init([CppArgumentKind(ArgumentKind.ConstReference)] byte[] key);

        /// <summary>
        /// Computes hash for the specified content.
        /// </summary>
        byte[] ComputeHash([CppArgumentKind(ArgumentKind.ConstReference)] byte[] content);

        /// <summary>
        /// Gets MAC size.
        /// </summary>
        int Size { get; }
    }
}
