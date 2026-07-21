// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2023 by Edward Voronov

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Bibliography
{
    /// <summary>
    /// Represents a bibliography source contributor. Can be either an corporate (an organization) or a list of persons.
    /// </summary>
    [CppVirtualInheritance("System.Object")]
    public abstract class Contributor
    {
        internal Contributor()
        {
        }

        internal abstract Contributor Clone();
    }
}
