// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2010 by Roman Korchagin

namespace Aspose.Words
{
    internal class AssemblyConstants
    {
        /// <summary>
        /// No ctor.
        /// </summary>
        private AssemblyConstants() { }

        internal const string Family = "Aspose.Words";

        internal const string Platform = ".NET";

        /// <summary>
        /// The full product name.
        /// </summary>
        internal const string Product = Family + " for " + Platform;

        /// <summary>
        /// The version of the assembly.
        /// Normal planned releases: yy.m (e.g. 17.1, 17.2, ... 17.12)
        /// Hot-fix releases: yy.m.n (e.g. 17.1.n where ‘n’ can be 1-9)
        /// Leading zero removed from minor version.
        /// </summary>
        internal const string Version = "26.2.0";

        /// <summary>
        /// Embed this string into all produced documents.
        /// </summary>
        internal const string GeneratorName = Product + " " + Version;
    }
}
